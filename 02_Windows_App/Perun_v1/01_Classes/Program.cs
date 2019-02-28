using System;
using System.Threading;
using System.Windows.Forms;

namespace Perun_v1
{

    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>

        // Mutex to block multiple instances
        static Mutex mutex = new Mutex(true, "{5a710507-92e9-4664-9b91-918b8b82f107}");
        [STAThread]

        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

                void Application_ApplicationExit(object sender, EventArgs e)
                {
                    Properties.Settings.Default.Save();
                }
                Application.Run(new form_Main());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                MessageBox.Show("Only one instance is allowed.");
            }
        }


    }
}
