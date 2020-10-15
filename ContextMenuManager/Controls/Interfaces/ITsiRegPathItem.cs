using BulePointLilac.Methods;
using System.Windows.Forms;
using static BulePointLilac.Methods.ObjectPath;

namespace ContextMenuManager.Controls
{
    interface ITsiRegPathItem
    {
        string RegPath { get; set; }
        ContextMenuStrip ContextMenuStrip { get; set; }
        RegLocationMenuItem TsiRegLocation { get; set; }
    }

    sealed class RegLocationMenuItem : ToolStripMenuItem
    {

        public RegLocationMenuItem(ITsiRegPathItem item) : base(AppString.Menu_RegistryLocation)
        {
            this.Click += (sender, e) => ShowPath(item.RegPath, PathType.Registry);
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                using(var key = RegistryEx.GetRegistryKey(item.RegPath))
                    this.Visible = key != null;
            };
        }
    }
}