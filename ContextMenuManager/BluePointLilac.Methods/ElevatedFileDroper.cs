using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    /// 代码用途：管理员UAC进程窗口拖放文件
    /// 代码来源1：https://zhuanlan.zhihu.com/p/48735364
    /// 代码来源2：https://github.com/volschin/sdimager/blob/master/ElevatedDragDropManager.cs
    /// 代码作者：雨少主（知乎）、volschin（Github）、蓝点lilac（转载、修改）
    /// 调用方法：var droper = new ElevatedFileDroper(control);
    /// control.DragDrop += (sender, e) => MessageBox.Show(droper.DropFilePaths[0]);

    public sealed class ElevatedFileDroper : IMessageFilter
    {
        #region native members
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint message, ChangeFilterAction action, in ChangeFilterStruct pChangeFilterStruct);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeWindowMessageFilter(uint msg, ChangeWindowMessageFilterFlags flags);

        [DllImport("shell32.dll", SetLastError = false)]
        private static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport("shell32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern uint DragQueryFile(IntPtr hWnd, uint iFile, StringBuilder lpszFile, int cch);

        [DllImport("shell32.dll", SetLastError = false)]
        private static extern bool DragQueryPoint(IntPtr hDrop, out Point lppt);

        [DllImport("shell32.dll", SetLastError = false)]
        private static extern void DragFinish(IntPtr hDrop);

        [StructLayout(LayoutKind.Sequential)]
        struct ChangeFilterStruct
        {
            public uint CbSize;
            public ChangeFilterStatu ExtStatus;
        }

        enum ChangeWindowMessageFilterFlags : uint
        {
            MSGFLT_ADD = 1,
            MSGFLT_REMOVE = 2
        }

        enum ChangeFilterAction : uint
        {
            MSGFLT_RESET,
            MSGFLT_ALLOW,
            MSGFLT_DISALLOW
        }

        enum ChangeFilterStatu : uint
        {
            MSGFLTINFO_NONE,
            MSGFLTINFO_ALREADYALLOWED_FORWND,
            MSGFLTINFO_ALREADYDISALLOWED_FORWND,
            MSGFLTINFO_ALLOWED_HIGHER
        }

        const uint WM_COPYGLOBALDATA = 0x0049;
        const uint WM_COPYDATA = 0x004A;
        const uint WM_DROPFILES = 0x0233;
        #endregion

        public Control ContainerControl { get; private set; }
        public string[] DropFilePaths { get; private set; }
        public Point DropPoint { get; private set; }

        public ElevatedFileDroper(Control ctr)
        {
            this.ContainerControl = ctr;
            ctr.DragDrop += (sender, e) => DropFilePaths = (string[])e.Data.GetData(typeof(string[]));
            ctr.DragEnter += (sender, e) => e.Effect = DragDropEffects.All;
            ctr.Disposed += (sender, e) => Application.RemoveMessageFilter(this);

            Version ver = Environment.OSVersion.Version;
            bool isVistaOrHigher = ver >= new Version(6, 0);
            bool isWin7OrHigher = ver >= new Version(6, 1);
            var status = new ChangeFilterStruct { CbSize = 8 };
            if(isVistaOrHigher)
            {
                foreach(uint msg in new[] { WM_DROPFILES, WM_COPYGLOBALDATA, WM_COPYDATA })
                {
                    bool error = false;
                    if(isWin7OrHigher)
                    {
                        error = !ChangeWindowMessageFilterEx(ctr.Handle, msg, ChangeFilterAction.MSGFLT_ALLOW, in status);
                    }
                    else
                    {
                        error = !ChangeWindowMessageFilter(msg, ChangeWindowMessageFilterFlags.MSGFLT_ADD);
                    }
                    if(error) throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            DragAcceptFiles(ctr.Handle, true);
            Application.AddMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if(m.Msg != WM_DROPFILES) return false;
            if(ContainerControl.AllowDrop)
            {
                DropPoint = ContainerControl.PointToClient(Cursor.Position);
            }
            else
            {
                IntPtr handle = m.WParam;
                uint fileCount = DragQueryFile(handle, uint.MaxValue, null, 0);
                string[] fileNames = new string[fileCount];
                for(uint i = 0; i < fileCount; i++)
                {
                    StringBuilder sb = new StringBuilder(260);
                    uint result = DragQueryFile(handle, i, sb, sb.Capacity);
                    if(result > 0) fileNames[i] = sb.ToString();
                }
                DragQueryPoint(handle, out Point point);
                DragFinish(handle);
                DropPoint = point;
                ContainerControl.AllowDrop = true;
                ContainerControl.DoDragDrop(fileNames, DragDropEffects.All);
            }
            ContainerControl.AllowDrop = false;
            return true;
        }
    }
}