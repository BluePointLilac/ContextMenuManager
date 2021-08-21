using BluePointLilac.Methods;
using System.IO;

namespace ContextMenuManager.Methods
{
    static class DesktopIni
    {
        private const string LocalizedFileNames = "LocalizedFileNames";

        public static string GetDesktopIniPath(string filePath) => $@"{Path.GetDirectoryName(filePath)}\desktop.ini";

        public static void DeleteLocalizedFileNames(string filePath)
        {
            IniWriter writer = new IniWriter(GetDesktopIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            writer.DeleteKey(LocalizedFileNames, fileName);
        }

        public static void SetLocalizedFileNames(string filePath, string name)
        {
            IniWriter writer = new IniWriter(GetDesktopIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            writer.SetValue(LocalizedFileNames, fileName, name);
        }

        public static string GetLocalizedFileNames(string filePath, bool translate = false)
        {
            IniWriter writer = new IniWriter(GetDesktopIniPath(filePath));
            string fileName = Path.GetFileName(filePath);
            string name = writer.GetValue(LocalizedFileNames, fileName);
            if(translate) name = ResourceString.GetDirectString(name);
            return name;
        }
    }
}