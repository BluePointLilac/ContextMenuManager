using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class WinXList : MyList
    {
        public static readonly string WinXPath = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Windows\WinX");
        public static readonly string DefaultWinXPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%\Users\Default\AppData\Local\Microsoft\Windows\WinX");

        public void LoadItems()
        {
            if(WindowsOsVersion.ISAfterOrEqual8)
            {
                this.AddNewItem();
                this.LoadWinXItems();
            }
        }

        private void LoadWinXItems()
        {
            string[] dirPaths = Directory.GetDirectories(WinXPath);
            Array.Reverse(dirPaths);
            bool sortable = AppConfig.WinXSortable;
            bool sorted = false;
            foreach(string dirPath in dirPaths)
            {
                WinXGroupItem groupItem = new WinXGroupItem(dirPath);
                this.AddItem(groupItem);
                string[] lnkPaths;
                if(sortable)
                {
                    lnkPaths = GetSortedPaths(dirPath, out bool flag);
                    if(flag) sorted = true;
                }
                else
                {
                    lnkPaths = Directory.GetFiles(dirPath, "*.lnk");
                    Array.Reverse(lnkPaths);
                }
                foreach(string path in lnkPaths)
                {
                    WinXItem winXItem = new WinXItem(path, groupItem);
                    winXItem.BtnMoveDown.Visible = winXItem.BtnMoveUp.Visible = sortable;
                    this.AddItem(winXItem);
                }
                groupItem.IsFold = true;
            }
            if(sorted)
            {
                ExplorerRestarter.Show();
                MessageBoxEx.Show(AppString.Message.WinXSorted);
            }
        }

        private void AddNewItem()
        {
            NewItem newItem = new NewItem();
            this.AddItem(newItem);
            PictureButton btnCreateDir = new PictureButton(AppImage.NewFolder);
            MyToolTip.SetToolTip(btnCreateDir, AppString.Tip.CreateGroup);
            newItem.AddCtr(btnCreateDir);
            btnCreateDir.MouseDown += (sender, e) => CreateNewGroup();
            newItem.AddNewItem += (sender, e) =>
            {
                using(NewLnkFileDialog dlg1 = new NewLnkFileDialog())
                {
                    if(dlg1.ShowDialog() != DialogResult.OK) return;
                    using(SelectDialog dlg2 = new SelectDialog())
                    {
                        dlg2.Title = AppString.Dialog.SelectGroup;
                        dlg2.Items = GetGroupNames();
                        if(dlg2.ShowDialog() != DialogResult.OK) return;
                        string dirPath = $@"{WinXPath}\{dlg2.Selected}";
                        string extension = Path.GetExtension(dlg1.ItemFilePath).ToLower();
                        string fileName = Path.GetFileNameWithoutExtension(dlg1.ItemFilePath);
                        int count = Directory.GetFiles(dirPath, "*.lnk").Length;
                        string index = (count + 1).ToString().PadLeft(2, '0');
                        string lnkName = $"{index} - {fileName}.lnk";
                        string lnkPath = $@"{dirPath}\{lnkName}";
                        ShellLink shellLink;
                        if(extension == ".lnk")
                        {
                            File.Copy(dlg1.ItemFilePath, lnkPath);
                            shellLink = new ShellLink(lnkPath);
                        }
                        else
                        {
                            shellLink = new ShellLink(lnkPath)
                            {
                                TargetPath = dlg1.ItemFilePath,
                                Arguments = dlg1.Arguments,
                            };
                            shellLink.WorkingDirectory = Path.GetDirectoryName(shellLink.TargetPath);
                        }
                        shellLink.Description = dlg1.ItemText;
                        shellLink.Save();
                        DesktopIni.SetLocalizedFileNames(lnkPath, dlg1.ItemText);
                        foreach(MyListItem ctr in this.Controls)
                        {
                            if(ctr is WinXGroupItem groupItem && groupItem.Text == dlg2.Selected)
                            {
                                WinXItem item = new WinXItem(lnkPath, groupItem) { Visible = !groupItem.IsFold };
                                item.BtnMoveDown.Visible = item.BtnMoveUp.Visible = AppConfig.WinXSortable;
                                this.InsertItem(item, this.GetItemIndex(groupItem) + 1);
                                break;
                            }
                        }
                        WinXHasher.HashLnk(lnkPath);
                        ExplorerRestarter.Show();
                    }
                }
            };
        }

        private void CreateNewGroup()
        {
            string dirPath = ObjectPath.GetNewPathWithIndex($@"{WinXPath}\Group", ObjectPath.PathType.Directory, 1);
            Directory.CreateDirectory(dirPath);
            string iniPath = $@"{dirPath}\desktop.ini";
            File.WriteAllText(iniPath, string.Empty, Encoding.Unicode);
            File.SetAttributes(dirPath, File.GetAttributes(dirPath) | FileAttributes.ReadOnly);
            File.SetAttributes(iniPath, File.GetAttributes(iniPath) | FileAttributes.Hidden | FileAttributes.System);
            this.InsertItem(new WinXGroupItem(dirPath), 1);
        }

        public static string[] GetGroupNames()
        {
            List<string> items = new List<string>();
            DirectoryInfo winxDi = new DirectoryInfo(WinXPath);
            Array.ForEach(winxDi.GetDirectories(), di => items.Add(di.Name));
            items.Reverse();
            return items.ToArray();
        }

        private static string[] GetSortedPaths(string groupPath, out bool sorted)
        {
            sorted = false;
            List<string> sortedPaths = new List<string>();
            string[] paths = Directory.GetFiles(groupPath, "*.lnk");
            for(int i = paths.Length - 1; i >= 0; i--)
            {
                string srcPath = paths[i];
                string name = Path.GetFileName(srcPath);
                int index = name.IndexOf(" - ");
                if(index >= 2 && int.TryParse(name.Substring(0, index), out int num) && num == i + 1)
                {
                    sortedPaths.Add(srcPath); continue;
                }
                string dstPath = $@"{groupPath}\{(i + 1).ToString().PadLeft(2, '0')} - {name.Substring(index + 3)}";
                dstPath = ObjectPath.GetNewPathWithIndex(dstPath, ObjectPath.PathType.File);
                string value = DesktopIni.GetLocalizedFileNames(srcPath);
                DesktopIni.DeleteLocalizedFileNames(srcPath);
                if(value != string.Empty) DesktopIni.SetLocalizedFileNames(dstPath, value);
                File.Move(srcPath, dstPath);
                sortedPaths.Add(dstPath);
                sorted = true;
            }
            return sortedPaths.ToArray();
        }
    }
}