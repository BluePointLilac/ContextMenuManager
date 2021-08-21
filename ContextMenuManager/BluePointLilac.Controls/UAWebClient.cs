using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace BluePointLilac.Controls
{
    public sealed class UAWebClient : WebClient
    {
        public UAWebClient()
        {
            //此类主要为了解决访问Github的一些问题
            //请求被中止: 未能创建 SSL/TLS 安全通道; 基础连接已经关闭: 发送时发生错误，一般添加TLS12即可
            //TLS12------0xc00，TLS11------0x300，TLS------0xc0，SSL------0x30;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc00 | 0x300 | 0xc0 | 0x30);
            //网络传输默认文本编码 UTF-8
            this.Encoding = Encoding.UTF8;
            //远程服务器返回错误: (403) 已禁止
            //浏览器 F12 console 输入 console.log(navigator.userAgent); 获取 User Agent
            this.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66");
        }

        /// <summary>获取网页文本</summary>
        public string GetWebString(string url)
        {
            try
            {
                string str = this.DownloadString(url);
                str = str?.Replace("\n", Environment.NewLine);//换行符转换
                return str;
            }
            catch { return null; }
        }

        /// <summary>将网络文本写入本地文件</summary>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="fileUrl">网络文件Raw路径</param>
        public bool WebStringToFile(string filePath, string fileUrl)
        {
            string contents = GetWebString(fileUrl);
            bool flag = contents != null;
            if(flag) File.WriteAllText(filePath, contents, Encoding.Unicode);
            return flag;
        }

        /// <summary>获取网页Json文本并加工为Xml</summary>
        public XmlDocument GetWebJsonToXml(string url)
        {
            try
            {
                byte[] bytes = this.DownloadData(url);
                using(XmlReader xReader = JsonReaderWriterFactory.CreateJsonReader(bytes, XmlDictionaryReaderQuotas.Max))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xReader);
                    return doc;
                }
            }
            catch { return null; }
        }
    }
}