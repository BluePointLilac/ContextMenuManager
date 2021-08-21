using BluePointLilac.Methods;
using System;
using System.Reflection;
using System.Text;

namespace ContextMenuManager.Methods
{
    static class AppString
    {
        private static readonly IniReader UserLangReader = new IniReader(AppConfig.LanguageIniPath);
        public static readonly IniReader DefLangReader = new IniReader(new StringBuilder(Properties.Resources.AppLanguageDic));

        private static string GetValue(string section, string key)
        {
            string value = UserLangReader.GetValue(section, key);
            if(string.IsNullOrEmpty(value)) value = DefLangReader.GetValue(section, key);
            return value.Replace("\\r", "\r").Replace("\\n", "\n");
        }

        /// <summary>加载语言</summary>
        public static void LoadStrings()
        {
            foreach(Type type in typeof(AppString).GetNestedTypes())
            {
                foreach(PropertyInfo pi in type.GetProperties())
                {
                    pi.SetValue(type, GetValue(type.Name, pi.Name), null);
                }
            }
        }

        /// <summary>常规</summary>
        public static class General
        {
            public static string AppName { get; set; }
        }

        /// <summary>工具栏</summary>
        public static class ToolBar
        {
            public static string Home { get; set; }
            public static string Type { get; set; }
            public static string Rule { get; set; }
            public static string Refresh { get; set; }
            public static string About { get; set; }
        }

        /// <summary>侧边栏</summary>
        public static class SideBar
        {
            public static string File { get; set; }
            public static string Folder { get; set; }
            public static string Directory { get; set; }
            public static string Background { get; set; }
            public static string Desktop { get; set; }
            public static string Drive { get; set; }
            public static string AllObjects { get; set; }
            public static string Computer { get; set; }
            public static string RecycleBin { get; set; }
            public static string Library { get; set; }
            public static string New { get; set; }
            public static string SendTo { get; set; }
            public static string OpenWith { get; set; }
            public static string WinX { get; set; }
            public static string LnkFile { get; set; }
            public static string UwpLnk { get; set; }
            public static string ExeFile { get; set; }
            public static string UnknownType { get; set; }
            public static string MenuAnalysis { get; set; }
            public static string CustomExtension { get; set; }
            public static string PerceivedType { get; set; }
            public static string DirectoryType { get; set; }
            public static string EnhanceMenu { get; set; }
            public static string DetailedEdit { get; set; }
            public static string GuidBlocked { get; set; }
            public static string DragDrop { get; set; }
            public static string PublicReferences { get; set; }
            public static string CustomRegPath { get; set; }
            public static string IEMenu { get; set; }
            public static string AppSetting { get; set; }
            public static string AboutApp { get; set; }
            public static string Dictionaries { get; set; }
            public static string AppLanguage { get; set; }
            public static string Donate { get; set; }
        }

        /// <summary>状态栏</summary>
        public static class StatusBar
        {
            public static string File { get; set; }
            public static string Folder { get; set; }
            public static string Directory { get; set; }
            public static string Background { get; set; }
            public static string Desktop { get; set; }
            public static string Drive { get; set; }
            public static string AllObjects { get; set; }
            public static string Computer { get; set; }
            public static string RecycleBin { get; set; }
            public static string Library { get; set; }
            public static string New { get; set; }
            public static string SendTo { get; set; }
            public static string OpenWith { get; set; }
            public static string WinX { get; set; }
            public static string LnkFile { get; set; }
            public static string UwpLnk { get; set; }
            public static string ExeFile { get; set; }
            public static string UnknownType { get; set; }
            public static string MenuAnalysis { get; set; }
            public static string CustomExtension { get; set; }
            public static string PerceivedType { get; set; }
            public static string DirectoryType { get; set; }
            public static string EnhanceMenu { get; set; }
            public static string DetailedEdit { get; set; }
            public static string GuidBlocked { get; set; }
            public static string DragDrop { get; set; }
            public static string PublicReferences { get; set; }
            public static string CustomRegPath { get; set; }
            public static string IEMenu { get; set; }
        }

        /// <summary>程序内右键菜单</summary>
        public static class Menu
        {
            public static string ChangeText { get; set; }
            public static string ItemIcon { get; set; }
            public static string ChangeIcon { get; set; }
            public static string AddIcon { get; set; }
            public static string DeleteIcon { get; set; }
            public static string ShieldIcon { get; set; }
            public static string ItemPosition { get; set; }
            public static string SetDefault { get; set; }
            public static string SetTop { get; set; }
            public static string SetBottom { get; set; }
            public static string OtherAttributes { get; set; }
            public static string OnlyWithShift { get; set; }
            public static string OnlyInExplorer { get; set; }
            public static string NoWorkingDirectory { get; set; }
            public static string NeverDefault { get; set; }
            public static string ShowAsDisabledIfHidden { get; set; }
            public static string Details { get; set; }
            public static string WebSearch { get; set; }
            public static string ChangeCommand { get; set; }
            public static string RunAsAdministrator { get; set; }
            public static string FileProperties { get; set; }
            public static string FileLocation { get; set; }
            public static string RegistryLocation { get; set; }
            public static string ExportRegistry { get; set; }
            public static string Delete { get; set; }
            public static string DeleteReference { get; set; }
            public static string HandleGuid { get; set; }
            public static string CopyGuid { get; set; }
            public static string BlockGuid { get; set; }
            public static string AddGuidDic { get; set; }
            public static string ClsidLocation { get; set; }
            public static string InitialData { get; set; }
            public static string BeforeSeparator { get; set; }
            public static string ChangeGroup { get; set; }
            public static string RestoreDefault { get; set; }
            public static string Edit { get; set; }
            public static string Save { get; set; }
            public static string FoldAll { get; set; }
            public static string UnfoldAll { get; set; }
        }

        /// <summary>对话框子窗口</summary>
        public static class Dialog
        {
            public static string Browse { get; set; }
            public static string Program { get; set; }
            public static string AllFiles { get; set; }
            public static string RegistryFile { get; set; }
            public static string ItemText { get; set; }
            public static string ItemCommand { get; set; }
            public static string CommandArguments { get; set; }
            public static string SingleMenu { get; set; }
            public static string MultiMenu { get; set; }
            public static string Public { get; set; }
            public static string Private { get; set; }
            public static string SelectAll { get; set; }
            public static string InputGuid { get; set; }
            public static string AddGuidDic { get; set; }
            public static string DeleteGuidDic { get; set; }
            public static string NoPerceivedType { get; set; }
            public static string TextFile { get; set; }
            public static string DocumentFile { get; set; }
            public static string ImageFile { get; set; }
            public static string VideoFile { get; set; }
            public static string AudioFile { get; set; }
            public static string CompressedFile { get; set; }
            public static string SystemFile { get; set; }
            public static string DocumentDirectory { get; set; }
            public static string ImageDirectory { get; set; }
            public static string VideoDirectory { get; set; }
            public static string AudioDirectory { get; set; }
            public static string EditSubItems { get; set; }
            public static string DetailedEdit { get; set; }
            public static string CheckReference { get; set; }
            public static string CheckCopy { get; set; }
            public static string SelectExtension { get; set; }
            public static string SelectPerceivedType { get; set; }
            public static string SelectDirectoryType { get; set; }
            public static string SelectNewItemType { get; set; }
            public static string SelectGroup { get; set; }
            public static string SelectObjectType { get; set; }
            public static string SelectDropEffect { get; set; }
            public static string DefaultDropEffect { get; set; }
            public static string CopyDropEffect { get; set; }
            public static string MoveDropEffect { get; set; }
            public static string CreateLinkDropEffect { get; set; }
            public static string DownloadLanguages { get; set; }
            public static string TranslateTool { get; set; }
            public static string DefaultText { get; set; }
            public static string OldTranslation { get; set; }
            public static string NewTranslation { get; set; }
            public static string DonateInfo { get; set; }
        }

        /// <summary>消息</summary>
        public static class Message
        {
            public static string TextCannotBeEmpty { get; set; }
            public static string CommandCannotBeEmpty { get; set; }
            public static string StringParsingFailed { get; set; }
            public static string TextLengthCannotExceed80 { get; set; }
            public static string ConfirmDeletePermanently { get; set; }
            public static string DeleteButCanRestore { get; set; }
            public static string ConfirmDeleteReference { get; set; }
            public static string ConfirmDelete { get; set; }
            public static string ConfirmDeleteReferenced { get; set; }
            public static string CannotAddNewItem { get; set; }
            public static string VistaUnsupportedMulti { get; set; }
            public static string CannotHideSubItem { get; set; }
            public static string UnsupportedFilename { get; set; }
            public static string NoOpenModeExtension { get; set; }
            public static string CannotChangePath { get; set; }
            public static string CopiedToClipboard { get; set; }
            public static string MalformedGuid { get; set; }
            public static string HasBeenAdded { get; set; }
            public static string EditInitialData { get; set; }
            public static string PromptIsOpenItem { get; set; }
            public static string SelectRegPath { get; set; }
            public static string RestartApp { get; set; }
            public static string UpdateInfo { get; set; }
            public static string UpdateSucceeded { get; set; }
            public static string DicUpdateSucceeded { get; set; }
            public static string FileNotExists { get; set; }
            public static string FolderNotExists { get; set; }
            public static string VersionIsLatest { get; set; }
            public static string AuthorityProtection { get; set; }
            public static string WinXSorted { get; set; }
            public static string RestoreDefault { get; set; }
            public static string DeleteGroup { get; set; }
            public static string WebDataReadFailed { get; set; }
            public static string OpenWebUrl { get; set; }
            public static string SelectSubMenuMode { get; set; }
        }

        /// <summary>提示文本</summary>
        public static class Tip
        {
            public static string RestartExplorer { get; set; }
            public static string CustomFolder { get; set; }
            public static string SendToDrive { get; set; }
            public static string BuildSendtoMenu { get; set; }
            public static string EditSubItems { get; set; }
            public static string InvalidItem { get; set; }
            public static string AddSeparator { get; set; }
            public static string AddReference { get; set; }
            public static string AddFromPublic { get; set; }
            public static string AddFromParentMenu { get; set; }
            public static string DeleteGuidDic { get; set; }
            public static string LockNewMenu { get; set; }
            public static string ConfigPath { get; set; }
            public static string CommandFiles { get; set; }
            public static string CreateGroup { get; set; }
            public static string DropOrSelectObject { get; set; }
            public static string ImmediatelyCheck { get; set; }
        }

        /// <summary>其他文本</summary>
        public static class Other
        {
            public static string CustomFolder { get; set; }
            public static string BuildSendtoMenu { get; set; }
            public static string NewItem { get; set; }
            public static string AddGuidBlockedItem { get; set; }
            public static string CurrentExtension { get; set; }
            public static string CurrentPerceivedType { get; set; }
            public static string CurrentDirectoryType { get; set; }
            public static string CurrentFilePath { get; set; }
            public static string CurrentRegPath { get; set; }
            public static string SelectRegPath { get; set; }
            public static string InvalidItem { get; set; }
            public static string Separator { get; set; }
            public static string LockNewMenu { get; set; }
            public static string RestartExplorer { get; set; }
            public static string WebDictionaries { get; set; }
            public static string SwitchDictionaries { get; set; }
            public static string UserDictionaries { get; set; }
            public static string DictionaryDescription { get; set; }
            public static string GuidInfosDictionary { get; set; }
            public static string UwpMode { get; set; }
            public static string Translators { get; set; }
            public static string AboutApp { get; set; }
            public static string Dictionaries { get; set; }
            public static string Donate { get; set; }
            public static string DonationList { get; set; }
            public static string ConfigPath { get; set; }
            public static string AppDataDir { get; set; }
            public static string AppDir { get; set; }
            public static string AutoBackup { get; set; }
            public static string SetUpdateFrequency { get; set; }
            public static string OnceAWeek { get; set; }
            public static string OnceAMonth { get; set; }
            public static string OnceASeason { get; set; }
            public static string NeverCheck { get; set; }
            public static string SetRequestRepo { get; set; }
            public static string ProtectOpenItem { get; set; }
            public static string WebSearchEngine { get; set; }
            public static string CustomEngine { get; set; }
            public static string SetCustomEngine { get; set; }
            public static string WinXSortable { get; set; }
            public static string ShowFilePath { get; set; }
            public static string OpenMoreRegedit { get; set; }
            public static string OpenMoreExplorer { get; set; }
            public static string HideDisabledItems { get; set; }
            public static string HideSysStoreItems { get; set; }
            public static string SetPerceivedType { get; set; }
            public static string SetDefaultDropEffect { get; set; }
            public static string TopMost { get; set; }
        }
    }
}
