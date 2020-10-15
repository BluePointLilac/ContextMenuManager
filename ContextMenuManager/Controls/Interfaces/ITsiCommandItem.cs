using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    interface ITsiCommandItem
    {
        string ItemCommand { get; set; }
        ChangeCommandMenuItem TsiChangeCommand { get; set; }
    }

    sealed class ChangeCommandMenuItem : ToolStripMenuItem
    {
        public ChangeCommandMenuItem(ITsiCommandItem item) : base(AppString.Menu_ChangeCommand)
        {
            this.Click += (sender, e) =>
            {
                string command = ChangeCommand(item.ItemCommand);
                if(command != null) item.ItemCommand = command;
            };
        }

        public static string ChangeCommand(string command)
        {
            using(InputDialog dlg = new InputDialog { Text = command, Title = AppString.Menu_ChangeCommand })
            {
                if(dlg.ShowDialog() != DialogResult.OK) return null;
                if(string.IsNullOrEmpty(dlg.Text))
                {
                    MessageBoxEx.Show(AppString.MessageBox_CommandCannotBeEmpty);
                    return ChangeCommand(command);
                }
                else return dlg.Text;
            }
        }
    }
}