using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BluePointLilac.Methods
{
    public static class FileExtension
    {
        [Flags]
        enum AssocF
        {
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200
        }

        enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic
        }

        [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, ref uint pcchOut);

        public const string FileExtsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        private static string GetExtentionInfo(AssocStr assocStr, string extension)
        {
            uint pcchOut = 0;
            AssocQueryString(AssocF.Verify, assocStr, extension, null, null, ref pcchOut);
            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            AssocQueryString(AssocF.Verify, assocStr, extension, null, pszOut, ref pcchOut);
            return pszOut.ToString();
        }

        public static string GetExecutablePath(string extension)
        {
            return GetExtentionInfo(AssocStr.Executable, extension);
        }

        public static string GetFriendlyDocName(string extension)
        {
            return GetExtentionInfo(AssocStr.FriendlyDocName, extension);
        }

        public static string GetOpenMode(string extension)
        {
            if(string.IsNullOrEmpty(extension)) return null;
            string mode = Registry.GetValue($@"{FileExtsPath}\{extension}\UserChoice", "ProgId", null)?.ToString();
            if(!string.IsNullOrEmpty(mode)) return mode;
            using(RegistryKey root = Registry.ClassesRoot)
            using(RegistryKey exKey = root.OpenSubKey(extension))
            {
                if(exKey == null) return null;
                mode = exKey.GetValue("")?.ToString();
                if(!mode.IsNullOrWhiteSpace()) return mode;
                using(RegistryKey pkey = exKey.OpenSubKey("OpenWithProgids"))
                {
                    if(pkey == null) return null;
                    foreach(string name in pkey.GetValueNames())
                    {
                        using(RegistryKey mKey = root.OpenSubKey(name))
                        {
                            if(mKey.GetValue("") != null) return name;
                        }
                    }
                }
            }
            return null;
        }
    }
}