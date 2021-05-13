using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiTextItem
    {
        string Text { get; set; }
        string ItemText { get; set; }
        ChangeTextMenuItem TsiChangeText { get; set; }
    }

    sealed class ChangeTextMenuItem : ToolStripMenuItem
    {
        public ChangeTextMenuItem(ITsiTextItem item) : base(AppString.Menu.ChangeText)
        {
            this.Click += (sender, e) =>
            {
                string name = ChangeText(item.Text);
                if(name != null) item.ItemText = name;
            };
            if(item is IFoldGroupItem == false)
            {
                ((MyListItem)item).TextDoubleClick += (sender, e) =>
                {
                    if(this.Enabled) this.OnClick(null);
                };
            }
        }

        private string ChangeText(string text)
        {
            using(InputDialog dlg = new InputDialog { Text = text, Title = AppString.Menu.ChangeText })
            {
                if(dlg.ShowDialog() != DialogResult.OK) return null;
                if(dlg.Text.Length == 0)
                {
                    MessageBoxEx.Show(AppString.Message.TextCannotBeEmpty);
                    return ChangeText(text);
                }
                else if(ResourceString.GetDirectString(dlg.Text).Length == 0)
                {
                    MessageBoxEx.Show(AppString.Message.StringParsingFailed);
                    return ChangeText(text);
                }
                else return dlg.Text;
            }
        }
    }
}
