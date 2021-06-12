using System.IO;
using System.Text;

namespace BluePointLilac.Methods
{
    /* 获取文本文件编码类型
     * 代码参考：https://www.cnblogs.com/guyun/p/4262587.html (Napoléon)

     * 各种带BOM的编码BOM值
     * UTF-7 : 2B 2F 76
     * UTF-8 : EF BB BF
     * UTF-16LE : FF FE
     * UTF-16BE : FE FF
     * UTF-32LE : FF FE 00 00
     * UTF-32BE : 00 00 FE FF
     */
    public static class EncodingType
    {
        /// <summary>获取给定的文件的编码类型</summary> 
        /// <param name=“filePath“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static Encoding GetType(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            if(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) return Encoding.UTF8;//UTF-8
            else if(bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00) return Encoding.UTF32;//UTF-32LE
            else if(bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE) return Encoding.Unicode; //UTF-16LE
            else if(bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF) return Encoding.BigEndianUnicode; //UTF-16BE
            else if(bytes.Length >= 3 && bytes[0] == 0x2B && bytes[1] == 0x2F && bytes[2] == 0x76) return Encoding.UTF7; //UTF-7
            else if(bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF) return new UTF32Encoding(true, true);//UTF-32BE
            else if(IsUTF8Bytes(bytes)) return Encoding.UTF8; //不带BOM的UTF-8
            return Encoding.Default;
        }

        /// <summary>判断是否是不带 BOM 的 UTF8 格式</summary> 
        /// <param name=“bytes“></param> 
        private static bool IsUTF8Bytes(byte[] bytes)
        {
            int count = 1; //计算当前正分析的字符应还有的字节数 
            for(int i = 0; i < bytes.Length; i++)
            {
                byte curByte = bytes[i];//当前分析的字节. 
                if(count == 1)
                {
                    if(curByte >= 0x80)
                    {
                        //判断当前 
                        while(((curByte <<= 1) & 0x80) != 0) count++;
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if(count == 1 || count > 6) return false;
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if((curByte & 0xC0) != 0x80) return false;
                    else count--;
                }
            }
            //if(count > 1) throw new Exception("非预期的byte格式");
            return true;
        }
    }
}