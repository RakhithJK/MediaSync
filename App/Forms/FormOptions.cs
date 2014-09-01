using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class FormOptions : Form
    {
        public event EventHandler OptionsUpdated;

        private const string ExtensionSeparators = ",;. ";
        private string[] SupportedTokens = { "year", "month", "day" };
        private FileIOHelper FileHelper;
        public FormOptions()
        {

            InitializeComponent();
            FileHelper = new FileIOHelper();
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
                ShouldLogDebug = chkLogOutput.Checked,
                FileExtensions = txtFileExtensions.Text.Trim().Split(ExtensionSeparators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()
            };
            syncConfig.Save();
            this.Close();
            OptionsUpdated(sender, e);
        }

        /// <summary>
        /// Take our best guess at options that would best suit the user.
        /// </summary>
        private void SetDefaultsWhenEmpty()
        {
            if (txtSourceDir.Text.Trim().Length == 0)
            {
                txtSourceDir.Text = Machine.MyPicturesDirectory;
            }
        }


        /// <summary>
        /// Set up this form for display for the user. Load config from disk and display to form elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormOptions_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            var syncConfig = SyncConfig.CreateFromFile();

            if (syncConfig != null)
            {
                WriteConfigToForm(syncConfig);
            }
        }

        /// <summary>
        /// Given the configuration options, write it to the form elements.
        /// </summary>
        /// <param name="syncConfig"></param>
        private void WriteConfigToForm(SyncConfig syncConfig)
        {
            txtSourceDir.Text = syncConfig.SourceDir.Trim();
            txtDestinationDir.Text = syncConfig.DestinationDir.Trim();
            txtFileExtensions.Text = string.Join(" ", syncConfig.FileExtensions);
            chkDeleteSourceAfterCopy.Checked = syncConfig.ShouldDeleteSourceWhenSuccessfullyCompleted;
            chkWarnOnDelete.Checked = syncConfig.ShouldWarnOnDelete;
            chkLogOutput.Checked = syncConfig.ShouldLogDebug;
        }

        /// <summary>
        /// When the user leaves the source directory textbox, ensure that the text is a good dir. Good == directory exists, and is *readable* by this user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            bool canRead = FileHelper.CanReadDir(txtSourceDir.Text);
            if (canRead == false)
            {
                lblMsg.Text = "You don't have read permissions on the source directory.";
                txtSourceDir.Focus();
            }
            btnSave.Enabled = canRead;
        }

        /// <summary>
        /// When the user leaves the destination directory textbox, ensure that the text is a good dir. Good == directory exists, and is *writable* by this user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DestLeave(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            lblMsg.Text = "";

            txtDestinationDir.Text = txtDestinationDir.Text.Trim();
            if (txtDestinationDir.Text.Length == 0)
            {
                return;
            }

            IEnumerable<string> userTokens = ReplacementTokens(txtDestinationDir.Text);

            if (userTokens.Any())
            {
                var unsupportedTokens = userTokens.Except(SupportedTokens);
                if (unsupportedTokens.Any())
                {
                    lblMsg.Text = "There are some tokens we don't recognize. " + string.Join(" - ", unsupportedTokens);
                    txtDestinationDir.Focus();
                    btnSave.Enabled = false;
                }
            }


            string baseTargetDirectory = GetDirWithoutTokens(txtDestinationDir.Text);

            DirectoryInfo f = new DirectoryInfo(baseTargetDirectory);
            if (f.Exists == false)
            {
                lblMsg.Text = string.Format("The destination directory '{0}'doesn't exist.",baseTargetDirectory);
                txtDestinationDir.Focus();
                return;
            }
            bool canWrite = FileHelper.CanWriteDir(baseTargetDirectory);
            if (canWrite == false)
            {
                lblMsg.Text = string.Format("We can't write to the destination directory '{0}'. Please check that you have permissions.", baseTargetDirectory);
                txtDestinationDir.Focus();
            }
            btnSave.Enabled = canWrite;
        }

        public string GetDirWithoutTokens(string path)
        {
            const string PathSep = @"\";
            string[] inputPathParts = path.Split(PathSep.ToCharArray());

            string basePath = "";

            foreach (var p in inputPathParts)
            {
                if (p.Contains("{"))
                {
                    break;
                }
                basePath += p + PathSep;
            }
            //clean up trailing slashes
            DirectoryInfo d = new DirectoryInfo(basePath);
            return d.FullName;
        }

        public IEnumerable<string> ReplacementTokens(string input)
        {
            var regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            return regex.Matches(input).Cast<Match>().Select(m => m.Value).Distinct().ToList();
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
