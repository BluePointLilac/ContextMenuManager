using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BulePointLilac.Methods
{
    public class ObjectPath
    {
        /// <summary>路径类型</summary>
        public enum PathType { File, Directory, Registry }

        private static readonly List<string> EnvironmentDirectorys = new List<string> {
                @"%SystemRoot%\System32",
                @"%SystemRoot%",
                @"%SystemRoot%\System32\WindowsPowerShell\v1.0"
            };

        private const string RegAppPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
        private const string RegLastPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";

        private static readonly char[] IllegalChars = { '/', '*', '?', '\"', '<', '>', '|' };
        private static readonly List<string> IgnoreCommandParts = new List<string> { "", "%1", "%v" };

        /// <summary>根据文件名获取完整的文件路径</summary>
        /// <remarks>fileName为Win+R、注册表等可直接使用的文件名</remarks>
        /// <param name="fileName">文件名</param>
        /// <returns>成功提取返回现有文件路径，否则返回值为null</returns>
        public static bool GetFullFilePath(string fileName, out string fullPath)
        {
            fullPath = null;
            if(File.Exists(fileName))
            {
                fullPath = fileName;
                return true;
            }
            foreach(string name in new[] { fileName, $"{fileName}.exe" })
            {
                foreach(string dir in EnvironmentDirectorys)
                {
                    fullPath = Environment.ExpandEnvironmentVariables($@"{dir}\{name}");
                    if(File.Exists(fullPath)) return true;
                }

                fullPath = Registry.GetValue($@"{RegAppPath}\{name}", "", null)?.ToString();
                if(File.Exists(fullPath)) return true;
            }
            return false;
        }

        /// <summary>从包含现有文件路径的命令语句中提取文件路径</summary>
        /// <param name="command">命令语句</param>
        /// <returns>成功提取返回现有文件路径，否则返回值为null</returns>
        public static string ExtractFilePath(string command)
        {
            if(command.IsNullOrWhiteSpace()) return null;
            command = Environment.ExpandEnvironmentVariables(command).Replace(@"\\", @"\");
            if(File.Exists(command)) return command;

            string[] strs = Array.FindAll(command.Split(IllegalChars), str
                => IgnoreCommandParts.Any(part => !part.Equals(str.Trim()))).Reverse().ToArray();
            foreach(string str in strs)
            {
                int index = str.Length;
                do
                {
                    string path1 = str.Substring(0, index);
                    List<string> paths = new List<string> { path1 };
                    if(path1.Contains(",")) paths.Add(path1.Substring(0, path1.LastIndexOf(',')));
                    foreach(string path in paths)
                    {
                        if(GetFullFilePath(path, out string fullPath)) return fullPath;
                    }
                    index = path1.LastIndexOf(' ');
                } while(index != -1);
            }
            return null;
        }


        /// <summary>移除文件或文件夹名称中的非法字符</summary>
        /// <param name="fileName">文件或文件夹名称</param>
        /// <returns>返回移除非法字符后的文件或文件夹名称</returns>
        public static string RemoveIllegalChars(string fileName)
        {
            Array.ForEach(IllegalChars, c => fileName = fileName.Replace(c.ToString(), ""));
            return fileName.Replace("\\", "").Replace(":", "");
        }


        /// <summary>判断文件或文件夹或注册表项是否存在</summary>
        /// <param name="path">文件或文件夹或注册表项路径</param>
        /// <param name="type">路径类型</param>
        /// <returns>目标路径存在返回true，否则返回false</returns>
        public static bool ObjectPathExist(string path, PathType type)
        {
            switch(type)
            {
                case PathType.File:
                    return File.Exists(path);
                case PathType.Directory:
                    return Directory.Exists(path);
                case PathType.Registry:
                    return RegistryEx.GetRegistryKey(path) != null;
                default:
                    return false;
            }
        }

        /// <summary>获取带序号的新路径</summary>
        /// <param name="oldPath">目标路径</param>
        /// <param name="type">路径类型</param>
        /// <returns>如果目标路径不存在则返回目标路径，否则返回带序号的新路径</returns>
        public static string GetNewPathWithIndex(string oldPath, PathType type)
        {
            string newPath;
            string dirPath = type == PathType.Registry ? RegistryEx.GetParentPath(oldPath) : Path.GetDirectoryName(oldPath);
            string name = type == PathType.Registry ? RegistryEx.GetKeyName(oldPath) : Path.GetFileNameWithoutExtension(oldPath);
            string extension = type == PathType.Registry ? "" : Path.GetExtension(oldPath);

            int index = 0;
            do
            {
                newPath = $@"{dirPath}\{name}";
                if(index > 0) newPath += index;
                newPath += extension;
                index++;
            } while(ObjectPathExist(newPath, type));
            return newPath;
        }

        public static void ShowPath(string path, PathType type)
        {
            switch(type)
            {
                case PathType.Directory:
                    Process.Start(path);
                    break;
                case PathType.File:
                    Process.Start("explorer.exe", $" /select,{path}");
                    break;
                case PathType.Registry:
                    Registry.SetValue(RegLastPath, "LastKey", path);
                    Process.Start("regedit.exe", "-m");
                    break;
            }
        }
    }
}