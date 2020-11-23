using System;

namespace BulePointLilac.Methods
{
    //判断Windows系统版本
    public static class WindowsOsVersion
    {
        public static readonly Version OsVersion = Environment.OSVersion.Version;
        private static readonly float ShortVersion = OsVersion.Major + OsVersion.Minor / 10;

        private const float Win10 = 10.0F;
        private const float Win8_1 = 6.3F;
        private const float Win8 = 6.2F;
        private const float Win7 = 6.1F;
        private const float Vista = 6.0F;

        public static bool IsEqual10 = ShortVersion == Win10;
        public static bool IsAfter10 = ShortVersion > Win10;
        public static bool IsBefore10 = ShortVersion < Win10;
        public static bool ISAfterOrEqual10 = ShortVersion >= Win10;
        public static bool ISBeforeOrEqual10 = ShortVersion <= Win10;

        public static bool IsEqual8_1 = ShortVersion == Win8_1;
        public static bool IsAfter8_1 = ShortVersion > Win8_1;
        public static bool IsBefore8_1 = ShortVersion < Win8_1;
        public static bool ISAfterOrEqual8_1 = ShortVersion >= Win8_1;
        public static bool ISBeforeOrEqual8_1 = ShortVersion <= Win8_1;
        
        public static bool IsEqual8 = ShortVersion == Win8;
        public static bool IsAfter8 = ShortVersion > Win8;
        public static bool IsBefore8 = ShortVersion < Win8;
        public static bool ISAfterOrEqual8 = ShortVersion >= Win8;
        public static bool ISBeforeOrEqual8 = ShortVersion <= Win8;
        
        public static bool IsEqual7 = ShortVersion == Win7;
        public static bool IsAfter7 = ShortVersion > Win7;
        public static bool IsBefore7 = ShortVersion < Win7;
        public static bool ISAfterOrEqual7 = ShortVersion >= Win7;
        public static bool ISBeforeOrEqual7 = ShortVersion <= Win7;

        public static bool IsEqualVista = ShortVersion == Vista;
        public static bool IsAfterVista = ShortVersion > Vista;
        public static bool IsBeforeVista = ShortVersion < Vista;
        public static bool ISAfterOrEqualVista = ShortVersion >= Vista;
        public static bool ISBeforeOrEqualVista = ShortVersion <= Vista;
    }
}