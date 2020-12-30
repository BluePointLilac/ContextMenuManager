using Microsoft.Win32;

namespace BulePointLilac.Methods
{
    public static class FileExtension
    {
        public const string FileExtsPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        public static string GetOpenMode(string extension)
        {
            string mode = Registry.GetValue($@"{FileExtsPath}\{extension}\UserChoice", "ProgId", null)?.ToString();
            if(!string.IsNullOrEmpty(mode)) return mode;
            mode = Registry.GetValue($@"HKEY_CLASSES_ROOT\{extension}", "", null)?.ToString();
            return mode;
        }
    }
}