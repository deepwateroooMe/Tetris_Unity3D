using System.Collections;
using System.Collections.Generic;
// using System.Collections.Generic;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]
    public class MinoDataCollection<TetrominoData, MinoData> : IList<MinoData>
    where TetrominoData : class
    where MinoData : IMinoDataItem<TetrominoData> {

        private TetrominoData parent;
    
        // private IList<MinoData> collection;
        public IList<MinoData> collection; // 这个改得很不好，再考虑一下
 
        public MinoDataCollection(TetrominoData parent) {
            this.parent = parent;
            this.collection = new List<MinoData>();
        }
 
        public MinoDataCollection(TetrominoData parent, IList<MinoData> collection) {
            this.parent = parent;
            this.collection = collection;
        }
 
#region IList<MinoData> Members
        public int IndexOf(MinoData item) {
            return collection.IndexOf(item);
        }
        public void Insert(int index, MinoData item) {
            if (item != null)
                item.Parent = parent;
            collection.Insert(index, item);
        }
        public void RemoveAt(int index) {
            MinoData oldItem = collection[index];
            collection.RemoveAt(index);
            if (oldItem != null)
                oldItem.Parent = null;
        }
        public MinoData this[int index] {
            get {
                return collection[index];
            }
            set {
                MinoData oldItem = collection[index];
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
        public void Add(MinoData item) {
            if (item != null)
                item.Parent = parent;
            collection.Add(item);
        }
        public void Clear() {
            foreach (MinoData item in collection) {
                if (item != null)
                    item.Parent = null;
            }
            collection.Clear();
        }
        public bool Contains(MinoData item) {
            return collection.Contains(item);
        }
        public void CopyTo(MinoData[] array, int arrayIndex) {
            collection.CopyTo(array, arrayIndex);
        }
        public int Count {
            get { return collection.Count; }
        }
        public bool IsReadOnly {
            get { return collection.IsReadOnly; }
        }
        public bool Remove(MinoData item) {
            bool b = collection.Remove(item);
            if (item != null)
                item.Parent = null;
            return b;
        }
#endregion
 
#region IEnumerable<MinoData> Members
        public IEnumerator<MinoData> GetEnumerator() {
            return collection.GetEnumerator();
        }
#endregion
 
#region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return (collection as System.Collections.IEnumerable).GetEnumerator();
        }
#endregion
    
    }
}