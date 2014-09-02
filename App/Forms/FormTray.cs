using MediaSync.Extensions;
using MediaSync.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class FormTray : Form
    {
        public FileIOHelper FileHelper { get; set; }
        public DateTime? LastRunOn { get; set; }
        public FormTray()
        {
            InitializeComponent();
            SetSyncCommandEnabledIfConfigValid();
            FileHelper = new FileIOHelper();
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

        public void SetLastRun()
        {
            lastRunToolStripMenuItem.Visible = LastRunOn.HasValue;
            if (LastRunOn.HasValue)
            {
                string lastNotify = "Last sync was {0}".FormatWith(LastRunOn.Value.ToRelativeDateString());
                notifyIcon1.Text =  
                    lastRunToolStripMenuItem.Text = lastNotify;
                

            }
        }

        private void syncNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var syncConfig = LoadSyncConfig();
            if (syncConfig != null)
            {
                if (syncConfig.ShouldLogDebug)
                {
                    Task.Run(async () =>
                    {
                        await FileHelper.WriteToErrLogAsync("Starting sync.");
                    });
                }
                Task.Run(async () =>
                {
                    await Sync(syncConfig);
                    System.Media.SystemSounds.Hand.Play();
                    SetLastRun();
                });
            }
        }

        /// <summary>
        /// Check the 
        /// </summary>
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

        private void sysTrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            SetLastRun();
        }


        private async Task Sync(SyncConfig syncConfig)
        {
            bool shouldLog = syncConfig.ShouldLogDebug;

            //do a sanity check on the config to avoid exceptions while processing
            if (ValidateConfigCreateMissingConfigMessage(syncConfig).HasValidConfig == false)
            {
                return;
            }
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                ShowBalloon("Finding items...");
                LastRunOn = DateTime.Now;
                stopwatch.Start();
                var fileSync = new FileSyncer(syncConfig);
                var copyResult = new SyncCopyResult();
                List<CopyTask> copyTasks = fileSync.BuildFileCopyTasks().ToList();
                if (shouldLog) { await FileHelper.WriteToErrLogAsync("Found {0} items to sync.".FormatWith(copyTasks.Count)); }
                if (copyTasks.Any())
                {
                    ShowBalloon("Copying {0} items...".FormatWith(copyTasks.Count));

                    copyResult = await fileSync.ExecuteFileCopyTasks(copyTasks);
                    if (shouldLog) { await FileHelper.WriteToErrLogAsync("Completed file copy tasks"); }

                    await DeleteSourceFilesIfRequired(syncConfig, copyTasks);

                }
                stopwatch.Stop();
                copyResult.TimeElapsedMsg = "Total sync time was {0}:{1}".FormatWith(
                    stopwatch.Elapsed.Minutes.ToString("00"),
                    stopwatch.Elapsed.Seconds.ToString("00"));
                if (shouldLog) { await FileHelper.WriteToErrLogAsync(copyResult.TimeElapsedMsg); }

                ShowCompletionBalloon(copyResult, syncConfig.DestinationDir);

            }
            catch (Exception ex)
            {
                var fileIOHelper = new FileIOHelper();
                fileIOHelper.WriteToErrLog(ex.Message);
                ShowBalloon("Sorry we had a problem when syncing. The log file has more details. " + FileIOHelper.OutputLogFile, 10);
            }
        }
        private async Task DeleteSourceFilesIfRequired(SyncConfig syncConfig, IEnumerable<CopyTask> copyTasks)
        {
            if (syncConfig.ShouldDeleteSourceWhenSuccessfullyCompleted)
            {
                DialogResult shouldContinue = MessageBox.Show(text: "Ready to delete source files on {0}?".FormatWith(syncConfig.SourceDir), caption: "MediaSync", buttons: MessageBoxButtons.YesNo);

                if (shouldContinue == DialogResult.Yes)
                {
                    var itemsToDelete = copyTasks.Where(x =>
                        x.CopyResult == CopyResult.CopiedSuccessfully ||
                        x.CopyResult == CopyResult.AlreadyExisted)
                        .Select(x => x.SourceFile);
                    await DeleteSourceFiles(itemsToDelete, syncConfig.ShouldLogDebug);
                }
            }
        }

        public async Task DeleteSourceFiles(IEnumerable<string> sourceFiles, bool writeLogEntryWhenDeleted)
        {
            foreach (string sourceFile in sourceFiles)
            {
                DeleteSuccessfullyCopiedSourceItem(sourceFile);
                if (writeLogEntryWhenDeleted) { await FileHelper.WriteToErrLogAsync("Deleted {0}".FormatWith(sourceFile)); }
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
            msg.AppendLine("{0} copied.".FormatWith(result.CopiedSuccessfullyCount));
            msg.AppendLine(result.TimeElapsedMsg);

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

        private void lastRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBalloon("");
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetLastRun();
        }
    }
}
