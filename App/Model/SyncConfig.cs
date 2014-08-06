using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaSync
{
    [Serializable]
    public class SyncConfig
    {
        public static string[] DefaultFileExtensions = new[] { "jpg", "jpeg", "png", "avi", "mov", "mp4", "m4v", "mpg", "mpeg" };
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }

        public bool ShouldDeleteSourceWhenSuccessfullyCompleted { get; set; }
        public bool ShouldWarnOnDelete { get; set; }

        /// <summary>
        /// File extensions to filter for sync. When empty, sync none.
        /// </summary>
        public List<string> FileExtensions { get; set; }

        private static string SaveFile
        {
            get
            {
                return Path.Combine(FileIOHelper.AppDir, "SyncConfig.json");
            }
        }

        public SyncConfig() { }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                EnsureDefaultSyncFileExists();
                File.WriteAllText(SaveFile, json);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Load the user's sync options. This includes source and destinations, and more. If a problem is found in reading from saved config, a new empty config will be returned.
        /// </summary>
        /// <returns></returns>
        public static SyncConfig CreateFromFile()
        {
            SyncConfig sync = new SyncConfig
            {
                SourceDir = Machine.MyPicturesDirectory,
                DestinationDir = string.Empty,
                FileExtensions = DefaultFileExtensions.ToList()
            };

            EnsureDefaultSyncFileExists();
            try
            {
                string fileContents = File.ReadAllText(SaveFile);
                if (fileContents != string.Empty)
                {
                    sync = JsonConvert.DeserializeObject<SyncConfig>(fileContents);
                }
            }
            catch (Exception) { }
            return sync;
        }

        public static void EnsureDefaultSyncFileExists()
        {
            var configSaveFile = new FileInfo(SaveFile);
            if (configSaveFile.Exists == false)
            {
                if (Directory.Exists(configSaveFile.DirectoryName) == false)
                {
                    Directory.CreateDirectory(configSaveFile.DirectoryName);
                }
                File.Create(SaveFile);
            }
        }
    }
}
