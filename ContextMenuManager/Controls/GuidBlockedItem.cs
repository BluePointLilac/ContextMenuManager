using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using System;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class GuidBlockedItem : MyListItem, IBtnDeleteItem, ITsiWebSearchItem, ITsiFilePathItem, ITsiGuidItem
    {
        public GuidBlockedItem(string value)
        {
            InitializeComponents();
            this.Value = value;
            if(GuidEx.TryParse(value, out Guid guid))
            {
                this.Guid = guid;
                this.Image = GuidInfo.GetImage(guid);
                this.ItemFilePath = GuidInfo.GetFilePath(Guid);
            }
            else
            {
                this.Guid = Guid.Empty;
                this.Image = AppImage.DllDefaultIcon;
            }
            this.Text = this.ItemText;
        }

        public string Value { get; set; }
        public Guid Guid { get; set; }

        public string ItemText
        {
            get
            {
                string text;
                if(GuidEx.TryParse(Value, out Guid guid))
                {
                    text = GuidInfo.GetText(guid);
                }
                else
                {
                    text = AppString.MessageBox.MalformedGuid;
                }
                text += "\n" + Value;
                return text;
            }
        }


        public string SearchText => Value;
        public string ItemFilePath { get; set; }
        public DeleteButton BtnDelete { get; set; }
        public ObjectPathButton BtnOpenPath { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public FileLocationMenuItem TsiFileLocation { get; set; }
        public FilePropertiesMenuItem TsiFileProperties { get; set; }
        public HandleGuidMenuItem TsiHandleGuid { get; set; }

        readonly ToolStripMenuItem TsiDetails = new ToolStripMenuItem(AppString.Menu.Details);

        private void InitializeComponents()
        {
            BtnDelete = new DeleteButton(this);
            ContextMenuStrip = new ContextMenuStrip();
            TsiSearch = new WebSearchMenuItem(this);
            TsiFileProperties = new FilePropertiesMenuItem(this);
            TsiFileLocation = new FileLocationMenuItem(this);
            TsiHandleGuid = new HandleGuidMenuItem(this, false);

            ContextMenuStrip.Items.AddRange(new ToolStripItem[] {TsiHandleGuid,
                new ToolStripSeparator(), TsiDetails });
            TsiDetails.DropDownItems.AddRange(new ToolStripItem[] { TsiSearch,
                new ToolStripSeparator(), TsiFileProperties, TsiFileLocation});

            MyToolTip.SetToolTip(BtnDelete, AppString.Menu.Delete);
        }

        public void DeleteMe()
        {
            Array.ForEach(GuidBlockedList.BlockedPaths, path => RegistryEx.DeleteValue(path, this.Value));
            if(!this.Guid.Equals(Guid.Empty)) ExplorerRestarter.Show();
            this.Dispose();
        }
    }
}