namespace MediaSync
{
    partial class FormOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptions));
            this.btnSave = new System.Windows.Forms.Button();
            this.chkDeleteSourceAfterCopy = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceDir = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.btnBrowseDest = new System.Windows.Forms.Button();
            this.txtDestinationDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.txtFileExtensions = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkWarnOnDelete = new System.Windows.Forms.CheckBox();
            this.lblDestinationHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(381, 169);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkDeleteSourceAfterCopy
            // 
            this.chkDeleteSourceAfterCopy.AutoSize = true;
            this.chkDeleteSourceAfterCopy.Location = new System.Drawing.Point(154, 14);
            this.chkDeleteSourceAfterCopy.Name = "chkDeleteSourceAfterCopy";
            this.chkDeleteSourceAfterCopy.Size = new System.Drawing.Size(238, 17);
            this.chkDeleteSourceAfterCopy.TabIndex = 1;
            this.chkDeleteSourceAfterCopy.Text = "Delete Source Images After Successful Copy";
            this.chkDeleteSourceAfterCopy.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source Media Directory";
            // 
            // txtSourceDir
            // 
            this.txtSourceDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceDir.Location = new System.Drawing.Point(154, 59);
            this.txtSourceDir.Name = "txtSourceDir";
            this.txtSourceDir.Size = new System.Drawing.Size(266, 20);
            this.txtSourceDir.TabIndex = 3;
            this.txtSourceDir.Leave += new System.EventHandler(this.SourceLeave);
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSource.Location = new System.Drawing.Point(426, 59);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(30, 23);
            this.btnBrowseSource.TabIndex = 4;
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // btnBrowseDest
            // 
            this.btnBrowseDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDest.Location = new System.Drawing.Point(426, 91);
            this.btnBrowseDest.Name = "btnBrowseDest";
            this.btnBrowseDest.Size = new System.Drawing.Size(30, 23);
            this.btnBrowseDest.TabIndex = 7;
            this.btnBrowseDest.Text = "...";
            this.btnBrowseDest.UseVisualStyleBackColor = true;
            this.btnBrowseDest.Click += new System.EventHandler(this.btnBrowseDest_Click);
            // 
            // txtDestinationDir
            // 
            this.txtDestinationDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestinationDir.Location = new System.Drawing.Point(154, 93);
            this.txtDestinationDir.Name = "txtDestinationDir";
            this.txtDestinationDir.Size = new System.Drawing.Size(266, 20);
            this.txtDestinationDir.TabIndex = 6;
            this.txtDestinationDir.Leave += new System.EventHandler(this.DestLeave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Destination Directory";
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lblMsg.Location = new System.Drawing.Point(12, 156);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(36, 13);
            this.lblMsg.TabIndex = 8;
            this.lblMsg.Text = "lblmsg";
            // 
            // txtFileExtensions
            // 
            this.txtFileExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileExtensions.Location = new System.Drawing.Point(154, 135);
            this.txtFileExtensions.Name = "txtFileExtensions";
            this.txtFileExtensions.Size = new System.Drawing.Size(266, 20);
            this.txtFileExtensions.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "File Extensions";
            // 
            // chkWarnOnDelete
            // 
            this.chkWarnOnDelete.AutoSize = true;
            this.chkWarnOnDelete.Location = new System.Drawing.Point(154, 37);
            this.chkWarnOnDelete.Name = "chkWarnOnDelete";
            this.chkWarnOnDelete.Size = new System.Drawing.Size(175, 17);
            this.chkWarnOnDelete.TabIndex = 11;
            this.chkWarnOnDelete.Text = "Warn Me Once Before Deleting";
            this.chkWarnOnDelete.UseVisualStyleBackColor = true;
            // 
            // lblDestinationHint
            // 
            this.lblDestinationHint.AutoSize = true;
            this.lblDestinationHint.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lblDestinationHint.Location = new System.Drawing.Point(156, 117);
            this.lblDestinationHint.Name = "lblDestinationHint";
            this.lblDestinationHint.Size = new System.Drawing.Size(222, 13);
            this.lblDestinationHint.TabIndex = 12;
            this.lblDestinationHint.Text = "You can use tokens like {year}\\(month}{day}";

            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 204);
            this.Controls.Add(this.lblDestinationHint);
            this.Controls.Add(this.chkWarnOnDelete);
            this.Controls.Add(this.txtFileExtensions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.btnBrowseDest);
            this.Controls.Add(this.txtDestinationDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowseSource);
            this.Controls.Add(this.txtSourceDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkDeleteSourceAfterCopy);
            this.Controls.Add(this.btnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptions";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.FormOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkDeleteSourceAfterCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Button btnBrowseDest;
        private System.Windows.Forms.TextBox txtDestinationDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.TextBox txtFileExtensions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkWarnOnDelete;
        private System.Windows.Forms.Label lblDestinationHint;
    }
}