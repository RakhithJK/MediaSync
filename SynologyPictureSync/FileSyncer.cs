using MediaSync.Extensions;
using MediaSync.Model;
using System;
using System.Collections.Generic;
using System.IO;

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
            string targetDirFile = DetermineTargetDir(task);
            task.DestinationFile = Path.Combine(destinationDir, targetDirFile);
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

        public SyncCopyResult ExecuteFileCopyTasks(IEnumerable<CopyTask> copyTasks)
        {
            var syncResult = new SyncCopyResult();
            foreach (var item in copyTasks)
            {
                try
                {
                    FileIOHelper.CreateDirectoryForFile(item.DestinationFile);

                    if (File.Exists(item.DestinationFile))
                    {
                        if (FileIOHelper.FileSizesAreSame(item.SourceFile, item.DestinationFile))
                        {
                            item.CopyStatus = "File Already Exists With Same Size";
                            syncResult.AlreadyExistedCount += 1;
                        }
                        else
                        {
                            item.CopyStatus = "File Already Exists - Different Sizes. Trying another .";
                            item.DestinationFile = ModifyDestinationFileNameForExistingSizeMismatch(item.DestinationFile);
                            syncResult.CopiedButAlreadyExistedDiffSizeCount += 1;
                        }
                        continue;
                    }
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

        private string ModifyDestinationFileNameForExistingSizeMismatch(string destinationFile)
        {
            var file = new FileInfo(destinationFile);
            string path = file.DirectoryName;
            int counter = 1;
            string newFileName = Path.Combine(path, "{0}-{1}".FormatWith(counter, file.Name));
            while (File.Exists(newFileName) && counter < int.MaxValue)
            {
                counter += 1;
                newFileName = Path.Combine(path, "{0}-{1}".FormatWith(counter, file.Name));
            }
            return newFileName;
        }


    }
}
