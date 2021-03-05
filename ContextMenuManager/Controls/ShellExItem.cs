using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellExItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem, IFoldSubItem, ITsiGuidItem,
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
                        try
                        {
                            using(RegistryKey key = cmKey.OpenSubKey(keyName))
                            {
                                if(!GuidEx.TryParse(key.GetValue("")?.ToString(), out Guid guid))
                                    GuidEx.TryParse(keyName, out guid);
                                if(!guid.Equals(Guid.Empty))
                                    dic.Add(key.Name, guid);
                            }
                        }
                        catch { continue; }
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
        public string ItemText => GuidInfo.GetText(Guid) ?? (KeyName.Equals(Guid.ToString("B"), StringComparison.OrdinalIgnoreCase) ? DefaultValue : KeyName);
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
        public HandleGuidMenuItem TsiHandleGuid { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

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
            TsiHandleGuid = new HandleGuidMenuItem(this, true);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiHandleGuid, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation, TsiRegExport});

            ContextMenuStrip.Opening += (sender, e) => TsiDeleteMe.Enabled = !(IsOpenLnkItem && AppConfig.ProtectOpenItem);
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