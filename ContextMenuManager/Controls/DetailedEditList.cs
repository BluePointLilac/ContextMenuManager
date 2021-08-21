using BluePointLilac.Methods;
using ContextMenuManager.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class DetailedEditList : SwitchDicList
    {
        public Guid GroupGuid { get; set; }

        public override void LoadItems()
        {
            base.LoadItems();
            int index = this.UseUserDic ? 1 : 0;
            XmlDocument doc = XmlDicHelper.DetailedEditDic[index];
            if(doc?.DocumentElement == null) return;
            foreach(XmlNode groupXN in doc.DocumentElement.ChildNodes)
            {
                try
                {
                    List<Guid> guids = new List<Guid>();
                    XmlNodeList guidList = groupXN.SelectNodes("Guid");
                    foreach(XmlNode guidXN in guidList)
                    {
                        if(!GuidEx.TryParse(guidXN.InnerText, out Guid guid)) continue;
                        if(!File.Exists(GuidInfo.GetFilePath(guid))) continue;
                        if(this.GroupGuid != Guid.Empty && this.GroupGuid != guid) continue;
                        guids.Add(guid);
                    }
                    if(guidList.Count > 0 && guids.Count == 0) continue;

                    FoldGroupItem groupItem;
                    bool isIniGroup = groupXN.SelectSingleNode("IsIniGroup") != null;
                    string attribute = isIniGroup ? "FilePath" : "RegPath";
                    ObjectPath.PathType pathType = isIniGroup ? ObjectPath.PathType.File : ObjectPath.PathType.Registry;
                    groupItem = new FoldGroupItem(groupXN.SelectSingleNode(attribute)?.InnerText, pathType);
                    foreach(XmlElement textXE in groupXN.SelectNodes("Text"))
                    {
                        if(XmlDicHelper.JudgeCulture(textXE)) groupItem.Text = ResourceString.GetDirectString(textXE.GetAttribute("Value"));
                    }
                    if(guids.Count > 0)
                    {
                        groupItem.Tag = guids;
                        if(groupItem.Text.IsNullOrWhiteSpace()) groupItem.Text = GuidInfo.GetText(guids[0]);
                        groupItem.Image = GuidInfo.GetImage(guids[0]);
                        string filePath = GuidInfo.GetFilePath(guids[0]);
                        string clsidPath = GuidInfo.GetClsidPath(guids[0]);
                        if(filePath != null || clsidPath != null) groupItem.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                        if(filePath != null)
                        {
                            ToolStripMenuItem tsi = new ToolStripMenuItem(AppString.Menu.FileLocation);
                            tsi.Click += (sender, e) => ExternalProgram.JumpExplorer(filePath, AppConfig.OpenMoreExplorer);
                            groupItem.ContextMenuStrip.Items.Add(tsi);
                        }
                        if(clsidPath != null)
                        {
                            ToolStripMenuItem tsi = new ToolStripMenuItem(AppString.Menu.ClsidLocation);
                            tsi.Click += (sender, e) => ExternalProgram.JumpRegEdit(clsidPath, null, AppConfig.OpenMoreRegedit);
                            groupItem.ContextMenuStrip.Items.Add(tsi);
                        }
                    }
                    XmlNode iconXN = groupXN.SelectSingleNode("Icon");
                    using(Icon icon = ResourceIcon.GetIcon(iconXN?.InnerText))
                    {
                        if(icon != null) groupItem.Image = icon.ToBitmap();
                    }
                    this.AddItem(groupItem);

                    string GetRuleFullRegPath(string regPath)
                    {
                        if(string.IsNullOrEmpty(regPath)) regPath = groupItem.GroupPath;
                        else if(regPath.StartsWith("\\")) regPath = groupItem.GroupPath + regPath;
                        return regPath;
                    };

                    foreach(XmlElement itemXE in groupXN.SelectNodes("Item"))
                    {
                        try
                        {
                            if(!XmlDicHelper.JudgeOSVersion(itemXE)) continue;
                            RuleItem ruleItem;
                            ItemInfo info = new ItemInfo();
                            foreach(XmlElement textXE in itemXE.SelectNodes("Text"))
                            {
                                if(XmlDicHelper.JudgeCulture(textXE)) info.Text = ResourceString.GetDirectString(textXE.GetAttribute("Value"));
                            }
                            foreach(XmlElement tipXE in itemXE.SelectNodes("Tip"))
                            {
                                if(XmlDicHelper.JudgeCulture(tipXE)) info.Tip = ResourceString.GetDirectString(tipXE.GetAttribute("Value"));
                            }
                            info.RestartExplorer = itemXE.SelectSingleNode("RestartExplorer") != null;

                            int defaultValue = 0, maxValue = 0, minValue = 0;
                            if(itemXE.SelectSingleNode("IsNumberItem") != null)
                            {
                                XmlElement ruleXE = (XmlElement)itemXE.SelectSingleNode("Rule");
                                defaultValue = ruleXE.HasAttribute("Default") ? Convert.ToInt32(ruleXE.GetAttribute("Default")) : 0;
                                maxValue = ruleXE.HasAttribute("Max") ? Convert.ToInt32(ruleXE.GetAttribute("Max")) : int.MaxValue;
                                minValue = ruleXE.HasAttribute("Min") ? Convert.ToInt32(ruleXE.GetAttribute("Min")) : int.MinValue;
                            }

                            if(isIniGroup)
                            {
                                XmlElement ruleXE = (XmlElement)itemXE.SelectSingleNode("Rule");
                                string iniPath = ruleXE.GetAttribute("FilePath");
                                if(iniPath.IsNullOrWhiteSpace()) iniPath = groupItem.GroupPath;
                                string section = ruleXE.GetAttribute("Section");
                                string keyName = ruleXE.GetAttribute("KeyName");
                                if(itemXE.SelectSingleNode("IsNumberItem") != null)
                                {
                                    var rule = new NumberIniRuleItem.IniRule
                                    {
                                        IniPath = iniPath,
                                        Section = section,
                                        KeyName = keyName,
                                        DefaultValue = defaultValue,
                                        MaxValue = maxValue,
                                        MinValue = maxValue
                                    };
                                    ruleItem = new NumberIniRuleItem(rule, info);
                                }
                                else if(itemXE.SelectSingleNode("IsStringItem") != null)
                                {
                                    var rule = new StringIniRuleItem.IniRule
                                    {
                                        IniPath = iniPath,
                                        Secation = section,
                                        KeyName = keyName
                                    };
                                    ruleItem = new StringIniRuleItem(rule, info);
                                }
                                else
                                {
                                    var rule = new VisbleIniRuleItem.IniRule
                                    {
                                        IniPath = iniPath,
                                        Section = section,
                                        KeyName = keyName,
                                        TurnOnValue = ruleXE.HasAttribute("On") ? ruleXE.GetAttribute("On") : null,
                                        TurnOffValue = ruleXE.HasAttribute("Off") ? ruleXE.GetAttribute("Off") : null,
                                    };
                                    ruleItem = new VisbleIniRuleItem(rule, info);
                                }
                            }
                            else
                            {
                                if(itemXE.SelectSingleNode("IsNumberItem") != null)
                                {
                                    XmlElement ruleXE = (XmlElement)itemXE.SelectSingleNode("Rule");
                                    var rule = new NumberRegRuleItem.RegRule
                                    {
                                        RegPath = GetRuleFullRegPath(ruleXE.GetAttribute("RegPath")),
                                        ValueName = ruleXE.GetAttribute("ValueName"),
                                        ValueKind = XmlDicHelper.GetValueKind(ruleXE.GetAttribute("ValueKind"), RegistryValueKind.DWord),
                                        DefaultValue = defaultValue,
                                        MaxValue = maxValue,
                                        MinValue = minValue
                                    };
                                    ruleItem = new NumberRegRuleItem(rule, info);
                                }
                                else if(itemXE.SelectSingleNode("IsStringItem") != null)
                                {
                                    XmlElement ruleXE = (XmlElement)itemXE.SelectSingleNode("Rule");
                                    var rule = new StringRegRuleItem.RegRule
                                    {
                                        RegPath = GetRuleFullRegPath(ruleXE.GetAttribute("RegPath")),
                                        ValueName = ruleXE.GetAttribute("ValueName"),
                                    };
                                    ruleItem = new StringRegRuleItem(rule, info);
                                }
                                else
                                {
                                    XmlNodeList ruleXNList = itemXE.SelectNodes("Rule");
                                    var rules = new VisibleRegRuleItem.RegRule[ruleXNList.Count];
                                    for(int i = 0; i < ruleXNList.Count; i++)
                                    {
                                        XmlElement ruleXE = (XmlElement)ruleXNList[i];
                                        rules[i] = new VisibleRegRuleItem.RegRule
                                        {
                                            RegPath = GetRuleFullRegPath(ruleXE.GetAttribute("RegPath")),
                                            ValueName = ruleXE.GetAttribute("ValueName"),
                                            ValueKind = XmlDicHelper.GetValueKind(ruleXE.GetAttribute("ValueKind"), RegistryValueKind.DWord)
                                        };
                                        string turnOn = ruleXE.HasAttribute("On") ? ruleXE.GetAttribute("On") : null;
                                        string turnOff = ruleXE.HasAttribute("Off") ? ruleXE.GetAttribute("Off") : null;
                                        switch(rules[i].ValueKind)
                                        {
                                            case RegistryValueKind.Binary:
                                                rules[i].TurnOnValue = turnOn != null ? XmlDicHelper.ConvertToBinary(turnOn) : null;
                                                rules[i].TurnOffValue = turnOff != null ? XmlDicHelper.ConvertToBinary(turnOff) : null;
                                                break;
                                            case RegistryValueKind.DWord:
                                                if(turnOn == null) rules[i].TurnOnValue = null;
                                                else rules[i].TurnOnValue = Convert.ToInt32(turnOn);
                                                if(turnOff == null) rules[i].TurnOffValue = null;
                                                else rules[i].TurnOffValue = Convert.ToInt32(turnOff);
                                                break;
                                            default:
                                                rules[i].TurnOnValue = turnOn;
                                                rules[i].TurnOffValue = turnOff;
                                                break;
                                        }
                                    }
                                    ruleItem = new VisibleRegRuleItem(rules, info);
                                }
                            }
                            this.AddItem(ruleItem);
                            ruleItem.FoldGroupItem = groupItem;
                            ruleItem.HasImage = ruleItem.Image != null;
                            ruleItem.Indent();
                        }
                        catch { continue; }
                    }
                    groupItem.SetVisibleWithSubItemCount();
                }
                catch { continue; }
            }
        }
    }
}