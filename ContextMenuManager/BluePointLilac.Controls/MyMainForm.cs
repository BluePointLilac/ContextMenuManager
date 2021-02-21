using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public class MyMainForm : Form
    {
        public MyMainForm()
        {
            this.Text = Application.ProductName;
            this.MinimumSize = this.Size = new Size(866, 649).DpiZoom();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Controls.AddRange(new Control[] { MainBody, SideBar, StatusBar, ToolBar });
            SideBar.Resize += (sender, e) => MainBody.Width = ClientSize.Width - SideBar.Width;
            ToolBar.CanMoveForm();
            StatusBar.CanMoveForm();
        }

        protected MyToolBar ToolBar = new MyToolBar();
        protected MySideBar SideBar = new MySideBar();
        protected MyStatusBar StatusBar = new MyStatusBar();
        protected MyListBox MainBody = new MyListBox
        {
            Dock = DockStyle.Left
        };

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            MainBody.Width = ClientSize.Width - SideBar.Width;
        }
    }
}