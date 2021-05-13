using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ContextMenuManager
{
    sealed class Updater
    {
        const string GithubLatest = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/releases/latest";
        const string GiteeLatest = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/releases/latest";
        const string GithubLangs = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/contents/languages";
        const string GiteeLangs = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/languages";
        const string GithubTexts = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/Texts";
        const string GiteeTexts = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts";
        const string GithubDonate = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/Donate.md";
        const string GiteeDonate = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Donate.md";

        public static void PeriodicUpdate()
        {
            int day = AppConfig.UpdateFrequency;
            if(day == -1) return;//自动检测更新频率为-1则从不自动检查更新
            //如果上次检测更新时间加上时间间隔早于今天以前就进行更新操作
            if(AppConfig.LastCheckUpdateTime.AddDays(day) < DateTime.Today) Update();
        }

        public static bool Update()
        {
            AppConfig.LastCheckUpdateTime = DateTime.Today;
            UpdateText();
            return UpdateApp();
        }

        private static bool UpdateApp()
        {
            string url = AppConfig.RequestUseGithub ? GithubLatest : GiteeLatest;
            XmlDocument doc = GetWebJsonToXml(url);
            if(doc == null) return false;
            XmlNode root = doc.FirstChild;
            XmlElement tagNameXE = (XmlElement)root.SelectSingleNode("tag_name");
            Version webVer = new Version(tagNameXE.InnerText);
            Version appVer = new Version(Application.ProductVersion);
            if(webVer > appVer)
            {
                XmlElement bodyXE = (XmlElement)root.SelectSingleNode("body");
                string info = AppString.Message.UpdateInfo.Replace("%v1", appVer.ToString()).Replace("%v2", webVer.ToString());
                info += "\r\n\r\n" + MachinedInfo(bodyXE.InnerText);
                if(MessageBoxEx.Show(info, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string netVer = Environment.Version > new Version(4, 0) ? "4.0" : "3.5";
                    XmlElement assetsXE = (XmlElement)root.SelectSingleNode("assets");
                    foreach(XmlElement itemXE in assetsXE.SelectNodes("item"))
                    {
                        XmlElement nameXE = (XmlElement)itemXE.SelectSingleNode("name");
                        if(nameXE != null && nameXE.InnerText.Contains(netVer))
                        {
                            XmlElement urlXE = (XmlElement)itemXE.SelectSingleNode("browser_download_url");
                            using(DownloadDialog dlg = new DownloadDialog())
                            {
                                dlg.Url = urlXE?.InnerText;
                                dlg.FilePath = Path.GetTempFileName();
                                if(dlg.ShowDialog() == DialogResult.OK)
                                {
                                    MessageBoxEx.Show(AppString.Message.UpdateSucceeded, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    SingleInstance.Restart(null, dlg.FilePath);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private static void UpdateText()
        {
            string url = AppConfig.RequestUseGithub ? GithubTexts : GiteeTexts;
            string[] fileNames = new[]
            {
                AppConfig.GUIDINFOSDICINI, AppConfig.ENHANCEMENUSICXML,
                AppConfig.THIRDRULESDICXML, AppConfig.UWPMODEITEMSDICXML
            };
            foreach(string fileName in fileNames)
            {
                string fileUrl = $"{url}/{fileName}";
                string filePath = $@"{AppConfig.WebDicsDir}\{fileName}";
                string contents = GetWebString(fileUrl);
                if(string.IsNullOrEmpty(contents)) continue;
                contents = contents.Replace("\n", Environment.NewLine);
                File.WriteAllText(filePath, contents, Encoding.Unicode);
            }
        }

        public static bool ShowLanguageDialog()
        {
            string url = AppConfig.RequestUseGithub ? GithubLangs : GiteeLangs;
            XmlDocument doc = GetWebJsonToXml(url);
            if(doc == null) return false;
            Dictionary<string, string> langs = new Dictionary<string, string>();
            foreach(XmlElement itemXE in doc.FirstChild.SelectNodes("item"))
            {
                XmlElement nameXE = (XmlElement)itemXE.SelectSingleNode("name");
                XmlElement urlXE = (XmlElement)itemXE.SelectSingleNode("download_url");
                string lang = Path.GetFileNameWithoutExtension(nameXE.InnerText);
                langs.Add(lang, urlXE.InnerText);
            }
            using(SelectDialog dlg = new SelectDialog())
            {
                dlg.Items = langs.Keys.ToArray();
                dlg.Title = AppString.Dialog.DownloadLanguages;
                string lang = CultureInfo.CurrentUICulture.Name;
                if(dlg.Items.Contains(lang)) dlg.Selected = lang;
                else dlg.SelectedIndex = 0;
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    lang = dlg.Selected;
                    string contents = GetWebString(langs[lang]);
                    string filePath = $@"{AppConfig.LangsDir}\{lang}.ini";
                    File.WriteAllText(filePath, contents, Encoding.Unicode);
                    return true;
                }
            }
            return false;
        }

        public static void ShowDonateDialog()
        {
            string url = AppConfig.RequestUseGithub ? GithubDonate : GiteeDonate;
            string contents = GetWebString(url);
            //string contents = File.ReadAllText(@"..\..\..\Donate.md");//用于求和更新Donate.md文件
            if(contents == null) ExternalProgram.OpenUrl(url);
            else
            {
                using(DonateListDialog dlg = new DonateListDialog())
                {
                    dlg.DanateData = contents;
                    dlg.ShowDialog();
                }
            }
        }

        private static string MachinedInfo(string info)
        {
            string str = string.Empty;
            string[] lines = info.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for(int m = 0; m < lines.Length; m++)
            {
                string line = lines[m];
                for(int n = 1; n <= 6; n++)
                {
                    if(line.StartsWith(new string('#', n) + ' '))
                    {
                        line = line.Substring(n + 1);
                        break;
                    }
                }
                str += line + Environment.NewLine;
            }
            return str;
        }

        private static string GetWebString(string url)
        {
            try
            {
                using(WebResponse response = WebRequest.Create(url).GetResponse())
                using(StreamReader stream = new StreamReader(response.GetResponseStream()))
                    return stream?.ReadToEnd();
            }
            catch { return null; }
        }

        private static XmlDocument GetWebJsonToXml(string url)
        {
            try
            {
                using(WebResponse response = WebRequest.Create(url).GetResponse())
                using(StreamReader stream = new StreamReader(response.GetResponseStream()))
                using(XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(stream?.BaseStream, XmlDictionaryReaderQuotas.Max))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);
                    return doc;
                }
            }
            catch { return null; }
        }
    }
}