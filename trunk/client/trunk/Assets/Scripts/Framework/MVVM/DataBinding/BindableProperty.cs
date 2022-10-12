using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.MVVM {

    // ILRuntime支持使用泛型:使用泛型来实现数据驱动,数据的变化通知回调等
    // 提供三四个对外接口: setter/getter, 数据变化的对外发布通知,以及ToString()
    public class BindableProperty<T> { // T: 使用了泛型,当然也包括了TProperty

        public Action<T, T> OnValueChanged; // <<<<<<<<<<<<<<<<<<<< 对外接口

        private T _value;
        public T Value { // <<<<<<<<<<<<<<<<<<<< 
            get {
                return _value;
            }
            set { // 在最初赋值或是值发生改变的时候，设定逻辑，一定会自动触发广告天下：通知所有的订阅监听者，该可观察属性的值发生了变化　
                if (!Equals(_value, value)) {
                    T old = _value;
                    _value = value;
                    ValueChanged(old, _value); // <<<<<<<<<< 
                }
            }
        }
        void ValueChanged(T oldValue, T newValue) { // <<<<<<<<<< 
            if (OnValueChanged != null) 
                OnValueChanged(oldValue, newValue); // 
        }
        
        public override string ToString() { // <<<<<<<<<<<<<<<<<<<< 
            return (Value != null ? Value.ToString() : "null");
        }
    }
}

