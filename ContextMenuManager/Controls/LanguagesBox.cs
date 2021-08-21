using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class LanguagesBox : FlowLayoutPanel
    {
        public LanguagesBox()
        {
            this.SuspendLayout();
            this.Dock = DockStyle.Fill;
            this.Font = SystemFonts.MenuFont;
            this.Font = new Font(this.Font.FontFamily, this.Font.Size + 1F);
            this.Controls.AddRange(new Control[] { cmbLanguages, btnOpenDir, btnDownLoad, btnTranslate, lblThank, pnlTranslators });
            this.VisibleChanged += (sender, e) => this.SetEnabled(this.Visible);
            cmbLanguages.SelectionChangeCommitted += (sender, e) => ChangeLanguage();
            btnOpenDir.MouseDown += (sender, e) => ExternalProgram.OpenDirectory(AppConfig.LangsDir);
            lblThank.MouseEnter += (sender, e) => lblThank.ForeColor = Color.FromArgb(0, 162, 255);
            lblThank.MouseLeave += (sender, e) => lblThank.ForeColor = Color.DimGray;
            btnDownLoad.MouseDown += (sender, e) =>
            {
                this.Cursor = Cursors.WaitCursor;
                this.ShowLanguageDialog();
                this.Cursor = Cursors.Default;
            };
            btnTranslate.MouseDown += (sender, e) =>
            {
                using(TranslateDialog dlg = new TranslateDialog())
                {
                    dlg.ShowDialog();
                }
            };
            ToolTipBox.SetToolTip(btnOpenDir, AppString.Menu.FileLocation);
            ToolTipBox.SetToolTip(btnDownLoad, AppString.Dialog.DownloadLanguages);
            ToolTipBox.SetToolTip(btnTranslate, AppString.Dialog.TranslateTool);
            lblHeader.Font = new Font(this.Font, FontStyle.Bold);
            cmbLanguages.AutosizeDropDownWidth();
            this.OnResize(null);
            this.ResumeLayout();
        }

        readonly ComboBox cmbLanguages = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 170.DpiZoom(),
        };
        readonly PictureButton btnOpenDir = new PictureButton(AppImage.Open);
        readonly PictureButton btnDownLoad = new PictureButton(AppImage.DownLoad);
        readonly PictureButton btnTranslate = new PictureButton(AppImage.Translate);
        readonly ToolTip toolTip = new ToolTip { InitialDelay = 1 };
        readonly Panel pnlTranslators = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };
        readonly Label lblHeader = new Label
        {
            Text = AppString.Other.Translators + "\r\n" + new string('-', 96),
            ForeColor = Color.DarkCyan,
            Dock = DockStyle.Top,
            AutoSize = true
        };
        readonly Label lblThank = new Label
        {
            Font = new Font("Lucida Handwriting", 11F),
            Text = "Thank you for your translation!",
            ForeColor = Color.DimGray,
            AutoSize = true,
        };
        readonly List<string> languages = new List<string>();

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int a = 20.DpiZoom();
            pnlTranslators.Width = this.ClientSize.Width - 2 * a;
            pnlTranslators.Height = this.ClientSize.Height - pnlTranslators.Top - a;
            cmbLanguages.Margin = pnlTranslators.Margin = lblThank.Margin = btnOpenDir.Margin
                = btnDownLoad.Margin = btnTranslate.Margin = new Padding(a, a, 0, 0);
        }

        public void LoadLanguages()
        {
            cmbLanguages.Items.Clear();
            cmbLanguages.Items.Add("(default) 简体中文");
            languages.Clear();
            languages.Add("default");
            pnlTranslators.SuspendLayout();
            pnlTranslators.Controls.Remove(lblHeader);
            foreach(Control ctr in pnlTranslators.Controls) ctr.Dispose();
            pnlTranslators.Controls.Clear();
            pnlTranslators.Controls.Add(lblHeader);
            if(Directory.Exists(AppConfig.LangsDir))
            {
                Dictionary<Label, Control[]> dic = new Dictionary<Label, Control[]>();
                foreach(string fileName in Directory.GetFiles(AppConfig.LangsDir, "*.ini"))
                {
                    IniWriter writer = new IniWriter(fileName);
                    string language = writer.GetValue("General", "Language");
                    string translator = writer.GetValue("General", "Translator");
                    string translatorUrl = writer.GetValue("General", "TranslatorUrl");

                    string langName = Path.GetFileNameWithoutExtension(fileName);
                    if(string.IsNullOrEmpty(language)) language = langName;
                    string[] translators = translator.Split(new[] { "\\r\\n", "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] urls = translatorUrl.Split(new[] { "\\r\\n", "\\n" }, StringSplitOptions.RemoveEmptyEntries);

                    Label lblLanguage = new Label
                    {
                        ForeColor = Color.DodgerBlue,
                        Text = language,
                        AutoSize = true,
                        Font = this.Font
                    };
                    Label[] ctrTranslators = new Label[translators.Length];
                    for(int i = 0; i < translators.Length; i++)
                    {
                        ctrTranslators[i] = new Label
                        {
                            AutoSize = true,
                            Font = this.Font,
                            Text = translators[i],
                            ForeColor = Color.DimGray,
                        };
                        if(urls.Length > i)
                        {
                            string url = urls[i].Trim();
                            if(url != "null")
                            {
                                toolTip.SetToolTip(ctrTranslators[i], url);
                                ctrTranslators[i].ForeColor = Color.SkyBlue;
                                ctrTranslators[i].Font = new Font(ctrTranslators[i].Font, FontStyle.Underline);
                                ctrTranslators[i].Click += (sender, e) => ExternalProgram.OpenWebUrl(url);
                            }
                        }
                    }
                    dic.Add(lblLanguage, ctrTranslators);
                    cmbLanguages.Items.Add(language);
                    languages.Add(langName);
                }
                int left = 0;
                dic.Keys.ToList().ForEach(lbl => left = Math.Max(left, lbl.Width));
                left += 250.DpiZoom();
                int top = lblHeader.Bottom + 10.DpiZoom();
                foreach(var item in dic)
                {
                    item.Key.Top = top;
                    pnlTranslators.Controls.Add(item.Key);
                    foreach(var ctr in item.Value)
                    {
                        ctr.Location = new Point(left, top);
                        pnlTranslators.Controls.Add(ctr);
                        top += ctr.Height + 10.DpiZoom();
                    }
                }
            }
            pnlTranslators.ResumeLayout();
            cmbLanguages.SelectedIndex = GetSelectIndex();
        }

        private void ChangeLanguage()
        {
            int index = GetSelectIndex();
            if(cmbLanguages.SelectedIndex == index) return;
            string language = languages[cmbLanguages.SelectedIndex];
            string msg = "";
            if(cmbLanguages.SelectedIndex != 0)
            {
                string langPath = $@"{AppConfig.LangsDir}\{language}.ini";
                msg = new IniWriter(langPath).GetValue("Message", "RestartApp");
            }
            if(msg == "") msg = AppString.Message.RestartApp;
            if(AppMessageBox.Show(msg, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                cmbLanguages.SelectedIndex = index;
            }
            else
            {
                if(language == CultureInfo.CurrentUICulture.Name) language = "";
                AppConfig.Language = language;
                SingleInstance.Restart();
            }
        }

        private int GetSelectIndex()
        {
            int index = languages.FindIndex(language => language.Equals(AppConfig.Language, StringComparison.OrdinalIgnoreCase));
            if(index == -1) index = 0;
            return index;
        }

        public void ShowLanguageDialog()
        {
            using(UAWebClient client = new UAWebClient())
            {
                string apiUrl = AppConfig.RequestUseGithub ? AppConfig.GithubLangsApi : AppConfig.GiteeLangsApi;
                XmlDocument doc = client.GetWebJsonToXml(apiUrl);
                if(doc == null)
                {
                    AppMessageBox.Show(AppString.Message.WebDataReadFailed);
                    return;
                }
                XmlNodeList list = doc.FirstChild.ChildNodes;
                string[] langs = new string[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    XmlNode nameXN = list.Item(i).SelectSingleNode("name");
                    langs[i] = Path.GetFileNameWithoutExtension(nameXN.InnerText);
                }
                if(langs.Length == 0)
                {
                    AppMessageBox.Show(AppString.Message.WebDataReadFailed);
                    return;
                }
                using(SelectDialog dlg = new SelectDialog())
                {
                    dlg.Items = langs;
                    dlg.Title = AppString.Dialog.DownloadLanguages;
                    string lang = CultureInfo.CurrentUICulture.Name;
                    if(dlg.Items.Contains(lang)) dlg.Selected = lang;
                    else dlg.SelectedIndex = 0;
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = $"{dlg.Selected}.ini";
                        string filePath = $@"{AppConfig.LangsDir}\{fileName}";
                        string dirUrl = AppConfig.RequestUseGithub ? AppConfig.GithubLangsRawDir : AppConfig.GiteeLangsRawDir;
                        string fileUrl = $"{dirUrl}/{fileName}";
                        bool flag = client.WebStringToFile(filePath, fileUrl);
                        if(!flag)
                        {
                            if(AppMessageBox.Show(AppString.Message.WebDataReadFailed + "\r\n ● " + fileName + "\r\n"
                                + AppString.Message.OpenWebUrl, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                ExternalProgram.OpenWebUrl(fileUrl);
                            }
                        }
                        else
                        {
                            this.LoadLanguages();
                            string language = new IniWriter(filePath).GetValue("General", "Language");
                            if(language == "") language = dlg.Selected;
                            cmbLanguages.Text = language;
                            ChangeLanguage();
                        }
                    }
                }
            }
        }

        sealed class TranslateDialog : CommonDialog
        {
            public override void Reset() { }

            protected override bool RunDialog(IntPtr hwndOwner)
            {
                using(TranslateForm frm = new TranslateForm())
                {
                    frm.TopMost = AppConfig.TopMost;
                    return frm.ShowDialog() == DialogResult.OK;
                }
            }

            sealed class TranslateForm : Form
            {
                public TranslateForm()
                {
                    this.SuspendLayout();
                    this.CancelButton = btnCancel;
                    this.Font = SystemFonts.MessageBoxFont;
                    this.SizeGripStyle = SizeGripStyle.Hide;
                    this.Text = AppString.Dialog.TranslateTool;
                    this.ShowInTaskbar = this.MinimizeBox = false;
                    this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    this.StartPosition = FormStartPosition.CenterParent;
                    this.InitializeComponents();
                    this.ResumeLayout();
                }

                readonly Label lblSections = new Label
                {
                    AutoSize = true,
                    Text = "Section"
                };
                readonly Label lblKeys = new Label
                {
                    AutoSize = true,
                    Text = "Key"
                };
                readonly Label lblDefault = new Label
                {
                    Text = AppString.Dialog.DefaultText,
                    AutoSize = true
                };
                readonly Label lblOld = new Label
                {
                    Text = AppString.Dialog.OldTranslation,
                    AutoSize = true
                };
                readonly Label lblNew = new Label
                {
                    Text = AppString.Dialog.NewTranslation,
                    AutoSize = true
                };
                readonly TextBox txtDefault = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true
                };
                readonly TextBox txtOld = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true
                };
                readonly TextBox txtNew = new TextBox
                {
                    Multiline = true
                };
                readonly ComboBox cmbSections = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                readonly ComboBox cmbKeys = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                readonly Button btnBrowse = new Button
                {
                    Text = AppString.Dialog.Browse,
                    AutoSize = true
                };
                readonly Button btnSave = new Button
                {
                    Text = AppString.Menu.Save,
                    AutoSize = true
                };
                readonly Button btnCancel = new Button
                {
                    DialogResult = DialogResult.Cancel,
                    Text = ResourceString.Cancel,
                    AutoSize = true
                };

                static TranslateForm()
                {
                    foreach(string section in AppString.DefLangReader.Sections)
                    {
                        var dic = new Dictionary<string, string>();
                        foreach(string key in AppString.DefLangReader.GetSectionKeys(section))
                        {
                            dic.Add(key, string.Empty);
                        }
                        EditingDic.Add(section, dic);
                    }
                }

                static readonly Dictionary<string, Dictionary<string, string>> EditingDic
                    = new Dictionary<string, Dictionary<string, string>>();

                static readonly IniWriter ReferentialWirter = new IniWriter();

                private string Section => cmbSections.Text;
                private string Key => cmbKeys.Text;

                private void InitializeComponents()
                {
                    this.Controls.AddRange(new Control[] { lblSections, cmbSections, lblKeys,
                    cmbKeys, lblDefault, txtDefault, lblOld, txtOld, lblNew,
                    txtNew, btnBrowse, btnSave, btnCancel });

                    txtDefault.SetAutoShowScroll(ScrollBars.Vertical);
                    txtOld.SetAutoShowScroll(ScrollBars.Vertical);
                    txtNew.SetAutoShowScroll(ScrollBars.Vertical);
                    txtDefault.CanSelectAllWhenReadOnly();
                    txtOld.CanSelectAllWhenReadOnly();
                    cmbSections.AutosizeDropDownWidth();
                    cmbKeys.AutosizeDropDownWidth();

                    int a = 20.DpiZoom();

                    lblSections.Top = lblSections.Left = cmbSections.Top = lblKeys.Left
                        = lblDefault.Left = lblOld.Left = lblNew.Left = btnBrowse.Left = a;

                    lblKeys.Top = cmbKeys.Top = cmbSections.Bottom + a;
                    lblDefault.Top = txtDefault.Top = cmbKeys.Bottom + a;
                    txtDefault.Height = txtOld.Height = txtNew.Height = 4 * a;
                    cmbSections.Width = cmbKeys.Width = txtDefault.Width = txtOld.Width = txtNew.Width = 20 * a;

                    int h = cmbSections.Height + cmbKeys.Height + btnBrowse.Height;
                    int[] ws = { lblSections.Width, lblKeys.Width, lblDefault.Width, lblOld.Width, lblNew.Width };
                    int w = ws.Max();

                    cmbSections.Left = cmbKeys.Left = txtDefault.Left = txtOld.Left = txtNew.Left = w + 2 * a;

                    this.Resize += (sender, e) =>
                    {
                        txtDefault.Height = txtOld.Height = txtNew.Height
                            = (this.ClientSize.Height - h - 7 * a) / 3;

                        lblOld.Top = txtOld.Top = txtDefault.Bottom + a;
                        lblNew.Top = txtNew.Top = txtOld.Bottom + a;
                        btnBrowse.Top = btnSave.Top = btnCancel.Top = txtNew.Bottom + a;

                        cmbSections.Width = cmbKeys.Width = txtDefault.Width = txtOld.Width = txtNew.Width
                            = this.ClientSize.Width - (w + 3 * a);

                        btnCancel.Left = this.ClientSize.Width - btnCancel.Width - a;
                        btnSave.Left = btnCancel.Left - btnSave.Width - a;
                        btnBrowse.Left = btnSave.Left - btnBrowse.Width - a;
                    };
                    this.ClientSize = new Size(w + 23 * a, h + 3 * 4 * a + 7 * a);
                    this.MinimumSize = this.Size;

                    cmbSections.Items.AddRange(AppString.DefLangReader.Sections);
                    cmbSections.SelectedIndexChanged += (sender, e) =>
                    {
                        cmbKeys.Items.Clear();
                        cmbKeys.Items.AddRange(AppString.DefLangReader.GetSectionKeys(Section));
                        cmbKeys.SelectedIndex = 0;
                    };
                    cmbKeys.SelectedIndexChanged += (sender, e) =>
                    {
                        txtDefault.Text = AppString.DefLangReader.GetValue(Section, Key).Replace("\\r", "\r").Replace("\\n", "\n");
                        txtOld.Text = ReferentialWirter.GetValue(Section, Key).Replace("\\r", "\r").Replace("\\n", "\n");
                        txtNew.Text = EditingDic[Section][Key].Replace("\\r", "\r").Replace("\\n", "\n");
                    };
                    cmbSections.SelectedIndex = 0;

                    txtOld.TextChanged += (sender, e) => { if(txtNew.Text == string.Empty) txtNew.Text = txtOld.Text; };
                    txtNew.TextChanged += (sender, e) => EditingDic[Section][Key] = txtNew.Text.Replace("\n", "\\n").Replace("\r", "\\r");
                    btnBrowse.Click += (sender, e) => SelectFile();
                    btnSave.Click += (sender, e) => Save();
                }

                private void SelectFile()
                {
                    using(OpenFileDialog dlg = new OpenFileDialog())
                    {
                        dlg.InitialDirectory = AppConfig.LangsDir;
                        dlg.Filter = $"{AppString.SideBar.AppLanguage}|*.ini";
                        if(dlg.ShowDialog() != DialogResult.OK) return;
                        ReferentialWirter.FilePath = dlg.FileName;
                        txtOld.Text = ReferentialWirter.GetValue(Section, Key).Replace("\\r", "\r").Replace("\\n", "\n");
                    }
                }

                private void Save()
                {
                    using(SaveFileDialog dlg = new SaveFileDialog())
                    {
                        string language = EditingDic["General"]["Language"];
                        int index = language.IndexOf(' ');
                        if(index > 0) language = language.Substring(0, index);
                        dlg.FileName = $"{language}.ini";
                        dlg.InitialDirectory = AppConfig.LangsDir;
                        dlg.Filter = $"{AppString.SideBar.AppLanguage}|*.ini";
                        if(dlg.ShowDialog() != DialogResult.OK) return;

                        string contents = string.Empty;
                        foreach(string section in EditingDic.Keys)
                        {
                            contents += $"[{section}]" + "\r\n";
                            foreach(string key in EditingDic[section].Keys)
                            {
                                string value = EditingDic[section][key];
                                contents += $"{key} = {value}" + "\r\n";
                            }
                            contents += "\r\n";
                        }
                        File.WriteAllText(dlg.FileName, contents, Encoding.Unicode);
                    }
                }
            }
        }
    }
}
