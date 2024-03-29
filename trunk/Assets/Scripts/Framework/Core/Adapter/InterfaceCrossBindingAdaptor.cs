﻿using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;

public class InterfaceCrossBindingAdaptor : CrossBindingAdaptor {

    // 因为是interface,只需要实现基类的三个方法就可以了吗?只有一个地方用到,HotFixILRunTime.cs里热更新程序域启动的时候会用到去找
    // 这个类好像继承定义得太宽泛,完全没有起到什么作用,需要更为具体的继承定义
    public override Type BaseCLRType {
        get {
            return typeof(IEnumerator);
        }
    }
    public override Type AdaptorType {
        get {
            return typeof(IEnumeratorObjectAdaptor.Adaptor);
        }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain,
                                             ILTypeInstance instance) {
        return new IEnumeratorObjectAdaptor.Adaptor(appdomain, instance);
    }
}
