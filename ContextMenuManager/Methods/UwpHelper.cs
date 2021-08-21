using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.IO;

namespace ContextMenuManager.Methods
{
    static class UwpHelper
    {
        private const string PackageRegPath = @"HKEY_CLASSES_ROOT\PackagedCom\Package";
        private const string PackagesRegPath = @"HKEY_CLASSES_ROOT\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\PackageRepository\Packages";

        public static string GetPackageName(string uwpName)
        {
            if(string.IsNullOrEmpty(uwpName)) return null;
            using(RegistryKey packageKey = RegistryEx.GetRegistryKey(PackageRegPath))
            {
                if(packageKey == null) return null;
                foreach(string packageName in packageKey.GetSubKeyNames())
                {
                    if(packageName.StartsWith(uwpName, StringComparison.OrdinalIgnoreCase))
                    {
                        return packageName;
                    }
                }
            }
            return null;
        }

        public static string GetRegPath(string uwpName, Guid guid)
        {
            string packageName = GetPackageName(uwpName);
            if(packageName == null) return null;
            else return $@"{PackageRegPath}\{packageName}\Class\{guid:B}";
        }

        public static string GetFilePath(string uwpName, Guid guid)
        {
            string regPath = GetRegPath(uwpName, guid);
            if(regPath == null) return null;
            string packageName = GetPackageName(uwpName);
            using(RegistryKey pKey = RegistryEx.GetRegistryKey($@"{PackagesRegPath}\{packageName}"))
            {
                if(pKey == null) return null;
                string dirPath = pKey.GetValue("Path")?.ToString();
                string dllPath = Registry.GetValue(regPath, "DllPath", null)?.ToString();
                string filePath = $@"{dirPath}\{dllPath}";
                if(File.Exists(filePath)) return filePath;
                string[] names = pKey.GetSubKeyNames();
                if(names.Length == 1)
                {
                    filePath = "shell:AppsFolder\\" + names[0];
                    return filePath;
                }
                if(Directory.Exists(dirPath)) return dirPath;
                return null;
            }
        }
    }
}