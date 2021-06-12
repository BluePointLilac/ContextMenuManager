using System.Net;
using System.Text;

namespace BluePointLilac.Methods
{
    /// <summary>此类主要为了解决访问Github的一些问题</summary>
    public class UAWebClient : WebClient
    {
        static UAWebClient()
        {
            //请求被中止: 未能创建 SSL/TLS 安全通道; 基础连接已经关闭: 发送时发生错误
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;//SecurityProtocolType.TLS12
        }

        public UAWebClient()
        {
            //网络传输默认文本编码 UTF-8
            this.Encoding = Encoding.UTF8;
            //远程服务器返回错误: (403) 已禁止
            //浏览器 F12 console 输入 console.log(navigator.userAgent); 获取 User Agent
            this.Headers.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.66");
        }
    }
}