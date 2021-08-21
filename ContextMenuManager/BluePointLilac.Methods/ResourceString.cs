using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BluePointLilac.Methods
{
    public static class ResourceString
    {
        //MSDN文档: https://docs.microsoft.com/windows/win32/api/shlwapi/nf-shlwapi-shloadindirectstring
        //提取.pri文件资源: https://docs.microsoft.com/windows/uwp/app-resources/makepri-exe-command-options
        //.pri转储.xml资源列表: MakePri.exe dump /if [priPath] /of [xmlPath]

        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);

        /// <summary>获取格式为"@[filename],-[strID]"或"@{[packageName]?ms-resource://[resPath]}"的直接字符串</summary>
        /// <param name="resStr">要转换的字符串</param>
        /// <returns>resStr为Null时返回值为string.Empty; resStr首字符为@但解析失败时返回string.Empty</returns>
        /// <remarks>[fileName]:文件路径; [strID]:字符串资源索引; [packageName]:UWP带版本号包名; [resPath]:pri资源路径</remarks>
        public static string GetDirectString(string resStr)
        {
            StringBuilder outBuff = new StringBuilder(1024);
            SHLoadIndirectString(resStr, outBuff, 1024, IntPtr.Zero);
            return outBuff.ToString();
        }

        public static readonly string OK = GetDirectString("@shell32.dll,-9752");
        public static readonly string Cancel = GetDirectString("@shell32.dll,-9751");
    }
}