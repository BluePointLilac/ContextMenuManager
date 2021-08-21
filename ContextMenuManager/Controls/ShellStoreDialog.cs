using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
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
                frm.TopMost = AppConfig.TopMost;
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
                this.SuspendLayout();
                this.Filter = filter;
                this.ShellPath = shellPath;
                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MessageBoxFont;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MinimizeBox = this.MaximizeBox = false;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(652, 422).DpiZoom();
                this.Text = isReference ? AppString.Dialog.CheckReference : AppString.Dialog.CheckCopy;
                btnOK.Click += (sender, e) => GetSelectedItems();
                chkSelectAll.Click += (sender, e) =>
                {
                    bool flag = chkSelectAll.Checked;
                    foreach(StoreShellItem item in list.Controls)
                    {
                        item.IsSelected = flag;
                    }
                };
                list.Owner = listBox;
                InitializeComponents();
                LoadItems(isReference);
                this.ResumeLayout();
            }

            readonly MyList list = new MyList();
            readonly MyListBox listBox = new MyListBox();
            readonly Panel pnlBorder = new Panel
            {
                BackColor = Color.FromArgb(200, 200, 200)
            };
            readonly Button btnOK = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                Text = ResourceString.OK,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel,
                Text = ResourceString.Cancel,
                AutoSize = true
            };
            readonly CheckBox chkSelectAll = new CheckBox
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Text = AppString.Dialog.SelectAll,
                Cursor = Cursors.Hand,
                AutoSize = true
            };

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { listBox, pnlBorder, btnOK, btnCancel, chkSelectAll });
                int a = 20.DpiZoom();
                listBox.Location = new Point(a, a);
                pnlBorder.Location = new Point(a - 1, a - 1);
                chkSelectAll.Top = btnOK.Top = btnCancel.Top = this.ClientSize.Height - btnCancel.Height - a;
                btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
                btnOK.Left = btnCancel.Left - btnOK.Width - a;
                chkSelectAll.Left = a;
                this.OnResize(null);
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                listBox.Width = ClientSize.Width - 2 * listBox.Left;
                listBox.Height = btnOK.Top - 2 * listBox.Top;
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
                        StoreShellItem item = new StoreShellItem(regPath, isReference);
                        item.SelectedChanged += () =>
                        {
                            foreach(StoreShellItem shellItem in list.Controls)
                            {
                                if(!shellItem.IsSelected)
                                {
                                    chkSelectAll.Checked = false;
                                    return;
                                }
                            }
                            chkSelectAll.Checked = true;
                        };
                        list.AddItem(item);
                    }
                }
            }

            private void GetSelectedItems()
            {
                List<string> names = new List<string>();
                foreach(StoreShellItem item in list.Controls)
                    if(item.IsSelected) names.Add(item.KeyName);
                this.SelectedKeyNames = names.ToArray();
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
                this.ContextMenuStrip = null;
                this.AddCtr(chkSelected);
                ChkVisible.Visible = BtnShowMenu.Visible = BtnSubItems.Visible = false;
                this.MouseClick += (sender, e) => chkSelected.Checked = !chkSelected.Checked;
                chkSelected.CheckedChanged += (sender, e) => SelectedChanged?.Invoke();
            }
            RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
        }

        public bool IsPublic { get; set; }
        public bool IsSelected
        {
            get => chkSelected.Checked;
            set => chkSelected.Checked = value;
        }

        readonly CheckBox chkSelected = new CheckBox
        {
            Cursor = Cursors.Hand,
            AutoSize = true
        };

        public Action SelectedChanged;

        public override void DeleteMe()
        {
            if(IsPublic && AppMessageBox.Show(AppString.Message.ConfirmDeleteReferenced,
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            base.DeleteMe();
        }
    }
}