using BluePointLilac.Methods;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiAdministratorItem
    {
        ContextMenuStrip ContextMenuStrip { get; set; }
        RunAsAdministratorItem TsiAdministrator { get; set; }
        ShellLink ShellLink { get; }
    }

    sealed class RunAsAdministratorItem : ToolStripMenuItem
    {
        public RunAsAdministratorItem(ITsiAdministratorItem item) : base(AppString.Menu.RunAsAdministrator)
        {
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                if(item.ShellLink == null)
                {
                    this.Enabled = false;
                    return;
                }
                string filePath = item.ShellLink.TargetPath;
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
                this.Checked = item.ShellLink.RunAsAdministrator;
            };
            this.Click += (sender, e) =>
            {
                item.ShellLink.RunAsAdministrator = !this.Checked;
                item.ShellLink.Save();
                if(item is WinXItem) ExplorerRestarter.Show();
            };
        }
    }
}