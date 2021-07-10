using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public sealed class MyToolBar : FlowLayoutPanel
    {
        public MyToolBar()
        {
            this.Height = 80.DpiZoom();
            this.Dock = DockStyle.Top;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(85, 145, 215);
        }

        private MyToolBarButton selectedButton;
        public MyToolBarButton SelectedButton
        {
            get => selectedButton;
            set
            {
                if(selectedButton == value) return;
                if(selectedButton != null)
                {
                    selectedButton.Opacity = 0;
                    selectedButton.Cursor = Cursors.Hand;
                }
                selectedButton = value;
                selectedButton.Opacity = 0.4F;
                selectedButton.Cursor = Cursors.Default;
                SelectedButtonChanged?.Invoke();
            }
        }

        public Action SelectedButtonChanged { get; set; }

        public int SelectedIndex
        {
            get => Controls.GetChildIndex(SelectedButton);
            set => SelectedButton = (MyToolBarButton)Controls[value];
        }

        public void AddButton(MyToolBarButton button)
        {
            button.Parent = this;
            button.Margin = new Padding(12, 4, 0, 0).DpiZoom();
            button.MouseDown += (sender, e) =>
            {
                if(button.CanBeSelected) SelectedButton = button;
            };
            button.MouseEnter += (sender, e) =>
            {
                if(button != SelectedButton) button.Opacity = 0.2F;
            };
            button.MouseLeave += (sender, e) =>
            {
                if(button != SelectedButton) button.Opacity = 0;
            };
        }

        public void AddButtons(MyToolBarButton[] buttons)
        {
            int maxWidth = 72.DpiZoom();
            Array.ForEach(buttons, button => maxWidth = Math.Max(maxWidth, TextRenderer.MeasureText(button.Text, button.Font).Width));
            Array.ForEach(buttons, button => { button.Width = maxWidth; AddButton(button); });
        }
    }

    public sealed class MyToolBarButton : Panel
    {
        public MyToolBarButton(Image image, string text)
        {
            this.DoubleBuffered = true;
            this.Size = new Size(72, 72).DpiZoom();
            this.Controls.AddRange(new Control[] { picImage, lblText });
            lblText.Resize += (sender, e) => this.OnResize(null);
            picImage.Top = 6.DpiZoom();
            lblText.Top = 52.DpiZoom();
            lblText.SetEnabled(false);
            this.Image = image;
            this.Text = text;
        }

        readonly PictureBox picImage = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(40, 40).DpiZoom(),
            BackColor = Color.Transparent,
            Enabled = false
        };

        readonly Label lblText = new Label
        {
            BackColor = Color.Transparent,
            Font = SystemFonts.MenuFont,
            ForeColor = Color.White,
            AutoSize = true,
        };

        public Image Image
        {
            get => picImage.Image;
            set => picImage.Image = value;
        }
        public new string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }
        public float Opacity
        {
            get => BackColor.A / 255;
            set => BackColor = Color.FromArgb((int)(value * 255), Color.White);
        }
        public bool CanBeSelected { get; set; } = true;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left) base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            lblText.Left = (this.Width - lblText.Width) / 2;
            picImage.Left = (this.Width - picImage.Width) / 2;
        }
    }
}