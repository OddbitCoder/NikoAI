namespace OddbitAi.AudioRecorder
{
    public partial class Divider : UserControl
    {
        public Divider()
        {
            InitializeComponent();
        }

        private void Divider_Resize(object sender, EventArgs e)
        {
            dist.Height = (Height - 2) / 2;
        }
    }
}
