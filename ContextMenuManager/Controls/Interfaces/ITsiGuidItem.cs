using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls.Interfaces
{
    interface ITsiGuidItem
    {
        Guid Guid { get; }
        string ItemText { get; }
        HandleGuidMenuItem TsiHandleGuid { get; set; }
        DetailedEditButton BtnDetailedEdit { get; set; }
    }

    sealed class HandleGuidMenuItem : ToolStripMenuItem
    {
        public HandleGuidMenuItem(ITsiGuidItem item) : base(AppString.Menu.HandleGuid)
        {
            this.Item = item;
            this.DropDownItems.AddRange(new ToolStripItem[] { TsiAddGuidDic,
                new ToolStripSeparator(), TsiCopyGuid, TsiBlockGuid, TsiClsidLocation });
            TsiCopyGuid.Click += (sender, e) => CopyGuid();
            TsiBlockGuid.Click += (sender, e) => BlockGuid();
            TsiAddGuidDic.Click += (sender, e) => AddGuidDic();
            TsiClsidLocation.Click += (sender, e) => OpenClsidPath();
            ((MyListItem)item).ContextMenuStrip.Opening += (sender, e) => RefreshMenuItem();
        }

        readonly ToolStripMenuItem TsiCopyGuid = new ToolStripMenuItem(AppString.Menu.CopyGuid);
        readonly ToolStripMenuItem TsiBlockGuid = new ToolStripMenuItem(AppString.Menu.BlockGuid);
        readonly ToolStripMenuItem TsiAddGuidDic = new ToolStripMenuItem(AppString.Menu.AddGuidDic);
        readonly ToolStripMenuItem TsiClsidLocation = new ToolStripMenuItem(AppString.Menu.ClsidLocation);

        public ITsiGuidItem Item { get; set; }

        private void CopyGuid()
        {
            string guid = Item.Guid.ToString("B");
            Clipboard.SetText(guid);
            AppMessageBox.Show($"{AppString.Message.CopiedToClipboard}\n{guid}",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BlockGuid()
        {
            foreach(string path in GuidBlockedList.BlockedPaths)
            {
                if(TsiBlockGuid.Checked)
                {
                    RegistryEx.DeleteValue(path, Item.Guid.ToString("B"));
                }
                else
                {
                    if(Item.Guid.Equals(ShellExItem.LnkOpenGuid) && AppConfig.ProtectOpenItem)
                    {
                        if(AppMessageBox.Show(AppString.Message.PromptIsOpenItem,
                            MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    }
                    Microsoft.Win32.Registry.SetValue(path, Item.Guid.ToString("B"), string.Empty);
                }
            }
            ExplorerRestarter.Show();
        }

        private void AddGuidDic()
        {
            using(AddGuidDicDialog dlg = new AddGuidDicDialog())
            {
                dlg.ItemText = GuidInfo.GetText(Item.Guid);
                dlg.ItemIcon = GuidInfo.GetImage(Item.Guid);
                var location = GuidInfo.GetIconLocation(Item.Guid);
                dlg.ItemIconPath = location.IconPath;
                dlg.ItemIconIndex = location.IconIndex;
                IniWriter writer = new IniWriter
                {
                    FilePath = AppConfig.UserGuidInfosDic,
                    DeleteFileWhenEmpty = true
                };
                string section = Item.Guid.ToString();
                MyListItem listItem = (MyListItem)Item;
                if(dlg.ShowDialog() != DialogResult.OK)
                {
                    if(dlg.IsDelete)
                    {
                        writer.DeleteSection(section);
                        GuidInfo.RemoveDic(Item.Guid);
                        listItem.Text = Item.ItemText;
                        listItem.Image = GuidInfo.GetImage(Item.Guid);
                    }
                    return;
                }
                if(dlg.ItemText.IsNullOrWhiteSpace())
                {
                    AppMessageBox.Show(AppString.Message.TextCannotBeEmpty);
                    return;
                }
                dlg.ItemText = ResourceString.GetDirectString(dlg.ItemText);
                if(dlg.ItemText.IsNullOrWhiteSpace())
                {
                    AppMessageBox.Show(AppString.Message.StringParsingFailed);
                    return;
                }
                else
                {
                    GuidInfo.RemoveDic(Item.Guid);
                    writer.SetValue(section, "Text", dlg.ItemText);
                    writer.SetValue(section, "Icon", dlg.ItemIconLocation);
                    listItem.Text = dlg.ItemText;
                    listItem.Image = dlg.ItemIcon;
                }
            }
        }

        private void OpenClsidPath()
        {
            string clsidPath = GuidInfo.GetClsidPath(Item.Guid);
            ExternalProgram.JumpRegEdit(clsidPath, null, AppConfig.OpenMoreRegedit);
        }

        private void RefreshMenuItem()
        {
            TsiClsidLocation.Visible = GuidInfo.GetClsidPath(Item.Guid) != null;
            TsiBlockGuid.Visible = TsiBlockGuid.Checked = false;
            if(Item is ShellExItem)
            {
                TsiBlockGuid.Visible = true;
                foreach(string path in GuidBlockedList.BlockedPaths)
                {
                    if(Microsoft.Win32.Registry.GetValue(path, Item.Guid.ToString("B"), null) != null)
                    {
                        TsiBlockGuid.Checked = true; break;
                    }
                }
            }
        }

        sealed class AddGuidDicDialog : CommonDialog
        {
            public Image ItemIcon { get; set; }
            public string ItemText { get; set; }
            public bool IsDelete { get; private set; }
            public string ItemIconPath { get; set; }
            public int ItemIconIndex { get; set; }
            public string ItemIconLocation
            {
                get
                {
                    if(ItemIconPath == null) return null;
                    return $"{ItemIconPath},{ItemIconIndex}";
                }
            }

            public override void Reset() { }

            protected override bool RunDialog(IntPtr hwndOwner)
            {
                using(AddGuidDicForm frm = new AddGuidDicForm())
                {
                    frm.ItemText = this.ItemText;
                    frm.ItemIcon = this.ItemIcon;
                    frm.ItemIconPath = this.ItemIconPath;
                    frm.ItemIconIndex = this.ItemIconIndex;
                    frm.TopMost = AppConfig.TopMost;
                    bool flag = frm.ShowDialog() == DialogResult.OK;
                    if(flag)
                    {
                        this.ItemText = frm.ItemText;
                        this.ItemIcon = frm.ItemIcon;
                        this.ItemIconPath = frm.ItemIconPath;
                        this.ItemIconIndex = frm.ItemIconIndex;
                    }
                    this.IsDelete = frm.IsDelete;
                    return flag;
                }
            }

            sealed class AddGuidDicForm : Form
            {
                public AddGuidDicForm()
                {
                    this.AcceptButton = btnOK;
                    this.CancelButton = btnCancel;
                    this.Font = SystemFonts.MenuFont;
                    this.Text = AppString.Dialog.AddGuidDic;
                    this.ShowIcon = this.ShowInTaskbar = false;
                    this.MaximizeBox = this.MinimizeBox = false;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    this.StartPosition = FormStartPosition.CenterParent;
                    InitializeComponents();
                }

                public string ItemText
                {
                    get => txtName.Text;
                    set => txtName.Text = value;
                }
                public Image ItemIcon
                {
                    get => picIcon.Image;
                    set => picIcon.Image = value;
                }
                public string ItemIconPath { get; set; }
                public int ItemIconIndex { get; set; }
                public bool IsDelete { get; private set; }

                readonly TextBox txtName = new TextBox();
                readonly Label lblName = new Label
                {
                    Text = AppString.Dialog.ItemText,
                    AutoSize = true
                };
                readonly Label lblIcon = new Label
                {
                    Text = AppString.Menu.ItemIcon,
                    AutoSize = true
                };
                readonly PictureBox picIcon = new PictureBox
                {
                    Size = SystemInformation.IconSize
                };
                readonly Button btnBrowse = new Button
                {
                    Text = AppString.Dialog.Browse,
                    AutoSize = true
                };
                readonly Button btnOK = new Button
                {
                    Text = ResourceString.OK,
                    DialogResult = DialogResult.OK,
                    AutoSize = true
                };
                readonly Button btnCancel = new Button
                {
                    Text = ResourceString.Cancel,
                    DialogResult = DialogResult.Cancel,
                    AutoSize = true
                };
                readonly Button btnDelete = new Button
                {
                    Text = AppString.Dialog.DeleteGuidDic,
                    DialogResult = DialogResult.Cancel,
                    AutoSize = true
                };

                private void InitializeComponents()
                {
                    this.Controls.AddRange(new Control[] { lblName, txtName, lblIcon, picIcon, btnBrowse, btnDelete, btnOK, btnCancel });
                    int a = 20.DpiZoom();
                    lblName.Left = lblName.Top = lblIcon.Left = btnDelete.Left = txtName.Top = a;
                    txtName.Left = lblName.Right + a;
                    btnOK.Left = btnDelete.Right + a;
                    btnCancel.Left = btnOK.Right + a;
                    txtName.Width = btnCancel.Right - txtName.Left;
                    btnBrowse.Left = btnCancel.Right - btnBrowse.Width;
                    picIcon.Left = btnOK.Left + (btnOK.Width - picIcon.Width) / 2;
                    btnBrowse.Top = txtName.Bottom + a;
                    picIcon.Top = btnBrowse.Top + (btnBrowse.Height - picIcon.Height) / 2;
                    lblIcon.Top = btnBrowse.Top + (btnBrowse.Height - lblIcon.Height) / 2;
                    btnDelete.Top = btnOK.Top = btnCancel.Top = btnBrowse.Bottom + a;
                    this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
                    ToolTipBox.SetToolTip(btnDelete, AppString.Tip.DeleteGuidDic);
                    btnBrowse.Click += (sender, e) => SelectIcon();
                    btnDelete.Click += (sender, e) => this.IsDelete = true;
                }

                private void SelectIcon()
                {
                    using(IconDialog dlg = new IconDialog())
                    {
                        dlg.IconPath = this.ItemIconPath;
                        dlg.IconIndex = this.ItemIconIndex;
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        using(Icon icon = ResourceIcon.GetIcon(dlg.IconPath, dlg.IconIndex))
                        {
                            Image image = icon?.ToBitmap();
                            if(image == null) return;
                            picIcon.Image = image;
                            this.ItemIconPath = dlg.IconPath;
                            this.ItemIconIndex = dlg.IconIndex;
                        }
                    }
                }
            }
        }
    }

    sealed class DetailedEditButton : PictureButton
    {
        public DetailedEditButton(ITsiGuidItem item) : base(AppImage.SubItems)
        {
            MyListItem listItem = (MyListItem)item;
            listItem.AddCtr(this);
            ToolTipBox.SetToolTip(this, AppString.SideBar.DetailedEdit);
            listItem.ParentChanged += (sender, e) =>
            {
                if(listItem.IsDisposed) return;
                if(listItem.Parent == null) return;
                this.Visible = XmlDicHelper.DetailedEditGuidDic.ContainsKey(item.Guid);
            };
            this.MouseDown += (sender, e) =>
            {
                using(DetailedEditDialog dlg = new DetailedEditDialog())
                {
                    dlg.GroupGuid = item.Guid;
                    dlg.ShowDialog();
                }
            };
        }
    }
}