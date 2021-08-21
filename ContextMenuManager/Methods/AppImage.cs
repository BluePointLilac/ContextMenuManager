using BluePointLilac.Methods;
using ContextMenuManager.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace ContextMenuManager.Methods
{
    static class AppImage
    {
        private static readonly double Scale = HighDpi.DpiScale / 1.5;
        ///<summary>主页</summary>
        public static readonly Image Home = Resources.Home.ResizeImage(Scale);
        ///<summary>文件类型</summary>
        public static readonly Image Type = Resources.Type.ResizeImage(Scale);
        ///<summary>五角星</summary>
        public static readonly Image Star = Resources.Star.ResizeImage(Scale);
        ///<summary>刷新</summary>
        public static readonly Image Refresh = Resources.Refresh.ResizeImage(Scale);
        ///<summary>关于问号</summary>
        public static readonly Image About = Resources.About.ResizeImage(Scale);
        ///<summary>设置按钮</summary>
        public static readonly Image Setting = Resources.Setting.ResizeImage(Scale);
        ///<summary>编辑子项</summary>
        public static readonly Image SubItems = Resources.SubItems.ResizeImage(Scale);
        ///<summary>删除</summary>
        public static readonly Image Delete = Resources.Delete.ResizeImage(Scale);
        ///<summary>添加</summary>
        public static readonly Image AddNewItem = Resources.Add.ResizeImage(Scale);
        ///<summary>添加已有项目</summary>
        public static readonly Image AddExisting = Resources.AddExisting.ResizeImage(Scale);
        ///<summary>添加分割线</summary>
        public static readonly Image AddSeparator = Resources.AddSeparator.ResizeImage(Scale);
        ///<summary>添加增强菜单</summary>
        public static readonly Image Enhance = Resources.Enhance.ResizeImage(Scale);
        ///<summary>打开</summary>
        public static readonly Image Open = Resources.Open.ResizeImage(Scale);
        ///<summary>下载</summary>
        public static readonly Image DownLoad = Resources.DownLoad.ResizeImage(Scale);
        ///<summary>翻译</summary>
        public static readonly Image Translate = Resources.Translate.ResizeImage(Scale);
        ///<summary>检查更新</summary>
        public static readonly Image CheckUpdate = Resources.CheckUpdate.ResizeImage(Scale);
        ///<summary>上</summary>
        public static readonly Image Up = Resources.Up.ResizeImage(Scale);
        ///<summary>下</summary>
        public static readonly Image Down = Up.RotateImage(RotateFlipType.Rotate180FlipNone);
        ///<summary>新建项目</summary>
        public static readonly Image NewItem = Resources.NewItem.ResizeImage(Scale);
        ///<summary>新建文件夹</summary>
        public static readonly Image NewFolder = Resources.NewFolder.ResizeImage(Scale);
        ///<summary>自定义</summary>
        public static readonly Image Custom = Resources.Custom.ResizeImage(Scale);
        ///<summary>选择</summary>
        public static readonly Image Select = Resources.Select.ResizeImage(Scale);
        ///<summary>跳转</summary>
        public static readonly Image Jump = Resources.Jump.ResizeImage(Scale);
        ///<summary>Microsoft Store</summary>
        public static readonly Image MicrosoftStore = Resources.MicrosoftStore.ResizeImage(Scale);
        ///<summary>用户</summary>
        public static readonly Image User = Resources.User.ResizeImage(Scale);
        ///<summary>网络</summary>
        public static readonly Image Web = Resources.Web.ResizeImage(Scale);
        ///<summary>系统文件</summary>
        public static readonly Image SystemFile = GetIconImage("imageres.dll", -67);
        ///<summary>资源不存在</summary>
        public static readonly Image NotFound = GetIconImage("imageres.dll", -2);
        ///<summary>管理员小盾牌</summary>
        public static readonly Image Shield = GetIconImage("imageres.dll", -78);
        ///<summary>资源管理器</summary>
        public static readonly Image Explorer = GetIconImage("explorer.exe", 0);
        ///<summary>重启Explorer</summary>
        public static readonly Image RestartExplorer = GetIconImage("shell32.dll", 238);
        ///<summary>网络驱动器</summary>
        public static readonly Image NetworkDrive = GetIconImage("imageres.dll", -33);
        ///<summary>发送到</summary>
        public static readonly Image SendTo = GetIconImage("imageres.dll", -185);
        ///<summary>回收站</summary>
        public static readonly Image RecycleBin = GetIconImage("imageres.dll", -55);
        ///<summary>磁盘</summary>
        public static readonly Image Drive = GetIconImage("imageres.dll", -30);
        ///<summary>文件</summary>
        public static readonly Image File = GetIconImage("imageres.dll", -19);
        ///<summary>文件夹</summary>
        public static readonly Image Folder = GetIconImage("imageres.dll", -3);
        ///<summary>目录</summary>
        public static readonly Image Directory = GetIconImage("imageres.dll", -162);
        ///<summary>所有对象</summary>
        public static readonly Image AllObjects = GetIconImage("imageres.dll", -117);
        ///<summary>锁定</summary>
        public static readonly Image Lock = GetIconImage("imageres.dll", -59);
        ///<summary>快捷方式图标</summary>
        public static readonly Image LnkFile = GetIconImage("shell32.dll", -16769);

        private static Image GetIconImage(string dllName, int iconIndex)
        {
            using(Icon icon = ResourceIcon.GetIcon(dllName, iconIndex)) return icon?.ToBitmap();
        }
    }
}