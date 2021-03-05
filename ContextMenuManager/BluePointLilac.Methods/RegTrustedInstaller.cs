using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace BluePointLilac.Methods
{
    /// 获取TrustedInstaller权限注册表项的所有权
    /// 代码主要为转载，仅做简单改动
    /// 代码作者：JPBlanc
    /// 代码原文：https://gist.github.com/JPBlanc/ca0e4f1830e4ca18a526#file-write_a_registry_own_by_trustedinstaller-cs
    public class RegTrustedInstaller
    {
        static class NativeMethod
        {
            public const string TakeOwnership = "SeTakeOwnershipPrivilege";
            public const string Restore = "SeRestorePrivilege";

            [StructLayout(LayoutKind.Sequential)]
            public struct LUID
            {
                public int lowPart;
                public int highPart;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct LUID_AND_ATTRIBUTES
            {
                public LUID Luid;
                public int Attributes;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct TOKEN_PRIVILEGES
            {
                public int PrivilegeCount;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
                public LUID_AND_ATTRIBUTES[] Privileges;
            }

            [Flags]
            public enum PrivilegeAttributes
            {
                /// <summary>特权被禁用.</summary>
                Disabled = 0,
                /// <summary>默认特权.</summary>
                EnabledByDefault = 1,
                /// <summary>特权被激活.</summary>
                Enabled = 2,
                /// <summary>特权被废除.</summary>
                Removed = 4,
                /// <summary>用于访问对象或服务的特权.</summary>
                UsedForAccess = -2147483648
            }

            [Flags]
            public enum TokenAccessRights
            {
                /// <summary>向进程附加主令牌的权限.</summary>
                AssignPrimary = 0,
                /// <summary>复制访问令牌的权利.</summary>
                Duplicate = 1,
                /// <summary>向进程附加模拟访问令牌的权限.</summary>
                Impersonate = 4,
                /// <summary>查询访问令牌的权利.</summary>
                Query = 8,
                /// <summary>有权查询访问令牌的来源.</summary>
                QuerySource = 16,
                /// <summary>启用或禁用访问令牌中的特权的权限.</summary>
                AdjustPrivileges = 32,
                /// <summary>调整访问令牌中的组属性的权限.</summary>
                AdjustGroups = 64,
                /// <summary>更改访问令牌的默认所有者、主组或DACL的权限.</summary>
                AdjustDefault = 128,
                /// <summary>正确调整访问令牌的会话ID.</summary>
                AdjustSessionId = 256,
                /// <summary>为令牌组合所有可能的访问权限.</summary>
                AllAccess = AccessTypeMasks.StandardRightsRequired | AssignPrimary | Duplicate | Impersonate
                            | Query | QuerySource | AdjustPrivileges | AdjustGroups | AdjustDefault | AdjustSessionId,
                /// <summary>结合需要阅读的标准权利</summary>
                Read = AccessTypeMasks.StandardRightsRead | Query,
                /// <summary>组合了写入所需的标准权限</summary>
                Write = AccessTypeMasks.StandardRightsWrite | AdjustPrivileges | AdjustGroups | AdjustDefault,
                /// <summary>合并执行所需的标准权限</summary>
                Execute = AccessTypeMasks.StandardRightsExecute | Impersonate
            }

            [Flags]
            private enum AccessTypeMasks
            {
                Delete = 65536,
                ReadControl = 131072,
                WriteDAC = 262144,
                WriteOwner = 524288,
                Synchronize = 1048576,
                StandardRightsRequired = 983040,
                StandardRightsRead = ReadControl,
                StandardRightsWrite = ReadControl,
                StandardRightsExecute = ReadControl,
                StandardRightsAll = 2031616,
                SpecificRightsAll = 65535
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool AdjustTokenPrivileges([In] IntPtr accessTokenHandle, [In] bool disableAllPrivileges,
                [In] ref TOKEN_PRIVILEGES newState, [In] int bufferLength, [In, Out] ref TOKEN_PRIVILEGES previousState, [In, Out] ref int returnLength);

            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool LookupPrivilegeValue([In] string systemName, [In] string name, [In, Out] ref LUID luid);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool OpenProcessToken([In] IntPtr processHandle, [In] TokenAccessRights desiredAccess, [In, Out] ref IntPtr tokenHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int GetLastError();

            public static bool TrySetPrivilege(string sPrivilege, bool enablePrivilege)
            {
                bool blRc;
                TOKEN_PRIVILEGES newTP = new TOKEN_PRIVILEGES();
                TOKEN_PRIVILEGES oldTP = new TOKEN_PRIVILEGES();
                LUID luid = new LUID();
                int retrunLength = 0;
                IntPtr processToken = IntPtr.Zero;

                //本地进程令牌恢复
                blRc = OpenProcessToken(Process.GetCurrentProcess().Handle, TokenAccessRights.AllAccess, ref processToken);
                if(blRc == false) return false;

                //恢复特权的唯一标识符空间
                blRc = LookupPrivilegeValue(null, sPrivilege, ref luid);
                if(blRc == false) return false;

                //建立或取消特权
                newTP.PrivilegeCount = 1;
                newTP.Privileges = new LUID_AND_ATTRIBUTES[64];
                newTP.Privileges[0].Luid = luid;

                if(enablePrivilege) newTP.Privileges[0].Attributes = (int)PrivilegeAttributes.Enabled;
                else newTP.Privileges[0].Attributes = (int)PrivilegeAttributes.Disabled;

                oldTP.PrivilegeCount = 64;
                oldTP.Privileges = new LUID_AND_ATTRIBUTES[64];
                blRc = AdjustTokenPrivileges(processToken, false, ref newTP, 16, ref oldTP, ref retrunLength);

                if(blRc == false) { GetLastError(); return false; }
                return true;
            }
        }

        /// <summary>获取注册表项权限</summary>
        /// <remarks>将注册表项所有者改为当前管理员用户</remarks>
        /// <param name="regPath">要获取权限的注册表完整路径</param>
        public static void TakeRegKeyOwnerShip(string regPath)
        {
            if(regPath.IsNullOrWhiteSpace()) return;
            RegistryKey key = null;
            WindowsIdentity id = null;
            //利用试错判断是否有写入权限
            try { key = RegistryEx.GetRegistryKey(regPath, true); }
            catch
            {
                try
                {
                    //获取当前用户的ID
                    id = WindowsIdentity.GetCurrent();

                    //添加TakeOwnership特权
                    bool flag = NativeMethod.TrySetPrivilege(NativeMethod.TakeOwnership, true);
                    if(!flag) throw new PrivilegeNotHeldException(NativeMethod.TakeOwnership);

                    //添加恢复特权(必须这样做才能更改所有者)
                    flag = NativeMethod.TrySetPrivilege(NativeMethod.Restore, true);
                    if(!flag) throw new PrivilegeNotHeldException(NativeMethod.Restore);

                    //打开没有权限的注册表路径
                    key = RegistryEx.GetRegistryKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);

                    RegistrySecurity security = key.GetAccessControl(AccessControlSections.All);

                    //得到真正所有者
                    //IdentityReference oldId = security.GetOwner(typeof(SecurityIdentifier));
                    //SecurityIdentifier siTrustedInstaller = new SecurityIdentifier(oldId.ToString());

                    //使进程用户成为所有者
                    security.SetOwner(id.User);
                    key.SetAccessControl(security);

                    //添加完全控制
                    RegistryAccessRule fullAccess = new RegistryAccessRule(id.User, RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
                    security.AddAccessRule(fullAccess);
                    key.SetAccessControl(security);

                    //注册表操作（写入、删除）
                    //key.SetValue("temp", "");//示例

                    //恢复原有所有者
                    //security.SetOwner(siTrustedInstaller);
                    //key.SetAccessControl(security);

                    //收回原有权利
                    //security.RemoveAccessRule(fullAccess);
                    //key.SetAccessControl(security);

                    ///得到真正所有者、注册表操作、恢复原有所有者、收回原有权利，这四部分在原文中没有被注释掉
                    ///但是如果保留这四部分，会在恢复原有所有者这一步抛出异常，提示没有权限，
                    ///不过我发现经过上面的操作，虽然无法还原所有者权限，但是已经获取了注册表权限
                    ///即已经将TrustedInstaller权限更改为当前管理员用户权限，我要的目的已经达到了
                }
                catch { }
            }
            finally { key?.Close(); id?.Dispose(); }
        }

        /// <summary>获取注册表项及其子项、递归子级子项权限</summary>
        /// <remarks>将注册表项所有者改为当前管理员用户</remarks>
        /// <param name="regPath">要获取权限的注册表完整路径</param>
        public static void TakeRegTreeOwnerShip(string regPath)
        {
            if(regPath.IsNullOrWhiteSpace()) return;
            TakeRegKeyOwnerShip(regPath);
            try
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(regPath))
                {
                    if(key == null) return;
                    foreach(string subKeyName in key.GetSubKeyNames())
                    {
                        TakeRegTreeOwnerShip($@"{key.Name}\{subKeyName}");
                    }
                }
            }
            catch { }
        }
    }
}