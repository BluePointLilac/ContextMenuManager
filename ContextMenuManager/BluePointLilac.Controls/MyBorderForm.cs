using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public class MyBorderForm : Form
    {
        public MyBorderForm()
        {
            this.HelpBox = false;
            //无边框窗体最大化不覆盖任务栏
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.FormBorderStyle = FormBorderStyle.None;
            this.InitializeComponents();
        }

        public new Icon Icon
        {
            get => base.Icon;
            set
            {
                base.Icon = value;
                picIcon.Image = value.ToBitmap().ResizeImage(picIcon.Size);
            }
        }
        public new string Text
        {
            get => base.Text;
            set
            {
                lblTilte.Text = base.Text = value;
                SetTitleLeft();
            }
        }
        public new bool MaximizeBox
        {
            get => lblMax.Visible;
            set => lblMax.Visible = value;
        }
        public new bool MinimizeBox
        {
            get => lblMin.Visible;
            set => lblMin.Visible = value;
        }
        public bool CloseBox
        {
            get => lblClose.Visible;
            set => lblClose.Visible = value;
        }
        public bool HelpBox
        {
            get => lblHelp.Visible;
            set => lblHelp.Visible = value;
        }
        public new bool ShowIcon
        {
            get => picIcon.Visible;
            set
            {
                picIcon.Visible = value;
                SetTitleLeft();
            }
        }
        public Color TitleBarBackColor
        {
            get => pnlTitleBar.BackColor;
            set => pnlTitleBar.BackColor = value;
        }
        public Color TitleForeColor
        {
            get => lblTilte.ForeColor;
            set => lblTilte.ForeColor = value;
        }
        private bool centerTitle;
        public bool CenterTitle
        {
            get => centerTitle;
            set
            {
                centerTitle = value;
                SetTitleLeft();
            }
        }

        readonly Panel pnlTitleBar = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Top,
            Height = 30.DpiZoom()
        };
        readonly FlowLayoutPanel flpControls = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Marlett", 11F),
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Top = 0
        };
        readonly Label lblHelp = new Label { Text = "s" };
        readonly Label lblMin = new Label { Text = "0" };
        readonly Label lblMax = new Label { Text = "1" };
        readonly Label lblClose = new Label { Text = "r" };
        readonly Label[] lblBorders = new Label[]
        {
            new Label { Cursor = Cursors.SizeWE, Dock = DockStyle.Left },
            new Label { Cursor = Cursors.SizeWE, Dock = DockStyle.Right },
            new Label { Cursor = Cursors.SizeNS, Dock = DockStyle.Top },
            new Label { Cursor = Cursors.SizeNS, Dock = DockStyle.Bottom }
        };
        readonly PictureBox picIcon = new PictureBox
        {
            Location = new Point(8, 8).DpiZoom(),
            Size = new Size(16, 16).DpiZoom(),
            Enabled = false
        };
        readonly Label lblTilte = new Label
        {
            Location = new Point(26, 8).DpiZoom(),
            Font = new Font(SystemFonts.CaptionFont.FontFamily, 9F),
            AutoSize = true
        };

        /// <summary>无边框窗体放缩窗体和移动窗体</summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg == HitTestMessage.WM_NCHITTEST && this.WindowState == FormWindowState.Normal)
            {
                Point point = PointToClient(Cursor.Position);
                int x = point.X;
                int y = point.Y;
                HitTestMessage.HitTest res = 0;
                if(x <= 5)
                {
                    if(y <= 5) res = HitTestMessage.HitTest.TopLeft;
                    else if(y >= ClientSize.Height - 5) res = HitTestMessage.HitTest.BottomLeft;
                    else res = HitTestMessage.HitTest.Left;
                }
                else if(x >= ClientSize.Width - 5)
                {
                    if(y <= 5) res = HitTestMessage.HitTest.TopRight;
                    else if(y >= ClientSize.Height - 5) res = HitTestMessage.HitTest.BottomRight;
                    else res = HitTestMessage.HitTest.Right;
                }
                else if(y <= 5) res = HitTestMessage.HitTest.Top;
                else if(y >= ClientSize.Height - 5) res = HitTestMessage.HitTest.Bottom;
                m.Result = (IntPtr)res;
            }
        }

        /// <summary>最小化后点击任务栏图标还原窗口</summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.Style |= 0x20000;//WS_MINIMIZEBOX
                return p;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            lblMax.Text = this.WindowState == FormWindowState.Normal ? "1" : "2";
            SetTitleLeft();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetTitleLeft();
        }

        private void SetMaxOrNormal()
        {
            if(this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void SetTitleLeft()
        {
            if(CenterTitle) lblTilte.Left = (pnlTitleBar.Width - lblTilte.Width) / 2;
            else if(ShowIcon) lblTilte.Left = 26.DpiZoom();
            else lblTilte.Left = 4.DpiZoom();
        }

        private void InitializeComponents()
        {
            this.Controls.Add(pnlTitleBar);
            this.ControlAdded += (sender, e) => pnlTitleBar.SendToBack();
            foreach(Label label in lblBorders)
            {
                label.Parent = this;
                label.Enabled = false;
                label.Size = new Size(1, 1);
                label.BackColor = Color.FromArgb(85, 145, 215);
                this.ControlAdded += (sender, e) => label.SendToBack();
            }
            lblTilte.SetEnabled(false);
            pnlTitleBar.CanMoveForm();
            pnlTitleBar.Controls.AddRange(new Control[] { flpControls, picIcon, lblTilte });
            flpControls.Left = pnlTitleBar.Width;
            flpControls.SizeChanged += (sender, e) => this.MinimumSize = new Size(flpControls.Width + 2, pnlTitleBar.Height + 2);
            foreach(Label label in new[] { lblClose, lblMax, lblMin, lblHelp })
            {
                label.Parent = flpControls;
                label.Margin = new Padding(0);
                label.Size = new Size(32, 30).DpiZoom();
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.MouseLeave += (sender, e) => label.BackColor = pnlTitleBar.BackColor;
                label.MouseEnter += (sender, e) => label.BackColor = Color.FromArgb(213, 225, 242);
                label.MouseDown += (sender, e) => label.BackColor = Color.FromArgb(163, 189, 227);
            }
            lblClose.MouseClick += (sender, e) => { if(e.Button == MouseButtons.Left) this.Close(); };
            lblMin.MouseClick += (sender, e) => { if(e.Button == MouseButtons.Left) this.WindowState = FormWindowState.Minimized; };
            lblMax.MouseClick += (sender, e) => { if(e.Button == MouseButtons.Left) this.SetMaxOrNormal(); };
            pnlTitleBar.MouseDoubleClick += (sender, e) => { if(e.Button == MouseButtons.Left) this.SetMaxOrNormal(); };
            lblHelp.Click += (sender, e) => this.OnHelpButtonClicked(null);

        }
    }
}