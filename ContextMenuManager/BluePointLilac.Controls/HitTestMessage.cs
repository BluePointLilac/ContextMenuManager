namespace BluePointLilac.Controls
{
    public static class HitTestMessage
    {
        /// <summary>光标移动或鼠标按下、释放时的消息</summary>
        public const int WM_NCHITTEST = 0x84;
        /// <summary>鼠标击中位置</summary>
        public enum HitTest : int
        {
            Error = -2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            TitleBar = 2,
            SysMenu = 3,
            Size = 4,
            GrowBox = 5,
            Hscroll = 6,
            Vscroll = 7,
            MinButton = 8,
            MaxButton = 9,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18,
            Close = 20,
            Help = 21
        }
    }
}