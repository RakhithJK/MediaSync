using MediaSync.Extensions;
using MediaSync.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaSync
{
    public class FileSyncer
    {
        private SyncConfig Config;
        private FileIOHelper FileHelper;

        public FileSyncer(SyncConfig config)
        {
            Config = config;
            FileHelper = new FileIOHelper();
        }

        /// <summary>
        /// Find all files in the source directory, and create a collection of CopyTasks. Analyzes the destination for each file, determines those that have been previously copied and should be skipped. Determines the destination file name.
        /// </summary>
        /// <returns>A list of CopyTask objects. </returns>
        public IEnumerable<CopyTask> BuildFileCopyTasks()
        {
            var allMatchingFilesInDir = FileHelper.GetAllFilesWithExtensions(Config.SourceDir, Config.FileExtensions);

            foreach (var mediaFile in allMatchingFilesInDir)
            {
                var task = new CopyTask
                {
                    SourceFile = mediaFile.FullName,
                    ImageTakenOnDate = MediaComparer.GetImageTakenOnDate(mediaFile.FullName),
                    FileCreatedOn = mediaFile.CreationTime,
                };
                BuildDestinationForItem(task);
                yield return task;
            }
        }

        /// <summary>
        /// Ensure we have a good destination name and determine whether that file exists already.
        /// </summary>
        /// <param name="task"></param>
        public void BuildDestinationForItem(CopyTask task)
        {
            string targetDirFileName = DetermineTargetDir(task);
            task.DestinationFile = EnsureUniqueDestinationFileName(targetDirFileName);
            task.FileExistsAlready = File.Exists(task.DestinationFile);
        }

        /// <summary>
        /// Checks whether the destination file exists. If not, then returns the combined dir and file.
        /// If it exists, it appends the file size to the file name. You should further check if this modified file name exists.
        /// </summary>
        /// <param name="destinationDir"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private string EnsureUniqueDestinationFileName(string destinationFile)
        {
            string fullFilePath = Path.Combine(Config.DestinationDir, destinationFile);

            var file = new FileInfo(fullFilePath);
            if (file.Exists)
            {
                if (FileHelper.DestinationDirHasFileNameAndSize(file))
                {
                    return fullFilePath;
                }

                return Path.Combine(file.DirectoryName, "{0}-{1}{2}".FormatWith(file.Name, file.Length, file.Extension));
            }
            return fullFilePath;
        }


        /// <summary>
        /// Builds the target file path directory in the user's pref. Like year/monthname/date/file.jpg. Take it from the config.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private string DetermineTargetDir(CopyTask task)
        {
            DateTime targetDate = task.ImageTakenOnDate.GetValueOrDefault(task.FileCreatedOn);
            var replacements = new Dictionary<string, string> 
            {             
                 {"{year}", targetDate.Year.ToString()},
                 {"{month}",  targetDate.ToShortMonthName()},
                 {"{day}", targetDate.Day.ToString()}
            };

            string destinationDir = Config.DestinationDir;

            foreach (var item in replacements)
            {
                destinationDir = destinationDir.Replace(item.Key, item.Value);
            }

            return Path.Combine(destinationDir, Path.GetFileName(task.SourceFile));
        }

        /// <summary>
        /// Execute a set of copy tasks.
        /// </summary>
        /// <param name="copyTasks"></param>
        /// <returns>The status of the copy tasks. A set of counts of success, fail, etc.</returns>
        public async Task<SyncCopyResult> ExecuteFileCopyTasks(IEnumerable<CopyTask> copyTasks)
        {
            var syncResult = new SyncCopyResult();
            syncResult.AlreadyExistedCount = copyTasks.Count(x => x.FileExistsAlready);
            foreach (var copyTask in copyTasks.Where(x => x.FileExistsAlready == false))
            {
                try
                {
                    FileHelper.CreateDirectoryForFile(copyTask.DestinationFile);

                    await FileHelper.CopyFile(copyTask);
                    copyTask.FileCopiedOn = DateTime.Now;
                    syncResult.CopiedSuccessfullyCount += 1;
                }
                catch (Exception e)
                {
                    copyTask.CopyStatus = e.Message;
                    syncResult.UncopiedProblemCount += 1;
                }
            }
            return syncResult;
        }
    }
}
