using ContextMenuManager;
using System.Windows.Forms;

namespace BulePointLilac.Methods
{
    public static class MessageBoxEx
    {
        public static DialogResult Show(string text, MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Warning, string caption = null)
        {
            return MessageBox.Show(text, caption ?? AppString.General.AppName, buttons, icon);
        }
    }
}