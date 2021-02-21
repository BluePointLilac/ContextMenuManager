using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class GuidBlockedItem : MyListItem, IBtnDeleteItem, ITsiWebSearchItem, ITsiFilePathItem
    {
        public const string HKLMBLOCKED = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public const string HKCUBLOCKED = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public static readonly string[] BlockedPaths = { HKLMBLOCKED, HKCUBLOCKED };

        public GuidBlockedItem(string value)
        {
            InitializeComponents();
            this.Value = value;
            if(GuidEx.TryParse(value, out Guid guid))
            {
                this.Guid = guid;
                this.Text = GuidInfo.GetText(guid);
                this.Image = GuidInfo.GetImage(guid);
                this.ItemFilePath = GuidInfo.GetFilePath(Guid);
            }
            else
            {
                this.Guid = Guid.Empty;
                this.Text = AppString.MessageBox.MalformedGuid;
                this.Image = AppImage.DllDefaultIcon;
            }
            this.Text += "\n" + value;
        }

        public string Value { get; set; }
        public Guid Guid { get; set; }

        public DeleteButton BtnDelete { get; set; }
        public ObjectPathButton BtnOpenPath { get; set; }

        public string SearchText => Value;
        public string ItemFilePath { get; set; }

        public WebSearchMenuItem TsiSearch { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public string RegPath { get; set; }

        private void InitializeComponents()
        {
            BtnDelete = new DeleteButton(this);
            ContextMenuStrip = new ContextMenuStrip();
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiSearch,
                new ToolStripSeparator(), TsiFileProperties, TsiFileLocation });

            MyToolTip.SetToolTip(BtnDelete, AppString.Menu.Delete);
        }

        public void DeleteMe()
        {
            Array.ForEach(BlockedPaths, path => { RegistryEx.DeleteValue(path, this.Value); });
            if(!this.Guid.Equals(Guid.Empty)) ExplorerRestarter.Show();
            this.Dispose();
        }
    }
}