using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class GuidBlockedItem : MyListItem, IBtnDeleteItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiRegPathItem
    {
        const string HKLMBLOCKED = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        const string HKCUBLOCKED = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public static readonly string[] BlockedPaths = { HKLMBLOCKED, HKCUBLOCKED };

        public GuidBlockedItem(Guid guid, string guidPath)
        {
            InitializeComponents();
            this.Guid = guid;
            this.RegPath = guidPath;
        }

        private Guid guid;
        public Guid Guid
        {
            get => guid;
            set
            {
                guid = value;
                this.Text = $"{GuidInfo.GetText(value)}\n{value}";
                this.Image = GuidInfo.GetImage(value);
            }
        }

        public DeleteButton BtnDelete { get; set; }
        public ObjectPathButton BtnOpenPath { get; set; }

        public string SearchText => Guid.ToString();
        public string ItemFilePath => GuidInfo.GetFilePath(Guid);

        public WebSearchMenuItem TsiSearch { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }
        public string RegPath { get; set; }

        private void InitializeComponents()
        {
            BtnDelete = new DeleteButton(this);
            ContextMenuStrip = new ContextMenuStrip();
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiRegLocation = new RegLocationMenuItem(this);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiSearch, new ToolStripSeparator(),
                TsiFileProperties, TsiFileLocation, TsiRegLocation });

            MyToolTip.SetToolTip(BtnDelete, AppString.Menu.Delete);
        }

        public void DeleteMe()
        {
            Array.ForEach(BlockedPaths, path => { RegistryEx.DeleteValue(path, Guid.ToString("B")); });
            ExplorerRestarter.NeedRestart = true;
            this.Dispose();
        }
    }
}