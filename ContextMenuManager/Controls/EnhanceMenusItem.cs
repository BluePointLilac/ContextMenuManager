using BulePointLilac.Controls;
using BulePointLilac.Methods;
using ContextMenuManager.Controls.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this.SetNoClickEvent();
        }

        private static void WriteAttributesValue(XmlNode valueXN, string regPath)
        {
            if(valueXN == null) return;
            XmlNode szXN = valueXN.SelectSingleNode("REG_SZ");
            XmlNode dwordXN = valueXN.SelectSingleNode("REG_DWORD");
            XmlNode expand_szXN = valueXN.SelectSingleNode("REG_EXPAND_SZ");
            if(szXN != null)
                foreach(XmlAttribute a in szXN.Attributes)
                    Registry.SetValue(regPath, a.Name, a.Value, RegistryValueKind.String);
            if(expand_szXN != null)
                foreach(XmlAttribute a in expand_szXN.Attributes)
                    Registry.SetValue(regPath, a.Name, a.Value, RegistryValueKind.ExpandString);
            if(dwordXN != null)
                foreach(XmlAttribute a in dwordXN.Attributes)
                {
                    int value = a.Value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                        ? Convert.ToInt32(a.Value, 16) : Convert.ToInt32(a.Value);
                    Registry.SetValue(regPath, a.Name, value, RegistryValueKind.DWord);
                }
        }

        private static void WriteSubKeysValue(XmlElement keyXE, string regPath)
        {
            if(keyXE == null) return;
            string defaultValue = keyXE.GetAttribute("Default");
            if(!defaultValue.IsNullOrWhiteSpace()) Registry.SetValue(regPath, "", defaultValue);
            WriteAttributesValue(keyXE.SelectSingleNode("Value"), regPath);

            XmlNode subKeyXN = keyXE.SelectSingleNode("SubKey");
            if(subKeyXN != null)
            {
                foreach(XmlElement xe in subKeyXN.ChildNodes)
                    WriteSubKeysValue(xe, $@"{regPath}\{xe.Name}");
            }
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