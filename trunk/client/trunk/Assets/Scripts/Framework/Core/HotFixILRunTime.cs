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

    public class HotFixILRunTime : SingletonMono<HotFixILRunTime>, IHotFixMain {
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
            // appDomain.DelegateManager.RegisterMethodDelegate<TapGesture>(); // 暂时不理会触屏手势
            // appDomain.DelegateManager.RegisterMethodDelegate<LongPressGesture>();
            // appDomain.DelegateManager.RegisterMethodDelegate<DragGesture>();
            // appDomain.DelegateManager.RegisterMethodDelegate<PinchGesture>();
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
            // appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<TapGesture>.GestureEventHandler>((action) => {
            //     return new GestureRecognizerTS<TapGesture>.GestureEventHandler((gesture) => {
            //         ((Action<TapGesture>)action)(gesture);
            //     });
            // });
            // appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<LongPressGesture>.GestureEventHandler>((action) => {
            //     return new GestureRecognizerTS<LongPressGesture>.GestureEventHandler((gesture) => {
            //         ((Action<LongPressGesture>)action)(gesture);
            //     });
            // });
            // appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<DragGesture>.GestureEventHandler>((action) => {
            //     return new GestureRecognizerTS<DragGesture>.GestureEventHandler((gesture) => {
            //         ((Action<DragGesture>)action)(gesture);
            //     });
            // });
            // appDomain.DelegateManager.RegisterDelegateConvertor<GestureRecognizerTS<PinchGesture>.GestureEventHandler>((action) => {
            //     return new GestureRecognizerTS<PinchGesture>.GestureEventHandler((gesture) => {
            //         ((Action<PinchGesture>)action)(gesture);
            //     });
            // });

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
                    appDomain.RegisterCLRMethodRedirection(i, CreateInstance); // 方法重定向 
            }
        }
        void InitializeAdapterSetting() {
            appDomain.RegisterCrossBindingAdaptor(new ViewModelBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new UnityGuiViewAdapter());
            appDomain.RegisterCrossBindingAdaptor(new ModuleBaseAdapter());
            appDomain.RegisterCrossBindingAdaptor(new IEnumeratorObjectAdaptor());
            appDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            appDomain.RegisterCrossBindingAdaptor(new InterfaceCrossBindingAdaptor()); // <<<<<<<<<<<<<<<<<<<< 
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
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            if (genericArguments != null && genericArguments.Length == 1) {
                var t = genericArguments[0];
                if (t is ILType)//如果T是热更DLL里的类型 
                    //通过ILRuntime的接口来创建实例
                    return ILIntepreter.PushObject(esp, mStack, ((ILType)t).Instantiate());
                else
                    return ILIntepreter.PushObject(esp, mStack, Activator.CreateInstance(t.TypeForCLR));//通过系统反射接口创建实例
            } else
                throw new EntryPointNotFoundException();
        }
    }
}