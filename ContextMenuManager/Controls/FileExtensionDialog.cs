using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Collections.Generic;

namespace ContextMenuManager.Controls
{
    sealed class FileExtensionDialog : SelectDialog
    {
        public string Extension
        {
            get => Selected.Trim();
            set => Selected = value?.Trim();
        }

        public FileExtensionDialog()
        {
            this.CanEdit = true;
            this.Title = AppString.Dialog.SelectExtension;
            List<string> items = new List<string>();
            using(var key = RegistryEx.GetRegistryKey(FileExtension.FileExtsPath))
            {
                if(key != null)
                {
                    foreach(string keyName in key.GetSubKeyNames())
                    {
                        if(keyName.StartsWith(".")) items.Add(keyName.Substring(1));
                    }
                }
            }
            this.Items = items.ToArray();
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool flag = base.RunDialog(hwndOwner);
            if(flag)
            {
                string extension = ObjectPath.RemoveIllegalChars(this.Extension);
                int index = extension.LastIndexOf('.');
                if(index >= 0) this.Extension = extension.Substring(index);
                else this.Extension = $".{extension}";
            }
            return flag;
        }
    }
}
