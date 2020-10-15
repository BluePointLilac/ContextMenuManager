namespace ContextMenuManager.Properties
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources
    {
        private static global::System.Resources.ResourceManager resourceMan;
        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() { }

        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if(object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ContextMenuManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap About
        {
            get
            {
                object obj = ResourceManager.GetObject("About", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Add
        {
            get
            {
                object obj = ResourceManager.GetObject("Add", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap AddCommon
        {
            get
            {
                object obj = ResourceManager.GetObject("AddCommon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap AddExisting
        {
            get
            {
                object obj = ResourceManager.GetObject("AddExisting", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap AddSeparator
        {
            get
            {
                object obj = ResourceManager.GetObject("AddSeparator", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找类似 ;此文件为ContextMenuManager程序的显示文本字典, 字典内换行符使用\n转义
        ///;如果你想要帮助作者为此程序添加其他语言字典, 可在.\config\languages文件夹内制作字典文件, 
        ///;比如英语字典保存为en-us.ini, 并给[General]\General_Language赋值 en-us 英语
        ///
        ///[General]
        ///General_Language = zh-cn 简体中文
        ///General_AppName = Windows右键管理
        ///
        ///[ToolBar]
        ///ToolBar_Home = 主页
        ///ToolBar_Type = 文件类型
        ///ToolBar_Rule = 其他规则
        ///ToolBar_About = 关于
        ///
        ///[SideBar]
        ///SideBar_File = 文件
        ///SideBar_Folder = 文件夹
        ///SideBar_Directory = 目录
        ///SideBar_Background = 目录背景
        ///SideBar_Desktop = 桌面背景
        ///SideBar_Drive = 磁盘分区
        ///SideBar_AllObjects = 所有对 [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string AppLanguageDic
        {
            get
            {
                return ResourceManager.GetString("AppLanguageDic", resourceCulture);
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap CustomType
        {
            get
            {
                object obj = ResourceManager.GetObject("CustomType", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Delete
        {
            get
            {
                object obj = ResourceManager.GetObject("Delete", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Donate
        {
            get
            {
                object obj = ResourceManager.GetObject("Donate", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找类似 ;&lt;说明&gt;:
        ///;由于ShellEx类型右键菜单的菜单名称和图标无法直接获取，只能通过制作字典给用户更直观的体验
        ///;此文件为依赖&lt;GUID&gt;的ShellEx类型右键菜单项目的名称和图标字典,用户可自行添加字典
        ///;&lt;GUID&gt;可通过右键程序内ShellEx项目&quot;复制guid&quot;获取
        ///;不带括号的&lt;GUID&gt;为字典索引
        ///
        ///;&lt;Text&gt;:
        ///;Text为菜单项目名称
        ///;可以赋值为引用资源文件字符串资源的本地化字符串,
        ///;格式为&quot;@&lt;资源文件路径&gt;,-&lt;字符串资源索引&gt;&quot;,如赋值为&quot;@shell32.dll,-3576&quot;
        ///;也可以赋值为直接显示名称,如赋值为&quot;使用XXX打开&quot;
        ///
        ///;&lt;Icon&gt;:
        ///;Icon为菜单项目图标资源位置
        ///;格式为&quot;&lt;资源文件路径&gt;,&lt;图标序号&gt;&quot;，如赋值为&quot;C:Windows\System32\imageres.dll,203&quot;
        ///;&lt;图标序号&gt;为负数则为图标资源索引,为非负数则为图标资源顺序序号
        ///;Icon为空时默认提取文件第一个图标，没有图标则使用dll文件默认图标
        ///
        ///;&lt;资源文件路径&gt;:
        ///;&lt;Text&gt;和&lt;Icon&gt;中的&lt;资源文件路径&gt;一般使用相对路径
        /// [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string GuidInfosDic
        {
            get
            {
                return ResourceManager.GetString("GuidInfosDic", resourceCulture);
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Home
        {
            get
            {
                object obj = ResourceManager.GetObject("Home", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap MicrosoftStore
        {
            get
            {
                object obj = ResourceManager.GetObject("MicrosoftStore", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap NewItem
        {
            get
            {
                object obj = ResourceManager.GetObject("NewItem", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Open
        {
            get
            {
                object obj = ResourceManager.GetObject("Open", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap SeparatorItem
        {
            get
            {
                object obj = ResourceManager.GetObject("SeparatorItem", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Setting
        {
            get
            {
                object obj = ResourceManager.GetObject("Setting", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找类似 &lt;?xml version=&apos;1.0&apos; encoding=&apos;utf-8&apos; ?&gt;
        ///&lt;!--此文件为常用右键菜单字典，如果想要添加字典，请保存为.\config\CommonItemsDic.xml后再进行添加--&gt;
        ///
        ///&lt;Data&gt;
        ///  &lt;Group RegPath=&apos;HKEY_CLASSES_ROOT\*&apos;&gt;
        ///    &lt;Shell&gt;
        ///      &lt;Item KeyName=&apos;CopyContent&apos; Tip=&apos;不需打开文件直接复制文件文本内容&amp;#x000A;非UTF-16 LE(或带BOM)编码会乱码&apos;&gt;
        ///        &lt;Value&gt;
        ///          &lt;REG_SZ MUIVerb=&apos;复制内容到剪切板&apos; Icon=&apos;DxpTaskSync.dll,-52&apos;/&gt;
        ///        &lt;/Value&gt;
        ///        &lt;SubKey&gt;
        ///          &lt;Command Default=&apos;cmd /c clip &amp;lt; &quot;%1&quot;&apos;/&gt;
        ///        &lt;/SubKey&gt;
        ///      &lt;/Item&gt;
        ///      &lt;Item KeyName=&apos;TakeOwnerShip&apos;&gt;
        ///     [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string ShellCommonDic
        {
            get
            {
                return ResourceManager.GetString("ShellCommonDic", resourceCulture);
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Star
        {
            get
            {
                object obj = ResourceManager.GetObject("Star", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap SubItems
        {
            get
            {
                object obj = ResourceManager.GetObject("SubItems", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;!--每个程序为一个Group，Text为Group项显示文本，Guid用于判断用户是否安装此程序并决定是否显示该Group，当菜单项为常驻菜单时添加属性Common，RegPath为程序相关注册表主路径;
        ///其相关菜单项目设置作为一个Item子元素，Item的Text为该Item项显示文本，Tag属性为鼠标悬浮在开关上时状态栏显示的相关提示信息，需要重启资源管理器生效则添加属性RestartExplorer;
        ///Item的子元素Rule为相关注册表内容，RegPath省略则默认为Group主路径，以\开头则为Group主路径的子项路径；ValueName为相关键名，On为启用键值，Off为禁用键值；
        ///每个Item可能受多个注册表Rule影响，按照顺序进行键值判定；程序优先判定为On，即只要所有Rule不匹配Off键值就判定为On，键值类型不符时也判定为On;
        ///ValueKind为键值类型，默认键值类型ValueKind为REG_DWORD，为默认值时可省略，键值类型有：REG_SZ、REG_BINARY、REG_DWOR [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string ThirdRulesDic
        {
            get
            {
                return ResourceManager.GetString("ThirdRulesDic", resourceCulture);
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap TurnOff
        {
            get
            {
                object obj = ResourceManager.GetObject("TurnOff", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap TurnOn
        {
            get
            {
                object obj = ResourceManager.GetObject("TurnOn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Type
        {
            get
            {
                object obj = ResourceManager.GetObject("Type", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Types
        {
            get
            {
                object obj = ResourceManager.GetObject("Types", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap Up
        {
            get
            {
                object obj = ResourceManager.GetObject("Up", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
