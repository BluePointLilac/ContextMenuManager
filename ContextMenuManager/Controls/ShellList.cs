using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellList : MyList
    {
        public const string MENUPATH_FILE = @"HKEY_CLASSES_ROOT\*";//文件
        public const string MENUPATH_FOLDER = @"HKEY_CLASSES_ROOT\Folder";//文件夹
        public const string MENUPATH_DIRECTORY = @"HKEY_CLASSES_ROOT\Directory";//目录
        public const string MENUPATH_BACKGROUND = @"HKEY_CLASSES_ROOT\Directory\Background";//目录背景
        public const string MENUPATH_DESKTOP = @"HKEY_CLASSES_ROOT\DesktopBackground";//桌面背景
        public const string MENUPATH_DRIVE = @"HKEY_CLASSES_ROOT\Drive";//磁盘分区
        public const string MENUPATH_ALLOBJECTS = @"HKEY_CLASSES_ROOT\AllFilesystemObjects";//所有对象
        public const string MENUPATH_COMPUTER = @"HKEY_CLASSES_ROOT\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}";//此电脑
        public const string MENUPATH_RECYCLEBIN = @"HKEY_CLASSES_ROOT\CLSID\{645FF040-5081-101B-9F08-00AA002F954E}";//回收站
        public const string MENUPATH_LIBRARY = @"HKEY_CLASSES_ROOT\LibraryFolder";//库
        public const string MENUPATH_LIBRARY_BACKGROUND = @"HKEY_CLASSES_ROOT\LibraryFolder\Background";//库背景
        public const string MENUPATH_LIBRARY_USER = @"HKEY_CLASSES_ROOT\UserLibraryFolder";//用户库
        public const string MENUPATH_UWPLNK = @"HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication";//UWP快捷方式
        public const string MENUPATH_UNKNOWN = @"HKEY_CLASSES_ROOT\Unknown";//未知格式
        public const string SYSFILEASSPATH = @"HKEY_CLASSES_ROOT\SystemFileAssociations";//系统扩展名注册表父项路径

        public enum Scenes
        {
            File, Folder, Directory, Background, Desktop, Drive, AllObjects, Computer, RecycleBin, Library,
            LnkFile, UwpLnk, ExeFile, UnknownType, CustomExtension, PerceivedType, DirectoryType, CommandStore, DragDrop
        }

        private static readonly string[] DirectoryTypes = { "Document", "Image", "Video", "Audio" };
        private static readonly string[] PerceivedTypes = { "Text", "Document", "Image", "Video", "Audio", "Compressed", "System" };
        private static readonly string[] DirectoryTypeNames =
        {
            AppString.Dialog.DocumentDirectory, AppString.Dialog.ImageDirectory,
            AppString.Dialog.VideoDirectory, AppString.Dialog.AudioDirectory
        };
        private static readonly string[] PerceivedTypeNames =
        {
            AppString.Dialog.TextFile, AppString.Dialog.DocumentFile, AppString.Dialog.ImageFile, AppString.Dialog.VideoFile,
            AppString.Dialog.AudioFile, AppString.Dialog.CompressedFile, AppString.Dialog.SystemFile
        };

        private static string GetDirectoryTypeName()
        {
            if(CurrentDirectoryType != null)
            {
                for(int i = 0; i < DirectoryTypes.Length; i++)
                {
                    if(CurrentDirectoryType.Equals(DirectoryTypes[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return DirectoryTypeNames[i];
                    }
                }
            }
            return null;
        }

        private static string GetPerceivedTypeName()
        {
            if(CurrentPerceivedType != null)
            {
                for(int i = 0; i < PerceivedTypes.Length; i++)
                {
                    if(CurrentPerceivedType.Equals(PerceivedTypes[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return PerceivedTypeNames[i];
                    }
                }
            }
            return null;
        }

        private static string CurrentExtension = null;
        private static string CurrentDirectoryType = null;
        private static string CurrentPerceivedType = null;

        private static string GetShellPath(string scenePath) => $@"{scenePath}\shell";
        private static string GetShellExPath(string scenePath) => $@"{scenePath}\shellEx";
        private static string GetSysAssExtPath(string typeName) => typeName != null ? $@"{SYSFILEASSPATH}\{typeName}" : null;
        private static string GetOpenModePath(string extension) => extension != null ? $@"HKEY_CLASSES_ROOT\{FileExtension.GetOpenMode(extension)}" : null;

        public Scenes Scene { get; set; }

        public ShellList()
        {
            SelectItem.SelectedChanged += (sender, e) => { this.ClearItems(); this.LoadItems(); };
        }

        public void LoadItems()
        {
            string scenePath = null;
            switch(Scene)
            {
                case Scenes.File:
                    scenePath = MENUPATH_FILE; break;
                case Scenes.Folder:
                    scenePath = MENUPATH_FOLDER; break;
                case Scenes.Directory:
                    scenePath = MENUPATH_DIRECTORY; break;
                case Scenes.Background:
                    scenePath = MENUPATH_BACKGROUND; break;
                case Scenes.Desktop:
                    //Vista系统没有这一项
                    if(WindowsOsVersion.IsEqualVista) return;
                    scenePath = MENUPATH_DESKTOP; break;
                case Scenes.Drive:
                    scenePath = MENUPATH_DRIVE; break;
                case Scenes.AllObjects:
                    scenePath = MENUPATH_ALLOBJECTS; break;
                case Scenes.Computer:
                    scenePath = MENUPATH_COMPUTER; break;
                case Scenes.RecycleBin:
                    scenePath = MENUPATH_RECYCLEBIN; break;
                case Scenes.Library:
                    //Vista系统没有这一项
                    if(WindowsOsVersion.IsEqualVista) return;
                    scenePath = MENUPATH_LIBRARY; break;
                case Scenes.LnkFile:
                    scenePath = GetSysAssExtPath(".lnk"); break;
                case Scenes.UwpLnk:
                    //Win8之前没有Uwp
                    if(WindowsOsVersion.IsBefore8) return;
                    scenePath = MENUPATH_UWPLNK; break;
                case Scenes.ExeFile:
                    scenePath = GetSysAssExtPath(".exe"); break;
                case Scenes.UnknownType:
                    scenePath = MENUPATH_UNKNOWN; break;
                case Scenes.CustomExtension:
                    scenePath = GetSysAssExtPath(CurrentExtension); break;
                case Scenes.PerceivedType:
                    scenePath = GetSysAssExtPath(CurrentPerceivedType); break;
                case Scenes.DirectoryType:
                    if(CurrentDirectoryType == null) scenePath = null;
                    else scenePath = GetSysAssExtPath($"Directory.{CurrentDirectoryType}"); break;
                case Scenes.CommandStore:
                    //Vista系统没有这一项
                    if(WindowsOsVersion.IsEqualVista) return;
                    this.AddNewItem(RegistryEx.GetParentPath(ShellItem.CommandStorePath));
                    this.LoadStoreItems();
                    return;
                case Scenes.DragDrop:
                    this.AddNewItem(MENUPATH_FOLDER);
                    this.LoadShellExItems(GetShellExPath(MENUPATH_FOLDER));
                    this.LoadShellExItems(GetShellExPath(MENUPATH_DIRECTORY));
                    this.LoadShellExItems(GetShellExPath(MENUPATH_DRIVE));
                    this.LoadShellExItems(GetShellExPath(MENUPATH_ALLOBJECTS));
                    return;
            }
            this.AddNewItem(scenePath);
            this.LoadItems(scenePath);
            switch(Scene)
            {
                case Scenes.File:
                    bool flag = WindowsOsVersion.ISAfterOrEqual10;
                    if(flag)
                    {
                        using(RegistryKey key = RegistryEx.GetRegistryKey(@"HKEY_CLASSES_ROOT\PackagedCom\Package"))
                        {
                            flag = key != null && key.GetSubKeyNames().ToList().Any(name => name.StartsWith("Microsoft.SkypeApp", StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    if(flag) this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.ShareWithSkype));
                    break;
                case Scenes.Background:
                    this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.CustomFolder));
                    break;
                case Scenes.Computer:
                    this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.NetworkDrive));
                    break;
                case Scenes.RecycleBin:
                    this.AddItem(new VisibleRegRuleItem(VisibleRegRuleItem.RecycleBinProperties));
                    break;
                case Scenes.Library:
                    this.LoadItems(MENUPATH_LIBRARY_BACKGROUND);
                    this.LoadItems(MENUPATH_LIBRARY_USER);
                    break;
                case Scenes.LnkFile:
                    this.LoadItems(GetOpenModePath(".lnk"));
                    break;
                case Scenes.ExeFile:
                    this.LoadItems(GetOpenModePath(".exe"));
                    break;
                case Scenes.CustomExtension:
                case Scenes.PerceivedType:
                case Scenes.DirectoryType:
                    this.InsertItem(new SelectItem(Scene), 0);
                    if(Scene == Scenes.CustomExtension) this.LoadItems(GetOpenModePath(CurrentExtension));
                    break;
            }
        }

        private void LoadItems(string scenePath)
        {
            if(scenePath == null) return;
            RegTrustedInstaller.TakeRegKeyOwnerShip(scenePath);
            this.LoadShellItems(GetShellPath(scenePath));
            this.LoadShellExItems(GetShellExPath(scenePath));
        }

        private void LoadShellItems(string shellPath)
        {
            using(RegistryKey shellKey = RegistryEx.GetRegistryKey(shellPath))
            {
                if(shellKey == null) return;
                RegTrustedInstaller.TakeRegTreeOwnerShip(shellKey.Name);
                Array.ForEach(shellKey.GetSubKeyNames(), keyName =>
                {
                    this.AddItem(new ShellItem($@"{shellPath}\{keyName}"));
                });
            }
        }

        private void LoadShellExItems(string shellExPath)
        {
            List<string> names = new List<string>();
            using(RegistryKey shellExKey = RegistryEx.GetRegistryKey(shellExPath))
            {
                if(shellExKey == null) return;
                bool isDragDrop = Scene == Scenes.DragDrop;
                RegTrustedInstaller.TakeRegTreeOwnerShip(shellExKey.Name);
                Dictionary<string, Guid> dic = ShellExItem.GetPathAndGuids(shellExPath, isDragDrop);
                GroupPathItem groupItem = null;
                if(isDragDrop)
                {
                    groupItem = GetDragDropGroupItem(shellExPath);
                    this.AddItem(groupItem);
                }
                foreach(string path in dic.Keys)
                {
                    string keyName = RegistryEx.GetKeyName(path);
                    if(!names.Contains(keyName))
                    {
                        ShellExItem item = new ShellExItem(dic[path], path);
                        if(groupItem != null) item.FoldGroupItem = groupItem;
                        this.AddItem(item);
                        names.Add(keyName);
                    }
                }
                if(groupItem != null) groupItem.IsFold = true;
            }
        }

        private GroupPathItem GetDragDropGroupItem(string shellExPath)
        {
            string text = null;
            Image image = null;
            string path = shellExPath.Substring(0, shellExPath.LastIndexOf('\\'));
            switch(path)
            {
                case MENUPATH_FOLDER:
                    text = AppString.SideBar.Folder;
                    image = AppImage.Folder;
                    break;
                case MENUPATH_DIRECTORY:
                    text = AppString.SideBar.Directory;
                    image = AppImage.Directory;
                    break;
                case MENUPATH_DRIVE:
                    text = AppString.SideBar.Drive;
                    image = AppImage.Drive;
                    break;
                case MENUPATH_ALLOBJECTS:
                    text = AppString.SideBar.AllObjects;
                    image = AppImage.AllObjects;
                    break;
            }
            return new GroupPathItem(shellExPath, ObjectPath.PathType.Registry) { Text = text, Image = image };
        }

        private void AddNewItem(string scenePath)
        {
            NewItem newItem = new NewItem { Visible = scenePath != null };
            this.AddItem(newItem);
            newItem.AddNewItem += (sender, e) =>
            {
                bool isShell;
                if(Scene == Scenes.DragDrop) isShell = false;
                else
                {
                    using(SelectDialog dlg = new SelectDialog())
                    {
                        dlg.Items = new[] { "Shell", "ShellEx" };
                        dlg.Title = "请选择新建菜单类型";
                        dlg.Selected = dlg.Items[0];
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        isShell = dlg.SelectedIndex == 0;
                    }
                }
                if(isShell) this.AddNewShellItem(scenePath);
                else this.AddNewShellExItem(scenePath);
            };
        }

        private void AddNewShellItem(string scenePath)
        {
            string shellPath = GetShellPath(scenePath);
            using(NewShellDialog dlg = new NewShellDialog())
            {
                dlg.ScenePath = scenePath;
                dlg.ShellPath = shellPath;
                if(dlg.ShowDialog() != DialogResult.OK) return;
                for(int i = 0; i < this.Controls.Count; i++)
                {
                    if(this.Controls[i] is NewItem)
                    {
                        this.InsertItem(new ShellItem(dlg.NewItemRegPath), i + 1);
                        break;
                    }
                }
            }
        }

        private void AddNewShellExItem(string scenePath)
        {
            bool isDragDrop = Scene == Scenes.DragDrop;
            using(InputDialog dlg1 = new InputDialog { Title = AppString.Dialog.InputGuid })
            {
                if(GuidEx.TryParse(Clipboard.GetText(), out Guid guid)) dlg1.Text = guid.ToString();
                if(dlg1.ShowDialog() != DialogResult.OK) return;
                if(GuidEx.TryParse(dlg1.Text, out guid))
                {
                    if(isDragDrop)
                    {
                        using(SelectDialog dlg2 = new SelectDialog())
                        {
                            dlg2.Title = AppString.Dialog.SelectGroup;
                            dlg2.Items = new[] { AppString.SideBar.Folder, AppString.SideBar.Directory,
                                        AppString.SideBar.Drive, AppString.SideBar.AllObjects };
                            dlg2.Selected = dlg2.Items[0];
                            if(dlg2.ShowDialog() != DialogResult.OK) return;
                            switch(dlg2.SelectedIndex)
                            {
                                case 0:
                                    scenePath = MENUPATH_FOLDER; break;
                                case 1:
                                    scenePath = MENUPATH_DIRECTORY; break;
                                case 2:
                                    scenePath = MENUPATH_DRIVE; break;
                                case 3:
                                    scenePath = MENUPATH_ALLOBJECTS; break;
                            }
                        }
                    }
                    string shellExPath = GetShellExPath(scenePath);
                    if(ShellExItem.GetPathAndGuids(shellExPath, isDragDrop).Values.Contains(guid))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.HasBeenAdded);
                    }
                    else
                    {
                        string part = isDragDrop ? ShellExItem.DdhParts[0] : ShellExItem.CmhParts[0];
                        string regPath = $@"{shellExPath}\{part}\{guid:B}";
                        Registry.SetValue(regPath, "", guid.ToString("B"));
                        ShellExItem item = new ShellExItem(guid, regPath);
                        for(int i = 0; i < this.Controls.Count; i++)
                        {
                            if(isDragDrop)
                            {
                                if(this.Controls[i] is GroupPathItem groupItem)
                                {
                                    if(groupItem.TargetPath.Equals(shellExPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        this.InsertItem(item, i + 1);
                                        item.FoldGroupItem = groupItem;
                                        item.Visible = !groupItem.IsFold;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if(this.Controls[i] is NewItem)
                                {
                                    this.InsertItem(item, i + 1);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBoxEx.Show(AppString.MessageBox.MalformedGuid);
                }
            }
        }

        ///<summary>“其他规则”-“公共引用”</summary>
        private void LoadStoreItems()
        {
            using(var shellKey = RegistryEx.GetRegistryKey(ShellItem.CommandStorePath))
            {
                Array.ForEach(Array.FindAll(shellKey.GetSubKeyNames(), itemName =>
                    !ShellItem.SysStoreItemNames.Contains(itemName, StringComparer.OrdinalIgnoreCase)), itemName =>
                    {
                        this.AddItem(new StoreShellItem($@"{ShellItem.CommandStorePath}\{itemName}", true, false));
                    });
            }
        }

        sealed class SelectItem : MyListItem
        {
            static string selected;
            public static string Selected
            {
                get => selected;
                set
                {
                    selected = value;
                    SelectedChanged?.Invoke(null, null);
                }
            }

            public static event EventHandler SelectedChanged;

            readonly PictureButton BtnSelect = new PictureButton(AppImage.Select);

            public SelectItem(Scenes scene)
            {
                this.SetNoClickEvent();
                this.Image = AppImage.Custom;
                this.Text = this.GetText(scene);
                this.AddCtr(BtnSelect);
                BtnSelect.MouseDown += (sender, e) => Select(scene);
            }

            private string GetText(Scenes scene)
            {
                switch(scene)
                {
                    case Scenes.CustomExtension:
                        if(CurrentExtension == null)
                        {
                            return AppString.Item.SelectExtension;
                        }
                        else
                        {
                            return AppString.Item.CurrentExtension.Replace("%s", CurrentExtension);
                        }
                    case Scenes.PerceivedType:
                        if(CurrentPerceivedType == null)
                        {
                            return AppString.Item.SelectPerceivedType;
                        }
                        else
                        {
                            return AppString.Item.CurrentPerceivedType.Replace("%s", GetPerceivedTypeName());
                        }
                    case Scenes.DirectoryType:
                        if(CurrentDirectoryType == null)
                        {
                            return AppString.Item.SelectDirectoryType;
                        }
                        else
                        {
                            return AppString.Item.CurrentDirectoryType.Replace("%s", GetDirectoryTypeName());
                        }
                    default:
                        return null;
                }
            }

            private void Select(Scenes scene)
            {
                SelectDialog dlg;
                switch(scene)
                {
                    case Scenes.CustomExtension:
                        dlg = new FileExtensionDialog
                        {
                            Selected = CurrentExtension?.Substring(1)
                        };
                        break;
                    case Scenes.PerceivedType:
                        dlg = new SelectDialog
                        {
                            Items = PerceivedTypeNames,
                            Title = AppString.Item.SelectPerceivedType,
                            Selected = GetPerceivedTypeName() ?? PerceivedTypeNames[0]
                        };
                        break;
                    case Scenes.DirectoryType:
                        dlg = new SelectDialog
                        {
                            Items = DirectoryTypeNames,
                            Title = AppString.Item.SelectDirectoryType,
                            Selected = GetDirectoryTypeName() ?? DirectoryTypeNames[0]
                        };
                        break;
                    default: return;
                }
                if(dlg.ShowDialog() != DialogResult.OK) return;
                switch(scene)
                {
                    case Scenes.CustomExtension:
                        Selected = CurrentExtension = dlg.Selected;
                        break;
                    case Scenes.PerceivedType:
                        Selected = CurrentPerceivedType = PerceivedTypes[dlg.SelectedIndex];
                        break;
                    case Scenes.DirectoryType:
                        Selected = CurrentDirectoryType = DirectoryTypes[dlg.SelectedIndex];
                        break;
                }
            }
        }
    }
}