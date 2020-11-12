using BulePointLilac.Controls;
using ContextMenuManager.Controls;
using System;
using System.Linq;

namespace ContextMenuManager
{
    sealed class MainForm : MyMainForm
    {
        public MainForm()
        {
            SetSideBarWidth();
            this.Text = AppString.General.AppName;
            this.Controls.Add(new ExplorerRestarter());
            shellList.Owner = shellNewList.Owner = sendToList.Owner = openWithList.Owner
                = winXList.Owner = guidBlockedList.Owner = thirdRuleList.Owner = MainBody;
            donateBox.Parent = aboutMeBox.Parent = dictionariesBox.Parent = languagesBox.Parent = MainBody;
            SideBar.SelectIndexChanged += (sender, e) => SwitchItem();
            SideBar.HoverIndexChanged += (sender, e) => ShowItemInfo();
            ToolBar.SelectedButtonChanged += (sender, e) => SwitchTab();
            ToolBar.AddButtons(ToolBarButtons);
            ToolBar.SelectedIndex = 0;
        }

        readonly MyToolBarButton[] ToolBarButtons = new MyToolBarButton[] {
            new MyToolBarButton(AppImage.Home, AppString.ToolBar.Home),//主页
            new MyToolBarButton(AppImage.Type, AppString.ToolBar.Type),//文件类型
            new MyToolBarButton(AppImage.Star, AppString.ToolBar.Rule),//其他规则
            new MyToolBarButton(AppImage.About, AppString.ToolBar.About)//关于
        };
        readonly ShellList shellList = new ShellList();
        readonly ShellNewList shellNewList = new ShellNewList();
        readonly SendToList sendToList = new SendToList();
        readonly OpenWithList openWithList = new OpenWithList();
        readonly WinXList winXList = new WinXList();
        readonly GuidBlockedList guidBlockedList = new GuidBlockedList();
        readonly ThirdRulesList thirdRuleList = new ThirdRulesList();
        readonly AboutAppBox aboutMeBox = new AboutAppBox
        {
            Text = AppString.Other.AboutApp
        };
        readonly DonateBox donateBox = new DonateBox();
        readonly LanguagesBox languagesBox = new LanguagesBox();
        readonly DictionariesBox dictionariesBox = new DictionariesBox();

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
            AppString.SideBar.TextFile,
            AppString.SideBar.ImageFile,
            AppString.SideBar.VideoFile,
            AppString.SideBar.AudioFile,
            null,
            AppString.SideBar.ImageDirectory,
            AppString.SideBar.VideoDirectory,
            AppString.SideBar.AudioDirectory,
            null,
            AppString.SideBar.UnknownType,
            null,
            AppString.SideBar.CustomType
        };
        static readonly string[] TypeItemInfos = {
            AppString.StatusBar.LnkFile,
            AppString.StatusBar.UwpLnk,
            AppString.StatusBar.ExeFile,
            null,
            AppString.StatusBar.TextFile,
            AppString.StatusBar.ImageFile,
            AppString.StatusBar.VideoFile,
            AppString.StatusBar.AudioFile,
            null,
            AppString.StatusBar.ImageDirectory,
            AppString.StatusBar.VideoDirectory,
            AppString.StatusBar.AudioDirectory,
            null,
            AppString.StatusBar.UnknownType,
            null,
            AppString.StatusBar.CustomType
        };

        static readonly string[] OtherRuleItems = {
            AppString.SideBar.GuidBlocked,
            AppString.SideBar.ThirdRules
        };
        static readonly string[] OtherRuleItemInfos = {
            AppString.StatusBar.GuidBlocked,
            AppString.StatusBar.ThirdRules
        };

        static readonly string[] AboutItems = {
            AppString.SideBar.AboutApp,
            AppString.SideBar.Dictionaries,
            AppString.SideBar.AppLanguage,
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
            ShellList.Scenes.TextFile,
            ShellList.Scenes.ImageFile,
            ShellList.Scenes.VideoFile,
            ShellList.Scenes.AudioFile,
            null,
            ShellList.Scenes.ImageDirectory,
            ShellList.Scenes.VideoDirectory,
            ShellList.Scenes.AudioDirectory,
            null,
            ShellList.Scenes.UnknownType,
            null,
            ShellList.Scenes.CustomType
        };

        private void HideAllParts()
        {
            shellList.Visible = shellNewList.Visible = sendToList.Visible = openWithList.Visible
                = winXList.Visible = guidBlockedList.Visible = thirdRuleList.Visible
                = donateBox.Visible = aboutMeBox.Visible = dictionariesBox.Visible = languagesBox.Visible = false;
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
                case 3:
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
                    SwitchGeneralItem(); return;
                case 1:
                    SwitchTypeItem(); return;
                case 2:
                    SwitchOtherRuleItem(); return;
                case 3:
                    SwitchAboutItem(); return;
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
                        shellList.Visible = true;
                    }
                    break;
            }
        }

        private void SwitchTypeItem()
        {
            shellList.Scene = (ShellList.Scenes)TypeShellScenes[SideBar.SelectIndex];
            shellList.Visible = true;
        }

        private void SwitchOtherRuleItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 0:
                    guidBlockedList.LoadItems(); guidBlockedList.Visible = true; return;
                case 1:
                    thirdRuleList.LoadItems(); thirdRuleList.Visible = true; return;
            }
        }

        private void SwitchAboutItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 0:
                    aboutMeBox.Visible = true;
                    return;
                case 1:
                    dictionariesBox.Visible = true;
                    dictionariesBox.LoadTexts();
                    return;
                case 2:
                    languagesBox.LoadLanguages();
                    languagesBox.Visible = true;
                    break;
                case 3:
                    donateBox.Visible = true;
                    return;
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