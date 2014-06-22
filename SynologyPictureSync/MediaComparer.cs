using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaSync
{
    public static class MediaComparer
    {
        private const int PropertyItemDateTakenOn = 36867;
        private static Regex r = new Regex(":");
        public static DateTime? GetImageTakenOnDate(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(PropertyItemDateTakenOn);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch (Exception) { }
            return null;
        }
    }
}
