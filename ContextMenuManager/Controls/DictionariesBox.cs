using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class DictionariesBox : TabControl
    {
        public DictionariesBox()
        {
            this.SuspendLayout();
            this.Dock = DockStyle.Fill;
            this.Controls.AddRange(pages);
            this.Font = SystemFonts.MenuFont;
            this.Font = new Font(this.Font.FontFamily, this.Font.Size + 1F);
            cms.Items.AddRange(items);
            for(int i = 0; i < 6; i++)
            {
                boxs[i] = new ReadOnlyRichTextBox { Parent = pages[i] };
                if(i > 0) boxs[i].ContextMenuStrip = cms;
            }
            items[0].Click += (sender, e) => ExternalProgram.OpenNotepadWithText(GetInitialText());
            items[2].Click += (sender, e) => SaveFile();
            boxs[0].Controls.Add(btnOpenDir);
            btnOpenDir.Top = boxs[0].Height - btnOpenDir.Height;
            ToolTipBox.SetToolTip(btnOpenDir, AppString.Menu.FileLocation);
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.OpenDirectory(AppConfig.DicsDir);
            this.SelectedIndexChanged += (sender, e) => LoadText();
            this.VisibleChanged += (sender, e) => this.SetEnabled(this.Visible);
            this.ResumeLayout();
        }

        readonly TabPage[] pages =
        {
            new TabPage(AppString.Other.DictionaryDescription),
            new TabPage(AppString.SideBar.AppLanguage),
            new TabPage(AppString.Other.GuidInfosDictionary),
            new TabPage(AppString.SideBar.DetailedEdit),
            new TabPage(AppString.SideBar.EnhanceMenu),
            new TabPage(AppString.Other.UwpMode)
        };
        readonly ReadOnlyRichTextBox[] boxs = new ReadOnlyRichTextBox[6];
        readonly PictureButton btnOpenDir = new PictureButton(AppImage.Open)
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
            Left = 0
        };
        readonly ContextMenuStrip cms = new ContextMenuStrip();
        readonly ToolStripItem[] items =
        {
            new ToolStripMenuItem(AppString.Menu.Edit),
            new ToolStripSeparator(),
            new ToolStripMenuItem(AppString.Menu.Save)
        };

        private void SaveFile()
        {
            using(SaveFileDialog dlg = new SaveFileDialog())
            {
                string dirPath = AppConfig.UserDicsDir;
                switch(SelectedIndex)
                {
                    case 1:
                        dirPath = AppConfig.LangsDir;
                        dlg.FileName = AppConfig.ZH_CNINI;
                        break;
                    case 2:
                        dlg.FileName = AppConfig.GUIDINFOSDICINI;
                        break;
                    case 3:
                        dlg.FileName = AppConfig.DETAILEDEDITDICXML;
                        break;
                    case 4:
                        dlg.FileName = AppConfig.ENHANCEMENUSICXML;
                        break;
                    case 5:
                        dlg.FileName = AppConfig.UWPMODEITEMSDICXML;
                        break;
                }
                dlg.Filter = $"{dlg.FileName}|*{Path.GetExtension(dlg.FileName)}";
                Directory.CreateDirectory(dirPath);
                dlg.InitialDirectory = dirPath;
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, GetInitialText(), Encoding.Unicode);
                }
            }
        }

        private string GetInitialText()
        {
            switch(this.SelectedIndex)
            {
                case 0:
                    return AppString.Other.Dictionaries;
                case 1:
                    return Properties.Resources.AppLanguageDic;
                case 2:
                    return Properties.Resources.GuidInfosDic;
                case 3:
                    return Properties.Resources.DetailedEditDic;
                case 4:
                    return Properties.Resources.EnhanceMenusDic;
                case 5:
                    return Properties.Resources.UwpModeItemsDic;
                default:
                    return string.Empty;
            }
        }

        public void LoadText()
        {
            int index = this.SelectedIndex;
            if(boxs[index].Text.Length > 0) return;
            Action<string> action = null;
            switch(index)
            {
                case 0:
                case 1:
                case 2:
                    action = boxs[index].LoadIni; break;
                case 3:
                case 4:
                case 5:
                    action = boxs[index].LoadXml; break;
            }
            this.BeginInvoke(action, new[] { GetInitialText() });
        }
    }
}