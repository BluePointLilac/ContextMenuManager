using BulePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiRegExportItem
    {
        string Text { get; set; }
        string RegPath { get; set; }
        ContextMenuStrip ContextMenuStrip { get; set; }
        RegExportMenuItem TsiRegExport { get; set; }
    }

    sealed class RegExportMenuItem : ToolStripMenuItem
    {
        public RegExportMenuItem(ITsiRegExportItem item) : base(AppString.Menu.ExportRegistry)
        {
            this.Click += (sender, e) =>
            {
                using(SaveFileDialog dlg = new SaveFileDialog())
                {
                    string dirPath = $@"{AppConfig.BackupDir}\{DateTime.Today.ToString("yyyy-MM-dd")}";
                    Directory.CreateDirectory(dirPath);
                    dlg.FileName = item.Text;
                    dlg.InitialDirectory = dirPath;
                    dlg.Filter = $"{AppString.Dialog.RegistryFile}|*.reg";
                    if(dlg.ShowDialog() != DialogResult.OK)
                    {
                        if(Directory.GetFiles(dirPath).Length == 0 && Directory.GetDirectories(dirPath).Length == 0)
                            Directory.Delete(dirPath);
                    }
                    else
                    {
                        RegistryEx.Export(item.RegPath, dlg.FileName);
                    }
                }
            };
        }
    }
}