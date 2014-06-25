using MediaSync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MediaSync
{
    public class FileIOHelper
    {
        public static bool FileSizesAreSame(string file1, string file2)
        {
            try
            {
                FileInfo f1 = new FileInfo(file1);
                FileInfo f2 = new FileInfo(file2);

                return f1.Length.Equals(f2.Length);
            }
            catch (Exception) { }
            return false;
        }

        public static void CreateDirectoryForFile(string filePath)
        {
            string dir = new FileInfo(filePath).DirectoryName;
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Determine whether there's another file that:
        /// - has a name that starts with this file's name.
        /// - is in the same directory as this file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DestinationDirHasFileNameAndSize(FileInfo file)
        {
            return file.Directory
                        .GetFiles()
                        .Any(x => x.Name.StartsWith(Path.GetFileNameWithoutExtension(file.Name)) &&
                                  x.Length == file.Length);

        }
        public static IEnumerable<FileInfo> GetAllFilesWithExtensions(string directory, IEnumerable<string> fileExtensions)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(directory);

            return fileExtensions
                    .SelectMany(fileExt => sourceDir.GetFiles("*." + fileExt, SearchOption.AllDirectories))

                    .Distinct()
                    .ToList();
        }

        /// <summary>
        /// Writes your string to the log file in the app directory on disk. Prefixes with current datetime.
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteToErrLog(string msg)
        {
            msg = "{0}\t{1}".FormatWith(DateTime.Now.ToString("yyyy-MMM-dd HH:mm"), msg);
            using (var file = File.AppendText(ErrLogFile))
            {
                file.Write(msg);
            }
        }

        public static bool CanReadDir(string dir)
        {
            try
            {
                Directory.GetFiles(dir);
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool CanWriteDir(string dir)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dir, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }

                return true;
            }
            catch (Exception) { return false; }
        }

        public static string ErrLogFile { get { return Path.Combine(AppDir, "ErrorLog.txt"); } }
        public static string AppDir { get { return Path.Combine(Machine.AppDataDirectory, Assembly.GetEntryAssembly().GetName().Name); } }
    }
}
