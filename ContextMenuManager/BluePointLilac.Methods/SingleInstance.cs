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
        /// <param name="updatePath">用于更新程序的新版本文件路径，为null则不更新</param>
        public static void Restart(string[] args = null, string updatePath = null)
        {
            string appPath = Application.ExecutablePath;
            string command = appPath;
            if(args != null && args.Length > 0) command += "," + string.Join(" ", args);
            List<string> contents = new List<string>();
            using(Process pApp = Process.GetCurrentProcess())
            {
                //Vbs命令逐行执行不等待，故加些代码确定上一条命令执行是否完成
                contents.AddRange(new[]{
                    "Dim wsh, fso, wmi, ps",
                    "Set wsh = CreateObject(\"WScript.Shell\")",
                    "Set fso = CreateObject(\"Scripting.FileSystemObject\")",
                    "fso.DeleteFile(WScript.ScriptFullName)",//vbs自删命令
                    $"wsh.Run \"taskkill /pid {pApp.Id} -f\",0",//杀死当前进程
                    "Dim isRun",
                    "Do",
                        "isRun = 0",
                        "WScript.Sleep 100",
                        "Set wmi = GetObject(\"WinMgmts:\")",
                        "Set ps = wmi.InstancesOf(\"Win32_Process\")",
                        //确定进程完全退出
                        "For Each p in ps",
                            $"If p.ProcessID = {pApp.Id} Then",
                                "isRun = 1",
                                "Exit For",
                            "End If",
                        "Next",
                    "Loop Until isRun = 0",
                });
            }

            if(File.Exists(updatePath))
            {
                contents.AddRange(new[]
                {
                    $"fso.DeleteFile \"{appPath}\"",
                    $"Do",
                        "WScript.Sleep 100",
                    "Loop Until fso.FileExists(\"{appPath}\") = False",//确定文件删除完成
                    $"fso.MoveFile \"{updatePath}\",\"{appPath}\"",//更新文件
                    $"Do",
                        "WScript.Sleep 100",
                    $"Loop Until fso.FileExists(\"{appPath}\")",//确定文件存在
                });
            }
            contents.AddRange(new[]
            {
                $"wsh.Run \"{command}\"",
                "Set wsh = Nothing",
                "Set fso = Nothing",
                "Set wmi = Nothing",
                "Set ps = Nothing",
            });

            string vbsPath = Path.GetTempPath() + Guid.NewGuid() + ".vbs";
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