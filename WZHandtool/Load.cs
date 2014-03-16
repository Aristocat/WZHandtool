using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    public partial class Load : Form
    {
        public Load()
        {
            InitializeComponent();
        }

        private void Load_OnLoad(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            if (Directory.Exists(Program.DataLocation))
                textBox1.Text = Program.DataLocation;
            //TODO: load new wz
            checkBox1.Enabled = false;
            checkBox1.Checked = true;
        }

        private void Load_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("Failed to locate MapleStory folder.");
                Environment.Exit(0);
            }
            string[] files = new string[] { };
            if (Program.OldVersion)
                files = new string[1] { "Data" };
            else
                files = new string[] { };
            foreach (string file in files)
            {
                if (!File.Exists(textBox1.Text + @"\" + file + ".wz"))
                {
                    MessageBox.Show("Failed to locate " + file + ".wz file.");
                    Environment.Exit(0);
                }
            }
            Program.DataLocation = textBox1.Text;
            Program.OldVersion = checkBox1.Checked;
        }

        private void btLoad_OnClick(object sender, EventArgs e)
        {
            Close();
        }

        private void btBrowse_OnClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fDialog = new FolderBrowserDialog())
            {
                fDialog.SelectedPath = Program.DataLocation;
                if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    textBox1.Text = fDialog.SelectedPath;
            }
        }
    }
}
