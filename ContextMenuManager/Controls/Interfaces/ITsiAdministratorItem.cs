using BluePointLilac.Methods;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiAdministratorItem
    {
        ContextMenuStrip ContextMenuStrip { get; set; }
        RunAsAdministratorItem TsiAdministrator { get; set; }
        WshShortcut Shortcut { get; }
    }

    sealed class RunAsAdministratorItem : ToolStripMenuItem
    {
        public RunAsAdministratorItem(ITsiAdministratorItem item) : base(AppString.Menu.RunAsAdministrator)
        {
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                if(item.Shortcut == null)
                {
                    this.Enabled = false;
                    return;
                }
                string filePath = item.Shortcut.TargetPath;
                string extension = Path.GetExtension(filePath)?.ToLower();
                switch(extension)
                {
                    case ".exe":
                    case ".bat":
                    case ".cmd":
                        this.Enabled = true;
                        break;
                    default:
                        this.Enabled = false;
                        break;
                }
                this.Checked = item.Shortcut.RunAsAdministrator;
            };
            this.Click += (sender, e) =>
            {
                item.Shortcut.RunAsAdministrator = !this.Checked;
            };
        }
    }
}