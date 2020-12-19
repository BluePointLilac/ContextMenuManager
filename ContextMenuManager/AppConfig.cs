using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BulePointLilac.Methods;

namespace ContextMenuManager
{
    static class AppConfig
    {
        static AppConfig()
        {
            CreateDirs();
            DeleteOldFiles();
        }

        [DllImport("kernel32.dll")]
        private static extern ushort GetUserDefaultUILanguage();

        public static readonly string AppConfigDir = $@"{Application.StartupPath}\Config";
        public static readonly string AppDataConfigDir = Environment.ExpandEnvironmentVariables(@"%AppData%\ContextMenuManager\Config");
        public static readonly string ConfigDir = Directory.Exists(AppConfigDir) ? AppConfigDir : AppDataConfigDir;
        public static readonly bool SaveToAppDir = ConfigDir == AppConfigDir;
        public static string ConfigIni = $@"{ConfigDir}\Config.ini";
        public static string BackupDir = $@"{ConfigDir}\Backup";
        public static string LangsDir = $@"{ConfigDir}\Languages";
        public static string DicsDir = $@"{ConfigDir}\Dictionaries";
        public static string WebDicsDir = $@"{DicsDir}\Web";
        public static string UserDicsDir = $@"{DicsDir}\User";
        public static string WebGuidInfosDic = $@"{WebDicsDir}\{GUIDINFOSDICINI}";
        public static string UserGuidInfosDic = $@"{UserDicsDir}\{GUIDINFOSDICINI}";
        public static string WebThirdRulesDic = $@"{WebDicsDir}\{ThIRDRULESDICXML}";
        public static string UserThirdRulesDic = $@"{UserDicsDir}\{ThIRDRULESDICXML}";
        public static string WebEnhanceMenusDic = $@"{WebDicsDir}\{ENHANCEMENUSICXML}";
        public static string UserEnhanceMenusDic = $@"{UserDicsDir}\{ENHANCEMENUSICXML}";

        public const string ZH_CNINI = "zh-CN.ini";
        public const string GUIDINFOSDICINI = "GuidInfosDic.ini";
        public const string ThIRDRULESDICXML = "ThirdRulesDic.xml";
        public const string ENHANCEMENUSICXML = "EnhanceMenusDic.xml";

        public static readonly string[] EngineUrls =
        {
            "https://www.baidu.com/s?wd=%s",          //百度搜索
            "https://www.bing.com/search?q=%s",       //必应搜索
            "https://www.google.com/search?q=%s",     //谷歌搜索
            "https://www.dogedoge.com/results?q=%s",  //多吉搜索
            "https://www.sogou.com/web?query=%s",     //搜狗搜索
            "https://www.so.com/s?q=%s",              //360搜索
        };

        private static IniReader ConfigReader => new IniReader(ConfigIni);
        private static IniWriter ConfigWriter => new IniWriter(ConfigIni);

        public static DateTime LastCheckUpdateTime
        {
            get
            {
                try
                {
                    string time = ConfigReader.GetValue("General", "LastCheckUpdateTime");
                    //二进制数据时间不会受系统时间格式影响
                    return DateTime.FromBinary(Convert.ToInt64(time));
                }
                catch
                {
                    //将上次检测更新时间推前到两个月前
                    return DateTime.Today.AddMonths(-2);
                }
            }
            set
            {
                ConfigWriter.SetValue("General", "LastCheckUpdateTime", value.ToBinary().ToString());
            }
        }

        public static string LanguageIniPath
        {
            get
            {
                string language = ConfigReader.GetValue("General", "Language");
                DirectoryInfo di = new DirectoryInfo(LangsDir);
                if(language == string.Empty && di.Exists)
                {
                    string sysLanguageName = new CultureInfo(GetUserDefaultUILanguage()).Name;
                    foreach(FileInfo fi in di.GetFiles())
                    {
                        string name = Path.GetFileNameWithoutExtension(fi.Name);
                        //如果为空，则赋值为系统显示语言文件名文件（存在时）
                        if(name.Equals(sysLanguageName, StringComparison.OrdinalIgnoreCase))
                        {
                            language = fi.FullName; break;
                        }
                    }
                }
                else if(!File.Exists(language)) language = string.Empty;
                return language;
            }
            set
            {
                ConfigWriter.SetValue("General", "Language", value);
            }
        }

        public static bool AutoBackup
        {
            get => ConfigReader.GetValue("General", "AutoBackup") != "0";
            set => ConfigWriter.SetValue("General", "AutoBackup", (value ? 1 : 0).ToString());
        }

        public static bool ProtectOpenItem
        {
            get => ConfigReader.GetValue("General", "ProtectOpenItem") != "0";
            set => ConfigWriter.SetValue("General", "ProtectOpenItem", (value ? 1 : 0).ToString());
        }

        public static string EngineUrl
        {
            get
            {
                string url = ConfigReader.GetValue("General", "EngineUrl");
                if(url.IsNullOrWhiteSpace()) url = EngineUrls[0];
                return url;
            }
            set
            {
                ConfigWriter.SetValue("General", "EngineUrl", value);
            }
        }

        private static void CreateDirs()
        {
            foreach(string dirPath in new[] { ConfigDir, BackupDir, LangsDir, DicsDir, WebDicsDir, UserDicsDir })
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private static void DeleteOldFiles()
        {
            DirectoryInfo configDi = new DirectoryInfo(ConfigDir);
            foreach(DirectoryInfo di in configDi.GetDirectories())
            {
                bool isOther = true;
                foreach(string path in new[] { BackupDir, LangsDir, DicsDir })
                {
                    if(di.FullName.Equals(path, StringComparison.OrdinalIgnoreCase))
                    {
                        isOther = false;
                        break;
                    }
                }
                if(isOther) Directory.Delete(di.FullName);
            }
            foreach(FileInfo fi in configDi.GetFiles())
            {
                bool isOther = true;
                foreach(string path in new[] { ConfigIni })
                {
                    if(fi.FullName.Equals(path, StringComparison.OrdinalIgnoreCase))
                    {
                        isOther = false;
                        break;
                    }
                }
                if(isOther) File.Delete(fi.FullName);
            }
        }
    }
}