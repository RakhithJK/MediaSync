using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaSync.Extensions;

namespace MediaSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Sync();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void syncNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sync();
        }

        private string MyPicturesDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); }
        }

        /// <summary>
        /// Builds the target directory in the user's pref. Like year/monthname/date/file.jpg
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

            string targetPath = Path.Combine(pathParts.ToArray());

            return targetPath;
        }

        private void Sync()
        {
            //load options
            var syncConfig = SyncConfig.CreateFromFile();

            //perform the sync
            try
            {
                var fileSync = new FileSyncer();
                syncConfig.SourceDir = syncConfig.SourceDir.Or(MyPicturesDirectory);
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
                    foreach (var item in copyTasks.Where(x=>x.WasCopiedSuccessfully))
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
            if(di.GetFiles().Count()==0 && di.GetDirectories().Count()==0)
            {
             //   di.Delete();

            }
        }



    }
}
