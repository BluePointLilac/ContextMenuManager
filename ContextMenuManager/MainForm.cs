using BluePointLilac.Controls;
using ContextMenuManager.Controls;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager
{
    sealed class MainForm : MyMainForm
    {
        public MainForm()
        {
            SetSideBarWidth();
            this.Text = AppString.General.AppName;
            this.ForeColor = Color.FromArgb(80, 80, 80);
            this.Controls.Add(new ExplorerRestarter());
            appSettingBox.Owner = shellList.Owner = shellNewList.Owner = sendToList.Owner = openWithList.Owner
                = winXList.Owner = guidBlockedList.Owner = enhanceMenusList.Owner = thirdRuleList.Owner = iEList.Owner = MainBody;
            donateBox.Parent = aboutMeBox.Parent = dictionariesBox.Parent = languagesBox.Parent = MainBody;
            SideBar.SelectIndexChanged += (sender, e) => SwitchItem();
            SideBar.HoverIndexChanged += (sender, e) => ShowItemInfo();
            ToolBar.SelectedButtonChanged += (sender, e) => SwitchTab();
            ToolBarButtons[3].MouseDown += (sender, e) => SwitchItem();
            ToolBar.AddButtons(ToolBarButtons);
            ToolBar.SelectedIndex = 0;
            if(AppConfig.ShowFilePath) ShowFilePath();
        }

        readonly MyToolBarButton[] ToolBarButtons = new MyToolBarButton[] {
            new MyToolBarButton(AppImage.Home, AppString.ToolBar.Home),//主页
            new MyToolBarButton(AppImage.Type, AppString.ToolBar.Type),//文件类型
            new MyToolBarButton(AppImage.Star, AppString.ToolBar.Rule),//其他规则
            new MyToolBarButton(AppImage.Refresh,AppString.ToolBar.Refresh){ CanBeSelected = false },//刷新
            new MyToolBarButton(AppImage.About, AppString.ToolBar.About)//关于
        };
        readonly ShellList shellList = new ShellList();
        readonly ShellNewList shellNewList = new ShellNewList();
        readonly SendToList sendToList = new SendToList();
        readonly OpenWithList openWithList = new OpenWithList();
        readonly WinXList winXList = new WinXList();
        readonly GuidBlockedList guidBlockedList = new GuidBlockedList();
        readonly EnhanceMenusList enhanceMenusList = new EnhanceMenusList();
        readonly ThirdRulesList thirdRuleList = new ThirdRulesList();
        readonly IEList iEList = new IEList();
        readonly ReadOnlyRichTextBox aboutMeBox = new ReadOnlyRichTextBox
        {
            Text = AppString.Other.AboutApp
        };
        readonly DonateBox donateBox = new DonateBox();
        readonly LanguagesBox languagesBox = new LanguagesBox();
        readonly DictionariesBox dictionariesBox = new DictionariesBox();
        readonly AppSettingBox appSettingBox = new AppSettingBox();

        static readonly string[] GeneralItems = {
            AppString.SideBar.File,
            AppString.SideBar.Folder,
            AppString.SideBar.Directory,
            AppString.SideBar.Background,
            AppString.SideBar.Desktop,
            AppString.SideBar.Drive,
            AppString.SideBar.AllObjects,
            AppString.SideBar.Computer,
            AppString.SideBar.RecycleBin,
            AppString.SideBar.Library,
            null,
            AppString.SideBar.New,
            AppString.SideBar.SendTo,
            AppString.SideBar.OpenWith,
            null,
            AppString.SideBar.WinX
        };
        static readonly string[] GeneralItemInfos = {
            AppString.StatusBar.File,
            AppString.StatusBar.Folder,
            AppString.StatusBar.Directory,
            AppString.StatusBar.Background,
            AppString.StatusBar.Desktop,
            AppString.StatusBar.Drive,
            AppString.StatusBar.AllObjects,
            AppString.StatusBar.Computer,
            AppString.StatusBar.RecycleBin,
            AppString.StatusBar.Library,
            null,
            AppString.StatusBar.New,
            AppString.StatusBar.SendTo,
            AppString.StatusBar.OpenWith,
            null,
            AppString.StatusBar.WinX
        };

        static readonly string[] TypeItems = {
            AppString.SideBar.LnkFile,
            AppString.SideBar.UwpLnk,
            AppString.SideBar.ExeFile,
            null,
            AppString.SideBar.CustomExtension,
            AppString.SideBar.PerceivedType,
            AppString.SideBar.DirectoryType,
            null,
            AppString.SideBar.UnknownType
        };
        static readonly string[] TypeItemInfos = {
            AppString.StatusBar.LnkFile,
            AppString.StatusBar.UwpLnk,
            AppString.StatusBar.ExeFile,
            null,
            AppString.StatusBar.CustomExtension,
            AppString.StatusBar.PerceivedType,
            AppString.StatusBar.DirectoryType,
            null,
            AppString.StatusBar.UnknownType
        };

        static readonly string[] OtherRuleItems = {
            AppString.SideBar.EnhanceMenu,
            AppString.SideBar.ThirdRules,
            null,
            AppString.SideBar.DragDrop,
            AppString.SideBar.PublicReferences,
            AppString.SideBar.GuidBlocked,
            null,
            AppString.SideBar.IEMenu
        };
        static readonly string[] OtherRuleItemInfos = {
            AppString.StatusBar.EnhanceMenu,
            AppString.StatusBar.ThirdRules,
            null,
            AppString.StatusBar.DragDrop,
            AppString.StatusBar.PublicReferences,
            AppString.StatusBar.GuidBlocked,
            null,
            AppString.StatusBar.IEMenu
        };

        static readonly string[] AboutItems = {
            AppString.SideBar.AppSetting,
            AppString.SideBar.AppLanguage,
            AppString.SideBar.Dictionaries,
            AppString.SideBar.AboutApp,
            AppString.SideBar.Donate
        };

        static readonly ShellList.Scenes[] GeneralShellScenes =
        {
            ShellList.Scenes.File,
            ShellList.Scenes.Folder,
            ShellList.Scenes.Directory,
            ShellList.Scenes.Background,
            ShellList.Scenes.Desktop,
            ShellList.Scenes.Drive,
            ShellList.Scenes.AllObjects,
            ShellList.Scenes.Computer,
            ShellList.Scenes.RecycleBin,
            ShellList.Scenes.Library
        };

        static readonly ShellList.Scenes?[] TypeShellScenes =
        {
            ShellList.Scenes.LnkFile,
            ShellList.Scenes.UwpLnk,
            ShellList.Scenes.ExeFile,
            null,
            ShellList.Scenes.CustomExtension,
            ShellList.Scenes.PerceivedType,
            ShellList.Scenes.DirectoryType,
            null,
            ShellList.Scenes.UnknownType
        };

        private void HideAllParts()
        {
            foreach(Control ctr in MainBody.Controls)
            {
                ctr.Visible = false;
                if(ctr.GetType().BaseType == typeof(MyList))
                {
                    if(ctr == appSettingBox) continue;
                    ((MyList)ctr).ClearItems();
                }
            }
        }

        private void SwitchTab()
        {
            switch(ToolBar.SelectedIndex)
            {
                case 0:
                    SideBar.ItemNames = GeneralItems; break;
                case 1:
                    SideBar.ItemNames = TypeItems; break;
                case 2:
                    SideBar.ItemNames = OtherRuleItems; break;
                case 4:
                    SideBar.ItemNames = AboutItems; break;
            }
            SideBar.SelectIndex = 0;
        }

        private void SwitchItem()
        {
            HideAllParts();
            if(SideBar.SelectIndex == -1) return;
            switch(ToolBar.SelectedIndex)
            {
                case 0:
                    SwitchGeneralItem(); break;
                case 1:
                    SwitchTypeItem(); break;
                case 2:
                    SwitchOtherRuleItem(); break;
                case 4:
                    SwitchAboutItem(); break;
            }
        }

        private void ShowItemInfo()
        {
            if(SideBar.HoverIndex >= 0)
            {
                int i = SideBar.HoverIndex;
                switch(ToolBar.SelectedIndex)
                {
                    case 0:
                        StatusBar.Text = GeneralItemInfos[i]; return;
                    case 1:
                        StatusBar.Text = TypeItemInfos[i]; return;
                    case 2:
                        StatusBar.Text = OtherRuleItemInfos[i]; return;
                }
            }
            StatusBar.Text = MyStatusBar.DefaultText;
        }

        private void ShowFilePath()
        {
            foreach(MyList list in new MyList[] { shellList, shellNewList, sendToList, openWithList, winXList, guidBlockedList, iEList })
            {
                list.HoveredItemChanged += (sender, e) =>
                {
                    MyListItem item = list.HoveredItem;
                    if(item is ITsiFilePathItem pathItem)
                    {
                        string path = pathItem.ItemFilePath;
                        if(File.Exists(path)) { StatusBar.Text = path; return; }
                    }
                    if(item is GuidBlockedItem guidItem)
                    {
                        StatusBar.Text = guidItem.Value; return;
                    }
                    StatusBar.Text = item.Text;
                };
            }
        }

        private void SwitchGeneralItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 11:
                    shellNewList.LoadItems(); shellNewList.Visible = true; break;
                case 12:
                    sendToList.LoadItems(); sendToList.Visible = true; break;
                case 13:
                    openWithList.LoadItems(); openWithList.Visible = true; break;
                case 15:
                    winXList.LoadItems(); winXList.Visible = true; break;
                default:
                    if(SideBar.SelectIndex <= 9)
                    {
                        shellList.Scene = GeneralShellScenes[SideBar.SelectIndex];
                        shellList.LoadItems();
                        shellList.Visible = true;
                    }
                    break;
            }
        }

        private void SwitchTypeItem()
        {
            shellList.Scene = (ShellList.Scenes)TypeShellScenes[SideBar.SelectIndex];
            shellList.LoadItems();
            shellList.Visible = true;
        }

        private void SwitchOtherRuleItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 0:
                    enhanceMenusList.LoadItems(); enhanceMenusList.Visible = true; break;
                case 1:
                    thirdRuleList.LoadItems(); thirdRuleList.Visible = true; break;
                case 3:
                    shellList.Scene = ShellList.Scenes.DragDrop; shellList.LoadItems(); shellList.Visible = true; break;
                case 4:
                    shellList.Scene = ShellList.Scenes.CommandStore; shellList.LoadItems(); shellList.Visible = true; break;
                case 5:
                    guidBlockedList.LoadItems(); guidBlockedList.Visible = true; break;
                case 7:
                    iEList.LoadItems(); iEList.Visible = true; break;
            }
        }

        private void SwitchAboutItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 0:
                    appSettingBox.LoadItems();
                    appSettingBox.Visible = true;
                    break;
                case 1:
                    languagesBox.LoadLanguages();
                    languagesBox.Visible = true;
                    break;
                case 2:
                    dictionariesBox.LoadTexts();
                    dictionariesBox.Visible = true;
                    break;
                case 3:
                    aboutMeBox.Visible = true;
                    break;
                case 4:
                    donateBox.Visible = true;
                    break;
            }
        }

        private void SetSideBarWidth()
        {
            int maxWidth = 0;
            string[] strs = GeneralItems.Concat(TypeItems).Concat(OtherRuleItems).Concat(AboutItems).ToArray();
            Array.ForEach(strs, str => maxWidth = Math.Max(maxWidth, SideBar.GetItemWidth(str)));
            SideBar.Width = maxWidth;
        }
    }
}