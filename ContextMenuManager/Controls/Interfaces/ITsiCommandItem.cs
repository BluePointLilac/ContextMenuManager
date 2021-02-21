using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiCommandItem
    {
        string ItemCommand { get; set; }
        ChangeCommandMenuItem TsiChangeCommand { get; set; }
    }

    sealed class ChangeCommandMenuItem : ToolStripMenuItem
    {
        public bool CommandCanBeEmpty { get; set; }

        public ChangeCommandMenuItem(ITsiCommandItem item) : base(AppString.Menu.ChangeCommand)
        {
            this.Click += (sender, e) =>
            {
                string command = ChangeCommand(item.ItemCommand);
                if(command != null) item.ItemCommand = command;
            };
        }

        private string ChangeCommand(string command)
        {
            using(InputDialog dlg = new InputDialog { Text = command, Title = AppString.Menu.ChangeCommand })
            {
                if(dlg.ShowDialog() != DialogResult.OK) return null;
                if(!CommandCanBeEmpty && string.IsNullOrEmpty(dlg.Text))
                {
                    MessageBoxEx.Show(AppString.MessageBox.CommandCannotBeEmpty);
                    return ChangeCommand(command);
                }
                else return dlg.Text;
            }
        }
    }
}