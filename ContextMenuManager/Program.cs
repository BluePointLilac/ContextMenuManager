using BulePointLilac.Methods;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ContextMenuManager
{
    static class Program
    {
        public const string ZH_CNINI = "zh-cn.ini";
        public const string CONFIGINI = "config.ini";
        public const string GUIDINFOSINI = "GuidInfos.ini";
        public const string ThIRDRULESDICXML = "ThirdRulesDic.xml";
        public const string SHELLCOMMONDICXML = "ShellCommonDic.xml";
        public static readonly string ConfigDir = $@"{Application.StartupPath}\config";
        public static readonly string LanguagesDir = $@"{ConfigDir}\languages";
        public static readonly string AppDataConfigDir = $@"{Environment.ExpandEnvironmentVariables("%AppData%")}\ContextMenuManager\config";
        public static readonly string ConfigIniPath = $@"{AppDataConfigDir}\{CONFIGINI}";
        public static readonly string GuidInfosDicPath = $@"{ConfigDir}\{GUIDINFOSINI}";
        public static readonly string AppDataGuidInfosDicPath = $@"{AppDataConfigDir}\{GUIDINFOSINI}";
        public static readonly string ThirdRulesDicPath = $@"{ConfigDir}\{ThIRDRULESDICXML}";
        public static readonly string AppDataThirdRulesDicPath = $@"{AppDataConfigDir}\{ThIRDRULESDICXML}";
        public static readonly string ShellCommonDicPath = $@"{ConfigDir}\{SHELLCOMMONDICXML}";
        public static readonly string AppDataShellCommonDicPath = $@"{AppDataConfigDir}\{SHELLCOMMONDICXML}";
        private static readonly IniReader ConfigReader = new IniReader(ConfigIniPath);
        public static string LanguageFilePath
        {
            get
            {
                string value = ConfigReader.GetValue("General", "Language");
                DirectoryInfo di = new DirectoryInfo(LanguagesDir);
                if(value.Equals(string.Empty) && di.Exists)
                {
                    foreach(FileInfo fi in di.GetFiles())
                    {
                        if(Path.GetFileNameWithoutExtension(fi.Name).Equals(new CultureInfo(GetUserDefaultUILanguage()).Name, StringComparison.OrdinalIgnoreCase))
                        {
                            value = fi.FullName; break;
                        }
                    }
                }
                return value;
            }
        }
        public static readonly DateTime LastCheckUpdateTime;

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
            if(!DateTime.TryParse(ConfigReader.GetValue("General", "LastCheckUpdateTime"), out LastCheckUpdateTime))
                LastCheckUpdateTime = DateTime.Today.AddMonths(-2);
            if(LastCheckUpdateTime.AddMonths(1).CompareTo(DateTime.Today) < 0)
            {
                Updater.CheckUpdate();
                new IniFileHelper(ConfigIniPath).SetValue("General", "LastCheckUpdateTime", DateTime.Today.ToShortDateString());
            }
        }
    }
}