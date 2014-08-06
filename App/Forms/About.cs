using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
            lnkGitHubLocation.Links.Remove(lnkGitHubLocation.Links[0]);
            lnkGitHubLocation.Links.Add(0, lnkGitHubLocation.Text.Length, lnkGitHubLocation.Text);
        }

        private void lnkGitHubLocation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var process = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(process);
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            lblVersion.Text = string.Format("Version: {0}", Application.ProductVersion);
            lblBuildDateTitle.Text = string.Format("Build Date: {0}", RetrieveLinkerTimestamp().ToString("dd-MMM-yyyy HH:mm"));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }

    }
}
