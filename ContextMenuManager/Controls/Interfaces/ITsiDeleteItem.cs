using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
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
                    if(AppMessageBox.Show(AppString.Message.DeleteButCanRestore,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    string date = DateTime.Today.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH.mm.ss");
                    string filePath = $@"{AppConfig.BackupDir}\{date}\{regItem.Text} - {time}.reg";
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    ExternalProgram.ExportRegistry(regItem.RegPath, filePath);
                }
                else if(AppMessageBox.Show(AppString.Message.ConfirmDeletePermanently,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                MyListItem listItem = (MyListItem)item;
                MyList list = (MyList)listItem.Parent;
                int index = list.GetItemIndex(listItem);
                if(index == list.Controls.Count - 1) index--;
                try
                {
                    item.DeleteMe();
                }
                catch
                {
                    AppMessageBox.Show(AppString.Message.AuthorityProtection);
                    return;
                }
                list.Controls.Remove(listItem);
                list.Controls[index].Focus();
                listItem.Dispose();
            };
        }
    }
}