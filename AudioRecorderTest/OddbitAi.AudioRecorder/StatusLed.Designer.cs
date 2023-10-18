namespace OddbitAi.AudioRecorder
{
    partial class StatusLed
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusLed));
            lblLabel = new Label();
            pbStatusLed = new PictureBox();
            imgLstStatusLeds = new ImageList(components);
            ((System.ComponentModel.ISupportInitialize)pbStatusLed).BeginInit();
            SuspendLayout();
            // 
            // lblLabel
            // 
            lblLabel.Dock = DockStyle.Fill;
            lblLabel.Location = new Point(70, 0);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(844, 70);
            lblLabel.TabIndex = 3;
            lblLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbStatusLed
            // 
            pbStatusLed.Dock = DockStyle.Left;
            pbStatusLed.Image = (Image)resources.GetObject("pbStatusLed.Image");
            pbStatusLed.Location = new Point(0, 0);
            pbStatusLed.Name = "pbStatusLed";
            pbStatusLed.Size = new Size(70, 70);
            pbStatusLed.SizeMode = PictureBoxSizeMode.StretchImage;
            pbStatusLed.TabIndex = 2;
            pbStatusLed.TabStop = false;
            // 
            // imgLstStatusLeds
            // 
            imgLstStatusLeds.ColorDepth = ColorDepth.Depth32Bit;
            imgLstStatusLeds.ImageStream = (ImageListStreamer)resources.GetObject("imgLstStatusLeds.ImageStream");
            imgLstStatusLeds.TransparentColor = Color.Transparent;
            imgLstStatusLeds.Images.SetKeyName(0, "off");
            imgLstStatusLeds.Images.SetKeyName(1, "on");
            // 
            // StatusLed
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblLabel);
            Controls.Add(pbStatusLed);
            Name = "StatusLed";
            Size = new Size(914, 70);
            ((System.ComponentModel.ISupportInitialize)pbStatusLed).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblLabel;
        private PictureBox pbStatusLed;
        private ImageList imgLstStatusLeds;
    }
}
