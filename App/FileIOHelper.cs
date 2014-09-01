using MediaSync.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaSync
{
    public class FileIOHelper
    {
        /// <summary>
        /// Given a file path, ensure its directory exists.
        /// </summary>
        /// <param name="filePath">The full file path of the file to be created.</param>
        public void CreateDirectoryForFile(string filePath)
        {
            string dir = new FileInfo(filePath).DirectoryName;
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

        }

        /// <summary>
        /// Determine whether there's another file that has a name that starts with this file's name and is in the same directory as this file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool DestinationDirHasFileNameAndSize(FileInfo file)
        {
            return file.Directory
                       .GetFiles()
                       .Any(x => x.Name.StartsWith(Path.GetFileNameWithoutExtension(file.Name)) &&
                                 x.Length == file.Length);
        }

        /// <summary>
        /// Given a directory, get all the files in subdirectories that you can read within.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileExtensions"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetAllFilesWithExtensions(string directory, IEnumerable<string> fileExtensions)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(directory);

            var matchingFiles = fileExtensions
                     .SelectMany(fileExt => sourceDir.GetFiles("*." + fileExt, SearchOption.TopDirectoryOnly))
                     .Distinct();
            return matchingFiles;
        }

        /// <summary>
        /// Writes your string to the log file in the app directory on disk. Prefixes with current datetime.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToErrLog(string msg)
        {
            msg = "{0}\t{1}{2}".FormatWith(DateTime.Now.ToString("yyyy-MMM-dd HH:mm"), msg, Environment.NewLine);
            using (var file = File.AppendText(OutputLogFile))
            {
                file.Write(msg);
            }
        }

        public async Task WriteToErrLogAsync(string msg)
        {
            msg = "{0}\t{1}{2}".FormatWith(DateTime.Now.ToString("yyyy-MMM-dd HH:mm"), msg, Environment.NewLine);
            using (StreamWriter writer = File.AppendText(OutputLogFile))
            {
                await writer.WriteAsync(msg);
            }
        }

        /// <summary>
        /// Determine whether the given directory is readable in the current security context. Can be a drive letter or UNC path.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>Whether the directory is readable</returns>
        public bool CanReadDir(string dir)
        {
            try
            {
                Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Determine whether the directory is writable.
        /// </summary>
        /// <param name="dir">The directory path to test for writability in the current security context. Can be a drive letter or UNC path.</param>
        /// <returns>Whether the directory is writable</returns>
        public bool CanWriteDir(string dir)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dir, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }

                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Copy a file from one location to another.
        /// </summary>
        /// <param name="copyTask"></param>
        /// <returns></returns>
        public async Task CopyFile(CopyTask copyTask)
        {
            byte[] fileBytes = File.ReadAllBytes(copyTask.SourceFile);

            using (FileStream destStream = new FileStream(copyTask.DestinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.Write, bufferSize: 4096, useAsync: true))
            {
                await destStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            };
        }

        /// <summary>
        /// The location of the error/debug log file.
        /// </summary>
        public static string OutputLogFile { get { return Path.Combine(AppDir, "Output.txt"); } }

        /// <summary>
        /// The location of our writable local dir for this app's usage.
        /// </summary>
        public static string AppDir { get { return Path.Combine(Machine.AppDataDirectory, Assembly.GetEntryAssembly().GetName().Name); } }
    }
}
