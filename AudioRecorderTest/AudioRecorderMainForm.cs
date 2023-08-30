using NAudio.Wave;
using System.Runtime.InteropServices;

// https://stackoverflow.com/questions/11500222/how-to-write-naudio-wavestream-to-a-memory-stream
// https://stackoverflow.com/questions/9804519/waveinevent-sample-event-frequency

namespace OddbitAi.AudioRecorder
{
    public partial class AudioRecorderMainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public AudioRecorderMainForm()
        {
            InitializeComponent();
        }

        private WaveInEvent waveIn = new() {
            BufferMilliseconds = 250,
            NumberOfBuffers = 2,
            //DeviceNumber = 0,
            WaveFormat = new WaveFormat(8000, 16, 1)
        };
        private AudioBuffer buffer;
        private bool closing = false;
        private TimeSpan snapshotTimeStep
            = TimeSpan.FromSeconds(2); // make a snapshot every 2 seconds
        private DateTime? lastSnapshotTimestamp
            = null;

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            AllocConsole();

            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            buffer = new(16000 * 10, waveIn.WaveFormat); // 10 seconds

            waveIn.DataAvailable += (s, a) =>
            {
                buffer.WriteData(a.Buffer, a.BytesRecorded);
                var ts = DateTime.UtcNow;
                if ((lastSnapshotTimestamp == null && buffer.RawByteCount >= 16000 * 2) || ts - lastSnapshotTimestamp >= snapshotTimeStep)
                {
                    Console.WriteLine(buffer.SnapshotId);
                    buffer.WriteToFile(Path.Combine(outputFolder, buffer.SnapshotId + ".wav"), waveIn.WaveFormat);
                    lastSnapshotTimestamp = ts;
                }
            };

            waveIn.RecordingStopped += (s, a) =>
            {
                buffer.Clear();
                buttonRecord.Enabled = true;
                buttonStop.Enabled = false;
                if (closing)
                {
                    waveIn.Dispose();
                }
            };
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            waveIn.StartRecording();
            buttonRecord.Enabled = false;
            buttonStop.Enabled = true;
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