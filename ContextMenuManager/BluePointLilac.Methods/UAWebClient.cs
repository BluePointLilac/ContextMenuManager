using System.Text;
using System.Net;

namespace BluePointLilac.Methods
{
    /// <summary>此类主要为了解决访问Github的一些问题</summary>
    public class UAWebClient : WebClient
    {
        public UAWebClient()
        {
            this.Encoding = Encoding.UTF8;
            this.Headers.Add("User-Agent", "UserAgent");//远程服务器返回错误: (403) 已禁止
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;//TLS12, 请求被中止: 未能创建 SSL/TLS 安全通道
        }
    }
}