using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Methods
{
    static class AppConfig
    {
        static AppConfig()
        {
            CreateDirectory();
            ReloadConfig();
            LoadLanguage();
        }

        public const string GithubLatest = "https://github.com/BluePointLilac/ContextMenuManager/releases/latest";
        public const string GithubLatestApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/releases/latest";
        public const string GithubLangsApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/contents/languages";
        public const string GithubLangsRawDir = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/languages";
        public const string GithubShellNewApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/contents/ContextMenuManager/Properties/Resources/ShellNew";
        public const string GithubShellNewRawDir = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/ShellNew";
        public const string GithubTexts = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/Texts";
        public const string GithubDonateRaw = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/Donate.md";
        public const string GithubDonate = "https://github.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        public const string GiteeReleases = "https://gitee.com/BluePointLilac/ContextMenuManager/releases";
        public const string GiteeLatestApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/releases/latest";
        public const string GiteeLangsApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/languages";
        public const string GiteeLangsRawDir = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/languages";
        public const string GiteeShellNewApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/ContextMenuManager/Properties/Resources/ShellNew";
        public const string GiteeShellNewRawDir = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/ShellNew";
        public const string GiteeTexts = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts";
        public const string GiteeDonateRaw = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Donate.md";
        public const string GiteeDonate = "https://gitee.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        public static readonly string AppConfigDir = $@"{Application.StartupPath}\Config";
        public static readonly string AppDataDir = Environment.ExpandEnvironmentVariables(@"%AppData%\ContextMenuManager");
        public static readonly string AppDataConfigDir = $@"{AppDataDir}\Config";
        public static readonly string ConfigDir = Directory.Exists(AppConfigDir) ? AppConfigDir : AppDataConfigDir;
        public static readonly bool SaveToAppDir = ConfigDir == AppConfigDir;
        public static readonly bool IsFirstRun = !Directory.Exists(ConfigDir);
        public static string ConfigIni = $@"{ConfigDir}\Config.ini";
        public static string BackupDir = $@"{ConfigDir}\Backup";
        public static string LangsDir = $@"{ConfigDir}\Languages";
        public static string ProgramsDir = $@"{ConfigDir}\Programs";
        public static string DicsDir = $@"{ConfigDir}\Dictionaries";
        public static string WebDicsDir = $@"{DicsDir}\Web";
        public static string UserDicsDir = $@"{DicsDir}\User";

        public static string WebGuidInfosDic = $@"{WebDicsDir}\{GUIDINFOSDICINI}";
        public static string WebDetailedEditDic = $@"{WebDicsDir}\{DETAILEDEDITDICXML}";
        public static string WebEnhanceMenusDic = $@"{WebDicsDir}\{ENHANCEMENUSICXML}";
        public static string WebUwpModeItemsDic = $@"{WebDicsDir}\{UWPMODEITEMSDICXML}";

        public static string UserGuidInfosDic = $@"{UserDicsDir}\{GUIDINFOSDICINI}";
        public static string UserDetailedEditDic = $@"{UserDicsDir}\{DETAILEDEDITDICXML}";
        public static string UserEnhanceMenusDic = $@"{UserDicsDir}\{ENHANCEMENUSICXML}";
        public static string UserUwpModeItemsDic = $@"{UserDicsDir}\{UWPMODEITEMSDICXML}";

        public const string ZH_CNINI = "zh-CN.ini";
        public const string GUIDINFOSDICINI = "GuidInfosDic.ini";
        public const string DETAILEDEDITDICXML = "DetailedEditDic.xml";
        public const string ENHANCEMENUSICXML = "EnhanceMenusDic.xml";
        public const string UWPMODEITEMSDICXML = "UwpModeItemsDic.xml";

        public static readonly Dictionary<string, string> EngineUrlsDic = new Dictionary<string, string>
        {
            { "Bing", "https://www.bing.com/search?q=%s" },
            { "Baidu", "https://www.baidu.com/s?wd=%s" },
            { "Google", "https://www.google.com/search?q=%s" },
            { "Yandex", "https://yandex.com/search/?text=%s" },
            { "DuckDuckGo", "https://duckduckgo.com/?q=%s" },
            { "Sogou", "https://www.sogou.com/web?query=%s" },
            { "360", "https://www.so.com/s?q=%s" },
        };

        private static readonly IniReader ConfigReader = new IniReader(ConfigIni);
        private static readonly IniWriter ConfigWriter = new IniWriter(ConfigIni);

        private static string GetGeneralValue(string key)
        {
            return ConfigReader.GetValue("General", key);
        }

        private static void SetGeneralValue(string key, object value)
        {
            ConfigWriter.SetValue("General", key, value);
            ReloadConfig();
        }

        private static string GetWindowValue(string key)
        {
            return ConfigReader.GetValue("Window", key);
        }

        private static void SetWindowValue(string key, object value)
        {
            ConfigWriter.SetValue("Window", key, value);
            ReloadConfig();
        }

        public static void ReloadConfig()
        {
            ConfigReader.LoadFile(ConfigIni);
        }

        private static void CreateDirectory()
        {
            foreach(string dirPath in new[] { AppDataDir, ConfigDir, ProgramsDir, BackupDir, LangsDir, DicsDir, WebDicsDir, UserDicsDir })
            {
                Directory.CreateDirectory(dirPath);
                Application.ApplicationExit += (sender, e) =>
                {
                    if(Directory.Exists(dirPath) && Directory.GetFileSystemEntries(dirPath).Length == 0)
                    {
                        Directory.Delete(dirPath);
                    }
                };
            }
        }

        private static void LoadLanguage()
        {
            language = GetGeneralValue("Language");
            if(language.ToLower() == "default")
            {
                LanguageIniPath = "";
                return;
            }
            if(language == "") language = CultureInfo.CurrentUICulture.Name;
            LanguageIniPath = $@"{LangsDir}\{language}.ini";
            if(!File.Exists(LanguageIniPath))
            {
                LanguageIniPath = "";
                Language = "";
            }
        }

        public static string LanguageIniPath { get; private set; }

        private static string language;
        public static string Language
        {
            get => language;
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
                    return DateTime.MinValue;
                    //返回文件上次修改时间
                    //return new FileInfo(Application.ExecutablePath).LastWriteTime;
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
                if(string.IsNullOrEmpty(url)) url = EngineUrlsDic.Values.ToArray()[0];
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

        public static bool OpenMoreExplorer
        {
            get => GetGeneralValue("OpenMoreExplorer") == "1";
            set => SetGeneralValue("OpenMoreExplorer", value ? 1 : 0);
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
                string value = GetGeneralValue("RequestUseGithub");
                if(!string.IsNullOrEmpty(value)) return value == "1";
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

        public static bool TopMost
        {
            get => GetWindowValue("TopMost") == "1";
            set => SetWindowValue("TopMost", value ? 1 : 0);
        }

        public static Size MainFormSize
        {
            get
            {
                string str = GetWindowValue("MainFormSize");
                int index = str.IndexOf(',');
                if(index == -1) return Size.Empty;
                if(int.TryParse(str.Substring(0, index), out int x))
                    if(int.TryParse(str.Substring(index + 1), out int y))
                        return new Size(x, y);
                return Size.Empty;
            }
            set => SetWindowValue("MainFormSize", value.Width + "," + value.Height);
        }
    }
}