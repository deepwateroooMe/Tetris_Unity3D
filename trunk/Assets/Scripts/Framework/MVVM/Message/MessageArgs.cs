using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.MVVM {

    public class MessageArgs<T> {
        public T Item {
            get;
            private set;
        }

        public MessageArgs(T item) {
            Item = item;
        }
    }
}
