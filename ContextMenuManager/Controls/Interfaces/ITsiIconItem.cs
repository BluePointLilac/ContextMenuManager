using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
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
                using(IconDialog dlg = new IconDialog())
                {
                    dlg.IconPath = item.IconPath;
                    dlg.IconIndex = item.IconIndex;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    using(Icon icon = ResourceIcon.GetIcon(dlg.IconPath, dlg.IconIndex))
                    {
                        Image image = icon?.ToBitmap();
                        if(image == null) return;
                        item.Image = image;
                        item.IconPath = dlg.IconPath;
                        item.IconIndex = dlg.IconIndex;
                        item.IconLocation = $"{dlg.IconPath},{dlg.IconIndex}";
                    }
                }
            };
            MyListItem listItem = (MyListItem)item;
            listItem.Disposed += (sender, e) => item.ItemIcon?.Dispose();
            listItem.ImageDoubleClick += () =>
            {
                if(listItem.FindForm() is ShellStoreDialog.ShellStoreForm) return;
                if(this.Enabled) this.OnClick(null);
            };
        }
    }
}