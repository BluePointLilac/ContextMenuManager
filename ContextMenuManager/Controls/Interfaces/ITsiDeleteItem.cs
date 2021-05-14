using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiDeleteItem
    {
        DeleteMeMenuItem TsiDeleteMe { get; set; }
        void DeleteMe();
    }

    interface ITsiRegDeleteItem : ITsiDeleteItem
    {
        string Text { get; }
        string RegPath { get; }
    }

    sealed class DeleteMeMenuItem : ToolStripMenuItem
    {
        public DeleteMeMenuItem(ITsiDeleteItem item) : base(AppString.Menu.Delete)
        {
            this.Click += (sender, e) =>
            {
                if(item is ITsiRegDeleteItem regItem && AppConfig.AutoBackup)
                {
                    if(MessageBoxEx.Show(AppString.Message.DeleteButCanRestore,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    string date = DateTime.Today.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH.mm.ss");
                    string filePath = $@"{AppConfig.BackupDir}\{date}\{regItem.Text} - {time}.reg";
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    RegistryEx.Export(regItem.RegPath, filePath);
                }
                else if(MessageBoxEx.Show(AppString.Message.ConfirmDeletePermanently,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                MyListItem listItem = (MyListItem)item;
                MyList list = (MyList)listItem.Parent;
                int index = list.GetItemIndex(listItem);
                item.DeleteMe();
                list.HoveredItem = (MyListItem)list.Controls[index - 1];
            };
        }
    }
}