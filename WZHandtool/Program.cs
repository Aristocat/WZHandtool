using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    static class Program
    {
        public static string DataLocation = @"C:\Nexon\Wizet\MapleStory\";
        public static bool OldVersion = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(new MainForm());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UnhandledExceptionLog Log = new UnhandledExceptionLog();
            Log.Log(e.Exception);
            Log.ShowDialog();
        }
    }
}
