using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

// Hotfix响应MonoBehaviour中的事件，可以用代理方法实现。
// 1.在Unity中新建一个Mono脚本，实现自己需要的接口，在这些接口被调用时，调用代理事件
// 2.在Hotfix工程中这样调用:
// MonoBehaviourEventTrigger monoTrigger = obj.AddComponent<MonoBehaviourEventTrigger>();

public class MonoBehaviourAdapter : CrossBindingAdaptor {
    private const String TAG = "MonoBehaviourAdapter";

// 三个公用接口类的公用方法调用     
    public override Type BaseCLRType {
        get {
            return typeof(MonoBehaviour);
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

    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType {

        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor() {}
        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }
        public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }
        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

// 根据自己游戏需要:可能需要实现 onEnable() onDestroy() 等相关方法,可以照葫芦画瓢地实现        
        IMethod mAwakeMethod;
        bool mAwakeMethodGot;
        public void Awake() {
            // Unity会在ILRuntime准备好这个实例前调用Awake，所以这里暂时先不掉用
            // Debug.Log(TAG + " Awake() (instance == null): " + (instance == null));
            if (instance != null) {
                if (!mAwakeMethodGot) {
                    mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                    mAwakeMethodGot = true;
                }
                if (mAwakeMethod != null) {
                    appdomain.Invoke(mAwakeMethod, instance, null);
                }
            }
        }
        IMethod mOnEnableMethod;
        bool mOnEnableMethodGot;
        public void OnEnable() {
            // Debug.Log(TAG + "  OnEnable() (instance == null): " + (instance == null));
            if (instance != null) {
                if (!mOnEnableMethodGot) {
                    mOnEnableMethod = instance.Type.GetMethod("OnEnable", 0);
                    mOnEnableMethodGot = true;
                }
                if (mStartMethod != null) {
                    appdomain.Invoke(mOnEnableMethod, instance, null);
                }
            }
        }

        IMethod mStartMethod;
        bool mStartMethodGot;
        public void Start() {
            // Debug.Log(TAG + "  Start() (instance == null): " + (instance == null));
            if (!mStartMethodGot) {
                mStartMethod = instance.Type.GetMethod("Start", 0);
                mStartMethodGot = true;
            }
            if (mStartMethod != null) {
                appdomain.Invoke(mStartMethod, instance, null);
            }
        }
        IMethod mUpdateMethod;
        bool mUpdateMethodGot;
        void Update() {
            // Debug.Log(TAG + "  Update() (instance == null): " + (instance == null));
            if (!mUpdateMethodGot) {
                mUpdateMethod = instance.Type.GetMethod("Update", 0);
                mUpdateMethodGot = true;
            }
            if (mStartMethod != null) {
                appdomain.Invoke(mUpdateMethod, instance, null);
            }
        }
        IMethod mOnDisableMethod;
        bool mOnDisableMethodGot;
        void OnDisable() {
            // Debug.Log(TAG + "  OnDisable() (instance == null): " + (instance == null));
            if (!mOnDisableMethodGot) {
                mOnDisableMethod = instance.Type.GetMethod("OnDisable", 0);
                mOnDisableMethodGot = true;
            }
            if (mStartMethod != null) {
                appdomain.Invoke(mOnDisableMethod, instance, null);
            }
        }

        IMethod mOnDestroyMethod;
        bool mOnDestroyMethodGot;
        void OnDestroy() {
            // Debug.Log(TAG + "  OnDestroy() (instance == null): " + (instance == null));
            if (!mOnDestroyMethodGot) {
                mOnDestroyMethod = instance.Type.GetMethod("OnDestroy", 0);
                mOnDestroyMethodGot = true;
            }
            if (mStartMethod != null) {
                appdomain.Invoke(mOnDestroyMethod, instance, null);
            }
        }

        public override string ToString() {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod) {
                return instance.ToString();
            } else
                return instance.Type.FullName;
        }
    }
}