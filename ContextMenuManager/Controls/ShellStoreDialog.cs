using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellStoreDialog : CommonDialog
    {
        public string[] SelectedKeyNames { get; private set; }
        public Func<string, bool> Filter { get; set; }
        public string ShellPath { get; set; }
        public bool IsReference { get; set; }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(ShellStoreForm frm = new ShellStoreForm(this.ShellPath, this.Filter, this.IsReference))
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.SelectedKeyNames = frm.SelectedKeyNames;
                return flag;
            }
        }

        public sealed class ShellStoreForm : Form
        {
            public string ShellPath { get; private set; }
            public Func<string, bool> Filter { get; private set; }
            public string[] SelectedKeyNames { get; private set; }

            public ShellStoreForm(string shellPath, Func<string, bool> filter, bool isReference)
            {
                this.ShellPath = shellPath;
                this.Filter = filter;
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MessageBoxFont;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MinimizeBox = this.MaximizeBox = false;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(652, 425).DpiZoom();
                this.Text = isReference ? AppString.Dialog.CheckReference : AppString.Dialog.CheckCopy;
                btnOk.Click += (sender, e) => GetSelectedItems();
                list.Owner = listBox;
                InitializeComponents();
                LoadItems(isReference);
            }

            readonly MyList list = new MyList();
            readonly MyListBox listBox = new MyListBox();
            readonly Panel pnlBorder = new Panel
            {
                BackColor = Color.FromArgb(200, 200, 200)
            };
            readonly Button btnOk = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                Text = AppString.Dialog.Ok,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel,
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { listBox, pnlBorder, btnOk, btnCancel });
                int a = 20.DpiZoom();
                listBox.Location = new Point(a, a);
                pnlBorder.Location = new Point(a - 1, a - 1);
                btnOk.Top = btnCancel.Top = this.ClientSize.Height - btnCancel.Height - a;
                btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
                btnOk.Left = btnCancel.Left - btnOk.Width - a;
                this.OnResize(null);
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                listBox.Width = ClientSize.Width - 2 * listBox.Left;
                listBox.Height = btnOk.Top - 2 * listBox.Top;
                pnlBorder.Width = listBox.Width + 2;
                pnlBorder.Height = listBox.Height + 2;
            }

            private void LoadItems(bool isReference)
            {
                using(var shellKey = RegistryEx.GetRegistryKey(ShellPath))
                {
                    foreach(string itemName in shellKey.GetSubKeyNames())
                    {
                        if(Filter != null && !Filter(itemName)) continue;
                        string regPath = $@"{ShellPath}\{itemName}";
                        list.AddItem(new StoreShellItem(regPath, isReference));
                    }
                }
            }

            private void GetSelectedItems()
            {
                List<string> names = new List<string>();
                foreach(StoreShellItem item in list.Controls)
                    if(item.IsSelected) names.Add(item.KeyName);
                SelectedKeyNames = names.ToArray();
            }
        }
    }

    sealed class StoreShellItem : ShellItem
    {
        public StoreShellItem(string regPath, bool isPublic, bool isSelect = true) : base(regPath)
        {
            this.IsPublic = isPublic;
            if(isSelect)
            {
                this.AddCtr(chkSelected, 40.DpiZoom());
                this.ContextMenuStrip = null;
                ChkVisible.Visible = BtnShowMenu.Visible = BtnSubItems.Visible = false;
            }
            RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
        }

        public bool IsPublic { get; set; }
        public bool IsSelected => chkSelected.Checked;

        readonly CheckBox chkSelected = new CheckBox { AutoSize = true };

        public override void DeleteMe()
        {
            if(IsPublic && MessageBoxEx.Show(AppString.Message.ConfirmDeleteReferenced,
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            base.DeleteMe();
        }
    }
}