using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    sealed class RegRuleItem : MyListItem, IChkVisibleItem, IFoldSubItem
    {
        public struct RegRule
        {
            public string RegPath { get; set; }
            public string ValueName { get; set; }
            public RegistryValueKind ValueKind { get; set; }
            public object TurnOnValue { get; set; }
            public object TurnOffValue { get; set; }
            public RegRule(string regPath, string valueName, object turnOnValue,
                object turnOffValue, RegistryValueKind valueKind = RegistryValueKind.DWord)
            {
                this.RegPath = regPath; this.ValueName = valueName;
                this.TurnOnValue = turnOnValue; this.TurnOffValue = turnOffValue;
                this.ValueKind = valueKind;
            }
        }

        public struct ItemInfo
        {
            public string Text { get; set; }
            public Image Image { get; set; }
            public string Tip { get; set; }
            public bool RestartExplorer { get; set; }
        }

        public struct RuleAndInfo
        {
            public RegRule[] Rules { get; set; }
            public ItemInfo ItemInfo { get; set; }
        }

        private RegRuleItem(ItemInfo info)
        {
            this.Text = info.Text;
            this.Image = info.Image;
            this.RestartExplorer = info.RestartExplorer;
            ChkVisible = new VisibleCheckBox(this);
            MyToolTip.SetToolTip(ChkVisible, info.Tip);
        }

        public RegRuleItem(RegRule[] rules, ItemInfo info)
            : this(info) { this.Rules = rules; }

        public RegRuleItem(RegRule rule, ItemInfo info)
            : this(info) { this.Rules = new[] { rule }; }

        public RegRuleItem(RuleAndInfo ruleAndInfo)
            : this(ruleAndInfo.Rules, ruleAndInfo.ItemInfo) { }

        private RegRule[] _Rules;
        public RegRule[] Rules
        {
            get => _Rules;
            set
            {
                _Rules = value;
                ChkVisible.Checked = ItemVisible;
            }
        }

        public int MarginRight
        {
            get => ChkVisible.Margin.Right;
            set
            {
                Padding p = ChkVisible.Margin;
                ChkVisible.Margin = new Padding(p.Left, p.Top, value, p.Bottom);
            }
        }

        public static readonly int SysMarginRignt = 78.DpiZoom();

        public VisibleCheckBox ChkVisible { get; set; }
        public bool RestartExplorer { get; set; }

        public bool ItemVisible
        {
            get
            {
                foreach(RegRule rule in Rules)
                {
                    using(RegistryKey key = RegistryEx.GetRegistryKey(rule.RegPath))
                    {
                        if(key?.GetValue(rule.ValueName) == null) continue;
                        if(key.GetValueKind(rule.ValueName) != rule.ValueKind) continue;
                        if(key.GetValue(rule.ValueName).ToString().ToLower()
                            == rule.TurnOffValue.ToString().ToLower()) return false;
                    }
                }
                return true;
            }
            set
            {
                foreach(RegRule rule in Rules)
                {
                    object data = value ? rule.TurnOnValue : rule.TurnOffValue;
                    Registry.SetValue(rule.RegPath, rule.ValueName, data, rule.ValueKind);
                }
                if(RestartExplorer) ExplorerRestarter.NeedRestart = true;
            }
        }

        public IFoldGroupItem FoldGroupItem { get; set; }

        const string CU_SMWCEA = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        const string LM_SMWCPE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string CU_SMWCPE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string LM_SMWCE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
        const string CU_SMWCE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer";
        const string LM_SPMWE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Explorer";
        const string CU_SPMWE = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer";

        public static RuleAndInfo CustomFolder = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoCustomizeThisFolder", 0, 1),
                new RegRule(LM_SMWCPE, "NoCustomizeWebView", 0, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeThisFolder", 0, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeWebView", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.CustomFolder,
                Image = AppImage.CustomFolder,
                Tip = AppString.Tip.CustomFolder,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo NetworkDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoNetConnectDisconnect", 0, 1),
                new RegRule(CU_SMWCPE, "NoNetConnectDisconnect", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = $"{AppString.Item.MapNetworkDrive} && {AppString.Item.DisconnectNetworkDrive}",
                Image = AppImage.NetworkDrive,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo RecycleBinProperties = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoPropertiesRecycleBin", 0, 1),
                new RegRule(CU_SMWCPE, "NoPropertiesRecycleBin", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.RecycleBinProperties,
                Image = AppImage.RecycleBinProperties,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo SendToDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoDrivesInSendToMenu", 0, 1),
                new RegRule(CU_SMWCPE, "NoDrivesInSendToMenu", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.RemovableDrive,
                Image = AppImage.Drive,
                Tip = AppString.Tip.SendToDrive,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo DeferBuildSendTo = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCE, "DelaySendToMenuBuild", 0, 1),
                new RegRule(CU_SMWCE, "DelaySendToMenuBuild", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.BuildSendtoMenu,
                Image = AppImage.SendTo,
                Tip = AppString.Tip.BuildSendtoMenu
            }
        };

        public static RuleAndInfo UseStoreOpenWith = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SPMWE, "NoUseStoreOpenWith", 0, 1),
                new RegRule(CU_SPMWE, "NoUseStoreOpenWith", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.UseStoreOpenWith,
                Image = AppImage.MicrosoftStore
            }
        };

        public static RuleAndInfo WinXPowerShell = new RuleAndInfo
        {
            Rules = new[]
            {
                new RegRule(CU_SMWCEA, "DontUsePowerShellOnWinX", 0, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Item.WinXPowerShell,
                Image = AppImage.Cmd,
                RestartExplorer = true
            }
        };
    }
}