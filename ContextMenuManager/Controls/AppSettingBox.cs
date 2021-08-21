using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class AppSettingBox : MyList
    {
        public AppSettingBox()
        {
            this.SuspendLayout();
            this.Font = SystemFonts.MenuFont;
            this.Font = new Font(this.Font.FontFamily, this.Font.Size + 1F);
            mliConfigDir.AddCtrs(new Control[] { cmbConfigDir, btnConfigDir });
            mliBackup.AddCtrs(new Control[] { chkBackup, btnBackupDir });
            mliUpdate.AddCtrs(new Control[] { cmbUpdate, btnUpdate });
            mliRepo.AddCtr(cmbRepo);
            mliTopMost.AddCtr(chkTopMost);
            mliProtect.AddCtr(chkProtect);
            mliEngine.AddCtr(cmbEngine);
            mliWinXSortable.AddCtr(chkWinXSortable);
            mliShowFilePath.AddCtr(chkShowFilePath);
            mliOpenMoreRegedit.AddCtr(chkOpenMoreRegedit);
            mliOpenMoreExplorer.AddCtr(chkOpenMoreExplorer);
            mliHideDisabledItems.AddCtr(chkHideDisabledItems);
            mliHideSysStoreItems.AddCtr(chkHideSysStoreItems);

            ToolTipBox.SetToolTip(btnUpdate, AppString.Tip.ImmediatelyCheck);
            ToolTipBox.SetToolTip(cmbConfigDir, AppString.Tip.ConfigPath);
            ToolTipBox.SetToolTip(btnConfigDir, AppString.Menu.FileLocation);
            ToolTipBox.SetToolTip(btnBackupDir, AppString.Menu.FileLocation);

            cmbRepo.Items.AddRange(new[] { "Github", "Gitee" });
            cmbConfigDir.Items.AddRange(new[] { AppString.Other.AppDataDir, AppString.Other.AppDir });
            cmbEngine.Items.AddRange(AppConfig.EngineUrlsDic.Keys.ToArray());
            cmbEngine.Items.Add(AppString.Other.CustomEngine);
            cmbUpdate.Items.AddRange(new[] { AppString.Other.OnceAWeek, AppString.Other.OnceAMonth,
                AppString.Other.OnceASeason, AppString.Other.NeverCheck });

            cmbConfigDir.Width = cmbEngine.Width = cmbUpdate.Width = cmbRepo.Width = 120.DpiZoom();
            cmbConfigDir.DropDownStyle = cmbEngine.DropDownStyle = cmbUpdate.DropDownStyle
                = cmbRepo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConfigDir.AutosizeDropDownWidth();
            cmbEngine.AutosizeDropDownWidth();
            cmbUpdate.AutosizeDropDownWidth();
            cmbRepo.AutosizeDropDownWidth();

            btnUpdate.MouseDown += (sender, e) =>
            {
                this.Cursor = Cursors.WaitCursor;
                Updater.Update(true);
                this.Cursor = Cursors.Default;
            };
            btnConfigDir.MouseDown += (sender, e) => ExternalProgram.OpenDirectory(AppConfig.ConfigDir);
            btnBackupDir.MouseDown += (sender, e) => ExternalProgram.OpenDirectory(AppConfig.BackupDir);
            chkBackup.CheckChanged += () => AppConfig.AutoBackup = chkBackup.Checked;
            chkProtect.CheckChanged += () => AppConfig.ProtectOpenItem = chkProtect.Checked;
            chkWinXSortable.CheckChanged += () => AppConfig.WinXSortable = chkWinXSortable.Checked;
            chkOpenMoreRegedit.CheckChanged += () => AppConfig.OpenMoreRegedit = chkOpenMoreRegedit.Checked;
            chkTopMost.CheckChanged += () => AppConfig.TopMost = this.FindForm().TopMost = chkTopMost.Checked;
            chkOpenMoreExplorer.CheckChanged += () => AppConfig.OpenMoreExplorer = chkOpenMoreExplorer.Checked;
            chkHideDisabledItems.CheckChanged += () => AppConfig.HideDisabledItems = chkHideDisabledItems.Checked;
            chkHideSysStoreItems.CheckChanged += () => AppConfig.HideSysStoreItems = chkHideSysStoreItems.Checked;
            cmbRepo.SelectionChangeCommitted += (sender, e) => AppConfig.RequestUseGithub = cmbRepo.SelectedIndex == 0;
            chkShowFilePath.CheckChanged += () => AppConfig.ShowFilePath = chkShowFilePath.Checked;
            cmbUpdate.SelectionChangeCommitted += (sender, e) => ChangeUpdateFrequency();
            cmbConfigDir.SelectionChangeCommitted += (sender, e) => ChangeConfigDir();
            cmbEngine.SelectionChangeCommitted += (sender, e) => ChangeEngineUrl();
            this.ResumeLayout();
        }

        readonly MyListItem mliConfigDir = new MyListItem
        {
            Text = AppString.Other.ConfigPath
        };
        readonly ComboBox cmbConfigDir = new ComboBox();
        readonly PictureButton btnConfigDir = new PictureButton(AppImage.Open);

        readonly MyListItem mliRepo = new MyListItem
        {
            Text = AppString.Other.SetRequestRepo
        };
        readonly ComboBox cmbRepo = new ComboBox();

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
        readonly ComboBox cmbUpdate = new ComboBox();
        readonly PictureButton btnUpdate = new PictureButton(AppImage.CheckUpdate);

        readonly MyListItem mliTopMost = new MyListItem
        {
            Text = AppString.Other.TopMost
        };
        readonly MyCheckBox chkTopMost = new MyCheckBox();

        readonly MyListItem mliProtect = new MyListItem
        {
            Text = AppString.Other.ProtectOpenItem
        };
        readonly MyCheckBox chkProtect = new MyCheckBox();

        readonly MyListItem mliEngine = new MyListItem
        {
            Text = AppString.Other.WebSearchEngine
        };
        readonly ComboBox cmbEngine = new ComboBox();

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

        readonly MyListItem mliOpenMoreExplorer = new MyListItem
        {
            Text = AppString.Other.OpenMoreExplorer
        };
        readonly MyCheckBox chkOpenMoreExplorer = new MyCheckBox();

        readonly MyListItem mliHideDisabledItems = new MyListItem
        {
            Text = AppString.Other.HideDisabledItems
        };
        readonly MyCheckBox chkHideDisabledItems = new MyCheckBox();

        readonly MyListItem mliWinXSortable = new MyListItem
        {
            Text = AppString.Other.WinXSortable,
            Visible = WinOsVersion.Current >= WinOsVersion.Win8
        };
        readonly MyCheckBox chkWinXSortable = new MyCheckBox();

        readonly MyListItem mliHideSysStoreItems = new MyListItem
        {
            Text = AppString.Other.HideSysStoreItems,
            Visible = WinOsVersion.Current >= WinOsVersion.Win7
        };
        readonly MyCheckBox chkHideSysStoreItems = new MyCheckBox();

        public override void ClearItems()
        {
            this.Controls.Clear();
        }

        public void LoadItems()
        {
            this.AddItems(new[] { mliConfigDir, mliUpdate, mliRepo, mliEngine, mliBackup, mliTopMost, mliProtect, mliShowFilePath,
                mliHideDisabledItems, mliHideSysStoreItems, mliOpenMoreRegedit, mliOpenMoreExplorer, mliWinXSortable });
            foreach(MyListItem item in this.Controls) item.HasImage = false;
            cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
            cmbRepo.SelectedIndex = AppConfig.RequestUseGithub ? 0 : 1;
            cmbUpdate.SelectedIndex = GetUpdateSelectIndex();
            cmbEngine.SelectedIndex = GetEngineSelectIndex();
            chkBackup.Checked = AppConfig.AutoBackup;
            chkTopMost.Checked = this.FindForm().TopMost;
            chkProtect.Checked = AppConfig.ProtectOpenItem;
            chkWinXSortable.Checked = AppConfig.WinXSortable;
            chkShowFilePath.Checked = AppConfig.ShowFilePath;
            chkOpenMoreRegedit.Checked = AppConfig.OpenMoreRegedit;
            chkOpenMoreExplorer.Checked = AppConfig.OpenMoreExplorer;
            chkHideDisabledItems.Checked = AppConfig.HideDisabledItems;
            chkHideSysStoreItems.Checked = AppConfig.HideSysStoreItems;
        }

        private void ChangeConfigDir()
        {
            string newPath = (cmbConfigDir.SelectedIndex == 0) ? AppConfig.AppDataConfigDir : AppConfig.AppConfigDir;
            if(newPath == AppConfig.ConfigDir) return;
            if(AppMessageBox.Show(AppString.Message.RestartApp, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                cmbConfigDir.SelectedIndex = AppConfig.SaveToAppDir ? 1 : 0;
            }
            else
            {
                DirectoryEx.CopyTo(AppConfig.ConfigDir, newPath);
                Directory.Delete(AppConfig.ConfigDir, true);
                SingleInstance.Restart();
            }
        }

        private void ChangeEngineUrl()
        {
            if(cmbEngine.SelectedIndex < cmbEngine.Items.Count - 1)
            {
                AppConfig.EngineUrl = AppConfig.EngineUrlsDic[cmbEngine.Text];
            }
            else
            {
                using(InputDialog dlg = new InputDialog())
                {
                    dlg.Text = AppConfig.EngineUrl;
                    dlg.Title = AppString.Other.SetCustomEngine;
                    if(dlg.ShowDialog() == DialogResult.OK) AppConfig.EngineUrl = dlg.Text;
                    cmbEngine.SelectedIndex = GetEngineSelectIndex();
                }
            }
        }

        private void ChangeUpdateFrequency()
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
        }

        private int GetUpdateSelectIndex()
        {
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
            return index;
        }

        private int GetEngineSelectIndex()
        {
            string[] urls = AppConfig.EngineUrlsDic.Values.ToArray();
            for(int i = 0; i < urls.Length; i++)
            {
                if(AppConfig.EngineUrl.Equals(urls[i])) return i;
            }
            return cmbEngine.Items.Count - 1;
        }
    }
}