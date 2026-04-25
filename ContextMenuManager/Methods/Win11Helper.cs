using BluePointLilac.Methods;
using Microsoft.Win32;

namespace ContextMenuManager.Methods
{
    public static class Win11Helper
    {
        private const string CLSID_CLASSIC_MENU = @"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
        private const string INPROC_SERVER32 = "InprocServer32";

        public static bool IsClassicMenuEnabled
        {
            get
            {
                using(RegistryKey key = RegistryEx.GetRegistryKey(CLSID_CLASSIC_MENU + "\\" + INPROC_SERVER32))
                {
                    return key != null;
                }
            }
            set
            {
                if(value)
                {
                    using(RegistryKey key = RegistryEx.GetRegistryKey(CLSID_CLASSIC_MENU + "\\" + INPROC_SERVER32, true, true))
                    {
                        key.SetValue("", "");
                    }
                }
                else
                {
                    RegistryEx.DeleteKeyTree(CLSID_CLASSIC_MENU);
                }
            }
        }
    }
}
