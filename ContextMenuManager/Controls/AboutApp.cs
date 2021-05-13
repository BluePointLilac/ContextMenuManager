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
        public DonateBox()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            this.Controls.AddRange(new Control[] { lblInfo, picQR, llbDonationList });
            llbDonationList.LinkClicked += (sender, e) => Updater.ShowDonateDialog();
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
            for(int i = 0; i < 6; i++)
            {
                boxs[i] = new ReadOnlyRichTextBox { Parent = pages[i] };
                if(i > 0) boxs[i].ContextMenuStrip = cms;
            }
            items[0].Click += (sender, e) => ExternalProgram.OpenNotepadWithText(GetInitialText());
            items[2].Click += (sender, e) => SaveFile();
            boxs[0].Controls.Add(btnOpenDir);
            boxs[0].Text = AppString.Other.Dictionaries;
            btnOpenDir.Top = boxs[0].Height - btnOpenDir.Height;
            MyToolTip.SetToolTip(btnOpenDir, AppString.Menu.FileLocation);
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.DicsDir);
            this.SelectedIndexChanged += (sender, e) => LoadText();
        }

        readonly TabPage[] pages = new TabPage[] {
            new TabPage(AppString.Other.DictionaryDescription),
            new TabPage(AppString.SideBar.AppLanguage),
            new TabPage(AppString.Other.GuidInfosDictionary),
            new TabPage(AppString.SideBar.ThirdRules),
            new TabPage(AppString.SideBar.EnhanceMenu),
            new TabPage(AppString.Other.UWPMode)
        };
        readonly ReadOnlyRichTextBox[] boxs = new ReadOnlyRichTextBox[6];
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
                    return Properties.Resources.ThirdRulesDic;
                case 4:
                    return Properties.Resources.EnhanceMenusDic;
                case 5:
                    return Properties.Resources.UwpModeItemsDic;
                default:
                    return string.Empty;
            }
        }

        private void LoadText()
        {
            int index = this.SelectedIndex;
            if(boxs[index].Text.Length > 0) return;
            Action<string> action = null;
            switch(index)
            {
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

    sealed class LanguagesBox : FlowLayoutPanel
    {
        public LanguagesBox()
        {
            this.Dock = DockStyle.Fill;
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            this.Controls.AddRange(new Control[] { cmbLanguages, btnOpenDir, btnDownLoad, btnTranslate, txtTranslators });
            cmbLanguages.SelectionChangeCommitted += (sender, e) => ChangeLanguage();
            btnDownLoad.MouseDown += (sender, e) => { if(Updater.ShowLanguageDialog()) LoadLanguages(); };
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.LangsDir);
            btnTranslate.MouseDown += (sender, e) => new TranslateDialog().ShowDialog();
            MyToolTip.SetToolTip(btnOpenDir, AppString.Menu.FileLocation);
            MyToolTip.SetToolTip(btnDownLoad, AppString.Dialog.DownloadLanguages);
            MyToolTip.SetToolTip(btnTranslate, AppString.Dialog.TranslateTool);
            txtTranslators.SetAutoShowScroll(ScrollBars.Vertical);
            cmbLanguages.AutosizeDropDownWidth();
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
                    string langName = Path.GetFileNameWithoutExtension(fileName);
                    IniWriter reader = new IniWriter(fileName);
                    string language = reader.GetValue("General", "Language");
                    if(language.IsNullOrWhiteSpace()) language = langName;
                    string translator = reader.GetValue("General", "Translator");
                    str += Environment.NewLine + language + new string('\t', 5) + translator;
                    cmbLanguages.Items.Add(language);
                    languages.Add(langName);
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
            if(MessageBoxEx.Show(AppString.Message.RestartApp, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                cmbLanguages.SelectedIndex = index;
            }
            else
            {
                AppConfig.Language = language;
                SingleInstance.Restart();
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
        public AppSettingBox()
        {
            this.Font = new Font(SystemFonts.MenuFont.FontFamily, 10F);
            mliConfigDir.AddCtrs(new Control[] { cmbConfigDir, btnConfigDir });
            mliBackup.AddCtrs(new Control[] { chkBackup, btnBackupDir });
            mliUpdate.AddCtrs(new Control[] { cmbUpdate, lblUpdate });
            mliRepo.AddCtr(cmbRepo);
            mliProtect.AddCtr(chkProtect);
            mliEngine.AddCtr(cmbEngine);
            mliWinXSortable.AddCtr(chkWinXSortable);
            mliShowFilePath.AddCtr(chkShowFilePath);
            mliOpenMoreRegedit.AddCtr(chkOpenMoreRegedit);
            mliHideDisabledItems.AddCtr(chkHideDisabledItems);
            cmbConfigDir.AutosizeDropDownWidth();
            cmbEngine.AutosizeDropDownWidth();
            cmbRepo.AutosizeDropDownWidth();
            MyToolTip.SetToolTip(cmbConfigDir, AppString.Tip.ConfigPath);
            MyToolTip.SetToolTip(btnConfigDir, AppString.Menu.FileLocation);
            MyToolTip.SetToolTip(btnBackupDir, AppString.Menu.FileLocation);

            cmbRepo.Items.AddRange(new[] { "Github", "Gitee" });
            cmbConfigDir.Items.AddRange(new[] { AppString.Other.AppDataDir, AppString.Other.AppDir });
            cmbEngine.Items.AddRange(new[] { "Bing", "Baidu", "Google", "DuckDuckGo", "DogeDoge", "Sogou", "360", AppString.Other.CustomEngine });
            cmbUpdate.Items.AddRange(new[] { AppString.Other.OnceAWeek, AppString.Other.OnceAMonth, AppString.Other.OnceASeason, AppString.Other.NeverCheck });

            btnConfigDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.ConfigDir);
            btnBackupDir.MouseDown += (sender, e) => ExternalProgram.JumpExplorer(AppConfig.BackupDir);
            lblUpdate.Click += (sender, e) =>
            {
                if(!Updater.Update()) MessageBoxEx.Show(AppString.Message.VersionIsLatest);
            };
            cmbConfigDir.SelectionChangeCommitted += (sender, e) =>
            {
                string newPath = (cmbConfigDir.SelectedIndex == 0) ? AppConfig.AppDataConfigDir : AppConfig.AppConfigDir;
                if(newPath == AppConfig.ConfigDir) return;
                if(MessageBoxEx.Show(AppString.Message.RestartApp, MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
                }
                else
                {
                    DirectoryEx.CopyTo(AppConfig.ConfigDir, newPath);
                    Directory.Delete(AppConfig.ConfigDir, true);
                    SingleInstance.Restart();
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
            cmbUpdate.SelectionChangeCommitted += (sender, e) =>
            {
                int day = 30;
                switch(cmbUpdate.SelectedIndex)
                {
                    case 0:
                        day = 7; break;
                    case 2:
                        day = 90; break;
                    case 3:
                        day = -1; break;
                }
                AppConfig.UpdateFrequency = day;
            };
            this.VisibleChanged += (sender, e) => this.Enabled = this.Visible;
            cmbRepo.SelectionChangeCommitted += (sender, e) => AppConfig.RequestUseGithub = cmbRepo.SelectedIndex == 0;
            chkBackup.MouseDown += (sender, e) => AppConfig.AutoBackup = chkBackup.Checked = !chkBackup.Checked;
            chkProtect.MouseDown += (sender, e) => AppConfig.ProtectOpenItem = chkProtect.Checked = !chkProtect.Checked;
            chkWinXSortable.MouseDown += (sender, e) => AppConfig.WinXSortable = chkWinXSortable.Checked = !chkWinXSortable.Checked;
            chkOpenMoreRegedit.MouseDown += (sender, e) => AppConfig.OpenMoreRegedit = chkOpenMoreRegedit.Checked = !chkOpenMoreRegedit.Checked;
            chkHideDisabledItems.MouseDown += (sender, e) => AppConfig.HideDisabledItems = chkHideDisabledItems.Checked = !chkHideDisabledItems.Checked;
            chkShowFilePath.MouseDown += (sender, e) =>
            {
                chkShowFilePath.Checked = !chkShowFilePath.Checked;
                if(MessageBoxEx.Show(AppString.Message.RestartApp, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    AppConfig.ShowFilePath = chkShowFilePath.Checked;
                    SingleInstance.Restart();
                }
                else
                {
                    chkShowFilePath.Checked = !chkShowFilePath.Checked;
                }
            };
        }

        readonly MyListItem mliConfigDir = new MyListItem
        {
            Text = AppString.Other.ConfigPath
        };
        readonly ComboBox cmbConfigDir = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };
        readonly PictureButton btnConfigDir = new PictureButton(AppImage.Open);

        readonly MyListItem mliRepo = new MyListItem
        {
            Text = AppString.Other.SetRequestRepo
        };
        readonly ComboBox cmbRepo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };

        readonly MyListItem mliBackup = new MyListItem
        {
            Text = AppString.Other.AutoBackup
        };
        readonly MyCheckBox chkBackup = new MyCheckBox();
        readonly PictureButton btnBackupDir = new PictureButton(AppImage.Open);

        readonly MyListItem mliUpdate = new MyListItem
        {
            Text = AppString.Other.SetUpdateFrequency
        };
        readonly ComboBox cmbUpdate = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120.DpiZoom()
        };
        readonly Label lblUpdate = new Label
        {
            Text = AppString.Other.ImmediatelyCheck,
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Hand,
            AutoSize = true
        };

        readonly MyListItem mliProtect = new MyListItem
        {
            Text = AppString.Other.ProtectOpenItem
        };
        readonly MyCheckBox chkProtect = new MyCheckBox();

        readonly MyListItem mliEngine = new MyListItem
        {
            Text = AppString.Other.WebSearchEngine
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
            Text = AppString.Other.ShowFilePath
        };
        readonly MyCheckBox chkShowFilePath = new MyCheckBox();

        readonly MyListItem mliOpenMoreRegedit = new MyListItem
        {
            Text = AppString.Other.OpenMoreRegedit
        };
        readonly MyCheckBox chkOpenMoreRegedit = new MyCheckBox();

        readonly MyListItem mliHideDisabledItems = new MyListItem
        {
            Text = AppString.Other.HideDisabledItems
        };
        readonly MyCheckBox chkHideDisabledItems = new MyCheckBox();

        public void LoadItems()
        {
            this.AddItems(new[] { mliUpdate, mliConfigDir, mliRepo, mliEngine, mliBackup, mliProtect,
                mliWinXSortable, mliShowFilePath, mliOpenMoreRegedit, mliHideDisabledItems });
            foreach(MyListItem item in this.Controls) item.HasImage = false;
            cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
            cmbRepo.SelectedIndex = AppConfig.RequestUseGithub ? 0 : 1;
            chkBackup.Checked = AppConfig.AutoBackup;
            chkProtect.Checked = AppConfig.ProtectOpenItem;
            chkWinXSortable.Checked = AppConfig.WinXSortable;
            chkShowFilePath.Checked = AppConfig.ShowFilePath;
            chkOpenMoreRegedit.Checked = AppConfig.OpenMoreRegedit;
            chkHideDisabledItems.Checked = AppConfig.HideDisabledItems;

            int index = 1;
            switch(AppConfig.UpdateFrequency)
            {
                case 7:
                    index = 0; break;
                case 90:
                    index = 2; break;
                case -1:
                    index = 3; break;
            }
            cmbUpdate.SelectedIndex = index;

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