using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MediaSync
{
    [Serializable]
    public class SyncConfig
    {
        public string SourceDir { get; set; }
        public string DestinationDir { get; set; }
        public bool ShouldDeleteSourceWhenSuccessfullyCompleted { get; set; }


        const string SaveFile = @"SyncConfig.json";
        public SyncConfig()
        {

        }
        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
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
            try
            {
                string fileContents = File.ReadAllText(SaveFile);
                return JsonConvert.DeserializeObject<SyncConfig>(fileContents);
            }
            catch (Exception) { }
            return new SyncConfig { SourceDir=Machine.MyPicturesDirectory, DestinationDir=string.Empty };
        }



    }
}
