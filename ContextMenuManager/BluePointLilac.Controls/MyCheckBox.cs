using ContextMenuManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BluePointLilac.Controls
{
    public class MyCheckBox : PictureBox
    {
        private bool? _Checked = null;
        public bool Checked
        {
            get => _Checked == true;
            set
            {
                if(_Checked == value) return;
                bool notFirst = _Checked != null;
                this.Image = SwitchImage(value);
                if(_Checked == null)
                {
                    _Checked = value;
                    return;
                }
                if(PreCheckChanging != null && !PreCheckChanging.Invoke())
                {
                    this.Image = SwitchImage(!value);
                    return;
                }
                else CheckChanging?.Invoke();
                if(PreCheckChanged != null && !PreCheckChanged.Invoke())
                {
                    this.Image = SwitchImage(!value);
                    return;
                }
                else
                {
                    _Checked = value;
                    CheckChanged?.Invoke();
                }
            }
        }

        public MyCheckBox()
        {
            this.Image = SwitchImage(false);
            this.Cursor = Cursors.Hand;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        public Func<bool> PreCheckChanging { get; set; }
        public Func<bool> PreCheckChanged { get; set; }
        public Action CheckChanging { get; set; }
        public Action CheckChanged { get; set; }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Left) this.Checked = !this.Checked;
        }

        private static Image SwitchImage(bool value)
        {
            return value ? AppImage.TurnOn : AppImage.TurnOff;
        }
    }
}