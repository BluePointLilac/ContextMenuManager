using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BulePointLilac.Methods
{
    public static class ResourceString
    {
        /// <summary> 获取格式为"@filename,-strID"直接字符串 </summary>
        /// <param name="resStr">要转换的字符串</param>
        /// <returns>resStr为Null时返回值为string.Empty; resStr首字符为@但解析失败时返回string.Empty</returns>
        public static string GetDirectString(string resStr)
        {
            StringBuilder outBuff = new StringBuilder(1024);
            SHLoadIndirectString(resStr, outBuff, 1024, IntPtr.Zero);
            return outBuff.ToString();
        }

        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);
    }
}