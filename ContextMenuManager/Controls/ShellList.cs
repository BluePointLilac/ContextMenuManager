using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public const string MENUPATH_LNKFILE = @"HKEY_CLASSES_ROOT\lnkfile";//快捷方式
        public const string MENUPATH_EXEFILE = @"HKEY_CLASSES_ROOT\exefile";//可执行文件
        public const string MENUPATH_SYSLNKFILE = @"HKEY_CLASSES_ROOT\SystemFileAssociations\.lnk";
        public const string MENUPATH_SYSEXEFILE = @"HKEY_CLASSES_ROOT\SystemFileAssociations\.exe";
        public const string MENUPATH_UWPLNK = @"HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication";
        public const string MENUPATH_UNKNOWN = @"HKEY_CLASSES_ROOT\Unknown";//未知格式
        public const string MENUPATH_TEXT = @"HKEY_CLASSES_ROOT\SystemFileAssociations\text";//通用文本文件
        public const string MENUPATH_IMAGE = @"HKEY_CLASSES_ROOT\SystemFileAssociations\image";//通用图像文件
        public const string MENUPATH_VIDEO = @"HKEY_CLASSES_ROOT\SystemFileAssociations\video";//通用视频文件
        public const string MENUPATH_AUDIO = @"HKEY_CLASSES_ROOT\SystemFileAssociations\audio";//通用音频文件
        public const string MENUPATH_DIRECTORY_IMAGE = @"HKEY_CLASSES_ROOT\SystemFileAssociations\Directory.Image";//通用图像文件目录
        public const string MENUPATH_DIRECTORY_VIDEO = @"HKEY_CLASSES_ROOT\SystemFileAssociations\Directory.Video";//通用视频文件目录
        public const string MENUPATH_DIRECTORY_AUDIO = @"HKEY_CLASSES_ROOT\SystemFileAssociations\Directory.Audio";//通用音频文件目录
        public const string SYSFILEASSPATH = @"HKEY_CLASSES_ROOT\SystemFileAssociations";//系统扩展名注册表父项路径

        public enum Scenes
        {
            File, Folder, Directory, Background, Desktop, Drive, AllObjects, Computer, RecycleBin,
            Library, LnkFile, UwpLnk, ExeFile, TextFile, ImageFile, VideoFile, AudioFile,
            ImageDirectory, VideoDirectory, AudioDirectory, UnknownType, CustomType
        }

        private Scenes scene;
        public Scenes Scene
        {
            get => scene;
            set { scene = value; LoadItems(); }
        }

        private static string GetShellPath(string scenePath) => $@"{scenePath}\shell";
        private static string GetShellExPath(string scenePath) => $@"{scenePath}\shellEx";

        public ShellList()
        {
            TypeItem.ExtensionChanged += (sender, e) => this.Scene = Scenes.CustomType;
        }

        private void LoadItems()
        {
            this.ClearItems();
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
                    scenePath = MENUPATH_LIBRARY; break;
                case Scenes.LnkFile:
                    scenePath = MENUPATH_LNKFILE; break;
                case Scenes.UwpLnk:
                    scenePath = MENUPATH_UWPLNK; break;
                case Scenes.ExeFile:
                    scenePath = MENUPATH_EXEFILE; break;
                case Scenes.TextFile:
                    scenePath = MENUPATH_TEXT; break;
                case Scenes.ImageFile:
                    scenePath = MENUPATH_IMAGE; break;
                case Scenes.VideoFile:
                    scenePath = MENUPATH_VIDEO; break;
                case Scenes.AudioFile:
                    scenePath = MENUPATH_AUDIO; break;
                case Scenes.ImageDirectory:
                    scenePath = MENUPATH_DIRECTORY_IMAGE; break;
                case Scenes.VideoDirectory:
                    scenePath = MENUPATH_DIRECTORY_VIDEO; break;
                case Scenes.AudioDirectory:
                    scenePath = MENUPATH_DIRECTORY_AUDIO; break;
                case Scenes.UnknownType:
                    scenePath = MENUPATH_UNKNOWN; break;
                case Scenes.CustomType:
                    scenePath = TypeItem.SysAssExtPath; break;
            }
            this.AddNewItem(scenePath);
            this.AddItems(scenePath);

            switch(scene)
            {
                case Scenes.File:
                    this.AddItem(new SpecialItems.SkypeShareItem());
                    break;
                case Scenes.Background:
                    this.AddItem(new RegRuleItem(RegRuleItem.CustomFolder) { MarginRight = RegRuleItem.SysMarginRignt });
                    break;
                case Scenes.Computer:
                    this.AddItem(new RegRuleItem(RegRuleItem.NetworkDrive) { MarginRight = RegRuleItem.SysMarginRignt });
                    break;
                case Scenes.RecycleBin:
                    this.AddItem(new RegRuleItem(RegRuleItem.RecycleBinProperties) { MarginRight = RegRuleItem.SysMarginRignt });
                    break;
                case Scenes.Library:
                    this.AddItems(MENUPATH_LIBRARY_BACKGROUND);
                    this.AddItems(MENUPATH_LIBRARY_USER);
                    break;
                case Scenes.LnkFile:
                    this.AddItems(MENUPATH_SYSLNKFILE);
                    break;
                case Scenes.ExeFile:
                    this.AddItems(MENUPATH_SYSEXEFILE);
                    break;
                case Scenes.CustomType:
                    this.InsertItem(new TypeItem(), 0);
                    this.AddItems(TypeItem.AssExtPath);
                    break;
            }
        }

        private void AddItems(string scenePath)
        {
            if(this.Scene == Scenes.CustomType && TypeItem.Extension == null) return;
            this.AddShellItems(GetShellPath(scenePath));
            this.AddShellExItems(GetShellExPath(scenePath));
        }

        private void AddShellItems(string shellPath)
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

        private void AddShellExItems(string shellExPath)
        {
            List<Guid> guids = new List<Guid> { Guid.Empty };
            using(RegistryKey shellExKey = RegistryEx.GetRegistryKey(shellExPath))
            {
                if(shellExKey == null) return;
                RegTrustedInstaller.TakeRegTreeOwnerShip(shellExKey.Name);
                Dictionary<string, Guid> dic = ShellExItem.GetPathAndGuids(shellExPath);
                foreach(string path in dic.Keys)
                {
                    Guid guid = dic[path];
                    if(!guids.Contains(guid))
                    {
                        this.AddItem(new ShellExItem(guid, path));
                        guids.Add(guid);
                    }
                }
            }
        }

        private void AddNewItem(string scenePath)
        {
            string shellPath = GetShellPath(scenePath);
            string shellExPath = GetShellExPath(scenePath);
            NewItem newItem = new NewItem();
            AddCommonButton btnAddCommon = new AddCommonButton();
            newItem.AddCtr(btnAddCommon);
            this.AddItem(newItem);
            if(this.Scene == Scenes.CustomType)
            {
                newItem.Visible = TypeItem.Extension != null;
                TypeItem.ExtensionChanged += (sender, e) => newItem.Visible = TypeItem.Extension != null;
            }
            newItem.NewItemAdd += (sender, e) =>
            {
                using(NewShellDialog dlg = new NewShellDialog
                {
                    ScenePath = scenePath,
                    ShellPath = shellPath
                })
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                        this.InsertItem(new ShellItem(dlg.NewItemRegPath), GetItemIndex(newItem) + 1);
                }
            };
            btnAddCommon.MouseDown += (sender, e) =>
            {
                using(ShellCommonDialog dlg = new ShellCommonDialog
                {
                    ScenePath = scenePath,
                    ShellPath = shellPath,
                    ShellExPath = shellExPath
                })
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        foreach(string path in dlg.SelectedShellExPathAndGuids.Keys)
                        {
                            this.InsertItem(new ShellExItem(dlg.SelectedShellExPathAndGuids[path], path), 1);
                        }
                        dlg.SelectedShellPaths.ForEach(path => this.InsertItem(new ShellItem(path), 1));
                    }
                }
            };
        }

        sealed class TypeItem : MyListItem
        {

            static string extension;
            public static string Extension
            {
                get => extension;
                set
                {
                    extension = value;
                    ExtensionChanged?.Invoke(null, null);
                }
            }

            const string HKCR = "HKEY_CLASSES_ROOT";
            public static string SysAssExtPath => Extension == null ? null : $@"{SYSFILEASSPATH}\{Extension}";
            public static string AssExtPath => Extension == null ? null : $@"{HKCR}\{FileExtensionDialog.GetTypeName(Extension)}";

            public static event EventHandler ExtensionChanged;

            public TypeItem()
            {
                GetImageAndText();
                this.AddCtr(BtnType);
                BtnType.MouseDown += (sender, e) =>
                {
                    using(FileExtensionDialog dlg = new FileExtensionDialog())
                        if(dlg.ShowDialog() == DialogResult.OK) Extension = dlg.Extension;
                };
                ExtensionChanged += (sender, e) => GetImageAndText();
            }

            readonly PictureButton BtnType = new PictureButton(AppImage.Types);

            private void GetImageAndText()
            {
                if(Extension == null)
                {
                    this.Image = AppImage.CustomType;
                    this.Text = AppString.Dialog.SelectExtension;
                }
                else
                {
                    this.Image = ResourceIcon.GetExtensionIcon(Extension)?.ToBitmap() ?? AppImage.NotFound;
                    this.Text = $"{AppString.Item.CurrentExtension}{Extension}";
                }
            }
        }
    }
}