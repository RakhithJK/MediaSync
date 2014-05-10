using MediaSync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaSync.Extensions;

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
            List<string> pathParts = new List<string>{
                task.CreatedOn.Year.ToString(),
                task.CreatedOn.ToShortMonthName(),
                task.CreatedOn.Day.ToString(),
                System.IO.Path.GetFileName(task.SourceFile)
            };

            return Path.Combine(pathParts.ToArray());
        }

        private ConfigValidCheckResult CreateMissingConfigMessage(SyncConfig config)
        {
            var result = new ConfigValidCheckResult { HasValidConfig=true};
            
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

        private void Sync()
        {
            //load options
            var syncConfig = SyncConfig.CreateFromFile();

            ConfigValidCheckResult validConfig = CreateMissingConfigMessage(syncConfig);
            if (validConfig.HasValidConfig==false)
            {
                MessageBox.Show(validConfig.Message);
                optionsToolStripMenuItem_Click(null, null);
                return;
            }

            //perform the sync
            try
            {
                var fileSync = new FileSyncer();
                syncConfig.SourceDir = syncConfig.SourceDir.Or(Machine.MyPicturesDirectory);
                syncConfig.DestinationDir = syncConfig.DestinationDir.Or(@"x:\");
                List<CopyTask> copyTasks = fileSync.FindFileCopyTasks(new DirectoryInfo(syncConfig.SourceDir)).ToList();
                foreach (var item in copyTasks)
                {
                    string targetDirFile = DetermineTargetDir(item);
                    item.DestinationFile = Path.Combine(syncConfig.DestinationDir, targetDirFile);
                }

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
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DeleteDirectoryIfEmpty(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            if (di.GetFiles().Count() == 0 && di.GetDirectories().Count() == 0)
            {
                //   di.Delete();
            }
        }
    }
}
