using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace ContextMenuManager.Methods
{
    /// 代码用途：添加WinX菜单项目
    /// 参考代码1：https://github.com/riverar/hashlnk/blob/master/hashlnk.cpp (Rafael Rivera)
    /// 参考代码2：https://github.com/xmoer/HashLnk/blob/main/HashLnk.cs (坑晨)
    static class WinXHasher
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern int HashData(
            [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] byte[] pbData, int cbData,
            [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)] byte[] pbHash, int cbHash);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern uint SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PSGetPropertyKeyFromName([In][MarshalAs(UnmanagedType.LPWStr)] string pszCanonicalName, out PropertyKey propkey);

        [ComImport]
        [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IPropertyStore
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCount([Out] out uint cProps);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetAt([In] uint iProp, out PropertyKey pkey);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetValue([In] ref PropertyKey key, out PropVariant pv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetValue([In] ref PropertyKey key, [In] ref PropVariant pv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Commit();
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        interface IShellItem
        {
            void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
            void GetParent(out IShellItem ppsi);
            void GetDisplayName(uint sigdnName, out IntPtr ppszName);
            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            void Compare(IShellItem psi, uint hint, out int piOrder);
        }

        [ComImport]
        [SuppressUnmanagedCodeSecurity]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("7e9fb0d3-919f-4307-ab2e-9b1860310c93")]
        interface IShellItem2 : IShellItem
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object BindToHandler(IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

            IShellItem GetParent();

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetDisplayName(uint sigdnName);

            uint GetAttributes(uint sfgaoMask);

            int Compare(IShellItem psi, uint hint);

            [return: MarshalAs(UnmanagedType.Interface)]
            IPropertyStore GetPropertyStore(GPS flags, [In] ref Guid riid);

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetPropertyStoreWithCreateObject(GPS flags, [MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject, [In] ref Guid riid);

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetPropertyStoreForKeys(IntPtr rgKeys, uint cKeys, GPS flags, [In] ref Guid riid);

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetPropertyDescriptionList(IntPtr keyType, [In] ref Guid riid);

            void Update(IBindCtx pbc);

            [SecurityCritical]
            void GetProperty(IntPtr key, [In][Out] PropVariant pv);

            Guid GetCLSID(IntPtr key);

            ComTypes.FILETIME GetFileTime(IntPtr key);

            int GetInt32(IntPtr key);

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetString(PropertyKey key);

            uint GetUInt32(IntPtr key);

            ulong GetUInt64(IntPtr key);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetBool(IntPtr key);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct PropertyKey
        {
            public Guid GUID;
            public int PID;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        struct PropVariant
        {
            [FieldOffset(0)] public VarEnum VarType;
            [FieldOffset(2)] public ushort wReserved1;
            [FieldOffset(4)] public ushort wReserved2;
            [FieldOffset(6)] public ushort wReserved3;
            [FieldOffset(8)] public byte bVal;
            [FieldOffset(8)] public sbyte cVal;
            [FieldOffset(8)] public ushort uiVal;
            [FieldOffset(8)] public short iVal;
            [FieldOffset(8)] public uint uintVal;
            [FieldOffset(8)] public int intVal;
            [FieldOffset(8)] public ulong ulVal;
            [FieldOffset(8)] public long lVal;
            [FieldOffset(8)] public float fltVal;
            [FieldOffset(8)] public double dblVal;
            [FieldOffset(8)] public short boolVal;
            [FieldOffset(8)] public IntPtr pclsidVal;
            [FieldOffset(8)] public IntPtr pszVal;
            [FieldOffset(8)] public IntPtr pwszVal;
            [FieldOffset(8)] public IntPtr punkVal;
            [FieldOffset(8)] public IntPtr ca;
            [FieldOffset(8)] public ComTypes.FILETIME filetime;

        }

        [Flags]
        enum GPS
        {
            DEFAULT = 0x00000000,
            HANDLERPROPERTIESONLY = 0x00000001,
            READWRITE = 0x00000002,
            TEMPORARY = 0x00000004,
            FASTPROPERTIESONLY = 0x00000008,
            OPENSLOWITEM = 0x00000010,
            DELAYCREATION = 0x00000020,
            BESTEFFORT = 0x00000040,
            NO_OPLOCK = 0x00000080,
            MASK_VALID = 0x000000FF
        }

        public static void HashLnk(string lnkPath)
        {
            SHCreateItemFromParsingName(lnkPath, null, typeof(IShellItem2).GUID, out IShellItem item);
            IShellItem2 item2 = (IShellItem2)item;
            PSGetPropertyKeyFromName("System.Link.TargetParsingPath", out PropertyKey pk);
            //shellPKey = PKEY_Link_TargetParsingPath
            //formatID = B9B4B3FC-2B51-4A42-B5D8-324146AFCF25, propID = 2
            string targetPath;
            try { targetPath = item2.GetString(pk); }
            catch { targetPath = null; }

            PSGetPropertyKeyFromName("System.Link.Arguments", out pk);
            //shellPKey = PKEY_Link_Arguments
            //formatID = 436F2667-14E2-4FEB-B30A-146C53B5B674, propID = 100
            string arguments;
            try { arguments = item2.GetString(pk); }
            catch { arguments = null; }

            string blob = GetGeneralizePath(targetPath) + arguments;
            blob += "do not prehash links.  this should only be done by the user.";//特殊但必须存在的字符串
            blob = blob.ToLower();
            byte[] inBytes = Encoding.Unicode.GetBytes(blob);
            int byteCount = inBytes.Length;
            byte[] outBytes = new byte[byteCount];
            HashData(inBytes, byteCount, outBytes, byteCount);
            uint hash = BitConverter.ToUInt32(outBytes, 0);

            Guid guid = typeof(IPropertyStore).GUID;
            IPropertyStore store = item2.GetPropertyStore(GPS.READWRITE, ref guid);
            PSGetPropertyKeyFromName("System.Winx.Hash", out pk);
            //shellPKey = PKEY_WINX_HASH
            //formatID = FB8D2D7B-90D1-4E34-BF60-6EAC09922BBF, propID = 2
            PropVariant pv = new PropVariant { VarType = VarEnum.VT_UI4, ulVal = hash };
            store.SetValue(ref pk, ref pv);
            store.Commit();

            Marshal.ReleaseComObject(store);
            Marshal.ReleaseComObject(item);
        }

        private static readonly Dictionary<string, string> GeneralizePathDic = new Dictionary<string, string>
        {
            { "%ProgramFiles%", "{905e63b6-c1bf-494e-b29c-65b732d3d21a}" },
            { "%SystemRoot%\\System32", "{1ac14e77-02e7-4e5d-b744-2eb1ae5198b7}" },
            { "%SystemRoot%", "{f38bf404-1d43-42f2-9305-67de0b28fc23}" }
        };

        private static string GetGeneralizePath(string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) return filePath;
            foreach(var kv in GeneralizePathDic)
            {
                string dirPath = Environment.ExpandEnvironmentVariables(kv.Key);
                if(filePath.StartsWith(dirPath + "\\", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = filePath.Replace(dirPath, kv.Value); break;
                }
            }
            return filePath;
        }
    }
}