using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ContextMenuManager.Controls
{
    sealed class WinXList : MyList
    {
        public static readonly string WinXPath = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Windows\WinX");
        private static readonly Dictionary<string, IniReader> DesktopIniReaders = new Dictionary<string, IniReader>();

        public static string GetMenuName(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            if(DesktopIniReaders.TryGetValue(dirPath, out IniReader reader))
            {
                string name = reader.GetValue("LocalizedFileNames", fileName);
                name = ResourceString.GetDirectString(name);
                return name;
            }
            else return string.Empty;
        }

        public void LoadItems()
        {
            this.ClearItems();
            DesktopIniReaders.Clear();
            Array.ForEach(new DirectoryInfo(WinXPath).GetDirectories(), di => LoadSubDirItems(di));
        }

        private void LoadSubDirItems(DirectoryInfo di)
        {
            GroupPathItem groupItem = new GroupPathItem
            {
                TargetPath = di.FullName,
                Text = Path.GetFileNameWithoutExtension(di.FullName),
                Image = ResourceIcon.GetFolderIcon(di.FullName).ToBitmap(),
                PathType = ObjectPath.PathType.Directory
            };
            this.AddItem(groupItem);
            string iniPath = $@"{di.FullName}\desktop.ini";
            DesktopIniReaders.Add(di.FullName, new IniReader(iniPath));
            Array.ForEach(di.GetFiles(), fi =>
            {
                if(fi.Extension.ToLower() == ".lnk") this.AddItem(new WinXItem(fi.FullName, groupItem));
            });
        }
    }
}