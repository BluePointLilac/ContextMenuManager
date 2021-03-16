using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class TranslateDialog : CommonDialog
    {
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(TranslateForm frm = new TranslateForm())
            {
                return frm.ShowDialog() == DialogResult.OK;
            }
        }

        sealed class TranslateForm : Form
        {
            public TranslateForm()
            {
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.Text = AppString.Dialog.TranslateTool;
                this.StartPosition = FormStartPosition.CenterParent;
                this.ShowIcon = this.ShowInTaskbar = this.MinimizeBox = false;
                this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10F);
                this.InitializeComponents();
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
                Text = AppString.Dialog.Cancel,
                AutoSize = true
            };

            static TranslateForm()
            {
                foreach(string section in DefaultDic.Keys)
                {
                    var dic = new Dictionary<string, string>();
                    foreach(string key in DefaultDic[section].Keys)
                    {
                        dic.Add(key, string.Empty);
                    }
                    EditingDic.Add(section, dic);
                }
            }

            static readonly Dictionary<string, Dictionary<string, string>> EditingDic
                = new Dictionary<string, Dictionary<string, string>>();
            static readonly Dictionary<string, Dictionary<string, string>> DefaultDic
                = AppString.DefaultLanguage.RootDic;

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
                    btnSave.Left = btnCancel.Left - btnSave.Width - 2 * a;
                    btnBrowse.Left = btnSave.Left - btnBrowse.Width - 2 * a;
                };
                this.ClientSize = new Size(w + 23 * a, h + 3 * 4 * a + 7 * a);
                this.MinimumSize = this.Size;

                cmbSections.Items.AddRange(DefaultDic.Keys.ToArray());
                cmbSections.SelectedIndexChanged += (sender, e) =>
                {
                    cmbKeys.Items.Clear();
                    cmbKeys.Items.AddRange(DefaultDic[Section].Keys.ToArray());
                    cmbKeys.SelectedIndex = 0;
                };
                cmbKeys.SelectedIndexChanged += (sender, e) =>
                {
                    txtNew.Text = EditingDic[Section][Key].Replace("\\n", Environment.NewLine);
                    txtDefault.Text = DefaultDic[Section][Key].Replace("\\n", Environment.NewLine);
                    txtOld.Text = ReferentialWirter.GetValue(Section, Key).Replace("\\n", Environment.NewLine);
                };
                cmbSections.SelectedIndex = 0;

                txtOld.TextChanged += (sender, e) => { if(txtNew.Text == string.Empty) txtNew.Text = txtOld.Text; };
                txtNew.TextChanged += (sender, e) => EditingDic[Section][Key] = txtNew.Text.Replace(Environment.NewLine, "\\n");
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
                    txtOld.Text = ReferentialWirter.GetValue(Section, Key).Replace("\\n", "\n");
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
                        contents += $"[{section}]" + Environment.NewLine;
                        foreach(string key in EditingDic[section].Keys)
                        {
                            string value = EditingDic[section][key];
                            contents += $"{key} = {value}" + Environment.NewLine;
                        }
                        contents += Environment.NewLine;
                    }
                    File.WriteAllText(dlg.FileName, contents, Encoding.Unicode);
                }
            }
        }
    }
}