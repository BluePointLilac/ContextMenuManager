using BulePointLilac.Controls;
using BulePointLilac.Methods;
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
            this.ClearItems();
            XmlDocument doc1 = new XmlDocument();
            if(!File.Exists(Program.AppDataThirdRulesDicPath))
            {
                File.WriteAllText(Program.AppDataThirdRulesDicPath, Properties.Resources.ThirdRulesDic, Encoding.UTF8);
            }
            doc1.Load(Program.AppDataThirdRulesDicPath);
            if(File.Exists(Program.ThirdRulesDicPath))
            {
                XmlDocument doc2 = new XmlDocument();
                doc2.Load(Program.ThirdRulesDicPath);
                foreach(XmlNode xn in doc2.DocumentElement.ChildNodes)
                {
                    XmlNode node = doc1.ImportNode(xn, true);
                    doc1.DocumentElement.AppendChild(node);
                }
            }
            foreach(XmlElement groupXE in doc1.DocumentElement.ChildNodes)
            {
                if(!GuidInfo.TryGetGuid(groupXE.GetAttribute("Guid"), out Guid guid)
                    && !groupXE.HasAttribute("Common")) continue;

                GroupPathItem groupItem = new GroupPathItem
                {
                    Text = groupXE.GetAttribute("Text"),
                    TargetPath = groupXE.GetAttribute("RegPath"),
                    PathType = ObjectPath.PathType.Registry,
                    Image = GuidInfo.GetImage(guid),
                };
                if(string.IsNullOrWhiteSpace(groupItem.Text)) groupItem.Text = GuidInfo.GetText(guid);
                this.AddItem(groupItem);

                foreach(XmlElement itemXE in groupXE.ChildNodes)
                {
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

                    this.AddItem(new RegRuleItem(rules, itemInfo) { FoldGroupItem = groupItem });
                }
                groupItem.IsFold = true;
            }
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