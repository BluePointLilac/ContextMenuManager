using System;
using System.Collections.Generic;
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
                List<string> contents = new List<string>();
                contents.AddRange(new[]{
                    "Dim wsh, fso",
                    "Set wsh = CreateObject(\"WScript.Shell\")",
                    "Set fso = CreateObject(\"Scripting.FileSystemObject\")",
                    "fso.DeleteFile(WScript.ScriptFullName)",//vbs自删命令
                    $"wsh.Run \"taskkill /pid {pApp.Id} -f\",0",//杀死当前进程
                    "WScript.Sleep 1000"
                });

                if(File.Exists(updatePath))
                {
                    contents.AddRange(new[]
                    {
                        $"fso.DeleteFile \"{Application.ExecutablePath}\"",
                        "WScript.Sleep 1000",
                        $"fso.MoveFile \"{updatePath}\",\"{Application.ExecutablePath}\""//更新文件
                    });
                }
                contents.AddRange(new[]
                {
                    $"wsh.Run \"{command}\"",
                    "Set wsh = Nothing",
                    "Set fso = Nothing"
                });

                string vbsPath = Path.GetTempPath() + "Restart.vbs";
                File.WriteAllLines(vbsPath, contents.ToArray(), Encoding.Unicode);
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