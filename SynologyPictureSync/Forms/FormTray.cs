using MediaSync.Extensions;
using MediaSync.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class FormTray : Form
    {
        public FormTray()
        {
            InitializeComponent();
            SetSyncCommandEnabledIfConfigValid();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var options = new FormOptions();
            options.OptionsUpdated += ConfigChangedEvent;
            options.Show();
        }

        public void ConfigChangedEvent(object sender, EventArgs e)
        {
            SetSyncCommandEnabledIfConfigValid();
        }

        private void syncNowToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var syncConfig = LoadSyncConfig();
            if (syncConfig != null)
            {
                Sync(syncConfig);
            }
        }

        private void SetSyncCommandEnabledIfConfigValid()
        {
            var syncConfig = SyncConfig.CreateFromFile();
            ConfigValidCheckResult validConfig = ValidateConfigCreateMissingConfigMessage(syncConfig);
            contextMenuStrip1.Items["syncNowToolStripMenuItem"].Enabled = (validConfig.HasValidConfig);
        }

        private ConfigValidCheckResult ValidateConfigCreateMissingConfigMessage(SyncConfig config)
        {
            var result = new ConfigValidCheckResult { HasValidConfig = true };

            if (config == null)
            {
                result.HasValidConfig = false;
            }
            else
            {
                if (config.DestinationDir.IsNullOrWhitespace())
                {
                    result.Message += "There isn't a destination media directory defined." + Environment.NewLine;
                    result.HasValidConfig = false;
                }

                if (config.SourceDir.IsNullOrWhitespace())
                {
                    result.Message += "There isn't a source media directory defined." + Environment.NewLine;
                    result.HasValidConfig = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Show the notification balloon in the system tray.
        /// </summary>
        /// <param name="msg">The message to show in the balloon.</param>
        /// <param name="displayTimeout">Number of seconds to show. If unspecified, it's 7 seconds.</param>
        private void ShowBalloon(string msg, int displayTimeout = 7)
        {
            notifyIcon1.ShowBalloonTip(timeout: displayTimeout, tipTitle: "Media Sync", tipText: msg, tipIcon: ToolTipIcon.None);
        }

        private SyncConfig LoadSyncConfig()
        {
            var syncConfig = SyncConfig.CreateFromFile();

            ConfigValidCheckResult validConfig = ValidateConfigCreateMissingConfigMessage(syncConfig);
            if (validConfig.HasValidConfig == false)
            {
                ShowBalloon(validConfig.Message);
                optionsToolStripMenuItem_Click(null, null);
                return null;
            }
            return syncConfig;
        }


        private void Sync(SyncConfig syncConfig)
        {
            //do a sanity check on the config to avoid exceptions while processing
            if (ValidateConfigCreateMissingConfigMessage(syncConfig).HasValidConfig == false)
            {
                return;
            }

            try
            {
                ShowBalloon("Finding items...");
                var fileSync = new FileSyncer();

                List<CopyTask> copyTasks = fileSync.BuildFileCopyTasks(syncConfig).ToList();

                ShowBalloon("Copying {0} items...".FormatWith(copyTasks.Count));

                SyncCopyResult copyResult = fileSync.ExecuteFileCopyTasks(copyTasks);

                DeleteSourceFilesIfRequired(syncConfig, copyTasks);

                ShowCompletionBalloon(copyResult, syncConfig.DestinationDir);

            }
            catch (Exception ex)
            {
                FileIOHelper.WriteToErrLog(ex.Message);
                ShowBalloon("Sorry we had a problem when syncing. The log file has more details. " + FileIOHelper.ErrLogFile, 10);
            }
        }
        private void DeleteSourceFilesIfRequired(SyncConfig syncConfig, IEnumerable<CopyTask> copyTasks)
        {
            if (syncConfig.ShouldDeleteSourceWhenSuccessfullyCompleted)
            {
                DialogResult shouldContinue = MessageBox.Show(text: "Ready to delete source files on {0}?".FormatWith(syncConfig.SourceDir), caption: "MediaSync", buttons: MessageBoxButtons.YesNo);

                if (shouldContinue == DialogResult.Yes)
                {
                    foreach (var item in copyTasks.Where(x => x.CopyResult == CopyResult.CopiedSuccessfully || x.CopyResult == CopyResult.AlreadyExisted))
                    {
                        DeleteSuccessfullyCopiedSourceItem(item.SourceFile);
                    }
                }
            }
        }
        private void ShowCompletionBalloon(SyncCopyResult result, string targetDir)
        {
            string completionMessage = CreateCompletedMessage(result, targetDir);
            ShowBalloon(completionMessage);
        }
        private void DeleteSuccessfullyCopiedSourceItem(string sourceFile)
        {
            try
            {
                string dir = Path.GetDirectoryName(sourceFile);
                File.Delete(sourceFile);
                DeleteDirectoryIfEmpty(dir);
            }
            catch (Exception) { }
        }

        private string CreateCompletedMessage(SyncCopyResult result, string targetDir)
        {
            var msg = new StringBuilder();
            msg.AppendLine("Done syncing to {0}.".FormatWith(targetDir));

            if (result.CopiedSuccessfullyCount > 0)
            {
                msg.AppendLine("{0} copied.".FormatWith(result.CopiedSuccessfullyCount));
            }

            if (result.AlreadyExistedCount > 0)
            {
                msg.AppendLine("{0} already existed.".FormatWith(result.AlreadyExistedCount));
            }

            if (result.CopiedButAlreadyExistedDiffSizeCount > 0)
            {
                msg.AppendLine("{0} new/changed files.".FormatWith(result.CopiedButAlreadyExistedDiffSizeCount));
            }
            if (result.UncopiedProblemCount > 0)
            {
                msg.AppendLine("{0} had problems copying.".FormatWith(result.UncopiedProblemCount));
            }

            return msg.ToString();
        }

        /// <summary>
        /// Deletes a directory if it's empty. A nice courtesy for the user.
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteDirectoryIfEmpty(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            if (di.GetFiles().Count() == 0 && di.GetDirectories().Count() == 0)
            {
                di.Delete();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout about = new FormAbout();
            about.Show();
        }
    }
}
