using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class SwitchDicList : MyList
    {
        public bool UseUserDic { get; set; }

        public virtual void LoadItems()
        {
            this.AddSwitchItem();
        }

        public void AddSwitchItem()
        {
            SwitchDicItem item = new SwitchDicItem { UseUserDic = this.UseUserDic };
            item.UseDicChanged += () =>
            {
                this.UseUserDic = item.UseUserDic;
                this.ClearItems();
                this.LoadItems();
            };
            this.AddItem(item);
        }
    }

    sealed class SwitchDicItem : MyListItem
    {
        public SwitchDicItem()
        {
            this.Text = AppString.Other.SwitchDictionaries;
            this.AddCtr(cmbDic);
            cmbDic.AutosizeDropDownWidth();
            cmbDic.Font = new Font(this.Font.FontFamily, this.Font.Size + 1F);
            cmbDic.Items.AddRange(new[] { AppString.Other.WebDictionaries, AppString.Other.UserDictionaries });
            cmbDic.SelectionChangeCommitted += (sender, e) =>
            {
                this.Focus();
                this.UseUserDic = cmbDic.SelectedIndex == 1;
            };
        }

        private bool? useUserDic = null;
        public bool UseUserDic
        {
            get => useUserDic == true;
            set
            {
                if(useUserDic == value) return;
                bool flag = useUserDic == null;
                useUserDic = value;
                this.Image = this.UseUserDic ? AppImage.User : AppImage.Web;
                cmbDic.SelectedIndex = value ? 1 : 0;
                if(!flag) UseDicChanged?.Invoke();
            }
        }

        public Action UseDicChanged;

        readonly ComboBox cmbDic = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };
    }
}