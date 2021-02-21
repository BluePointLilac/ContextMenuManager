using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class ThirdRulesList : MyList
    {
        public void LoadItems()
        {
            try
            {
                foreach(XmlElement groupXE in ReadXml().DocumentElement.ChildNodes)
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

                    GroupPathItem groupItem;
                    bool isIniGroup = groupXE.HasAttribute("IsIniGroup");
                    string attribute = isIniGroup ? "FilePath" : "RegPath";
                    ObjectPath.PathType pathType = isIniGroup ? ObjectPath.PathType.File : ObjectPath.PathType.Registry;
                    groupItem = new GroupPathItem(groupXE.GetAttribute(attribute), pathType)
                    {
                        Text = groupXE.GetAttribute("Text"),
                        Image = GuidInfo.GetImage(guid)
                    };
                    if(groupItem.Text.IsNullOrWhiteSpace()) groupItem.Text = GuidInfo.GetText(guid);
                    this.AddItem(groupItem);

                    string GetRuleFullRegPath(string regPath)
                    {
                        if(string.IsNullOrEmpty(regPath)) regPath = groupItem.TargetPath;
                        else if(regPath.StartsWith("\\")) regPath = groupItem.TargetPath + regPath;
                        return regPath;
                    };

                    foreach(XmlElement itemXE in groupXE.ChildNodes)
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
                            if(iniPath.IsNullOrWhiteSpace()) iniPath = groupItem.TargetPath;
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
                                        ValueKind = GetValueKind(ruleXE.GetAttribute("ValueKind")),
                                        TurnOnValue = ruleXE.HasAttribute("On") ? ruleXE.GetAttribute("On") : null,
                                        TurnOffValue = ruleXE.HasAttribute("Off") ? ruleXE.GetAttribute("Off") : null,
                                    };
                                }
                                ruleItem = new VisibleRegRuleItem(rules, info);
                            }
                        }
                        this.AddItem(ruleItem);
                        ruleItem.HasImage = false;
                        ruleItem.FoldGroupItem = groupItem;
                    }
                    groupItem.IsFold = true;
                    groupItem.HideWhenNoSubItem();
                }
            }
            catch { }
        }

        private XmlDocument ReadXml()
        {
            XmlDocument doc1 = new XmlDocument();
            try
            {
                if(File.Exists(AppConfig.WebThirdRulesDic))
                {
                    doc1.LoadXml(File.ReadAllText(AppConfig.WebThirdRulesDic, EncodingType.GetType(AppConfig.WebThirdRulesDic)));
                }
                else
                {
                    doc1.LoadXml(Properties.Resources.ThirdRulesDic);
                }
                if(File.Exists(AppConfig.UserThirdRulesDic))
                {
                    XmlDocument doc2 = new XmlDocument();
                    doc2.LoadXml(File.ReadAllText(AppConfig.UserThirdRulesDic, EncodingType.GetType(AppConfig.UserThirdRulesDic)));
                    foreach(XmlNode xn in doc2.DocumentElement.ChildNodes)
                    {
                        XmlNode node = doc1.ImportNode(xn, true);
                        doc1.DocumentElement.AppendChild(node);
                    }
                }
            }
            catch { }
            return doc1;
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