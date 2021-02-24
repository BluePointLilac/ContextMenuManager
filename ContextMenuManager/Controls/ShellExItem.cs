using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellExItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, IFoldSubItem,
        ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiRegDeleteItem, ITsiRegExportItem
    {
        public static Dictionary<string, Guid> GetPathAndGuids(string shellExPath, bool isDragDrop = false)
        {
            Dictionary<string, Guid> dic = new Dictionary<string, Guid>();
            string[] parts = isDragDrop ? DdhParts : CmhParts;
            foreach(string part in parts)
            {
                using(RegistryKey cmKey = RegistryEx.GetRegistryKey($@"{shellExPath}\{part}"))
                {
                    if(cmKey == null) continue;
                    foreach(string keyName in cmKey.GetSubKeyNames())
                    {
                        using(RegistryKey key = cmKey.OpenSubKey(keyName))
                        {
                            if(!GuidEx.TryParse(key.GetValue("")?.ToString(), out Guid guid))
                                GuidEx.TryParse(keyName, out guid);
                            if(!guid.Equals(Guid.Empty))
                                dic.Add(key.Name, guid);
                        }
                    }
                }
            }
            return dic;
        }

        public static readonly string[] DdhParts = { "DragDropHandlers", "-DragDropHandlers" };
        public static readonly string[] CmhParts = { "ContextMenuHandlers", "-ContextMenuHandlers" };
        private const string LnkOpenGuid = "00021401-0000-0000-c000-000000000046";

        public ShellExItem(Guid guid, string regPath)
        {
            InitializeComponents();
            this.Guid = guid;
            this.RegPath = regPath;
        }

        private string regPath;
        public string RegPath
        {
            get => regPath;
            set
            {
                regPath = value;
                this.Text = this.ItemText;
                this.Image = GuidInfo.GetImage(Guid);
                ChkVisible.Checked = this.ItemVisible;
            }
        }

        public string ValueName => null;
        public Guid Guid { get; set; }
        public string SearchText => Text;
        public string ItemFilePath => GuidInfo.GetFilePath(Guid);
        private string KeyName => RegistryEx.GetKeyName(RegPath);
        private string ParentPath => RegistryEx.GetParentPath(RegPath);
        private string ShellExPath => RegistryEx.GetParentPath(ParentPath);
        private string ParentKeyName => RegistryEx.GetKeyName(ParentPath);
        private string DefaultValue => Registry.GetValue(RegPath, "", null)?.ToString();
        public string ItemText => GuidInfo.GetText(Guid) ?? ((Guid.ToString("B") == KeyName) ? DefaultValue : KeyName);
        private GuidInfo.IconLocation IconLocation => GuidInfo.GetIconLocation(this.Guid);
        private bool IsOpenLnkItem => Guid.ToString() == LnkOpenGuid;
        public bool IsDragDropItem => ParentKeyName.EndsWith(DdhParts[0], StringComparison.OrdinalIgnoreCase);

        private string BackupPath
        {
            get
            {
                string[] parts = IsDragDropItem ? DdhParts : CmhParts;
                return $@"{ShellExPath}\{(ItemVisible ? parts[1] : parts[0])}\{KeyName}";
            }
        }

        public bool ItemVisible
        {
            get
            {
                string[] parts = IsDragDropItem ? DdhParts : CmhParts;
                return ParentKeyName.Equals(parts[0], StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                if(!value && TryProtectOpenItem()) return;
                try
                {
                    RegistryEx.MoveTo(RegPath, BackupPath);
                }
                catch
                {
                    MessageBoxEx.Show(AppString.MessageBox.AuthorityProtection);
                    return;
                }
                RegPath = BackupPath;
            }
        }

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        public RegExportMenuItem TsiRegExport { get; set; }
        public IFoldGroupItem FoldGroupItem { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiHandleGuid = new ToolStripMenuItem(AppString.Menu.HandleGuid);
        readonly ToolStripMenuItem TsiCopyGuid = new ToolStripMenuItem(AppString.Menu.CopyGuid);
        readonly ToolStripMenuItem TsiBlockGuid = new ToolStripMenuItem(AppString.Menu.BlockGuid);
        readonly ToolStripMenuItem TsiAddGuidDic = new ToolStripMenuItem(AppString.Menu.AddGuidDic);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiRegExport = new RegExportMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiHandleGuid, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiHandleGuid.DropDownItems.AddRange(new ToolStripItem[] { TsiCopyGuid, new ToolStripSeparator(),
                TsiBlockGuid, new ToolStripSeparator(), TsiAddGuidDic });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport});

            ContextMenuStrip.Opening += (sender, e) => RefreshMenuItem();
            TsiCopyGuid.Click += (sender, e) => CopyGuid();
            TsiBlockGuid.Click += (sender, e) => BlockGuid();
            TsiAddGuidDic.Click += (sender, e) => AddGuidDic();
        }

        private void CopyGuid()
        {
            Clipboard.SetText(Guid.ToString());
            MessageBoxEx.Show($"{AppString.MessageBox.CopiedToClipboard}\n{Guid}",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BlockGuid()
        {
            foreach(string path in GuidBlockedItem.BlockedPaths)
            {
                if(TsiBlockGuid.Checked)
                {
                    RegistryEx.DeleteValue(path, this.Guid.ToString("B"));
                }
                else
                {
                    Registry.SetValue(path, this.Guid.ToString("B"), string.Empty);
                }
            }
            ExplorerRestarter.Show();
        }

        private void AddGuidDic()
        {
            using(AddGuidDicDialog dlg = new AddGuidDicDialog())
            {
                dlg.ItemName = this.Text;
                dlg.ItemIcon = this.Image;
                dlg.ItemIconPath = this.IconLocation.IconPath;
                dlg.ItemIconIndex = this.IconLocation.IconIndex;
                IniWriter writer = new IniWriter
                {
                    FilePath = AppConfig.UserGuidInfosDic,
                    DeleteFileWhenEmpty = true
                };
                string section = this.Guid.ToString();
                if(dlg.ShowDialog() != DialogResult.OK)
                {
                    if(dlg.IsDelete)
                    {
                        writer.DeleteSection(section);
                        GuidInfo.ItemTextDic.Remove(this.Guid);
                        GuidInfo.ItemImageDic.Remove(this.Guid);
                        GuidInfo.IconLocationDic.Remove(this.Guid);
                        GuidInfo.UserDic.RootDic.Remove(section);
                        this.Text = this.ItemText;
                        this.Image = GuidInfo.GetImage(Guid);
                    }
                    return;
                }
                string name = ResourceString.GetDirectString(dlg.ItemName);
                if(!name.IsNullOrWhiteSpace())
                {
                    writer.SetValue(section, "Text", dlg.ItemName);
                    this.Text = name;
                    if(GuidInfo.ItemTextDic.ContainsKey(this.Guid))
                    {
                        GuidInfo.ItemTextDic[this.Guid] = this.Text;
                    }
                    else
                    {
                        GuidInfo.ItemTextDic.Add(this.Guid, this.Text);
                    }
                }
                else
                {
                    MessageBoxEx.Show(AppString.MessageBox.StringParsingFailed);
                    return;
                }
                if(dlg.ItemIconLocation != null)
                {
                    writer.SetValue(section, "Icon", dlg.ItemIconLocation);
                    var location = new GuidInfo.IconLocation { IconPath = dlg.ItemIconPath, IconIndex = dlg.ItemIconIndex };
                    if(GuidInfo.IconLocationDic.ContainsKey(this.Guid))
                    {
                        GuidInfo.IconLocationDic[this.Guid] = location;
                    }
                    else
                    {
                        GuidInfo.IconLocationDic.Add(this.Guid, location);
                    }
                    this.Image = dlg.ItemIcon;
                    if(GuidInfo.ItemImageDic.ContainsKey(this.Guid))
                    {
                        GuidInfo.ItemImageDic[this.Guid] = this.Image;
                    }
                    else
                    {
                        GuidInfo.ItemImageDic.Add(this.Guid, this.Image);
                    }
                }
            }
        }

        private void RefreshMenuItem()
        {
            TsiDeleteMe.Enabled = !(IsOpenLnkItem && AppConfig.ProtectOpenItem);
            TsiBlockGuid.Checked = false;
            foreach(string path in GuidBlockedItem.BlockedPaths)
            {
                if(Registry.GetValue(path, this.Guid.ToString("B"), null) != null)
                {
                    TsiBlockGuid.Checked = true;
                    break;
                }
            }
        }

        private bool TryProtectOpenItem()
        {
            if(!IsOpenLnkItem) return false;
            if(!AppConfig.ProtectOpenItem) return false;
            return MessageBoxEx.Show(AppString.MessageBox.PromptIsOpenItem, MessageBoxButtons.YesNo) != DialogResult.Yes;
        }

        public void DeleteMe()
        {
            try
            {
                RegistryEx.DeleteKeyTree(this.RegPath, true);
                RegistryEx.DeleteKeyTree(this.BackupPath);
            }
            catch
            {
                MessageBoxEx.Show(AppString.MessageBox.AuthorityProtection);
                return;
            }
            this.Dispose();
        }
    }
}