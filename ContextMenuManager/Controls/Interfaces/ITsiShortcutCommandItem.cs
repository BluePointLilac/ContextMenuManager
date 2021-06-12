using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiShortcutCommandItem
    {
        ShellLink ShellLink { get; }
        ShortcutCommandMenuItem TsiChangeCommand { get; set; }
        ContextMenuStrip ContextMenuStrip { get; set; }
    }

    sealed class ShortcutCommandMenuItem : ToolStripMenuItem
    {
        public ShortcutCommandMenuItem(ITsiShortcutCommandItem item) : base(AppString.Menu.ChangeCommand)
        {
            item.ContextMenuStrip.Opening += (sender, e) =>
            {
                this.Visible = !string.IsNullOrEmpty(item.ShellLink?.TargetPath);
            };
        }

        public bool ChangeCommand(ShellLink shellLink)
        {
            using(CommandDialog dlg = new CommandDialog())
            {
                dlg.Command = shellLink.TargetPath;
                dlg.Arguments = shellLink.Arguments;
                if(dlg.ShowDialog() != DialogResult.OK) return false;
                shellLink.TargetPath = dlg.Command;
                shellLink.Arguments = dlg.Arguments;
                shellLink.Save();
                return true;
            }
        }

        sealed class CommandDialog : CommonDialog
        {
            public string Command { get; set; }
            public string Arguments { get; set; }

            private static Size LastSize = new Size();

            public override void Reset() { }

            protected override bool RunDialog(IntPtr hwndOwner)
            {
                using(CommandForm frm = new CommandForm())
                {
                    frm.Size = LastSize;
                    frm.Command = this.Command;
                    frm.Arguments = this.Arguments;
                    bool flag = frm.ShowDialog() == DialogResult.OK;
                    LastSize = frm.Size;
                    if(flag)
                    {
                        this.Command = frm.Command;
                        this.Arguments = frm.Arguments;
                    }
                    return flag;
                }
            }

            sealed class CommandForm : ResizbleForm
            {
                public CommandForm()
                {
                    this.AcceptButton = btnOk;
                    this.CancelButton = btnCancel;
                    this.VerticalResizable = false;
                    this.Font = SystemFonts.MessageBoxFont;
                    this.Text = AppString.Menu.ChangeCommand;
                    this.SizeGripStyle = SizeGripStyle.Hide;
                    this.StartPosition = FormStartPosition.CenterParent;
                    this.MaximizeBox = MinimizeBox = ShowIcon = ShowInTaskbar = false;
                    InitializeComponents();
                }

                public string Command
                {
                    get => txtCommand.Text;
                    set => txtCommand.Text = value;
                }

                public string Arguments
                {
                    get => txtArguments.Text;
                    set => txtArguments.Text = value;
                }

                readonly Label lblCommand = new Label
                {
                    Text = AppString.Dialog.ItemCommand,
                    AutoSize = true
                };
                readonly Label lblArguments = new Label
                {
                    Text = AppString.Dialog.CommandArguments,
                    AutoSize = true
                };
                readonly TextBox txtCommand = new TextBox();
                readonly TextBox txtArguments = new TextBox();
                readonly Button btnOk = new Button
                {
                    DialogResult = DialogResult.OK,
                    Text = AppString.Dialog.Ok,
                    AutoSize = true
                };
                readonly Button btnCancel = new Button
                {
                    DialogResult = DialogResult.Cancel,
                    Text = AppString.Dialog.Cancel,
                    AutoSize = true
                };

                private void InitializeComponents()
                {
                    this.Controls.AddRange(new Control[] { lblCommand, lblArguments, txtCommand, txtArguments, btnOk, btnCancel });
                    int a = 20.DpiZoom();
                    lblArguments.Left = lblCommand.Left = lblCommand.Top = txtCommand.Top = a;
                    lblArguments.Top = txtArguments.Top = txtCommand.Bottom + a;
                    btnOk.Top = btnCancel.Top = txtArguments.Bottom + a;
                    int b = Math.Max(lblCommand.Width, lblArguments.Width) + 3 * a;
                    this.ClientSize = new Size(250.DpiZoom() + b, btnOk.Bottom + a);
                    btnOk.Anchor = btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
                    btnOk.Left = btnCancel.Left - btnOk.Width - a;
                    this.Resize += (sender, e) =>
                    {
                        txtArguments.Width = txtCommand.Width = this.ClientSize.Width - b;
                        txtArguments.Left = txtCommand.Left = btnCancel.Right - txtCommand.Width;
                    };
                    this.OnResize(null);
                    this.MinimumSize = this.Size;
                }
            }
        }
    }
}