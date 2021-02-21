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
                    if(MessageBoxEx.Show(AppString.MessageBox.DeleteButCanRestore,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    string date = DateTime.Today.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH.mm.ss");
                    string filePath = $@"{AppConfig.BackupDir}\{date}\{regItem.Text}-{time}.reg";
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    RegistryEx.Export(regItem.RegPath, filePath);
                }
                else if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDeletePermanently,
                     MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                item.DeleteMe();
            };
        }
    }
}