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

        private static string GetGeneralValue(string key) => GetValue("General", key);
        private static string GetToolBarValue(string key) => GetValue("ToolBar", key);
        private static string GetSideBarValue(string key) => GetValue("SideBar", key);
        private static string GetStatusBarValue(string key) => GetValue("StatusBar", key);
        private static string GetMessageBoxValue(string key) => GetValue("MessageBox", key);
        private static string GetMenuValue(string key) => GetValue("Menu", key);
        private static string GetTextValue(string key) => GetValue("Text", key);
        private static string GetTipValue(string key) => GetValue("Tip", key);

        #region 常规
        public static string General_Language = GetGeneralValue("Language");
        public static string General_AppName = GetGeneralValue("AppName");
        #endregion
        #region 工具栏
        public static string ToolBar_Home = GetToolBarValue("Home");
        public static string ToolBar_Type = GetToolBarValue("Type");
        public static string ToolBar_Rule = GetToolBarValue("Rule");
        public static string ToolBar_About = GetToolBarValue("About");
        #endregion
        #region 侧边栏
        public static string SideBar_File = GetSideBarValue("File");
        public static string SideBar_Folder = GetSideBarValue("Folder");
        public static string SideBar_Directory = GetSideBarValue("Directory");
        public static string SideBar_Background = GetSideBarValue("Background");
        public static string SideBar_Desktop = GetSideBarValue("Desktop");
        public static string SideBar_Drive = GetSideBarValue("Drive");
        public static string SideBar_AllObjects = GetSideBarValue("AllObjects");
        public static string SideBar_Computer = GetSideBarValue("Computer");
        public static string SideBar_RecycleBin = GetSideBarValue("RecycleBin");
        public static string SideBar_Library = GetSideBarValue("Library");
        public static string SideBar_New = GetSideBarValue("New");
        public static string SideBar_SendTo = GetSideBarValue("SendTo");
        public static string SideBar_OpenWith = GetSideBarValue("OpenWith");
        public static string SideBar_WinX = GetSideBarValue("WinX");

        public static string SideBar_LnkFile = GetSideBarValue("LnkFile");
        public static string SideBar_ExeFile = GetSideBarValue("ExeFile");
        public static string SideBar_TextFile = GetSideBarValue("TextFile");
        public static string SideBar_ImageFile = GetSideBarValue("ImageFile");
        public static string SideBar_VideoFile = GetSideBarValue("VideoFile");
        public static string SideBar_AudioFile = GetSideBarValue("AudioFile");
        public static string SideBar_ImageDirectory = GetSideBarValue("ImageDirectory");
        public static string SideBar_VideoDirectory = GetSideBarValue("VideoDirectory");
        public static string SideBar_AudioDirectory = GetSideBarValue("AudioDirectory");
        public static string SideBar_UnknownType = GetSideBarValue("UnknownType");
        public static string SideBar_CustomType = GetSideBarValue("CustomType");

        public static string SideBar_GuidBlocked = GetSideBarValue("GuidBlocked");
        public static string SideBar_ThirdRules = GetSideBarValue("ThirdRules");

        public static string SideBar_AboutApp = GetSideBarValue("AboutApp");
        public static string SideBar_Dictionaries = GetSideBarValue("Dictionaries");
        public static string SideBar_AppLanguage = GetSideBarValue("AppLanguage");
        public static string SideBar_Donate = GetSideBarValue("Donate");
        #endregion
        #region 状态栏
        public static string StatusBar_File = GetStatusBarValue("File");
        public static string StatusBar_Folder = GetStatusBarValue("Folder");
        public static string StatusBar_Directory = GetStatusBarValue("Directory");
        public static string StatusBar_Background = GetStatusBarValue("Background");
        public static string StatusBar_Desktop = GetStatusBarValue("Desktop");
        public static string StatusBar_Drive = GetStatusBarValue("Drive");
        public static string StatusBar_AllObjects = GetStatusBarValue("AllObjects");
        public static string StatusBar_Computer = GetStatusBarValue("Computer");
        public static string StatusBar_RecycleBin = GetStatusBarValue("RecycleBin");
        public static string StatusBar_Library = GetStatusBarValue("Library");
        public static string StatusBar_New = GetStatusBarValue("New");
        public static string StatusBar_SendTo = GetStatusBarValue("SendTo");
        public static string StatusBar_OpenWith = GetStatusBarValue("OpenWith");
        public static string StatusBar_WinX = GetStatusBarValue("WinX");

        public static string StatusBar_LnkFile = GetStatusBarValue("LnkFile");
        public static string StatusBar_ExeFile = GetStatusBarValue("ExeFile");
        public static string StatusBar_TextFile = GetStatusBarValue("TextFile");
        public static string StatusBar_ImageFile = GetStatusBarValue("ImageFile");
        public static string StatusBar_VideoFile = GetStatusBarValue("VideoFile");
        public static string StatusBar_AudioFile = GetStatusBarValue("AudioFile");
        public static string StatusBar_ImageDirectory = GetStatusBarValue("ImageDirectory");
        public static string StatusBar_VideoDirectory = GetStatusBarValue("VideoDirectory");
        public static string StatusBar_AudioDirectory = GetStatusBarValue("AudioDirectory");
        public static string StatusBar_UnknownType = GetStatusBarValue("UnknownType");
        public static string StatusBar_CustomType = GetStatusBarValue("CustomType");

        public static string StatusBar_GuidBlocked = GetStatusBarValue("GuidBlocked");
        public static string StatusBar_ThirdRules = GetStatusBarValue("ThirdRules");
        #endregion
        #region 菜单
        public static string Menu_ChangeText = GetMenuValue("ChangeText");
        public static string Menu_ItemIcon = GetMenuValue("ItemIcon");
        public static string Menu_ChangeIcon = GetMenuValue("ChangeIcon");
        public static string Menu_AddIcon = GetMenuValue("AddIcon");
        public static string Menu_DeleteIcon = GetMenuValue("DeleteIcon");
        public static string Menu_ItemPosition = GetMenuValue("ItemPosition");
        public static string Menu_SetDefault = GetMenuValue("SetDefault");
        public static string Menu_SetTop = GetMenuValue("SetTop");
        public static string Menu_SetBottom = GetMenuValue("SetBottom");
        public static string Menu_OtherAttributes = GetMenuValue("OtherAttributes");
        public static string Menu_OnlyWithShift = GetMenuValue("OnlyWithShift");
        public static string Menu_OnlyInExplorer = GetMenuValue("OnlyInExplorer");
        public static string Menu_NoWorkingDirectory = GetMenuValue("NoWorkingDirectory");
        public static string Menu_ShowSeparator = GetMenuValue("ShowSeparator");
        public static string Menu_Details = GetMenuValue("Details");
        public static string Menu_WebSearch = GetMenuValue("WebSearch");
        public static string Menu_ChangeCommand = GetMenuValue("ChangeCommand");
        public static string Menu_FileProperties = GetMenuValue("FileProperties");
        public static string Menu_FileLocation = GetMenuValue("FileLocation");
        public static string Menu_RegistryLocation = GetMenuValue("RegistryLocation");
        public static string Menu_Delete = GetMenuValue("Delete");
        public static string Menu_DeleteReference = GetMenuValue("DeleteReference");
        public static string Menu_CopyGuid = GetMenuValue("CopyGuid");
        public static string Menu_InitialData = GetMenuValue("InitialData");
        #endregion
        #region 消息框
        public static string MessageBox_TextCannotBeEmpty = GetMessageBoxValue("TextCannotBeEmpty");
        public static string MessageBox_CommandCannotBeEmpty = GetMessageBoxValue("CommandCannotBeEmpty");
        public static string MessageBox_StringParsingFailed = GetMessageBoxValue("StringParsingFailed");
        public static string MessageBox_TextLengthCannotExceed80 = GetMessageBoxValue("TextLengthCannotExceed80");
        public static string MessageBox_ConfirmDeletePermanently = GetMessageBoxValue("ConfirmDeletePermanently");
        public static string MessageBox_ConfirmDeleteReference = GetMessageBoxValue("ConfirmDeleteReference");
        public static string MessageBox_ConfirmDelete = GetMessageBoxValue("ConfirmDelete");
        public static string MessageBox_ConfirmDeleteReferenced = GetMessageBoxValue("ConfirmDeleteReferenced");
        public static string MessageBox_CannotAddNewItem = GetMessageBoxValue("CannotAddNewItem");
        public static string MessageBox_UnsupportedFilename = GetMessageBoxValue("UnsupportedFilename");
        public static string MessageBox_UnsupportedExtension = GetMessageBoxValue("UnsupportedExtension");
        public static string MessageBox_CannotChangePath = GetMessageBoxValue("CannotChangePath");
        public static string MessageBox_CopiedToClipboard = GetMessageBoxValue("CopiedToClipboard");
        public static string MessageBox_UnknownGuid = GetMessageBoxValue("UnknownGuid");
        public static string MessageBox_HasBeenAdded = GetMessageBoxValue("HasBeenAdded");
        public static string MessageBox_EditInitialData = GetMessageBoxValue("EditInitialData");
        public static string MessageBox_PromptIsOpenItem = GetMessageBoxValue("PromptIsOpenItem");
        public static string MessageBox_RestartApp = GetMessageBoxValue("RestartApp");
        public static string MessageBox_UpdateApp = GetMessageBoxValue("UpdateApp");
        #endregion
        #region 其他文本
        public static string Text_ItemName = GetTextValue("ItemName");
        public static string Text_ItemCommand = GetTextValue("ItemCommand");

        public static string Text_Single = GetTextValue("Single");
        public static string Text_Multi = GetTextValue("Multi");

        public static string Text_EditSubItems = GetTextValue("EditSubItems");
        public static string Text_Separator = GetTextValue("Separator");
        public static string Text_InvalidItem = GetTextValue("InvalidItem");
        public static string Text_CheckReference = GetTextValue("CheckReference");
        public static string Text_CheckCommon = GetTextValue("CheckCommon");
        public static string Text_InputGuid = GetTextValue("InputGuid");

        public static string Text_Explore = GetTextValue("Explore");
        public static string Text_CustomFolder = GetTextValue("CustomFolder");
        public static string Text_BuildSendtoMenu = GetTextValue("BuildSendtoMenu");
        public static string Text_UseStoreOpenWith = GetTextValue("UseStoreOpenWith");
        public static string Text_RestartExplorer = GetTextValue("RestartExplorer");

        public static string Text_NewItem = GetTextValue("NewItem");
        public static string Text_NewShellItem = GetTextValue("NewShellItem");
        public static string Text_NewSendToItem = GetTextValue("NewSendToItem");
        public static string Text_NewOpenWithItem = GetTextValue("NewOpenWithItem");
        public static string Text_NewGuidBlockedItem = GetTextValue("NewGuidBlockedItem");

        public static string Text_SelectExtension = GetTextValue("SelectExtension");
        public static string Text_CurrentExtension = GetTextValue("CurrentExtension");

        public static string Text_DictionaryDescription = GetTextValue("DictionaryDescription");
        public static string Text_LanguageDictionary = GetTextValue("LanguageDictionary");
        public static string Text_GuidInfosDictionary = GetTextValue("GuidInfosDictionary");
        public static string Text_ThridRulesDictionary = GetTextValue("ThridRulesDictionary");
        public static string Text_CommonItemsDictionary = GetTextValue("CommonItemsDictionary");
        public static string Text_Translators = GetTextValue("Translators");
        public static string Text_OtherLanguages = GetTextValue("OtherLanguages");
        public static string Text_SelectSubMenuMode = GetTextValue("SelectSubMenuMode");
        public static string Text_AboutApp = GetTextValue("AboutApp");
        public static string Text_Dictionaries = GetTextValue("Dictionaries");
        public static string Text_Donate = GetTextValue("Donate");
        #endregion
        #region 提示
        public static string Tip_RestartExplorer = GetTipValue("RestartExplorer");
        public static string Tip_CustomFolder = GetTipValue("CustomFolder");
        public static string Tip_SendToDrive = GetTipValue("SendToDrive");
        public static string Tip_BuildSendtoMenu = GetTipValue("BuildSendtoMenu");
        public static string Tip_UseStoreOpenWith = GetTipValue("UseStoreOpenWith");
        public static string Tip_EditSubItems = GetTipValue("EditSubItems");
        public static string Tip_InvalidItem = GetTipValue("InvalidItem");
        public static string Tip_AddSeparator = GetTipValue("AddSeparator");
        public static string Tip_Separator = GetTipValue("Separator");
        public static string Tip_AddExistingItems = GetTipValue("AddExistingItems");
        public static string Tip_AddCommonItems = GetTipValue("AddCommonItems");
        #endregion
        #region 国际化字符串
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
        #endregion
    }
}