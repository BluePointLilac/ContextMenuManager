using BluePointLilac.Methods;
using ContextMenuManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public sealed class InputDialog : CommonDialog
    {
        /// <summary>输入对话框标题</summary>
        public string Title { get; set; } = AppString.General.AppName;
        /// <summary>输入对话框文本框文本</summary>
        public string Text { get; set; }

        private static Size LastSize = new Size();
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(InputBox frm = new InputBox { Text = Title, Size = LastSize })
            {
                frm.InputedText = this.Text;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                this.Text = flag ? frm.InputedText : null;
                LastSize = frm.Size;
                return flag;
            }
        }

        sealed class InputBox : Form
        {
            public InputBox()
            {
                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Font = SystemFonts.MessageBoxFont;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(400, 150).DpiZoom();
                this.MaximizeBox = MinimizeBox = ShowIcon = ShowInTaskbar = false;
                this.Controls.AddRange(new Control[] { txtInput, btnOk, btnCancel });
                txtInput.CanResizeFont();
                InitializeComponents();
            }

            public string InputedText
            {
                get => txtInput.Text;
                set => txtInput.Text = value;
            }

            readonly TextBox txtInput = new TextBox
            {
                Font = new Font(SystemFonts.MenuFont.FontFamily, 11F),
                ScrollBars = ScrollBars.Vertical,
                Multiline = true
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

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                txtInput.Width = btnCancel.Right - txtInput.Left;
                txtInput.Height = btnCancel.Top - 2 * txtInput.Top;
            }

            private void InitializeComponents()
            {
                int a = 20.DpiZoom();
                txtInput.Location = new Point(a, a);
                btnCancel.Top = btnOk.Top = this.ClientSize.Height - btnOk.Height - a;
                btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
                btnOk.Left = btnCancel.Left - btnOk.Width - a;
                this.OnResize(null);
            }
        }
    }
}