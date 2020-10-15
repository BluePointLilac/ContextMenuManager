using BulePointLilac.Methods;
using ContextMenuManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public sealed class InputDialog : CommonDialog
    {
        /// <summary>输入对话框标题</summary>
        public string Title { get; set; } = AppString.General_AppName;
        /// <summary>输入对话框文本框文本</summary>
        public string Text { get; set; }

        private static Size LastSize = new Size();
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(InputBox frm = new InputBox { Text = Title, Size = LastSize })
            {
                frm.TxtInput.Text = this.Text;
                bool flag = frm.ShowDialog() == DialogResult.OK;
                this.Text = flag ? frm.TxtInput.Text : null;
                LastSize = frm.Size;
                return flag;
            }
        }

        sealed class InputBox : Form
        {
            public InputBox()
            {
                this.AcceptButton = BtnOk;
                this.CancelButton = BtnCancel;
                this.Font = SystemFonts.MessageBoxFont;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = this.Size = new Size(400, 150).DpiZoom();
                this.MaximizeBox = MinimizeBox = ShowIcon = ShowInTaskbar = false;
                this.Controls.AddRange(new Control[] { TxtInput, BtnOk, BtnCancel });
                TxtInput.MouseWheel += ResizeFont;
                InitializeComponents();
            }

            public readonly TextBox TxtInput = new TextBox
            {
                Font = new Font(SystemFonts.MenuFont.FontFamily, 11F),
                ScrollBars = ScrollBars.Vertical,
                Multiline = true
            };
            readonly Button BtnOk = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                Text = AppString.Ok,
                AutoSize = true
            };
            readonly Button BtnCancel = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel,
                Text = AppString.Cancel,
                AutoSize = true
            };

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                TxtInput.Width = BtnCancel.Right - TxtInput.Left;
                TxtInput.Height = BtnCancel.Top - 2 * TxtInput.Top;
            }

            private void InitializeComponents()
            {
                int a = 20.DpiZoom();
                TxtInput.Location = new Point(a, a);
                BtnCancel.Top = BtnOk.Top = this.ClientSize.Height - BtnOk.Height - a;
                BtnCancel.Left = this.ClientSize.Width - BtnCancel.Width - a;
                BtnOk.Left = BtnCancel.Left - BtnOk.Width - a;
                this.OnResize(null);
            }

            /// <summary>缩放文本框字体</summary>
            private void ResizeFont(object sender, MouseEventArgs e)
            {
                if(ModifierKeys != Keys.Control) return;
                float size = TxtInput.Font.Size;
                if(size < 8F && e.Delta < 0) return;
                if(size > 40F && e.Delta > 0) return;
                TxtInput.Font = new Font(TxtInput.Font.FontFamily, size + (e.Delta > 0 ? 1 : -1));
            }
        }
    }
}