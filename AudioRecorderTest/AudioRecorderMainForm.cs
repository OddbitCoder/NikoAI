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

        //WaveFileWriter writer = null;
        AudioBuffer buffer = new(160000); // 10 seconds
        WaveInEvent waveIn = new() {
            BufferMilliseconds = 250,
            NumberOfBuffers = 2,
            //DeviceNumber = 0,
            WaveFormat = new WaveFormat(8000, 16, 1)
        };
        bool closing = false;
        string outputFilePath;

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            AllocConsole();

            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            outputFilePath = Path.Combine(outputFolder, "recorded.wav");

            waveIn.DataAvailable += (s, a) =>
            {
                //writer.Write(a.Buffer, 0, a.BytesRecorded);
                //if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                //{
                //    waveIn.StopRecording();
                //}
                buffer.WriteData(a.Buffer, a.BytesRecorded);
                if (buffer.RawByteCount == 160000) 
                {
                    waveIn.StopRecording();
                }
            };

            waveIn.RecordingStopped += (s, a) =>
            {
                //writer?.Dispose();
                //writer = null;
                //buffer.WriteToFile(outputFilePath, waveIn.WaveFormat);
                File.WriteAllBytes(outputFilePath, buffer.GetWavBytes(waveIn.WaveFormat));
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
            //writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
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