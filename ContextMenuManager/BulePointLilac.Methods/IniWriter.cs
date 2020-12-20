using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BulePointLilac.Methods
{
    public sealed class IniWriter
    {
        public IniWriter() { }

        public IniWriter(string filePath)
        {
            this.FilePath = filePath;
            if(File.Exists(FilePath)) this.Encoding = EncodingType.GetType(FilePath);
        }

        public string FilePath { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool DeleteFileWhenEmpty { get; set; }

        private List<string> GetLines()
        {
            List<string> lines = new List<string>();
            if(!File.Exists(FilePath)) return lines;
            using(StreamReader reader = new StreamReader(FilePath, Encoding))
            {
                while(!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine().Trim());
                }
            }
            return lines;
        }

        /// <param name="isGetValue">是否是获取value值</param>
        private void SetValue(string section, string key, ref string value, bool isGetValue)
        {
            if(section == null) return;
            List<string> lines = GetLines();
            string sectionLine = $"[{section}]";
            string keyLine = $"{key}={value}";
            int sectionRow = -1, keyRow = -1;//先假设不存在目标section和目标key
            int nextSectionRow = -1;//下一个section的行数
            for(int i = 0; i < lines.Count; i++)
            {
                if(lines[i].StartsWith(sectionLine, StringComparison.OrdinalIgnoreCase))
                {
                    sectionRow = i; break;//得到目标section所在行
                }
            }
            if(sectionRow >= 0)//如果目标section存在
            {
                for(int i = sectionRow + 1; i < lines.Count; i++)
                {
                    if(lines[i].StartsWith(";") || lines[i].StartsWith("#"))
                    {
                        continue;//跳过注释
                    }
                    if(lines[i].StartsWith("["))
                    {
                        nextSectionRow = i; break;//读取到下一个section
                    }
                    if(key != null && keyRow == -1)
                    {
                        int index = lines[i].IndexOf('=');
                        if(index < 0) continue;
                        string str = lines[i].Substring(0, index).TrimEnd();
                        if(str.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            if(isGetValue)//如果是获取Value值，直接返回
                            {
                                value = lines[i].Substring(index + 1).Trim();
                                return;
                            }
                            keyRow = i; continue;//得到目标key行
                        }
                    }
                }
            }

            if(sectionRow == -1)
            {
                if(key != null && value != null)
                {
                    if(lines.Count > 0) lines.Add(string.Empty);//添加空行
                    //目标section不存在则添加到最后
                    lines.Add(sectionLine);
                    lines.Add(keyLine);
                }
            }
            else
            {
                if(keyRow == -1)
                {
                    if(key != null)
                    {
                        //存在下一个section时插入到其上方
                        if(nextSectionRow != -1)
                        {
                            //目标section存在但目标key不存在
                            keyRow = nextSectionRow;
                            lines.Insert(keyRow, keyLine);
                        }
                        else
                        {
                            //不存在下一个section则添加到最后
                            lines.Add(keyLine);
                        }
                    }
                    else
                    {
                        //key为null则删除整个section
                        int count;
                        if(nextSectionRow == -1) count = lines.Count - sectionRow;
                        else count = nextSectionRow - sectionRow;
                        lines.RemoveRange(sectionRow, count);
                    }
                }
                else
                {
                    if(value != null)
                    {
                        //目标section和目标key都存在
                        lines[keyRow] = keyLine;
                    }
                    else
                    {
                        //赋值为null则删除key
                        lines.RemoveAt(keyRow);
                    }
                }
            }

            if(DeleteFileWhenEmpty && lines.Count == 0 && File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllLines(FilePath, lines.ToArray(), Encoding);
            }
        }

        public void SetValue(string section, string key, string value)
        {
            SetValue(section, key, ref value, false);
        }

        public void DeleteKey(string section, string key)
        {
            SetValue(section, key, null);
        }

        public void DeleteSection(string section)
        {
            SetValue(section, null, null);
        }

        /// <summary>一次读取只获取一个值，用此方法比IniReader.GetValue要快</summary>
        public string GetValue(string section, string key)
        {
            string value = string.Empty;
            SetValue(section, key, ref value, true);
            return value;
        }
    }
}