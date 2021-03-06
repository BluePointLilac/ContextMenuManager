﻿using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    public sealed class ExplorerRestarter : MyListItem
    {
        public ExplorerRestarter()
        {
            this.Visible = false;
            this.Dock = DockStyle.Bottom;
            this.Image = AppImage.Explorer;
            this.Text = AppString.Other.RestartExplorer;
            ToolTipBox.SetToolTip(BtnRestart, AppString.Tip.RestartExplorer);
            this.AddCtr(BtnRestart);
            this.CanMoveForm();
            BtnRestart.MouseDown += (sender, e) => { ExternalProgram.RestartExplorer(); this.Visible = false; };
            ShowHandler += () => this.Visible = true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if(this.Parent != null) this.Parent.Height += Visible ? Height : -Height;
        }

        private readonly PictureButton BtnRestart = new PictureButton(AppImage.RestartExplorer);

        private static Action ShowHandler { get; set; }

        public static new void Show() { ShowHandler?.Invoke(); }
    }
}