using BulePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class NewShellDialog : CommonDialog
    {
        public string ShellPath { get; set; }//传入的Shell注册表路径
        public string ScenePath { get; set; }//菜单项所处环境注册表路径
        public string NewItemRegPath { get; private set; }//返回的新ShellItem的注册表路径
        public string NewItemKeyName => RegistryEx.GetKeyName(NewItemRegPath);

        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(NewShellItemForm frm = new NewShellItemForm
            {
                ScenePath = this.ScenePath,
                ShellPath = this.ShellPath
            })
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.NewItemRegPath = frm.NewItemRegPath;
                return flag;
            }
        }

        sealed class NewShellItemForm : NewItemForm
        {
            public string ShellPath { get; set; }
            public string NewItemRegPath { get; private set; }//返回的新建菜单项注册表路径
            public string ScenePath { get; set; }//菜单所处环境路径，用于判断添加后缀

            readonly RadioButton rdoSingle = new RadioButton
            {
                Text = AppString.Text.Single,
                AutoSize = true,
                Checked = true
            };
            readonly RadioButton rdoMulti = new RadioButton
            {
                Text = AppString.Text.Multi,
                AutoSize = true
            };

            static readonly string[] DirScenePaths = {
                ShellList.MENUPATH_DIRECTORY,
                ShellList.MENUPATH_DIRECTORY_IMAGE,
                ShellList.MENUPATH_DIRECTORY_VIDEO,
                ShellList.MENUPATH_DIRECTORY_AUDIO
            };
            static readonly string[] FileObjectsScenePaths = {
                ShellList.MENUPATH_FILE,
                ShellList.MENUPATH_FOLDER,
                ShellList.MENUPATH_ALLOBJECTS,
                ShellList.SYSFILEASSPATH,
                ShellList.MENUPATH_UNKNOWN
            };

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                this.Text = AppString.Text.NewShellItem;
                this.Controls.AddRange(new[] { rdoSingle, rdoMulti });
                rdoSingle.Top = rdoMulti.Top = btnOk.Top;
                rdoSingle.Left = lblCommand.Left;
                rdoMulti.Left = rdoSingle.Right + 20.DpiZoom();

                rdoMulti.CheckedChanged += (sender, e) =>
                {
                    lblCommand.Enabled = txtCommand.Enabled
                    = btnBrowse.Enabled = !rdoMulti.Checked;
                };

                btnBrowse.Click += (sender, e) => BrowseFile();

                btnOk.Click += (sender, e) =>
                {
                    if(string.IsNullOrWhiteSpace(txtText.Text))
                    {
                        MessageBoxEx.Show(AppString.MessageBox.TextCannotBeEmpty);
                    }
                    else
                    {
                        AddNewItem();
                        DialogResult = DialogResult.OK;
                    }
                };
            }

            private void BrowseFile()
            {
                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = $"{AppString.Indirect.Programs}|*.exe;*.bat;*.cmd;*.pif;*.com";
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemCommand = $"\"{dlg.FileName}\"";
                    ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    if(Array.FindIndex(DirScenePaths, path
                       => ScenePath.Equals(path, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        ItemCommand += " \"%V\"";//自动加目录后缀
                    }
                    else if(Array.FindIndex(FileObjectsScenePaths, path
                       => ScenePath.StartsWith(path, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        ItemCommand += " \"%1\"";//自动加文件对象后缀
                    }
                }
            }

            private void AddNewItem()
            {
                using(var shellKey = RegistryEx.GetRegistryKey(ShellPath, true, true))
                {
                    string keyName = ItemText.Replace("\\", "").Trim();
                    NewItemRegPath = ObjectPath.GetNewPathWithIndex($@"{ShellPath}\{keyName}", ObjectPath.PathType.Registry);
                    keyName = RegistryEx.GetKeyName(NewItemRegPath);

                    using(var key = shellKey.CreateSubKey(keyName, true))
                    {
                        key.SetValue("MUIVerb", ItemText);
                        if(rdoMulti.Checked)
                            key.SetValue("SubCommands", "");
                        else
                        {
                            if(!string.IsNullOrWhiteSpace(ItemCommand))
                                key.CreateSubKey("command", true).SetValue("", ItemCommand);
                        }
                    }
                }
            }
        }
    }
}