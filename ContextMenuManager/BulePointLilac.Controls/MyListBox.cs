using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BulePointLilac.Controls
{
    public class MyListBox : Panel
    {
        public MyListBox()
        {
            this.AutoScroll = true;
            this.BackColor = Color.FromArgb(250, 250, 250);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //使滚动幅度与MyListItem的高度相配合，防止滚动过快导致来不及重绘界面变花
            base.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, Math.Sign(e.Delta) * 50.DpiZoom()));
        }
    }

    public class MyList : FlowLayoutPanel
    {
        public MyListBox Owner
        {
            get => (MyListBox)this.Parent;
            set => this.Parent = value;
        }

        public MyList(MyListBox owner) : this()
        {
            this.Owner = owner;
        }

        public MyList()
        {
            this.AutoSize = true;
            this.Dock = DockStyle.Top;
            this.DoubleBuffered = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private MyListItem hoveredItem;
        public MyListItem HoveredItem
        {
            get => hoveredItem;
            set
            {
                if(hoveredItem == value) return;
                if(hoveredItem != null)
                {
                    hoveredItem.ForeColor = Color.FromArgb(90, 90, 90);
                    //hoveredItem.BackColor = Color.FromArgb(250, 250, 250);
                }
                hoveredItem = value;
                value.ForeColor = Color.FromArgb(0, 138, 217);
                //value.BackColor = Color.FromArgb(200, 230, 250);
                value.Focus();
            }
        }

        public void AddItem(MyListItem item)
        {
            if(item.Parent == this) return;
            item.Parent = this;
            item.Width = Owner.Width - item.Margin.Horizontal;
            Owner.Resize += (sender, e) => item.Width = Owner.Width - item.Margin.Horizontal;
            this.MouseWheel += (sender, e) => item.ContextMenuStrip?.Close();
            item.MouseEnter += (sender, e) => HoveredItem = item;
        }

        public void AddItems(MyListItem[] items)
        {
            Array.ForEach(items, item => AddItem(item));
        }

        public void AddItems(List<MyListItem> items)
        {
            items.ForEach(item => AddItem(item));
        }

        public void SetItemIndex(MyListItem item, int newIndex)
        {
            this.Controls.SetChildIndex(item, newIndex);
        }

        public int GetItemIndex(MyListItem item)
        {
            return this.Controls.GetChildIndex(item);
        }

        public void InsertItem(MyListItem item, int index)
        {
            if(item == null) return;
            this.AddItem(item);
            this.SetItemIndex(item, index);
        }

        public void ClearItems()
        {
            foreach(Control control in Controls) control.Dispose();
            this.Controls.Clear();
        }

        public void SortItemByText()
        {
            List<MyListItem> items = new List<MyListItem>();
            foreach(MyListItem item in this.Controls) items.Add(item);
            this.Controls.Clear();
            items.Sort(new TextComparer());
            items.ForEach(item => this.AddItem(item));
        }

        public class TextComparer : IComparer<MyListItem>
        {
            public int Compare(MyListItem x, MyListItem y)
            {
                if(x.Equals(y)) return 0;
                string[] strs = { x.Text, y.Text };
                Array.Sort(strs);
                if(strs[0] == x.Text) return -1;
                else return 1;
            }
        }
    }

    public class MyListItem : Panel
    {
        public MyListItem()
        {
            this.HasImage = true;
            this.DoubleBuffered = true;
            this.Height = 50.DpiZoom();
            this.Margin = new Padding(0);
            this.ForeColor = Color.FromArgb(80, 80, 80);
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Font = SystemFonts.IconTitleFont;
            this.Controls.AddRange(new Control[] { lblSeparator, flpControls, lblText, picImage });
            flpControls.MouseEnter += (sender, e) => this.OnMouseEnter(null);
            flpControls.MouseDown += (sender, e) => this.OnMouseEnter(null);
            flpControls.Left = this.ClientSize.Width;
            lblText.SetEnabled(false);
            CenterControl(lblText);
            CenterControl(picImage);
        }

        public Image Image
        {
            get => picImage.Image;
            set
            {
                picImage.Image = value;
                picImage.Visible = value != null;
            }
        }
        public new string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }
        public new Font Font
        {
            get => lblText.Font;
            set => lblText.Font = value;
        }
        public new Color ForeColor
        {
            get => lblText.ForeColor;
            set => lblText.ForeColor = value;
        }

        private bool hasImage;
        public bool HasImage
        {
            get => hasImage;
            set
            {
                hasImage = value;
                picImage.Visible = value;
                lblText.Left = value ? 60.DpiZoom() : 20.DpiZoom();
            }
        }

        private readonly Label lblText = new Label
        {
            AutoSize = true
        };
        private readonly PictureBox picImage = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Left = 20.DpiZoom(),
            Enabled = false
        };
        private readonly FlowLayoutPanel flpControls = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Top = 0
        };
        private readonly Label lblSeparator = new Label
        {
            BackColor = Color.FromArgb(200, 200, 200),
            Dock = DockStyle.Bottom,
            Height = 1
        };

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); OnMouseEnter(null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e); flpControls.Height = this.Height;
        }

        private void CenterControl(Control ctr)
        {
            void reSize(Control c)
            {
                if(c.Parent == null) return;
                int top = (c.Parent.Height - c.Height) / 2;
                if(c.Parent == this) c.Top = top;
                else if(c.Parent == flpControls)
                {
                    c.Margin = new Padding(0, top, c.Margin.Right, top);
                }
            }
            ctr.Parent.Resize += (sender, e) => reSize(ctr);
            ctr.Resize += (sender, e) => reSize(ctr);
            reSize(ctr);
        }

        public void AddCtr(Control ctr)
        {
            int space = 20.DpiZoom();
            //为第一个ctr预留垂直滚动条的宽度
            if(flpControls.Controls.Count == 0) space += SystemInformation.VerticalScrollBarWidth;
            AddCtr(ctr, space);
        }

        public void AddCtr(Control ctr, int space)
        {
            ctr.Parent = flpControls;
            ctr.Margin = new Padding(0, 0, space, 0);
            ctr.MouseEnter += (sender, e) => this.OnMouseEnter(null);
            ctr.MouseDown += (sender, e) => this.OnMouseEnter(null);
            CenterControl(ctr);
        }

        public void AddCtrs(Control[] ctrs)
        {
            Array.ForEach(ctrs, ctr => AddCtr(ctr));
        }

        public void RemoveCtr(Control ctr)
        {
            if(ctr.Parent == flpControls) flpControls.Controls.Remove(ctr);
        }

        public void RemoveCtrAt(int index)
        {
            if(flpControls.Controls.Count > index) flpControls.Controls.RemoveAt(index);
        }

        public int GetCtrIndex(Control ctr)
        {
            return flpControls.Controls.GetChildIndex(ctr, true);
        }

        public void SetCtrIndex(Control ctr, int newIndex)
        {
            flpControls.Controls.SetChildIndex(ctr, newIndex);
        }
    }
}