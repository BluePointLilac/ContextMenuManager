using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class SingleInstance
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>判断单实例程序是否正在运行</summary>
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

        /// <summary>重启单实例程序</summary>
        /// <param name="args">重启程序时传入参数</param>
        /// <param name="updatePath">用于更新程序的新版本文件路径，为null则不更新</param>
        public static void Restart(string[] args = null, string updatePath = null)
        {
            using(Process pApp = Process.GetCurrentProcess())
            {
                string command = Application.ExecutablePath;
                if(args != null && args.Length > 0) command += "," + string.Join(" ", args);
                string contents =
                    "Dim wsh, fso\r\n" +
                    "Set wsh = CreateObject(\"WScript.Shell\")\r\n" +
                    "Set fso = CreateObject(\"Scripting.FileSystemObject\")\r\n" +
                    "fso.DeleteFile(WScript.ScriptFullName)\r\n" +  //vbs自删命令
                    $"wsh.Run \"taskkill /pid {pApp.Id} -f\",0\r\n" +
                    "WScript.Sleep 1000\r\n";

                if(updatePath != null) contents +=
                    $"fso.DeleteFile \"{Application.ExecutablePath}\"\r\n" +
                    "WScript.Sleep 1000\r\n" +
                    $"fso.MoveFile \"{updatePath}\",\"{Application.ExecutablePath}\"\r\n";

                contents +=
                    $"wsh.Run \"{command}\"\r\n" +
                    "Set wsh = Nothing\r\n" +
                    "Set fso = Nothing\r\n";

                string vbsPath = Path.GetTempPath() + "Restart.vbs";
                File.WriteAllText(vbsPath, contents, Encoding.Unicode);
                using(Process pVbs = new Process())
                {
                    pVbs.StartInfo.FileName = "wscript.exe";
                    pVbs.StartInfo.Arguments = vbsPath;
                    pVbs.Start();
                }
            }
        }
    }
}