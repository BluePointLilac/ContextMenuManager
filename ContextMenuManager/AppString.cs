using BulePointLilac.Methods;
using System.Text;

namespace ContextMenuManager
{
    public static class AppString
    {
        public static IniReader UserLanguage = new IniReader(Program.LanguageFilePath);
        private static readonly IniReader DefaultLanguage = new IniReader(new StringBuilder(Properties.Resources.AppLanguageDic));

        private static string GetValue(string section, string key)
        {
            string value = UserLanguage?.GetValue(section, key);
            if(string.IsNullOrEmpty(value)) value = DefaultLanguage.GetValue(section, key);
            return value.Replace("\\n", "\n");
        }

        /// <summary>常规</summary>
        public static class General
        {
            private static string GetGeneralValue(string key) => GetValue("General", key);
            public static string Language = GetGeneralValue("Language");
            public static string AppName = GetGeneralValue("AppName");
        }

        /// <summary>工具栏</summary>
        public static class ToolBar
        {
            private static string GetToolBarValue(string key) => GetValue("ToolBar", key);
            public static string Home = GetToolBarValue("Home");
            public static string Type = GetToolBarValue("Type");
            public static string Rule = GetToolBarValue("Rule");
            public static string About = GetToolBarValue("About");
        }

        /// <summary>侧边栏</summary>
        public static class SideBar
        {
            private static string GetSideBarValue(string key) => GetValue("SideBar", key);
            public static string File = GetSideBarValue("File");
            public static string Folder = GetSideBarValue("Folder");
            public static string Directory = GetSideBarValue("Directory");
            public static string Background = GetSideBarValue("Background");
            public static string Desktop = GetSideBarValue("Desktop");
            public static string Drive = GetSideBarValue("Drive");
            public static string AllObjects = GetSideBarValue("AllObjects");
            public static string Computer = GetSideBarValue("Computer");
            public static string RecycleBin = GetSideBarValue("RecycleBin");
            public static string Library = GetSideBarValue("Library");
            public static string New = GetSideBarValue("New");
            public static string SendTo = GetSideBarValue("SendTo");
            public static string OpenWith = GetSideBarValue("OpenWith");
            public static string WinX = GetSideBarValue("WinX");

            public static string LnkFile = GetSideBarValue("LnkFile");
            public static string ExeFile = GetSideBarValue("ExeFile");
            public static string TextFile = GetSideBarValue("TextFile");
            public static string ImageFile = GetSideBarValue("ImageFile");
            public static string VideoFile = GetSideBarValue("VideoFile");
            public static string AudioFile = GetSideBarValue("AudioFile");
            public static string ImageDirectory = GetSideBarValue("ImageDirectory");
            public static string VideoDirectory = GetSideBarValue("VideoDirectory");
            public static string AudioDirectory = GetSideBarValue("AudioDirectory");
            public static string UnknownType = GetSideBarValue("UnknownType");
            public static string CustomType = GetSideBarValue("CustomType");

            public static string GuidBlocked = GetSideBarValue("GuidBlocked");
            public static string ThirdRules = GetSideBarValue("ThirdRules");

            public static string AboutApp = GetSideBarValue("AboutApp");
            public static string Dictionaries = GetSideBarValue("Dictionaries");
            public static string AppLanguage = GetSideBarValue("AppLanguage");
            public static string Donate = GetSideBarValue("Donate");
        }

        /// <summary>状态栏</summary>
        public static class StatusBar
        {
            private static string GetStatusBarValue(string key) => GetValue("StatusBar", key);
            public static string File = GetStatusBarValue("File");
            public static string Folder = GetStatusBarValue("Folder");
            public static string Directory = GetStatusBarValue("Directory");
            public static string Background = GetStatusBarValue("Background");
            public static string Desktop = GetStatusBarValue("Desktop");
            public static string Drive = GetStatusBarValue("Drive");
            public static string AllObjects = GetStatusBarValue("AllObjects");
            public static string Computer = GetStatusBarValue("Computer");
            public static string RecycleBin = GetStatusBarValue("RecycleBin");
            public static string Library = GetStatusBarValue("Library");
            public static string New = GetStatusBarValue("New");
            public static string SendTo = GetStatusBarValue("SendTo");
            public static string OpenWith = GetStatusBarValue("OpenWith");
            public static string WinX = GetStatusBarValue("WinX");

            public static string LnkFile = GetStatusBarValue("LnkFile");
            public static string ExeFile = GetStatusBarValue("ExeFile");
            public static string TextFile = GetStatusBarValue("TextFile");
            public static string ImageFile = GetStatusBarValue("ImageFile");
            public static string VideoFile = GetStatusBarValue("VideoFile");
            public static string AudioFile = GetStatusBarValue("AudioFile");
            public static string ImageDirectory = GetStatusBarValue("ImageDirectory");
            public static string VideoDirectory = GetStatusBarValue("VideoDirectory");
            public static string AudioDirectory = GetStatusBarValue("AudioDirectory");
            public static string UnknownType = GetStatusBarValue("UnknownType");
            public static string CustomType = GetStatusBarValue("CustomType");

            public static string GuidBlocked = GetStatusBarValue("GuidBlocked");
            public static string ThirdRules = GetStatusBarValue("ThirdRules");
        }

        /// <summary>菜单</summary>
        public static class Menu
        {
            private static string GetMenuValue(string key) => GetValue("Menu", key);
            public static string ChangeText = GetMenuValue("ChangeText");
            public static string ItemIcon = GetMenuValue("ItemIcon");
            public static string ChangeIcon = GetMenuValue("ChangeIcon");
            public static string AddIcon = GetMenuValue("AddIcon");
            public static string DeleteIcon = GetMenuValue("DeleteIcon");
            public static string ItemPosition = GetMenuValue("ItemPosition");
            public static string SetDefault = GetMenuValue("SetDefault");
            public static string SetTop = GetMenuValue("SetTop");
            public static string SetBottom = GetMenuValue("SetBottom");
            public static string OtherAttributes = GetMenuValue("OtherAttributes");
            public static string OnlyWithShift = GetMenuValue("OnlyWithShift");
            public static string OnlyInExplorer = GetMenuValue("OnlyInExplorer");
            public static string NoWorkingDirectory = GetMenuValue("NoWorkingDirectory");
            public static string ShowSeparator = GetMenuValue("ShowSeparator");
            public static string Details = GetMenuValue("Details");
            public static string WebSearch = GetMenuValue("WebSearch");
            public static string ChangeCommand = GetMenuValue("ChangeCommand");
            public static string FileProperties = GetMenuValue("FileProperties");
            public static string FileLocation = GetMenuValue("FileLocation");
            public static string RegistryLocation = GetMenuValue("RegistryLocation");
            public static string Delete = GetMenuValue("Delete");
            public static string DeleteReference = GetMenuValue("DeleteReference");
            public static string CopyGuid = GetMenuValue("CopyGuid");
            public static string InitialData = GetMenuValue("InitialData");
        }

        /// <summary>消息框</summary>
        public static class MessageBox
        {
            private static string GetMessageBoxValue(string key) => GetValue("MessageBox", key);
            public static string TextCannotBeEmpty = GetMessageBoxValue("TextCannotBeEmpty");
            public static string CommandCannotBeEmpty = GetMessageBoxValue("CommandCannotBeEmpty");
            public static string StringParsingFailed = GetMessageBoxValue("StringParsingFailed");
            public static string TextLengthCannotExceed80 = GetMessageBoxValue("TextLengthCannotExceed80");
            public static string ConfirmDeletePermanently = GetMessageBoxValue("ConfirmDeletePermanently");
            public static string ConfirmDeleteReference = GetMessageBoxValue("ConfirmDeleteReference");
            public static string ConfirmDelete = GetMessageBoxValue("ConfirmDelete");
            public static string ConfirmDeleteReferenced = GetMessageBoxValue("ConfirmDeleteReferenced");
            public static string CannotAddNewItem = GetMessageBoxValue("CannotAddNewItem");
            public static string UnsupportedFilename = GetMessageBoxValue("UnsupportedFilename");
            public static string UnsupportedExtension = GetMessageBoxValue("UnsupportedExtension");
            public static string CannotChangePath = GetMessageBoxValue("CannotChangePath");
            public static string CopiedToClipboard = GetMessageBoxValue("CopiedToClipboard");
            public static string UnknownGuid = GetMessageBoxValue("UnknownGuid");
            public static string HasBeenAdded = GetMessageBoxValue("HasBeenAdded");
            public static string EditInitialData = GetMessageBoxValue("EditInitialData");
            public static string PromptIsOpenItem = GetMessageBoxValue("PromptIsOpenItem");
            public static string RestartApp = GetMessageBoxValue("RestartApp");
            public static string UpdateApp = GetMessageBoxValue("UpdateApp");
        }

        /// <summary>其他文本</summary>
        public static class Text
        {
            private static string GetTextValue(string key) => GetValue("Text", key);
            public static string ItemName = GetTextValue("ItemName");
            public static string ItemCommand = GetTextValue("ItemCommand");

            public static string Single = GetTextValue("Single");
            public static string Multi = GetTextValue("Multi");

            public static string EditSubItems = GetTextValue("EditSubItems");
            public static string Separator = GetTextValue("Separator");
            public static string InvalidItem = GetTextValue("InvalidItem");
            public static string CheckReference = GetTextValue("CheckReference");
            public static string CheckCommon = GetTextValue("CheckCommon");
            public static string InputGuid = GetTextValue("InputGuid");

            public static string Explore = GetTextValue("Explore");
            public static string CustomFolder = GetTextValue("CustomFolder");
            public static string BuildSendtoMenu = GetTextValue("BuildSendtoMenu");
            public static string UseStoreOpenWith = GetTextValue("UseStoreOpenWith");
            public static string RestartExplorer = GetTextValue("RestartExplorer");

            public static string NewItem = GetTextValue("NewItem");
            public static string NewShellItem = GetTextValue("NewShellItem");
            public static string NewSendToItem = GetTextValue("NewSendToItem");
            public static string NewOpenWithItem = GetTextValue("NewOpenWithItem");
            public static string NewGuidBlockedItem = GetTextValue("NewGuidBlockedItem");

            public static string SelectExtension = GetTextValue("SelectExtension");
            public static string CurrentExtension = GetTextValue("CurrentExtension");

            public static string DictionaryDescription = GetTextValue("DictionaryDescription");
            public static string LanguageDictionary = GetTextValue("LanguageDictionary");
            public static string GuidInfosDictionary = GetTextValue("GuidInfosDictionary");
            public static string ThridRulesDictionary = GetTextValue("ThridRulesDictionary");
            public static string CommonItemsDictionary = GetTextValue("CommonItemsDictionary");
            public static string Translators = GetTextValue("Translators");
            public static string OtherLanguages = GetTextValue("OtherLanguages");
            public static string SelectSubMenuMode = GetTextValue("SelectSubMenuMode");
            public static string AboutApp = GetTextValue("AboutApp");
            public static string Dictionaries = GetTextValue("Dictionaries");
            public static string Donate = GetTextValue("Donate");
        }

        /// <summary>提示文本</summary>
        public static class Tip
        {
            private static string GetTipValue(string key) => GetValue("Tip", key);
            public static string RestartExplorer = GetTipValue("RestartExplorer");
            public static string CustomFolder = GetTipValue("CustomFolder");
            public static string SendToDrive = GetTipValue("SendToDrive");
            public static string BuildSendtoMenu = GetTipValue("BuildSendtoMenu");
            public static string UseStoreOpenWith = GetTipValue("UseStoreOpenWith");
            public static string EditSubItems = GetTipValue("EditSubItems");
            public static string InvalidItem = GetTipValue("InvalidItem");
            public static string AddSeparator = GetTipValue("AddSeparator");
            public static string Separator = GetTipValue("Separator");
            public static string AddExistingItems = GetTipValue("AddExistingItems");
            public static string AddCommonItems = GetTipValue("AddCommonItems");
        }

        /// <summary>国际化字符串</summary>
        public static class Indirect
        {
            /// <summary>确定</summary>
            public static readonly string Ok = ResourceString.GetDirectString("@shell32.dll,-9752");
            /// <summary>取消</summary>
            public static readonly string Cancel = ResourceString.GetDirectString("@shell32.dll,-9751");
            /// <summary>浏览</summary>
            public static readonly string Browse = ResourceString.GetDirectString("@shell32.dll,-9015");
            /// <summary>打开</summary>
            public static readonly string Open = ResourceString.GetDirectString("@shell32.dll,-12850");
            /// <summary>编辑</summary>
            public static readonly string Edit = ResourceString.GetDirectString("@shell32.dll,-37398");
            /// <summary>打印</summary>
            public static readonly string Print = ResourceString.GetDirectString("@shell32.dll,-31250");
            /// <summary>搜索</summary>
            public static readonly string Find = ResourceString.GetDirectString("@shell32.dll,-9031");
            /// <summary>播放</summary>
            public static readonly string Play = ResourceString.GetDirectString("@shell32.dll,-31283");
            /// <summary>以管理员身份运行</summary>
            public static readonly string Runas = ResourceString.GetDirectString("@shell32.dll,-37417");
            /// <summary>保存</summary>
            public static readonly string Save = ResourceString.GetDirectString("@shell32.dll,-38243");
            /// <summary>程序</summary>
            public static readonly string Programs = ResourceString.GetDirectString("@shell32.dll,-21782");
            /// <summary>可移动磁盘</summary>
            public static readonly string RemovableDrive = ResourceString.GetDirectString("@shell32.dll,-9309");
            /// <summary>映射网络驱动器</summary>
            public static readonly string MapNetworkDrive = ResourceString.GetDirectString("@shell32.dll,-31300");
            /// <summary>断开网络驱动器的连接</summary>
            public static readonly string DisconnectNetworkDrive = ResourceString.GetDirectString("@shell32.dll,-31304");
            /// <summary>回收站属性</summary>
            public static readonly string RecycleBinProperties = ResourceString.GetDirectString("@shell32.dll,-31338");
            ///<summary>文件或文件夹不存在</summary>
            public static readonly string FileOrFolderNotExists = ResourceString.GetDirectString("@shell32.dll,-4132");
        }
    }
}