using System;
using System.Runtime.InteropServices;

namespace BulePointLilac.Methods
{
    public static class PropertiesDialog
    {
        public static bool Show(string filePath)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO
            {
                lpVerb = "Properties",
                //lpParameters = "详细信息";//显示选项卡,此处有语言差异
                lpFile = filePath,
                nShow = SW_SHOW,
                fMask = SEE_MASK_INVOKEIDLIST,
                cbSize = CbSize
            };
            return ShellExecuteEx(ref info);
        }

        private const int SW_SHOW = 5;

        private const uint SEE_MASK_INVOKEIDLIST = 12;

        private static readonly int CbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO));

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