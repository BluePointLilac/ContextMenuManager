using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class ShellStoreDialog : CommonDialog
    {
        public List<string> SelectedKeyNames { get; private set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(ShellStoreForm frm = new ShellStoreForm())
            {
                bool flag = frm.ShowDialog() == DialogResult.OK;
                if(flag) this.SelectedKeyNames = frm.SelectedItemNames;
                return flag;
            }
        }

        sealed class ShellStoreForm : SelectItemsForm
        {
            /// <summary>系统原有项目</summary>
            private static readonly string[] SystemItemNames = { "Windows.aboutWindows", "Windows.AddColumns",
                "Windows.AddDevice", "Windows.AddMediaServer", "Windows.AddNetworkLocation", "Windows.AddPrinter",
                "Windows.AddRemovePrograms", "Windows.AddToFavorites", "Windows.Autoplay", "Windows.Backup",
                "Windows.BitLocker", "Windows.BitLocker.Encrypt", "Windows.BitLocker.Manage",
                "Windows.BitLocker.ResetPasswordPin", "Windows.burn", "Windows.Burn.Action", "Windows.change-passphrase",
                "Windows.change-pin", "Windows.ChangeIndexedLocations", "Windows.ChooseColumns", "Windows.CleanUp",
                "Windows.ClearAddressBarHistory", "Windows.ClearFrequentHistory", "Windows.clearRecentDocs",
                "Windows.closewindow", "Windows.cmd", "Windows.cmdPromptAsAdministrator", "Windows.CompressedFile.extract",
                "Windows.CompressedFile.ExtractTo", "Windows.CompressedFolder.extract", "Windows.CompressedItem.extract",
                "Windows.Computer.Manage", "Windows.connectNetworkDrive", "Windows.copy", "Windows.copyaspath",
                "Windows.CopyToBrowser", "Windows.CopyToMenu", "Windows.CscSync", "Windows.CscWorkOfflineOnline",
                "Windows.cut", "Windows.Defragment", "Windows.delete", "Windows.Dialog.DisconnectNetworkDrive",
                "Windows.DiscImage.burn", "Windows.DisconnectNetworkDrive", "Windows.DiskFormat",
                "Windows.DriveFolder.DisconnectNetworkDrive", "Windows.edit", "Windows.Eject", "Windows.email",
                "Windows.encrypt-bde", "Windows.encrypt-bde-elev", "Windows.Enqueue", "Windows.EraseDisc",
                "Windows.EraseDisc.Action", "Windows.fax", "Windows.FinishBurn", "Windows.folderoptions",
                "Windows.GroupByColumn", "Windows.help", "Windows.HideSelected", "Windows.HistoryVaultRestore",
                "Windows.HomeGroupCPL", "Windows.HomeGroupJoin", "Windows.HomeGroupPassword", "Windows.HomeGroupSharing",
                "Windows.HomeGroupTroubleshooter", "Windows.IconSize", "Windows.includeinlibrary", "Windows.invertselection",
                "Windows.layout", "Windows.LibraryChangeIcon", "Windows.LibraryDefaultSaveLocation",
                "Windows.LibraryIncludeInLibrary", "Windows.LibraryManageLibrary", "Windows.LibraryOptimizeLibraryFor",
                "Windows.LibraryPublicSaveLocation", "Windows.LibraryRestoreDefaults", "Windows.LibrarySelChangeIcon",
                "Windows.LibrarySelDefaultSaveLocation", "Windows.LibrarySelManageLibrary", "Windows.LibrarySelOptimizeLibraryFor",
                "Windows.LibrarySelPublicSaveLocation", "Windows.LibrarySelRestoreDefaults", "Windows.LibrarySelShowInNavPane",
                "Windows.LibraryShowInNavPane", "Windows.location.cmd", "Windows.location.cmdPromptAsAdministrator",
                "Windows.location.opennewprocess", "Windows.location.opennewtab", "Windows.location.opennewwindow",
                "Windows.location.Powershell", "Windows.location.PowershellAsAdmin", "Windows.manage-bde",
                "Windows.manage-bde-elev", "Windows.MapNetworkDrive", "Windows.menubar", "Windows.ModernShare",
                "Windows.mount", "Windows.MoveToBrowser", "Windows.MoveToMenu", "Windows.MultiVerb.cmd",
                "Windows.MultiVerb.cmdPromptAsAdministrator", "Windows.MultiVerb.opennewprocess", "Windows.MultiVerb.opennewtab",
                "Windows.MultiVerb.opennewwindow", "Windows.MultiVerb.Powershell", "Windows.MultiVerb.PowershellAsAdmin",
                "Windows.navpane", "Windows.NavPaneExpandToCurrentFolder", "Windows.NavPaneShowAllFolders",
                "Windows.NavPaneShowLibraries", "Windows.NetworkAndSharing", "Windows.NetworkViewDeviceWebpage",
                "Windows.newfolder", "Windows.newitem", "Windows.open", "Windows.OpenContainingFolder.opencontaining",
                "Windows.OpenControlPanel", "Windows.opennewprocess", "Windows.opennewtab", "Windows.opennewwindow",
                "Windows.OpenPrinterServerProperty", "Windows.OpenPrintQueue", "Windows.OpenSearch.openfilelocation",
                "Windows.OpenSearchViewSite", "Windows.OpenWith", "Windows.organize", "Windows.paste", "Windows.pastelink",
                "Windows.PermanentDelete", "Windows.PinToHome", "Windows.pintostartscreen", "Windows.play", "Windows.playall",
                "Windows.playmusic", "Windows.PowershellAsAdmin", "Windows.previewpane", "Windows.print", "Windows.properties",
                "Windows.readingpane", "Windows.recycle", "Windows.RecycleBin.Empty", "Windows.RecycleBin.Location.properties",
                "Windows.RecycleBin.properties", "Windows.RecycleBin.RestoreAll", "Windows.RecycleBin.RestoreItems",
                "Windows.RecycleBin.Selection.properties", "Windows.redo", "Windows.remotedesktop", "Windows.RemoveMediaServer",
                "Windows.removeproperties", "Windows.rename", "Windows.RibbonDelete", "Windows.RibbonPermissionsDialog",
                "Windows.RibbonShare", "Windows.RibbonSync.MakeAvailableOffline", "Windows.RibbonSync.SyncThisFolder",
                "Windows.RibbonSync.WorkOfflineOnline", "Windows.rotate270", "Windows.rotate90", "Windows.runas",
                "Windows.runasuser", "Windows.SearchActiveDirectory", "Windows.SearchClearMru", "Windows.SearchCloseTab",
                "Windows.SearchFilterDate", "Windows.SearchFilterKind", "Windows.SearchFilterMoreProperties",
                "Windows.SearchFilterSize", "Windows.SearchMru", "Windows.SearchOpenLocation", "Windows.SearchOptionCompressed",
                "Windows.SearchOptionContents", "Windows.SearchOptionDeep", "Windows.SearchOptionShallow",
                "Windows.SearchOptionSystem", "Windows.SearchSave", "Windows.SearchSendTo", "Windows.SearchSendToComputer",
                "Windows.selectall", "Windows.SelectionCheckboxes", "Windows.selectMode", "Windows.selectnone",
                "Windows.separator", "Windows.setdesktopwallpaper", "Windows.Share", "Windows.SharePrivate",
                "Windows.ShareSpecificUsers", "Windows.Shortcut.opencontaining", "Windows.ShowFileExtensions",
                "Windows.ShowHiddenFiles", "Windows.SizeAllColumns", "Windows.slideshow", "Windows.SortAscending",
                "Windows.SortByColumn", "Windows.SortDescending", "Windows.SortGroupsAscending", "Windows.SortGroupsDescending",
                "Windows.StartScan", "Windows.statusbar", "Windows.Sync", "Windows.SystemProperties", "Windows.taskbarpin",
                "Windows.ToggleRecycleConfirmations", "Windows.topviewrestoredefault", "Windows.Troubleshoot", "Windows.undo",
                "Windows.UpdatePrinterDriver", "Windows.v2.Powershell", "Windows.View.OptionsGallery",
                "Windows.ViewRemotePrinters", "Windows.zip", "Windows.Zip.Action"
            };

            public List<string> SelectedItemNames { get; private set; } = new List<string>();

            public ShellStoreForm()
            {
                this.Text = AppString.Text.CheckReference;
                btnOk.Click += (sender, e) => GetSelectedItems();
                LoadItems();
            }

            private void LoadItems()
            {
                using(var shellKey = RegistryEx.GetRegistryKey(ShellItem.CommandStorePath))
                {
                    Array.ForEach(Array.FindAll(shellKey.GetSubKeyNames(), itemName =>
                        !SystemItemNames.Contains(itemName, StringComparer.OrdinalIgnoreCase)), itemName =>
                        {
                            string regPath = $@"{ShellItem.CommandStorePath}\{itemName}";
                            list.AddItem(new StoreShellItem(regPath));
                            RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
                        });
                }
            }

            private void GetSelectedItems()
            {
                foreach(StoreShellItem item in list.Controls)
                    if(item.IsSelected) SelectedItemNames.Add(item.KeyName);
            }

            sealed class StoreShellItem : ShellItem
            {
                public StoreShellItem(string regPath) : base(regPath)
                {
                    this.AddCtr(chkSelected);
                    this.SetCtrIndex(chkSelected, 2);
                }
                public bool IsSelected => chkSelected.Checked;
                readonly CheckBox chkSelected = new CheckBox { AutoSize = true };
            }
        }
    }
}