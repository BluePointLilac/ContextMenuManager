using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;

namespace ContextMenuManager.Methods
{
    static class UwpHelper
    {
        private const string PackageRegPath = @"HKEY_CLASSES_ROOT\PackagedCom\Package";
        private const string PackagesRegPath = @"HKEY_CLASSES_ROOT\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\PackageRepository\Packages";

        internal sealed class ContextMenuRegistration
        {
            public string PackageName { get; set; }
            public Guid Guid { get; set; }
            public string ItemType { get; set; }
            public string Text { get; set; }
            public string RegPath => $@"{PackageRegPath}\{PackageName}\Class\{Guid:B}";
        }

        // Only classes explicitly declared as context-menu verbs are menus. Other
        // packaged COM classes may provide unrelated application functionality.
        internal static List<ContextMenuRegistration> ReadContextMenus(XmlDocument manifest, string packageName)
        {
            List<ContextMenuRegistration> items = new List<ContextMenuRegistration>();
            XmlNamespaceManager ns = new XmlNamespaceManager(manifest.NameTable);
            ns.AddNamespace("d4", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/4");
            ns.AddNamespace("d5", "http://schemas.microsoft.com/appx/manifest/desktop/windows10/5");
            const string query = "//d4:Extension[@Category='windows.fileExplorerContextMenus']/d4:FileExplorerContextMenus/"
                + "*[self::d4:ItemType or self::d5:ItemType]/*[self::d4:Verb or self::d5:Verb]";
            foreach(XmlElement verb in manifest.SelectNodes(query, ns))
            {
                Guid guid;
                if(!GuidEx.TryParse(verb.GetAttribute("Clsid"), out guid) || guid == Guid.Empty) continue;
                string itemType = ((XmlElement)verb.ParentNode).GetAttribute("Type");
                if(string.IsNullOrEmpty(itemType)) continue;
                string name = packageName.Split('_')[0];
                string id = verb.GetAttribute("Id");
                items.Add(new ContextMenuRegistration
                {
                    PackageName = packageName, Guid = guid, ItemType = itemType,
                    Text = string.IsNullOrEmpty(id) ? name : name + " (" + id + ")"
                });
            }
            return items;
        }

        public static List<ContextMenuRegistration> GetContextMenus()
        {
            List<ContextMenuRegistration> items = new List<ContextMenuRegistration>();
            using(RegistryKey packages = Registry.ClassesRoot.OpenSubKey(@"PackagedCom\Package"))
            {
                if(packages == null) return items;
                foreach(string packageName in packages.GetSubKeyNames())
                {
                    try
                    {
                        string directory = Registry.GetValue($@"{PackagesRegPath}\{packageName}", "Path", null) as string;
                        if(string.IsNullOrEmpty(directory)) continue;
                        XmlDocument manifest = new XmlDocument { XmlResolver = null };
                        // DTD processing is prohibited by default, including on .NET 3.5.
                        XmlReaderSettings settings = new XmlReaderSettings { XmlResolver = null };
                        using(XmlReader reader = XmlReader.Create(Path.Combine(directory, "AppxManifest.xml"), settings))
                            manifest.Load(reader);
                        foreach(ContextMenuRegistration item in ReadContextMenus(manifest, packageName))
                        {
                            using(RegistryKey key = packages.OpenSubKey($@"{packageName}\Class\{item.Guid:B}"))
                            {
                                if(key != null) items.Add(item);
                            }
                        }
                    }
                    // A stale or inaccessible package must not hide other menus.
                    catch(IOException) { }
                    catch(UnauthorizedAccessException) { }
                    catch(SecurityException) { }
                    catch(XmlException) { }
                }
            }
            return items;
        }

        internal static bool MatchesScene(string itemType, string scenePath)
        {
            const string root = @"HKEY_CLASSES_ROOT\";
            if(scenePath == null || !scenePath.StartsWith(root, StringComparison.OrdinalIgnoreCase)) return false;
            string type = scenePath.Substring(root.Length);
            const string associations = @"SystemFileAssociations\";
            if(type.StartsWith(associations, StringComparison.OrdinalIgnoreCase))
                type = type.Substring(associations.Length);
            return string.Equals(itemType, type, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetPackageName(string uwpName)
        {
            if(string.IsNullOrEmpty(uwpName)) return null;
            using(RegistryKey packageKey = RegistryEx.GetRegistryKey(PackageRegPath))
            {
                if(packageKey == null) return null;
                foreach(string packageName in packageKey.GetSubKeyNames())
                {
                    if(packageName.Equals(uwpName, StringComparison.OrdinalIgnoreCase)
                        || packageName.StartsWith(uwpName + "_", StringComparison.OrdinalIgnoreCase))
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
                string filePath = string.IsNullOrEmpty(dllPath) ? null
                    : Path.IsPathRooted(dllPath) ? dllPath
                    : string.IsNullOrEmpty(dirPath) ? null : Path.Combine(dirPath, dllPath);
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
