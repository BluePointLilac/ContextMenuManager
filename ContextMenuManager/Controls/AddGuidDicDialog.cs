using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class AddGuidDicDialog : CommonDialog
    {
        public Image ItemIcon { get; set; }
        public string ItemText { get; set; }
        public bool IsDelete { get; private set; }
        public string ItemIconPath { get; set; }
        public int ItemIconIndex { get; set; }
        public string ItemIconLocation
        {
            get
            {
                if(ItemIconPath == null) return null;
                return $"{ItemIconPath},{ItemIconIndex}";
            }
        }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(AddGuidDicForm frm = new AddGuidDicForm())
            {
                frm.ItemText = this.ItemText;
                frm.ItemIcon = this.ItemIcon;
                frm.ItemIconPath = this.ItemIconPath;
                frm.ItemIconIndex = this.ItemIconIndex;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag)
                {
                    this.ItemText = frm.ItemText;
                    this.ItemIcon = frm.ItemIcon;
                    this.ItemIconPath = frm.ItemIconPath;
                    this.ItemIconIndex = frm.ItemIconIndex;
                }
                this.IsDelete = frm.IsDelete;
                return flag;
            }
        }

        sealed class AddGuidDicForm : Form
        {
            public AddGuidDicForm()
            {
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MenuFont;
                this.Text = AppString.Dialog.AddGuidDic;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                InitializeComponents();
            }

            public string ItemText
            {
                get => txtName.Text;
                set => txtName.Text = value;
            }
            public Image ItemIcon
            {
                get => picIcon.Image;
                set => picIcon.Image = value;
            }
            public string ItemIconPath { get; set; }
            public int ItemIconIndex { get; set; }
            public bool IsDelete { get; private set; }

            readonly TextBox txtName = new TextBox();
            readonly Label lblName = new Label
            {
                Text = AppString.Dialog.ItemName,
                AutoSize = true
            };
            readonly Label lblIcon = new Label
            {
                Text = AppString.Dialog.ItemIcon,
                AutoSize = true
            };
            readonly PictureBox picIcon = new PictureBox
            {
                Size = SystemInformation.IconSize
            };
            readonly Button btnBrowse = new Button
            {
                Text = AppString.Dialog.Browse,
                AutoSize = true
            };
            readonly Button btnOk = new Button
            {
                Text = AppString.Dialog.Ok,
                DialogResult = DialogResult.OK,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                Text = AppString.Dialog.Cancel,
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };
            readonly Button btnDelete = new Button
            {
                Text = AppString.Dialog.DeleteGuidDic,
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { lblName, txtName, lblIcon, picIcon, btnBrowse, btnDelete, btnOk, btnCancel });
                int a = 20.DpiZoom();
                lblName.Left = lblName.Top = lblIcon.Left = btnDelete.Left = txtName.Top = a;
                txtName.Left = lblName.Right + a;
                btnOk.Left = btnDelete.Right + a;
                btnCancel.Left = btnOk.Right + a;
                txtName.Width = btnCancel.Right - txtName.Left;
                btnBrowse.Left = btnCancel.Right - btnBrowse.Width;
                picIcon.Left = btnOk.Left + (btnOk.Width - picIcon.Width) / 2;
                btnBrowse.Top = txtName.Bottom + a;
                picIcon.Top = btnBrowse.Top + (btnBrowse.Height - picIcon.Height) / 2;
                lblIcon.Top = btnBrowse.Top + (btnBrowse.Height - lblIcon.Height) / 2;
                btnDelete.Top = btnOk.Top = btnCancel.Top = btnBrowse.Bottom + a;
                this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
                MyToolTip.SetToolTip(btnDelete, AppString.Tip.DeleteGuidDic);
                btnBrowse.Click += (sender, e) => SelectIcon();
                btnDelete.Click += (sender, e) => this.IsDelete = true;
            }

            private void SelectIcon()
            {
                using(IconDialog dlg = new IconDialog())
                {
                    dlg.IconPath = this.ItemIconPath;
                    dlg.IconIndex = this.ItemIconIndex;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemIconPath = dlg.IconPath;
                    ItemIconIndex = dlg.IconIndex;
                    picIcon.Image = ResourceIcon.GetIcon(ItemIconPath, ItemIconIndex).ToBitmap();
                }
            }
        }
    }
}