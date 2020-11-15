using System;
using System.Reflection;

namespace BulePointLilac.Methods
{
    //为兼容.Net Framework 3.5无法使用dynamic和Interop.IWshRuntimeLibrary.dll专门写出此类
    sealed class WshShortcut
    {
        private static readonly Type ShellType = Type.GetTypeFromProgID("WScript.Shell");
        private static readonly object Shell = Activator.CreateInstance(ShellType);
        private static readonly BindingFlags InvokeMethodFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
        private static readonly BindingFlags GetPropertyFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
        private static readonly BindingFlags SetPropertyFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

        private static object CreateShortcut(string lnkPath)
        {
            return ShellType.InvokeMember("CreateShortcut", InvokeMethodFlag, null, Shell, new[] { lnkPath });
        }

        private Type ShortcutType;
        private object Shortcut;

        private string fullName;
        public string FullName
        {
            get => fullName;
            set => Load(value);
        }

        public string TargetPath
        {
            get => GetValue("TargetPath").ToString();
            set => SetValue("TargetPath", value);
        }
        public string Arguments
        {
            get => GetValue("Arguments").ToString();
            set => SetValue("Arguments", value);
        }
        public string WorkingDirectory
        {
            get => GetValue("WorkingDirectory").ToString();
            set => SetValue("WorkingDirectory", value);
        }
        public string IconLocation
        {
            get => GetValue("IconLocation").ToString();
            set => SetValue("IconLocation", value);
        }
        public string Description
        {
            get => GetValue("Description").ToString();
            set => SetValue("Description", value);
        }
        public string Hotkey
        {
            get => GetValue("Hotkey").ToString();
            set => SetValue("Hotkey", value);
        }
        public int WindowStyle
        {
            get => Convert.ToInt32(GetValue("WindowStyle"));
            set => SetValue("WindowStyle", value);
        }
        //public string RelativePath { get; set; }//暂时不知道这是啥

        private object GetValue(string name)
        {
            return ShortcutType.InvokeMember(name, GetPropertyFlag, null, Shortcut, null);
        }

        private void SetValue(string name, object value)
        {
            ShortcutType.InvokeMember(name, SetPropertyFlag, null, Shortcut, new[] { value });
        }

        public void Load(string lnkPath)
        {
            fullName = lnkPath;
            Shortcut = CreateShortcut(lnkPath);
            ShortcutType = Shortcut.GetType();
            Save();
        }

        public void Save()
        {
            //存储快捷方式为写入文件行为，如果没有权限会报错
            ShortcutType.InvokeMember("Save", InvokeMethodFlag, null, Shortcut, null);
        }
    }
}