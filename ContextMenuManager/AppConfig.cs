using BluePointLilac.Methods;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager
{
    static class AppConfig
    {
        static AppConfig()
        {
            foreach(string dirPath in new[] { ConfigDir, ProgramsDir, BackupDir, LangsDir, DicsDir, WebDicsDir, UserDicsDir })
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public static readonly string AppConfigDir = $@"{Application.StartupPath}\Config";
        public static readonly string AppDataConfigDir = Environment.ExpandEnvironmentVariables(@"%AppData%\ContextMenuManager\Config");
        public static readonly string ConfigDir = Directory.Exists(AppConfigDir) ? AppConfigDir : AppDataConfigDir;
        public static readonly bool SaveToAppDir = ConfigDir == AppConfigDir;
        public static string ConfigIni = $@"{ConfigDir}\Config.ini";
        public static string BackupDir = $@"{ConfigDir}\Backup";
        public static string LangsDir = $@"{ConfigDir}\Languages";
        public static string ProgramsDir = $@"{ConfigDir}\Programs";
        public static string DicsDir = $@"{ConfigDir}\Dictionaries";
        public static string WebDicsDir = $@"{DicsDir}\Web";
        public static string UserDicsDir = $@"{DicsDir}\User";

        public static string WebGuidInfosDic = $@"{WebDicsDir}\{GUIDINFOSDICINI}";
        public static string WebThirdRulesDic = $@"{WebDicsDir}\{THIRDRULESDICXML}";
        public static string WebEnhanceMenusDic = $@"{WebDicsDir}\{ENHANCEMENUSICXML}";
        public static string WebUwpModeItemsDic = $@"{WebDicsDir}\{UWPMODEITEMSDICXML}";

        public static string UserGuidInfosDic = $@"{UserDicsDir}\{GUIDINFOSDICINI}";
        public static string UserThirdRulesDic = $@"{UserDicsDir}\{THIRDRULESDICXML}";
        public static string UserEnhanceMenusDic = $@"{UserDicsDir}\{ENHANCEMENUSICXML}";
        public static string UserUwpModeItemsDic = $@"{UserDicsDir}\{UWPMODEITEMSDICXML}";

        public const string ZH_CNINI = "zh-CN.ini";
        public const string GUIDINFOSDICINI = "GuidInfosDic.ini";
        public const string THIRDRULESDICXML = "ThirdRulesDic.xml";
        public const string ENHANCEMENUSICXML = "EnhanceMenusDic.xml";
        public const string UWPMODEITEMSDICXML = "UwpModeItemsDic.xml";

        public static readonly string[] EngineUrls =
        {
            "https://www.bing.com/search?q=%s",       //必应搜索
            "https://www.baidu.com/s?wd=%s",          //百度搜索
            "https://www.google.com/search?q=%s",     //谷歌搜索
            "https://duckduckgo.com/?q=%s",           //DuckDuckGo
            "https://www.dogedoge.com/results?q=%s",  //多吉搜索
            "https://www.sogou.com/web?query=%s",     //搜狗搜索
            "https://www.so.com/s?q=%s",              //360搜索
        };

        private static readonly IniWriter ConfigWriter = new IniWriter(ConfigIni);
        private static string GetGeneralValue(string key) => ConfigWriter.GetValue("General", key);
        private static void SetGeneralValue(string key, object value) => ConfigWriter.SetValue("General", key, value);
        public static string LanguageIniPath => $@"{LangsDir}\{Language}.ini";

        public static string Language
        {
            get
            {
                string language = GetGeneralValue("Language");
                if(language == string.Empty)
                {
                    language = CultureInfo.CurrentUICulture.Name;
                }
                if(!File.Exists($@"{LangsDir}\{language}.ini"))
                {
                    language = string.Empty;
                }
                return language;
            }
            set => SetGeneralValue("Language", value);
        }

        public static bool AutoBackup
        {
            get => GetGeneralValue("AutoBackup") != "0";
            set => SetGeneralValue("AutoBackup", value ? 1 : 0);
        }

        public static DateTime LastCheckUpdateTime
        {
            get
            {
                try
                {
                    string time = GetGeneralValue("LastCheckUpdateTime");
                    //二进制数据时间不会受系统时间格式影响
                    return DateTime.FromBinary(Convert.ToInt64(time));
                }
                catch
                {
                    //返回文件上次修改时间
                    return new FileInfo(Application.ExecutablePath).LastWriteTime;
                }
            }
            set => SetGeneralValue("LastCheckUpdateTime", value.ToBinary());
        }

        public static bool ProtectOpenItem
        {
            get => GetGeneralValue("ProtectOpenItem") != "0";
            set => SetGeneralValue("ProtectOpenItem", value ? 1 : 0);
        }

        public static string EngineUrl
        {
            get
            {
                string url = GetGeneralValue("EngineUrl");
                if(url.IsNullOrWhiteSpace()) url = EngineUrls[0];
                return url;
            }
            set => SetGeneralValue("EngineUrl", value);
        }

        public static bool ShowFilePath
        {
            get => GetGeneralValue("ShowFilePath") == "1";
            set => SetGeneralValue("ShowFilePath", value ? 1 : 0);
        }

        public static bool WinXSortable
        {
            get => GetGeneralValue("WinXSortable") == "1";
            set => SetGeneralValue("WinXSortable", value ? 1 : 0);
        }

        public static bool OpenMoreRegedit
        {
            get => GetGeneralValue("OpenMoreRegedit") == "1";
            set => SetGeneralValue("OpenMoreRegedit", value ? 1 : 0);
        }

        public static bool HideDisabledItems
        {
            get => GetGeneralValue("HideDisabledItems") == "1";
            set => SetGeneralValue("HideDisabledItems", value ? 1 : 0);
        }

        public static bool HideSysStoreItems
        {
            get => GetGeneralValue("HideSysStoreItems") != "0";
            set => SetGeneralValue("HideSysStoreItems", value ? 1 : 0);
        }

        public static bool RequestUseGithub
        {
            get
            {
                if(GetGeneralValue("RequestUseGithub") == "1") return true;
                if(CultureInfo.CurrentCulture.Name == "zh-CN") return false;
                return true;
            }
            set => SetGeneralValue("RequestUseGithub", value ? 1 : 0);
        }

        public static int UpdateFrequency
        {
            get
            {
                string value = GetGeneralValue("UpdateFrequency");
                if(int.TryParse(value, out int day))
                {
                    if(day == -1 || day == 7 || day == 90) return day;
                }
                return 30;
            }
            set => SetGeneralValue("UpdateFrequency", value);
        }
    }
}