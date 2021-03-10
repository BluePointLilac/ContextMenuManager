using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class FileExtensionDialog : SelectDialog
    {
        public string Extension
        {
            get => Selected;
            set => Selected = value;
        }

        public FileExtensionDialog()
        {
            this.Title = AppString.Dialog.SelectExtension;
            this.DropDownStyle = ComboBoxStyle.DropDown;
            List<string> items = new List<string>();
            foreach(string keyName in Microsoft.Win32.Registry.ClassesRoot.GetSubKeyNames())
            {
                if(keyName.StartsWith(".")) items.Add(keyName.Substring(1));
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
