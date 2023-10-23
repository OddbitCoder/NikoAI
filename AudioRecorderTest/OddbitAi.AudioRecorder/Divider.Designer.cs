namespace OddbitAi.AudioRecorder
{
    partial class Divider
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
            div = new Label();
            dist = new Label();
            SuspendLayout();
            // 
            // div
            // 
            div.BorderStyle = BorderStyle.Fixed3D;
            div.Dock = DockStyle.Top;
            div.Location = new Point(0, 53);
            div.Margin = new Padding(0);
            div.Name = "div";
            div.Size = new Size(1445, 2);
            div.TabIndex = 2;
            // 
            // dist
            // 
            dist.Dock = DockStyle.Top;
            dist.Location = new Point(0, 0);
            dist.Name = "dist";
            dist.Size = new Size(1445, 53);
            dist.TabIndex = 1;
            // 
            // Divider
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(div);
            Controls.Add(dist);
            Name = "Divider";
            Size = new Size(1445, 777);
            Resize += Divider_Resize;
            ResumeLayout(false);
        }

        #endregion

        private Label div;
        private Label dist;
    }
}
