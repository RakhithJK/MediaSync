namespace MediaSync
{
    partial class FormAbout
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.lblBuildDateTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lnkGitHubLocation = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBuildDateTitle
            // 
            this.lblBuildDateTitle.AutoSize = true;
            this.lblBuildDateTitle.Location = new System.Drawing.Point(9, 31);
            this.lblBuildDateTitle.Name = "lblBuildDateTitle";
            this.lblBuildDateTitle.Size = new System.Drawing.Size(59, 13);
            this.lblBuildDateTitle.TabIndex = 2;
            this.lblBuildDateTitle.Text = "Build Date:";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(9, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 13);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "Version:";
            // 
            // lnkGitHubLocation
            // 
            this.lnkGitHubLocation.AutoSize = true;
            this.lnkGitHubLocation.Location = new System.Drawing.Point(36, 62);
            this.lnkGitHubLocation.Name = "lnkGitHubLocation";
            this.lnkGitHubLocation.Size = new System.Drawing.Size(206, 13);
            this.lnkGitHubLocation.TabIndex = 4;
            this.lnkGitHubLocation.TabStop = true;
            this.lnkGitHubLocation.Text = "https://github.com/philoushka/mediasync";
            this.lnkGitHubLocation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGitHubLocation_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Location = new System.Drawing.Point(12, 95);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 129);
            this.Controls.Add(this.lnkGitHubLocation);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblBuildDateTitle);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.Text = "About";
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBuildDateTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel lnkGitHubLocation;
        private System.Windows.Forms.Button btnClose;
    }
}