using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class NewIEDialog : CommonDialog
    {
        public string RegPath { get; private set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(NewIEForm frm = new NewIEForm())
            {
                frm.TopMost = AppConfig.TopMost;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.RegPath = frm.RegPath;
                return flag;
            }
        }

        sealed class NewIEForm : NewItemForm
        {
            public string RegPath { get; set; }

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                btnOK.Click += (sender, e) =>
                {
                    if(ItemText.IsNullOrWhiteSpace())
                    {
                        AppMessageBox.Show(AppString.Message.TextCannotBeEmpty);
                        return;
                    }
                    if(ItemCommand.IsNullOrWhiteSpace())
                    {
                        AppMessageBox.Show(AppString.Message.CommandCannotBeEmpty);
                        return;
                    }
                    AddNewItem();
                    DialogResult = DialogResult.OK;
                };

                btnBrowse.Click += (sender, e) =>
                {
                    using(OpenFileDialog dlg = new OpenFileDialog())
                    {
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        this.ItemFilePath = dlg.FileName;
                        this.ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                };
            }

            private void AddNewItem()
            {
                this.RegPath = $@"{IEList.IEPath}\{IEItem.MeParts[0]}\{ItemText.Replace("\\", "")}";
                Microsoft.Win32.Registry.SetValue(RegPath, "", ItemCommand);
            }
        }
    }
}