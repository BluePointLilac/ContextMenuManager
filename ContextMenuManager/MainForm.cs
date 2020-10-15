using BulePointLilac.Controls;
using ContextMenuManager.Controls;

namespace ContextMenuManager
{
    sealed class MainForm : MyMainForm
    {
        public MainForm()
        {
            this.Text = AppString.General_AppName;
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
            new MyToolBarButton(AppImage.Home, AppString.ToolBar_Home),//主页
            new MyToolBarButton(AppImage.Type, AppString.ToolBar_Type),//文件类型
            new MyToolBarButton(AppImage.Star, AppString.ToolBar_Rule),//其他规则
            new MyToolBarButton(AppImage.About, AppString.ToolBar_About)//关于
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
            Text = AppString.Text_AboutApp
        };
        readonly DonateBox donateBox = new DonateBox();
        readonly LanguagesBox languagesBox = new LanguagesBox();
        readonly DictionariesBox dictionariesBox = new DictionariesBox();

        static readonly string[] GeneralItems = {
            AppString.SideBar_File,
            AppString.SideBar_Folder,
            AppString.SideBar_Directory,
            AppString.SideBar_Background,
            AppString.SideBar_Desktop,
            AppString.SideBar_Drive,
            AppString.SideBar_AllObjects,
            AppString.SideBar_Computer,
            AppString.SideBar_RecycleBin,
            AppString.SideBar_Library,
            null,
            AppString.SideBar_New,
            AppString.SideBar_SendTo,
            AppString.SideBar_OpenWith,
            null,
            AppString.SideBar_WinX
        };
        static readonly string[] GeneralItemInfos = {
            AppString.StatusBar_File,
            AppString.StatusBar_Folder,
            AppString.StatusBar_Directory,
            AppString.StatusBar_Background,
            AppString.StatusBar_Desktop,
            AppString.StatusBar_Drive,
            AppString.StatusBar_AllObjects,
            AppString.StatusBar_Computer,
            AppString.StatusBar_RecycleBin,
            AppString.StatusBar_Library,
            null,
            AppString.StatusBar_New,
            AppString.StatusBar_SendTo,
            AppString.StatusBar_OpenWith,
            null,
            AppString.StatusBar_WinX
        };

        static readonly string[] TypeItems = {
            AppString.SideBar_LnkFile,
            AppString.SideBar_ExeFile,
            null,
            AppString.SideBar_TextFile,
            AppString.SideBar_ImageFile,
            AppString.SideBar_VideoFile,
            AppString.SideBar_AudioFile,
            null,
            AppString.SideBar_ImageDirectory,
            AppString.SideBar_VideoDirectory,
            AppString.SideBar_AudioDirectory,
            null,
            AppString.SideBar_UnknownType,
            null,
            AppString.SideBar_CustomType
        };
        static readonly string[] TypeItemInfos = {
            AppString.StatusBar_LnkFile,
            AppString.StatusBar_ExeFile,
            null,
            AppString.StatusBar_TextFile,
            AppString.StatusBar_ImageFile,
            AppString.StatusBar_VideoFile,
            AppString.StatusBar_AudioFile,
            null,
            AppString.StatusBar_ImageDirectory,
            AppString.StatusBar_VideoDirectory,
            AppString.StatusBar_AudioDirectory,
            null,
            AppString.StatusBar_UnknownType,
            null,
            AppString.StatusBar_CustomType
        };

        static readonly string[] OtherRuleItems = {
            AppString.SideBar_GuidBlocked,
            AppString.SideBar_ThirdRules
        };
        static readonly string[] OtherRuleItemInfos = {
            AppString.StatusBar_GuidBlocked,
            AppString.StatusBar_ThirdRules
        };

        static readonly string[] AboutItems = {
            AppString.SideBar_AboutApp,
            AppString.SideBar_Dictionaries,
            AppString.SideBar_AppLanguage,
            AppString.SideBar_Donate
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
                case 0:
                    shellList.Scene = ShellList.Scenes.File; break;
                case 1:
                    shellList.Scene = ShellList.Scenes.Folder; break;
                case 2:
                    shellList.Scene = ShellList.Scenes.Directory; break;
                case 3:
                    shellList.Scene = ShellList.Scenes.Background; break;
                case 4:
                    shellList.Scene = ShellList.Scenes.Desktop; break;
                case 5:
                    shellList.Scene = ShellList.Scenes.Drive; break;
                case 6:
                    shellList.Scene = ShellList.Scenes.AllObjects; break;
                case 7:
                    shellList.Scene = ShellList.Scenes.Computer; break;
                case 8:
                    shellList.Scene = ShellList.Scenes.RecycleBin; break;
                case 9:
                    shellList.Scene = ShellList.Scenes.Library; break;
                case 11:
                    shellNewList.LoadItems(); shellNewList.Visible = true; break;
                case 12:
                    sendToList.LoadItems(); sendToList.Visible = true; break;
                case 13:
                    openWithList.LoadItems(); openWithList.Visible = true; break;
                case 15:
                    winXList.LoadItems(); winXList.Visible = true; break;
            }
            shellList.Visible = SideBar.SelectIndex <= 9;
        }

        private void SwitchTypeItem()
        {
            switch(SideBar.SelectIndex)
            {
                case 0:
                    shellList.Scene = ShellList.Scenes.LnkFile; break;
                case 1:
                    shellList.Scene = ShellList.Scenes.ExeFile; break;
                case 3:
                    shellList.Scene = ShellList.Scenes.Text; break;
                case 4:
                    shellList.Scene = ShellList.Scenes.Image; break;
                case 5:
                    shellList.Scene = ShellList.Scenes.Video; break;
                case 6:
                    shellList.Scene = ShellList.Scenes.Audio; break;
                case 8:
                    shellList.Scene = ShellList.Scenes.ImageDirectory; break;
                case 9:
                    shellList.Scene = ShellList.Scenes.VideoDirectory; break;
                case 10:
                    shellList.Scene = ShellList.Scenes.AudioDirectory; break;
                case 12:
                    shellList.Scene = ShellList.Scenes.Unknown; break;
                case 14:
                    shellList.Scene = ShellList.Scenes.CustomExtension; break;
            }
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
    }
}