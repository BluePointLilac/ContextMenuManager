using BluePointLilac.Methods;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ContextMenuManager.Methods
{
    static class XmlDicHelper
    {
        public static readonly List<XmlDocument> EnhanceMenusDic = new List<XmlDocument>();
        public static readonly List<XmlDocument> DetailedEditDic = new List<XmlDocument>();
        public static readonly List<XmlDocument> UwpModeItemsDic = new List<XmlDocument>();
        public static readonly Dictionary<string, bool> EnhanceMenuPathDic
            = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public static readonly Dictionary<Guid, bool> DetailedEditGuidDic = new Dictionary<Guid, bool>();

        /// <summary>重新加载字典</summary>
        public static void ReloadDics()
        {
            XmlDocument LoadXml(string xmlPath)
            {
                if(!File.Exists(xmlPath)) return null;
                try
                {
                    XmlDocument doc = new XmlDocument();
                    Encoding encoding = EncodingType.GetType(xmlPath);
                    string xml = File.ReadAllText(xmlPath, encoding).Trim();
                    doc.LoadXml(xml);
                    return doc;
                }
                catch(Exception e)
                {
                    AppMessageBox.Show(e.Message + "\n" + xmlPath);
                    return null;
                }
            }

            void LoadDic(List<XmlDocument> dic, string webPath, string userPath, string defaultContent)
            {
                if(!File.Exists(webPath)) File.WriteAllText(webPath, defaultContent, Encoding.Unicode);
                dic.Clear();
                dic.Add(LoadXml(webPath));
                dic.Add(LoadXml(userPath));
            }

            LoadDic(UwpModeItemsDic, AppConfig.WebUwpModeItemsDic,
                AppConfig.UserUwpModeItemsDic, Properties.Resources.UwpModeItemsDic);
            LoadDic(EnhanceMenusDic, AppConfig.WebEnhanceMenusDic,
                AppConfig.UserEnhanceMenusDic, Properties.Resources.EnhanceMenusDic);
            LoadDic(DetailedEditDic, AppConfig.WebDetailedEditDic,
                AppConfig.UserDetailedEditDic, Properties.Resources.DetailedEditDic);

            EnhanceMenuPathDic.Clear();
            for(int i = 0; i < 2; i++)
            {
                XmlDocument doc = EnhanceMenusDic[i];
                if(doc?.DocumentElement == null) continue;
                foreach(XmlNode pathXN in doc.SelectNodes("Data/Group/RegPath"))
                {
                    if(EnhanceMenuPathDic.ContainsKey(pathXN.InnerText)) continue;
                    EnhanceMenuPathDic.Add(pathXN.InnerText, i == 1);
                }
            }

            DetailedEditGuidDic.Clear();
            for(int i = 0; i < 2; i++)
            {
                XmlDocument doc = DetailedEditDic[i];
                if(doc?.DocumentElement == null) continue;
                foreach(XmlNode guidXN in doc.SelectNodes("Data/Group/Guid"))
                {
                    if(GuidEx.TryParse(guidXN.InnerText, out Guid guid))
                    {
                        if(DetailedEditGuidDic.ContainsKey(guid)) continue;
                        DetailedEditGuidDic.Add(guid, i == 1);
                    }
                }
            }
        }

        public static bool JudgeOSVersion(XmlNode itemXN)
        {
            //return true;//测试用
            bool JudgeOne(XmlNode osXN)
            {
                Version ver = new Version(osXN.InnerText);
                Version osVer = Environment.OSVersion.Version;
                int compare = osVer.CompareTo(ver);
                string symbol = ((XmlElement)osXN).GetAttribute("Compare");
                switch(symbol)
                {
                    case ">":
                        return compare > 0;
                    case "<":
                        return compare < 0;
                    case "=":
                        return compare == 0;
                    case ">=":
                        return compare >= 0;
                    case "<=":
                        return compare <= 0;
                    default:
                        return true;
                }
            }

            foreach(XmlNode osXN in itemXN.SelectNodes("OSVersion"))
            {
                if(!JudgeOne(osXN)) return false;
            }
            return true;
        }

        public static bool FileExists(XmlNode itemXN)
        {
            //return true;//测试用
            foreach(XmlNode feXN in itemXN.SelectNodes("FileExists"))
            {
                string path = Environment.ExpandEnvironmentVariables(feXN.InnerText);
                if(!File.Exists(path)) return false;
            }
            return true;
        }

        public static bool JudgeCulture(XmlNode itemXN)
        {
            //return true;//测试用
            string culture = itemXN.SelectSingleNode("Culture")?.InnerText;
            if(string.IsNullOrEmpty(culture)) return true;
            if(culture.Equals(AppConfig.Language, StringComparison.OrdinalIgnoreCase)) return true;
            if(culture.Equals(CultureInfo.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static byte[] ConvertToBinary(string value)
        {
            try
            {
                string[] strs = value.Split(' ');
                byte[] bs = new byte[strs.Length];
                for(int i = 0; i < strs.Length; i++)
                {
                    bs[i] = Convert.ToByte(strs[i], 16);
                }
                return bs;
            }
            catch { return null; }
        }

        public static RegistryValueKind GetValueKind(string type, RegistryValueKind defaultKind)
        {
            switch(type.ToUpper())
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
                    return defaultKind;
            }
        }
    }
}