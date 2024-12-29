using System.Windows.Forms;

namespace Labirent
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.labirentPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(868, 699);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(120, 40);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Başlat";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // labirentPanel
            // 
            this.labirentPanel.Location = new System.Drawing.Point(12, 12);
            this.labirentPanel.Name = "labirentPanel";
            this.labirentPanel.Size = new System.Drawing.Size(840, 800);
            this.labirentPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1000, 792);
            this.Controls.Add(this.labirentPanel);
            this.Controls.Add(this.btnStart);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pikachu Labirent";
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel labirentPanel;
    }
}
