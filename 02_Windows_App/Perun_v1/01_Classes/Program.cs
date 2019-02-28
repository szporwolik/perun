using System;
using System.Windows.Forms;

namespace Perun_v1
{

    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]

    
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            void Application_ApplicationExit(object sender, EventArgs e)
            {
                Properties.Settings.Default.Save();
            }

            Application.Run(new form_Main());
        }

       
    }
}
