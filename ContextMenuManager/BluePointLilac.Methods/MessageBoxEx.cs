using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    /// <summary>在窗体居中显示的MessageBox</summary>
    public static class MessageBoxEx
    {
        /// <summary>弹出一个消息框</summary>
        /// <param name="text">要在消息框中显示的文本</param>
        /// <param name="caption">要在消息框的标题栏中显示的文本</param>
        /// <param name="buttons">指定在消息框中显示哪些按钮</param>
        /// <param name="boxIcon">指定在消息框中显示哪个图标</param>
        /// <param name="owner">指定消息框的拥有者</param>
        /// <param name="defaultResult">指定默认结果，使对应按钮预先获取焦点</param>
        /// <param name="canMoveParent">能否移动父窗体</param>
        /// <returns>System.Windows.Forms.DialogResult 值之一</returns>
        public static DialogResult Show(string text, string caption = null,
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon boxIcon = MessageBoxIcon.None,
            IWin32Window owner = null, DialogResult defaultResult = DialogResult.None, bool canMoveParent = true)
        {
            using(MessageBoxForm frm = new MessageBoxForm(text, caption, buttons, boxIcon, defaultResult, canMoveParent))
            {
                return frm.ShowDialog(owner);
            }
        }

        /// <summary>弹出一个消息框</summary>
        /// <param name="text">要在消息框中显示的文本</param>
        /// <param name="caption">要在消息框的标题栏中显示的文本</param>
        /// <param name="buttonTexts">从右至左添加数组长度的个数的按钮，按钮文本为数组对应成员</param>
        /// <param name="boxImaage">指定在消息框中显示的图标图像</param>
        /// <param name="owner">指定消息框的拥有者</param>
        /// <param name="defaultResult">指定默认结果，使对应按钮预先获取焦点</param>
        /// <param name="canMoveParent">能否移动父窗体</param>
        /// <param name="closeBox">消息框关闭按钮是否可用</param>
        /// <returns>返回用户点击的按钮所显示的文本</returns>
        public static string Show(string text, string caption,
            string[] buttonTexts, Image boxImaage,
            IWin32Window owner = null, string defaultResult = null,
            bool canMoveParent = true, bool closeBox = true)
        {
            using(MessageBoxForm frm = new MessageBoxForm(text, caption, buttonTexts, boxImaage, defaultResult, canMoveParent, closeBox))
            {
                frm.ShowDialog(owner);
                return frm.Tag?.ToString();
            }
        }

        sealed class MessageBoxForm : Form
        {
            private MessageBoxForm(string text, string caption, bool canMoveParent)
            {
                lblText.Text = text;
                this.Text = caption;
                this.CanMoveParent = canMoveParent;
                this.Font = SystemFonts.MessageBoxFont;
                this.ShowIcon = this.ShowInTaskbar = false;
                this.MaximizeBox = this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterParent;
            }

            public MessageBoxForm(string text, string caption,
                string[] buttonTexts, Image boxImage,
                string defaultResult, bool canMoveParent, bool closeBox) : this(text, caption, canMoveParent)
            {
                this.CloseBox = closeBox;
                this.InitializeComponents(buttonTexts, boxImage);
                foreach(Button button in flpButtons.Controls)
                {
                    button.Click += (sender, e) =>
                    {
                        this.Tag = button.Text;
                        this.Close();
                    };
                    this.Shown += (sender, e) =>
                    {
                        if(button.Text == defaultResult) button.Focus();
                    };
                }
            }

            public MessageBoxForm(string text, string caption,
                MessageBoxButtons buttons, MessageBoxIcon boxIcon,
                DialogResult defaultResult, bool canMoveParent) : this(text, caption, canMoveParent)
            {
                string[] buttonTexts = null;
                Image boxImage = null;
                switch(buttons)
                {
                    case MessageBoxButtons.OK:
                        buttonTexts = new[] { "OK" }; break;
                    case MessageBoxButtons.OKCancel:
                        buttonTexts = new[] { "Cancel", "OK" }; break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        buttonTexts = new[] { "&Ignore", "&Retry", "&Abort" }; break;
                    case MessageBoxButtons.YesNoCancel:
                        buttonTexts = new[] { "Cancel", "&No", "&Yes" }; break;
                    case MessageBoxButtons.YesNo:
                        buttonTexts = new[] { "&No", "&Yes" }; break;
                    case MessageBoxButtons.RetryCancel:
                        buttonTexts = new[] { "Cancel", "&Retry" }; break;
                }
                switch(boxIcon)
                {
                    case MessageBoxIcon.Question:
                        boxImage = MessageBoxImage.Question; break;
                    case MessageBoxIcon.Error:
                        boxImage = MessageBoxImage.Error; break;
                    case MessageBoxIcon.Warning:
                        boxImage = MessageBoxImage.Warning; break;
                    case MessageBoxIcon.Information:
                        boxImage = MessageBoxImage.Information; break;
                }
                this.InitializeComponents(buttonTexts, boxImage);
                foreach(Button button in flpButtons.Controls)
                {
                    switch(button.Text)
                    {
                        case "OK":
                            if(buttons == MessageBoxButtons.OK)
                            {
                                this.CancelButton = button;
                                this.FormClosing += (sender, e) => button.PerformClick();
                            }
                            button.DialogResult = DialogResult.OK; break;
                        case "Cancel":
                            this.CancelButton = button;
                            button.DialogResult = DialogResult.Cancel; break;
                        case "&Yes":
                            button.DialogResult = DialogResult.Yes; break;
                        case "&No":
                            button.DialogResult = DialogResult.No; break;
                        case "&Abort":
                            button.DialogResult = DialogResult.Abort; break;
                        case "&Retry":
                            button.DialogResult = DialogResult.Retry; break;
                        case "&Ignore":
                            button.DialogResult = DialogResult.Ignore; break;
                    }
                    this.Shown += (sender, e) =>
                    {
                        if(button.DialogResult == defaultResult) button.Focus();
                    };
                }
                this.CloseBox = this.CancelButton != null;
            }

            private void InitializeComponents(string[] buttonTexts, Image boxImage)
            {
                this.SuspendLayout();
                int w1 = 36.DpiZoom();
                Size buttonSize = new Size(75, 27).DpiZoom();
                for(int i = 0; i < buttonTexts.Length; i++)
                {
                    Button button = new Button
                    {
                        Margin = new Padding(12, 0, 0, 0).DpiZoom(),
                        Text = buttonTexts[i],
                        Parent = flpButtons,
                        AutoSize = true,
                    };
                    button.Width = Math.Max(buttonSize.Width, button.Width);
                    button.Height = Math.Max(buttonSize.Height, button.Height);
                    buttonSize = button.Size;
                    w1 += button.Width + button.Margin.Horizontal;
                }
                picIcon.Image = boxImage;
                if(boxImage == null)
                {
                    picIcon.Visible = false;
                    lblText.Left = picIcon.Left;
                }
                pnlInfo.Controls.AddRange(new Control[] { picIcon, lblText });
                this.Controls.AddRange(new Control[] { pnlInfo, flpButtons });
                pnlInfo.Height = lblText.Height + lblText.Top * 2;
                if(picIcon.Height > lblText.Height / 2)
                {
                    picIcon.Top = (pnlInfo.Height - picIcon.Height) / 2;
                }
                int w2 = lblText.Right + picIcon.Left;
                int w = Math.Max(w1, w2);
                int h = pnlInfo.Height + flpButtons.Height;
                this.ClientSize = new Size(w, h);
                this.ResumeLayout();
            }

            readonly FlowLayoutPanel flpButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12.DpiZoom()),
                Dock = DockStyle.Bottom,
                Height = 50.DpiZoom(),
                WrapContents = false,
            };
            readonly Panel pnlInfo = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
            };
            readonly PictureBox picIcon = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(32, 32).DpiZoom(),
            };
            readonly Label lblText = new Label
            {
                Location = new Point(68, 32).DpiZoom(),
                AutoSize = true,
            };

            readonly bool CloseBox = true;//关闭按钮可用性
            readonly bool CanMoveParent = true;//可移动父窗体

            protected override CreateParams CreateParams
            {
                get
                {
                    const int CP_NOCLOSE_BUTTON = 0x200;
                    CreateParams cp = base.CreateParams;
                    if(!CloseBox) cp.ClassStyle |= CP_NOCLOSE_BUTTON; //禁用关闭按钮
                    return cp;
                }
            }

            protected override void OnLoad(EventArgs e)
            {
                if(this.Owner == null && Form.ActiveForm != this) this.Owner = Form.ActiveForm;
                if(this.Owner == null) this.StartPosition = FormStartPosition.CenterScreen;
                else
                {
                    this.TopMost = this.Owner.TopMost;
                    this.StartPosition = FormStartPosition.CenterParent;
                    if(this.CanMoveParent) this.MoveAsMove(this.Owner);
                }
                base.OnLoad(e);
            }
        }
    }

    public static class MessageBoxImage
    {
        // SystemIcons 里面的图标不是扁平的,❌、⚠️、❔、ℹ️
        public static readonly Image Error = GetImage(-98);
        public static readonly Image Warning = GetImage(-84);
        public static readonly Image Question = GetImage(-99);
        public static readonly Image Information = GetImage(-81);

        private static Image GetImage(int index)
        {
            using(Icon icon = ResourceIcon.GetIcon("imageres.dll", index))
            {
                return icon?.ToBitmap();
            }
        }
    }
}