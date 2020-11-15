using System;

namespace BulePointLilac.Methods
{
    //为兼容.Net Framework 3.5，无法引用Microsoft.CSharp.dll（中的Guid.TryParse）写了这个扩展方法
    public static class GuidEx
    {
        public static bool TryParse(string str, out Guid guid)
        {
            try
            {
                guid = new Guid(str);
                return true;
            }
            catch(Exception) {
                guid = Guid.Empty;
                return false;
            }
        }
    }
}