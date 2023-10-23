using System.ComponentModel;

namespace OddbitAi.Niko.Components
{
    public partial class StatusLed : UserControl
    {
        private bool status
            = false; // off

        [Category("Appearance")]
        public string Label
        {
            get => lblLabel.Text;
            set => lblLabel.Text = value;
        }

        [Category("Appearance")]
        public bool Status
        {
            get => status;
            set
            {
                if (status == value) { return; }
                status = value;
                pbStatusLed.Image = imgLstStatusLeds.Images[status ? "on" : "off"];
                Refresh();
            }
        }

        public StatusLed()
        {
            InitializeComponent();
        }
    }
}
