using OddbitAi.Niko.Components;
using OddbitAi.Niko.Components;

namespace OddbitAi.Niko
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
            buttonRecord = new Button();
            buttonStop = new Button();
            pnlRightPanel = new Panel();
            ledStateGreet = new StatusLed();
            ledStateLookAround = new StatusLed();
            ledStateIdle = new StatusLed();
            div = new Divider();
            ledLlama = new StatusLed();
            ledYolo = new StatusLed();
            ledDeepface = new StatusLed();
            ledWhisper = new StatusLed();
            pnlTranscript = new Panel();
            transcriptViewer = new TranscriptViewer();
            videoOverlay = new Overlay();
            pnlVideo = new Panel();
            pbVideo = new PictureBox();
            pnlRightPanel.SuspendLayout();
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
            pnlRightPanel.Controls.Add(ledStateGreet);
            pnlRightPanel.Controls.Add(ledStateLookAround);
            pnlRightPanel.Controls.Add(ledStateIdle);
            pnlRightPanel.Controls.Add(div);
            pnlRightPanel.Controls.Add(ledLlama);
            pnlRightPanel.Controls.Add(ledYolo);
            pnlRightPanel.Controls.Add(ledDeepface);
            pnlRightPanel.Controls.Add(ledWhisper);
            pnlRightPanel.Controls.Add(buttonRecord);
            pnlRightPanel.Controls.Add(buttonStop);
            pnlRightPanel.Dock = DockStyle.Right;
            pnlRightPanel.Location = new Point(1241, 0);
            pnlRightPanel.Name = "pnlRightPanel";
            pnlRightPanel.Size = new Size(565, 1081);
            pnlRightPanel.TabIndex = 3;
            // 
            // ledStateGreet
            // 
            ledStateGreet.Dock = DockStyle.Top;
            ledStateGreet.Label = "Greet";
            ledStateGreet.Location = new Point(0, 463);
            ledStateGreet.Name = "ledStateGreet";
            ledStateGreet.Size = new Size(565, 70);
            ledStateGreet.Status = false;
            ledStateGreet.TabIndex = 9;
            // 
            // ledStateLookAround
            // 
            ledStateLookAround.Dock = DockStyle.Top;
            ledStateLookAround.Label = "LookAround";
            ledStateLookAround.Location = new Point(0, 393);
            ledStateLookAround.Name = "ledStateLookAround";
            ledStateLookAround.Size = new Size(565, 70);
            ledStateLookAround.Status = false;
            ledStateLookAround.TabIndex = 8;
            // 
            // ledStateIdle
            // 
            ledStateIdle.Dock = DockStyle.Top;
            ledStateIdle.Label = "Idle";
            ledStateIdle.Location = new Point(0, 323);
            ledStateIdle.Name = "ledStateIdle";
            ledStateIdle.Size = new Size(565, 70);
            ledStateIdle.Status = true;
            ledStateIdle.TabIndex = 7;
            // 
            // div
            // 
            div.Dock = DockStyle.Top;
            div.Location = new Point(0, 280);
            div.Name = "div";
            div.Size = new Size(565, 43);
            div.TabIndex = 6;
            // 
            // ledLlama
            // 
            ledLlama.Dock = DockStyle.Top;
            ledLlama.Label = "LLaMA/ChatGPT";
            ledLlama.Location = new Point(0, 210);
            ledLlama.Name = "ledLlama";
            ledLlama.Size = new Size(565, 70);
            ledLlama.Status = false;
            ledLlama.TabIndex = 5;
            // 
            // ledYolo
            // 
            ledYolo.Dock = DockStyle.Top;
            ledYolo.Label = "YOLO";
            ledYolo.Location = new Point(0, 140);
            ledYolo.Name = "ledYolo";
            ledYolo.Size = new Size(565, 70);
            ledYolo.Status = false;
            ledYolo.TabIndex = 4;
            // 
            // ledDeepface
            // 
            ledDeepface.Dock = DockStyle.Top;
            ledDeepface.Label = "Deepface";
            ledDeepface.Location = new Point(0, 70);
            ledDeepface.Name = "ledDeepface";
            ledDeepface.Size = new Size(565, 70);
            ledDeepface.Status = false;
            ledDeepface.TabIndex = 3;
            // 
            // ledWhisper
            // 
            ledWhisper.Dock = DockStyle.Top;
            ledWhisper.Label = "Whisper";
            ledWhisper.Location = new Point(0, 0);
            ledWhisper.Name = "ledWhisper";
            ledWhisper.Size = new Size(565, 70);
            ledWhisper.Status = false;
            ledWhisper.TabIndex = 2;
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
            // AudioRecorderMainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1806, 1081);
            Controls.Add(pnlVideo);
            Controls.Add(pnlTranscript);
            Controls.Add(pnlRightPanel);
            Name = "AudioRecorderMainForm";
            Text = "Niko";
            FormClosing += AudioRecorderMainForm_FormClosing;
            Load += AudioRecorderMainForm_Load;
            pnlRightPanel.ResumeLayout(false);
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
        private TranscriptViewer transcriptViewer;
        private StatusLed ledWhisper;
        private StatusLed ledLlama;
        private StatusLed ledYolo;
        private StatusLed ledDeepface;
        private StatusLed ledStateIdle;
        private Divider div;
        private StatusLed ledStateGreet;
        private StatusLed ledStateLookAround;
    }
}