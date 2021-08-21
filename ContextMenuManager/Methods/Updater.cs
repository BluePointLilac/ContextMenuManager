using BluePointLilac.Controls;
using BluePointLilac.Methods;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ContextMenuManager.Methods
{
    sealed class Updater
    {
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
            using(UAWebClient client = new UAWebClient())
            {
                string url = AppConfig.RequestUseGithub ? AppConfig.GithubLatestApi : AppConfig.GiteeLatestApi;
                XmlDocument doc = client.GetWebJsonToXml(url);
                if(doc == null)
                {
                    if(isManual)
                    {
                        if(AppMessageBox.Show(AppString.Message.WebDataReadFailed + "\r\n"
                            + AppString.Message.OpenWebUrl, MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                        url = AppConfig.RequestUseGithub ? AppConfig.GithubLatest : AppConfig.GiteeReleases;
                        ExternalProgram.OpenWebUrl(url);
                    }
                    return;
                }
                XmlNode root = doc.FirstChild;
                XmlNode tagNameXN = root.SelectSingleNode("tag_name");
                Version webVer = new Version(tagNameXN.InnerText);
                Version appVer = new Version(Application.ProductVersion);
#if DEBUG
            appVer = new Version(0, 0, 0, 0);//测试用
#endif
                if(appVer >= webVer)
                {
                    if(isManual) AppMessageBox.Show(AppString.Message.VersionIsLatest,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XmlNode bodyXN = root.SelectSingleNode("body");
                    string info = AppString.Message.UpdateInfo.Replace("%v1", appVer.ToString()).Replace("%v2", webVer.ToString());
                    info += "\r\n\r\n" + MachinedInfo(bodyXN.InnerText);
                    if(AppMessageBox.Show(info, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string netVer = Environment.Version > new Version(4, 0) ? "4.0" : "3.5";
                        XmlNode assetsXN = root.SelectSingleNode("assets");
                        foreach(XmlNode itemXN in assetsXN.SelectNodes("item"))
                        {
                            XmlNode nameXN = itemXN.SelectSingleNode("name");
                            if(nameXN != null && nameXN.InnerText.Contains(netVer))
                            {
                                XmlNode urlXN = itemXN.SelectSingleNode("browser_download_url");
                                using(DownloadDialog dlg = new DownloadDialog())
                                {
                                    dlg.Url = urlXN?.InnerText;
                                    dlg.FilePath = $@"{AppConfig.AppDataDir}\{webVer}.exe";
                                    dlg.Text = AppString.General.AppName;
                                    if(dlg.ShowDialog() == DialogResult.OK)
                                    {
                                        AppMessageBox.Show(AppString.Message.UpdateSucceeded,
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        SingleInstance.Restart(null, dlg.FilePath);
                                    }
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
            string dirUrl;
            string[] filePaths;
            void WriteFiles(string dirName, out string succeeded, out string failed)
            {
                succeeded = failed = "";
                foreach(string filePath in filePaths)
                {
                    using(UAWebClient client = new UAWebClient())
                    {
                        string fileUrl = $"{dirUrl}/{Path.GetFileName(filePath)}";
                        var func = new Func<string, string, bool>(client.WebStringToFile);
                        bool flag = func.EndInvoke(func.BeginInvoke(filePath, fileUrl, null, null));
                        string item = "\r\n ● " + Path.GetFileName(filePath);
                        if(flag) succeeded += item;
                        else failed += item;
                    }
                }
                dirName = "\r\n\r\n" + dirName + ":";
                if(succeeded != "") succeeded = dirName + succeeded;
                if(failed != "") failed = dirName + failed;
            }

            dirUrl = AppConfig.RequestUseGithub ? AppConfig.GithubTexts : AppConfig.GiteeTexts;
            filePaths = new[]
            {
                AppConfig.WebGuidInfosDic, AppConfig.WebEnhanceMenusDic,
                AppConfig.WebDetailedEditDic, AppConfig.WebUwpModeItemsDic
            };
            WriteFiles("Dictionaries", out string succeeded1, out string failed1);

            dirUrl = AppConfig.RequestUseGithub ? AppConfig.GithubLangsRawDir : AppConfig.GiteeLangsRawDir;
            filePaths = Directory.GetFiles(AppConfig.LangsDir, "*.ini");
            WriteFiles("Languages", out string succeeded2, out string failed2);

            if(isManual)
            {
                string failed = failed1 + failed2;
                string succeeded = succeeded1 + succeeded2;
                if(failed != "") AppMessageBox.Show(AppString.Message.WebDataReadFailed + failed);
                if(succeeded != "") AppMessageBox.Show(AppString.Message.DicUpdateSucceeded + succeeded,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                str += line + "\r\n";
            }
            return str;
        }
    }
}