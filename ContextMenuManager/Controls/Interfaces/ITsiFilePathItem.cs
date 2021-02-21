using BluePointLilac.Methods;
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
                this.Visible = File.Exists(item.ItemFilePath) || Directory.Exists(item.ItemFilePath);
            this.Click += (sender, e) => ExternalProgram.JumpExplorer(item.ItemFilePath);
        }
    }

    sealed class FilePropertiesMenuItem : ToolStripMenuItem
    {
        public FilePropertiesMenuItem(ITsiFilePathItem item) : base(AppString.Menu.FileProperties)
        {
            item.ContextMenuStrip.Opening += (sender, e)
                => this.Visible = File.Exists(item.ItemFilePath) || Directory.Exists(item.ItemFilePath);
            this.Click += (sender, e) => ExternalProgram.ShowPropertiesDialog(item.ItemFilePath);
        }
    }
}