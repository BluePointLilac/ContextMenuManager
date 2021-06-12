**[简体中文](README.md)** | **English**
# ContextMenuManager
------
> 🖱️ A program to manage the Windows right-click context menu.

## Key features
* Enable and disable context menu options for files, folders, submenus (e.g. open, send to), Internet Explorer, and Win + X
* Modify names and icons of menu options
* Delete context menu entries
* Open menu option locations in the registry or file explorer
* Add custom menu items and commands

## Supported systems and features 
* Windows 7, 8, 8.1, 10, Vista
* 32 and 64 bit systems
* Support for display scaling, optimally at 150%
* localization support, contributions are welcome

## Screenshots
![](Screenshot/Screenshot-en.png)

## Third-party resources
* Main program icon from [EasyIcon][EasyIcon]<br>![][AppIcon]
* [Button icons][AppImage] from [Alibaba Iconfont][IconFont]![](Screenshot/AppImage.png)

## Updates
* Program and dictionary updates can be installed within the program, overwritting the original files.
* Due to limitations with Github and Gitee Raw, the program can only check for updates once a month. <br> The latest updates can always be found on [Github Releases][GitHub Releases] or [Gitee Releases][Gitee Releases].

## Notices
* Some special menu items (Shell extensions, file encryption(&Y)) may not be displayed in the context menu, but will still show as enabled within the program; this is normal.
* Different context menu manager programs may use different methods for disabling menu options. Using multiple managers at once is not recommended. While other programs may use destructive methods, this program will utilize the registry keys provided by the system to hide menu items when possible.
<br>If you have used other context menu managers in the past, it is recommended that you use that program to restore the menu items before using this one, in order to avoid any potential issues.
* This program is not designed to perform clean uninstalls; however, it can help you find the registry and file locations of menu items so that they can be modified. If you are not familar with such operations, it is recommended you use the enable/disable functions only.

## Author contact
* This program was developed independently by me (BluePointLilac), though I would like to thank [PcMoe][PcMoe] admin @坑晨 for answering my questions. There will inevitably be bugs, so any reports and suggestions are welcome.
* My Bilibili page: [蓝点lilac][Bilibili]（Follow me!）
* My e-mail: 1617859183@qq.com

## Donations
This program is completely free of charge; if you find this program useful, you can donate by scanning the QR codes below（WeChat, Alipay, QQ) 
<br>Any amount is welcome，thank you for your understanding and support! Please also don't forget to star this repo (It means a lot to me!）<br>![][Donate]

  [EasyIcon]: https://www.easyicon.net/1208132-mouse_icon.html
  [AppIcon]: ContextMenuManager/Properties/AppIcon.ico
  [AppImage]: ContextMenuManager/Properties/Resources/Images
  [IconFont]: https://www.iconfont.cn
  [HashLnk]: https://github.com/riverar/hashlnk
  [GitHub Releases]: https://github.com/BluePointLilac/ContextMenuManager/releases
  [Gitee Releases]: https://gitee.com/BluePointLilac/ContextMenuManager/releases
  [PcMoe]: http://www.pcmoe.net
  [Bilibili]: https://space.bilibili.com/34492771
  [Donate]: ContextMenuManager/Properties/Resources/Images/Donate.png