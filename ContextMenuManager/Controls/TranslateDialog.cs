using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class TranslateDialog : CommonDialog
    {
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(TranslateForm frm = new TranslateForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                return flag;
            }
        }

        sealed class TranslateForm : Form
        {
            public TranslateForm()
            {
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.StartPosition = FormStartPosition.CenterParent;
                this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10F);
                cmbSections.Width = cmbKeys.Width = 200.DpiZoom();
                cmbSections.Left = cmbSections.Top = cmbKeys.Top = 20.DpiZoom();
                cmbKeys.Left = cmbSections.Right + 20.DpiZoom();
                this.Width = cmbKeys.Right + 20.DpiZoom();
                this.Controls.AddRange(new Control[] { cmbSections, cmbKeys });
                cmbSections.Items.AddRange(AppString.DefaultLanguage.RootDic.Keys.ToArray());
                cmbSections.SelectedIndexChanged += (sender, e) =>
                {
                    cmbKeys.Items.Clear();
                    cmbKeys.Items.AddRange(AppString.DefaultLanguage.RootDic[cmbSections.Text].Keys.ToArray());
                    cmbKeys.SelectedIndex = 0;
                };
                cmbSections.SelectedIndex = 0;
            }

            readonly ComboBox cmbSections = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            readonly ComboBox cmbKeys = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }
    }
}