using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ExplorerRestarter : MyListItem
    {
        public ExplorerRestarter()
        {
            this.Visible = false;
            this.DoubleBuffered = false;
            this.Dock = DockStyle.Bottom;
            this.Image = AppImage.Explorer;
            this.Text = AppString.Other.RestartExplorer;
            ToolTipBox.SetToolTip(BtnRestart, AppString.Tip.RestartExplorer);
            this.AddCtr(BtnRestart);
            this.CanMoveForm();
            ShowHandler += () => this.Visible = true;
            HideHandler += () => this.Visible = false;
            BtnRestart.MouseDown += (sender, e) =>
            {
                ExternalProgram.RestartExplorer();
                this.Visible = false;
            };
        }

        public new bool Visible
        {
            get => base.Visible;
            set
            {
                bool flag = base.Visible != value && this.Parent != null;
                base.Visible = value;
                if(flag) this.Parent.Height += value ? Height : -Height;
            }
        }

        private readonly PictureButton BtnRestart = new PictureButton(AppImage.RestartExplorer);

        private static Action ShowHandler;
        private static Action HideHandler;

        public static new void Show() => ShowHandler?.Invoke();
        public static new void Hide() => HideHandler?.Invoke();
    }
}