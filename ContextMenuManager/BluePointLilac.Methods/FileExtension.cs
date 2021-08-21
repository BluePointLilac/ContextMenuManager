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

        public enum AssocStr
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

        public const string FILEEXTSPATH = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        private const string HKCRCLASSES = @"HKEY_CURRENT_USER\SOFTWARE\Classes";
        private const string HKLMCLASSES = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes";

        public static string GetExtentionInfo(AssocStr assocStr, string extension)
        {
            uint pcchOut = 0;
            AssocQueryString(AssocF.Verify, assocStr, extension, null, null, ref pcchOut);
            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            AssocQueryString(AssocF.Verify, assocStr, extension, null, pszOut, ref pcchOut);
            return pszOut.ToString();
        }

        public static string GetOpenMode(string extension)
        {
            if(string.IsNullOrEmpty(extension)) return null;
            string mode;
            bool CheckMode()
            {
                if(mode.IsNullOrWhiteSpace()) return false;
                if(mode.Length > 255) return false;
                if(mode.ToLower().StartsWith(@"applications\")) return false;
                using(RegistryKey root = Registry.ClassesRoot)
                using(RegistryKey key = root.OpenSubKey(mode))
                {
                    return key != null;
                }
            }
            mode = Registry.GetValue($@"{FILEEXTSPATH}\{extension}\UserChoice", "ProgId", null)?.ToString();
            if(CheckMode()) return mode;
            mode = Registry.GetValue($@"{HKLMCLASSES}\{extension}", "", null)?.ToString();
            if(CheckMode()) return mode;
            mode = Registry.GetValue($@"{HKCRCLASSES}\{extension}", "", null)?.ToString();
            if(CheckMode()) return mode;
            return null;
        }
    }
}