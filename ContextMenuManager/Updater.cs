using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ContextMenuManager
{
    sealed class Updater
    {
        const string GithubLatest = "https://github.com/BluePointLilac/ContextMenuManager/releases/latest";
        const string GithubLatestApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/releases/latest";
        const string GithubLangsApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/contents/languages";
        const string GithubTexts = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/Texts";
        const string GithubDonateRaw = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/Donate.md";
        const string GithubDonate = "https://github.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        const string GiteeReleases = "https://gitee.com/BluePointLilac/ContextMenuManager/releases";
        const string GiteeLatestApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/releases/latest";
        const string GiteeLangsApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/languages";
        const string GiteeTexts = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts";
        const string GiteeDonateRaw = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Donate.md";
        const string GiteeDonate = "https://gitee.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        /// <summary>定期检查更新</summary>
        public static void PeriodicUpdate()
        {
            int day = AppConfig.UpdateFrequency;
            if(day == -1) return;//自动检测更新频率为-1则从不自动检查更新
            //如果上次检测更新时间加上时间间隔早于或等于今天以前就进行更新操作
            if(AppConfig.LastCheckUpdateTime.AddDays(day) <= DateTime.Today) Update(false);
        }

        /// <summary>更新程序以及程序字典</summary>
        /// <param name="isManual">是否为手动点击更新</param>
        public static void Update(bool isManual)
        {
            AppConfig.LastCheckUpdateTime = DateTime.Today;
            UpdateText(isManual);
            UpdateApp(isManual);
        }

        /// <summary>更新程序</summary>
        /// <param name="isManual">是否为手动点击更新</param>
        private static void UpdateApp(bool isManual)
        {
            string url = AppConfig.RequestUseGithub ? GithubLatestApi : GiteeLatestApi;
            XmlDocument doc = GetWebJsonToXml(url);
            if(doc == null)
            {
                if(isManual)
                {
                    MessageBoxEx.Show(AppString.Message.NetworkDtaReadFailed);
                    url = AppConfig.RequestUseGithub ? GithubLatest : GiteeReleases;
                    ExternalProgram.OpenUrl(url);
                }
                return;
            }
            XmlNode root = doc.FirstChild;
            XmlElement tagNameXE = (XmlElement)root.SelectSingleNode("tag_name");
            Version webVer = new Version(tagNameXE.InnerText);
            Version appVer = new Version(Application.ProductVersion);
            //appVer = new Version(0, 0, 0, 0);//测试用
            if(appVer >= webVer)
            {
                if(isManual) MessageBoxEx.Show(AppString.Message.VersionIsLatest);
            }
            else
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
            }
        }

        /// <summary>更新程序字典</summary>
        /// <param name="isManual">是否为手动点击更新</param>
        private static void UpdateText(bool isManual)
        {
            bool flag = isManual;
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
                if(string.IsNullOrEmpty(contents)) { flag = false; continue; }
                contents = contents.Replace("\n", Environment.NewLine);
                File.WriteAllText(filePath, contents, Encoding.Unicode);
            }
            if(!flag) MessageBoxEx.Show(AppString.Message.NetworkDtaReadFailed);
        }

        /// <summary>显示语言下载对话框</summary>
        /// <returns>返回值为是否下载了语言文件</returns>
        public static bool ShowLanguageDialog()
        {
            string url = AppConfig.RequestUseGithub ? GithubLangsApi : GiteeLangsApi;
            XmlDocument doc = GetWebJsonToXml(url);
            if(doc == null)
            {
                MessageBoxEx.Show(AppString.Message.NetworkDtaReadFailed);
                return false;
            }
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

        /// <summary>显示捐赠名单对话框</summary>
        public static void ShowDonateDialog()
        {
            string url = AppConfig.RequestUseGithub ? GithubDonateRaw : GiteeDonateRaw;
            string contents = GetWebString(url);
            //contents = File.ReadAllText(@"..\..\..\Donate.md");//用于求和更新Donate.md文件
            if(contents == null)
            {
                MessageBoxEx.Show(AppString.Message.NetworkDtaReadFailed);
                url = AppConfig.RequestUseGithub ? GithubDonate : GiteeDonate;
                ExternalProgram.OpenUrl(url);
            }
            else
            {
                using(DonateListDialog dlg = new DonateListDialog())
                {
                    dlg.DanateData = contents;
                    dlg.ShowDialog();
                }
            }
        }

        /// <summary>加工处理更新信息，去掉标题头</summary>
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

        /// <summary>获取网页文本</summary>
        private static string GetWebString(string url)
        {
            try
            {
                using(UAWebClient client = new UAWebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch { return null; }
        }

        /// <summary>获取网页Json文本并加工为Xml</summary>
        private static XmlDocument GetWebJsonToXml(string url)
        {
            try
            {
                using(UAWebClient client = new UAWebClient())
                {
                    byte[] bytes = client.DownloadData(url);
                    using(XmlReader xReader = JsonReaderWriterFactory.CreateJsonReader(bytes, XmlDictionaryReaderQuotas.Max))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(xReader);
                        return doc;
                    }
                }
            }
            catch { return null; }
        }
    }
}