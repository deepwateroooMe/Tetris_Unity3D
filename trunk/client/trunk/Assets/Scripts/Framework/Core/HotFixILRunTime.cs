using Framework.MVVM;
using Framework.ResMgr;
using Framework.Util;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Core {

    // public class HotFixILRunTime : SingletonMono<HotFixILRunTime>, IHotFixMain {
// 现在改得丑一点儿就让它丑一点儿,先把它弄运行了再说,把它改成同样例一样   
    public class HotFixILRunTime : MonoBehaviour, IHotFixMain { 

// 现在改得丑一点儿就让它丑一点儿,先把它弄运行了再说,把它改成同样例一样   
        static HotFixILRunTime instance;

        private const string TAG = "HotFixILRunTime"; 

        public static ILRuntime.Runtime.Enviorment.AppDomain appDomain;

        public static HotFixILRunTime Instance {
            get {
                if (instance == null) {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<HotFixILRunTime>();
                    obj.name = instance.GetType().Name;
                }
                return instance;
            }
        }
        void Start() {
            instance = this;

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

        // public void Update() {
	    //     DoStaticMethod("HotFix.HotFixMain", "Update"); // <<<<<<<<<<<<<<<<<<<< 
		// }
        
		void StartApplication() {
            InitializeILRunTimeHotFixSetting();
            DoStaticMethod("HotFix.HotFixMain", "Start");
// 暂时不测这个,会想办法解决问题            
// // 不知道这里会不会报错
//             InitializeCustomizedILTypes();
        }
        void InitializeILRunTimeHotFixSetting() {
            InitializeDelegateSetting();
            InitializeCLRBindSetting();
            InitializeAdapterSetting();
            InitializeValueTypeSetting();
        }
// // 自己模拟的测试, 不通.因为我把下面的自定义方法去掉了,所以这里commented for tmp
//         void InitializeCustomizedILTypes() { // 我感觉还是那两个类不被认识 ??
//             Debug.Log(TAG + " InitializeCustomizedILTypes");
//             var type = appDomain.LoadedTypes["HotFix.Control.Tetromino"] as ILType; // 这个没有问题,可以认识这个类了
//             //var type2 = appDomain.LoadedTypes["HotFix.Control.GhostTetromino"] as ILType;
// // 这里仍然是空,会报错,             
//             var smb = GetComponent(type);   // 这里说,现处unity主工程,劫持改造一个系统方法GetComponent,以方便去拿热更新工程中所定义的这两个类
//             //var smb2 = GetComponent(type2); // 这里说,现处unity主工程,劫持改造一个系统方法GetComponent,以方便去拿热更新工程中所定义的这两个类
//             Debug.Log(TAG + " (type == null): " + (type == null));
//             Debug.Log(TAG + " (smb == null): " + (smb == null));
//             var method = type.GetMethod("Update");
// // 这里仍然是空,会报错            :　错会报在下面这一行
//             appDomain.Invoke(method, smb, null);
//         }

		void InitializeDelegateSetting() {
            appDomain.DelegateManager.RegisterMethodDelegate<int>();
            appDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<string>();
            appDomain.DelegateManager.RegisterMethodDelegate<int, int>();
// 感觉这一步的加虽然消除了一个运行时错误,但内存的运行效率有可能是降低了: 还是必要的,至少是它不再报错了            
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3, UnityEngine.Vector3>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Quaternion, UnityEngine.Quaternion>();
            appDomain.DelegateManager.RegisterMethodDelegate<List<int>, List<int>>();
            appDomain.DelegateManager.RegisterMethodDelegate<string, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, MessageArgs<object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, MessageArgs<ILTypeInstance>>();
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Networking.UnityWebRequest>();
            appDomain.DelegateManager.RegisterMethodDelegate<TMP_FontAsset>();
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
// 我们先销毁掉之前创建的不合法的MonoBehaviour, 这里是销毁之前创建的不合法的 ?
// 这里说,在主工程中消除游戏引擎反射系统        中的CreateInstance方法,去调用ILRuntime框架中自定义改造过的相应方法,以便劫持逻辑
        unsafe void InitializeCLRBindSetting() {
            Debug.Log(TAG + " InitializeCLRBindSetting");

            foreach (var i in typeof(System.Activator).GetMethods()) {
                // 找到名字为CreateInstance，并且是泛型方法的方法定义
// 我觉得这里只定义这一类的方法可能不够用,按照网上的把AddComponent<>() GetComponent<>()也都加上                
                if (i.Name == "CreateInstance" && i.IsGenericMethodDefinition) {
                    appDomain.RegisterCLRMethodRedirection(i, CreateInstance); // 方法重定向
// 因为没有用,所以需要去掉                
                    Debug.Log(TAG + " InitializeCLRBindSetting CreateInstance()");
                }
// // 这里想要顺手牵羊地顺承下来的代码,逻辑并不对,抄错了,两个方法没有注册成功                
//                 else if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1) {
// 	                appDomain.RegisterCLRMethodRedirection(i, AddComponent);
//                     Debug.Log(TAG + " InitializeCLRBindSetting AddComponent<>()");
//                 // }
//                 } else if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1) {
//                     appDomain.RegisterCLRMethodRedirection(i, GetComponent);
//                     Debug.Log(TAG + " InitializeCLRBindSetting GetComponent<ILType>()");
//                 }

                //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
                var arr = typeof(GameObject).GetMethods();
                foreach (var k in arr) {
                    if (k.Name == "AddComponent" && k.GetGenericArguments().Length == 1) {
                        appDomain.RegisterCLRMethodRedirection(k, AddComponent);
                        Debug.Log(TAG + " InitializeCLRBindSetting AddComponent<>()");
                    }
                }
                //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
                var arr2 = typeof(GameObject).GetMethods();
                foreach (var j in arr2) {
                    if (j.Name == "GetComponent" && j.GetGenericArguments().Length == 1) {
                        appDomain.RegisterCLRMethodRedirection(j, GetComponent);
                        Debug.Log(TAG + " InitializeCLRBindSetting GetComponent<ILType>()");
                    }
                }

            }
        }
        // unsafe void SetupCLRAddComponentRedirection() {
        //         //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        //         var arr = typeof(GameObject).GetMethods();
        //         foreach (var i in arr) {
        //             if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1) {
        //                 appdomain.RegisterCLRMethodRedirection(i, AddComponent);
        //             }
        //         }
        //     }
        // unsafe void SetupCLRGetComponentRedirection() {
        //         //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        //         var arr = typeof(GameObject).GetMethods();
        //         foreach (var i in arr) {
        //             if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1) {
        //                 appdomain.RegisterCLRMethodRedirection(i, GetComponent);
        //             }
        //         }
        //     }


        void InitializeAdapterSetting() {
            appDomain.RegisterCrossBindingAdaptor(new ViewModelBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new UnityGuiViewAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ModuleBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IEnumeratorObjectAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new InterfaceCrossBindingAdaptor()); // <<<<<<<<<<<<<<<<<<<< 
            appDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        }
        void InitializeValueTypeSetting() {
            appDomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appDomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            appDomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        }

        object DoStaticMethod(string type, string method) {
            var hotfixType = appDomain.GetType(type);
            //IMethod staticMethod;
            //if (method.Equals("Start"))
            var staticMethod = hotfixType.GetMethod(method, 0);
            //else 
            //    staticMethod = hotfixType.GetMethod(method, 0);
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

// 刚才没有把问题想明白:因为经过了适配,本身的UnityEngine.AddComponent<T>() UnityEngine.GetComponent<T>() 在热更新工程中的正常运行是没有问题的
    // 出问题的特殊之处是在: Tetromini.cs GhostTetromino.cs是在热更新工程中定义的,当游戏运行,unity工程无法得知热更新工程中Tetromino.cs GhostTetromino.cs为何物
    // 上面说得不对,因为加component本身是在热更新工程中,它是知道自己工程中所定义的部件的
// 所以得想办法把这两个类移到Unity工程中来(这个反而可能会比较繁琐,也可能逻辑不通)
// 按照官方建议,我们是可以重置这两个方法的,让它有办法认得热更新工程中所定义的脚本(顺着这条途径把问题理顺,那么就发现别人的控件逻辑是在Unity主工程的,也就是有主工程中的MonoBehaviour系来驱动各生命周期事件,但是我的热更新控制逻辑是在热更新工程中,并没有一个默认的游戏引擎来驱动事件的自行发生)
// 所以,没有设置好的原因,另一个是在热更新工程中,我没有哪个地方来调用UNITY工程的系统的自动运行;
        // 前面的各种适配是适配给unity,让它认识热更新工程中的诸多类型函数等
        // 可是按照自己游戏逻辑,感觉更像是热更新工程中需要适配unity MonoBehaviour的生命周期事件 ?
        // 那么再回到上面,刚想过的
// 所以得想办法把这两个类移到Unity工程中来(这个反而可能会比较繁琐,也可能逻辑不通)
        // 那么这么试一下,倒还是有可能的,unity MonoBehaviour系能够自动驱动生命周期事件,引导必要时候游戏的进行 ??? 测试一下

// 示例工程中这些劫持是,代码适配用于提供给Unity工程来加载或是获取(AddComponent<>(), GetComponent<>())热更新工程中unity所不认识的定义的类等,与自己游戏逻辑不同,不用        
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

// 还可能需要再检查一下这里的控件设置      
        public unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            Debug.Log(TAG + " AddComponent<T>()");
            // CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // 成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            // GameObject instance = StackObject.ToObject(ptr, appDomain, __mStack) as GameObject;
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
            Debug.Log(TAG + " GetComponent<>() returns StackObject*");
            // CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // 成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            // GameObject instance = StackObject.ToObject(ptr, appDomain, __mStack) as GameObject;
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

