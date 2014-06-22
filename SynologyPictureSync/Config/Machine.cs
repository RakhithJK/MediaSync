using System;

namespace MediaSync
{
    static class Machine
    {
        public static string MyPicturesDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); }
        }

        public static string AppDataDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }
    }
}
