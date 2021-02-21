using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BluePointLilac.Methods
{
    public static class ExternalProgram
    {
        public static void JumpRegEdit(string regPath, string valueName = null, bool moreOpen = false)
        {
            IntPtr hMain = FindWindow("RegEdit_RegEdit", null);
            if(moreOpen || hMain == IntPtr.Zero)
            {
                using(Process process = Process.Start("regedit.exe", "-m"))
                {
                    process.WaitForInputIdle();
                    hMain = process.MainWindowHandle;
                }
            }

            IntPtr hTree = FindWindowEx(hMain, IntPtr.Zero, "SysTreeView32", null);
            IntPtr hList = FindWindowEx(hMain, IntPtr.Zero, "SysListView32", null);

            SetForegroundWindow(hTree);
            SetFocus(hTree);
            SendMessage(hTree, WM_KEYDOWN, VK_HOME, null);
            Thread.Sleep(50);
            SendMessage(hTree, WM_KEYDOWN, VK_RIGHT, null);
            foreach(char chr in Encoding.Default.GetBytes(regPath))
            {
                if(chr == '\\')
                {
                    Thread.Sleep(50);
                    SendMessage(hTree, WM_KEYDOWN, VK_RIGHT, null);
                }
                else SendMessage(hTree, WM_CHAR, Convert.ToInt16(chr), null);
            }

            if(!string.IsNullOrEmpty(valueName))
            {
                Thread.Sleep(50);
                SetForegroundWindow(hList);
                SetFocus(hList);
                SendMessage(hList, WM_KEYDOWN, VK_HOME, null);
                foreach(char chr in Encoding.Default.GetBytes(valueName))
                {
                    SendMessage(hList, WM_CHAR, Convert.ToInt16(chr), null);
                }
            }
        }

        public static void JumpExplorer(string filePath)
        {
            using(Process process = new Process())
            {
                if(File.Exists(filePath))
                {
                    process.StartInfo.FileName = "explorer.exe";
                    process.StartInfo.Arguments = $"/select,{filePath}";
                    process.Start();
                }
                else if(Directory.Exists(filePath))
                {
                    process.StartInfo.FileName = filePath;
                    process.Start();
                }
            }
        }

        public static bool ShowPropertiesDialog(string filePath)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO
            {
                lpVerb = "Properties",
                //lpParameters = "详细信息";//显示选项卡,此处有语言差异
                lpFile = filePath,
                nShow = SW_SHOW,
                fMask = SEE_MASK_INVOKEIDLIST,
                cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO))
            };
            return ShellExecuteEx(ref info);
        }

        public static void RestartExplorer()
        {
            using(Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = "-f -im explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                process.Start();
                process.WaitForExit();
                process.StartInfo = new ProcessStartInfo("explorer.exe");
                process.Start();
            }
        }

        public static void OpenUrl(string url)
        {
            //替换网址转义符
            url = url.Replace("%", "%25").Replace("#", "%23").Replace("&", "%26").Replace("+", "%2B");
            using(Process process = new Process())
            {
                //通过explorer来调用浏览器打开链接，避免管理员权限影响
                process.StartInfo.FileName = "explorer.exe";
                process.StartInfo.Arguments = $"\"{url}\"";
                process.Start();
            }
        }

        public static void OpenNotepadWithText(string text)
        {
            using(Process process = Process.Start("notepad.exe"))
            {
                Thread.Sleep(200);
                IntPtr handle = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, "Edit", null);
                SendMessage(handle, WM_SETTEXT, 0, text);
            }
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        private const int WM_SETTEXT = 0xC;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_CHAR = 0x102;
        private const int VK_HOME = 0x24;
        private const int VK_RIGHT = 0x27;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern bool SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChild, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }
    }
}