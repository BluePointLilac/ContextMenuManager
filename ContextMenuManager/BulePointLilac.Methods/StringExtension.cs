namespace BulePointLilac.Methods
{
    //为兼容.Net Framework 3.5，无法引用Microsoft.CSharp.dll（中的string.IsNullOrWhiteSpace）写了这个扩展方法
    public static class StringExtension
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            if(string.IsNullOrEmpty(str)) return true;
            return str.Trim().Length == 0;
        }
    }
}