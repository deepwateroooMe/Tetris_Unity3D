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

// ?????????????????????????????????????????????????????????,?????????????????????????????????????????????: ???????????????,???????????????????????????
// ??????????????????????????????,??????????????????????????????,???????????????????????????GC Alloc
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
                // ???????????????CreateInstance???????????????????????????????????????
                if (i.Name == "CreateInstance" && i.IsGenericMethodDefinition) 
                    appDomain.RegisterCLRMethodRedirection(i, CreateInstance); // ???????????????,????????????????????????????????????
            }
// ??????????????????????????????????????????,???AddComponent<>() GetComponent<>()????????????                
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
		void StartApplication() {
            InitializeILRunTimeHotFixSetting();
            DoStaticMethod("HotFix.HotFixMain", "Start");
        }
// IHotFixMain ???????????????????????????         
#region Override
        public void startEducational() {
            Debug.Log(TAG + " startEducational");
            DoStaticMethod("HotFix.HotFixMain", "startEducational");
        }
        public void startClassical() {
            Debug.Log(TAG + " startClassical");
            DoStaticMethod("HotFix.HotFixMain", "startClassical");
        }
        public void startChallenging() {
            Debug.Log(TAG + " startChallenging");
            DoStaticMethod("HotFix.HotFixMain", "startChallenging");
        }
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

// ????????????:??????????????????????????????????????????????????????? ?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????        
        public unsafe static StackObject* CreateInstance(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj) {
            Debug.Log(TAG + " CreateInstance()");
            // ??????????????????<T>???????????????
            IType[] genericArguments = method.GenericArguments;
            if (genericArguments != null && genericArguments.Length == 1) {
                var t = genericArguments[0];
                if (t is ILType) // ??????T?????????DLL???????????? 
                    // ??????ILRuntime????????????????????????
                    return ILIntepreter.PushObject(esp, mStack, ((ILType)t).Instantiate());
                else
                    return ILIntepreter.PushObject(esp, mStack, Activator.CreateInstance(t.TypeForCLR)); // ????????????????????????????????????
            } else
                throw new EntryPointNotFoundException();
        }
// ??????????????????????????????,???????????????????????????Unity???????????????????????????(AddComponent<>(), GetComponent<>())??????????????????unity??????????????????????????????????????????????????????
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
            // CLR?????????????????????????????????????????????????????????????????????
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // ?????????????????????????????????this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);
            var genericArgument = __method.GenericArguments;
            // AddComponent??????????????????1???????????????
            if (genericArgument != null && genericArgument.Length == 1) {
                var type = genericArgument[0];
                object res;
                if(type is CLRType) {
                    // Unity?????????????????????????????????????????????????????????Unity??????
                    res = instance.AddComponent(type.TypeForCLR);
                } else {
                    // ??????DLL??????????????????????????????????????????????????????????????????
                    var ilInstance = new ILTypeInstance(type as ILType, false);// ??????????????????????????????????????????new MonoBehaviour?????????Unity????????????
                    // ???????????????Adapter??????
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    // unity??????????????????????????????DLL??????????????????????????????????????????
                    clrInstance.ILInstance = ilInstance;
                    // clrInstance.AppDomain = __domain;
                    clrInstance.AppDomain = appDomain;
                    // ???????????????????????????CLRInstance????????????AddComponent?????????????????????????????????????????????
                    ilInstance.CLRInstance = clrInstance;
                    res = clrInstance.ILInstance;// ??????ILRuntime??????????????????ILInstance
                    clrInstance.Awake();// ??????Unity????????????????????????????????????????????????????????????
                }
                return ILIntepreter.PushObject(ptr, __mStack, res);
            }
            return __esp;
        }
        public unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj) {
            // CLR?????????????????????????????????????????????????????????????????????
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            var ptr = __esp - 1;
            // ?????????????????????????????????this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);
            var genericArgument = __method.GenericArguments;
            // AddComponent??????????????????1???????????????
            if (genericArgument != null && genericArgument.Length == 1) {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType) {
                    // Unity?????????????????????????????????????????????????????????Unity??????
                    res = instance.GetComponent(type.TypeForCLR);
                } else {
                    // ????????????DLL?????????MonoBehaviour??????????????????Component?????????????????????????????????????????????
                    var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for(int i = 0; i < clrInstances.Length; i++) {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null) { // ILInstance???null, ??????????????????MonoBehaviour???????????? 
                            if (clrInstance.ILInstance.Type == type) {
                                res = clrInstance.ILInstance;// ??????ILRuntime??????????????????ILInstance
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