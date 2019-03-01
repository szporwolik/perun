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

        // Mutex to block multiple instances
        static Mutex mutex = new Mutex(true, "{5a710507-92e9-4664-9b91-918b8b82f107}");

        [STAThread]

        static void Main()
        {
            // Check if no other instances running
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                // Only instance
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

                void Application_ApplicationExit(object sender, EventArgs e)
                {
                    Properties.Settings.Default.Save(); // We will save settings on exit
                }
                Application.Run(new form_Main()); // Run main form
                mutex.ReleaseMutex(); // Release mutex
            }
            else
            {
                // Multiple instance - send our Win32 message to make the currently running instance jump to front
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                MessageBox.Show("Only one Perun instance is allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
