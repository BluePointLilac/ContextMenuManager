using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellStoreDialog : CommonDialog
    {
        public List<string> SelectedKeyNames { get; private set; }
        public List<string> IgnoredKeyNames { get; set; }
        public string ShellPath { get; set; }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(ShellStoreForm frm = new ShellStoreForm(this.ShellPath, this.IgnoredKeyNames))
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.SelectedKeyNames = frm.SelectedKeyNames;
                return flag;
            }
        }

        sealed class ShellStoreForm : Form
        {
            public string ShellPath { get; private set; }
            public List<string> IgnoredKeyNames { get; private set; }
            public List<string> SelectedKeyNames { get; private set; } = new List<string>();
            private bool IsPublic => ShellPath.Equals(ShellItem.CommandStorePath, StringComparison.OrdinalIgnoreCase);

            public ShellStoreForm(string shellPath, List<string> ignoredKeyNames)
            {
                this.ShellPath = shellPath;
                this.IgnoredKeyNames = ignoredKeyNames;
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MessageBoxFont;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(652, 425).DpiZoom();
                this.Text = IsPublic ? AppString.Dialog.CheckReference : AppString.Dialog.CheckCopy;
                btnOk.Click += (sender, e) => GetSelectedItems();
                list.Owner = listBox;
                InitializeComponents();
                LoadItems();
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

            private void LoadItems()
            {
                using(var shellKey = RegistryEx.GetRegistryKey(ShellPath))
                {
                    Array.ForEach(Array.FindAll(shellKey.GetSubKeyNames(), itemName =>
                        !IgnoredKeyNames.Contains(itemName, StringComparer.OrdinalIgnoreCase)), itemName =>
                        {
                            string regPath = $@"{ShellPath}\{itemName}";
                            list.AddItem(new StoreShellItem(regPath, IsPublic));
                        });
                }
            }

            private void GetSelectedItems()
            {
                foreach(StoreShellItem item in list.Controls)
                    if(item.IsSelected) SelectedKeyNames.Add(item.KeyName);
            }

            sealed class StoreShellItem : ShellItem
            {
                public StoreShellItem(string regPath, bool isPublic) : base(regPath)
                {
                    this.IsPublic = isPublic;
                    this.AddCtr(chkSelected);
                    ChkVisible.Visible = BtnSubItems.Visible = false;
                    RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
                }
                public bool IsSelected => chkSelected.Checked;
                public bool IsPublic { get; set; }
                readonly CheckBox chkSelected = new CheckBox { AutoSize = true };

                public override void DeleteMe()
                {
                    if(IsPublic && MessageBoxEx.Show(AppString.MessageBox.ConfirmDeleteReferenced,
                        MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    base.DeleteMe();
                }
            }
        }
    }
}