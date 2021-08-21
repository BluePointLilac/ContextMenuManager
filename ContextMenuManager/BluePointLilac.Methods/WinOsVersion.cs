using System;

namespace BluePointLilac.Methods
{
    // 判断Windows系统版本
    // https://docs.microsoft.com/windows/release-health/release-information
    public static class WinOsVersion
    {
        public static readonly Version Current = Environment.OSVersion.Version;
        public static readonly Version Win10 = new Version(10, 0);
        public static readonly Version Win8_1 = new Version(6, 3);
        public static readonly Version Win8 = new Version(6, 2);
        public static readonly Version Win7 = new Version(6, 1);
        public static readonly Version Vista = new Version(6, 0);
        public static readonly Version XP = new Version(5, 1);

        public static readonly Version Win10_1507 = new Version(10, 0, 10240);
        public static readonly Version Win10_1511 = new Version(10, 0, 10586);
        public static readonly Version Win10_1607 = new Version(10, 0, 14393);
        public static readonly Version Win10_1703 = new Version(10, 0, 15063);
        public static readonly Version Win10_1709 = new Version(10, 0, 16299);
        public static readonly Version Win10_1803 = new Version(10, 0, 17134);
        public static readonly Version Win10_1809 = new Version(10, 0, 17763);
        public static readonly Version Win10_1903 = new Version(10, 0, 18362);
        public static readonly Version Win10_1909 = new Version(10, 0, 18363);
        public static readonly Version Win10_2004 = new Version(10, 0, 19041);
        public static readonly Version Win10_20H2 = new Version(10, 0, 19042);
    }
}