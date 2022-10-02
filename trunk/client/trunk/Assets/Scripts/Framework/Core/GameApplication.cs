using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.ResMgr;
using Framework.Util;
using System.Json;

// using cn.sharesdk.unity3d;
namespace Framework.Core {
    // 入口类
    public class GameApplication : MonoBehaviour {
        
        private static GameApplication _instance;
        public static GameApplication Instance {
            get {
                return _instance;
            }
        }
        public IHotFixMain HotFix {
            get;
            set;
        }
        // 是否使用PDB调试信息
        public bool usePDB = false;
        // 是否使用ILRuntime模式热更新
        public bool useILRuntime = false;
        // 是否使用本地资源
        public bool useLocal = false;
        // 资源服务器路径
        public string webRoot = string.Empty;
        // 强制登录
        public bool forceLogin = false;
// 手指的触屏系统相关的逻辑晚点儿再补        
        // public ScreenRaycaster ScreenRaycaster {
        //     get;
        //     private set;
        // }
// google, hotmail, LinkedIn facebook etc        
        // public ShareSDK ShareSDK { 
        //     get;
        //     private set;
        // }
        void Awake() {
            _instance = this;
            // ScreenRaycaster = GameObject.Find("Gestures").GetComponent<ScreenRaycaster>();
            DontDestroyOnLoad(gameObject);
            // InitializeClientConfig();
            // InitializeSDKs();
            CoroutineHelper.StartCoroutine(Initialize());
#region TestSamples
            // FingerEventTemp.Instance.RegisterGestureEvents();
            // TestNTS.Instance.TestLinesAngle();
            // GeometryManager.Instance.Test();
#endregion
        }
        void InitializeClientConfig() {
            var str = FileHelp.ReadString("ClientConfig.txt");
            if (!string.IsNullOrEmpty(str)) {
                JsonObject jsonObject = JsonSerializer.Deserialize(str) as JsonObject;
                if (jsonObject != null) {
                    if (jsonObject.ContainsKey("usePDB")) 
                        usePDB = (bool)jsonObject["usePDB"];
                    if (jsonObject.ContainsKey("useILRuntime")) 
                        usePDB = (bool)jsonObject["useILRuntime"];
                    if (jsonObject.ContainsKey("useLocal")) 
                        usePDB = (bool)jsonObject["useLocal"];
                    if (jsonObject.ContainsKey("webRoot")) 
                        ResourceConstant.ResourceWebRoot = jsonObject["webRoot"].ToString();
                    if (jsonObject.ContainsKey("forceLogin")) 
                        forceLogin = (bool)jsonObject["forceLogin"];
                }
            }
        }
        void InitializeSDKs() {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
                InitializeShareSDK();
        }
        void InitializeShareSDK() {
            // ShareSDK = GetComponent<ShareSDK>();
            // ShareSDK.authHandler = AuthResultHandler;
            // ShareSDK.Authorize(PlatformType.WeChat);
        }
        // ShareSDK执行授权回调: 这里因为需要接入不同的SDK, 所以这里暂时再等一等再来实现
        // void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result) {
        //     if (state == ResponseState.Success) {
        //         Debug.Log("ShareSDK authorize success!");
        //     } else if (state == ResponseState.Fail) {
        //         Debug.Log("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        //     } else if (state == ResponseState.Cancel) {
        //         Debug.Log("cancel!");
        //     }
        // }
        IEnumerator Initialize() {
            ResourceMap resourceMap = gameObject.AddComponent<ResourceMap>();
            resourceMap.OnInitializeSuccess += StartHotFix;
            ResourceConstant.Loader = resourceMap;
            yield return new WaitForEndOfFrame();
        }
        public void StartHotFix() {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                HotFix = HotFixILRunTime.Instance;
            } else {
                if (useILRuntime) {
                    HotFix = HotFixILRunTime.Instance;
                } else {
                    HotFix = HotFixReflector.Instance;
                }
            }
        }
    }
}
