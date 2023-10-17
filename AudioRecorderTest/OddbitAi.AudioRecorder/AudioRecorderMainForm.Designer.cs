namespace OddbitAi.AudioRecorder
{
    partial class AudioRecorderMainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioRecorderMainForm));
            buttonRecord = new Button();
            buttonStop = new Button();
            pnlRightPanel = new Panel();
            pnlStatusDeepface = new Panel();
            lblStatusDeepface = new Label();
            pbStatusLedDeepface = new PictureBox();
            pnlStatusYOLO = new Panel();
            lblStatusYOLO = new Label();
            pbStatusLedYOLO = new PictureBox();
            pnlStatusWhisper = new Panel();
            lblStatusWhisper = new Label();
            pbStatusLedWhisper = new PictureBox();
            pnlTranscript = new Panel();
            transcriptViewer = new TranscriptViewer();
            videoOverlay = new Overlay();
            pnlVideo = new Panel();
            pbVideo = new PictureBox();
            imgLstStatusLeds = new ImageList(components);
            pnlRightPanel.SuspendLayout();
            pnlStatusDeepface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbStatusLedDeepface).BeginInit();
            pnlStatusYOLO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbStatusLedYOLO).BeginInit();
            pnlStatusWhisper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbStatusLedWhisper).BeginInit();
            pnlTranscript.SuspendLayout();
            pnlVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbVideo).BeginInit();
            SuspendLayout();
            // 
            // buttonRecord
            // 
            buttonRecord.Location = new Point(93, 968);
            buttonRecord.Name = "buttonRecord";
            buttonRecord.Size = new Size(188, 58);
            buttonRecord.TabIndex = 0;
            buttonRecord.Text = "Record";
            buttonRecord.UseVisualStyleBackColor = true;
            buttonRecord.Click += buttonRecord_Click;
            // 
            // buttonStop
            // 
            buttonStop.Enabled = false;
            buttonStop.Location = new Point(323, 968);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(188, 58);
            buttonStop.TabIndex = 1;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += buttonStop_Click;
            // 
            // pnlRightPanel
            // 
            pnlRightPanel.Controls.Add(pnlStatusDeepface);
            pnlRightPanel.Controls.Add(pnlStatusYOLO);
            pnlRightPanel.Controls.Add(pnlStatusWhisper);
            pnlRightPanel.Controls.Add(buttonRecord);
            pnlRightPanel.Controls.Add(buttonStop);
            pnlRightPanel.Dock = DockStyle.Right;
            pnlRightPanel.Location = new Point(1241, 0);
            pnlRightPanel.Name = "pnlRightPanel";
            pnlRightPanel.Size = new Size(565, 1081);
            pnlRightPanel.TabIndex = 3;
            // 
            // pnlStatusDeepface
            // 
            pnlStatusDeepface.Controls.Add(lblStatusDeepface);
            pnlStatusDeepface.Controls.Add(pbStatusLedDeepface);
            pnlStatusDeepface.Dock = DockStyle.Top;
            pnlStatusDeepface.Location = new Point(0, 140);
            pnlStatusDeepface.Name = "pnlStatusDeepface";
            pnlStatusDeepface.Size = new Size(565, 70);
            pnlStatusDeepface.TabIndex = 4;
            // 
            // lblStatusDeepface
            // 
            lblStatusDeepface.Dock = DockStyle.Fill;
            lblStatusDeepface.Location = new Point(70, 0);
            lblStatusDeepface.Name = "lblStatusDeepface";
            lblStatusDeepface.Size = new Size(495, 70);
            lblStatusDeepface.TabIndex = 1;
            lblStatusDeepface.Text = "Deepface";
            lblStatusDeepface.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbStatusLedDeepface
            // 
            pbStatusLedDeepface.Dock = DockStyle.Left;
            pbStatusLedDeepface.Image = (Image)resources.GetObject("pbStatusLedDeepface.Image");
            pbStatusLedDeepface.Location = new Point(0, 0);
            pbStatusLedDeepface.Name = "pbStatusLedDeepface";
            pbStatusLedDeepface.Size = new Size(70, 70);
            pbStatusLedDeepface.SizeMode = PictureBoxSizeMode.StretchImage;
            pbStatusLedDeepface.TabIndex = 0;
            pbStatusLedDeepface.TabStop = false;
            // 
            // pnlStatusYOLO
            // 
            pnlStatusYOLO.Controls.Add(lblStatusYOLO);
            pnlStatusYOLO.Controls.Add(pbStatusLedYOLO);
            pnlStatusYOLO.Dock = DockStyle.Top;
            pnlStatusYOLO.Location = new Point(0, 70);
            pnlStatusYOLO.Name = "pnlStatusYOLO";
            pnlStatusYOLO.Size = new Size(565, 70);
            pnlStatusYOLO.TabIndex = 3;
            // 
            // lblStatusYOLO
            // 
            lblStatusYOLO.Dock = DockStyle.Fill;
            lblStatusYOLO.Location = new Point(70, 0);
            lblStatusYOLO.Name = "lblStatusYOLO";
            lblStatusYOLO.Size = new Size(495, 70);
            lblStatusYOLO.TabIndex = 1;
            lblStatusYOLO.Text = "YOLO";
            lblStatusYOLO.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbStatusLedYOLO
            // 
            pbStatusLedYOLO.Dock = DockStyle.Left;
            pbStatusLedYOLO.Image = (Image)resources.GetObject("pbStatusLedYOLO.Image");
            pbStatusLedYOLO.Location = new Point(0, 0);
            pbStatusLedYOLO.Name = "pbStatusLedYOLO";
            pbStatusLedYOLO.Size = new Size(70, 70);
            pbStatusLedYOLO.SizeMode = PictureBoxSizeMode.StretchImage;
            pbStatusLedYOLO.TabIndex = 0;
            pbStatusLedYOLO.TabStop = false;
            // 
            // pnlStatusWhisper
            // 
            pnlStatusWhisper.Controls.Add(lblStatusWhisper);
            pnlStatusWhisper.Controls.Add(pbStatusLedWhisper);
            pnlStatusWhisper.Dock = DockStyle.Top;
            pnlStatusWhisper.Location = new Point(0, 0);
            pnlStatusWhisper.Name = "pnlStatusWhisper";
            pnlStatusWhisper.Size = new Size(565, 70);
            pnlStatusWhisper.TabIndex = 2;
            // 
            // lblStatusWhisper
            // 
            lblStatusWhisper.Dock = DockStyle.Fill;
            lblStatusWhisper.Location = new Point(70, 0);
            lblStatusWhisper.Name = "lblStatusWhisper";
            lblStatusWhisper.Size = new Size(495, 70);
            lblStatusWhisper.TabIndex = 1;
            lblStatusWhisper.Text = "Whisper";
            lblStatusWhisper.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbStatusLedWhisper
            // 
            pbStatusLedWhisper.Dock = DockStyle.Left;
            pbStatusLedWhisper.Image = (Image)resources.GetObject("pbStatusLedWhisper.Image");
            pbStatusLedWhisper.Location = new Point(0, 0);
            pbStatusLedWhisper.Name = "pbStatusLedWhisper";
            pbStatusLedWhisper.Size = new Size(70, 70);
            pbStatusLedWhisper.SizeMode = PictureBoxSizeMode.StretchImage;
            pbStatusLedWhisper.TabIndex = 0;
            pbStatusLedWhisper.TabStop = false;
            // 
            // pnlTranscript
            // 
            pnlTranscript.Controls.Add(transcriptViewer);
            pnlTranscript.Dock = DockStyle.Bottom;
            pnlTranscript.Location = new Point(0, 725);
            pnlTranscript.Name = "pnlTranscript";
            pnlTranscript.Size = new Size(1241, 356);
            pnlTranscript.TabIndex = 4;
            pnlTranscript.Resize += pnlTranscript_Resize;
            // 
            // transcriptViewer
            // 
            transcriptViewer.BackColor = SystemColors.Window;
            transcriptViewer.Dock = DockStyle.Fill;
            transcriptViewer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            transcriptViewer.Location = new Point(0, 0);
            transcriptViewer.Name = "transcriptViewer";
            transcriptViewer.Size = new Size(1241, 356);
            transcriptViewer.TabIndex = 2;
            // 
            // videoOverlay
            // 
            videoOverlay.AnnotationLineWidth = 8F;
            videoOverlay.Dock = DockStyle.Fill;
            videoOverlay.FaceAnnotationColor = Color.Yellow;
            videoOverlay.FaceAnnotationLabelTextColor = Color.Black;
            videoOverlay.Location = new Point(0, 0);
            videoOverlay.Name = "videoOverlay";
            videoOverlay.ObjectAnnotationColor = Color.Red;
            videoOverlay.ObjectAnnotationLabelTextColor = Color.White;
            videoOverlay.Size = new Size(1241, 725);
            videoOverlay.TabIndex = 4;
            // 
            // pnlVideo
            // 
            pnlVideo.Controls.Add(videoOverlay);
            pnlVideo.Controls.Add(pbVideo);
            pnlVideo.Dock = DockStyle.Fill;
            pnlVideo.Location = new Point(0, 0);
            pnlVideo.Name = "pnlVideo";
            pnlVideo.Size = new Size(1241, 725);
            pnlVideo.TabIndex = 5;
            // 
            // pbVideo
            // 
            pbVideo.Dock = DockStyle.Fill;
            pbVideo.Location = new Point(0, 0);
            pbVideo.Name = "pbVideo";
            pbVideo.Size = new Size(1241, 725);
            pbVideo.SizeMode = PictureBoxSizeMode.Zoom;
            pbVideo.TabIndex = 3;
            pbVideo.TabStop = false;
            // 
            // imgLstStatusLeds
            // 
            imgLstStatusLeds.ColorDepth = ColorDepth.Depth32Bit;
            imgLstStatusLeds.ImageStream = (ImageListStreamer)resources.GetObject("imgLstStatusLeds.ImageStream");
            imgLstStatusLeds.TransparentColor = Color.Transparent;
            imgLstStatusLeds.Images.SetKeyName(0, "off");
            imgLstStatusLeds.Images.SetKeyName(1, "on");
            // 
            // AudioRecorderMainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1806, 1081);
            Controls.Add(pnlVideo);
            Controls.Add(pnlTranscript);
            Controls.Add(pnlRightPanel);
            Name = "AudioRecorderMainForm";
            Text = "Audio Recorder Test";
            FormClosing += AudioRecorderMainForm_FormClosing;
            Load += AudioRecorderMainForm_Load;
            pnlRightPanel.ResumeLayout(false);
            pnlStatusDeepface.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbStatusLedDeepface).EndInit();
            pnlStatusYOLO.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbStatusLedYOLO).EndInit();
            pnlStatusWhisper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbStatusLedWhisper).EndInit();
            pnlTranscript.ResumeLayout(false);
            pnlVideo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbVideo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonRecord;
        private Button buttonStop;
        private Panel pnlRightPanel;
        private Panel pnlTranscript;
        private Overlay videoOverlay;
        private Panel pnlVideo;
        private PictureBox pbVideo;
        private Panel pnlStatusWhisper;
        private Label lblStatusWhisper;
        private PictureBox pbStatusLedWhisper;
        private Panel pnlStatusDeepface;
        private Label lblStatusDeepface;
        private PictureBox pbStatusLedDeepface;
        private Panel pnlStatusYOLO;
        private Label lblStatusYOLO;
        private PictureBox pbStatusLedYOLO;
        private ImageList imgLstStatusLeds;
        private TranscriptViewer transcriptViewer;
    }
}