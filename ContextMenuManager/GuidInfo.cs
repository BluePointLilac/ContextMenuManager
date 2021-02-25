using BluePointLilac.Methods;
using ContextMenuManager.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace ContextMenuManager
{
    static class GuidInfo
    {
        public static readonly string[] ClsidPaths = {
            @"HKEY_CLASSES_ROOT\CLSID",
            @"HKEY_CLASSES_ROOT\WOW6432Node\CLSID",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Classes\CLSID",
        };

        public struct IconLocation
        {
            public string IconPath { get; set; }
            public int IconIndex { get; set; }
        }

        private static readonly IniReader AppDic = new IniReader(new StringBuilder(Properties.Resources.GuidInfosDic));
        public static readonly IniReader UserDic = new IniReader(AppConfig.UserGuidInfosDic);
        public static readonly IniReader WebDic = new IniReader(AppConfig.WebGuidInfosDic);
        public static readonly Dictionary<Guid, IconLocation> IconLocationDic = new Dictionary<Guid, IconLocation>();
        private static readonly Dictionary<Guid, string> FilePathDic = new Dictionary<Guid, string>();
        public static readonly Dictionary<Guid, string> ItemTextDic = new Dictionary<Guid, string>();
        public static readonly Dictionary<Guid, Image> ItemImageDic = new Dictionary<Guid, Image>();
        public static readonly Dictionary<Guid, string> UwpNameDic = new Dictionary<Guid, string>();

        private static bool TryGetValue(Guid guid, string key, out string value)
        {
            //用户自定义字典优先
            string section = guid.ToString();
            if(UserDic.TryGetValue(section, key, out value)) return true;
            if(WebDic.TryGetValue(section, key, out value)) return true;
            if(AppDic.TryGetValue(section, key, out value)) return true;
            return false;
        }

        public static string GetFilePath(Guid guid)
        {
            string filePath = null;
            if(guid.Equals(Guid.Empty)) return filePath;
            if(FilePathDic.ContainsKey(guid)) filePath = FilePathDic[guid];
            else
            {
                string uwpName = GetUwpName(guid);
                if(!string.IsNullOrEmpty(uwpName))
                {
                    filePath = UwpModeItem.GetFilePath(uwpName, guid);
                }
                else
                {
                    foreach(string clsidPath in ClsidPaths)
                    {
                        using(RegistryKey guidKey = RegistryEx.GetRegistryKey($@"{clsidPath}\{guid:B}"))
                        {
                            if(guidKey == null) continue;
                            foreach(string keyName in new[] { "InprocServer32", "LocalServer32" })
                            {
                                using(RegistryKey key = guidKey.OpenSubKey(keyName))
                                {
                                    if(key == null) continue;
                                    string value1 = key.GetValue("CodeBase")?.ToString()?.Replace("file:///", "")?.Replace('/', '\\');
                                    if(File.Exists(value1))
                                    {
                                        filePath = value1; break;
                                    }
                                    string value2 = key.GetValue("")?.ToString();
                                    value2 = ObjectPath.ExtractFilePath(value2);
                                    if(File.Exists(value2))
                                    {
                                        filePath = value2; break;
                                    }
                                }
                            }
                            if(File.Exists(filePath)) break;
                        }
                    }
                }
                FilePathDic.Add(guid, filePath);
            }
            return filePath;
        }

        public static string GetText(Guid guid)
        {
            string itemText = null;
            if(guid.Equals(Guid.Empty)) return itemText;
            if(ItemTextDic.ContainsKey(guid)) itemText = ItemTextDic[guid];
            else
            {
                if(TryGetValue(guid, "Text", out itemText))
                {
                    itemText = GetAbsStr(guid, itemText, true);
                    itemText = ResourceString.GetDirectString(itemText);
                }
                if(itemText.IsNullOrWhiteSpace())
                {
                    foreach(string clsidPath in ClsidPaths)
                    {
                        foreach(string value in new[] { "LocalizedString", "InfoTip", "" })
                        {
                            itemText = Registry.GetValue($@"{clsidPath}\{guid:B}", value, null)?.ToString();
                            itemText = ResourceString.GetDirectString(itemText);
                            if(!itemText.IsNullOrWhiteSpace()) break;
                        }
                        if(!itemText.IsNullOrWhiteSpace()) break;
                    }
                }
                if(itemText.IsNullOrWhiteSpace())
                {
                    string filePath = GetFilePath(guid);
                    if(File.Exists(filePath))
                    {
                        itemText = FileVersionInfo.GetVersionInfo(filePath).FileDescription;
                        if(itemText.IsNullOrWhiteSpace())
                        {
                            itemText = Path.GetFileName(filePath);
                        }
                    }
                    else itemText = null;
                }
                ItemTextDic.Add(guid, itemText);
            }
            return itemText;
        }

        public static Image GetImage(Guid guid)
        {
            if(ItemImageDic.TryGetValue(guid, out Image image)) return image;
            IconLocation location = GetIconLocation(guid);
            string iconPath = location.IconPath;
            int iconIndex = location.IconIndex;
            if(iconPath == null && iconIndex == 0) image = AppImage.DllDefaultIcon;
            else if(Path.GetFileName(iconPath).ToLower() == "shell32.dll" && iconIndex == 0) image = AppImage.DllDefaultIcon;
            else image = ResourceIcon.GetIcon(iconPath, iconIndex)?.ToBitmap() ?? AppImage.DllDefaultIcon;
            ItemImageDic.Add(guid, image);
            return image;
        }

        public static IconLocation GetIconLocation(Guid guid)
        {
            IconLocation location = new IconLocation();
            if(guid.Equals(Guid.Empty)) return location;
            if(IconLocationDic.ContainsKey(guid)) location = IconLocationDic[guid];
            else
            {
                if(TryGetValue(guid, "Icon", out string value))
                {
                    value = GetAbsStr(guid, value, false);
                    int index = value.LastIndexOf(',');
                    if(int.TryParse(value.Substring(index + 1), out int iconIndex))
                    {
                        location.IconPath = value.Substring(0, index);
                        location.IconIndex = iconIndex;
                    }
                    else location.IconPath = value;
                }
                else location.IconPath = GetFilePath(guid);
                IconLocationDic.Add(guid, location);
            }
            return location;
        }

        public static string GetUwpName(Guid guid)
        {
            string uwpName = null;
            if(guid.Equals(Guid.Empty)) return uwpName;
            if(UwpNameDic.ContainsKey(guid)) uwpName = UwpNameDic[guid];
            else
            {
                TryGetValue(guid, "UwpName", out uwpName);
                UwpNameDic.Add(guid, uwpName);
            }
            return uwpName;
        }

        private static string GetAbsStr(Guid guid, string relStr, bool isName)
        {
            string absStr = relStr;
            if(isName)
            {
                if(!relStr.StartsWith("@")) return absStr;
                else absStr = relStr.Substring(1);
            }
            string filePath = GetFilePath(guid);
            if(filePath == null) return absStr;
            string dirPath = Path.GetDirectoryName(filePath);
            if(absStr.StartsWith("*"))
            {
                absStr = filePath + absStr.Substring(1);
            }
            else if(absStr.StartsWith(".\\"))
            {
                absStr = dirPath + absStr.Substring(1);
            }
            else if(absStr.StartsWith("..\\"))
            {
                do
                {
                    dirPath = Path.GetDirectoryName(dirPath);
                    absStr = absStr.Substring(3);
                } while(absStr.StartsWith("..\\"));
                absStr = dirPath + "\\" + absStr;
            }
            if(isName) absStr = "@" + absStr;
            return absStr;
        }
    }
}