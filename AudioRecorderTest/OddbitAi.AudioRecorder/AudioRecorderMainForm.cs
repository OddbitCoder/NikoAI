using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using OddbitAi.Whisper;
using OddbitAi.Whisper.Dto;

namespace OddbitAi.AudioRecorder
{
    public partial class AudioRecorderMainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

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
        private readonly TextBuffer textBuffer
            = new(TimeSpan.FromSeconds(/*N=*/1)); // trim audio buffer N seconds on each side
        private DateTime? lastSnapshotTimestamp
            = null;
        private WhisperService.WhisperServiceClient? whisperClient;
        //private string? outputFolder;
        private Task? whisperCallTask 
            = null;
        private const double noSpeechProbThresh
            = 0.3;

        public AudioRecorderMainForm()
        {
            InitializeComponent();
        }

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            buffer = new(/*N=*/16, waveIn.WaveFormat); // buffer size N seconds

            var channel = new Channel("127.0.0.1", 9010, ChannelCredentials.Insecure);
            whisperClient = new WhisperService.WhisperServiceClient(channel);

            //outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            //Directory.CreateDirectory(outputFolder);

            void whisperCall()
            {
                try
                {
                    var bytes = buffer.GetWavBytes(waveIn.WaveFormat);
                    DateTime bufferStartTime = buffer.StartTime!.Value;
                    DateTime bufferEndTime = buffer.EndTime!.Value;
                    var reply = whisperClient.ProcessAudio(new ProcessAudioRequest { AudioData = ByteString.CopyFrom(bytes) });
                    //Console.WriteLine(reply.Text);
                    //buffer.WriteToFile(Path.Combine(outputFolder, buffer.SnapshotId + ".wav"), waveIn.WaveFormat);
                    var textObj = JsonSerializer.Deserialize<TextDto>(reply.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
                    var words = new List<Word>();
                    if (textObj?.Format == ResponseFormat.NeMo)
                    {
                        // handle NeMo response
                        Console.WriteLine(textObj?.Text);
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
                        textBuffer.AddWords(words, bufferStartTime, bufferEndTime);
                        Console.WriteLine(reply.Text);
                        textBuffer.Print();
                        Console.WriteLine("--");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
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