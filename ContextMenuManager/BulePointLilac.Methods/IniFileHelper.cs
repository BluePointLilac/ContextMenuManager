using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BulePointLilac.Methods
{
    public class IniFileHelper
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, BestFitMapping = false, SetLastError = false, ThrowOnUnmappableChar = true)]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, BestFitMapping = false, SetLastError = false, ThrowOnUnmappableChar = true)]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        public IniFileHelper(string filePath) { this.FilePath = filePath; }

        public string FilePath { get; private set; }

        public string GetValue(string section, string key)
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, this.FilePath);
            return retVal.ToString();
        }

        public void SetValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.FilePath);
        }

        public void DeleteKey(string section, string key)
        {
            SetValue(section, key, null);
        }

        public void DeleteSection(string section)
        {
            SetValue(section, null, null);
        }

        public bool KeyExists(string section, string key)
        {
            return GetValue(section, key) != string.Empty;
        }

        public bool TryGetValue(string section, string key, out string value)
        {
            value = GetValue(section, key);
            return value != string.Empty;
        }
    }

    public static class DesktopIniHelper
    {
        public const string LocalizedFileNames = "LocalizedFileNames";

        public static void SetLocalizedFileName(string filePath, string name)
        {
            string fileName = Path.GetFileName(filePath);
            string iniPath = $@"{Path.GetDirectoryName(filePath)}\desktop.ini";
            IniFileHelper helper = new IniFileHelper(iniPath);
            helper.SetValue(LocalizedFileNames, fileName, name);
        }

        public static string GetLocalizedFileName(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string iniPath = $@"{Path.GetDirectoryName(filePath)}\desktop.ini";
            IniFileHelper helper = new IniFileHelper(iniPath);
            return helper.GetValue(LocalizedFileNames, fileName);
        }

        public static void DeleteLocalizedFileName(string filePath)
        {
            SetLocalizedFileName(filePath, null);
        }
    }
}