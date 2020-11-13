using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class AddGuidDicDialog : CommonDialog
    {
        public Image ItemIcon { get; set; }
        public string ItemName { get; set; }
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
                frm.ItemName = this.ItemName;
                frm.ItemIcon = this.ItemIcon;
                frm.ItemIconPath = this.ItemIconPath;
                frm.ItemIconIndex = this.ItemIconIndex;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag)
                {
                    this.ItemName = frm.ItemName;
                    this.ItemIcon = frm.ItemIcon;
                    this.ItemIconPath = frm.ItemIconPath;
                    this.ItemIconIndex = frm.ItemIconIndex;
                }
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

            public string ItemName
            {
                get=> txtName.Text;
                set => txtName.Text = value;
            }
            public string ItemIconPath { get; set; }
            public int ItemIconIndex { get; set; }
            public Image ItemIcon {
                get => picIcon.Image;
                set => picIcon.Image = value;
            }

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
                Size = SystemInformation.IconSize,
                BackColor = Color.White
            };
            readonly Button btnIcon = new Button
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

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { lblName, txtName, lblIcon, picIcon, btnIcon, btnOk, btnCancel });
                int a = 20.DpiZoom();
                lblName.Left = lblName.Top = lblIcon.Left = txtName.Top = a;
                txtName.Left = lblName.Right + a;
                lblIcon.Top = picIcon.Top = btnIcon.Top = lblName.Bottom + a;
                btnOk.Left = picIcon.Left = lblIcon.Right + a;
                btnOk.Top = btnCancel.Top = picIcon.Bottom + a;
                btnIcon.Left = btnCancel.Left = btnOk.Right + a;
                txtName.Width = btnCancel.Right - txtName.Left;
                this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);

                btnIcon.Click += (sender, e) => SelectIcon();
            }

            private void SelectIcon()
            {
                using(IconDialog dlg = new IconDialog())
                {
                    dlg.IconPath = this.ItemIconPath;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemIconPath = dlg.IconPath;
                    ItemIconIndex = dlg.IconIndex;
                    picIcon.Image = ResourceIcon.GetIcon(ItemIconPath, ItemIconIndex).ToBitmap();
                }
            }
        }
    }
}