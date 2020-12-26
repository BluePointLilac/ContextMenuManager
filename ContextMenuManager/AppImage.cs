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
        ///<summary>刷新图标</summary>
        public static readonly Image Refresh = Resources.Refresh.ResizeImage(Scale);
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
        ///<summary>Skype图标</summary>
        public static readonly Image Skype = Resources.Skype.ResizeImage(Scale);
        ///<summary>dll文件默认图标</summary>
        public static readonly Image DllDefaultIcon = ResourceIcon.GetExtensionIcon(".dll").ToBitmap();
        ///<summary>资源不存在图标</summary>
        public static readonly Image NotFound = ResourceIcon.GetIcon("imageres.dll", -2).ToBitmap();
        ///<summary>管理员小盾牌</summary>
        public static readonly Image Shield = ResourceIcon.GetIcon("imageres.dll", -78).ToBitmap();
        ///<summary>资源管理器图标</summary>
        public static readonly Image Explorer = ResourceIcon.GetIcon("explorer.exe", 0).ToBitmap();
        ///<summary>命令提示符图标</summary>
        public static readonly Image Cmd= ResourceIcon.GetIcon("cmd.exe", 0).ToBitmap();
        ///<summary>重启Explorer图标</summary>
        public static readonly Image RestartExplorer = ResourceIcon.GetIcon("shell32.dll", 238).ToBitmap();
        ///<summary>网络驱动器图标</summary>
        public static readonly Image NetworkDrive = ResourceIcon.GetIcon("imageres.dll", -33).ToBitmap();
        ///<summary>发送到图标</summary>
        public static readonly Image SendTo = ResourceIcon.GetIcon("imageres.dll", -185).ToBitmap();
        ///<summary>回收站图标</summary>
        public static readonly Image RecycleBin = ResourceIcon.GetIcon("imageres.dll", -55).ToBitmap();
        ///<summary>此电脑图标</summary>
        public static readonly Image Computer = ResourceIcon.GetIcon("imageres.dll", -109).ToBitmap();
        ///<summary>磁盘图标</summary>
        public static readonly Image Drive = ResourceIcon.GetIcon("imageres.dll", -30).ToBitmap();
        ///<summary>文件图标</summary>
        public static readonly Image File = ResourceIcon.GetIcon("imageres.dll", -19).ToBitmap();
        ///<summary>文件夹图标</summary>
        public static readonly Image Folder = ResourceIcon.GetIcon("imageres.dll", -3).ToBitmap();
        ///<summary>目录图标</summary>
        public static readonly Image Directory = ResourceIcon.GetIcon("imageres.dll", -162).ToBitmap();
        ///<summary目录背景图标</summary>
        public static readonly Image Background = ResourceIcon.GetIcon("imageres.dll", 0).ToBitmap();
        ///<summary>桌面图标</summary>
        public static readonly Image Desktop = ResourceIcon.GetIcon("imageres.dll", -183).ToBitmap();
        ///<summary>所有对象图标</summary>
        public static readonly Image AllObjects = ResourceIcon.GetIcon("imageres.dll", -117).ToBitmap();
        ///<summary>锁定图标</summary>
        public static readonly Image Lock = ResourceIcon.GetIcon("imageres.dll", -59).ToBitmap();
    }
}