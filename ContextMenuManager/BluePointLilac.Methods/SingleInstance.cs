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
        /// <remarks>若正在运行激活窗口</remarks>
        public static bool IsRunning()
        {
            using(Process current = Process.GetCurrentProcess())
            {
                foreach(Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    using(process)
                    {
                        if(process.Id == current.Id) continue;
                        if(process.MainModule.FileName == current.MainModule.FileName)
                        {
                            const int SW_RESTORE = 9;
                            ShowWindowAsync(process.MainWindowHandle, SW_RESTORE);
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
        /// <param name="updatePath">用于更新程序的新版本文件路径，为null则为普通重启</param>
        public static void Restart(string[] args = null, string updatePath = null)
        {
            string appPath = Application.ExecutablePath;
            string command = appPath;
            if(args != null && args.Length > 0) command += "," + string.Join(" ", args);
            List<string> contents = new List<string>();
            //vbs命令逐行执行不等待，故加些代码确定上一条命令执行是否完成
            contents.AddRange(new[]
            {
                "On Error Resume Next",
                "WScript.Sleep 1000",//等待程序结束
                "Dim wsh, fso",
                "Set wsh = CreateObject(\"WScript.Shell\")",
                "Set fso = CreateObject(\"Scripting.FileSystemObject\")",
            });

            if(File.Exists(updatePath))
            {
                contents.AddRange(new[]
                {
                    $"fso.DeleteFile \"{appPath}\"",
                    $"Do While fso.FileExists(\"{appPath}\")",
                        "WScript.Sleep 100",
                    "Loop",//确定文件删除完成
                    $"fso.MoveFile \"{updatePath}\",\"{appPath}\"",//更新文件
                    $"Do While fso.FileExists(\"{updatePath}\")",//确定文件已被移动
                        "WScript.Sleep 100",
                    $"Loop",
                });
            }
            contents.AddRange(new[]
            {
                $"wsh.Run \"{command}\"",
                "fso.DeleteFile(WScript.ScriptFullName)",//vbs自删命令
                "Set wsh = Nothing",
                "Set fso = Nothing",
            });

            string vbsPath = Path.GetTempPath() + Guid.NewGuid() + ".vbs";
            File.WriteAllLines(vbsPath, contents.ToArray(), Encoding.Unicode);
            Application.Exit();
            using(Process process = new Process())
            {
                process.StartInfo.FileName = "wscript.exe";
                process.StartInfo.Arguments = vbsPath;
                process.Start();
            }
        }
    }
}