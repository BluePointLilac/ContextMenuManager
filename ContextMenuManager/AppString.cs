using BulePointLilac.Methods;
using System.Text;

namespace ContextMenuManager
{
    public static class AppString
    {
        public static IniReader UserLanguage = new IniReader(Program.LanguageFilePath);
        private static readonly IniReader DefaultLanguage = new IniReader(new StringBuilder(Properties.Resources.AppLanguageDic));


        private static string GetStringValue(string section, string key)
        {
            string value = UserLanguage.GetValue(section, key);
            if(string.IsNullOrEmpty(value)) value = DefaultLanguage.GetValue(section, key);
            return value.Replace("\\n", "\n");
        }

        /// <summary>常规</summary>
        public static class General
        {
            private static string GetValue(string key) => GetStringValue("General", key);
            public static string Language => GetValue("Language");
            public static string AppName => GetValue("AppName");
        }

        /// <summary>工具栏</summary>
        public static class ToolBar
        {
            private static string GetValue(string key) => GetStringValue("ToolBar", key);
            public static string Home => GetValue("Home");
            public static string Type => GetValue("Type");
            public static string Rule => GetValue("Rule");
            public static string About => GetValue("About");
        }

        /// <summary>侧边栏</summary>
        public static class SideBar
        {
            private static string GetValue(string key) => GetStringValue("SideBar", key);
            public static string File => GetValue("File");
            public static string Folder => GetValue("Folder");
            public static string Directory => GetValue("Directory");
            public static string Background => GetValue("Background");
            public static string Desktop => GetValue("Desktop");
            public static string Drive => GetValue("Drive");
            public static string AllObjects => GetValue("AllObjects");
            public static string Computer => GetValue("Computer");
            public static string RecycleBin => GetValue("RecycleBin");
            public static string Library => GetValue("Library");
            public static string New => GetValue("New");
            public static string SendTo => GetValue("SendTo");
            public static string OpenWith => GetValue("OpenWith");
            public static string WinX => GetValue("WinX");
            public static string LnkFile => GetValue("LnkFile");
            public static string UwpLnk => GetValue("UwpLnk");
            public static string ExeFile => GetValue("ExeFile");
            public static string TextFile => GetValue("TextFile");
            public static string ImageFile => GetValue("ImageFile");
            public static string VideoFile => GetValue("VideoFile");
            public static string AudioFile => GetValue("AudioFile");
            public static string ImageDirectory => GetValue("ImageDirectory");
            public static string VideoDirectory => GetValue("VideoDirectory");
            public static string AudioDirectory => GetValue("AudioDirectory");
            public static string UnknownType => GetValue("UnknownType");
            public static string CustomType => GetValue("CustomType");
            public static string GuidBlocked => GetValue("GuidBlocked");
            public static string ThirdRules => GetValue("ThirdRules");
            public static string AboutApp => GetValue("AboutApp");
            public static string Dictionaries => GetValue("Dictionaries");
            public static string AppLanguage => GetValue("AppLanguage");
            public static string Donate => GetValue("Donate");
        }

        /// <summary>状态栏</summary>
        public static class StatusBar
        {
            private static string GetValue(string key) => GetStringValue("StatusBar", key);
            public static string File => GetValue("File");
            public static string Folder => GetValue("Folder");
            public static string Directory => GetValue("Directory");
            public static string Background => GetValue("Background");
            public static string Desktop => GetValue("Desktop");
            public static string Drive => GetValue("Drive");
            public static string AllObjects => GetValue("AllObjects");
            public static string Computer => GetValue("Computer");
            public static string RecycleBin => GetValue("RecycleBin");
            public static string Library => GetValue("Library");
            public static string New => GetValue("New");
            public static string SendTo => GetValue("SendTo");
            public static string OpenWith => GetValue("OpenWith");
            public static string WinX => GetValue("WinX");
            public static string LnkFile => GetValue("LnkFile");
            public static string UwpLnk => GetValue("UwpLnk");
            public static string ExeFile => GetValue("ExeFile");
            public static string TextFile => GetValue("TextFile");
            public static string ImageFile => GetValue("ImageFile");
            public static string VideoFile => GetValue("VideoFile");
            public static string AudioFile => GetValue("AudioFile");
            public static string ImageDirectory => GetValue("ImageDirectory");
            public static string VideoDirectory => GetValue("VideoDirectory");
            public static string AudioDirectory => GetValue("AudioDirectory");
            public static string UnknownType => GetValue("UnknownType");
            public static string CustomType => GetValue("CustomType");
            public static string GuidBlocked => GetValue("GuidBlocked");
            public static string ThirdRules => GetValue("ThirdRules");
        }

        /// <summary>菜单</summary>
        public static class Menu
        {
            private static string GetValue(string key) => GetStringValue("Menu", key);
            public static string ChangeText => GetValue("ChangeText");
            public static string ItemIcon => GetValue("ItemIcon");
            public static string ChangeIcon => GetValue("ChangeIcon");
            public static string AddIcon => GetValue("AddIcon");
            public static string DeleteIcon => GetValue("DeleteIcon");
            public static string ItemPosition => GetValue("ItemPosition");
            public static string SetDefault => GetValue("SetDefault");
            public static string SetTop => GetValue("SetTop");
            public static string SetBottom => GetValue("SetBottom");
            public static string OtherAttributes => GetValue("OtherAttributes");
            public static string OnlyWithShift => GetValue("OnlyWithShift");
            public static string OnlyInExplorer => GetValue("OnlyInExplorer");
            public static string NoWorkingDirectory => GetValue("NoWorkingDirectory");
            public static string ShowSeparator => GetValue("ShowSeparator");
            public static string Details => GetValue("Details");
            public static string WebSearch => GetValue("WebSearch");
            public static string ChangeCommand => GetValue("ChangeCommand");
            public static string FileProperties => GetValue("FileProperties");
            public static string FileLocation => GetValue("FileLocation");
            public static string RegistryLocation => GetValue("RegistryLocation");
            public static string Delete => GetValue("Delete");
            public static string DeleteReference => GetValue("DeleteReference");
            public static string HandleGuid => GetValue("HandleGuid");
            public static string CopyGuid => GetValue("CopyGuid");
            public static string BlockGuid => GetValue("BlockGuid");
            public static string AddGuidDic => GetValue("AddGuidDic");
            public static string InitialData => GetValue("InitialData");
            public static string Edit => GetValue("Edit");
            public static string Save => GetValue("Save");
        }

        /// <summary>特殊项目文本</summary>
        public static class Item
        {
            private static string GetValue(string key) => GetStringValue("Item", key);
            public static string Open => GetValue("Open");
            public static string Edit => GetValue("Edit");
            public static string Explore => GetValue("Explore");
            public static string Play => GetValue("Play");
            public static string Print => GetValue("Print");
            public static string Find => GetValue("Find");
            public static string Runas => GetValue("Runas");
            public static string CustomFolder => GetValue("CustomFolder");
            public static string MapNetworkDrive => GetValue("MapNetworkDrive");
            public static string DisconnectNetworkDrive => GetValue("DisconnectNetworkDrive");
            public static string RecycleBinProperties => GetValue("RecycleBinProperties");
            public static string RemovableDrive => GetValue("RemovableDrive");
            public static string BuildSendtoMenu => GetValue("BuildSendtoMenu");
            public static string UseStoreOpenWith => GetValue("UseStoreOpenWith");
            public static string WinXPowerShell => GetValue("WinXPowerShell");
            public static string NewItem => GetValue("NewItem");
            public static string AddGuidBlockedItem => GetValue("AddGuidBlockedItem");
            public static string CurrentExtension => GetValue("CurrentExtension");
            public static string EditSubItems => GetValue("EditSubItems");
            public static string InvalidItem => GetValue("InvalidItem");
            public static string Separator => GetValue("Separator");
        }

        public static class Dialog
        {
            private static string GetValue(string key) => GetStringValue("Dialog", key);
            public static string Ok => GetValue("Ok");
            public static string Cancel => GetValue("Cancel");
            public static string Browse => GetValue("Browse");
            public static string Program => GetValue("Program");
            public static string NewShellItem => GetValue("NewShellItem");
            public static string NewSendToItem => GetValue("NewSendToItem");
            public static string NewOpenWithItem => GetValue("NewOpenWithItem");
            public static string ItemText => GetValue("ItemText");
            public static string ItemCommand => GetValue("ItemCommand");
            public static string ItemName => GetValue("ItemName");
            public static string ItemIcon => GetValue("ItemIcon");
            public static string SingleMenu => GetValue("SingleMenu");
            public static string MultiMenu => GetValue("MultiMenu");
            public static string InputGuid => GetValue("InputGuid");
            public static string AddGuidDic => GetValue("AddGuidDic");
            public static string DeleteGuidDic => GetValue("DeleteGuidDic");
            public static string SelectExtension => GetValue("SelectExtension");
            public static string CheckReference => GetValue("CheckReference");
            public static string CheckCommon => GetValue("CheckCommon");
            public static string SelectSubMenuMode => GetValue("SelectSubMenuMode");
        }

        /// <summary>消息框</summary>
        public static class MessageBox
        {
            private static string GetValue(string key) => GetStringValue("MessageBox", key);
            public static string TextCannotBeEmpty => GetValue("TextCannotBeEmpty");
            public static string CommandCannotBeEmpty => GetValue("CommandCannotBeEmpty");
            public static string StringParsingFailed => GetValue("StringParsingFailed");
            public static string TextLengthCannotExceed80 => GetValue("TextLengthCannotExceed80");
            public static string ConfirmDeletePermanently => GetValue("ConfirmDeletePermanently");
            public static string ConfirmDeleteReference => GetValue("ConfirmDeleteReference");
            public static string ConfirmDelete => GetValue("ConfirmDelete");
            public static string ConfirmDeleteReferenced => GetValue("ConfirmDeleteReferenced");
            public static string CannotAddNewItem => GetValue("CannotAddNewItem");
            public static string UnsupportedFilename => GetValue("UnsupportedFilename");
            public static string UnsupportedExtension => GetValue("UnsupportedExtension");
            public static string CannotChangePath => GetValue("CannotChangePath");
            public static string CopiedToClipboard => GetValue("CopiedToClipboard");
            public static string UnknownGuid => GetValue("UnknownGuid");
            public static string HasBeenAdded => GetValue("HasBeenAdded");
            public static string EditInitialData => GetValue("EditInitialData");
            public static string PromptIsOpenItem => GetValue("PromptIsOpenItem");
            public static string RestartApp => GetValue("RestartApp");
            public static string UpdateApp => GetValue("UpdateApp");
            public static string FileOrFolderNotExists => GetValue("FileOrFolderNotExists");
        }

        /// <summary>其他文本</summary>
        public static class Other
        {
            private static string GetValue(string key) => GetStringValue("Other", key);
            public static string RestartExplorer => GetValue("RestartExplorer");
            public static string DictionaryDescription => GetValue("DictionaryDescription");
            public static string LanguageDictionary => GetValue("LanguageDictionary");
            public static string GuidInfosDictionary => GetValue("GuidInfosDictionary");
            public static string ThridRulesDictionary => GetValue("ThridRulesDictionary");
            public static string CommonItemsDictionary => GetValue("CommonItemsDictionary");
            public static string Translators => GetValue("Translators");
            public static string OtherLanguages => GetValue("OtherLanguages");
            public static string SelectSubMenuMode => GetValue("SelectSubMenuMode");
            public static string AboutApp => GetValue("AboutApp");
            public static string Dictionaries => GetValue("Dictionaries");
            public static string Donate => GetValue("Donate");
        }

        /// <summary>提示文本</summary>
        public static class Tip
        {
            private static string GetValue(string key) => GetStringValue("Tip", key);
            public static string RestartExplorer => GetValue("RestartExplorer");
            public static string CustomFolder => GetValue("CustomFolder");
            public static string SendToDrive => GetValue("SendToDrive");
            public static string BuildSendtoMenu => GetValue("BuildSendtoMenu");
            public static string EditSubItems => GetValue("EditSubItems");
            public static string InvalidItem => GetValue("InvalidItem");
            public static string AddSeparator => GetValue("AddSeparator");
            public static string Separator => GetValue("Separator");
            public static string AddExistingItems => GetValue("AddExistingItems");
            public static string AddCommonItems => GetValue("AddCommonItems");
            public static string DeleteGuidDic => GetValue("DeleteGuidDic");
        }
    }
}