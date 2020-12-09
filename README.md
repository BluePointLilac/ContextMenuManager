# ContextMenuManager
------
> 一个纯粹的Windows右键菜单管理程序

## 主要功能
* 启用或禁用文件、文件夹、新建、发送到、打开方式、自定义文件格式等右键菜单项目
* 对上述场景右键菜单项目进行修改名称、修改图标、导航注册表位置、导航文件位置、永久删除等操作
* 对上述场景右键菜单自定义添加项目，自定义菜单命令

## 兼容性能
* 适用于Win7、8、8.1、10、Vista
* 适用于 x64、x32 CPU 操作系统
* 适用于高分屏，最佳显示缩放比为150%
* 程序支持国际化多语言显示，欢迎为此程序制作语言字典

## 运行截图
![](https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/Screenshot.png)

## 程序图标
* 程序主图标来自 [Easyicon][1]<br>![](https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/AppIcon.ico)
* [程序按钮图标][2] 主要来自于 [阿里巴巴矢量图标资源库][3]


## 下载更新
* 程序有检查更新功能，除了更新程序本身还会更新程序字典，下载完成后直接覆盖原文件即可
* 由于Github Raw被墙，Gitee Raw有月访问次数上限，故将程序设置为每月自动检测一次更新，<br>大家也可以自行浏览 [Github Releases][4] 或者 [Gitee 发行版][5] 检查程序是否有更新。

## 温馨提示
* 一些特殊菜单项目(ShellEx类型，比如文件的加密(&Y))可能会受到其他因素影响，导致不会显示<br>在右键菜单中，但是按照程序使用的通用规则在此程序中仍会显示为启用，这是正常现象。
* 每个右键菜单管理程序禁用菜单方法可能不同，建议不要同时使用多个右键菜单管理程序，大部分<br>同类型程序使用简单暴力的备份-删除法，此程序尽可能使用了系统提供的键值进行隐藏操作；如果<br>之前使用过其他程序禁用右键菜单项目，建议先使用对应软件还原，不然可能无法在此程序中显示。
* 此程序不用于清理未卸载干净的程序，但是可以帮助你快速定位菜单项相关注册表位置和文件位置，<br>你可以根据相关内容进行你的操作。如果你是一个电脑小白，建议只使用启用\禁用功能。

## 联系作者
* 程序由我个人独立开发，当然也要感谢 [萌研社][6] 站长 @坑晨 平时的答疑解惑。能力有限，难免出现<br>一些Bug，欢迎大家积极反馈Bug和提出优化建议。
* 个人B站：[蓝点lilac][7]（欢迎大家关注我！）
* 个人邮箱：1617859183@qq.com

## 捐赠作者
此程序完全免费，如果你觉得这个程序对你有所帮助，可以通过扫面下方二维码（微信、支付宝、QQ）<br>进行捐赠，金额请随意，谢谢你的理解和支持！更加期待你为此项目点亮Star（这对我很重要！）<br>![](https://gitee.com/BluePointLilac/ContextMenuManager/raw/master/ContextMenuManager/Properties/Resources/Images/Donate.png)

  [1]: https://www.easyicon.net/1208132-mouse_icon.html
  [2]: https://github.com/BluePointLilac/ContextMenuManager/tree/master/ContextMenuManager/Properties/Resources/Images
  [3]: https://www.iconfont.cn/
  [4]: https://github.com/BluePointLilac/ContextMenuManager/releases
  [5]: https://gitee.com/BluePointLilac/ContextMenuManager/releases
  [6]: http://www.pcmoe.net/
  [7]: https://space.bilibili.com/34492771