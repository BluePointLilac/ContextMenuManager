using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ContextMenuManager.Controls
{
    sealed class EnhanceShellItem : MyListItem, IFoldSubItem, IChkVisibleItem
    {
        public string RegPath { get; set; }
        public XmlElement ItemXE { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public IFoldGroupItem FoldGroupItem { get; set; }
        public bool ItemVisible
        {
            get
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(RegPath))
                    return key != null;
            }
            set
            {
                if(value) WriteSubKeysValue(ItemXE, RegPath);
                else RegistryEx.DeleteKeyTree(RegPath);
            }
        }

        public EnhanceShellItem()
        {
            ChkVisible = new VisibleCheckBox(this);
        }

        private static void WriteAttributesValue(XmlNode valueXN, string regPath)
        {
            if(valueXN == null) return;
            XmlNode szXN = valueXN.SelectSingleNode("REG_SZ");
            XmlNode binaryXN = valueXN.SelectSingleNode("REG_SZ");
            XmlNode dwordXN = valueXN.SelectSingleNode("REG_DWORD");
            XmlNode expand_szXN = valueXN.SelectSingleNode("REG_EXPAND_SZ");
            using(RegistryKey key = RegistryEx.GetRegistryKey(regPath, true, true))
            {
                if(szXN != null)
                    foreach(XmlAttribute a in szXN.Attributes)
                        key.SetValue(a.Name, a.Value, RegistryValueKind.String);
                if(expand_szXN != null)
                    foreach(XmlAttribute a in expand_szXN.Attributes)
                        key.SetValue(a.Name, a.Value, RegistryValueKind.ExpandString);
                if(binaryXN != null)
                {
                    foreach(XmlAttribute a in binaryXN.Attributes)
                        key.SetValue(a.Name, EnhanceMenusList.ConvertToBinary(a.Value), RegistryValueKind.Binary);
                }
                if(dwordXN != null)
                    foreach(XmlAttribute a in dwordXN.Attributes)
                    {
                        int value = a.Value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                            ? Convert.ToInt32(a.Value, 16) : Convert.ToInt32(a.Value);
                        key.SetValue(a.Name, value, RegistryValueKind.DWord);
                    }

            }
        }

        private static void WriteSubKeysValue(XmlElement keyXE, string regPath)
        {
            if(keyXE == null) return;
            string defaultValue = Environment.ExpandEnvironmentVariables(keyXE.GetAttribute("Default"));
            if(!defaultValue.IsNullOrWhiteSpace())
            {
                Registry.SetValue(regPath, "", defaultValue);
            }
            else if(keyXE.Name == "Command")
            {
                //按照规则Command节点无默认值则创建文件
                WriteCommandValue(keyXE, regPath);
            }
            WriteAttributesValue(keyXE.SelectSingleNode("Value"), regPath);

            XmlNode subKeyXN = keyXE.SelectSingleNode("SubKey");
            if(subKeyXN != null)
            {
                foreach(XmlElement xe in subKeyXN.ChildNodes)
                    WriteSubKeysValue(xe, $@"{regPath}\{xe.Name}");
            }
        }

        private static void WriteCommandValue(XmlElement cmdXE, string regPath)
        {
            XmlElement fnXE = (XmlElement)cmdXE.SelectSingleNode("FileName");
            XmlElement argXE = (XmlElement)cmdXE.SelectSingleNode("Arguments");
            XmlElement seXE = (XmlElement)cmdXE.SelectSingleNode("ShellExecute");

            string command;
            string fileName = fnXE?.InnerText.Trim();
            string arguments = argXE?.InnerText.Trim();
            if(string.IsNullOrEmpty(fileName)) fileName = CreateCommandFile(fnXE);
            if(string.IsNullOrEmpty(arguments)) arguments = CreateCommandFile(argXE);
            fileName = Environment.ExpandEnvironmentVariables(fileName);
            arguments = Environment.ExpandEnvironmentVariables(arguments);
            string prefix = argXE?.GetAttribute("Prefix");
            string suffix = argXE?.GetAttribute("Suffix");
            arguments = prefix + arguments + suffix;
            if(seXE != null)
            {
                string verb = seXE.HasAttribute("Verb") ? seXE.GetAttribute("Verb") : "open";
                int windowStyle = seXE.HasAttribute("WindowStyle") ? Convert.ToInt32(seXE.GetAttribute("WindowStyle")) : 1;
                string directory = Environment.ExpandEnvironmentVariables(seXE.GetAttribute("Directory"));
                command = ShellExecuteDialog.GetCommand(fileName, arguments, verb, windowStyle, directory);
            }
            else
            {
                command = fileName;
                if(arguments != string.Empty) command += $" {arguments}";
            }
            Registry.SetValue(regPath, "", command);
        }

        private static string CreateCommandFile(XmlElement xe)
        {
            if(xe == null) return string.Empty;
            XmlElement cfXE = (XmlElement)xe.SelectSingleNode("CreateFile");
            if(cfXE == null) return string.Empty;
            string fileName = cfXE.GetAttribute("FileName");
            string content = cfXE.GetAttribute("Content");
            string extension = Path.GetExtension(fileName).ToLower();
            string filePath = $@"{AppConfig.ProgramsDir}\{fileName}";
            Encoding encoding = Encoding.Unicode;
            if(extension == ".bat" || extension == ".cmd") encoding = Encoding.Default;
            if(File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllText(filePath, content, encoding);
            return filePath;
        }
    }

    sealed class EnhanceShellExItem : MyListItem, IFoldSubItem, IChkVisibleItem
    {
        public string ShellExPath { get; set; }
        public string DefaultKeyName { get; set; }
        public Guid Guid { get; set; }
        public VisibleCheckBox ChkVisible { get; set; }
        public IFoldGroupItem FoldGroupItem { get; set; }
        public bool ItemVisible
        {
            get => ShellExItem.GetPathAndGuids(ShellExPath).Values.Contains(Guid);
            set
            {
                if(value)
                {
                    string regPath = ObjectPath.GetNewPathWithIndex
                    ($@"{ShellExPath}\ContextMenuHandlers\{DefaultKeyName}", ObjectPath.PathType.Registry);
                    Registry.SetValue(regPath, "", this.Guid.ToString("B"));
                }
                else
                {
                    Dictionary<string, Guid> dic = ShellExItem.GetPathAndGuids(ShellExPath);
                    foreach(string regPath in dic.Keys)
                    {
                        if(dic[regPath].Equals(this.Guid))
                            RegistryEx.DeleteKeyTree(regPath);
                    }
                }
            }
        }

        public EnhanceShellExItem()
        {
            ChkVisible = new VisibleCheckBox(this);
        }
    }
}