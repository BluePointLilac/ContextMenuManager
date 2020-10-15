using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class DonateBox : Panel
    {
        public DonateBox()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.Controls.AddRange(new Control[] { lblInfo, picQR });
        }

        readonly Label lblInfo = new Label
        {
            Font = new Font(SystemFonts.MenuFont.FontFamily, 10F),
            Text = AppString.Text_Donate,
            AutoSize = true
        };

        readonly PictureBox picQR = new PictureBox
        {
            Image = Properties.Resources.Donate,
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(600, 200).DpiZoom()
        };

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            picQR.Left = (this.Width - picQR.Width) / 2;
            lblInfo.Left = (this.Width - lblInfo.Width) / 2;
            picQR.Top = (this.Height - picQR.Height + lblInfo.Height) / 2;
            lblInfo.Top = picQR.Top - lblInfo.Height * 2;
        }
    }

    sealed class AboutAppBox : RichTextBox
    {
        public AboutAppBox()
        {
            this.ReadOnly = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.ForeColor = Color.FromArgb(60, 60, 60);
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 11F);
        }

        const int WM_SETFOCUS = 0x0007;
        const int WM_KILLFOCUS = 0x0008;
        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_SETFOCUS:
                    m.Msg = WM_KILLFOCUS; break;
            }
            base.WndProc(ref m);
        }

        protected override void OnLinkClicked(LinkClickedEventArgs e)
        {
            base.OnLinkClicked(e); Process.Start(e.LinkText);
        }
    }

    sealed class DictionariesBox : TabControl
    {
        public DictionariesBox()
        {
            this.Dock = DockStyle.Fill;
            this.Controls.AddRange(pages);
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 11F);
            cms.Items.AddRange(items);
            for(int i = 0; i < 5; i++)
            {
                boxs[i] = new AboutAppBox { Parent = pages[i] };
                if(i > 0) boxs[i].ContextMenuStrip = cms;
            }
            items[0].Click += (sender, e) => EditText();
            items[2].Click += (sender, e) => SaveFile();
        }

        readonly TabPage[] pages = new TabPage[] {
            new TabPage(AppString.Text_DictionaryDescription),
            new TabPage(AppString.Text_LanguageDictionary),
            new TabPage(AppString.Text_GuidInfosDictionary),
            new TabPage(AppString.Text_ThridRulesDictionary),
            new TabPage(AppString.Text_CommonItemsDictionary)
        };
        readonly AboutAppBox[] boxs = new AboutAppBox[5];
        readonly ContextMenuStrip cms = new ContextMenuStrip();
        readonly ToolStripItem[] items = new ToolStripItem[] {
            new ToolStripMenuItem(AppString.Edit),
            new ToolStripSeparator(),
            new ToolStripMenuItem(AppString.Save)
        };

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        const int WM_SETTEXT = 0x000C;
        private void EditText()
        {
            Process process = Process.Start("notepad.exe");
            Thread.Sleep(200);
            IntPtr handle = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, "Edit", null);
            SendMessage(handle, WM_SETTEXT, 0, GetInitialText());
        }

        private void SaveFile()
        {
            using(SaveFileDialog dlg = new SaveFileDialog())
            {
                string dirPath = Program.ConfigDir;
                switch(SelectedIndex)
                {
                    case 1:
                        dirPath = Program.LanguagesDir;
                        dlg.FileName = Program.ZH_CNINI;
                        break;
                    case 2:
                        dlg.FileName = Program.GUIDINFOSINI;
                        break;
                    case 3:
                        dlg.FileName = Program.ThIRDRULESDICXML;
                        break;
                    case 4:
                        dlg.FileName = Program.SHELLCOMMONDICXML;
                        break;
                }
                dlg.Filter = $"{dlg.FileName}|*{Path.GetExtension(dlg.FileName)}";
                Directory.CreateDirectory(dirPath);
                dlg.InitialDirectory = dirPath;
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, GetInitialText(), Encoding.UTF8);
                }
            }
        }

        private string GetInitialText()
        {
            switch(this.SelectedIndex)
            {
                case 1:
                    return Properties.Resources.AppLanguageDic;
                case 2:
                    return Properties.Resources.GuidInfosDic;
                case 3:
                    return Properties.Resources.ThirdRulesDic;
                case 4:
                    return Properties.Resources.ShellCommonDic;
                default:
                    return string.Empty;
            }
        }

        bool hadLoaded = false;
        public async void LoadTexts()
        {
            if(hadLoaded) return;
            hadLoaded = true;
            boxs[0].Text = AppString.Text_Dictionaries;
            await Task.Run(() => boxs[1].BeginInvoke(new Action<string>(boxs[1].LoadIni), new[] { Properties.Resources.AppLanguageDic }));
            await Task.Run(() => boxs[2].BeginInvoke(new Action<string>(boxs[2].LoadIni), new[] { Properties.Resources.GuidInfosDic }));
            await Task.Run(() => boxs[3].BeginInvoke(new Action<string>(boxs[3].LoadXml), new[] { Properties.Resources.ThirdRulesDic }));
            await Task.Run(() => boxs[4].BeginInvoke(new Action<string>(boxs[4].LoadXml), new[] { Properties.Resources.ShellCommonDic }));
        }
    }

    sealed class LanguagesBox : Panel
    {
        const string OtherLanguagesUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/tree/master/languages";

        public LanguagesBox()
        {
            this.Dock = DockStyle.Fill;
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 11F);
            this.Controls.AddRange(new Control[] { cmbLanguages, llbOtherLanguages, txtTranslators });
            this.OnResize(null);
            cmbLanguages.SelectionChangeCommitted += (sender, e) => ChangeLanguage();
            llbOtherLanguages.LinkClicked += (sender, e) => Process.Start(OtherLanguagesUrl);
        }

        readonly ComboBox cmbLanguages = new ComboBox
        {
            Width = 200.DpiZoom(),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        readonly LinkLabel llbOtherLanguages = new LinkLabel
        {
            Text = AppString.Text_OtherLanguages,
            AutoSize = true
        };

        readonly TextBox txtTranslators = new TextBox
        {
            ReadOnly = true,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        readonly List<string> iniPaths = new List<string>();

        int selectIndex = 0;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int a = 20.DpiZoom();
            txtTranslators.Left = cmbLanguages.Left = cmbLanguages.Top = llbOtherLanguages.Top = a;
            txtTranslators.Top = cmbLanguages.Bottom + a;
            txtTranslators.Width = this.ClientSize.Width - 2 * a;
            txtTranslators.Height = this.ClientSize.Height - txtTranslators.Top - a;
            txtTranslators.BackColor = this.BackColor;
            llbOtherLanguages.Left = txtTranslators.Right - llbOtherLanguages.Width;
        }

        public void LoadLanguages()
        {
            cmbLanguages.Items.Clear();
            cmbLanguages.Items.Add("(默认) 简体中文");
            cmbLanguages.SelectedIndex = 0;
            string str = AppString.Text_Translators + Environment.NewLine;
            DirectoryInfo di = new DirectoryInfo(Program.LanguagesDir);
            if(di.Exists)
            {
                iniPaths.Clear();
                FileInfo[] fis = di.GetFiles();
                for(int i = 0; i < fis.Length; i++)
                {
                    FileInfo fi = fis[i];
                    IniReader reader = new IniReader(fi.FullName);
                    string name = reader.GetValue("General", "Language");
                    string translator = reader.GetValue("General", "Translator");
                    cmbLanguages.Items.Add(name);
                    str += Environment.NewLine + name + "\t\t" + translator;
                    iniPaths.Add(fi.FullName);
                    if(fi.FullName.Equals(Program.LanguageFilePath, StringComparison.OrdinalIgnoreCase))
                        cmbLanguages.SelectedIndex = i + 1;
                }
                txtTranslators.Text = str;
            }
            selectIndex = cmbLanguages.SelectedIndex;
        }

        private void ChangeLanguage()
        {
            if(cmbLanguages.SelectedIndex == selectIndex) return;
            string path = "default";
            if(cmbLanguages.SelectedIndex > 0) path = iniPaths[cmbLanguages.SelectedIndex - 1];
            new IniFileHelper(Program.ConfigIniPath).SetValue("General", "Language", path);
            MessageBoxEx.Show(AppString.MessageBox_RestartApp, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
        }
    }
}