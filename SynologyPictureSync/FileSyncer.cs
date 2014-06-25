using MediaSync.Extensions;
using MediaSync.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaSync
{
    public class FileSyncer
    {
        public IEnumerable<CopyTask> BuildFileCopyTasks(SyncConfig config)
        {
            var allMatchingFilesInDir = FileIOHelper.GetAllFilesWithExtensions(config.SourceDir, config.FileExtensions);

            foreach (var mediaFile in allMatchingFilesInDir)
            {
                var task = new CopyTask
                {
                    SourceFile = mediaFile.FullName,
                    ImageTakenOnDate = MediaComparer.GetImageTakenOnDate(mediaFile.FullName),
                    FileCreatedOn = mediaFile.CreationTime,
                };
                BuildDestinationForItem(task, config.DestinationDir);
                yield return task;
            }
        }

        public void BuildDestinationForItem(CopyTask task, string destinationDir)
        {
            string targetDirFileName = DetermineTargetDir(task);
            task.DestinationFile = EnsureUniqueDestinationFileName(destinationDir, targetDirFileName);
            task.FileExistsAlready = File.Exists(task.DestinationFile);
        }

        /// <summary>
        /// Checks whether the destination file exists. If not, then returns the combined dir and file.
        /// If it exists, it appends the file size to the file name. You should further check if this modified file name exists.
        /// </summary>
        /// <param name="destinationDir"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private string EnsureUniqueDestinationFileName(string destinationDir, string destinationFile)
        {
            string fullFilePath = Path.Combine(destinationDir, destinationFile);

            var file = new FileInfo(fullFilePath);
            if (file.Exists)
            {
                if (FileIOHelper.DestinationDirHasFileNameAndSize(file))
                {
                    return fullFilePath;
                }

                return Path.Combine(file.DirectoryName, "{0}-{1}{2}".FormatWith(file.Name, file.Length, file.Extension));
            }
            return fullFilePath;
        }


        /// <summary>
        /// Builds the target file path directory in the user's pref. Like year/monthname/date/file.jpg. 
        /// TODO make this a config item isntead of hard coded.
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

        public SyncCopyResult ExecuteFileCopyTasks(IEnumerable<CopyTask> copyTasks)
        {
            var syncResult = new SyncCopyResult();
            syncResult.AlreadyExistedCount = copyTasks.Count(x => x.FileExistsAlready);
            foreach (var item in copyTasks.Where(x => x.FileExistsAlready == false))
            {
                try
                {
                    EnsureTargetDirectoryExists(item);
                    File.Copy(item.SourceFile, item.DestinationFile);
                    item.FileCopiedOn = DateTime.Now;
                    item.CopyStatus = "Successfully copied to destination.";
                    syncResult.CopiedSuccessfullyCount += 1;
                }
                catch (Exception e)
                {
                    item.CopyStatus = e.Message;
                    syncResult.UncopiedProblemCount += 1;
                }
            }
            return syncResult;
        }

        private static void EnsureTargetDirectoryExists(CopyTask item)
        {
            FileIOHelper.CreateDirectoryForFile(item.DestinationFile);
        }
    }
}
