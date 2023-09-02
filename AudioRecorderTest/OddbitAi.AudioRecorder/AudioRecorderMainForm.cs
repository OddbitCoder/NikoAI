using System.Runtime.InteropServices;
using System.Text.Json;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using OddbitAi.Whisper;
using OddbitAi.Whisper.Dto;

// Refs
// https://stackoverflow.com/questions/11500222/how-to-write-naudio-wavestream-to-a-memory-stream
// https://stackoverflow.com/questions/9804519/waveinevent-sample-event-frequency

namespace OddbitAi.AudioRecorder
{
    public partial class AudioRecorderMainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private readonly WaveInEvent waveIn = new()
        {
            BufferMilliseconds = 250,
            NumberOfBuffers = 2,
            //DeviceNumber = 0,
            WaveFormat = new WaveFormat(8000, 16, 1)
        };
        private AudioBuffer? buffer;
        private bool closing = false;
        private readonly TimeSpan snapshotTimeStep
            = TimeSpan.FromSeconds(/*N=*/2); // make a snapshot every N seconds
        private DateTime? lastSnapshotTimestamp
            = null;
        private WhisperService.WhisperServiceClient? whisperClient;
        private string? outputFolder;
        Task? whisperCallTask = null;

        public AudioRecorderMainForm()
        {
            InitializeComponent();
        }

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            buffer = new(16000 * /*N=*/10, waveIn.WaveFormat); // buffer size N seconds

            var channel = new Channel("127.0.0.1", 9010, ChannelCredentials.Insecure);
            whisperClient = new WhisperService.WhisperServiceClient(channel);

            outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);

            void whisperCall()
            {
                try
                {
                    var bytes = buffer.GetWavBytes(waveIn.WaveFormat);
                    var reply = whisperClient.ProcessAudio(new ProcessAudioRequest { AudioData = ByteString.CopyFrom(bytes) });
                    //Console.WriteLine(reply.Text);
                    //buffer.WriteToFile(Path.Combine(outputFolder, buffer.SnapshotId + ".wav"), waveIn.WaveFormat);
                    var textObj = JsonSerializer.Deserialize<TextDto>(reply.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    foreach (var seg in textObj?.Segments ?? Array.Empty<SegmentDto>())
                    {
                        if (seg.NoSpeechProb < 0.3) // WARNME
                        {
                            foreach (var word in seg.Words ?? Array.Empty<WordDto>())
                            {
                                Console.Write(word.Word);
                            }
                        }
                    }
                    Console.WriteLine();
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