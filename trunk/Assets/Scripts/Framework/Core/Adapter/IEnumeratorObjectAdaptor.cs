using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;

public class IEnumeratorObjectAdaptor : CrossBindingAdaptor {

    // 实现基类的三个方法
    public override Type BaseCLRType {
        get {
            return typeof(IEnumerator<object>);
        }
    }
    public override Type AdaptorType {
        get {
            return typeof(Adaptor);
        }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adaptor(appdomain, instance);
    }

// IEnumerator是所有非泛型枚举器的基接口。换而言之就是IEnumerator定义了一种适用于任意集合的迭代方式
// 协程里yield关键字是一个迭代器，相当于实现了IEnumerator枚举器;所以这里的逻辑是连通起来的
    // public interface IEnumerator {
    //     object Current { get; }
    //     bool MoveNext();
    //     void Reset();
    // }
    public class Adaptor : IEnumerator<object>, CrossBindingAdaptorType {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor() {}
        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
            _get_Current = instance.Type.GetMethod(".get_Current", 0);
        }
        public ILTypeInstance ILInstance { get { return instance; } }
        
        public object Current { // 猎取当前元素
            get {
                var obj = appdomain.Invoke(_get_Current, null);
                return obj;
            }
        }
// 枚举器类型的四个迭代方法,好像仍然也?是协程中每个需要分布完成的小步骤        
        IMethod _MoveNext;
        IMethod _get_Current;
        IMethod _Reset;
        IMethod _Dispose;

        public bool MoveNext() {
            if (_MoveNext == null) 
                _MoveNext = instance.Type.GetMethod("MoveNext", 0);
            if (_MoveNext != null)
                return (bool)appdomain.Invoke(_MoveNext, instance);
            return false;
        }
        public void Reset() {
            if (_Reset == null) 
                _Reset = instance.Type.GetMethod("MoveNext", 0);
            if (_Reset != null)
                appdomain.Invoke(_Reset, instance);
        }
        public void Dispose() {
            if (_Dispose == null) 
                _Dispose = instance.Type.GetMethod("Dispose", 0);
            if (_Dispose != null)
                appdomain.Invoke(_Dispose, instance);
        }
    }
}