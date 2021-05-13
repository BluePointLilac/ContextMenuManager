using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class ThirdRulesList : MyList
    {
        public void LoadItems()
        {
            string webPath = AppConfig.WebThirdRulesDic;
            string userPath = AppConfig.UserThirdRulesDic;
            string contents = Properties.Resources.ThirdRulesDic;
            if(!File.Exists(webPath)) File.WriteAllText(webPath, contents, Encoding.Unicode);
            GroupPathItem webGroupItem = new GroupPathItem(webPath, ObjectPath.PathType.File);
            GroupPathItem userGroupItem = new GroupPathItem(userPath, ObjectPath.PathType.File);
            webGroupItem.Text = AppString.SideBar.Dictionaries;
            userGroupItem.Text = AppString.Other.UserDictionaries;
            webGroupItem.Image = AppImage.App;
            userGroupItem.Image = AppImage.User;
            LoadDocItems(webPath, webGroupItem);
            LoadDocItems(userPath, userGroupItem);
        }

        private void LoadDocItems(string xmlPath, GroupPathItem groupItem)
        {
            if(!File.Exists(xmlPath)) return;
            this.AddItem(groupItem);
            XmlDocument doc = new XmlDocument();
            try { doc.LoadXml(File.ReadAllText(xmlPath, EncodingType.GetType(xmlPath))); }
            catch { return; }
            foreach(XmlElement groupXE in doc.DocumentElement.ChildNodes)
            {
                try
                {
                    Guid guid = Guid.Empty;
                    if(groupXE.HasAttribute("Guid"))
                    {
                        if(GuidEx.TryParse(groupXE.GetAttribute("Guid"), out guid))
                        {
                            if(!File.Exists(GuidInfo.GetFilePath(guid))) continue;
                        }
                        else continue;
                    }

                    SubGroupItem subGroupItem;
                    bool isIniGroup = groupXE.HasAttribute("IsIniGroup");
                    string attribute = isIniGroup ? "FilePath" : "RegPath";
                    ObjectPath.PathType pathType = isIniGroup ? ObjectPath.PathType.File : ObjectPath.PathType.Registry;
                    subGroupItem = new SubGroupItem(groupXE.GetAttribute(attribute), pathType)
                    {
                        Text = groupXE.GetAttribute("Text"),
                        Image = GuidInfo.GetImage(guid)
                    };
                    if(subGroupItem.Text.IsNullOrWhiteSpace()) subGroupItem.Text = GuidInfo.GetText(guid);
                    this.AddItem(subGroupItem);

                    string GetRuleFullRegPath(string regPath)
                    {
                        if(string.IsNullOrEmpty(regPath)) regPath = subGroupItem.TargetPath;
                        else if(regPath.StartsWith("\\")) regPath = subGroupItem.TargetPath + regPath;
                        return regPath;
                    };

                    foreach(XmlElement itemXE in groupXE.ChildNodes)
                    {
                        try
                        {
                            if(!EnhanceMenusList.JudgeOSVersion(itemXE)) continue;
                            RuleItem ruleItem;
                            ItemInfo info = new ItemInfo
                            {
                                Text = itemXE.GetAttribute("Text"),
                                Tip = itemXE.GetAttribute("Tip"),
                                RestartExplorer = itemXE.HasAttribute("RestartExplorer"),
                            };
                            int defaultValue = 0, maxValue = 0, minValue = 0;
                            if(itemXE.HasAttribute("IsNumberItem"))
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
                                if(iniPath.IsNullOrWhiteSpace()) iniPath = subGroupItem.TargetPath;
                                string section = ruleXE.GetAttribute("Section");
                                string keyName = ruleXE.GetAttribute("KeyName");
                                if(itemXE.HasAttribute("IsNumberItem"))
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
                                else if(itemXE.HasAttribute("IsStringItem"))
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
                                if(itemXE.HasAttribute("IsNumberItem"))
                                {
                                    XmlElement ruleXE = (XmlElement)itemXE.SelectSingleNode("Rule");
                                    var rule = new NumberRegRuleItem.RegRule
                                    {
                                        RegPath = GetRuleFullRegPath(ruleXE.GetAttribute("RegPath")),
                                        ValueName = ruleXE.GetAttribute("ValueName"),
                                        ValueKind = GetValueKind(ruleXE.GetAttribute("ValueKind")),
                                        DefaultValue = defaultValue,
                                        MaxValue = maxValue,
                                        MinValue = minValue
                                    };
                                    ruleItem = new NumberRegRuleItem(rule, info);
                                }
                                else if(itemXE.HasAttribute("IsStringItem"))
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
                                            ValueKind = GetValueKind(ruleXE.GetAttribute("ValueKind"))
                                        };
                                        string turnOn = ruleXE.HasAttribute("On") ? ruleXE.GetAttribute("On") : null;
                                        string turnOff = ruleXE.HasAttribute("Off") ? ruleXE.GetAttribute("Off") : null;
                                        switch(rules[i].ValueKind)
                                        {
                                            case RegistryValueKind.Binary:
                                                rules[i].TurnOnValue = turnOn != null ? EnhanceMenusList.ConvertToBinary(turnOn) : null;
                                                rules[i].TurnOffValue = turnOff != null ? EnhanceMenusList.ConvertToBinary(turnOff) : null;
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
                            ruleItem.FoldGroupItem = subGroupItem;
                        }
                        catch { continue; }
                    }
                    subGroupItem.HideWhenNoSubItem();
                    subGroupItem.FoldGroupItem = groupItem;
                }
                catch { continue; }
            }
            groupItem.IsFold = true;
            groupItem.HideWhenNoSubItem();
        }

        private static RegistryValueKind GetValueKind(string data)
        {
            switch(data.ToUpper())
            {
                case "REG_SZ":
                    return RegistryValueKind.String;
                case "REG_BINARY":
                    return RegistryValueKind.Binary;
                case "REG_DWORD":
                    return RegistryValueKind.DWord;
                case "REG_QWORD":
                    return RegistryValueKind.QWord;
                case "REG_MULTI_SZ":
                    return RegistryValueKind.MultiString;
                case "REG_EXPAND_SZ":
                    return RegistryValueKind.ExpandString;
                default:
                    return RegistryValueKind.DWord;
            }
        }
    }
}