using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class NewItemForm : ResizeLimitedForm
    {
        public NewItemForm()
        {
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Text = AppString.Other.NewItem;
            this.Font = SystemFonts.MenuFont;
            this.MaximizeBox = this.MinimizeBox = false;
            this.ShowIcon = this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.VerticalResizable = false;
            InitializeComponents();
        }

        public string ItemText { get => txtText.Text; set => txtText.Text = value; }
        public string ItemFilePath { get => txtFilePath.Text; set => txtFilePath.Text = value; }
        public string Arguments { get => txtArguments.Text; set => txtArguments.Text = value; }
        public string ItemCommand
        {
            get
            {
                string filePath = ItemFilePath;
                string arguments = Arguments;
                if(arguments.IsNullOrWhiteSpace()) return filePath;
                if(filePath.IsNullOrWhiteSpace()) return arguments;
                if(filePath.Contains(" ")) filePath = $"\"{filePath}\"";
                if(!arguments.Contains("\"")) arguments = $"\"{arguments}\"";
                return $"{filePath} {arguments}";
            }
        }

        protected readonly Label lblText = new Label
        {
            Text = AppString.Dialog.ItemText,
            AutoSize = true
        };
        protected readonly Label lblCommand = new Label
        {
            Text = AppString.Dialog.ItemCommand,
            AutoSize = true
        };
        protected readonly Label lblArguments = new Label
        {
            Text = AppString.Dialog.CommandArguments,
            AutoSize = true
        };
        protected readonly TextBox txtText = new TextBox();
        protected readonly TextBox txtFilePath = new TextBox();
        protected readonly TextBox txtArguments = new TextBox();
        protected readonly Button btnBrowse = new Button
        {
            Text = AppString.Dialog.Browse,
            AutoSize = true
        };
        protected readonly Button btnOK = new Button
        {
            Text = ResourceString.OK,
            AutoSize = true
        };
        protected readonly Button btnCancel = new Button
        {
            DialogResult = DialogResult.Cancel,
            Text = ResourceString.Cancel,
            AutoSize = true
        };

        protected virtual void InitializeComponents()
        {
            this.Controls.AddRange(new Control[] { lblText, lblCommand, lblArguments,
                txtText, txtFilePath, txtArguments, btnBrowse, btnOK, btnCancel });
            int a = 20.DpiZoom();
            btnBrowse.Anchor = btnOK.Anchor = btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            txtText.Top = lblText.Top = lblText.Left = lblCommand.Left = lblArguments.Left = a;
            btnBrowse.Top = txtFilePath.Top = lblCommand.Top = txtText.Bottom + a;
            lblArguments.Top = txtArguments.Top = txtFilePath.Bottom + a;
            btnOK.Top = btnCancel.Top = txtArguments.Bottom + a;
            btnCancel.Left = btnBrowse.Left = this.ClientSize.Width - btnCancel.Width - a;
            btnOK.Left = btnCancel.Left - btnOK.Width - a;
            int b = Math.Max(Math.Max(lblText.Width, lblCommand.Width), lblArguments.Width) + btnBrowse.Width + 4 * a;
            this.ClientSize = new Size(320.DpiZoom() + b, btnOK.Bottom + a);
            this.MinimumSize = this.Size;
            this.Resize += (sender, e) =>
            {
                txtText.Width = txtFilePath.Width = txtArguments.Width = this.ClientSize.Width - b;
                txtText.Left = txtFilePath.Left = txtArguments.Left = btnBrowse.Left - txtFilePath.Width - a;
            };
            this.OnResize(null);
        }
    }
}