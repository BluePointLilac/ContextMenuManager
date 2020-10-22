using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    interface ITsiIconItem
    {
        ChangeIconMenuItem TsiChangeIcon { get; set; }
        string IconLocation { get; set; }
        string IconPath { get; set; }
        int IconIndex { get; set; }
        Image Image { get; set; }
        Icon ItemIcon { get; }
    }

    sealed class ChangeIconMenuItem : ToolStripMenuItem
    {
        public ChangeIconMenuItem(ITsiIconItem item) : base(AppString.Menu.ChangeIcon)
        {
            this.Click += (sender, e) =>
            {
                string iconPath = item.IconPath;
                int iconIndex = item.IconIndex;
                using(Icon icon = ChangeIcon(ref iconPath, ref iconIndex))
                {
                    if(icon == null) return;
                    item.IconPath = iconPath;
                    item.IconIndex = iconIndex;
                    item.IconLocation = $"{iconPath},{iconIndex}";
                    item.Image = icon.ToBitmap();
                }
            };
        }

        public static Icon ChangeIcon(ref string iconPath, ref int iconIndex)
        {
            using(IconDialog dlg = new IconDialog { IconPath = iconPath, IconIndex = iconIndex })
            {
                if(dlg.ShowDialog() != DialogResult.OK) return null;
                iconPath = dlg.IconPath;
                iconIndex = dlg.IconIndex;
            }
            return ResourceIcon.GetIcon(iconPath, iconIndex);
        }
    }
}