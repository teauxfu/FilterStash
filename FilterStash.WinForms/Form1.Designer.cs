namespace WinFormsShell
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openPoE2FolderToolStripMenuItem = new ToolStripMenuItem();
            openCacheFolderToolStripMenuItem = new ToolStripMenuItem();
            restartFilterStashToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            changeBackgroundToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            checkForUpdatesToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1174, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openPoE2FolderToolStripMenuItem, openCacheFolderToolStripMenuItem, restartFilterStashToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openPoE2FolderToolStripMenuItem
            // 
            openPoE2FolderToolStripMenuItem.Name = "openPoE2FolderToolStripMenuItem";
            openPoE2FolderToolStripMenuItem.Size = new Size(171, 22);
            openPoE2FolderToolStripMenuItem.Text = "Open PoE2 folder";
            openPoE2FolderToolStripMenuItem.Click += openPoE2FolderToolStripMenuItem_Click;
            // 
            // openCacheFolderToolStripMenuItem
            // 
            openCacheFolderToolStripMenuItem.Name = "openCacheFolderToolStripMenuItem";
            openCacheFolderToolStripMenuItem.Size = new Size(171, 22);
            openCacheFolderToolStripMenuItem.Text = "Open cache folder";
            openCacheFolderToolStripMenuItem.Click += openCacheFolderToolStripMenuItem_Click;
            // 
            // restartFilterStashToolStripMenuItem
            // 
            restartFilterStashToolStripMenuItem.Name = "restartFilterStashToolStripMenuItem";
            restartFilterStashToolStripMenuItem.Size = new Size(171, 22);
            restartFilterStashToolStripMenuItem.Text = "Restart FilterStash";
            restartFilterStashToolStripMenuItem.Click += restartFilterStashToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changeBackgroundToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // changeBackgroundToolStripMenuItem
            // 
            changeBackgroundToolStripMenuItem.Name = "changeBackgroundToolStripMenuItem";
            changeBackgroundToolStripMenuItem.Size = new Size(182, 22);
            changeBackgroundToolStripMenuItem.Text = "Change background";
            changeBackgroundToolStripMenuItem.Click += changeBackgroundToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { helpToolStripMenuItem, checkForUpdatesToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(180, 22);
            helpToolStripMenuItem.Text = "Help";
            helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(1174, 787);
            panel1.TabIndex = 2;
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            checkForUpdatesToolStripMenuItem.Size = new Size(180, 22);
            checkForUpdatesToolStripMenuItem.Text = "Check for updates";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1174, 811);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openPoE2FolderToolStripMenuItem;
        private ToolStripMenuItem openCacheFolderToolStripMenuItem;
        private ToolStripMenuItem restartFilterStashToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Panel panel1;
        private ToolStripMenuItem changeBackgroundToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
    }
}
