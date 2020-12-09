using BulePointLilac.Methods;
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
        string ItemText { get; }
        string RegPath { get; }
    }

    sealed class DeleteMeMenuItem : ToolStripMenuItem
    {
        public DeleteMeMenuItem(ITsiDeleteItem item) : base(AppString.Menu.Delete)
        {
            this.Click += (sender, e) =>
            {
                if(MessageBoxEx.Show(AppString.MessageBox.ConfirmDeletePermanently,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if(item is ITsiRegDeleteItem regItem && AppConfig.AutoBackup)
                    {
                        string date = DateTime.Today.ToString("yyyy-MM-dd");
                        string fileName = ObjectPath.RemoveIllegalChars(regItem.ItemText);
                        string filePath = $@"{AppConfig.BackupDir}\{date}\{fileName}.reg";
                        filePath = ObjectPath.GetNewPathWithIndex(filePath, ObjectPath.PathType.File);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        RegistryEx.Export(regItem.RegPath, filePath);
                    }
                    item.DeleteMe();
                }
            };
        }
    }
}