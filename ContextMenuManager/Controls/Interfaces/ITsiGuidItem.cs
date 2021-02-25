using BluePointLilac.Controls;
using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
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
        public HandleGuidMenuItem(ITsiGuidItem item, bool isShellExItem) : base(AppString.Menu.HandleGuid)
        {
            this.Item = item;
            this.DropDownItems.Add(TsiCopyGuid);
            if(isShellExItem)
            {
                this.DropDownItems.Add(new ToolStripSeparator());
                this.DropDownItems.Add(TsiBlockGuid);
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            this.DropDownItems.Add(TsiAddGuidDic);
            TsiCopyGuid.Click += (sender, e) => CopyGuid();
            TsiBlockGuid.Click += (sender, e) => BlockGuid();
            TsiAddGuidDic.Click += (sender, e) => AddGuidDic();
            ((Control)item).ContextMenuStrip.Opening += (sender, e) =>
            {
                TsiBlockGuid.Checked = false;
                foreach(string path in GuidBlockedList.BlockedPaths)
                {
                    if(Registry.GetValue(path, Item.Guid.ToString("B"), null) != null)
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

        public ITsiGuidItem Item { get; set; }

        private void CopyGuid()
        {
            Clipboard.SetText(Item.Guid.ToString());
            MessageBoxEx.Show($"{AppString.MessageBox.CopiedToClipboard}\n{Item.Guid}",
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
                    Registry.SetValue(path, Item.Guid.ToString("B"), string.Empty);
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
                    MessageBoxEx.Show(AppString.MessageBox.StringParsingFailed);
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
    }
}