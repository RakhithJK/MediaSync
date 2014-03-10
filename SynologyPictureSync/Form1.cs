using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void syncNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sync();
        }


        private void Sync()
        {
            //load options
            var syncConfig =   SyncConfig.CreateFromFile();
          
            //perform the sync
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }



    }
}
