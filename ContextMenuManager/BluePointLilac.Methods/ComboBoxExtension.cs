using System;
using System.Windows.Forms;

namespace BluePointLilac.Methods
{
    public static class ComboBoxExtension
    {
        public static void AutosizeDropDownWidth(this ComboBox box)
        {
            box.DropDown += (sender, e) =>
            {
                int maxWidth = 0;
                foreach(var item in box.Items)
                {
                    maxWidth = Math.Max(maxWidth, TextRenderer.MeasureText(item.ToString(), box.Font).Width);
                }
                maxWidth = Math.Max(maxWidth, box.Width);
                box.DropDownWidth = maxWidth;
            };
        }
    }
}