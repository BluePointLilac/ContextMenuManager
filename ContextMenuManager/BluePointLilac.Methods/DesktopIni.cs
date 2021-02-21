using System.IO;

namespace BluePointLilac.Methods
{
    public static class DesktopIni
    {
        private const string LocalizedFileNames = "LocalizedFileNames";

        private static string GetIniPath(string filePath) => $@"{Path.GetDirectoryName(filePath)}\desktop.ini";

        public static void DeleteLocalizedFileNames(string filePath)
        {
            IniWriter writer = new IniWriter(GetIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            writer.DeleteKey(LocalizedFileNames, fileName);
        }

        public static void SetLocalizedFileNames(string filePath, string name)
        {
            IniWriter writer = new IniWriter(GetIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            writer.SetValue(LocalizedFileNames, fileName, name);
        }

        public static string GetLocalizedFileNames(string filePath, bool translate = false)
        {
            IniWriter writer = new IniWriter(GetIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            string name = writer.GetValue(LocalizedFileNames, fileName);
            if(translate) name = ResourceString.GetDirectString(name);
            return name;
        }
    }
}