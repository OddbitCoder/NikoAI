using OddbitAi.Controls;

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
            pnlLedDeepface = new Panel();
            lblLedDeepface = new Label();
            pbLedDeepface = new PictureBox();
            pnlLedYOLO = new Panel();
            lblLedYOLO = new Label();
            pbLedYOLO = new PictureBox();
            pnlLedWhisper = new Panel();
            lblLedWhisper = new Label();
            pbLedWhisper = new PictureBox();
            pnlTranscript = new Panel();
            txtBoxTranscript = new RichTextBox();
            pbOverlay = new PictureBox();
            pnlVideo = new Panel();
            pbVideo = new PictureBox();
            imgLstLights = new ImageList(components);
            overlayVideo = new Overlay();
            pnlRightPanel.SuspendLayout();
            pnlLedDeepface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLedDeepface).BeginInit();
            pnlLedYOLO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLedYOLO).BeginInit();
            pnlLedWhisper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLedWhisper).BeginInit();
            pnlTranscript.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbOverlay).BeginInit();
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
            buttonRecord.Visible = false;
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
            buttonStop.Visible = false;
            buttonStop.Click += buttonStop_Click;
            // 
            // pnlRightPanel
            // 
            pnlRightPanel.Controls.Add(pnlLedDeepface);
            pnlRightPanel.Controls.Add(pnlLedYOLO);
            pnlRightPanel.Controls.Add(pnlLedWhisper);
            pnlRightPanel.Controls.Add(buttonRecord);
            pnlRightPanel.Controls.Add(buttonStop);
            pnlRightPanel.Dock = DockStyle.Right;
            pnlRightPanel.Location = new Point(1241, 0);
            pnlRightPanel.Name = "pnlRightPanel";
            pnlRightPanel.Size = new Size(565, 1081);
            pnlRightPanel.TabIndex = 3;
            // 
            // pnlLedDeepface
            // 
            pnlLedDeepface.Controls.Add(lblLedDeepface);
            pnlLedDeepface.Controls.Add(pbLedDeepface);
            pnlLedDeepface.Dock = DockStyle.Top;
            pnlLedDeepface.Location = new Point(0, 140);
            pnlLedDeepface.Name = "pnlLedDeepface";
            pnlLedDeepface.Size = new Size(565, 70);
            pnlLedDeepface.TabIndex = 4;
            // 
            // lblLedDeepface
            // 
            lblLedDeepface.Dock = DockStyle.Fill;
            lblLedDeepface.Location = new Point(70, 0);
            lblLedDeepface.Name = "lblLedDeepface";
            lblLedDeepface.Size = new Size(495, 70);
            lblLedDeepface.TabIndex = 1;
            lblLedDeepface.Text = "Deepface";
            lblLedDeepface.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbLedDeepface
            // 
            pbLedDeepface.Dock = DockStyle.Left;
            pbLedDeepface.Image = (Image)resources.GetObject("pbLedDeepface.Image");
            pbLedDeepface.Location = new Point(0, 0);
            pbLedDeepface.Name = "pbLedDeepface";
            pbLedDeepface.Size = new Size(70, 70);
            pbLedDeepface.SizeMode = PictureBoxSizeMode.StretchImage;
            pbLedDeepface.TabIndex = 0;
            pbLedDeepface.TabStop = false;
            // 
            // pnlLedYOLO
            // 
            pnlLedYOLO.Controls.Add(lblLedYOLO);
            pnlLedYOLO.Controls.Add(pbLedYOLO);
            pnlLedYOLO.Dock = DockStyle.Top;
            pnlLedYOLO.Location = new Point(0, 70);
            pnlLedYOLO.Name = "pnlLedYOLO";
            pnlLedYOLO.Size = new Size(565, 70);
            pnlLedYOLO.TabIndex = 3;
            // 
            // lblLedYOLO
            // 
            lblLedYOLO.Dock = DockStyle.Fill;
            lblLedYOLO.Location = new Point(70, 0);
            lblLedYOLO.Name = "lblLedYOLO";
            lblLedYOLO.Size = new Size(495, 70);
            lblLedYOLO.TabIndex = 1;
            lblLedYOLO.Text = "YOLO";
            lblLedYOLO.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbLedYOLO
            // 
            pbLedYOLO.Dock = DockStyle.Left;
            pbLedYOLO.Image = (Image)resources.GetObject("pbLedYOLO.Image");
            pbLedYOLO.Location = new Point(0, 0);
            pbLedYOLO.Name = "pbLedYOLO";
            pbLedYOLO.Size = new Size(70, 70);
            pbLedYOLO.SizeMode = PictureBoxSizeMode.StretchImage;
            pbLedYOLO.TabIndex = 0;
            pbLedYOLO.TabStop = false;
            // 
            // pnlLedWhisper
            // 
            pnlLedWhisper.Controls.Add(lblLedWhisper);
            pnlLedWhisper.Controls.Add(pbLedWhisper);
            pnlLedWhisper.Dock = DockStyle.Top;
            pnlLedWhisper.Location = new Point(0, 0);
            pnlLedWhisper.Name = "pnlLedWhisper";
            pnlLedWhisper.Size = new Size(565, 70);
            pnlLedWhisper.TabIndex = 2;
            // 
            // lblLedWhisper
            // 
            lblLedWhisper.Dock = DockStyle.Fill;
            lblLedWhisper.Location = new Point(70, 0);
            lblLedWhisper.Name = "lblLedWhisper";
            lblLedWhisper.Size = new Size(495, 70);
            lblLedWhisper.TabIndex = 1;
            lblLedWhisper.Text = "Whisper";
            lblLedWhisper.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbLedWhisper
            // 
            pbLedWhisper.Dock = DockStyle.Left;
            pbLedWhisper.Image = (Image)resources.GetObject("pbLedWhisper.Image");
            pbLedWhisper.Location = new Point(0, 0);
            pbLedWhisper.Name = "pbLedWhisper";
            pbLedWhisper.Size = new Size(70, 70);
            pbLedWhisper.SizeMode = PictureBoxSizeMode.StretchImage;
            pbLedWhisper.TabIndex = 0;
            pbLedWhisper.TabStop = false;
            // 
            // pnlTranscript
            // 
            pnlTranscript.Controls.Add(txtBoxTranscript);
            pnlTranscript.Dock = DockStyle.Bottom;
            pnlTranscript.Location = new Point(0, 725);
            pnlTranscript.Name = "pnlTranscript";
            pnlTranscript.Size = new Size(1241, 356);
            pnlTranscript.TabIndex = 4;
            pnlTranscript.Resize += pnlTranscript_Resize;
            // 
            // txtBoxTranscript
            // 
            txtBoxTranscript.BorderStyle = BorderStyle.None;
            txtBoxTranscript.Dock = DockStyle.Fill;
            txtBoxTranscript.Location = new Point(0, 0);
            txtBoxTranscript.Name = "txtBoxTranscript";
            txtBoxTranscript.Size = new Size(1241, 356);
            txtBoxTranscript.TabIndex = 0;
            txtBoxTranscript.Text = "";
            // 
            // pbOverlay
            // 
            pbOverlay.BackColor = Color.Transparent;
            pbOverlay.Dock = DockStyle.Fill;
            pbOverlay.Location = new Point(0, 0);
            pbOverlay.Name = "pbOverlay";
            pbOverlay.Size = new Size(1241, 725);
            pbOverlay.TabIndex = 4;
            pbOverlay.TabStop = false;
            pbOverlay.Paint += pbOverlay_Paint;
            // 
            // pnlVideo
            // 
            pnlVideo.Controls.Add(pbOverlay);
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
            // imgLstLights
            // 
            imgLstLights.ColorDepth = ColorDepth.Depth32Bit;
            imgLstLights.ImageStream = (ImageListStreamer)resources.GetObject("imgLstLights.ImageStream");
            imgLstLights.TransparentColor = Color.Transparent;
            imgLstLights.Images.SetKeyName(0, "off");
            imgLstLights.Images.SetKeyName(1, "on");
            // 
            // overlayVideo
            // 
            overlayVideo.BackColor = Color.IndianRed;
            overlayVideo.Dock = DockStyle.Fill;
            overlayVideo.Location = new Point(0, 0);
            overlayVideo.Name = "overlayVideo";
            overlayVideo.Size = new Size(1241, 725);
            overlayVideo.TabIndex = 4;
            overlayVideo.TabStop = false;
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
            pnlLedDeepface.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLedDeepface).EndInit();
            pnlLedYOLO.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLedYOLO).EndInit();
            pnlLedWhisper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbLedWhisper).EndInit();
            pnlTranscript.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbOverlay).EndInit();
            pnlVideo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbVideo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonRecord;
        private Button buttonStop;
        private Panel pnlRightPanel;
        private Panel pnlTranscript;
        private RichTextBox txtBoxTranscript;
        private PictureBox pbOverlay;
        private Panel pnlVideo;
        private PictureBox pbVideo;
        private Panel pnlLedWhisper;
        private Label lblLedWhisper;
        private PictureBox pbLedWhisper;
        private Panel pnlLedDeepface;
        private Label lblLedDeepface;
        private PictureBox pbLedDeepface;
        private Panel pnlLedYOLO;
        private Label lblLedYOLO;
        private PictureBox pbLedYOLO;
        private ImageList imgLstLights;
        private Overlay overlayVideo;
    }
}