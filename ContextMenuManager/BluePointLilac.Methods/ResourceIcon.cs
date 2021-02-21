using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class ResourceIcon
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(string lpFileName, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, uint nIcons);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hLibModule);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadImage(IntPtr hInst, string lpFileName, uint uType, int cx, int cy, uint fuLoad);

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, FileInfoFlags uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        public enum FileInfoFlags : uint
        {
            SHGFI_ICON = 0x000000100,               // get icon
            SHGFI_DISPLAYNAME = 0x000000200,        // get display name
            SHGFI_TYPENAME = 0x000000400,           // get type name
            SHGFI_ATTRIBUTES = 0x000000800,         // get attributes
            SHGFI_ICONLOCATION = 0x000001000,       // get icon location
            SHGFI_EXETYPE = 0x000002000,            // return exe type
            SHGFI_SYSICONINDEX = 0x000004000,       // get system icon index
            SHGFI_LINKOVERLAY = 0x000008000,        // put a link overlay on icon
            SHGFI_SELECTED = 0x000010000,           // show icon in selected state
            SHGFI_ATTR_SPECIFIED = 0x000020000,     // get only specified attributes
            SHGFI_LARGEICON = 0x000000000,          // get large icon
            SHGFI_SMALLICON = 0x000000001,          // get small icon
            SHGFI_OPENICON = 0x000000002,           // get open icon
            SHGFI_SHELLICONSIZE = 0x000000004,      // get shell size icon
            SHGFI_PIDL = 0x000000008,               // pszPath is a pidl
            SHGFI_USEFILEATTRIBUTES = 0x000000010,  // use passed dwFileAttribute
            SHGFI_ADDOVERLAYS = 0x000000020,        // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040        // get the index of the overlay in the upper 8 bits of the iIcon
        }

        /// <summary>获取文件类型的关联图标</summary>
        /// <param name="extension">文件类型的扩展名，如.txt</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetExtensionIcon(string extension)
        {
            FileInfoFlags flags = FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_LARGEICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES;
            return GetIcon(extension, flags);
        }

        /// <summary>获取文件夹、磁盘驱动器的图标</summary>
        /// <param name="folderPath">文件夹或磁盘驱动器路径</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetFolderIcon(string folderPath)
        {
            FileInfoFlags flags = FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_LARGEICON;
            return GetIcon(folderPath, flags);
        }

        /// <summary>根据文件信息标志提取指定文件路径的图标</summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="flags">文件信息标志</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetIcon(string filePath, FileInfoFlags flags)
        {
            SHFILEINFO info = new SHFILEINFO();
            IntPtr hInfo = SHGetFileInfo(filePath, 0, ref info, (uint)Marshal.SizeOf(info), flags);
            if(hInfo.Equals(IntPtr.Zero)) return null;
            Icon icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            DestroyIcon(info.hIcon); //释放资源
            return icon;
        }

        /// <summary>获取指定位置的图标</summary>
        /// <param name="iconLocation">图标位置</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetIcon(string iconLocation)
        {
            return GetIcon(iconLocation, out _, out _);
        }

        /// <summary>获取指定位置的图标</summary>
        /// <param name="iconLocation">图标位置</param>
        /// <param name="iconPath">返回图标文件路径</param>
        /// <param name="iconIndex">返回图标索引</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetIcon(string iconLocation, out string iconPath, out int iconIndex)
        {
            iconIndex = 0; iconPath = null;
            if(iconLocation.IsNullOrWhiteSpace()) return null;
            iconLocation = Environment.ExpandEnvironmentVariables(iconLocation).Replace("\"", "");
            int index = iconLocation.LastIndexOf(',');
            if(index == -1) iconPath = iconLocation;
            else
            {
                if(File.Exists(iconLocation)) iconPath = iconLocation;
                else
                {
                    bool flag = int.TryParse(iconLocation.Substring(index + 1), out iconIndex);
                    iconPath = flag ? iconLocation.Substring(0, index) : null;
                }
            }
            return GetIcon(iconPath, iconIndex);
        }

        /// <summary>获取指定文件中指定索引的图标</summary>
        /// <param name="iconPath">图标文件路径</param>
        /// <param name="iconIndex">图标索引</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetIcon(string iconPath, int iconIndex)
        {
            Icon icon = null;
            if(iconPath.IsNullOrWhiteSpace()) return icon;
            iconPath = Environment.ExpandEnvironmentVariables(iconPath).Replace("\"", "");

            if(Path.GetFileName(iconPath).ToLower() == "shell32.dll")
            {
                iconPath = "shell32.dll";//系统强制文件重定向
                icon = GetReplacedShellIcon(iconIndex);//注册表图标重定向
                if(icon != null) return icon;
            }

            IntPtr hInst = IntPtr.Zero;
            IntPtr[] hIcons = new[] { IntPtr.Zero };
            //iconIndex为负数就是指定资源标识符, 为正数就是该图标在资源文件中的顺序序号, 为-1时不能使用ExtractIconEx提取图标
            if(iconIndex == -1)
            {
                hInst = LoadLibrary(iconPath);
                hIcons[0] = LoadImage(hInst, "#1", 1, SystemInformation.IconSize.Width, SystemInformation.IconSize.Height, 0);
            }
            else ExtractIconEx(iconPath, iconIndex, hIcons, null, 1);

            try { icon = (Icon)Icon.FromHandle(hIcons[0]).Clone(); }
            catch { icon = null; }
            finally { DestroyIcon(hIcons[0]); FreeLibrary(hInst); }//释放资源
            return icon;
        }

        private const string ShellIconPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons";
        /// <summary>获取shell32.dll中的图标被替换后的图标</summary>
        /// <param name="iconIndex">图标索引</param>
        /// <returns>获取到的图标</returns>
        public static Icon GetReplacedShellIcon(int iconIndex)
        {
            string iconLocation = Registry.GetValue(ShellIconPath, iconIndex.ToString(), null)?.ToString();
            if(iconLocation != null) return GetIcon(iconLocation) ?? GetIcon("imageres.dll", 2);
            else return null;
        }
    }
}