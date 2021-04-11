using BluePointLilac.Methods;
using System;
using System.Text;

namespace ContextMenuManager
{
    public static class AppString
    {
        public static readonly IniReader UserLanguage = new IniReader(AppConfig.LanguageIniPath);
        public static readonly IniReader DefaultLanguage = new IniReader(new StringBuilder(Properties.Resources.AppLanguageDic));

        private static string GetStringValue(string section, string key)
        {
            string value = UserLanguage.GetValue(section, key);
            if(string.IsNullOrEmpty(value)) value = DefaultLanguage.GetValue(section, key);
            return value.Replace("\\n", Environment.NewLine);
        }

        /// <summary>常规</summary>
        public static class General
        {
            private static string GetValue(string key) => GetStringValue("General", key);
            public static string AppName => GetValue("AppName");
        }

        /// <summary>工具栏</summary>
        public static class ToolBar
        {
            private static string GetValue(string key) => GetStringValue("ToolBar", key);
            public static string Home => GetValue("Home");
            public static string Type => GetValue("Type");
            public static string Rule => GetValue("Rule");
            public static string Refresh => GetValue("Refresh");
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
            public static string UnknownType => GetValue("UnknownType");
            public static string MenuAnalysis => GetValue("MenuAnalysis");
            public static string CustomExtension => GetValue("CustomExtension");
            public static string PerceivedType => GetValue("PerceivedType");
            public static string DirectoryType => GetValue("DirectoryType");
            public static string EnhanceMenu => GetValue("EnhanceMenu");
            public static string ThirdRules => GetValue("ThirdRules");
            public static string OtherAccounts => GetValue("OtherAccounts");
            public static string GuidBlocked => GetValue("GuidBlocked");
            public static string DragDrop => GetValue("DragDrop");
            public static string PublicReferences => GetValue("PublicReferences");
            public static string CustomRegPath => GetValue("CustomRegPath");
            public static string IEMenu => GetValue("IEMenu");
            public static string AppSetting => GetValue("AppSetting");
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
            public static string UnknownType => GetValue("UnknownType");
            public static string MenuAnalysis => GetValue("MenuAnalysis");
            public static string CustomExtension => GetValue("CustomExtension");
            public static string PerceivedType => GetValue("PerceivedType");
            public static string DirectoryType => GetValue("DirectoryType");
            public static string EnhanceMenu => GetValue("EnhanceMenu");
            public static string ThirdRules => GetValue("ThirdRules");
            public static string GuidBlocked => GetValue("GuidBlocked");
            public static string DragDrop => GetValue("DragDrop");
            public static string PublicReferences => GetValue("PublicReferences");
            public static string CustomRegPath => GetValue("CustomRegPath");
            public static string IEMenu => GetValue("IEMenu");
        }

        /// <summary>程序内右键菜单</summary>
        public static class Menu
        {
            private static string GetValue(string key) => GetStringValue("Menu", key);
            public static string ChangeText => GetValue("ChangeText");
            public static string ItemIcon => GetValue("ItemIcon");
            public static string ChangeIcon => GetValue("ChangeIcon");
            public static string AddIcon => GetValue("AddIcon");
            public static string DeleteIcon => GetValue("DeleteIcon");
            public static string ShieldIcon => GetValue("ShieldIcon");
            public static string ItemPosition => GetValue("ItemPosition");
            public static string SetDefault => GetValue("SetDefault");
            public static string SetTop => GetValue("SetTop");
            public static string SetBottom => GetValue("SetBottom");
            public static string OtherAttributes => GetValue("OtherAttributes");
            public static string OnlyWithShift => GetValue("OnlyWithShift");
            public static string OnlyInExplorer => GetValue("OnlyInExplorer");
            public static string NoWorkingDirectory => GetValue("NoWorkingDirectory");
            public static string NeverDefault => GetValue("NeverDefault");
            public static string ShowAsDisabledIfHidden => GetValue("ShowAsDisabledIfHidden");
            public static string Details => GetValue("Details");
            public static string WebSearch => GetValue("WebSearch");
            public static string ChangeCommand => GetValue("ChangeCommand");
            public static string RunAsAdministrator => GetValue("RunAsAdministrator");
            public static string FileProperties => GetValue("FileProperties");
            public static string FileLocation => GetValue("FileLocation");
            public static string RegistryLocation => GetValue("RegistryLocation");
            public static string ExportRegistry => GetValue("ExportRegistry");
            public static string Delete => GetValue("Delete");
            public static string DeleteReference => GetValue("DeleteReference");
            public static string HandleGuid => GetValue("HandleGuid");
            public static string CopyGuid => GetValue("CopyGuid");
            public static string BlockGuid => GetValue("BlockGuid");
            public static string AddGuidDic => GetValue("AddGuidDic");
            public static string InitialData => GetValue("InitialData");
            public static string BeforeSeparator => GetValue("BeforeSeparator");
            public static string ChangeGroup => GetValue("ChangeGroup");
            public static string RestoreDefault => GetValue("RestoreDefault");
            public static string Edit => GetValue("Edit");
            public static string Save => GetValue("Save");
        }

        /// <summary>对话框子窗口</summary>
        public static class Dialog
        {
            private static string GetValue(string key) => GetStringValue("Dialog", key);
            public static string Ok => GetValue("Ok");
            public static string Cancel => GetValue("Cancel");
            public static string Browse => GetValue("Browse");
            public static string Program => GetValue("Program");
            public static string ItemText => GetValue("ItemText");
            public static string ItemCommand => GetValue("ItemCommand");
            public static string CommandArguments => GetValue("CommandArguments");
            public static string SingleMenu => GetValue("SingleMenu");
            public static string MultiMenu => GetValue("MultiMenu");
            public static string Public => GetValue("Public");
            public static string Private => GetValue("Private");
            public static string InputGuid => GetValue("InputGuid");
            public static string AddGuidDic => GetValue("AddGuidDic");
            public static string DeleteGuidDic => GetValue("DeleteGuidDic");
            public static string NoPerceivedType => GetValue("NoPerceivedType");
            public static string TextFile => GetValue("TextFile");
            public static string DocumentFile => GetValue("DocumentFile");
            public static string ImageFile => GetValue("ImageFile");
            public static string VideoFile => GetValue("VideoFile");
            public static string AudioFile => GetValue("AudioFile");
            public static string CompressedFile => GetValue("CompressedFile");
            public static string SystemFile => GetValue("SystemFile");
            public static string DocumentDirectory => GetValue("DocumentDirectory");
            public static string ImageDirectory => GetValue("ImageDirectory");
            public static string VideoDirectory => GetValue("VideoDirectory");
            public static string AudioDirectory => GetValue("AudioDirectory");
            public static string EditSubItems => GetValue("EditSubItems");
            public static string CheckReference => GetValue("CheckReference");
            public static string CheckCopy => GetValue("CheckCopy");
            public static string SelectExtension => GetValue("SelectExtension");
            public static string SelectPerceivedType => GetValue("SelectPerceivedType");
            public static string SelectDirectoryType => GetValue("SelectDirectoryType");
            public static string SelectSubMenuMode => GetValue("SelectSubMenuMode");
            public static string SelectNewItemType => GetValue("SelectNewItemType");
            public static string RegistryFile => GetValue("RegistryFile");
            public static string SelectGroup => GetValue("SelectGroup");
            public static string SelectObjectType => GetValue("SelectObjectType");
            public static string TranslateTool => GetValue("TranslateTool");
            public static string DefaultText => GetValue("DefaultText");
            public static string OldTranslation => GetValue("OldTranslation");
            public static string NewTranslation => GetValue("NewTranslation");
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
            public static string DeleteButCanRestore => GetValue("DeleteButCanRestore");
            public static string ConfirmDeleteReference => GetValue("ConfirmDeleteReference");
            public static string ConfirmDelete => GetValue("ConfirmDelete");
            public static string ConfirmDeleteReferenced => GetValue("ConfirmDeleteReferenced");
            public static string CannotAddNewItem => GetValue("CannotAddNewItem");
            public static string VistaUnsupportedMulti => GetValue("VistaUnsupportedMulti");
            public static string CannotHideSubItem => GetValue("CannotHideSubItem");
            public static string UnsupportedFilename => GetValue("UnsupportedFilename");
            public static string NoOpenModeExtension => GetValue("NoOpenModeExtension");
            public static string CannotChangePath => GetValue("CannotChangePath");
            public static string CopiedToClipboard => GetValue("CopiedToClipboard");
            public static string MalformedGuid => GetValue("MalformedGuid");
            public static string HasBeenAdded => GetValue("HasBeenAdded");
            public static string EditInitialData => GetValue("EditInitialData");
            public static string PromptIsOpenItem => GetValue("PromptIsOpenItem");
            public static string SelectRegPath => GetValue("SelectRegPath");
            public static string RestartApp => GetValue("RestartApp");
            public static string UpdateApp => GetValue("UpdateApp");
            public static string FileNotExists => GetValue("FileNotExists");
            public static string FolderNotExists => GetValue("FolderNotExists");
            public static string NoUpdateDetected => GetValue("NoUpdateDetected");
            public static string AuthorityProtection => GetValue("AuthorityProtection");
            public static string WinXSorted => GetValue("WinXSorted");
            public static string RestoreDefault => GetValue("RestoreDefault");
            public static string DeleteGroup => GetValue("DeleteGroup");
        }

        /// <summary>其他文本</summary>
        public static class Other
        {
            private static string GetValue(string key) => GetStringValue("Other", key);
            public static string Open => GetValue("Open");
            public static string Edit => GetValue("Edit");
            public static string Explore => GetValue("Explore");
            public static string ExploreOld => GetValue("ExploreOld");
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
            public static string NewItem => GetValue("NewItem");
            public static string AddGuidBlockedItem => GetValue("AddGuidBlockedItem");
            public static string CurrentExtension => GetValue("CurrentExtension");
            public static string CurrentPerceivedType => GetValue("CurrentPerceivedType");
            public static string CurrentDirectoryType => GetValue("CurrentDirectoryType");
            public static string CurrentFilePath => GetValue("CurrentFilePath");
            public static string CurrentRegPath => GetValue("CurrentRegPath");
            public static string SelectRegPath => GetValue("SelectRegPath");
            public static string InvalidItem => GetValue("InvalidItem");
            public static string Separator => GetValue("Separator");
            public static string LockNewMenu => GetValue("LockNewMenu");
            public static string RestartExplorer => GetValue("RestartExplorer");
            public static string DictionaryDescription => GetValue("DictionaryDescription");
            public static string GuidInfosDictionary => GetValue("GuidInfosDictionary");
            public static string Translators => GetValue("Translators");
            public static string AboutApp => GetValue("AboutApp");
            public static string Dictionaries => GetValue("Dictionaries");
            public static string Donate => GetValue("Donate");
            public static string DonationList => GetValue("DonationList");
            public static string ConfigPath => GetValue("ConfigPath");
            public static string AppDataDir => GetValue("AppDataDir");
            public static string AppDir => GetValue("AppDir");
            public static string OpenConfigDir => GetValue("OpenConfigDir");
            public static string AutoBackup => GetValue("AutoBackup");
            public static string OpenBackupDir => GetValue("OpenBackupDir");
            public static string CheckUpdate => GetValue("CheckUpdate");
            public static string ImmediatelyCheckUpdate => GetValue("ImmediatelyCheckUpdate");
            public static string ProtectOpenItem => GetValue("ProtectOpenItem");
            public static string WebSearchEngine => GetValue("WebSearchEngine");
            public static string CustomEngine => GetValue("CustomEngine");
            public static string SetCustomEngine => GetValue("SetCustomEngine");
            public static string WinXSortable => GetValue("WinXSortable");
            public static string ShowFilePath => GetValue("ShowFilePath");
            public static string OpenMoreRegedit => GetValue("OpenMoreRegedit");
            public static string HideDisabledItems => GetValue("HideDisabledItems");
            public static string SetPerceivedType => GetValue("SetPerceivedType");
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
            public static string AddReference => GetValue("AddReference");
            public static string AddFromParentMenu => GetValue("AddFromParentMenu");
            public static string DeleteGuidDic => GetValue("DeleteGuidDic");
            public static string LockNewMenu => GetValue("LockNewMenu");
            public static string CheckUpdate => GetValue("CheckUpdate");
            public static string LastCheckUpdateTime => GetValue("LastCheckUpdateTime");
            public static string OpenLanguagesDir => GetValue("OpenLanguagesDir");
            public static string OtherLanguages => GetValue("OtherLanguages");
            public static string OpenDictionariesDir => GetValue("OpenDictionariesDir");
            public static string ConfigPath => GetValue("ConfigPath");
            public static string CommandFiles => GetValue("CommandFiles");
            public static string CreateGroup => GetValue("CreateGroup");
            public static string DropOrSelectObject => GetValue("DropOrSelectObject");
        }
    }
}