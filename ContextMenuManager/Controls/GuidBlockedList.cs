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
            this.ClearItems();
            this.AddNewItem();
            this.LoadCommonItems();
        }

        private void LoadCommonItems()
        {
            List<string> values = new List<string>();
            Array.ForEach(GuidBlockedItem.BlockedPaths, path =>
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(path))
                    if(key != null) values.AddRange(key.GetValueNames());
            });
            Array.ForEach(values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(), value =>
            {
                if(GuidInfo.TryGetGuid(value, out Guid guid, out string guidPath))
                    this.AddItem(new GuidBlockedItem(guid, guidPath));
            });
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem { Text = AppString.Item.AddGuidBlockedItem };
            this.AddItem(newItem);
            newItem.NewItemAdd += (sender, e) =>
            {
                using(InputDialog dlg = new InputDialog { Title = AppString.Dialog.InputGuid })
                {
                    if(GuidInfo.TryGetGuid(Clipboard.GetText(), out Guid guid)) dlg.Text = guid.ToString();
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    if(GuidInfo.TryGetGuid(dlg.Text, out guid, out string guidPath))
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
                        this.InsertItem(new GuidBlockedItem(guid, guidPath), 1);
                        ExplorerRestarter.NeedRestart = true;
                    }
                    else MessageBoxEx.Show(AppString.MessageBox.UnknownGuid);
                }
            };
        }
    }
}