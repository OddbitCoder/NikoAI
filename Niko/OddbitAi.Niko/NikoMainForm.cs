#pragma warning disable CA1416

using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing.Imaging;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using OddbitAi.Models;
using Emgu.CV;
using OddbitAi.Niko.Cogs;
using OddbitAi.Models.Dto;

namespace OddbitAi.Niko
{
    public partial class NikoMainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        // audio

        private const int sampleRate = 16000;
        private const int bitsPerSample = 16;
        private const int channels = 1;

        private readonly WaveInEvent waveIn = new()
        {
            BufferMilliseconds = 250,
            NumberOfBuffers = 2,
            //DeviceNumber = 0,
            WaveFormat = new WaveFormat(sampleRate, bitsPerSample, channels)
        };
        private AudioBuffer? buffer;
        private bool closing
            = false;
        private readonly TimeSpan snapshotTimeStep
            = TimeSpan.FromSeconds(/*N=*/2); // make a snapshot every N seconds
        private readonly TimeSpan yoloFrameTimeStep
            = TimeSpan.FromSeconds(/*N=*/0); // process video frame every N seconds
        private readonly TimeSpan dfFrameTimeStep
            = TimeSpan.FromSeconds(/*N=*/0); // process video frame every N seconds
        private readonly TextBuffer textBuffer
            = new(TimeSpan.FromSeconds(/*N=*/1)); // trim audio buffer N seconds on each side
        private DateTime? lastSnapshotTimestamp
            = null;
        private DateTime? lastYoloFrameTimestamp
            = DateTime.UtcNow;
        private DateTime? lastDfFrameTimestamp
            = DateTime.UtcNow;
        private ModelService.ModelServiceClient? whisperClient;
        private ModelService.ModelServiceClient? yoloClient;
        private ModelService.ModelServiceClient? dfClient;
        private string? outputFolder;
        private Task? whisperCallTask
            = null;
        private Task? yoloCallTask
            = null;
        private Task? dfCallTask
            = null;
        private const double noSpeechProbThresh
            = 0.3;

        // state machine

        private readonly StateMachine stateMachine
            = new();

        // cam video

        private readonly VideoCapture capture
            = new();

        public NikoMainForm()
        {
            InitializeComponent();
            // camera video overlay
            pbVideo.Controls.Add(videoOverlay);
            videoOverlay.FrameSize = new Size(capture.Width, capture.Height);
        }

        private void ProcessVideoFrame(object? sender, EventArgs e)
        {
            void yoloCall(byte[] frameBytes)
            {
                try
                {
                    var reply = yoloClient!.Process(new ProcessRequest { Data = ByteString.CopyFrom(frameBytes) });
                    var replyObj = JsonSerializer.Deserialize<VisionModelResponseDto>(reply.Reply, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    var detectedObjects = replyObj?.Objects ?? Array.Empty<DetectedObjectDto>();
                    Invoke(() => videoOverlay.UpdateObjectAnnotations(detectedObjects));
                    stateMachine.AddDetectedObjects(detectedObjects);
                    //Console.WriteLine($"\"{replyObj?.Summary}\"");
                    ledYolo.Status = true;
                }
                catch (Exception)
                {
                    Invoke(() =>
                    {
                        ledYolo.Status = false;
                        videoOverlay.UpdateObjectAnnotations(Array.Empty<DetectedObjectDto>());
                    });
                    Thread.Sleep(1000);
                }
            }

            void dfCall(byte[] frameBytes)
            {
                try
                {
                    var reply = dfClient!.Process(new ProcessRequest { Data = ByteString.CopyFrom(frameBytes) });
                    var replyObj = JsonSerializer.Deserialize<VisionModelResponseDto>(reply.Reply, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    var detectedFaces = replyObj?.Objects?.Where(x => x.Verified) ?? Array.Empty<DetectedObjectDto>();
                    Invoke(() => videoOverlay.UpdateFaceAnnotations(detectedFaces));
                    stateMachine.AddDetectedFaces(detectedFaces);
                    ledDeepface.Status = true;
                }
                catch (Exception)
                {
                    Invoke(() =>
                    {
                        ledDeepface.Status = false;
                        videoOverlay.UpdateFaceAnnotations(Array.Empty<DetectedObjectDto>());
                    });
                    Thread.Sleep(1000);
                }
            }

            var frame = new Mat();
            capture.Retrieve(frame);

            if (!frame.IsEmpty)
            {
                pbVideo.Image = frame.ToBitmap();
                var frameBmp = frame.ToBitmap();
                var ts = DateTime.UtcNow;
                if (ts - lastYoloFrameTimestamp >= yoloFrameTimeStep && (yoloCallTask == null || yoloCallTask.IsCompleted))
                {
                    var frameBytes = Array.Empty<byte>();
                    using (var ms = new MemoryStream())
                    {
                        frameBmp.Save(ms, ImageFormat.Png);
                        frameBytes = ms.ToArray();
                    }
                    yoloCallTask = Task.Run(() => yoloCall(frameBytes));
                    lastYoloFrameTimestamp = ts;
                }
                if (ts - lastDfFrameTimestamp >= dfFrameTimeStep && (dfCallTask == null || dfCallTask.IsCompleted))
                {
                    // TODO: create function to handle this dup code
                    var frameBytes = Array.Empty<byte>();
                    using (var ms = new MemoryStream())
                    {
                        frameBmp.Save(ms, ImageFormat.Png);
                        frameBytes = ms.ToArray();
                    }
                    dfCallTask = Task.Run(() => dfCall(frameBytes));
                    lastDfFrameTimestamp = ts;
                }
            }
        }

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            buffer = new(/*N=*/8, waveIn.WaveFormat); // buffer size N seconds

            var whisperCh = new Channel("127.0.0.1", 9010, ChannelCredentials.Insecure);
            whisperClient = new ModelService.ModelServiceClient(whisperCh);

            var yoloCh = new Channel("127.0.0.1", 9011, ChannelCredentials.Insecure);
            yoloClient = new ModelService.ModelServiceClient(yoloCh);

            var dfCh = new Channel("127.0.0.1", 9012, ChannelCredentials.Insecure);
            dfClient = new ModelService.ModelServiceClient(dfCh);

            outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            var logFileName = Path.Combine(outputFolder, "log.txt");

            capture.ImageGrabbed += (sender, e) => Invoke(() => ProcessVideoFrame(sender, e));
            capture.Start();

            void whisperCall()
            {
                try
                {
                    var bytes = buffer.GetWavBytes(waveIn.WaveFormat);
                    DateTime bufferStartTime = buffer.StartTime!.Value;
                    DateTime bufferEndTime = buffer.EndTime!.Value;
                    var reply = whisperClient.Process(new ProcessRequest { Data = ByteString.CopyFrom(bytes) });
                    //buffer.WriteToFile(Path.Combine(outputFolder, buffer.SnapshotId + ".wav"), waveIn.WaveFormat);
                    var textObj = JsonSerializer.Deserialize<SpeechModelResponseDto>(reply.Reply, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    var words = new List<Word>();
                    foreach (var seg in textObj?.Segments ?? Array.Empty<SpeechSegmentDto>())
                    {
                        if (seg.NoSpeechProb < noSpeechProbThresh)
                        {
                            foreach (var word in seg.Words ?? Array.Empty<WordDto>())
                            {
                                words.Add(new Word
                                {
                                    String = word.Word,
                                    Probability = word.Probability,
                                    SegmentId = seg.Id,
                                    StartTime = bufferStartTime + TimeSpan.FromSeconds(word.Start),
                                    EndTime = bufferStartTime + TimeSpan.FromSeconds(word.End)
                                });
                            }
                        }
                    }
                    if (words.Any())
                    {
                        File.AppendAllText(logFileName, $"AUDIO BUFFER START TIME {buffer.StartTime:HH:mm:ss.fff}\n");
                        File.AppendAllText(logFileName, $"AUDIO BUFFER END TIME {buffer.EndTime:HH:mm:ss.fff}\n");
                        File.AppendAllText(logFileName, "\nTEXT BEFORE ");
                        textBuffer.WriteToFile(logFileName); // before
                        File.AppendAllText(logFileName, $"\nRAW RESPONSE {reply.Reply}\n");
                        File.AppendAllText(logFileName, "\nSNIPPET ");
                        textBuffer.WriteToFile(logFileName, words);
                        textBuffer.AddWords(words, bufferStartTime, bufferEndTime);
                        File.AppendAllText(logFileName, "\nTEXT AFTER ");
                        textBuffer.WriteToFile(logFileName); // after
                        File.AppendAllText(logFileName, "\n--\n\n");
                        textBuffer.Print();
                        var chatItems = textBuffer.CreateChatItems();
                        transcriptViewer.SetChatItems(chatItems);
                        stateMachine.SetChatContext(chatItems);
                        Console.WriteLine("--");
                    }
                    ledWhisper.Status = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ledWhisper.Status = false;
                }
            }

            waveIn.DataAvailable += (s, a) =>
            {
                buffer.WriteData(a.Buffer, a.BytesRecorded);
                var ts = DateTime.UtcNow;
                if (ts - lastSnapshotTimestamp >= snapshotTimeStep && (whisperCallTask == null || whisperCallTask.IsCompleted))
                {
                    whisperCallTask = Task.Run(whisperCall);
                    lastSnapshotTimestamp = ts;
                }
            };

            waveIn.RecordingStopped += async (s, a) =>
            {
                // flush (sort of gracefully)
                await Task.Run(async () =>
                {
                    buttonStop.Enabled = false;
                    while (whisperCallTask != null && !whisperCallTask.IsCompleted)
                    {
                        await Task.Delay(25);
                    }
                    whisperCall();
                    buffer.Clear();
                });
                buttonRecord.Enabled = true;
                if (closing)
                {
                    waveIn.Dispose();
                    capture.Dispose();
                }
            };

            AllocConsole();
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            waveIn.StartRecording();
            buttonRecord.Enabled = false;
            buttonStop.Enabled = true;
            lastSnapshotTimestamp = DateTime.UtcNow;
        }

        private void AudioRecorderMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closing = true;
            waveIn.StopRecording();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();
        }

        private void pnlTranscript_Resize(object sender, EventArgs e)
        {
            transcriptViewer.Refresh(); // WARNME: why?!
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            stateMachine.Transition();
        }
    }
}