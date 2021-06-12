using BluePointLilac.Methods;
using System;
using System.Windows.Forms;

namespace ContextMenuManager
{
    //使用VS 2017、VS 2019编译通过，最低兼容.Net Framework 3.5
    //在编译前在Properties\应用程序\目标框架中修改为对应框架即可
    //完美兼容.Net Framework 3.5(Win7自带)、.Net Framework 4(Win8、Win8.1、Win10自带)
    //避免用户未安装或不会安装系统未携带对应版本的.Net Framework，这样可以直接启动程序，不用担心.Net框架问题，
    //同时兼容Vista，但是需要已安装.Net Framework 3.5或.Net Framework 4才能使用对应的版本
    //实在不想再往.Net Framework 3.0兼容了，xp系统更不用说，直接放弃
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(SingleInstance.IsRunning()) return;
            Updater.PeriodicUpdate();
            Application.Run(new MainForm());
        }
    }
}