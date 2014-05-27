using MediaSync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Sync();
        }

        private void SetSyncCommandEnabledIfConfigValid()
        {
            var syncConfig = SyncConfig.CreateFromFile();
            ConfigValidCheckResult validConfig = CreateMissingConfigMessage(syncConfig);
            contextMenuStrip1.Items["syncNowToolStripMenuItem"].Enabled = (validConfig.HasValidConfig);
        }

        /// <summary>
        /// Builds the target file path directory in the user's pref. Like year/monthname/date/file.jpg
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private string DetermineTargetDir(CopyTask task)
        {
            DateTime targetDate = task.ImageTakenOnDate.GetValueOrDefault(task.FileCreatedOn);

            List<string> pathParts = new List<string>{
                targetDate.Year.ToString(),
                targetDate.ToShortMonthName(),
                targetDate.Day.ToString(),
                System.IO.Path.GetFileName(task.SourceFile)
            };

            return Path.Combine(pathParts.ToArray());
        }

        private ConfigValidCheckResult CreateMissingConfigMessage(SyncConfig config)
        {
            var result = new ConfigValidCheckResult { HasValidConfig = true };

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

            return result;
        }

        private void ShowBalloon(string msg)
        {
            notifyIcon1.ShowBalloonTip(timeout: 7, tipTitle: "Media Sync", tipText: msg, tipIcon: ToolTipIcon.None);
        }

        private void Sync()
        {
            ShowBalloon("Finding items...");
            //load options
            var syncConfig = SyncConfig.CreateFromFile();

            ConfigValidCheckResult validConfig = CreateMissingConfigMessage(syncConfig);
            if (validConfig.HasValidConfig == false)
            {
                MessageBox.Show(validConfig.Message);
                optionsToolStripMenuItem_Click(null, null);
                return;
            }

            try
            {
                var fileSync = new FileSyncer();

                List<CopyTask> copyTasks = fileSync.FindFileCopyTasks(new DirectoryInfo(syncConfig.SourceDir), syncConfig.FileExtensions).ToList();
                foreach (var item in copyTasks)
                {
                    string targetDirFile = DetermineTargetDir(item);
                    item.DestinationFile = Path.Combine(syncConfig.DestinationDir, targetDirFile);
                }
                ShowBalloon("Copying {0} items...".FormatWith(copyTasks.Count));

                fileSync.ExecuteFileCopyTasks(copyTasks);

                if (syncConfig.ShouldDeleteSourceWhenSuccessfullyCompleted)
                {
                    foreach (var item in copyTasks.Where(x => x.WasCopiedSuccessfully))
                    {
                        string dir = Path.GetDirectoryName(item.SourceFile);
                        File.Delete(item.SourceFile);
                        DeleteDirectoryIfEmpty(dir);
                    }
                }
                ShowBalloon(CreateCompletedMessage(copyTasks, syncConfig.DestinationDir));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CreateCompletedMessage(IEnumerable<CopyTask> copyTasks, string targetDir)
        {
            int numCopied = copyTasks.Count(x => x.WasCopiedSuccessfully);
            return "Done. {0} file{1} sync'd to {2}".FormatWith(numCopied,
                (numCopied > 1) ? "s" : "",
                targetDir);
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
