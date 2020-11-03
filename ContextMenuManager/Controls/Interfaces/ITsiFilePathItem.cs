using BulePointLilac.Methods;
using System.IO;
using System.Windows.Forms;
using static BulePointLilac.Methods.ObjectPath;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiFilePathItem
    {
        string ItemFilePath { get; }
        ContextMenuStrip ContextMenuStrip { get; set; }
        FileLocationMenuItem TsiFileLocation { get; set; }
        FilePropertiesMenuItem TsiFileProperties { get; set; }
    }

    sealed class FileLocationMenuItem : ToolStripMenuItem
    {
        public FileLocationMenuItem(ITsiFilePathItem item) : base(AppString.Menu.FileLocation)
        {
            bool FileExists() => File.Exists(item.ItemFilePath);
            bool DirExists() => Directory.Exists(item.ItemFilePath);
            item.ContextMenuStrip.Opening += (sender, e)
                => this.Visible = FileExists() || DirExists();
            this.Click += (sender, e) =>
            {
                if(FileExists()) ShowPath(item.ItemFilePath, PathType.File);
                else if(DirExists()) ShowPath(item.ItemFilePath, PathType.Directory);
            };
        }
    }

    sealed class FilePropertiesMenuItem : ToolStripMenuItem
    {
        public FilePropertiesMenuItem(ITsiFilePathItem item) : base(AppString.Menu.FileProperties)
        {
            item.ContextMenuStrip.Opening += (sender, e)
                => this.Visible = File.Exists(item.ItemFilePath) || Directory.Exists(item.ItemFilePath);
            this.Click += (sender, e) => PropertiesDialog.Show(item.ItemFilePath);
        }
    }
}