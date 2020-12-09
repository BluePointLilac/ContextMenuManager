using BulePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public class MyToolBar : FlowLayoutPanel
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
                if(selectedButton != null) selectedButton.Opacity = 0;
                selectedButton = value;
                value.Opacity = 0.4F;
                SelectedButtonChanged?.Invoke(null, null);
            }
        }

        public event EventHandler SelectedButtonChanged;

        public int SelectedIndex
        {
            get => Controls.GetChildIndex(SelectedButton);
            set => SelectedButton = (MyToolBarButton)Controls[value];
        }

        public void AddButton(MyToolBarButton button)
        {
            button.Parent = this;
            button.Margin = new Padding(12.DpiZoom(), 4.DpiZoom(), 0, 0);
            button.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Left) { SelectedButton = button; button.Cursor = Cursors.Default; }
            };
            button.MouseEnter += (sender, e) =>
            {
                if(button != SelectedButton) { button.Opacity = 0.2F; button.Cursor = Cursors.Hand; }
            };
            button.MouseLeave += (sender, e) =>
            {
                if(button != SelectedButton) button.Opacity = 0;
            };
        }

        public void AddButtons(MyToolBarButton[] buttons)
        {
            Array.ForEach(buttons, button => AddButton(button));
        }
    }

    public class MyToolBarButton : Panel
    {
        public MyToolBarButton(Image image, string text)
        {
            this.DoubleBuffered = true;
            this.Size = new Size(72, 72).DpiZoom();
            this.Controls.AddRange(new Control[] { picImage, lblText });
            picImage.Location = new Point(16, 6).DpiZoom();
            lblText.Top = 52.DpiZoom();
            lblText.Resize += (sender, e) => lblText.Left = (Width - lblText.Width) / 2;
            this.Image = image;
            this.Text = text;
            MyToolTip.SetToolTip(this, text);
            lblText.SetEnabled(false);
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
            AutoSize = true,
            Font = SystemFonts.MenuFont,
            BackColor = Color.Transparent,
            ForeColor = Color.White
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
    }
}