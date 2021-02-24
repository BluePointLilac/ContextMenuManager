using BluePointLilac.Methods;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ContextMenuManager
{
    sealed class Updater
    {
        const string UpdateUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Update.ini";
        const string GuidInfosDicUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts/GuidInfosDic.ini";
        const string ThirdRulesDicUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts/ThirdRulesDic.xml";
        const string EnhanceMenusDicUrl = "https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Texts/EnhanceMenusDic.xml";

        public static void PeriodicUpdate()
        {
            Version appVersion = new Version(Application.ProductVersion);
            //如果上次检测更新时间为一个月以前就进行更新操作
            bool flag1 = AppConfig.LastCheckUpdateTime.AddMonths(1).CompareTo(DateTime.Today) < 0;
            //如果配置文件中的版本号低于程序版本号也进行更新操作
            bool flag2 = appVersion.CompareTo(AppConfig.Version) > 0;
            if(flag1 || flag2)
            {
                CheckUpdate();
                AppConfig.LastCheckUpdateTime = DateTime.Today;
                AppConfig.Version = appVersion;
            }
        }

        public static bool CheckUpdate()
        {
            UpdateText(AppConfig.WebGuidInfosDic, GuidInfosDicUrl);
            UpdateText(AppConfig.WebThirdRulesDic, ThirdRulesDicUrl);
            UpdateText(AppConfig.WebEnhanceMenusDic, EnhanceMenusDicUrl);
            try { return UpdateApp(); } catch { return false; }
        }

        private static bool UpdateApp()
        {
            IniReader reader = new IniReader(new StringBuilder(GetWebString(UpdateUrl)));
            Version version1;
            try
            {
                version1 = new Version(reader.GetValue("Update", "Version"));
            }
            catch
            {
                version1 = new Version(Application.ProductVersion);
            }
            Version version2 = new Version(Application.ProductVersion);
            if(version1.CompareTo(version2) > 0)
            {
                string info = reader.GetValue("Update", "Info").Replace("\\n", "\n");
                if(MessageBoxEx.Show($"{AppString.MessageBox.UpdateApp}{version1}\n{info}",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string url = reader.GetValue("Update", "Url");
                    ExternalProgram.OpenUrl(url);
                    return true;
                }
            }
            return false;
        }

        private static void UpdateText(string filePath, string url)
        {
            string contents = GetWebString(url);
            if(!contents.IsNullOrWhiteSpace())
                File.WriteAllText(filePath, contents.Replace("\n", "\r\n"), Encoding.Unicode);
        }

        private static string GetWebString(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using(StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch { return null; }
        }
    }
}