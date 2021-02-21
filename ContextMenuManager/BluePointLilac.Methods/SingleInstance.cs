using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class SingleInstance
    {
        public static bool IsRunning()
        {
            using(Process current = Process.GetCurrentProcess())
            {
                string fileName = current.MainModule.FileName;
                string processName = Path.GetFileNameWithoutExtension(fileName);
                foreach(Process process in Process.GetProcessesByName(processName))
                {
                    using(process)
                    {
                        if(process.Id == current.Id) continue;
                        if(process.MainModule.FileName == fileName)
                        {
                            ShowWindowAsync(process.MainWindowHandle, 1);//SW_SHOWNORMAL
                            SetForegroundWindow(process.MainWindowHandle);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public static void Restart()
        {
            using(Process process = new Process())
            {
                process.StartInfo.FileName = Application.ExecutablePath;
                process.StartInfo.Arguments = "Restart";
                process.Start();
            }
            Application.Exit();
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}