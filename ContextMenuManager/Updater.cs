using BulePointLilac.Methods;
using System;
using System.Diagnostics;
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
            //如果上次检测更新时间为一个月以前就进行更新操作
            if(AppConfig.LastCheckUpdateTime.AddMonths(1).CompareTo(DateTime.Today) < 0)
            {
                CheckUpdate();
                AppConfig.LastCheckUpdateTime = DateTime.Today;
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
            IniReader reader = new IniReader(new StringBuilder(GetWebString(UpdateUrl).Replace("\\n", "\n")));
            Version version1 = new Version(reader.GetValue("Update", "Version"));
            Version version2 = new Version(Application.ProductVersion);
            if(version1.CompareTo(version2) > 0)
            {
                string info = reader.GetValue("Update", "Info");
                if(MessageBoxEx.Show($"{AppString.MessageBox.UpdateApp}{version1}\n{info}",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Process.Start(reader.GetValue("Update", "Url"));
                    return true;
                }
            }
            return false;
        }

        private static void UpdateText(string filePath, string url)
        {
            string contents = GetWebString(url);
            if(!contents.IsNullOrWhiteSpace())
                File.WriteAllText(filePath, contents, Encoding.UTF8);
        }

        private static string GetWebString(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch { return null; }
        }
    }
}