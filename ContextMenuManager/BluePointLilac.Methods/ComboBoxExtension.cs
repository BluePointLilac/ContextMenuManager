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
            //ToolTip toolTip = new ToolTip() { AutoPopDelay = 0, InitialDelay = 0, ReshowDelay = 0, ShowAlways = true, };
            //box.DrawMode = DrawMode.OwnerDrawFixed;
            //box.DrawItem += (s, e) =>
            //{
            //    e.DrawBackground();
            //    string text = box.GetItemText(box.Items[e.Index]);
            //    using(SolidBrush br = new SolidBrush(e.ForeColor))
            //        e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            //    if((e.State & DrawItemState.Selected) == DrawItemState.Selected && box.DroppedDown)
            //        toolTip.Show(text, box, e.Bounds.Right, e.Bounds.Bottom + 4);
            //    e.DrawFocusRectangle();
            //};
            //box.DropDownClosed += (s, e) =>
            //    toolTip.Hide(box);
        }
    }
}