using BluePointLilac.Controls;
using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class GuidBlockedList : MyList
    {
        public const string HKLMBLOCKED = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public const string HKCUBLOCKED = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public static readonly string[] BlockedPaths = { HKLMBLOCKED, HKCUBLOCKED };

        public void LoadItems()
        {
            this.AddNewItem();
            this.LoadBlockedItems();
        }

        private void LoadBlockedItems()
        {
            List<string> values = new List<string>();
            Array.ForEach(BlockedPaths, path =>
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(path))
                    if(key != null) values.AddRange(key.GetValueNames());
            });
            Array.ForEach(values.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(), value =>
                this.AddItem(new GuidBlockedItem(value))
            );
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem(AppString.Item.AddGuidBlockedItem);
            this.AddItem(newItem);
            newItem.AddNewItem += (sender, e) =>
            {
                using(InputDialog dlg = new InputDialog { Title = AppString.Dialog.InputGuid })
                {
                    if(GuidEx.TryParse(Clipboard.GetText(), out Guid guid)) dlg.Text = guid.ToString();
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    if(GuidEx.TryParse(dlg.Text, out guid))
                    {
                        string value = guid.ToString("B");
                        Array.ForEach(BlockedPaths, path => Registry.SetValue(path, value, ""));
                        for(int i = 1; i < Controls.Count; i++)
                        {
                            if(((GuidBlockedItem)Controls[i]).Guid.Equals(guid))
                            {
                                MessageBoxEx.Show(AppString.MessageBox.HasBeenAdded);
                                return;
                            }
                        }
                        this.InsertItem(new GuidBlockedItem(value), 1);
                        ExplorerRestarter.Show();
                    }
                    else MessageBoxEx.Show(AppString.MessageBox.MalformedGuid);
                }
            };
        }
    }
}