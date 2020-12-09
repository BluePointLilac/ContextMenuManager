using BulePointLilac.Controls;
using BulePointLilac.Methods;
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
            try
            {
                foreach(XmlNode xn in ReadXml().DocumentElement.ChildNodes)
                {
                    string path = null;
                    string text = null;
                    Image image = null;
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
                            image = ResourceIcon.GetIcon(xe.GetAttribute("Icon"))?.ToBitmap() ?? AppImage.NotFound;
                            break;
                    }
                    if(string.IsNullOrEmpty(path) || string.IsNullOrEmpty(text)) continue;
                    GroupPathItem groupItem = new GroupPathItem(path, ObjectPath.PathType.Registry)
                    {
                        Image = image,
                        Text = text,
                    };
                    this.AddItem(groupItem);
                    XmlElement shellXE = (XmlElement)xn.SelectSingleNode("Shell");
                    XmlElement shellExXE = (XmlElement)xn.SelectSingleNode("ShellEx");
                    if(shellXE != null) LoadShellItems(shellXE, groupItem);
                    if(shellExXE != null) LoadShellExItems(shellExXE, groupItem);
                    groupItem.IsFold = true;
                }
            }
            catch { }
        }

        private XmlDocument ReadXml()
        {
            XmlDocument doc1 = new XmlDocument();
            try
            {
                //如果没有网络下载的，则将程序内置的写入
                if(!File.Exists(AppConfig.WebEnhanceMenusDic))
                {
                    File.WriteAllText(AppConfig.WebEnhanceMenusDic, Properties.Resources.EnhanceMenusDic, Encoding.UTF8);
                }
                doc1.Load(AppConfig.WebEnhanceMenusDic);
                if(File.Exists(AppConfig.UserEnhanceMenusDic))
                {
                    XmlDocument doc2 = new XmlDocument();
                    doc2.Load(AppConfig.UserEnhanceMenusDic);
                    foreach(XmlNode xn in doc2.DocumentElement.ChildNodes)
                    {
                        XmlNode node = doc1.ImportNode(xn, true);
                        doc1.DocumentElement.AppendChild(node);
                    }
                }
            }
            catch { }
            return doc1;
        }

        private void LoadShellItems(XmlElement shellXE, GroupPathItem groupItem)
        {
            foreach(XmlElement itemXE in shellXE.GetElementsByTagName("Item"))
            {
                XmlElement verXE = (XmlElement)itemXE.SelectSingleNode("OSVersion");
                if(!JudgeOSVersion(verXE)) continue;
                XmlElement szXE = (XmlElement)itemXE.SelectSingleNode("Value/REG_SZ");
                string keyName = itemXE.GetAttribute("KeyName");
                if(keyName.IsNullOrWhiteSpace()) continue;
                string regPath = $@"{groupItem.TargetPath}\shell\{keyName}";
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
                }
                if(item.Image == null) item.Image = AppImage.NotFound;
                if(item.Text.IsNullOrWhiteSpace()) item.Text = keyName;
                item.ChkVisible.Checked = item.ItemVisible;
                MyToolTip.SetToolTip(item.ChkVisible, itemXE.GetAttribute("Tip"));
                this.AddItem(item);
            }
        }

        private void LoadShellExItems(XmlElement shellExXE, GroupPathItem groupItem)
        {
            foreach(XmlElement itemXE in shellExXE.GetElementsByTagName("Item"))
            {
                XmlElement verXE = (XmlElement)itemXE.SelectSingleNode("OSVersion");
                if(!JudgeOSVersion(verXE)) continue;
                if(!GuidInfo.TryGetGuid(itemXE.GetAttribute("Guid"), out Guid guid)) continue;
                EnhanceShellExItem item = new EnhanceShellExItem
                {
                    FoldGroupItem = groupItem,
                    ShellExPath = $@"{groupItem.TargetPath}\ShellEx",
                    Image = ResourceIcon.GetIcon(itemXE.GetAttribute("Icon"))?.ToBitmap() ?? AppImage.DllDefaultIcon,
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

        public static bool JudgeOSVersion(XmlElement osXE)
        {
            if(osXE == null) return true;
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
    }
}