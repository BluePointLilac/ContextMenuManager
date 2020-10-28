using BulePointLilac.Methods;
using ContextMenuManager.Properties;
using System.Drawing;

namespace ContextMenuManager
{
    public static class AppImage
    {
        private static readonly double Scale = HighDpi.DpiScale / 1.5;
        ///<summary>主页图标</summary>
        public static readonly Image Home = Resources.Home.ResizeImage(Scale);
        ///<summary>文件类型图标</summary>
        public static readonly Image Type = Resources.Type.ResizeImage(Scale);
        ///<summary>五角星图标</summary>
        public static readonly Image Star = Resources.Star.ResizeImage(Scale);
        ///<summary>关于问号图标</summary>
        public static readonly Image About = Resources.About.ResizeImage(Scale);
        ///<summary>设置按钮图标</summary>
        public static readonly Image Setting = Resources.Setting.ResizeImage(Scale);
        ///<summary>开关打开状态图片</summary>
        public static readonly Image TurnOn = Resources.TurnOn.ResizeImage(Scale);
        ///<summary>开关关闭状态图片</summary>
        public static readonly Image TurnOff = Resources.TurnOff.ResizeImage(Scale);
        ///<summary>编辑子项图标</summary>
        public static readonly Image SubItems = Resources.SubItems.ResizeImage(Scale);
        ///<summary>删除图标</summary>
        public static readonly Image Delete = Resources.Delete.ResizeImage(Scale);
        ///<summary>添加图标</summary>
        public static readonly Image AddNewItem = Resources.Add.ResizeImage(Scale);
        ///<summary>添加常用菜单项图标</summary>
        public static readonly Image AddCommon = Resources.AddCommon.ResizeImage(Scale);
        ///<summary>添加已有项目图标</summary>
        public static readonly Image AddExisting = Resources.AddExisting.ResizeImage(Scale);
        ///<summary>添加分割线图标</summary>
        public static readonly Image AddSeparator = Resources.AddSeparator.ResizeImage(Scale);
        ///<summary>打开图标</summary>
        public static readonly Image Open = Resources.Open.ResizeImage(Scale);
        ///<summary>上图标</summary>
        public static readonly Image Up = Resources.Up.ResizeImage(Scale);
        ///<summary>下图标</summary>
        public static readonly Image Down = Up.RotateImage(RotateFlipType.Rotate180FlipNone);
        ///<summary>新建项目图标</summary>
        public static readonly Image NewItem = Resources.NewItem.ResizeImage(Scale);
        ///<summary>分隔线图标</summary>
        public static readonly Image Separator = Resources.SeparatorItem.ResizeImage(Scale);
        ///<summary>自定义类型图标</summary>
        public static readonly Image CustomType = Resources.CustomType.ResizeImage(Scale);
        ///<summary>所有文件类型图标</summary>
        public static readonly Image Types = Resources.Types.ResizeImage(Scale);
        ///<summary>Microsoft Store图标</summary>
        public static readonly Image MicrosoftStore = Resources.MicrosoftStore.ResizeImage(Scale);
        ///<summary>dll文件默认图标</summary>
        public static readonly Image DllDefaultIcon = ResourceIcon.GetExtensionIcon(".dll").ToBitmap();
        ///<summary>资源不存在图标</summary>
        public static readonly Image NotFound = ResourceIcon.GetIcon("imageres.dll", 2).ToBitmap();
        ///<summary>管理员小盾牌</summary>
        public static readonly Image Shield = ResourceIcon.GetIcon("imageres.dll", 73).ToBitmap();
        ///<summary>资源管理器图标</summary>
        public static readonly Image Explorer = ResourceIcon.GetIcon("explorer.exe", 0).ToBitmap();
        ///<summary>刷新图标</summary>
        public static readonly Image Refresh = ResourceIcon.GetIcon("shell32.dll", 238).ToBitmap();
        ///<summary>自定义文件夹图标</summary>
        public static readonly Image CustomFolder = ResourceIcon.GetIcon("imageres.dll", 3).ToBitmap();
        ///<summary>网络驱动器图标</summary>
        public static readonly Image NetworkDrive = ResourceIcon.GetIcon("imageres.dll", 28).ToBitmap();
        ///<summary>回收站属性图标</summary>
        public static readonly Image RecycleBinProperties = ResourceIcon.GetIcon("shell32.dll", -254).ToBitmap();
        ///<summary>磁盘图标</summary>
        public static readonly Image Drive = ResourceIcon.GetIcon("imageres.dll", 27).ToBitmap();
        ///<summary>发送到图标</summary>
        public static readonly Image SendTo = ResourceIcon.GetIcon("imageres.dll", 176).ToBitmap();
    }
}