using BluePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiRegPathItem
    {
        string RegPath { get; }
        string ValueName { get; }
        ContextMenuStrip ContextMenuStrip { get; set; }
        RegLocationMenuItem TsiRegLocation { get; set; }
    }

    sealed class RegLocationMenuItem : ToolStripMenuItem
    {
        public RegLocationMenuItem(ITsiRegPathItem item) : base(AppString.Menu.RegistryLocation)
        {
            this.Click += (sender, e) => ExternalProgram.JumpRegEdit(item.RegPath, item.ValueName, AppConfig.OpenMoreRegedit);
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                using(var key = RegistryEx.GetRegistryKey(item.RegPath))
                    this.Visible = key != null;
            };
        }
    }
}