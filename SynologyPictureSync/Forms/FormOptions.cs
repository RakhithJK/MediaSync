using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class FormOptions : Form
    {
        public event EventHandler OptionsUpdated;

        private const string ExtensionSeparators = ",;. ";

        public FormOptions()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SetDefaultsWhenEmpty();

            var syncConfig = new SyncConfig
            {
                DestinationDir = txtDestinationDir.Text.Trim(),
                SourceDir = txtSourceDir.Text.Trim(),
                ShouldDeleteSourceWhenSuccessfullyCompleted = chkDeleteSourceAfterCopy.Checked,
                ShouldWarnOnDelete = chkWarnOnDelete.Checked,
                FileExtensions = txtFileExtensions.Text.Trim().Split(ExtensionSeparators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()
            };
            syncConfig.Save();
            this.Close();
            OptionsUpdated(sender, e);

        }

        private void SetDefaultsWhenEmpty()
        {
            if (txtSourceDir.Text.Trim().Length == 0)
            {
                txtSourceDir.Text = Machine.MyPicturesDirectory;
            }
        }

      

        private void FormOptions_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            var syncConfig = SyncConfig.CreateFromFile();

            if (syncConfig != null)
            {
                WriteConfigToForm(syncConfig);
            }

        }

        private void WriteConfigToForm(SyncConfig syncConfig)
        {
            txtSourceDir.Text = syncConfig.SourceDir.Trim();
            txtDestinationDir.Text = syncConfig.DestinationDir.Trim();
            txtFileExtensions.Text = string.Join(" ", syncConfig.FileExtensions);
            chkDeleteSourceAfterCopy.Checked = syncConfig.ShouldDeleteSourceWhenSuccessfullyCompleted;
            chkWarnOnDelete.Checked = syncConfig.ShouldWarnOnDelete;
        }

        private void SourceLeave(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            txtSourceDir.Text = txtSourceDir.Text.Trim();
            if (txtSourceDir.Text.Length == 0)
            {
                return;
            }
            DirectoryInfo f = new DirectoryInfo(txtSourceDir.Text);
            if (f.Exists == false)
            {
                lblMsg.Text = "The source directory doesn't exist.";
                btnSave.Enabled = f.Exists;
                txtSourceDir.Focus();
                return;
            }

            bool canRead = FileIOHelper.CanReadDir(txtSourceDir.Text);
            if (canRead == false)
            {
                lblMsg.Text = "You don't have read permissions on the source directory.";
                txtSourceDir.Focus();
            }
            btnSave.Enabled = canRead;
        }

        private void DestLeave(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            lblMsg.Text = "";

            txtDestinationDir.Text = txtDestinationDir.Text.Trim();
            if (txtDestinationDir.Text.Length == 0)
            {
                return;
            }
            DirectoryInfo f = new DirectoryInfo(txtDestinationDir.Text);
            if (f.Exists == false)
            {
                lblMsg.Text = "The destination directory doesn't exist.";
                txtDestinationDir.Focus();
                return;
            }
            bool canWrite = FileIOHelper.CanWriteDir(txtDestinationDir.Text);
            if (canWrite == false)
            {
                lblMsg.Text = "We can't write to the destination directory. Please check that you have permissions.";
                txtDestinationDir.Focus();
            }
            btnSave.Enabled = canWrite;
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowNewFolderButton = true;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtSourceDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnBrowseDest_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowNewFolderButton = true;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtDestinationDir.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
