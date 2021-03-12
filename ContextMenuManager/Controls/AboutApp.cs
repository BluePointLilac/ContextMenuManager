using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class DonateBox : Panel
    {
        private const string DonateListUrl = "https://github.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        public DonateBox()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            this.Controls.AddRange(new Control[] { lblInfo, picQR, llbDonationList });
            llbDonationList.LinkClicked += (sender, e) => ExternalProgram.OpenUrl(DonateListUrl);
        }

        readonly Label lblInfo = new Label
        {
            Text = AppString.Other.Donate,
            AutoSize = true
        };

        readonly LinkLabel llbDonationList = new LinkLabel
        {
            Text = AppString.Other.DonationList,
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
            int a = 60.DpiZoom();
            base.OnResize(e);
            picQR.Left = (this.Width - picQR.Width) / 2;
            lblInfo.Left = (this.Width - lblInfo.Width) / 2;
            llbDonationList.Left = (this.Width - llbDonationList.Width) / 2;
            lblInfo.Top = a;
            picQR.Top = lblInfo.Bottom + a;
            llbDonationList.Top = picQR.Bottom + a;
        }
    }

    sealed class DictionariesBox : TabControl
    {
        public DictionariesBox()
        {
            this.Dock = DockStyle.Fill;
            this.Controls.AddRange(pages);
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            cms.Items.AddRange(items);
            for(int i = 0; i < 5; i++)
            {
                boxs[i] = new ReadOnlyRichTextBox { Parent = pages[i] };
                if(i > 0) boxs[i].ContextMenuStrip = cms;
            }
            items[0].Click += (sender, e) => ExternalProgram.OpenNotepadWithText(GetInitialText());
            items[2].Click += (sender, e) => SaveFile();
            boxs[0].Controls.Add(btnOpenDir);
            btnOpenDir.Top = boxs[0].Height - btnOpenDir.Height;
            MyToolTip.SetToolTip(btnOpenDir, AppString.Tip.OpenDictionariesDir);
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.DicsDir);
        }

        readonly TabPage[] pages = new TabPage[] {
            new TabPage(AppString.Other.DictionaryDescription),
            new TabPage(AppString.SideBar.AppLanguage),
            new TabPage(AppString.Other.GuidInfosDictionary),
            new TabPage(AppString.SideBar.ThirdRules),
            new TabPage(AppString.SideBar.EnhanceMenu)
        };
        readonly ReadOnlyRichTextBox[] boxs = new ReadOnlyRichTextBox[5];
        readonly PictureButton btnOpenDir = new PictureButton(AppImage.Open)
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
            Left = 0
        };
        readonly ContextMenuStrip cms = new ContextMenuStrip();
        readonly ToolStripItem[] items = new ToolStripItem[] {
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
                        dlg.FileName = AppConfig.THIRDRULESDICXML;
                        break;
                    case 4:
                        dlg.FileName = AppConfig.ENHANCEMENUSICXML;
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
                case 1:
                    return Properties.Resources.AppLanguageDic;
                case 2:
                    return Properties.Resources.GuidInfosDic;
                case 3:
                    return Properties.Resources.ThirdRulesDic;
                case 4:
                    return Properties.Resources.EnhanceMenusDic;
                default:
                    return string.Empty;
            }
        }

        bool hadLoaded = false;
        public void LoadTexts()
        {
            if(hadLoaded) return;
            hadLoaded = true;
            boxs[0].Text = AppString.Other.Dictionaries;
            this.BeginInvoke(new Action<string>(boxs[1].LoadIni), new[] { Properties.Resources.AppLanguageDic });
            this.BeginInvoke(new Action<string>(boxs[2].LoadIni), new[] { Properties.Resources.GuidInfosDic });
            this.BeginInvoke(new Action<string>(boxs[3].LoadXml), new[] { Properties.Resources.ThirdRulesDic });
            this.BeginInvoke(new Action<string>(boxs[4].LoadXml), new[] { Properties.Resources.EnhanceMenusDic });
        }
    }

    sealed class LanguagesBox : FlowLayoutPanel
    {
        const string OtherLanguagesUrl = "https://github.com/BluePointLilac/ContextMenuManager/tree/master/languages";

        public LanguagesBox()
        {
            this.Dock = DockStyle.Fill;
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            this.Controls.AddRange(new Control[] { cmbLanguages, btnOpenDir, btnDownLoad, txtTranslators });
            cmbLanguages.SelectionChangeCommitted += (sender, e) => ChangeLanguage();
            btnDownLoad.MouseDown += (sender, e) => ExternalProgram.OpenUrl(OtherLanguagesUrl);
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.LangsDir);
            btnTranslate.MouseDown += (sender, e) => new TranslateDialog().ShowDialog();
            MyToolTip.SetToolTip(btnOpenDir, AppString.Tip.OpenLanguagesDir);
            MyToolTip.SetToolTip(btnDownLoad, AppString.Tip.OtherLanguages);
            this.OnResize(null);
        }

        readonly ComboBox cmbLanguages = new ComboBox
        {
            Width = 150.DpiZoom(),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        readonly ReadOnlyTextBox txtTranslators = new ReadOnlyTextBox();
        readonly PictureButton btnOpenDir = new PictureButton(AppImage.Open);
        readonly PictureButton btnDownLoad = new PictureButton(AppImage.DownLoad);
        readonly PictureButton btnTranslate = new PictureButton(AppImage.Translate);
        readonly List<string> languages = new List<string>();

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int a = 20.DpiZoom();
            txtTranslators.Width = this.ClientSize.Width - 2 * a;
            txtTranslators.Height = this.ClientSize.Height - txtTranslators.Top - a;
            cmbLanguages.Margin = txtTranslators.Margin = btnOpenDir.Margin
                = btnDownLoad.Margin = btnTranslate.Margin = new Padding(a, a, 0, 0);
        }

        public void LoadLanguages()
        {
            cmbLanguages.Items.Clear();
            cmbLanguages.Items.Add("(default) 简体中文");
            string str = AppString.Other.Translators + Environment.NewLine + new string('-', 74);
            if(Directory.Exists(AppConfig.LangsDir))
            {
                languages.Clear();
                foreach(string fileName in Directory.GetFiles(AppConfig.LangsDir, "*.ini"))
                {
                    languages.Add(Path.GetFileNameWithoutExtension(fileName));
                    IniReader reader = new IniReader(fileName);
                    string language = reader.GetValue("General", "Language");
                    string translator = reader.GetValue("General", "Translator");
                    str += Environment.NewLine + language + new string('\t', 5) + translator;
                    cmbLanguages.Items.Add(language);
                }
            }
            txtTranslators.Text = str;
            cmbLanguages.SelectedIndex = GetSelectIndex();
        }

        private void ChangeLanguage()
        {
            string language = "default";
            int index = GetSelectIndex();
            if(cmbLanguages.SelectedIndex == index) return;
            if(cmbLanguages.SelectedIndex > 0) language = languages[cmbLanguages.SelectedIndex - 1];
            if(MessageBoxEx.Show(AppString.MessageBox.RestartApp, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                cmbLanguages.SelectedIndex = index;
            }
            else
            {
                AppConfig.Language = language;
                SingleInstance.Restart(true);
            }
        }

        private int GetSelectIndex()
        {
            string language = AppConfig.Language;
            for(int i = 0; i < languages.Count; i++)
            {
                if(languages[i].Equals(language, StringComparison.OrdinalIgnoreCase)) return i + 1;
            }
            return 0;
        }
    }

    sealed class AppSettingBox : MyList
    {
        private const string GithubUrl = "https://github.com/BluePointLilac/ContextMenuManager/releases";
        private const string GiteeUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/releases";

        public AppSettingBox()
        {
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            mliConfigDir.AddCtrs(new Control[] { cmbConfigDir, btnConfigDir });
            mliBackup.AddCtrs(new Control[] { chkBackup, btnBackupDir });
            mliUpdate.AddCtrs(new Control[] { lblGitee, lblGithub, lblUpdate });
            mliProtect.AddCtr(chkProtect);
            mliEngine.AddCtr(cmbEngine);
            mliWinXSortable.AddCtr(chkWinXSortable);
            mliShowFilePath.AddCtr(chkShowFilePath);
            mliOpenMoreRegedit.AddCtr(chkOpenMoreRegedit);
            MyToolTip.SetToolTip(cmbConfigDir, AppString.Tip.ConfigPath);
            MyToolTip.SetToolTip(btnConfigDir, AppString.Other.OpenConfigDir);
            MyToolTip.SetToolTip(btnBackupDir, AppString.Other.OpenBackupDir);
            MyToolTip.SetToolTip(mliUpdate, AppString.Tip.CheckUpdate + Environment.NewLine
                + AppString.Tip.LastCheckUpdateTime + AppConfig.LastCheckUpdateTime.ToLongDateString());
            cmbConfigDir.Items.AddRange(new[] { AppString.Other.AppDataDir, AppString.Other.AppDir });
            cmbEngine.Items.AddRange(new[] { "Baidu", "Bing", "Google", "DogeDoge", "Sogou", "360", AppString.Other.CustomEngine });
            btnConfigDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.ConfigDir);
            btnBackupDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.BackupDir);
            lblGithub.Click += (sender, e) => ExternalProgram.OpenUrl(GithubUrl);
            lblGitee.Click += (sender, e) => ExternalProgram.OpenUrl(GiteeUrl);
            lblUpdate.Click += (sender, e) =>
            {
                if(!Updater.CheckUpdate()) MessageBoxEx.Show(AppString.MessageBox.NoUpdateDetected);
            };
            cmbConfigDir.SelectionChangeCommitted += (sender, e) =>
            {
                string newPath = (cmbConfigDir.SelectedIndex == 0) ? AppConfig.AppDataConfigDir : AppConfig.AppConfigDir;
                if(newPath == AppConfig.ConfigDir) return;
                if(MessageBoxEx.Show(AppString.MessageBox.RestartApp, MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
                }
                else
                {
                    DirectoryEx.CopyTo(AppConfig.ConfigDir, newPath);
                    Directory.Delete(AppConfig.ConfigDir, true);
                    SingleInstance.Restart(true);
                }
            };
            cmbEngine.SelectionChangeCommitted += (sender, e) =>
            {
                if(cmbEngine.SelectedIndex < cmbEngine.Items.Count - 1)
                {
                    AppConfig.EngineUrl = AppConfig.EngineUrls[cmbEngine.SelectedIndex];
                }
                else
                {
                    using(InputDialog dlg = new InputDialog { Title = AppString.Other.SetCustomEngine })
                    {
                        dlg.Text = AppConfig.EngineUrl;
                        if(dlg.ShowDialog() == DialogResult.OK) AppConfig.EngineUrl = dlg.Text;
                        string url = AppConfig.EngineUrl;
                        for(int i = 0; i < AppConfig.EngineUrls.Length; i++)
                        {
                            if(url.Equals(AppConfig.EngineUrls[i]))
                            {
                                cmbEngine.SelectedIndex = i; break;
                            }
                        }
                    }
                }
            };
            chkBackup.MouseDown += (sender, e) => AppConfig.AutoBackup = chkBackup.Checked = !chkBackup.Checked;
            chkProtect.MouseDown += (sender, e) => AppConfig.ProtectOpenItem = chkProtect.Checked = !chkProtect.Checked;
            chkWinXSortable.MouseDown += (sender, e) => AppConfig.WinXSortable = chkWinXSortable.Checked = !chkWinXSortable.Checked;
            chkOpenMoreRegedit.MouseDown += (sender, e) => AppConfig.OpenMoreRegedit = chkOpenMoreRegedit.Checked = !chkOpenMoreRegedit.Checked;
            chkShowFilePath.MouseDown += (sender, e) =>
            {
                chkShowFilePath.Checked = !chkShowFilePath.Checked;
                if(MessageBoxEx.Show(AppString.MessageBox.RestartApp, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    AppConfig.ShowFilePath = chkShowFilePath.Checked;
                    SingleInstance.Restart(true);
                }
                else
                {
                    chkShowFilePath.Checked = !chkShowFilePath.Checked;
                }
            };
        }

        readonly MyListItem mliConfigDir = new MyListItem
        {
            Text = AppString.Other.ConfigPath,
            HasImage = false
        };
        readonly ComboBox cmbConfigDir = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };
        readonly PictureButton btnConfigDir = new PictureButton(AppImage.Open);

        readonly MyListItem mliBackup = new MyListItem
        {
            Text = AppString.Other.AutoBackup,
            HasImage = false
        };
        readonly MyCheckBox chkBackup = new MyCheckBox();
        readonly PictureButton btnBackupDir = new PictureButton(AppImage.Open);

        readonly MyListItem mliUpdate = new MyListItem
        {
            Text = AppString.Other.CheckUpdate,
            HasImage = false
        };
        readonly Label lblUpdate = new Label
        {
            Text = AppString.Other.ImmediatelyCheckUpdate,
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
            AutoSize = true
        };
        readonly Label lblGithub = new Label
        {
            Text = "Github",
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
            AutoSize = true
        };
        readonly Label lblGitee = new Label
        {
            Text = "Gitee",
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
            AutoSize = true
        };

        readonly MyListItem mliProtect = new MyListItem
        {
            Text = AppString.Other.ProtectOpenItem,
            HasImage = false
        };
        readonly MyCheckBox chkProtect = new MyCheckBox();

        readonly MyListItem mliEngine = new MyListItem
        {
            Text = AppString.Other.WebSearchEngine,
            HasImage = false
        };
        readonly ComboBox cmbEngine = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };

        readonly MyListItem mliWinXSortable = new MyListItem
        {
            Text = AppString.Other.WinXSortable,
            Visible = WindowsOsVersion.ISAfterOrEqual8,
            HasImage = false
        };
        readonly MyCheckBox chkWinXSortable = new MyCheckBox();

        readonly MyListItem mliShowFilePath = new MyListItem
        {
            Text = AppString.Other.ShowFilePath,
            HasImage = false
        };
        readonly MyCheckBox chkShowFilePath = new MyCheckBox();

        readonly MyListItem mliOpenMoreRegedit = new MyListItem
        {
            Text = AppString.Other.OpenMoreRegedit,
            HasImage = false
        };
        readonly MyCheckBox chkOpenMoreRegedit = new MyCheckBox();

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Enabled = this.Visible;
        }

        public void LoadItems()
        {
            this.AddItems(new[] { mliUpdate, mliConfigDir, mliEngine, mliBackup,
                mliProtect, mliWinXSortable, mliShowFilePath, mliOpenMoreRegedit });
            cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
            chkBackup.Checked = AppConfig.AutoBackup;
            chkProtect.Checked = AppConfig.ProtectOpenItem;
            chkWinXSortable.Checked = AppConfig.WinXSortable;
            chkShowFilePath.Checked = AppConfig.ShowFilePath;
            chkOpenMoreRegedit.Checked = AppConfig.OpenMoreRegedit;

            string url = AppConfig.EngineUrl;
            for(int i = 0; i <= AppConfig.EngineUrls.Length; i++)
            {
                if(i == AppConfig.EngineUrls.Length || url.Equals(AppConfig.EngineUrls[i]))
                {
                    cmbEngine.SelectedIndex = i; break;
                }
            }
        }
    }
}