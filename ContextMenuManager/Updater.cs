using BluePointLilac.Controls;
using BluePointLilac.Methods;
using ContextMenuManager.Controls;
using System;
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
        const string GithubLangsRawDir = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/languages";
        const string GithubShellNewApi = "https://api.github.com/repos/BluePointLilac/ContextMenuManager/contents/ContextMenuManager/Properties/Resources/ShellNew";
        const string GithubShellNewRawDir = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/ShellNew";
        const string GithubTexts = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/ContextMenuManager/Properties/Resources/Texts";
        const string GithubDonateRaw = "https://raw.githubusercontent.com/BluePointLilac/ContextMenuManager/master/Donate.md";
        const string GithubDonate = "https://github.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        const string GiteeReleases = "https://gitee.com/BluePointLilac/ContextMenuManager/releases";
        const string GiteeLatestApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/releases/latest";
        const string GiteeLangsApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/languages";
        const string GiteeLangsRawDir = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/languages";
        const string GiteeShellNewApi = "https://gitee.com/api/v5/repos/BluePointLilac/ContextMenuManager/contents/ContextMenuManager/Properties/Resources/ShellNew";
        const string GiteeShellNewRawDir = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/ShellNew";
        const string GiteeTexts = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts";
        const string GiteeDonateRaw = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Donate.md";
        const string GiteeDonate = "https://gitee.com/BluePointLilac/ContextMenuManager/blob/master/Donate.md";

        /// <summary>定期检查更新</summary>
        public static void PeriodicUpdate()
        {
            int day = AppConfig.UpdateFrequency;
            if(day == -1) return;//自动检测更新频率为-1则从不自动检查更新
            //如果上次检测更新时间加上时间间隔早于或等于今天以前就进行更新操作
            DateTime time = AppConfig.LastCheckUpdateTime.AddDays(day);
            //time = DateTime.Today;//测试用
            if(time <= DateTime.Today) new Action<bool>(Update).BeginInvoke(false, null, null);
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
                    if(MessageBoxEx.Show(AppString.Message.WebDataReadFailed + "\r\n"
                        + AppString.Message.OpenWebUrl, MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                    url = AppConfig.RequestUseGithub ? GithubLatest : GiteeReleases;
                    ExternalProgram.OpenWebUrl(url);
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
                if(isManual) MessageBoxEx.Show(AppString.Message.VersionIsLatest,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XmlElement bodyXE = (XmlElement)root.SelectSingleNode("body");
                string info = AppString.Message.UpdateInfo.Replace("%v1", appVer.ToString()).Replace("%v2", webVer.ToString());
                info += Environment.NewLine + Environment.NewLine + MachinedInfo(bodyXE.InnerText);
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
                                dlg.FilePath = $@"{AppConfig.AppDataDir}\{webVer}.exe";
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
            string error1 = null;
            string error2 = null;
            bool hasError1 = false;
            bool hasError2 = false;
            var func = new Func<string, string, bool>(WebStringToFile);
            string dirUrl = AppConfig.RequestUseGithub ? GithubTexts : GiteeTexts;
            string[] filePaths = new[]
            {
                AppConfig.WebGuidInfosDic, AppConfig.WebEnhanceMenusDic,
                AppConfig.WebThirdRulesDic, AppConfig.WebUwpModeItemsDic
            };
            foreach(string filePath in filePaths)
            {
                string fileUrl = $"{dirUrl}/{Path.GetFileName(filePath)}";
                bool flag = func.EndInvoke(func.BeginInvoke(filePath, fileUrl, null, null));
                if(!flag)
                {
                    hasError1 = true;
                    error1 += "\r\n ● " + Path.GetFileName(filePath);
                }
            }
            if(hasError1) error1 = "\r\n\r\nDictionaries:" + error1;

            dirUrl = AppConfig.RequestUseGithub ? GithubLangsRawDir : GiteeLangsRawDir;
            filePaths = Directory.GetFiles(AppConfig.LangsDir, "*.ini");
            foreach(string filePath in filePaths)
            {
                string fileUrl = $"{dirUrl}/{Path.GetFileName(filePath)}";
                bool flag = func.EndInvoke(func.BeginInvoke(filePath, fileUrl, null, null));
                if(!flag)
                {
                    hasError2 = true;
                    error2 += "\r\n ● " + Path.GetFileName(filePath);
                }
            }
            if(hasError2) error2 = "\r\n\r\nLanguages:" + error2;

            if(isManual)
            {
                if(hasError1 || hasError2) MessageBoxEx.Show(AppString.Message.WebDataReadFailed + error1 + error2);
                else MessageBoxEx.Show(AppString.Message.DicUpdateSucceeded, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>将网络文本写入本地文件</summary>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="dirUrl">网络文件Raw路径</param>
        private static bool WebStringToFile(string filePath, string fileUrl)
        {
            string contents = GetWebString(fileUrl);
            bool flag = contents != null;
            if(flag) File.WriteAllText(filePath, contents, Encoding.Unicode);
            return flag;
        }

        /// <summary>显示语言下载对话框</summary>
        /// <returns>返回值为是否下载了语言文件</returns>
        public static bool ShowLanguageDialog()
        {
            string apiUrl = AppConfig.RequestUseGithub ? GithubLangsApi : GiteeLangsApi;
            XmlDocument doc = GetWebJsonToXml(apiUrl);
            if(doc == null)
            {
                MessageBoxEx.Show(AppString.Message.WebDataReadFailed);
                return false;
            }
            XmlNodeList list = doc.FirstChild.ChildNodes;
            string[] langs = new string[list.Count];
            for(int i = 0; i < list.Count; i++)
            {
                XmlNode nameXN = list.Item(i).SelectSingleNode("name");
                langs[i] = Path.GetFileNameWithoutExtension(nameXN.InnerText);
            }
            using(SelectDialog dlg = new SelectDialog())
            {
                dlg.Items = langs;
                dlg.Title = AppString.Dialog.DownloadLanguages;
                string lang = CultureInfo.CurrentUICulture.Name;
                if(dlg.Items.Contains(lang)) dlg.Selected = lang;
                else dlg.SelectedIndex = 0;
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    string fileName = $"{dlg.Selected}.ini";
                    string filePath = $@"{AppConfig.LangsDir}\{fileName}";
                    string dirUrl = AppConfig.RequestUseGithub ? GithubLangsRawDir : GiteeLangsRawDir;
                    string fileUrl = $"{dirUrl}/{fileName}";
                    bool flag = WebStringToFile(filePath, fileUrl);
                    if(!flag)
                    {
                        if(MessageBoxEx.Show(AppString.Message.WebDataReadFailed + "\r\n ● " + fileName + "\r\n"
                            + AppString.Message.OpenWebUrl, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ExternalProgram.OpenWebUrl(fileUrl);
                        }
                    }
                    return flag;
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
                if(MessageBoxEx.Show(AppString.Message.WebDataReadFailed + "\r\n"
                    + AppString.Message.OpenWebUrl, MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                url = AppConfig.RequestUseGithub ? GithubDonate : GiteeDonate;
                ExternalProgram.OpenWebUrl(url);
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

        public static byte[] GetShellNewData(string extension)
        {
            string apiUrl = AppConfig.RequestUseGithub ? GithubShellNewApi : GiteeShellNewApi;
            XmlDocument doc = GetWebJsonToXml(apiUrl);
            if(doc == null) return null;
            foreach(XmlNode node in doc.FirstChild.ChildNodes)
            {
                XmlNode nameXN = node.SelectSingleNode("name");
                string str = Path.GetExtension(nameXN.InnerText);
                if(string.Equals(str, extension, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using(UAWebClient client = new UAWebClient())
                        {
                            string dirUrl = AppConfig.RequestUseGithub ? GithubShellNewRawDir : GiteeShellNewRawDir;
                            string fileUrl = $"{dirUrl}/{nameXN.InnerText}";
                            return client.DownloadData(fileUrl);
                        }
                    }
                    catch { return null; }
                }
            }
            return null;
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
                    string str = client.DownloadString(url);
                    str = str?.Replace("\n", Environment.NewLine);//换行符转换
                    return str;
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