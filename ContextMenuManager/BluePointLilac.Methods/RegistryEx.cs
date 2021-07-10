using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

namespace BluePointLilac.Methods
{
    public static class RegistryEx
    {
        public const string CLASSESROOT = "HKEY_CLASSES_ROOT";
        public const string CURRENTUSER = "HKEY_CURRENT_USER";
        public const string LOCALMACHINE = "HKEY_LOCAL_MACHINE";
        public const string CURRENTCONFIG = "HKEY_CURRENT_CONFIG";
        public const string USERS = "HKEY_USERS";

        public static void CopyTo(this RegistryKey srcKey, RegistryKey dstKey)
        {
            foreach(string name in srcKey.GetValueNames())
            {
                dstKey.SetValue(name, srcKey.GetValue(name), srcKey.GetValueKind(name));
            }
            foreach(string name in srcKey.GetSubKeyNames())
            {
                using(RegistryKey srcSubKey = srcKey.OpenSubKey(name))
                using(RegistryKey dstSubKey = dstKey.CreateSubKey(name, true))
                    srcSubKey.CopyTo(dstSubKey);
            }
        }

        public static void CopyTo(string srcPath, string dstPath)
        {
            using(RegistryKey srcKey = GetRegistryKey(srcPath))
            using(RegistryKey dstKey = GetRegistryKey(dstPath, true, true))
            {
                CopyTo(srcKey, dstKey);
            }
        }

        public static void MoveTo(this RegistryKey srcKey, RegistryKey dstKey)
        {
            CopyTo(srcKey, dstKey);
            DeleteKeyTree(srcKey.Name, true);
        }

        public static void MoveTo(string srcPath, string dstPath)
        {
            CopyTo(srcPath, dstPath);
            DeleteKeyTree(srcPath, true);
        }

        public static RegistryKey CreateSubKey(this RegistryKey key, string subKeyName, bool writable)
        {
            using(key.CreateSubKey(subKeyName))
                return key.OpenSubKey(subKeyName, writable);
        }

        /// <summary>获取指定路径注册表项的上一级路径</summary>
        public static string GetParentPath(string regPath) => regPath.Substring(0, regPath.LastIndexOf('\\'));

        /// <summary>获取指定路径注册表项的项名</summary>
        public static string GetKeyName(string regPath) => regPath.Substring(regPath.LastIndexOf('\\') + 1);

        /// <summary>获取指定路径注册表项的根项项名</summary>
        public static string GetRootName(string regPath) => regPath.Substring(0, regPath.IndexOf('\\'));

        /// <summary>获取不包含根项部分的注册表路径</summary>
        public static string GetPathWithoutRoot(string regPath) => regPath.Substring(regPath.IndexOf('\\') + 1);

        /// <summary>删除指定路径的注册表项的指定名称的键值</summary>
        /// <param name="regPath">注册表项路径</param>
        /// <param name="valueName">要删除的键值名称</param>
        /// <param name="throwOnMissingValue">找不到键值时是否抛出异常</param>
        public static void DeleteValue(string regPath, string valueName, bool throwOnMissingValue = false)
        {
            GetRegistryKey(regPath, true)?.DeleteValue(valueName, throwOnMissingValue);
        }

        /// <summary>递归删除指定注册表项及所有子项</summary>
        /// <param name="regPath">注册表路径</param>
        /// <param name="throwOnMissingKey">找不到注册表项或者没有操作权限时是否抛出异常</param>
        public static void DeleteKeyTree(string regPath, bool throwOnMissingKey = false)
        {
            string dirPath = GetParentPath(regPath);
            string keyName = GetKeyName(regPath);
            try
            {
                GetRegistryKey(dirPath, true)?.DeleteSubKeyTree(keyName);
            }
            catch(Exception)
            {
                if(throwOnMissingKey) throw;
            }
        }

        /// <summary>获取指定注册表路径的根项RegistryKey和不包含根项部分的注册表路径</summary>
        /// <param name="regPath">注册表路径</param>
        /// <param name="root">成功解析返回一个RegistryKey，否则抛出异常</param>
        /// <param name="subRegPath">不包含根项的注册表路径</param>
        public static void GetRootAndSubRegPath(string regPath, out RegistryKey root, out string subRegPath)
        {
            string rootPath;
            int index = regPath.IndexOf('\\');
            if(index > 0)
            {
                rootPath = regPath.Substring(0, index).ToUpper();
                subRegPath = regPath.Substring(index + 1);
            }
            else
            {
                rootPath = regPath;
                subRegPath = string.Empty;
            }
            switch(rootPath)
            {
                case CLASSESROOT:
                    root = Registry.ClassesRoot;
                    break;
                case CURRENTUSER:
                    root = Registry.CurrentUser;
                    break;
                case LOCALMACHINE:
                    root = Registry.LocalMachine;
                    break;
                case USERS:
                    root = Registry.Users;
                    break;
                case CURRENTCONFIG:
                    root = Registry.CurrentConfig;
                    break;
                default:
                    throw new ArgumentNullException("The root key resolution failed!");
            }
        }

        /// <summary>获取指定注册表项路径的RegistryKey</summary>
        /// <param name="regPath">注册表项路径</param>
        /// <param name="writable">写入访问权限</param>
        /// <param name="create">是否创建新项</param>
        public static RegistryKey GetRegistryKey(string regPath, bool writable = false, bool create = false)
        {
            GetRootAndSubRegPath(regPath, out RegistryKey root, out string keyPath);
            using(root)
            {
                if(create) return root.CreateSubKey(keyPath, writable);
                else
                {
                    RegTrustedInstaller.TakeRegTreeOwnerShip(keyPath);
                    return root.OpenSubKey(keyPath, writable);
                }
            }
        }

        public static RegistryKey GetRegistryKey(string regPath, RegistryKeyPermissionCheck check, RegistryRights rights)
        {
            GetRootAndSubRegPath(regPath, out RegistryKey root, out string keyPath);
            using(root) return root.OpenSubKey(keyPath, check, rights);
        }
    }
}