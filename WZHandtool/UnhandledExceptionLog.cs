using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    public partial class UnhandledExceptionLog : Form
    {
        public UnhandledExceptionLog()
        {
            InitializeComponent();
        }

        private void UnhandledExceptionLog_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        public void Log(Exception ex)
        {
            fieldExceptionLog.AppendText("An unhandled exception has occured.\r\n");
            fieldExceptionLog.AppendText(ex.ToString());
        }
    }
}
