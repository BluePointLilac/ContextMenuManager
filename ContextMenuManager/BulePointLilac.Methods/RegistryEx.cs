using Microsoft.Win32;
using System;
using System.Security.AccessControl;

namespace BulePointLilac.Methods
{

    public static class RegistryEx
    {

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
        /// <param name="throwOnMissingKey">找不到注册表项时是否抛出异常</param>
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
            int index = regPath.IndexOf('\\');
            subRegPath = regPath.Substring(index + 1);
            string rootPath = regPath.Substring(0, index).ToUpper();
            switch(rootPath)
            {
                case "HKEY_CLASSES_ROOT":
                    root = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    root = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    root = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    root = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    root = Registry.CurrentConfig;
                    break;
                default:
                    throw new ArgumentNullException("非法的根项!");
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
                else return root.OpenSubKey(keyPath, writable);
            }
        }

        public static RegistryKey GetRegistryKey(string regPath, RegistryKeyPermissionCheck check, RegistryRights rights)
        {
            GetRootAndSubRegPath(regPath, out RegistryKey root, out string keyPath);
            using(root) return root.OpenSubKey(keyPath, check, rights);
        }
    }

    public static class RegistryKeyExtension
    {
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

        public static RegistryKey CreateSubKey(this RegistryKey key, string subKeyName, bool writable)
        {
            key.CreateSubKey(subKeyName).Close();
            RegTrustedInstaller.TakeRegTreeOwnerShip($@"{key.Name}\{subKeyName}");
            return key.OpenSubKey(subKeyName, writable);
        }
    }
}