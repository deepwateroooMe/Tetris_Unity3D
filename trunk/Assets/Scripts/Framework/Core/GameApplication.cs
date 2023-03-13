using System.Collections;
using UnityEngine;
using Framework.ResMgr;
using Framework.Util;
using System.Json;
using deepwaterooo.tetris3d;
using UnityEngine.UI;
using DeepwateroooWang;
//using cn.sharesdk.unity3d;
//using cn.sharesdk.unity3d;
//using cn.sharesdk.unity3d;

// using cn.sharesdk.unity3d;
namespace Framework.Core {
    // 入口类
    public class GameApplication : MonoBehaviour { // 
        private const string TAG = "GameApplication"; 

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

        // private GameObject menu;
        // private Button settingBtn; // SettingsView toggle Button
        // private GameObject settingsView;
        
// 手指的触屏系统相关的逻辑晚点儿再补: 当分处在两个不同的程序域，热更新程序域里是无法检测到用户的点击事件的，
// 所以这个手势识别库包用来判定视图上点击触摸事件的程序包必须得包装给热更新程序域使用
        public ScreenRaycaster ScreenRaycaster { // 这个包裹里还有两个没能适配的小BUG,可能会出问题，到时再解决　Handle.CircleCap() ==> Handle.CircleHandleCap()
            get;
            private set;
        }
        
// google, hotmail, LinkedIn facebook etc        
        //public ShareSDK ShareSDK { // 这只是一个微信登录的 单微信 第三方接入的库
        //    get;
        //    private set; 
        //}
        // void Awake() {
        void Start() {
            Debug.Log(TAG + " Start()");
            _instance = this;
            
            // settingBtn = GameObject.Find("settingBtn").GetComponent<Button>();
            // settingBtn.onClick.AddListener(onSettingBtnClicked);
            // settingsView = GameObject.Find("SettingsView");
            // settingsView.SetActive(false);

            // menu = GameObject.Find("menu");
            // menu = GameObject.Find("MenuViewPanel");
            // Debug.Log(TAG + " (menu != null): " + (menu != null));
// 这里相当于是自己实现了射线检测，是否点击中某个UI上控件的按钮，比如最开始第一屏的“开始游戏”等。＝＝＞　去追到这个按钮的回调过程            
// 这里有点儿没有弄明白，这个的启动过程和起作用的过程细节是什么样的？？？
            ScreenRaycaster = GameObject.Find("Gestures").GetComponent<ScreenRaycaster>();
            DontDestroyOnLoad(gameObject);
            // InitializeClientConfig();
            //InitializeSDKs(); // 可是这里仍然只是多一步登录的步骤,并不该影响热更新域的加载,这是可能会涉及到一点儿服务端的授权许可热更新域的加载?
            // menu.SetActive(false);
            CoroutineHelper.StartCoroutine(Initialize());
#region TestSamples
            // FingerEventTemp.Instance.RegisterGestureEvents();
            // TestNTS.Instance.TestLinesAngle();
            // GeometryManager.Instance.Test();
#endregion
        }
        // void onSettingBtnClicked() {
        //     Debug.Log(TAG + " onSettingBtnClicked()");
        //     settingsView.SetActive(true);
        // }
         void InitializeClientConfig() {
            var str = FileHelp.ReadString("ClientConfig.txt"); // 这此是写在用户手机的配置文件里的
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
        //void InitializeSDKs() {
        //    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
        //        InitializeShareSDK();
        //}
        //void InitializeShareSDK() {
        //    ShareSDK = GetComponent<ShareSDK>();
        //    ShareSDK.authHandler = AuthResultHandler;
        //    ShareSDK.Authorize(PlatformType.WeChat);
        //}
        //// ShareSDK执行授权回调: 这里因为需要接入不同的SDK, 所以这里暂时再等一等再来实现
        //void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result) {
        //    if (state == ResponseState.Success) {
        //        Debug.Log("ShareSDK authorize success!");
        //    } else if (state == ResponseState.Fail) {
        //        Debug.Log("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        //    } else if (state == ResponseState.Cancel) {
        //        Debug.Log("cancel!");
        //    }
        //}

        IEnumerator Initialize() {
// // 与安卓SDK桥接层的初始化: Unity SDK 与 上层 游戏端的最底层【这一块儿的逻辑，我暂不考虑了】
//             Deepwaterooo.instance.Initialize();
//             Deepwaterooo.instance.InitializeDW(); // 游戏加载的时候,就要调用加载游戏数据库.要求与安卓SDK桥接层也初始化好

            ResourceMap resourceMap = gameObject.AddComponent<ResourceMap>();
// 这个脚本的添加过程，也是热更新资源的管理更新过程,和程序资源的加载启动？
// 过程，去看这个资源管理类的加载资源细节过程，什么时候结束的，结束了才调用热更新程序集的启动            
            resourceMap.OnInitializeSuccess += StartHotFix;

            ResourceConstant.Loader = resourceMap;
            yield return new WaitForEndOfFrame();
        }
        public void StartHotFix() {
            Debug.Log(TAG + " StartHotFix()");
            // menu.SetActive(true); // 为了点一个按钮测试一下【把这个步骤跳过，直接进入热更新程序域里去】
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                HotFix = HotFixILRunTime.Instance;
            } else {
                if (useILRuntime) {
                    HotFix = HotFixILRunTime.Instance; // <<<<<<<<<< 
                } else {
                    HotFix = HotFixReflector.Instance;
                }
            }
        }
    }
}