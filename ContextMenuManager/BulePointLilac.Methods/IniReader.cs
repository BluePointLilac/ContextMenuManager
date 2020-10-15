using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BulePointLilac.Methods
{
    public class IniReader
    {
        public IniReader(StringBuilder sb)
        {
            if(string.IsNullOrWhiteSpace(sb.ToString())) return;
            List<string> lines = sb.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries).ToList();//拆分为行
            lines.ForEach(line => line.Trim());
            ReadLines(lines);
        }

        public IniReader(string filePath)
        {
            if(!File.Exists(filePath)) return;
            List<string> lines = new List<string>();
            using(StreamReader reader = new StreamReader(filePath, EncodingType.GetType(filePath)))
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if(line != string.Empty) lines.Add(line);
                }
            ReadLines(lines);
        }

        protected readonly Dictionary<string, Dictionary<string, string>> rootDic
            = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private void ReadLines(List<string> lines)
        {
            lines.RemoveAll(
                line => line.StartsWith(";")//移除注释
                || (!line.StartsWith("[") && !line.Contains("=")));//移除非section行且非key行

            if(lines.Count == 0) return;

            List<int> indexs = new List<int> { 0 };
            for(int i = 1; i < lines.Count; i++)
            {
                if(lines[i].StartsWith("[")) indexs.Add(i);//获取section行号
            }
            indexs.Add(lines.Count);

            for(int i = 0; i < indexs.Count - 1; i++)
            {
                string section = lines[indexs[i]];
                int m = section.IndexOf(']') - 1;
                if(m < 0) continue;
                section = section.Substring(1, m);
                if(rootDic.ContainsKey(section)) continue;
                var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                rootDic.Add(section, keyValues);

                for(int j = indexs[i] + 1; j < indexs[i + 1]; j++)
                {
                    int k = lines[j].IndexOf('=');
                    string key = lines[j].Substring(0, k).TrimEnd();
                    string value = lines[j].Substring(k + 1).TrimStart();
                    keyValues.Add(key, value);
                }
            }
        }

        public string GetValue(string section, string key)
        {
            if(rootDic.TryGetValue(section, out Dictionary<string, string> sectionDic))
                if(sectionDic.TryGetValue(key, out string value))
                    return value;
            return string.Empty;
        }

        public bool TryGetValue(string section, string key, out string value)
        {
            value = GetValue(section, key);
            return value != string.Empty;
        }
    }
}