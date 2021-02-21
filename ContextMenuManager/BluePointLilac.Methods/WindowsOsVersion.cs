using System;

namespace BluePointLilac.Methods
{
    //判断Windows系统版本
    public static class WindowsOsVersion
    {
        public static readonly Version OsVersion = Environment.OSVersion.Version;
        private static readonly float ShortVersion = OsVersion.Major + OsVersion.Minor / 10F;

        private const float Win10 = 10.0F;
        private const float Win8_1 = 6.3F;
        private const float Win8 = 6.2F;
        private const float Win7 = 6.1F;
        private const float Vista = 6.0F;

        public static readonly bool IsEqual10 = ShortVersion == Win10;
        public static readonly bool IsAfter10 = ShortVersion > Win10;
        public static readonly bool IsBefore10 = ShortVersion < Win10;
        public static readonly bool ISAfterOrEqual10 = ShortVersion >= Win10;
        public static readonly bool ISBeforeOrEqual10 = ShortVersion <= Win10;

        public static readonly bool IsEqual8_1 = ShortVersion == Win8_1;
        public static readonly bool IsAfter8_1 = ShortVersion > Win8_1;
        public static readonly bool IsBefore8_1 = ShortVersion < Win8_1;
        public static readonly bool ISAfterOrEqual8_1 = ShortVersion >= Win8_1;
        public static readonly bool ISBeforeOrEqual8_1 = ShortVersion <= Win8_1;

        public static readonly bool IsEqual8 = ShortVersion == Win8;
        public static readonly bool IsAfter8 = ShortVersion > Win8;
        public static readonly bool IsBefore8 = ShortVersion < Win8;
        public static readonly bool ISAfterOrEqual8 = ShortVersion >= Win8;
        public static readonly bool ISBeforeOrEqual8 = ShortVersion <= Win8;

        public static readonly bool IsEqual7 = ShortVersion == Win7;
        public static readonly bool IsAfter7 = ShortVersion > Win7;
        public static readonly bool IsBefore7 = ShortVersion < Win7;
        public static readonly bool ISAfterOrEqual7 = ShortVersion >= Win7;
        public static readonly bool ISBeforeOrEqual7 = ShortVersion <= Win7;

        public static readonly bool IsEqualVista = ShortVersion == Vista;
        public static readonly bool IsAfterVista = ShortVersion > Vista;
        public static readonly bool IsBeforeVista = ShortVersion < Vista;
        public static readonly bool ISAfterOrEqualVista = ShortVersion >= Vista;
        public static readonly bool ISBeforeOrEqualVista = ShortVersion <= Vista;

        public static readonly bool IsAfterOrEqualWin10_1703 = OsVersion.CompareTo(new Version(10, 0, 15063)) >= 0;
    }
}