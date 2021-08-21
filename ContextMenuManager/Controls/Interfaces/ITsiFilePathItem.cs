using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System.IO;
using System.Windows.Forms;

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
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                this.Visible = item.ItemFilePath != null;
            };
            this.Click += (sender, e) => ExternalProgram.JumpExplorer(item.ItemFilePath, AppConfig.OpenMoreExplorer);
        }
    }

    sealed class FilePropertiesMenuItem : ToolStripMenuItem
    {
        public FilePropertiesMenuItem(ITsiFilePathItem item) : base(AppString.Menu.FileProperties)
        {
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                string path = item.ItemFilePath;
                this.Visible = Directory.Exists(path) || File.Exists(path);
            };
            this.Click += (sender, e) => ExternalProgram.ShowPropertiesDialog(item.ItemFilePath);
        }
    }
}