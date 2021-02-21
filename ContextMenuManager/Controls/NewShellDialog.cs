using BluePointLilac.Methods;
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
            using(NewShellForm frm = new NewShellForm
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

        sealed class NewShellForm : NewItemForm
        {
            public string ShellPath { get; set; }
            public string NewItemRegPath { get; private set; }//返回的新建菜单项注册表路径
            public string ScenePath { get; set; }//菜单所处环境路径，用于判断添加后缀

            readonly RadioButton rdoSingle = new RadioButton
            {
                Text = AppString.Dialog.SingleMenu,
                AutoSize = true,
                Checked = true
            };
            readonly RadioButton rdoMulti = new RadioButton
            {
                Text = AppString.Dialog.MultiMenu,
                AutoSize = true
            };
            readonly ShellExecuteCheckBox chkSE = new ShellExecuteCheckBox();

            static readonly string[] DirScenePaths = {
                ShellList.MENUPATH_DIRECTORY,
                $@"{ShellList.SYSFILEASSPATH}\Directory."
            };
            static readonly string[] FileObjectsScenePaths = {
                ShellList.MENUPATH_FILE,
                ShellList.MENUPATH_FOLDER,
                ShellList.MENUPATH_ALLOBJECTS,
                ShellList.SYSFILEASSPATH,
                ShellList.MENUPATH_UNKNOWN,
                ShellList.MENUPATH_UWPLNK
            };

            protected override void InitializeComponents()
            {
                base.InitializeComponents();
                this.Controls.AddRange(new Control[] { rdoSingle, rdoMulti, chkSE });
                rdoSingle.Top = rdoMulti.Top = btnOk.Top;
                rdoSingle.Left = lblCommand.Left;
                rdoMulti.Left = rdoSingle.Right + 20.DpiZoom();
                chkSE.Top = txtArguments.Top + (txtArguments.Height - chkSE.Height) / 2;
                this.Resize += (sender, e) => chkSE.Left = txtArguments.Right + 20.DpiZoom();
                this.OnResize(null);

                rdoMulti.CheckedChanged += (sender, e) =>
                {
                    if(WindowsOsVersion.IsEqualVista && rdoMulti.Checked)
                    {
                        MessageBoxEx.Show(AppString.MessageBox.VistaUnsupportedMulti);
                        rdoSingle.Checked = true;
                        return;
                    }
                    lblCommand.Enabled = txtFilePath.Enabled = lblArguments.Enabled
                    = txtArguments.Enabled = btnBrowse.Enabled = chkSE.Enabled = !rdoMulti.Checked;
                };

                btnBrowse.Click += (sender, e) => BrowseFile();

                btnOk.Click += (sender, e) =>
                {
                    if(txtText.Text.IsNullOrWhiteSpace())
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
                    dlg.Filter = $"{AppString.Dialog.Program}|*.exe;*.bat;*.cmd;*.pif;*.com;*.vbs;*.vbe;*.js;*.jse;*.wsf";
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemText = Path.GetFileNameWithoutExtension(dlg.FileName);
                    string extension = Path.GetExtension(dlg.FileName).ToLower();
                    switch(extension)
                    {
                        case ".vbs":
                        case ".vbe":
                        case ".js":
                        case ".jse":
                        case ".wsf":
                            ItemFilePath = "wscript.exe";
                            Arguments = dlg.FileName;
                            break;
                        default:
                            ItemFilePath = dlg.FileName;
                            break;
                    }
                    if(Array.FindIndex(DirScenePaths, path
                       => ScenePath.StartsWith(path, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        if(!Arguments.IsNullOrWhiteSpace()) Arguments += " ";
                        if(ScenePath != ShellList.MENUPATH_BACKGROUND)
                            Arguments += "\"%V\"";//自动加目录后缀
                    }
                    else if(Array.FindIndex(FileObjectsScenePaths, path
                       => ScenePath.StartsWith(path, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        if(!Arguments.IsNullOrWhiteSpace()) Arguments += " ";
                        Arguments += "\"%1\"";//自动加文件对象后缀
                    }
                }
            }

            private void AddNewItem()
            {
                using(var shellKey = RegistryEx.GetRegistryKey(ShellPath, true, true))
                {
                    string keyName = "Item";
                    NewItemRegPath = ObjectPath.GetNewPathWithIndex($@"{ShellPath}\{keyName}", ObjectPath.PathType.Registry, 0);
                    keyName = RegistryEx.GetKeyName(NewItemRegPath);

                    using(var key = shellKey.CreateSubKey(keyName, true))
                    {
                        key.SetValue("MUIVerb", ItemText);
                        if(rdoMulti.Checked)
                            key.SetValue("SubCommands", "");
                        else
                        {
                            if(!ItemCommand.IsNullOrWhiteSpace())
                            {
                                string command;
                                if(!chkSE.Checked) command = ItemCommand;
                                else command = ShellExecuteDialog.GetCommand(ItemFilePath, Arguments, chkSE.Verb, chkSE.WindowStyle);
                                key.CreateSubKey("command", true).SetValue("", command);
                            }
                        }
                    }
                }
            }
        }
    }
}