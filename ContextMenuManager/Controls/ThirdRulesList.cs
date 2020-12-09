using BulePointLilac.Controls;
using BulePointLilac.Methods;
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
            try
            {
                foreach(XmlElement groupXE in ReadXml().DocumentElement.ChildNodes)
                {
                    Guid guid = Guid.Empty;
                    if(groupXE.HasAttribute("Guid") && !GuidInfo.TryGetGuid(groupXE.GetAttribute("Guid"), out guid)) continue;

                    GroupPathItem groupItem = new GroupPathItem(groupXE.GetAttribute("RegPath"), ObjectPath.PathType.Registry)
                    {
                        Text = groupXE.GetAttribute("Text"),
                        Image = GuidInfo.GetImage(guid),
                    };
                    if(groupItem.Text.IsNullOrWhiteSpace()) groupItem.Text = GuidInfo.GetText(guid);
                    this.AddItem(groupItem);

                    foreach(XmlElement itemXE in groupXE.ChildNodes)
                    {
                        XmlElement verXE = (XmlElement)itemXE.SelectSingleNode("OSVersion");
                        if(!EnhanceMenusList.JudgeOSVersion(verXE)) continue;
                        RegRuleItem.ItemInfo itemInfo = new RegRuleItem.ItemInfo
                        {
                            Text = itemXE.GetAttribute("Text"),
                            Tip = itemXE.GetAttribute("Tip"),
                            RestartExplorer = itemXE.HasAttribute("RestartExplorer"),
                        };

                        XmlNodeList ruleXNList = itemXE.GetElementsByTagName("Rule");//Rules
                        RegRuleItem.RegRule[] rules = new RegRuleItem.RegRule[ruleXNList.Count];
                        for(int i = 0; i < ruleXNList.Count; i++)
                        {
                            XmlElement ruleXE = (XmlElement)ruleXNList[i];
                            rules[i] = new RegRuleItem.RegRule
                            {
                                RegPath = ruleXE.GetAttribute("RegPath"),
                                ValueName = ruleXE.GetAttribute("ValueName"),
                                TurnOnValue = ruleXE.GetAttribute("On"),
                                TurnOffValue = ruleXE.GetAttribute("Off"),
                                ValueKind = GetValueKind(ruleXE.GetAttribute("ValueKind"))
                            };
                            if(string.IsNullOrEmpty(rules[i].RegPath)) rules[i].RegPath = groupItem.TargetPath;
                            else if(rules[i].RegPath.StartsWith("\\")) rules[i].RegPath = groupItem.TargetPath + rules[i].RegPath;
                        }

                        this.AddItem(new RegRuleItem(rules, itemInfo) { FoldGroupItem = groupItem, HasImage = false });
                    }
                    groupItem.IsFold = true;
                }
            }
            catch { }
        }

        private XmlDocument ReadXml()
        {
            XmlDocument doc1 = new XmlDocument();
            try
            {
                if(!File.Exists(AppConfig.WebThirdRulesDic))
                {
                    File.WriteAllText(AppConfig.WebThirdRulesDic, Properties.Resources.ThirdRulesDic, Encoding.UTF8);
                }
                doc1.Load(AppConfig.WebThirdRulesDic);
                if(File.Exists(AppConfig.UserThirdRulesDic))
                {
                    XmlDocument doc2 = new XmlDocument();
                    doc2.Load(AppConfig.UserThirdRulesDic);
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