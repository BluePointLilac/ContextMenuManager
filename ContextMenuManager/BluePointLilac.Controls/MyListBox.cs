using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ContextMenuManager.Methods;

namespace BluePointLilac.Controls
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
            this.WrapContents = true;
            this.Dock = DockStyle.Top;
            this.DoubleBuffered = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        // MyList 基类 控件仓库(缓存) 
        private Dictionary<string, MyCustomList<MyListItem>> itemRepo = new Dictionary<string, MyCustomList<MyListItem>>();
        // 重置仓库的 Idx
        protected void ResetRepo()
        {
            foreach (string className in itemRepo.Keys)
                itemRepo[className].Idx = 0;
        }
        // 从仓库获取控件
        protected bool GetFromRepo(string itemClassName,out MyListItem ret)
        {
            if (itemRepo.TryGetValue(itemClassName, out MyCustomList<MyListItem> itemlist))
            {
                for (int i = itemlist.Idx; i < itemlist.Count; i++)
                {
                    MyListItem ctr = itemlist[i];
                    if (ctr.Parent == null)
                    {
                        ret = ctr;
                        itemlist.Idx = i + 1;
                        return true;
                    }
                }
                itemlist.Idx = itemlist.Count;
            }
            ret = null;
            return false;
        }
        // 保存控件到缓存
        protected void SaveToRepo(string itemClassName,MyListItem ctr)
        {
            if (!itemRepo.TryGetValue(itemClassName, out MyCustomList<MyListItem> itemlist)||itemlist==null)
            {
                itemlist = new MyCustomList<MyListItem>();
                itemRepo[itemClassName] = itemlist;
            }
            itemlist.Add(ctr);
            // 表示控件可重用
            ctr.Reuse = true;
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
                    //hoveredItem.Font = new Font(hoveredItem.Font, FontStyle.Regular);
                }
                hoveredItem = value;
                if(hoveredItem != null)
                {
                    value.ForeColor = Color.FromArgb(0, 138, 217);
                    //value.BackColor = Color.FromArgb(200, 230, 250);
                    //value.Font = new Font(hoveredItem.Font, FontStyle.Bold);
                    value.Focus();
                }
                HoveredItemChanged?.Invoke(this, null);
            }
        }

        public event EventHandler HoveredItemChanged;

        public void AddItem(MyListItem item)
        {
            this.SuspendLayout();
            item.Parent = this;
            item.MouseEnter += (sender, e) => HoveredItem = item;
            this.MouseWheel += (sender, e) => item.ContextMenuStrip?.Close();
            void ResizeItem() => item.Width = Owner.Width - item.Margin.Horizontal;
            Owner.Resize += (sender, e) => ResizeItem();
            ResizeItem();
            this.ResumeLayout();
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
            return Controls.GetChildIndex(item);
        }

        public void InsertItem(MyListItem item, int index)
        {
            if(item == null) return;
            this.AddItem(item);
            this.SetItemIndex(item, index);
        }

        public virtual void ClearItems()
        {
            if(this.Controls.Count == 0) return;
            this.SuspendLayout();
            for(int i = this.Controls.Count - 1; i >= 0; i--)
            {
                Control ctr = this.Controls[i];
                this.Controls.Remove(ctr);
                // 不销毁 Reuse=true的控件
                if (ctr is MyListItem myListItem)
                {
                    if (myListItem.Reuse)
                    {
                        continue;
                    }
                }
                ctr.Dispose();
            }
            this.ResumeLayout();
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
            this.SuspendLayout();
            this.HasImage = true;
            this.DoubleBuffered = true;
            this.Height = 50.DpiZoom();
            this.Margin = new Padding(0);
            this.Font = SystemFonts.IconTitleFont;
            this.ForeColor = Color.FromArgb(80, 80, 80);
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.AddRange(new Control[] { lblSeparator, flpControls, lblText, picImage });
            this.Resize += (Sender, e) => pnlScrollbar.Height = this.ClientSize.Height;
            flpControls.MouseClick += (sender, e) => this.OnMouseClick(e);
            flpControls.MouseEnter += (sender, e) => this.OnMouseEnter(e);
            flpControls.MouseDown += (sender, e) => this.OnMouseDown(e);
            lblSeparator.SetEnabled(false);
            lblText.SetEnabled(false);
            CenterControl(lblText);
            CenterControl(picImage);
            AddCtr(pnlScrollbar, 0);
            this.ResumeLayout();
        }
        // 需要重用,不能销毁(Dispose)
        public bool Reuse { get; set; } = false;
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
                lblText.Left = (value ? 60 : 20).DpiZoom();
            }
        }

        private readonly Label lblText = new Label
        {
            AutoSize = true,
            Name = "Text"
        };
        private readonly PictureBox picImage = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            Left = 20.DpiZoom(),
            Enabled = false,
            Name = "Image"
        };
        private readonly FlowLayoutPanel flpControls = new FlowLayoutPanel
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.RightToLeft,
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Name = "Controls"
        };
        private readonly Label lblSeparator = new Label
        {
            BackColor = Color.FromArgb(220, 220, 220),
            Dock = DockStyle.Bottom,
            Name = "Separator",
            Height = 1
        };//分割线
        private readonly Panel pnlScrollbar = new Panel
        {
            Width = SystemInformation.VerticalScrollBarWidth,
            Enabled = false
        };//预留滚动条宽度

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); OnMouseEnter(null);
        }

        private void CenterControl(Control ctr)
        {
            void reSize()
            {
                if(ctr.Parent == null) return;
                int top = (this.ClientSize.Height - ctr.Height) / 2;
                ctr.Top = top;
                if(ctr.Parent == flpControls)
                {
                    ctr.Margin = new Padding(0, top, ctr.Margin.Right, top);
                }
            }
            ctr.Parent.Resize += (sender, e) => reSize();
            ctr.Resize += (sender, e) => reSize();
            reSize();
        }

        public void AddCtr(Control ctr)
        {
            AddCtr(ctr, 20.DpiZoom());
        }

        public void AddCtr(Control ctr, int space)
        {
            this.SuspendLayout();
            ctr.Parent = flpControls;
            ctr.Margin = new Padding(0, 0, space, 0);
            ctr.MouseEnter += (sender, e) => this.OnMouseEnter(e);
            ctr.MouseDown += (sender, e) => this.OnMouseEnter(e);
            CenterControl(ctr);
            this.ResumeLayout();
        }

        public void AddCtrs(Control[] ctrs)
        {
            Array.ForEach(ctrs, ctr => AddCtr(ctr));
        }

        public void RemoveCtrAt(int index)
        {
            if(flpControls.Controls.Count > index) flpControls.Controls.RemoveAt(index + 1);
        }

        public int GetCtrIndex(Control ctr)
        {
            return flpControls.Controls.GetChildIndex(ctr, true) - 1;
        }

        public void SetCtrIndex(Control ctr, int newIndex)
        {
            flpControls.Controls.SetChildIndex(ctr, newIndex + 1);
        }
    }
}