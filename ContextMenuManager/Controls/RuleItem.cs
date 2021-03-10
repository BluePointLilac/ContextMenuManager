using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Controls
{
    class RuleItem : MyListItem, IFoldSubItem, IBtnShowMenuItem, ITsiWebSearchItem
    {
        public RuleItem(ItemInfo info)
        {
            this.Text = info.Text;
            this.Image = info.Image;
            this.RestartExplorer = info.RestartExplorer;
            BtnShowMenu = new MenuButton(this);
            TsiSearch = new WebSearchMenuItem(this);
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add(TsiSearch);
        }

        public IFoldGroupItem FoldGroupItem { get; set; }
        public WebSearchMenuItem TsiSearch { get; set; }
        public MenuButton BtnShowMenu { get; set; }

        public bool RestartExplorer { get; set; }

        public string SearchText
        {
            get
            {
                if(this.FoldGroupItem == null) return this.Text;
                else return $"{FoldGroupItem.Text} {this.Text}";
            }
        }
    }

    public struct ItemInfo
    {
        public string Text { get; set; }
        public Image Image { get; set; }
        public string Tip { get; set; }
        public bool RestartExplorer { get; set; }
    }

    sealed class VisibleRegRuleItem : RuleItem, IChkVisibleItem, ITsiRegPathItem
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

        public struct RuleAndInfo
        {
            public RegRule[] Rules { get; set; }
            public ItemInfo ItemInfo { get; set; }
        }

        private VisibleRegRuleItem(ItemInfo info) : base(info)
        {
            ChkVisible = new VisibleCheckBox(this);
            MyToolTip.SetToolTip(ChkVisible, info.Tip);
            TsiRegLocation = new RegLocationMenuItem(this);
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { new ToolStripSeparator(), TsiRegLocation });
        }

        public VisibleRegRuleItem(RegRule[] rules, ItemInfo info)
            : this(info) { this.Rules = rules; }

        public VisibleRegRuleItem(RegRule rule, ItemInfo info)
            : this(info) { this.Rules = new[] { rule }; }

        public VisibleRegRuleItem(RuleAndInfo ruleAndInfo)
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

        public VisibleCheckBox ChkVisible { get; set; }
        public RegLocationMenuItem TsiRegLocation { get; set; }

        public bool ItemVisible
        {
            get
            {
                for(int i = 0; i < Rules.Length; i++)
                {
                    RegRule rule = Rules[i];
                    using(RegistryKey key = RegistryEx.GetRegistryKey(rule.RegPath))
                    {
                        string value = key?.GetValue(rule.ValueName)?.ToString().ToLower();
                        string turnOnValue = rule.TurnOnValue?.ToString().ToLower();
                        string turnOffValue = rule.TurnOffValue?.ToString().ToLower();
                        if(value == null || key.GetValueKind(rule.ValueName) != rule.ValueKind)
                        {
                            if(i < Rules.Length - 1) continue;
                        }
                        if(value == turnOnValue) return true;
                        if(value == turnOffValue) return false;
                    }
                }
                return true;
            }
            set
            {
                foreach(RegRule rule in Rules)
                {
                    object data = value ? rule.TurnOnValue : rule.TurnOffValue;
                    if(data != null)
                    {
                        Registry.SetValue(rule.RegPath, rule.ValueName, data, rule.ValueKind);
                    }
                    else
                    {
                        RegistryEx.DeleteValue(rule.RegPath, rule.ValueName);
                    }
                }
                if(RestartExplorer) ExplorerRestarter.Show();
            }
        }

        public string RegPath => Rules[0].RegPath;
        public string ValueName => Rules[0].ValueName;

        const string LM_SMWCPE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string CU_SMWCPE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string LM_SMWCE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
        const string CU_SMWCE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer";
        const string LM_SPMWE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Explorer";
        const string CU_SPMWE = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer";

        public static RuleAndInfo CustomFolder = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoCustomizeThisFolder", null, 1),
                new RegRule(LM_SMWCPE, "NoCustomizeWebView", null, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeThisFolder", null, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeWebView", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Other.CustomFolder,
                Image = AppImage.Folder,
                Tip = AppString.Tip.CustomFolder,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo NetworkDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoNetConnectDisconnect", null, 1),
                new RegRule(CU_SMWCPE, "NoNetConnectDisconnect", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = $"{AppString.Other.MapNetworkDrive} && {AppString.Other.DisconnectNetworkDrive}",
                Image = AppImage.NetworkDrive,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo RecycleBinProperties = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoPropertiesRecycleBin", null, 1),
                new RegRule(CU_SMWCPE, "NoPropertiesRecycleBin", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Other.RecycleBinProperties,
                Image = AppImage.RecycleBin,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo SendToDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoDrivesInSendToMenu", null, 1),
                new RegRule(CU_SMWCPE, "NoDrivesInSendToMenu", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Other.RemovableDrive,
                Image = AppImage.Drive,
                Tip = AppString.Tip.SendToDrive,
                RestartExplorer = true
            }
        };

        public static RuleAndInfo DeferBuildSendTo = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCE, "DelaySendToMenuBuild", null, 1),
                new RegRule(CU_SMWCE, "DelaySendToMenuBuild", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Other.BuildSendtoMenu,
                Image = AppImage.SendTo,
                Tip = AppString.Tip.BuildSendtoMenu
            }
        };

        public static RuleAndInfo UseStoreOpenWith = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SPMWE, "NoUseStoreOpenWith", null, 1),
                new RegRule(CU_SPMWE, "NoUseStoreOpenWith", null, 1)
            },
            ItemInfo = new ItemInfo
            {
                Text = AppString.Other.UseStoreOpenWith,
                Image = AppImage.MicrosoftStore
            }
        };
    }

    sealed class NumberRegRuleItem : RuleItem, ITsiRegPathItem
    {
        public struct RegRule
        {
            public string RegPath { get; set; }
            public string ValueName { get; set; }
            public RegistryValueKind ValueKind { get; set; }
            public int MaxValue { get; set; }
            public int MinValue { get; set; }
            public int DefaultValue { get; set; }
        }

        readonly NumericUpDown NudValue = new NumericUpDown
        {
            Font = new Font(SystemFonts.MenuFont.FontFamily, 12F),
            TextAlign = HorizontalAlignment.Center,
            Width = 80.DpiZoom()
        };
        public RegLocationMenuItem TsiRegLocation { get; set; }

        public NumberRegRuleItem(RegRule rule, ItemInfo info) : base(info)
        {
            this.AddCtr(NudValue);
            MyToolTip.SetToolTip(NudValue, info.Tip);
            TsiRegLocation = new RegLocationMenuItem(this);
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { new ToolStripSeparator(), TsiRegLocation });
            this.Rule = rule;
            NudValue.Maximum = rule.MaxValue;
            NudValue.Minimum = rule.MinValue;
            NudValue.ValueChanged += (sender, e) =>
            {
                if(NudValue.Value == Rule.DefaultValue) NudValue.ForeColor = Color.Red;
                else NudValue.ForeColor = Color.Black;
                this.ItemValue = (int)NudValue.Value;
            };
            NudValue.Value = ItemValue;
        }

        public string RegPath => Rule.RegPath;
        public string ValueName => Rule.ValueName;
        public RegRule Rule { get; set; }

        public int ItemValue
        {
            get
            {
                object value = Registry.GetValue(Rule.RegPath, Rule.ValueName, null);
                if(value == null) return Rule.DefaultValue;
                int num = Convert.ToInt32(value);
                if(num > Rule.MaxValue) return Rule.MaxValue;
                if(num < Rule.MinValue) return Rule.MinValue;
                else return num;
            }
            set
            {
                Registry.SetValue(Rule.RegPath, Rule.ValueName, value, Rule.ValueKind);
            }
        }
    }

    sealed class StringRegRuleItem : RuleItem, ITsiRegPathItem
    {
        public struct RegRule
        {
            public string RegPath { get; set; }
            public string ValueName { get; set; }
        }

        readonly Label LblValue = new Label
        {
            Font = new Font(SystemFonts.MenuFont.FontFamily, 12F),
            BorderStyle = BorderStyle.FixedSingle,
            AutoSize = true
        };

        public RegLocationMenuItem TsiRegLocation { get; set; }

        public StringRegRuleItem(RegRule rule, ItemInfo info) : base(info)
        {
            this.AddCtr(LblValue);
            MyToolTip.SetToolTip(LblValue, info.Tip);
            TsiRegLocation = new RegLocationMenuItem(this);
            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { new ToolStripSeparator(), TsiRegLocation });
            this.Rule = rule;
            LblValue.Text = ItemValue;
            LblValue.MouseDown += (sender, e) =>
            {
                using(InputDialog dlg = new InputDialog())
                {
                    dlg.Title = AppString.Menu.ChangeText;
                    dlg.Text = ItemValue;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemValue = LblValue.Text = dlg.Text;
                }
            };
            LblValue.TextChanged += (sender, e) => ItemValue = LblValue.Text;
        }

        public string RegPath => Rule.RegPath;
        public string ValueName => Rule.ValueName;
        public RegRule Rule { get; set; }

        public string ItemValue
        {
            get => Registry.GetValue(Rule.RegPath, Rule.ValueName, null)?.ToString();
            set => Registry.SetValue(Rule.RegPath, Rule.ValueName, value);
        }
    }

    sealed class VisbleIniRuleItem : RuleItem, IChkVisibleItem
    {
        public struct IniRule
        {
            public string IniPath { get; set; }
            public string Section { get; set; }
            public string KeyName { get; set; }
            public string TurnOnValue { get; set; }
            public string TurnOffValue { get; set; }
        }

        public VisbleIniRuleItem(IniRule rule, ItemInfo info) : base(info)
        {
            this.Rule = rule;
            this.IniWriter = new IniWriter(rule.IniPath);
            ChkVisible = new VisibleCheckBox(this) { Checked = ItemVisible };
            MyToolTip.SetToolTip(ChkVisible, info.Tip);
        }

        public IniRule Rule { get; set; }
        public IniWriter IniWriter { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public bool ItemVisible
        {
            get => IniWriter.GetValue(Rule.Section, Rule.KeyName) == Rule.TurnOnValue;
            set => IniWriter.SetValue(Rule.Section, Rule.KeyName, value ? Rule.TurnOnValue : Rule.TurnOffValue);
        }
    }

    sealed class NumberIniRuleItem : RuleItem
    {
        public struct IniRule
        {
            public string IniPath { get; set; }
            public string Section { get; set; }
            public string KeyName { get; set; }
            public int MaxValue { get; set; }
            public int MinValue { get; set; }
            public int DefaultValue { get; set; }
        }

        public NumberIniRuleItem(IniRule rule, ItemInfo info) : base(info)
        {
            this.AddCtr(NudValue);
            this.Rule = rule;
            this.IniWriter = new IniWriter(rule.IniPath);
            MyToolTip.SetToolTip(NudValue, info.Tip);
            NudValue.Maximum = rule.MaxValue;
            NudValue.Minimum = rule.MinValue;
            NudValue.ValueChanged += (sender, e) =>
            {
                if(NudValue.Value == Rule.DefaultValue) NudValue.ForeColor = Color.Red;
                else NudValue.ForeColor = Color.Black;
                this.ItemValue = (int)NudValue.Value;
            };
            NudValue.Value = ItemValue;
        }

        public IniRule Rule { get; set; }
        public IniWriter IniWriter { get; set; }

        readonly NumericUpDown NudValue = new NumericUpDown
        {
            Font = new Font(SystemFonts.MenuFont.FontFamily, 12F),
            TextAlign = HorizontalAlignment.Center,
            Width = 80.DpiZoom()
        };

        public int ItemValue
        {
            get
            {
                string value = IniWriter.GetValue(Rule.Section, Rule.KeyName);
                if(value.IsNullOrWhiteSpace()) return Rule.DefaultValue;
                int num = Convert.ToInt32(value);
                if(num > Rule.MaxValue) return Rule.MaxValue;
                if(num < Rule.MinValue) return Rule.MinValue;
                else return num;
            }
            set
            {
                IniWriter.SetValue(Rule.Section, Rule.KeyName, value);
            }
        }
    }

    sealed class StringIniRuleItem : RuleItem
    {
        public struct IniRule
        {
            public string IniPath { get; set; }
            public string Secation { get; set; }
            public string KeyName { get; set; }
        }


        readonly Label LblValue = new Label
        {
            Font = new Font(SystemFonts.MenuFont.FontFamily, 12F),
            BorderStyle = BorderStyle.FixedSingle,
            AutoSize = true
        };

        public StringIniRuleItem(IniRule rule, ItemInfo info) : base(info)
        {
            this.Rule = rule;
            this.IniWriter = new IniWriter(rule.IniPath);
            this.AddCtr(LblValue);
            MyToolTip.SetToolTip(LblValue, info.Tip);
            LblValue.Text = ItemValue;
            LblValue.MouseDown += (sender, e) =>
            {
                using(InputDialog dlg = new InputDialog())
                {
                    dlg.Title = AppString.Menu.ChangeText;
                    dlg.Text = ItemValue;
                    if(dlg.ShowDialog() != DialogResult.OK) return;
                    ItemValue = LblValue.Text = dlg.Text;
                }
            };
            LblValue.TextChanged += (sender, e) => ItemValue = LblValue.Text;
        }

        public IniRule Rule { get; set; }
        public IniWriter IniWriter { get; set; }

        public string ItemValue
        {
            get => IniWriter.GetValue(Rule.Secation, Rule.KeyName);
            set => IniWriter.SetValue(Rule.Secation, Rule.KeyName, value);
        }
    }
}