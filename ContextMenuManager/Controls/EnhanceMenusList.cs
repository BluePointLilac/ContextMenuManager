using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class EnhanceMenusList : MyList
    {
        public void LoadItems()
        {
            string webPath = AppConfig.WebEnhanceMenusDic;
            string userPath = AppConfig.UserEnhanceMenusDic;
            string contents = Properties.Resources.EnhanceMenusDic;
            if(!File.Exists(webPath)) File.WriteAllText(webPath, contents, Encoding.Unicode);
            GroupPathItem webGroupItem = new GroupPathItem(webPath, ObjectPath.PathType.File);
            GroupPathItem userGroupItem = new GroupPathItem(userPath, ObjectPath.PathType.File);
            webGroupItem.Text = AppString.SideBar.Dictionaries;
            userGroupItem.Text = AppString.Other.UserDictionaries;
            webGroupItem.Image = AppImage.App;
            userGroupItem.Image = AppImage.User;
            LoadDocItems(webPath, webGroupItem);
            LoadDocItems(userPath, userGroupItem);
        }

        private void LoadDocItems(string xmlPath, GroupPathItem groupItem)
        {
            if(!File.Exists(xmlPath)) return;
            this.AddItem(groupItem);
            XmlDocument doc = new XmlDocument();
            try { doc.LoadXml(File.ReadAllText(xmlPath, EncodingType.GetType(xmlPath)).Trim()); }
            catch(Exception e) { MessageBoxEx.Show(e.Message); return; }
            foreach(XmlNode xn in doc.DocumentElement.ChildNodes)
            {
                try
                {
                    SubGroupItem subGroupItem = GetGroupPathItem(xn);
                    if(subGroupItem == null) continue;
                    this.AddItem(subGroupItem);
                    XmlElement shellXE = (XmlElement)xn.SelectSingleNode("Shell");
                    XmlElement shellExXE = (XmlElement)xn.SelectSingleNode("ShellEx");
                    if(shellXE != null) LoadShellItems(shellXE, subGroupItem);
                    if(shellExXE != null) LoadShellExItems(shellExXE, subGroupItem);
                    subGroupItem.HideWhenNoSubItem();
                    subGroupItem.FoldGroupItem = groupItem;
                }
                catch { continue; }
            }
            groupItem.IsFold = true;
            groupItem.HideWhenNoSubItem();
        }

        private SubGroupItem GetGroupPathItem(XmlNode xn)
        {
            string path;
            string text;
            Image image;
            switch(xn.Name)
            {
                case "File":
                    path = ShellList.MENUPATH_FILE;
                    text = AppString.SideBar.File;
                    image = AppImage.File;
                    break;
                case "Folder":
                    path = ShellList.MENUPATH_FOLDER;
                    text = AppString.SideBar.Folder;
                    image = AppImage.Folder;
                    break;
                case "Directory":
                    path = ShellList.MENUPATH_FOLDER;
                    text = AppString.SideBar.Directory;
                    image = AppImage.Directory;
                    break;
                case "Background":
                    path = ShellList.MENUPATH_BACKGROUND;
                    text = AppString.SideBar.Background;
                    image = AppImage.Background;
                    break;
                case "Desktop":
                    path = ShellList.MENUPATH_DESKTOP;
                    //Vista没有桌面右键菜单的独立注册表项
                    if(WindowsOsVersion.IsEqualVista) path = ShellList.MENUPATH_BACKGROUND;
                    text = AppString.SideBar.Desktop;
                    image = AppImage.Desktop;
                    break;
                case "Drive":
                    path = ShellList.MENUPATH_DRIVE;
                    text = AppString.SideBar.Drive;
                    image = AppImage.Drive;
                    break;
                case "AllObjects":
                    path = ShellList.MENUPATH_ALLOBJECTS;
                    text = AppString.SideBar.AllObjects;
                    image = AppImage.AllObjects;
                    break;
                case "Computer":
                    path = ShellList.MENUPATH_COMPUTER;
                    text = AppString.SideBar.Computer;
                    image = AppImage.Computer;
                    break;
                case "RecycleBin":
                    path = ShellList.MENUPATH_RECYCLEBIN;
                    text = AppString.SideBar.RecycleBin;
                    image = AppImage.RecycleBin;
                    break;
                default:
                    XmlElement xe = (XmlElement)xn;
                    path = xe.GetAttribute("RegPath");
                    text = ResourceString.GetDirectString(xe.GetAttribute("Text"));
                    if(string.IsNullOrEmpty(path) || string.IsNullOrEmpty(text)) return null;
                    using(Icon icon = ResourceIcon.GetIcon(xe.GetAttribute("Icon")))
                    {
                        if(icon != null) image = icon.ToBitmap();
                        else image = AppImage.NotFound;
                    }
                    break;
            }
            return new SubGroupItem(path, ObjectPath.PathType.Registry) { Image = image, Text = text };
        }

        private void LoadShellItems(XmlElement shellXE, SubGroupItem groupItem)
        {
            foreach(XmlElement itemXE in shellXE.SelectNodes("Item"))
            {
                if(!JudgeOSVersion(itemXE)) continue;
                if(!FileExists(itemXE)) continue;
                XmlElement szXE = (XmlElement)itemXE.SelectSingleNode("Value/REG_SZ");
                string keyName = itemXE.GetAttribute("KeyName");
                if(keyName.IsNullOrWhiteSpace()) continue;
                EnhanceShellItem item = new EnhanceShellItem()
                {
                    RegPath = $@"{groupItem.TargetPath}\shell\{keyName}",
                    FoldGroupItem = groupItem,
                    ItemXE = itemXE
                };
                if(szXE != null)
                {
                    item.Text = ResourceString.GetDirectString(szXE.GetAttribute("MUIVerb"));
                    if(szXE.HasAttribute("Icon")) item.Image = ResourceIcon.GetIcon(szXE.GetAttribute("Icon"))?.ToBitmap();
                    else if(szXE.HasAttribute("HasLUAShield")) item.Image = AppImage.Shield;
                    else
                    {
                        XmlElement cmdXE = (XmlElement)itemXE.SelectSingleNode("SubKey/Command");
                        if(cmdXE != null)
                        {
                            Icon icon = null;
                            if(cmdXE.HasAttribute("Default"))
                            {
                                string filePath = ObjectPath.ExtractFilePath(cmdXE.GetAttribute("Default"));
                                icon = ResourceIcon.GetIcon(filePath);
                            }
                            item.Image = icon?.ToBitmap();
                            icon?.Dispose();
                        }
                    }
                }
                if(item.Image == null) item.Image = AppImage.NotFound;
                if(item.Text.IsNullOrWhiteSpace()) item.Text = keyName;
                item.ChkVisible.Checked = item.ItemVisible;
                string tip = itemXE.GetAttribute("Tip");
                if(itemXE.GetElementsByTagName("CreateFile").Count > 0)
                {
                    if(!tip.IsNullOrWhiteSpace()) tip += "\n";
                    tip += AppString.Tip.CommandFiles;
                }
                MyToolTip.SetToolTip(item.ChkVisible, tip);
                this.AddItem(item);
            }
        }

        private void LoadShellExItems(XmlElement shellExXE, SubGroupItem groupItem)
        {
            foreach(XmlElement itemXE in shellExXE.SelectNodes("Item"))
            {
                if(!JudgeOSVersion(itemXE)) continue;
                if(!GuidEx.TryParse(itemXE.GetAttribute("Guid"), out Guid guid)) continue;
                EnhanceShellExItem item = new EnhanceShellExItem
                {
                    FoldGroupItem = groupItem,
                    ShellExPath = $@"{groupItem.TargetPath}\ShellEx",
                    Image = ResourceIcon.GetIcon(itemXE.GetAttribute("Icon"))?.ToBitmap() ?? AppImage.SystemFile,
                    Text = ResourceString.GetDirectString(itemXE.GetAttribute("Text")),
                    DefaultKeyName = itemXE.GetAttribute("KeyName"),
                    Guid = guid
                };
                if(item.Text.IsNullOrWhiteSpace()) item.Text = GuidInfo.GetText(guid);
                if(item.DefaultKeyName.IsNullOrWhiteSpace()) item.DefaultKeyName = guid.ToString("B");
                item.ChkVisible.Checked = item.ItemVisible;
                MyToolTip.SetToolTip(item.ChkVisible, itemXE.GetAttribute("Tip"));
                this.AddItem(item);
            }
        }

        public static bool JudgeOSVersion(XmlElement itemXE)
        {
            //return true;//测试用
            bool JudgeOne(XmlElement osXE)
            {
                Version ver = new Version(osXE.InnerText);
                Version osVer = Environment.OSVersion.Version;
                int compare = osVer.CompareTo(ver);
                string symbol = osXE.GetAttribute("Compare");
                switch(symbol)
                {
                    case ">":
                        return compare > 0;
                    case "<":
                        return compare < 0;
                    case "=":
                        return compare == 0;
                    case ">=":
                        return compare >= 0;
                    case "<=":
                        return compare <= 0;
                    default:
                        return true;
                }
            }

            foreach(XmlElement osXE in itemXE.SelectNodes("OSVersion"))
            {
                if(!JudgeOne(osXE)) return false;
            }
            return true;
        }

        private static bool FileExists(XmlElement itemXE)
        {
            //return true;//测试用
            foreach(XmlElement feXE in itemXE.SelectNodes("FileExists"))
            {
                string path = Environment.ExpandEnvironmentVariables(feXE.InnerText);
                if(!File.Exists(path)) return false;
            }
            return true;
        }

        public static byte[] ConvertToBinary(string value)
        {
            try
            {
                string[] strs = value.Split(' ');
                byte[] bs = new byte[strs.Length];
                for(int i = 0; i < strs.Length; i++)
                {
                    bs[i] = Convert.ToByte(strs[i], 16);
                }
                return bs;
            }
            catch { return null; }
        }
    }
}