using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SporaVaccination.Properties;

namespace SporaVaccination
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayApplicationContext());
        }

        public class TrayApplicationContext : ApplicationContext
        {
            private readonly NotifyIcon trayIcon;

            public TrayApplicationContext()
            {
                trayIcon = new NotifyIcon
                {
                    Icon = Resources.MinervaLogo,
                    ContextMenu = new ContextMenu(new[]
                    {
                        new MenuItem("Exit", Exit)
                    }),
                    Visible = true
                };

                VaccinateSpora();
            }

            private bool VaccinateSpora()
            {
                uint volumeSerialNumber, maxComponentLength;
                WinApi.FileSystemFeature fileSystemFlags;
                if (!WinApi.GetVolumeInformation(@"C:\", null, 0, out volumeSerialNumber, out maxComponentLength, out fileSystemFlags, null, 0))
                {
                    return false;
                }

                string mutexName = "m" + volumeSerialNumber;
                if (WinApi.CreateMutex(IntPtr.Zero, false, mutexName) == IntPtr.Zero)
                {
                    return false;
                }

                return true;
            }

            private void Exit(object sender, EventArgs e)
            {
                trayIcon.Visible = false;
                Application.Exit();
            }
        }
    }
}
