using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSync
{
    static class Machine
    {
        public static string MyPicturesDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); }
        }
    }
}
