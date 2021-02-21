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

        public static bool IsGuid(string str)
        {
            if(string.IsNullOrEmpty(str)) return false;
            Regex guidRegEx = new Regex(@"[a-fA-F0-9]{8}(\-[a-fA-F0-9]{4}){3}\-[a-fA-F0-9]{12}");
            return guidRegEx.IsMatch(str);
        }
    }
}