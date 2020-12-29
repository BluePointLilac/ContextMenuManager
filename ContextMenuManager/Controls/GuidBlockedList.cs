using BulePointLilac.Controls;
using BulePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class GuidBlockedList : MyList
    {
        public void LoadItems()
        {
            this.AddNewItem();
            this.LoadBlockedItems();
        }

        private void LoadBlockedItems()
        {
            List<string> values = new List<string>();
            Array.ForEach(GuidBlockedItem.BlockedPaths, path =>
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(path))
                    if(key != null) values.AddRange(key.GetValueNames());
            });
            Array.ForEach(values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(), value =>
            {
                    this.AddItem(new GuidBlockedItem(value));
            });
        }

        private void AddNewItem()
        {
            MyListItem newItem = new MyListItem
            {
                Text = AppString.Item.AddGuidBlockedItem,
                Image = AppImage.AddNewItem
            };
            PictureButton btnAddNewItem = new PictureButton(AppImage.AddNewItem);
            newItem.AddCtr(btnAddNewItem);
            newItem.SetNoClickEvent();
            this.AddItem(newItem);
            MyToolTip.SetToolTip(btnAddNewItem, newItem.Text);
            btnAddNewItem.MouseDown += (sender, e) =>
            {
                using(InputDialog dlg = new InputDialog { Title = AppString.Dialog.InputGuid })
                {
                    if(GuidEx.TryParse(Clipboard.GetText(), out Guid guid)) dlg.Text = guid.ToString();
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    if(GuidEx.TryParse(dlg.Text, out guid))
                    {
                        Array.ForEach(GuidBlockedItem.BlockedPaths, path =>
                        {
                            Registry.SetValue(path, guid.ToString("B"), string.Empty);
                        });
                        for(int i = 1; i < Controls.Count; i++)
                        {
                            if(((GuidBlockedItem)Controls[i]).Guid.Equals(guid))
                            {
                                MessageBoxEx.Show(AppString.MessageBox.HasBeenAdded);
                                return;
                            }
                        }
                        this.InsertItem(new GuidBlockedItem(dlg.Text), 1);
                        ExplorerRestarter.NeedRestart = true;
                    }
                    else MessageBoxEx.Show(AppString.MessageBox.MalformedGuid);
                }
            };
        }
    }
}