using ContextMenuManager;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public class MyCheckBox : PictureBox
    {
        private bool _Checked;
        public bool Checked
        {
            get => _Checked;
            set
            {
                _Checked = value;
                Image = value ? AppImage.TurnOn : AppImage.TurnOff;
            }
        }

        public MyCheckBox()
        {
            this.Checked = false;
            this.Cursor = Cursors.Hand;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
        }
    }
}