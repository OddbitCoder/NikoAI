using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing.Imaging;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using OddbitAi.Whisper;
using OddbitAi.Whisper.Dto;
using OddbitAi.Models;
using OddbitAi.Models.YoloDto;
using Emgu.CV;

namespace OddbitAi.AudioRecorder
{
    public partial class AudioRecorderMainForm : Form
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
        private WhisperService.WhisperServiceClient? whisperClient;
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

        // cam video

        private readonly VideoCapture capture
            = new();

        public AudioRecorderMainForm()
        {
            InitializeComponent();
        }

        private void ProcessCamFrame(object? sender, EventArgs e)
        {
            void yoloCall(byte[] frameBytes)
            {
                try
                {
                    var reply = yoloClient!.Process(new ProcessRequest { Data = ByteString.CopyFrom(frameBytes) });
                    //Console.WriteLine(reply);
                    var replyObj = JsonSerializer.Deserialize<YoloReplyDto>(reply.Reply, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    Console.WriteLine(replyObj?.Summary?.TrimEnd(',', ' ')); // TODO: move trimming to python
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            void dfCall(byte[] frameBytes)
            {
                try
                {
                    var reply = dfClient!.Process(new ProcessRequest { Data = ByteString.CopyFrom(frameBytes) });
                    //Console.WriteLine(reply.Reply);
                    var replyObj = JsonSerializer.Deserialize<YoloReplyDto>(reply.Reply, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    foreach (var obj in replyObj?.Objects ?? Array.Empty<YoloObjectDto>())
                    {
                        if (obj.Verified) 
                        {
                            Console.Write($"{obj.Name} ");
                        }
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            var frame = new Mat();
            capture.Retrieve(frame);
            
            if (!frame.IsEmpty)
            {
                pbCamCapture.Invoke(() => pbCamCapture.Image = frame.ToBitmap());
                var ts = DateTime.UtcNow;
                if (ts - lastYoloFrameTimestamp >= yoloFrameTimeStep && (yoloCallTask == null || yoloCallTask.IsCompleted))
                {
                    var frameBytes = Array.Empty<byte>();
                    using (var ms = new MemoryStream())
                    {
                        frame.ToBitmap().Save(ms, ImageFormat.Png);
                        frameBytes = ms.ToArray();
                    }
                    //yoloCallTask = Task.Run(() => yoloCall(frameBytes)); // WARNME: temporarily disabled
                    lastYoloFrameTimestamp = ts;
                }
                if (ts - lastDfFrameTimestamp >= dfFrameTimeStep && (dfCallTask == null || dfCallTask.IsCompleted))
                {
                    // TODO: create function to handle this dup code
                    var frameBytes = Array.Empty<byte>();
                    using (var ms = new MemoryStream())
                    {
                        frame.ToBitmap().Save(ms, ImageFormat.Png);
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
            whisperClient = new WhisperService.WhisperServiceClient(whisperCh);

            var yoloCh = new Channel("127.0.0.1", 9011, ChannelCredentials.Insecure);
            yoloClient = new ModelService.ModelServiceClient(yoloCh);

            var dfCh = new Channel("127.0.0.1", 9012, ChannelCredentials.Insecure);
            dfClient = new ModelService.ModelServiceClient(dfCh);

            outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            var logFileName = Path.Combine(outputFolder, "log.txt");

            capture.ImageGrabbed += ProcessCamFrame;
            capture.Start();

            void whisperCall()
            {
                try
                {
                    var bytes = buffer.GetWavBytes(waveIn.WaveFormat);
                    DateTime bufferStartTime = buffer.StartTime!.Value;
                    DateTime bufferEndTime = buffer.EndTime!.Value;
                    var reply = whisperClient.ProcessAudio(new ProcessAudioRequest { AudioData = ByteString.CopyFrom(bytes) });
                    //buffer.WriteToFile(Path.Combine(outputFolder, buffer.SnapshotId + ".wav"), waveIn.WaveFormat);
                    var textObj = JsonSerializer.Deserialize<TextDto>(reply.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    var words = new List<Word>();
                    if (textObj?.Format == ResponseFormat.NeMo)
                    {
                        // handle NeMo response
                        Console.WriteLine(textObj.Text);
                        return;
                    }
                    else if (textObj?.Format == ResponseFormat.Exception)
                    {
                        Console.WriteLine(textObj.Text);
                        return;
                    }
                    foreach (var seg in textObj?.Segments ?? Array.Empty<SegmentDto>())
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
                        File.AppendAllText(logFileName, $"\nRAW RESPONSE {reply.Text}\n");
                        File.AppendAllText(logFileName, "\nSNIPPET ");
                        textBuffer.WriteToFile(logFileName, words);
                        textBuffer.AddWords(words, bufferStartTime, bufferEndTime);
                        File.AppendAllText(logFileName, "\nTEXT AFTER ");
                        textBuffer.WriteToFile(logFileName); // after
                        File.AppendAllText(logFileName, "\n--\n\n");
                        textBuffer.Print();
                        Console.WriteLine("--");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
    }
}