using System.Collections.Generic;

namespace ContextMenuManager.Methods
{
    // List带一个Idx属性
    public class MyCustomList<T>: List<T>
    {

        public MyCustomList():base(100)
        {
        }
        public MyCustomList(int n):base(n)
        {
        }
        // 列表迭代时起点索引
        private int idx=0;
        
        public int Idx
        {
            get => idx;
            set => idx = value;
        }
    }
}