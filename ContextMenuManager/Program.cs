using BulePointLilac.Methods;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ContextMenuManager
{
    //完美兼容.Net Framework 3.5(Win7)、.Net Framework 4.5(Win8、Win8.1)、.Net Framework 4.6(Win10)
    //避免用户未安装或不会安装系统未自带的.Net Framework版本，这样可以直接启动程序，不用担心框架问题，
    //Win10和Win8、Win8.1可共用一个exe文件(.Net Framework 4.5)，Win7另用一个版本(.Net Framework 3.5)
    //在编译前在Properties\应用程序\目标框架中修改为对应框架即可
    static class Program
    {
        public const string ZH_CNINI = "zh-CN.ini";
        public const string CONFIGINI = "config.ini";
        public const string GUIDINFOSDICINI = "GuidInfosDic.ini";
        public const string ThIRDRULESDICXML = "ThirdRulesDic.xml";
        public const string SHELLCOMMONDICXML = "ShellCommonDic.xml";
        public static readonly string ConfigDir = $@"{Application.StartupPath}\config";
        public static readonly string LanguagesDir = $@"{ConfigDir}\languages";
        public static readonly string AppDataConfigDir = $@"{Environment.ExpandEnvironmentVariables("%AppData%")}\ContextMenuManager\config";
        public static readonly string ConfigIniPath = $@"{AppDataConfigDir}\{CONFIGINI}";
        public static readonly string GuidInfosDicPath = $@"{ConfigDir}\{GUIDINFOSDICINI}";
        public static readonly string AppDataGuidInfosDicPath = $@"{AppDataConfigDir}\{GUIDINFOSDICINI}";
        public static readonly string ThirdRulesDicPath = $@"{ConfigDir}\{ThIRDRULESDICXML}";
        public static readonly string AppDataThirdRulesDicPath = $@"{AppDataConfigDir}\{ThIRDRULESDICXML}";
        public static readonly string ShellCommonDicPath = $@"{ConfigDir}\{SHELLCOMMONDICXML}";
        public static readonly string AppDataShellCommonDicPath = $@"{AppDataConfigDir}\{SHELLCOMMONDICXML}";
        private static readonly IniReader ConfigReader = new IniReader(ConfigIniPath);
        public static readonly DateTime LastCheckUpdateTime;

        private static string languageFilePath = null;
        public static string LanguageFilePath
        {
            get
            {
                if(languageFilePath != null) return languageFilePath;
                languageFilePath = ConfigReader.GetValue("General", "Language");
                DirectoryInfo di = new DirectoryInfo(LanguagesDir);
                if(languageFilePath.Equals(string.Empty) && di.Exists)
                {
                    string sysLanguageName = new CultureInfo(GetUserDefaultUILanguage()).Name;
                    foreach(FileInfo fi in di.GetFiles())
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fi.Name);
                        if(fileName.Equals(sysLanguageName, StringComparison.OrdinalIgnoreCase))
                        {
                            languageFilePath = fi.FullName; break;
                        }
                    }
                }
                else if(!File.Exists(languageFilePath))
                {
                    languageFilePath = string.Empty;
                }
                return languageFilePath;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        [DllImport("kernel32.dll")]
        private static extern ushort GetUserDefaultUILanguage();

        static Program()
        {
            Directory.CreateDirectory(AppDataConfigDir);
            try
            {
                string time = ConfigReader.GetValue("General", "LastCheckUpdateTime");
                //二进制数据时间不会受系统时间格式影响
                LastCheckUpdateTime = DateTime.FromBinary(Convert.ToInt64(time));
            }
            catch
            {
                //将上次检测更新时间推前到两个月前
                LastCheckUpdateTime = DateTime.Today.AddMonths(-2);
            }
            //如果上次检测更新时间为一个月以前就进行更新操作
            if(LastCheckUpdateTime.AddMonths(1).CompareTo(DateTime.Today) < 0)
            {
                Updater.CheckUpdate();
                string time = DateTime.Today.ToBinary().ToString();
                IniFileHelper helper = new IniFileHelper(ConfigIniPath);
                new IniFileHelper(ConfigIniPath).SetValue("General", "LastCheckUpdateTime", time);
            }
        }
    }
}