using BulePointLilac.Controls;
using BulePointLilac.Methods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static Microsoft.Win32.Registry;

namespace ContextMenuManager.Controls
{
    sealed class ShellCommonDialog : CommonDialog
    {
        public List<string> SelectedShellPaths { get; private set; }
        public Dictionary<string, Guid> SelectedShellExPathAndGuids { get; private set; }
        public string ShellExPath { get; set; }
        public string ShellPath { get; set; }
        public string ScenePath { get; set; }
        public override void Reset() { }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            using(ShellCommonForm frm = new ShellCommonForm(ScenePath, ShellPath, ShellExPath))
            {
                if(frm.ShowDialog() == DialogResult.OK)
                {
                    this.SelectedShellPaths = frm.SelectedShellPaths;
                    this.SelectedShellExPathAndGuids = frm.SelectedShellExPathAndGuids;
                    return true;
                }
                return false;
            }
        }

        sealed class ShellCommonForm : SelectItemsForm
        {
            public ShellCommonForm(string scenePath, string shellPath, string shellExPath)
            {
                this.ScenePath = scenePath;
                this.ShellPath = shellPath;
                this.ShellExPath = shellExPath;
                this.Text = AppString.Text_CheckCommon;
                btnOk.Click += (sender, e) => GetSelectedItems();
                LoadItems();
            }

            public List<string> SelectedShellPaths { get; private set; } = new List<string>();
            public Dictionary<string, Guid> SelectedShellExPathAndGuids { get; private set; } = new Dictionary<string, Guid>();
            public string ScenePath { get; set; }
            public string ShellPath { get; set; }
            public string ShellExPath { get; set; }
            private XmlElement shellXE;
            private XmlElement shellExXE;

            public void LoadItems()
            {
                XmlDocument doc1 = new XmlDocument();
                if(!File.Exists(Program.AppDataShellCommonDicPath))
                {
                    File.WriteAllText(Program.AppDataShellCommonDicPath, Properties.Resources.ShellCommonDic, Encoding.UTF8);
                }
                doc1.Load(Program.AppDataShellCommonDicPath);

                if(File.Exists(Program.ShellCommonDicPath))
                {
                    XmlDocument doc2 = new XmlDocument();
                    doc2.Load(Program.ShellCommonDicPath);
                    foreach(XmlNode xn in doc2.DocumentElement.ChildNodes)
                    {
                        XmlNode node = doc1.ImportNode(xn, true);
                        doc1.DocumentElement.AppendChild(node);
                    }
                }

                foreach(XmlElement groupXE in doc1.DocumentElement.ChildNodes)
                {
                    string scenePath = groupXE.GetAttribute("RegPath");
                    if(ScenePath.Equals(scenePath, StringComparison.OrdinalIgnoreCase))
                    {
                        shellXE = (XmlElement)groupXE.SelectSingleNode("Shell");
                        shellExXE = (XmlElement)groupXE.SelectSingleNode("ShellEx");
                        if(shellXE != null) LoadShellItems();
                        if(ShellExPath != null && shellExXE != null) LoadShellExItems();
                    }
                }
            }

            private void LoadShellItems()
            {
                foreach(XmlElement itemXE in shellXE.GetElementsByTagName("Item"))
                {
                    XmlElement szXE = (XmlElement)itemXE.SelectSingleNode("Value/REG_SZ");
                    string keyName = itemXE.GetAttribute("KeyName");
                    if(string.IsNullOrWhiteSpace(keyName)) continue;
                    ShellCommonItem item = new ShellCommonItem
                    {
                        DefaultKeyName = keyName,
                        ItemXE = itemXE
                    };
                    if(szXE != null)
                    {
                        item.Text = ResourceString.GetDirectString(szXE.GetAttribute("MUIVerb"));
                        if(szXE.HasAttribute("Icon")) item.Image = ResourceIcon.GetIcon(szXE.GetAttribute("Icon"))?.ToBitmap();
                        else if(szXE.HasAttribute("HasLUAShield")) item.Image = AppImage.Shield;
                    }
                    if(item.Image == null) item.Image = AppImage.NotFound;
                    if(string.IsNullOrWhiteSpace(item.Text)) item.Text = item.DefaultKeyName;
                    item.SetTip(itemXE.GetAttribute("Tip"));
                    list.AddItem(item);
                }
            }

            private void LoadShellExItems()
            {
                foreach(XmlElement itemXE in shellExXE.GetElementsByTagName("Item"))
                {
                    if(!GuidInfo.TryGetGuid(itemXE.GetAttribute("Guid"), out Guid guid)) continue;
                    if(ShellExItem.GetPathAndGuids(ShellExPath).Values.Contains(guid)) continue;
                    ShellExCommonItem item = new ShellExCommonItem
                    {
                        Image = ResourceIcon.GetIcon(itemXE.GetAttribute("Icon"))?.ToBitmap() ?? AppImage.DllDefaultIcon,
                        Text = ResourceString.GetDirectString(itemXE.GetAttribute("Text")),
                        DefaultKeyName = itemXE.GetAttribute("KeyName"),
                        Guid = guid
                    };
                    if(string.IsNullOrWhiteSpace(item.Text)) item.Text = GuidInfo.GetText(guid);
                    if(string.IsNullOrWhiteSpace(item.DefaultKeyName)) item.DefaultKeyName = guid.ToString("B");
                    item.SetTip(itemXE.GetAttribute("Tip"));
                    list.AddItem(item);
                }
            }

            private void GetSelectedItems()
            {
                foreach(Control ctr in list.Controls)
                {
                    if(ctr.GetType() == typeof(ShellCommonItem)) CreateShellItem((ShellCommonItem)ctr);
                    else if(ctr.GetType() == typeof(ShellExCommonItem)) CreateShellExItem((ShellExCommonItem)ctr);
                }
            }

            private void CreateShellItem(ShellCommonItem item)
            {
                if(!item.IsSelected) return;
                string regPath = ObjectPath.GetNewPathWithIndex
                    ($@"{ShellPath}\{item.DefaultKeyName}", ObjectPath.PathType.Registry);
                ShellCommonItem.WriteSubKeysValue(item.ItemXE, regPath);
                SelectedShellPaths.Add(regPath);
            }

            private void CreateShellExItem(ShellExCommonItem item)
            {
                if(!item.IsSelected) return;
                string regPath = ObjectPath.GetNewPathWithIndex
                    ($@"{ShellExPath}\ContextMenuHandlers\{item.DefaultKeyName}", ObjectPath.PathType.Registry);
                SetValue(regPath, "", item.Guid.ToString("B"));
                SelectedShellExPathAndGuids.Add(regPath, item.Guid);
            }
        }
    }

    class CheckBoxItem : MyListItem
    {
        readonly CheckBox chkSelected = new CheckBox { AutoSize = true };
        public bool IsSelected => chkSelected.Checked;

        public CheckBoxItem()
        {
            this.AddCtr(chkSelected);
        }

        public void SetTip(string tip)
        {
            MyToolTip.SetToolTip(chkSelected, tip);
        }
    }

    sealed class ShellCommonItem : CheckBoxItem
    {
        public string DefaultKeyName { get; set; }
        public XmlElement ItemXE { get; set; }

        public static void WriteAttributesValue(XmlNode valueXN, string regPath)
        {
            if(valueXN == null) return;
            XmlNode szXN = valueXN.SelectSingleNode("REG_SZ");
            XmlNode dwordXN = valueXN.SelectSingleNode("REG_DWORD");
            XmlNode expand_szXN = valueXN.SelectSingleNode("REG_EXPAND_SZ");
            if(szXN != null)
                foreach(XmlAttribute a in szXN.Attributes)
                    SetValue(regPath, a.Name, a.Value, Microsoft.Win32.RegistryValueKind.String);
            if(expand_szXN != null)
                foreach(XmlAttribute a in expand_szXN.Attributes)
                    SetValue(regPath, a.Name, a.Value, Microsoft.Win32.RegistryValueKind.ExpandString);
            if(dwordXN != null)
                foreach(XmlAttribute a in dwordXN.Attributes)
                {
                    int value = a.Value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                        ? Convert.ToInt32(a.Value, 16) : Convert.ToInt32(a.Value);
                    SetValue(regPath, a.Name, value, Microsoft.Win32.RegistryValueKind.DWord);
                }
        }

        public static void WriteSubKeysValue(XmlElement keyXE, string regPath)
        {
            if(keyXE == null) return;
            string defaultValue = keyXE.GetAttribute("Default");
            if(!string.IsNullOrWhiteSpace(defaultValue)) SetValue(regPath, "", defaultValue);
            WriteAttributesValue(keyXE.SelectSingleNode("Value"), regPath);

            XmlNode subKeyXN = keyXE.SelectSingleNode("SubKey");
            if(subKeyXN != null)
            {
                foreach(XmlElement xe in subKeyXN.ChildNodes)
                    WriteSubKeysValue(xe, $@"{regPath}\{xe.Name}");
            }
        }
    }

    sealed class ShellExCommonItem : CheckBoxItem
    {
        public string DefaultKeyName { get; set; }
        public Guid Guid { get; set; }
    }
}