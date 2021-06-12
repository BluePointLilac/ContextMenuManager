using BluePointLilac.Controls;
using BluePointLilac.Methods;
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
    }

    sealed class HandleGuidMenuItem : ToolStripMenuItem
    {
        public HandleGuidMenuItem(ITsiGuidItem item) : base(AppString.Menu.HandleGuid)
        {
            this.Item = item;
            this.DropDownItems.AddRange(new ToolStripItem[] {
                TsiAddGuidDic, new ToolStripSeparator(), TsiCopyGuid });
            if(item is ShellExItem shellExItem)
            {
                this.DropDownItems.AddRange(new ToolStripItem[] { TsiBlockGuid, TsiClsidLocation });
                shellExItem.ContextMenuStrip.Opening += (sender, e) => TsiClsidLocation.Visible = shellExItem.ClsidPath != null;
                TsiClsidLocation.Click += (sender, e) => ExternalProgram.JumpRegEdit(shellExItem.ClsidPath);
            }
            TsiCopyGuid.Click += (sender, e) => CopyGuid();
            TsiBlockGuid.Click += (sender, e) => BlockGuid();
            TsiAddGuidDic.Click += (sender, e) => AddGuidDic();
            MyListItem listItem = (MyListItem)item;
            listItem.ImageDoubleClick += (sender, e) => AddGuidDic();
            listItem.TextDoubleClick += (sender, e) => AddGuidDic();
            listItem.ContextMenuStrip.Opening += (sender, e) =>
            {
                TsiBlockGuid.Checked = false;
                foreach(string path in GuidBlockedList.BlockedPaths)
                {
                    if(Microsoft.Win32.Registry.GetValue(path, Item.Guid.ToString("B"), null) != null)
                    {
                        TsiBlockGuid.Checked = true;
                        break;
                    }
                }
            };
        }

        readonly ToolStripMenuItem TsiCopyGuid = new ToolStripMenuItem(AppString.Menu.CopyGuid);
        readonly ToolStripMenuItem TsiBlockGuid = new ToolStripMenuItem(AppString.Menu.BlockGuid);
        readonly ToolStripMenuItem TsiAddGuidDic = new ToolStripMenuItem(AppString.Menu.AddGuidDic);
        readonly ToolStripMenuItem TsiClsidLocation = new ToolStripMenuItem(AppString.Menu.ClsidLocation);

        public ITsiGuidItem Item { get; set; }

        private void CopyGuid()
        {
            Clipboard.SetText(Item.Guid.ToString());
            MessageBoxEx.Show($"{AppString.Message.CopiedToClipboard}\n{Item.Guid}",
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
                if(dlg.ShowDialog() != DialogResult.OK)
                {
                    if(dlg.IsDelete)
                    {
                        writer.DeleteSection(section);
                        GuidInfo.ItemTextDic.Remove(Item.Guid);
                        GuidInfo.ItemImageDic.Remove(Item.Guid);
                        GuidInfo.IconLocationDic.Remove(Item.Guid);
                        GuidInfo.UserDic.RootDic.Remove(section);
                        ((MyListItem)Item).Text = Item.ItemText;
                        ((MyListItem)Item).Image = GuidInfo.GetImage(Item.Guid);
                    }
                    return;
                }
                string name = ResourceString.GetDirectString(dlg.ItemText);
                if(!name.IsNullOrWhiteSpace())
                {
                    writer.SetValue(section, "Text", dlg.ItemText);
                    ((MyListItem)Item).Text = name;
                    if(GuidInfo.ItemTextDic.ContainsKey(Item.Guid))
                    {
                        GuidInfo.ItemTextDic[Item.Guid] = name;
                    }
                    else
                    {
                        GuidInfo.ItemTextDic.Add(Item.Guid, name);
                    }
                }
                else
                {
                    MessageBoxEx.Show(AppString.Message.StringParsingFailed);
                    return;
                }
                if(dlg.ItemIconLocation != null)
                {
                    writer.SetValue(section, "Icon", dlg.ItemIconLocation);
                    location = new GuidInfo.IconLocation { IconPath = dlg.ItemIconPath, IconIndex = dlg.ItemIconIndex };
                    if(GuidInfo.IconLocationDic.ContainsKey(Item.Guid))
                    {
                        GuidInfo.IconLocationDic[Item.Guid] = location;
                    }
                    else
                    {
                        GuidInfo.IconLocationDic.Add(Item.Guid, location);
                    }
                     ((MyListItem)Item).Image = dlg.ItemIcon;
                    if(GuidInfo.ItemImageDic.ContainsKey(Item.Guid))
                    {
                        GuidInfo.ItemImageDic[Item.Guid] = dlg.ItemIcon;
                    }
                    else
                    {
                        GuidInfo.ItemImageDic.Add(Item.Guid, dlg.ItemIcon);
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
                    this.AcceptButton = btnOk;
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
                readonly Button btnOk = new Button
                {
                    Text = AppString.Dialog.Ok,
                    DialogResult = DialogResult.OK,
                    AutoSize = true
                };
                readonly Button btnCancel = new Button
                {
                    Text = AppString.Dialog.Cancel,
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
                    this.Controls.AddRange(new Control[] { lblName, txtName, lblIcon, picIcon, btnBrowse, btnDelete, btnOk, btnCancel });
                    int a = 20.DpiZoom();
                    lblName.Left = lblName.Top = lblIcon.Left = btnDelete.Left = txtName.Top = a;
                    txtName.Left = lblName.Right + a;
                    btnOk.Left = btnDelete.Right + a;
                    btnCancel.Left = btnOk.Right + a;
                    txtName.Width = btnCancel.Right - txtName.Left;
                    btnBrowse.Left = btnCancel.Right - btnBrowse.Width;
                    picIcon.Left = btnOk.Left + (btnOk.Width - picIcon.Width) / 2;
                    btnBrowse.Top = txtName.Bottom + a;
                    picIcon.Top = btnBrowse.Top + (btnBrowse.Height - picIcon.Height) / 2;
                    lblIcon.Top = btnBrowse.Top + (btnBrowse.Height - lblIcon.Height) / 2;
                    btnDelete.Top = btnOk.Top = btnCancel.Top = btnBrowse.Bottom + a;
                    this.ClientSize = new Size(btnCancel.Right + a, btnCancel.Bottom + a);
                    MyToolTip.SetToolTip(btnDelete, AppString.Tip.DeleteGuidDic);
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
                        ItemIconPath = dlg.IconPath;
                        ItemIconIndex = dlg.IconIndex;
                        picIcon.Image = ResourceIcon.GetIcon(ItemIconPath, ItemIconIndex).ToBitmap();
                    }
                }
            }
        }
    }
}