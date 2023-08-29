using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    public partial class AudioRecorderMainForm : Form
    {
        public AudioRecorderMainForm()
        {
            InitializeComponent();
        }

        WaveFileWriter writer = null;
        WaveInEvent waveIn = new();
        bool closing = false;
        string outputFilePath;

        private void AudioRecorderMainForm_Load(object sender, EventArgs e)
        {
            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            outputFilePath = Path.Combine(outputFolder, "recorded.wav");

            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                {
                    waveIn.StopRecording();
                }
            };

            waveIn.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
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
            writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
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