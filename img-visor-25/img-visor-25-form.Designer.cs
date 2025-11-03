namespace img_visor_25
{
    partial class img_visor_25_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(img_visor_25_form));
            label1 = new Label();
            statusStrip1 = new StatusStrip();
            statusStrip = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            notifyIcon = new NotifyIcon(components);
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 20);
            label1.Name = "label1";
            label1.Size = new Size(224, 20);
            label1.TabIndex = 0;
            label1.Text = "img-visor-25 Ver.1.0.2511011324";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusStrip, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 58);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(305, 26);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "Welcome! System is inactive.";
            // 
            // statusStrip
            // 
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(172, 20);
            statusStrip.Text = "Welcome! System is Idle.";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 18);
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "img-visor is running";
            notifyIcon.BalloonTipTitle = "Welcome to img-visor";
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "img-visor";
            notifyIcon.Visible = true;
            notifyIcon.Click += notifyIcon_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(305, 84);
            Controls.Add(statusStrip1);
            Controls.Add(label1);
            MaximizeBox = false;
            Name = "Form1";
            Opacity = 0.8D;
            ShowIcon = false;
            Text = "img-visor-25";
            WindowState = FormWindowState.Minimized;
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            Resize += Form1_Resize;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusStrip;
        private ToolStripProgressBar toolStripProgressBar1;
        private NotifyIcon notifyIcon;
    }
}
