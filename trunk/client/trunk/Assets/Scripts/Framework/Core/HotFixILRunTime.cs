using Framework.MVVM;
using Framework.ResMgr;
using Framework.Util;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Core {

    public class HotFixILRunTime : SingletonMono<HotFixILRunTime>, IHotFixMain {
        private const string TAG = "HotFixILRunTime"; 

        public static ILRuntime.Runtime.Enviorment.AppDomain appDomain;

        void Start() {
            appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if UNITY_EDITOR
            appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            TextAsset dllAsset = ResourceConstant.Loader.LoadAsset<TextAsset>("HotFix.dll", "HotFix.dll");
            var msDll = new System.IO.MemoryStream(dllAsset.bytes);
            if (GameApplication.Instance.usePDB) {
                ResourceConstant.Loader.LoadAssetAsyn<TextAsset>("HotFix.pdb", "HotFix.pdb", (pdbAsset) => {
                    var msPdb = new System.IO.MemoryStream(pdbAsset.bytes);
                    appDomain.LoadAssembly(msDll, msPdb, new Mono.Cecil.Mdb.MdbReaderProvider());
                    StartApplication();
                }, EAssetBundleUnloadLevel.ChangeSceneOver);
            } else {
                appDomain.LoadAssembly(msDll, null, new Mono.Cecil.Mdb.MdbReaderProvider());
                StartApplication();
            }
            // ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
        }
        
		void StartApplication() {
            InitializeILRunTimeHotFixSetting();
            DoStaticMethod("HotFix.HotFixMain", "Start");
        }

        void InitializeILRunTimeHotFixSetting() {
            InitializeDelegateSetting();
            InitializeCLRBindSetting();
            InitializeAdapterSetting();
            InitializeValueTypeSetting();
        }

		void InitializeDelegateSetting() {
            appDomain.DelegateManager.RegisterMethodDelegate<int>();
            appDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<string>();
            appDomain.DelegateManager.RegisterMethodDelegate<int, int>();

// 感觉这一步的加虽然消除了一个运行时错误,但内存的运行效率有可能是降低了: 还是必要的,至少是它不再报错了
// 参照官方一点儿的例子,改成下面的相对高效的,原理在于不再频繁地GC Alloc
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3, UnityEngine.Vector3>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform, UnityEngine.Transform>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Quaternion, UnityEngine.Quaternion>();
            appDomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Boolean>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.Dictionary<UnityEngine.GameObject, System.Boolean>, System.Collections.Generic.Dictionary<UnityEngine.GameObject, System.Boolean>>();
            
            appDomain.DelegateManager.RegisterMethodDelegate<List<int>, List<int>>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, MessageArgs<object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, MessageArgs<ILTypeInstance>>();
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Networking.UnityWebRequest>();
            //appDomain.DelegateManager.RegisterMethodDelegate<TMP_FontAsset>();
            appDomain.DelegateManager.RegisterMethodDelegate<Font>();
            appDomain.DelegateManager.RegisterMethodDelegate<AnimationClip>();
            appDomain.DelegateManager.RegisterMethodDelegate<AnimatorOverrideController>();
            appDomain.DelegateManager.RegisterMethodDelegate<RuntimeAnimatorController>();
            appDomain.DelegateManager.RegisterMethodDelegate<AudioClip>();
            appDomain.DelegateManager.RegisterMethodDelegate<Material>();
            appDomain.DelegateManager.RegisterMethodDelegate<TextAsset>();
            appDomain.DelegateManager.RegisterMethodDelegate<Sprite>();
            appDomain.DelegateManager.RegisterMethodDelegate<Texture2D>(); 
            appDomain.DelegateManager.RegisterMethodDelegate<TapGesture>(); 
            appDomain.DelegateManager.RegisterMethodDelegate<LongPressGesture>();
            appDomain.DelegateManager.RegisterMethodDelegate<DragGesture>();
            appDomain.DelegateManager.RegisterMethodDelegate<PinchGesture>();
            appDomain.DelegateManager.RegisterMethodDelegate<Exception>();
            appDomain.DelegateManager.RegisterFunctionDelegate<GameObject, GameObject>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, ILTypeInstance, int>();
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((action) => {
               return new UnityAction(() => {
                   ((Action)action)();
               });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<bool>>((action) => {
                return new UnityAction<bool>((b) => {
                    ((Action<bool>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<int>>((action) => {
                return new UnityAction<int>((b) => {
                    ((Action<int>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<long>>((action) => {
                return new UnityAction<long>((b) => {
                    ((Action<long>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<float>>((action) => {
                return new UnityAction<float>((b) => {
                    ((Action<float>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<string>>((action) => {
                return new UnityAction<string>((b) => {
                    ((Action<string>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<BaseEventData>>((action) => {
                return new UnityAction<BaseEventData>((b) => {
                    ((Action<BaseEventData>)action)(b);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<TapGesture>.GestureEventHandler>((action) => {
                return new GestureRecognizerTS<TapGesture>.GestureEventHandler((gesture) => {
                    ((Action<TapGesture>)action)(gesture);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<LongPressGesture>.GestureEventHandler>((action) => {
                return new GestureRecognizerTS<LongPressGesture>.GestureEventHandler((gesture) => {
                    ((Action<LongPressGesture>)action)(gesture);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<DragGesture>.GestureEventHandler>((action) => {
                return new GestureRecognizerTS<DragGesture>.GestureEventHandler((gesture) => {
                    ((Action<DragGesture>)action)(gesture);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<PinchGesture>.GestureEventHandler>((action) => {
                return new GestureRecognizerTS<PinchGesture>.GestureEventHandler((gesture) => {
                    ((Action<PinchGesture>)action)(gesture);
                });
            });

#if UNITY_IPHONE
            appDomain.DelegateManager.RegisterDelegateConvertor<com.mob.FinishedRecordEvent>((action) => {
                return new com.mob.FinishedRecordEvent((ex) => {
                    ((Action<Exception>)action)(ex);
                });
            });
#endif
            appDomain.DelegateManager.RegisterDelegateConvertor<Comparison<ILTypeInstance>>((action) => {
                return new Comparison<ILTypeInstance>((x, y) => {
                    return ((Func<ILTypeInstance, ILTypeInstance, System.Int32>)action)(x, y);
                });
            });
        }

        unsafe void InitializeCLRBindSetting() {
            foreach (var i in typeof(System.Activator).GetMethods()) {
                // 找到名字为CreateInstance，并且是泛型方法的方法定义
                if (i.Name == "CreateInstance" && i.IsGenericMethodDefinition) 
                    appDomain.RegisterCLRMethodRedirection(i, CreateInstance); // 方法重定向,到下面的自定义的方法中来
            }
// 这里只定义这一类的方法不够用,把AddComponent<>() GetComponent<>()也都加上                
            var arr = typeof(GameObject).GetMethods();
            foreach (var k in arr) {
                if (k.Name == "AddComponent" && k.GetGenericArguments().Length == 1) 
                    appDomain.RegisterCLRMethodRedirection(k, AddComponent);
                else if (k.Name == "GetComponent" && k.GetGenericArguments().Length == 1) 
                    appDomain.RegisterCLRMethodRedirection(k, GetComponent);
            }
        }

        void InitializeAdapterSetting() {
            appDomain.RegisterCrossBindingAdaptor(new ViewModelBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new UnityGuiViewAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ModuleBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IEnumeratorObjectAdaptor());
            // appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            appDomain.RegisterCrossBindingAdaptor(new InterfaceCrossBindingAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        }
        
        void InitializeValueTypeSetting() {
            appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        }

        object DoStaticMethod(string type, string method) {
            var hotfixType = appDomain.GetType(type);
            var staticMethod = hotfixType.GetMethod(method, 0);
            return appDomain.Invoke(staticMethod, null, null);
        }

// IHotFixMain 里的两个方法的实现         
#region Override
        public Type LoadType(string typeName) {
            if (appDomain.LoadedTypes.ContainsKey(typeName)) {
                return appDomain.LoadedTypes[typeName].ReflectionType;
            }
            return null;
        }
        public object CreateInstance(string typeName) {
            ILType type = (ILType)appDomain.LoadedTypes[typeName];
            var instance = type.Instantiate();
            return instance;
        }
#endregion

// 回想一下:热更新程序域里是如何使用这三个方法的? 那么就可以照葫芦画飘地定义类似的方法作为桥梁来调用安卓原生音量控制里的相关设置        
        public unsafe static StackObject* CreateInstance(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj) {
            Debug.Log(TAG + " CreateInstance()");
            // 获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            if (genericArguments != null && genericArguments.Length == 1) {
                var t = genericArguments[0];
                if (t is ILType) // 如果T是热更DLL里的类型 
                    // 通过ILRuntime的接口来创建实例
                    return ILIntepreter.PushObject(esp, mStack, ((ILType)t).Instantiate());
                else
                    return ILIntepreter.PushObject(esp, mStack, Activator.CreateInstance(t.TypeForCLR)); // 通过系统反射接口创建实例
            } else
                throw new EntryPointNotFoundException();
        }
// 示例工程中这些劫持是,代码适配用于提供给Unity工程来加载或是获取(AddComponent<>(), GetComponent<>())热更新工程中unity所不认识的热更新程序域里所定义的类等
        MonoBehaviourAdapter.Adaptor GetComponent(ILType type) {
            Debug.Log(TAG + " GetComponent<>()");
            var arr = GetComponents<MonoBehaviourAdapter.Adaptor>();
            for(int i = 0; i < arr.Length; i++) {
                var instance = arr[i];
                if (instance.ILInstance != null && instance.ILInstance.Type == type) 
                    return instance;
            }
            return null;
        }
        public unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            // CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // 成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);
            var genericArgument = __method.GenericArguments;
            // AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) {
                var type = genericArgument[0];
                object res;
                if(type is CLRType) {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.AddComponent(type.TypeForCLR);
                } else {
                    // 热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                    var ilInstance = new ILTypeInstance(type as ILType, false);// 手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                    // 接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    // unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    // clrInstance.AppDomain = __domain;
                    clrInstance.AppDomain = appDomain;
                    // 这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;
                    res = clrInstance.ILInstance;// 交给ILRuntime的实例应该为ILInstance
                    clrInstance.Awake();// 因为Unity调用这个方法时还没准备好所以这里补调一次
                }
                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }
        public unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            // CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // 成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);
            var genericArgument = __method.GenericArguments;
            // AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1) {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType) {
                    // Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.GetComponent(type.TypeForCLR);
                } else {
                    // 因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                    var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; i++) {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null) { // ILInstance为null, 表示是无效的MonoBehaviour，要略过 
                            if (clrInstance.ILInstance.Type == type) {
                                res = clrInstance.ILInstance;// 交给ILRuntime的实例应该为ILInstance
                                break;
                            }
                        }
                    }
                }
                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }
    }
}