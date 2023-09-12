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
            buttonRecord = new Button();
            buttonStop = new Button();
            pbCamCapture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbCamCapture).BeginInit();
            SuspendLayout();
            // 
            // buttonRecord
            // 
            buttonRecord.Location = new Point(12, 12);
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
            buttonStop.Location = new Point(206, 12);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(188, 58);
            buttonStop.TabIndex = 1;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += buttonStop_Click;
            // 
            // pbCamCapture
            // 
            pbCamCapture.Location = new Point(309, 111);
            pbCamCapture.Name = "pbCamCapture";
            pbCamCapture.Size = new Size(10, 10);
            pbCamCapture.SizeMode = PictureBoxSizeMode.AutoSize;
            pbCamCapture.TabIndex = 2;
            pbCamCapture.TabStop = false;
            // 
            // AudioRecorderMainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1759, 862);
            Controls.Add(pbCamCapture);
            Controls.Add(buttonStop);
            Controls.Add(buttonRecord);
            Name = "AudioRecorderMainForm";
            Text = "Audio Recorder Test";
            FormClosing += AudioRecorderMainForm_FormClosing;
            Load += AudioRecorderMainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pbCamCapture).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonRecord;
        private Button buttonStop;
        private PictureBox pbCamCapture;
    }
}