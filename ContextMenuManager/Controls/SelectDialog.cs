using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class SelectDialog : CommonDialog
    {
        public string Title { get; set; }
        public string Selected { get; set; }
        public int SelectedIndex { get; set; }
        public string[] Items { get; set; }
        public bool CanEdit { get; set; }

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(SelectForm frm = new SelectForm())
            {
                frm.Text = this.Title;
                frm.Items = this.Items;
                if(this.Selected != null) frm.Selected = this.Selected;
                else frm.SelectedIndex = this.SelectedIndex;
                frm.CanEdit = this.CanEdit;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag)
                {
                    this.Selected = frm.Selected;
                    this.SelectedIndex = frm.SelectedIndex;
                }
                return flag;
            }
        }

        sealed class SelectForm : Form
        {
            public SelectForm()
            {
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MenuFont;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
                this.InitializeComponents();
            }

            public string Selected
            {
                get => cmbItems.Text;
                set => cmbItems.Text = value;
            }

            public string[] Items
            {
                get
                {
                    string[] value = new string[cmbItems.Items.Count];
                    cmbItems.Items.CopyTo(value, 0);
                    return value;
                }
                set
                {
                    cmbItems.Items.Clear();
                    cmbItems.Items.AddRange(value);
                }
            }

            public bool CanEdit
            {
                get => cmbItems.DropDownStyle == ComboBoxStyle.DropDown;
                set => cmbItems.DropDownStyle = value ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            }

            public int SelectedIndex
            {
                get => cmbItems.SelectedIndex;
                set => cmbItems.SelectedIndex = value;
            }

            readonly Button btnOk = new Button
            {
                DialogResult = DialogResult.OK,
                Text = AppString.Dialog.Ok,
                AutoSize = true
            };
            readonly Button btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };
            readonly ComboBox cmbItems = new ComboBox
            {
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                DropDownHeight = 294.DpiZoom(),
                ImeMode = ImeMode.Disable
            };

            private void InitializeComponents()
            {
                this.Controls.AddRange(new Control[] { cmbItems, btnOk, btnCancel });
                int a = 20.DpiZoom();
                cmbItems.Left = a;
                cmbItems.Width = 85.DpiZoom();
                cmbItems.Top = btnOk.Top = btnCancel.Top = a;
                btnOk.Left = cmbItems.Right + a;
                btnCancel.Left = btnOk.Right + a;
                this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
                cmbItems.AutosizeDropDownWidth();
            }
        }
    }
}