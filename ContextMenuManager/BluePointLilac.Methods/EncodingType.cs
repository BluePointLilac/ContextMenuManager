using System;
using System.IO;
using System.Text;

namespace BluePointLilac.Methods
{
    /// 获取文本文件编码类型
    /// 代码主要为转载，仅做简单改动
    /// 代码作者：Napoléon
    /// 代码原文：https://www.cnblogs.com/guyun/p/4262587.html
    public static class EncodingType
    {
        /// <summary>给定文件的路径，读取文件的二进制数据，判断文件的编码类型</summary> 
        /// <param name=“filePath“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static Encoding GetType(string filePath)
        {
            using(FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                return GetType(fs);
        }

        /// <summary> 通过给定的文件流，判断文件的编码类型</summary>
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static Encoding GetType(FileStream fs)
        {
            byte[] ss;
            int.TryParse(fs.Length.ToString(), out int i);
            using(BinaryReader reader = new BinaryReader(fs, Encoding.Default))
                ss = reader.ReadBytes(i);
            if(IsUTF8Bytes(ss)) return Encoding.UTF8;
            if(ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF) return Encoding.UTF8;//带BOM 
            if(ss[0] == 0xFE && ss[1] == 0xFF) return Encoding.BigEndianUnicode;     //UTF-16BE
            if(ss[0] == 0xFF && ss[1] == 0xFE) return Encoding.Unicode;              //UTF-16LE
            return Encoding.Default;
        }

        /// <summary>判断是否是不带 BOM 的 UTF8 格式</summary> 
        /// <param name=“data“></param> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int count = 1; //计算当前正分析的字符应还有的字节数 
            for(int i = 0; i < data.Length; i++)
            {
                byte curByte = data[i];//当前分析的字节. 
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
            if(count > 1) throw new Exception("非预期的byte格式");
            return true;
        }
    }
}