using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BluePointLilac.Methods
{
    //为兼容.Net Framework 3.5无法使用dynamic和Interop.IWshRuntimeLibrary.dll专门写出此类
    public sealed class WshShortcut : IDisposable
    {
        private static readonly Type ShellType = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
        private static readonly object Shell = Activator.CreateInstance(ShellType);
        private static readonly BindingFlags InvokeMethodFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
        private static readonly BindingFlags GetPropertyFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
        private static readonly BindingFlags SetPropertyFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

        private static object CreateShortcut(string lnkPath)
        {
            return ShellType.InvokeMember("CreateShortcut", InvokeMethodFlag, null, Shell, new[] { lnkPath });
        }

        public WshShortcut(string lnkPath)
        {
            //只调用CreateShortcut方法时，TargetPath值为null，
            //其他属性值正常，只有调用一下Save方法才能获取到TargetPath值
            this.lnkPath = lnkPath;
            shortcut = CreateShortcut(lnkPath);
            shortcutType = shortcut.GetType();
            shellLink = (IShellLinkA)new ShellLink();
            if(File.Exists(lnkPath))
            {
                Save();
                ((IPersistFile)shellLink).Load(lnkPath, 2);//STGM_READWRITE
            }
        }

        private readonly string lnkPath;
        private readonly object shortcut;
        private readonly Type shortcutType;
        private readonly IShellLinkA shellLink;

        public string FullName
        {
            get => GetValue("FullName")?.ToString();
        }
        public string TargetPath
        {
            get => GetValue("TargetPath")?.ToString();
            set => SetValue("TargetPath", value);
        }
        public string Arguments
        {
            get => GetValue("Arguments")?.ToString();
            set => SetValue("Arguments", value);
        }
        public string WorkingDirectory
        {
            get => GetValue("WorkingDirectory")?.ToString();
            set => SetValue("WorkingDirectory", value);
        }
        public string IconLocation
        {
            get => GetValue("IconLocation")?.ToString();
            set => SetValue("IconLocation", value);
        }
        public string Description
        {
            get => GetValue("Description")?.ToString();
            set => SetValue("Description", value);
        }
        public string Hotkey
        {
            get => GetValue("Hotkey")?.ToString();
            set => SetValue("Hotkey", value);
        }
        public int WindowStyle
        {
            get => Convert.ToInt32(GetValue("WindowStyle"));
            set => SetValue("WindowStyle", value);
        }
        public bool RunAsAdministrator
        {
            get
            {
                ((IShellLinkDataList)shellLink).GetFlags(out ShellLinkDataFlags flags);
                return (flags & ShellLinkDataFlags.RunasUser) == ShellLinkDataFlags.RunasUser;
            }
            set
            {
                ((IShellLinkDataList)shellLink).GetFlags(out ShellLinkDataFlags flags);
                if(value) flags |= ShellLinkDataFlags.RunasUser;
                else flags &= ~ShellLinkDataFlags.RunasUser;
                ((IShellLinkDataList)shellLink).SetFlags(flags);
                ((IPersistFile)shellLink).Save(lnkPath, true);
            }
        }

        private object GetValue(string name)
        {
            try { return shortcutType.InvokeMember(name, GetPropertyFlag, null, shortcut, null); }
            catch { return null; }
        }

        private void SetValue(string name, object value)
        {
            shortcutType.InvokeMember(name, SetPropertyFlag, null, shortcut, new[] { value });
        }

        public void Save()
        {
            //存储快捷方式为写入文件行为，如果没有权限会报错
            shortcutType.InvokeMember("Save", InvokeMethodFlag, null, shortcut, null);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(shortcut);
            Marshal.ReleaseComObject(shellLink);
        }

        ~WshShortcut() { Dispose(); }


        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink { }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214EE-0000-0000-C000-000000000046")]
        public interface IShellLinkA
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPStr)] string pszFile);
        }

        [ComImport]
        [Guid("45e2b4ae-b1c3-11d0-b92f-00a0c90312e1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IShellLinkDataList
        {
            void AddDataBlock(IntPtr pDataBlock);
            void CopyDataBlock(uint dwSig, out IntPtr ppDataBlock);
            void RemoveDataBlock(uint dwSig);
            void GetFlags(out ShellLinkDataFlags pdwFlags);
            void SetFlags(ShellLinkDataFlags dwFlags);
        }

        [Flags] // SHELL_LINK_DATA_FLAGS
        public enum ShellLinkDataFlags : uint
        {
            Default = 0x00000000,
            HasIdList = 0x00000001,
            HasLinkInfo = 0x00000002,
            HasName = 0x00000004,
            HasRelpath = 0x00000008,
            HasWorkingdir = 0x00000010,
            HasArgs = 0x00000020,
            HasIconLocation = 0x00000040,
            Unicode = 0x00000080,
            ForceNoLinkInfo = 0x00000100,
            HasExpSz = 0x00000200,
            RunInSeparate = 0x00000400,
            HasLogo3Id = 0x00000800,
            HasDarwinId = 0x00001000,
            RunasUser = 0x00002000,
            HasExpIconSz = 0x00004000,
            NoPidlAlias = 0x00008000,
            ForceUncname = 0x00010000,
            RunWithShimlayer = 0x00020000,
            ForceNoLinktrack = 0x00040000,
            EnableTargetMetadata = 0x00080000,
            DisableLinkPathTracking = 0x00100000,
            DisableKnownfolderRelativeTracking = 0x00200000,
            NoKFAlias = 0x00400000,
            AllowLinkToLink = 0x00800000,
            UnaliasOnSave = 0x01000000,
            PreferEnvironmentPath = 0x02000000,
            KeepLocalIdListForUncTarget = 0x04000000,
            Valid = 0x07fff7ff,
            Reserved = 0x80000000
        }
    }
}