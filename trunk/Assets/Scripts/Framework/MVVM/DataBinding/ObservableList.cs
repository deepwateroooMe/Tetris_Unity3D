using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework.MVVM {

    // 可观察结构集合的泛型管理类:采用集合链表进行封装,泛型类,需要实现集合类链表所有必要相关的方法
    public class ObservableList<T> : IList<T> {

        public Action<List<T>, List<T>> OnValueChanged;
        public Action<T> OnAdd;
        public Action<int, T> OnInsert;
        public Action<T> OnRemove;

        private List<T> _value = new List<T>();
        public List<T> Value {
            get {
                return _value;
            }
            set {
                if (!Equals(_value, value)) {
                    var old = _value;
                    _value = value;
                    ValueChanged(old, _value);
                }
            }
        }

        void ValueChanged(List<T> oldValue, List<T> newValue) {
            if (OnValueChanged != null) 
                OnValueChanged(oldValue, newValue);
        }

        public IEnumerator<T> GetEnumerator() {
            return _value.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(T item) {
            _value.Add(item);
            if (OnAdd != null) 
                OnAdd(item);
        }
        public void Clear() {
            _value.Clear();
        }
        public bool Contains(T item) {
            return _value.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            _value.CopyTo(array, arrayIndex);
        }
        public bool Remove(T item) {
            if (_value.Remove(item)) {
                if (OnRemove != null) 
                    OnRemove(item);
                return true;
            }
            return false;
        }
        public int Count {
            get {
                return _value.Count;
            }
        }
        public bool IsReadOnly {
            get;
            private set;
        }
        public int IndexOf(T item) {
            return _value.IndexOf(item);
        }
        public void Insert(int index, T item) {
            _value.Insert(index, item);
            if (OnInsert != null) {
                OnInsert(index, item);
            }
        }
        public void RemoveAt(int index) {
            _value.RemoveAt(index);
        }

        public T this[int index] {
            get {
                return _value[index];
            }
            set {
                _value[index] = value;
            }
        }
    }
}



