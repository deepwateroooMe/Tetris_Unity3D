using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.Data {

    // 原游戏中照搬过来的,没有检查逻辑
    // 感觉这个步骤有点儿: 把问题系统化了,是一个系列嵌套下来的;
    // 但同时也把问题复杂化了,它所需要知道的只是几个子方块的位置信息而已;
    // 需要考虑到消除的时候移除单个立方体:所以什么样的设计,既可以简化序列化,同样方便添减操作
    // 同样需要考虑到以后的扩展,比如某个方块砖会出现5个小立方体等
    public class MinoDataCollectionCon <TetrominoDataCon, MinoDataCon> : IList<MinoDataCon>
        where TetrominoDataCon : class
        where MinoDataCon : IMinoData<TetrominoDataCon>
    {

        // 相比于普通集合链表,这里的优点:维护了每个小立方体与父控件的父子关系,方便游戏过程中使用
        private TetrominoDataCon parent;

        private IList<MinoDataCon> collection;
        // public IList<MinoData> collection; // 这个改得很不好，再考虑一下
 
        public MinoDataCollectionCon(TetrominoDataCon parent) {
            this.parent = parent;
            this.collection = new List<MinoDataCon>();
        }
 
        public MinoDataCollectionCon(TetrominoDataCon parent, IList<MinoDataCon> collection) {
            this.parent = parent;
            this.collection = collection;
        }
 
#region IList<MinoData> Members
        public int IndexOf(MinoDataCon item) {
            return collection.IndexOf(item);
        }
        public void Insert(int index, MinoDataCon item) {
            if (item != null)
                item.Parent = parent; // 每个子控件,设置它的父控件
            collection.Insert(index, item);
        }
        public void RemoveAt(int index) {
			MinoDataCon oldItem = collection[index];
            collection.RemoveAt(index);
            if (oldItem != null)
                oldItem.Parent = null;
        }
        public MinoDataCon this[int index] {
            get {
                return collection[index];
            }
            set {
				MinoDataCon oldItem = collection[index];
                if (value != null)
                    value.Parent = parent;
                collection[index] = value;
                if (oldItem != null)
                    oldItem.Parent = null;
            }
        }
        // public MinoData ForEach() { } // ???
#endregion
 
#region ICollection<MinoData> Members
        public void Add(MinoDataCon item) {
            if (item != null)
                item.Parent = parent;
            collection.Add(item);
        }
        public void Clear() {
            foreach (MinoDataCon item in collection) {
                if (item != null)
                    item.Parent = null;
            }
            collection.Clear();
        }
        public bool Contains(MinoDataCon item) {
            return collection.Contains(item);
        }
        public void CopyTo(MinoDataCon[] array, int arrayIndex) {
            collection.CopyTo(array, arrayIndex);
        }
        public int Count {
            get { return collection.Count; }
        }
        public bool IsReadOnly {
            get { return collection.IsReadOnly; }
        }

		MinoDataCon IList<MinoDataCon>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool Remove(MinoDataCon item) {
            bool b = collection.Remove(item);
            if (item != null)
                item.Parent = null;
            return b;
        }
#endregion
 
#region IEnumerable<MinoData> Members
        public IEnumerator<MinoDataCon> GetEnumerator() {
            return collection.GetEnumerator();
        }
#endregion
 
#region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return (collection as System.Collections.IEnumerable).GetEnumerator();
        }

		//public int IndexOf(MinoDataCon item) => throw new NotImplementedException();
		//public void Insert(int index, MinoDataCon item) => throw new NotImplementedException();
		//public void Add(MinoDataCon item) => throw new NotImplementedException();
		//public bool Contains(MinoDataCon item) => throw new NotImplementedException();
		//public void CopyTo(MinoDataCon[] array, int arrayIndex) => throw new NotImplementedException();
		//public bool Remove(MinoDataCon item) => throw new NotImplementedException();
		IEnumerator<MinoDataCon> IEnumerable<MinoDataCon>.GetEnumerator() => throw new NotImplementedException();
		#endregion

	}
}