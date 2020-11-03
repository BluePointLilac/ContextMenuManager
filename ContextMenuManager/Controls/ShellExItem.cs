using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellExItem : MyListItem, IChkVisibleItem, IBtnShowMenuItem,
        ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem, ITsiDeleteItem
    {
        public static Dictionary<string, Guid> GetPathAndGuids(string shellExPath)
        {
            Dictionary<string, Guid> dic = new Dictionary<string, Guid>();
            foreach(string cmhPart in CmhParts)
            {
                using(RegistryKey cmKey = RegistryEx.GetRegistryKey($@"{shellExPath}\{cmhPart}"))
                {
                    if(cmKey == null) continue;
                    foreach(string keyName in cmKey.GetSubKeyNames())
                    {
                        using(RegistryKey key = cmKey.OpenSubKey(keyName))
                        {
                            if(!GuidInfo.TryGetGuid(key.GetValue("")?.ToString(), out Guid guid))
                                GuidInfo.TryGetGuid(keyName, out guid);
                            if(!guid.Equals(Guid.Empty))
                                dic.Add(key.Name, guid);
                        }
                    }
                }
            }
            return dic;
        }

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

        public Guid Guid { get; set; }
        public string SearchText => Text;
        public string ItemFilePath => GuidInfo.GetFilePath(Guid);
        private string KeyName => RegistryEx.GetKeyName(RegPath);
        private string ShellExPath => RegistryEx.GetParentPath(RegistryEx.GetParentPath(RegPath));
        private string CmhKeyName => RegistryEx.GetKeyName(RegistryEx.GetParentPath(RegPath));
        private string DefaultValue => Registry.GetValue(RegPath, "", null)?.ToString();
        private string ItemText => GuidInfo.GetText(Guid) ?? ((Guid.ToString("B") == KeyName) ? DefaultValue : KeyName);
        private string BuckupPath => $@"{ShellExPath}\{(ItemVisible ? CmhParts[1] : CmhParts[0])}\{KeyName}";
        private bool IsOpenLnkItem => Guid.ToString() == LnkOpenGuid;
        private bool TryProtectOpenItem => IsOpenLnkItem && MessageBoxEx.Show
            (AppString.MessageBox.PromptIsOpenItem, MessageBoxButtons.YesNo) != DialogResult.Yes;

        public bool ItemVisible
        {
            get => CmhKeyName.Equals(CmhParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                if(!value && TryProtectOpenItem) return;
                using(RegistryKey srcKey = RegistryEx.GetRegistryKey(RegPath))
                using(RegistryKey dstKey = RegistryEx.GetRegistryKey(BuckupPath, true, true))
                    srcKey?.CopyTo(dstKey);
                RegistryEx.DeleteKeyTree(RegPath);
                RegPath = BuckupPath;
            }
        }

        public VisibleCheckBox ChkVisible { get; set; }
        public MenuButton BtnShowMenu { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public DeleteMeMenuItem TsiDeleteMe { get; set; }
        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);
        readonly ToolStripMenuItem TsiCopyGuid = new ToolStripMenuItem(AppString.Menu.CopyGuid);

        private void InitializeComponents()
        {
            BtnShowMenu = new MenuButton(this);
            ChkVisible = new VisibleCheckBox(this);
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);
            TsiDeleteMe = new DeleteMeMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] { TsiCopyGuid, new ToolStripSeparator(),
                TsiDetails, new ToolStripSeparator(), TsiDeleteMe });

            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation});

            ContextMenuStrip.Opening += (sender, e) => TsiDeleteMe.Enabled = !IsOpenLnkItem;
            TsiCopyGuid.Click += (sender, e) => CopyGuid();
        }

        private void CopyGuid()
        {
            Clipboard.SetText(Guid.ToString());
            MessageBoxEx.Show($"{AppString.MessageBox.CopiedToClipboard}:\n{Guid}",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void DeleteMe()
        {
            RegistryEx.DeleteKeyTree(this.RegPath);
            RegistryEx.DeleteKeyTree(this.BuckupPath);
            this.Dispose();
        }
    }
}