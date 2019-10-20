using System;
using System.Threading;
using System.Windows.Forms;

namespace Perun_v1
{
    static class Program
    {
        /// <summary>
        /// Main entry point to app
        /// </summary>

        [STAThread]

        static void Main()
        {
               // Main entry point to the app
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            void Application_ApplicationExit(object sender, EventArgs e)
            {
                Properties.Settings.Default.Save(); // We will save settings on exit
            }
            Application.Run(new form_Main()); // Run main form
        }
    }
}
