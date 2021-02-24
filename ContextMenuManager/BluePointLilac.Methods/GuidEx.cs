using System;
using System.Text.RegularExpressions;

namespace BluePointLilac.Methods
{
    //为兼容.Net Framework 3.5，无法引用Microsoft.CSharp.dll（中的Guid.TryParse）写了这个扩展方法
    public static class GuidEx
    {
        public static bool TryParse(string str, out Guid guid)
        {
            if(IsGuid(str))
            {
                guid = new Guid(str);
                return true;
            }
            else
            {
                guid = Guid.Empty;
                return false;
            }
        }

        private static readonly Regex GuidRegex = new Regex(@"[A-F0-9]{8}(\-[A-F0-9]{4}){3}\-[A-F0-9]{12}", RegexOptions.IgnoreCase);

        public static bool IsGuid(string str)
        {
            if(string.IsNullOrEmpty(str)) return false;
            if(str.Length == 38 && str.StartsWith("{") && str.EndsWith("}") && GuidRegex.IsMatch(str)) return true;
            if(str.Length == 36 && GuidRegex.IsMatch(str)) return true;
            return false;
        }
    }
}