using Assets.deepwaterooo.Scripts;
using Framework.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// using DWInterface = DWater.Deepwaterooo; // 这里是自己之前理解错的,是游戏相对底层方便调用这个现最底层的时候的引用 

// 这个是游戏端最底层,与安卓SDK 直接交互的相关类,安卓SDK与这里面的方法可以直接相互调用
// 再往上,可以 再封一个游戏端的最底层,但是目前测试,对我的项目来说,可以略过

// 那么这个设计就是说: 游戏端分为两部分,与安卓SDK相交互的,不可热更新的一部分,常驻游戏unity域,和可以热更新的游戏逻辑视图域
// 将原热更新域中的SettingsView移至游戏unity域中去,通过游戏端的一个常驻按钮settingsBtn,显示设置界面

// 将这个命名空间设计为:将来任何自己游戏的最底端,负责与自己安卓SDK交互, 不要牵涉上层游戏逻辑,只为上层提供必要的接口
namespace DWater {

// 源项目包装得太多了,分两个不同的文件夹来包装的,一个是SDK向游戏端传,一个是游戏端调用安卓SDK,这里暂时不深入去看这些了    
    public class Deepwaterooo : MonoBehaviour { // 感觉这个脚本也是需要加到某个控件上的
        private const string TAG = "Deepwaterooo";
        private static readonly string GO_NAME = "Deepwaterooo"; 

        private static Deepwaterooo _instance; // 游戏应用全局单例模式
        public static Deepwaterooo Instance {
            get {
                Debug.Log(TAG + " Instance get");
                if (_instance == null) {
                    _instance = FindObjectOfType<Deepwaterooo>();
                    if (_instance == null)
                        _instance = new GameObject().AddComponent<Deepwaterooo>();
                }
                return _instance;
            }
        }

        private ISDK _sdkCalls;
        bool _isInitialized;
        // DWInterface _dw; // 这些,这里不必要的桥接,都可以删除掉

        // paused is true when the user has oppened one of the square panda screens while in the game screen
        public bool _paused = false;
        
        // Occurs when sdk screen is oppened.
        public event UnityAction pause; // 好像这个游戏没有订阅这个事件的,再找找看
        // Occurs when sdkscreen is closed.
        public event UnityAction unpause; // 上层封装会有订阅

        void Awake() {
            Debug.Log(TAG + " Awake()");
            if (_instance != null)
                Destroy(gameObject);
            _instance = this;
            _isInitialized = false;
            gameObject.name = GO_NAME;
            DontDestroyOnLoad(gameObject);
// 分不同的平台来启动        
#if UNITY_EDITOR
            _sdkCalls = new EditorSDK();
#elif UNITY_ANDROID
            _sdkCalls = new AndroidSDK();
// #elif UNITY_IOS
//         _sdkCalls = new IOSSDK();
#endif
            Debug.Log("[Deepwaterooo] Init()");
            _sdkCalls.Init(); // <<<<<<<<<<<<<<<<<<<< 这里需要一个初始化: SplashScreen可以在SDK中定制

// TODO: 测试unity游戏端向安卓SDK发送 或 接收广播, 游戏端的广播代理 也需要 初始化
            curVol.Value = 0;
            Debug.Log(TAG + "initBroadcast  ===============================");
            UnityBroadcastReceiver.instance.initBroadcast(); // 广播接收器的初始化
        }
        public BindableProperty<int> curVol = new BindableProperty<int>();
// 游戏向安卓发广播方法样例: 也是可以发广播的!!!
        public void SendBroadcastWithArgs() {
            Debug.Log(TAG + " SendBroadcastWithArgs()");
            SendBroadcastWithArgs("selfDefineReceiver", Application.identifier, "com.unity3d.player.UABroadcastReceiver");
        }
        void SendBroadcastWithArgs(string actionName, string packageName, string broadcastName) {
            Debug.Log(TAG + " SendBroadcastWithArgs() actionName: " + actionName + " packageName: " + packageName + " broadcastName: " + broadcastName);
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", actionName);
            intentObject.Call<AndroidJavaObject>("putExtra", "enable", false);
            intentObject.Call<AndroidJavaObject>("putExtra", "enable2", true);
            intentObject.Call<AndroidJavaObject>("putExtra", "mystring", "mystring args");
            double[] data = { 4.3f, 45, -78, 10000, 89 };
            intentObject.Call<AndroidJavaObject>("putExtra", "datas", data);
            AndroidJavaObject componentNameJO = new AndroidJavaObject("android.content.ComponentName", packageName, broadcastName);
            intentObject.Call<AndroidJavaObject>("setComponent", componentNameJO);
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = unity.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
            context.Call("sendBroadcast", intentObject);
        }

// This must be called first to initialize the plugin.
        public void Initialize() {
            Debug.Log("[Deepwterooo] Initialize()");
            if (!_isInitialized) {
                _isInitialized = true;
                Debug.Log("[Deepwterooo] initialized");
            }
        }

        private void OnDestroy() {
            _sdkCalls = null;
        }

        
        // this gets called when an sdk screen is closed
        // will set _paused to false if it wasnt already paused
        private void _onSDKScreenClose() {
#if UNITY_EDITOR
            return;
#endif
            _paused = false;
            Debug.Log("sdk is unpausing");
            if (unpause != null) // 通知游戏上层,恢复游戏
                unpause();
        }
        // called when an sdkscreen is opened
        // sets _paused to true
        public void _onSDKScreenOpen() { // 调控游戏端: SDK打开了,暂停游戏
#if UNITY_EDITOR
            return;
#endif
            _paused = true;
            if (pause != null) // 通知游戏上层,暂停游戏
                pause();
        }
        
        // shows the login screen
        public void dwShowLogin() {
            Debug.Log("[Deepwaterooo] dwShowLogin()");
            _sdkCalls.ShowLogin();
        }
        // logs the user out. This will always show the parent lock screen and log out if successful
        public void dwLogout() {
#if UNITY_ANDROID
            Debug.Log("[Deepwaterooo] spLogOut()");
            _sdkCalls.Logout(); // 从安卓SDK 里去登出.游戏端用unity 第三方库登出与安卓SDK端安卓第三方库登出,有哪上结性能差异?
// #else
//             spGetUnlockPermission (logoutCallback); // 感觉像是iOS
#endif
        }
        // the logout callback: 这里是iOS端吗 ?
        private void logoutCallback(bool b) {
            if (b) {
                _onSDKScreenOpen();
                _sdkCalls.Logout();
            }
        }

        private void SendMsg() {
            Debug.Log(TAG + " SendMsg()");
            // _sdkCalls.sendMsg("我是 unity ===");
            AndroidJavaObject helper = new AndroidJavaObject("pers.study.android2unity.Helper");
            helper.CallStatic("getMessageFormUnity", "我是 unity ===");
        }

        // Unity 监听手机按返系统回键
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                //showToast();
                Application.Quit();
            }
        }

        private void onAdd(string v) { // 这里需要再馐一下,把SDK拿到的结果传给游戏端
            Debug.Log(TAG + " onAdd()");
        }
         
        public void sendMsg(string s) {
            Debug.Log(TAG + " sendMsg()");
            _sdkCalls.sendMsg(s);
        }

// 游戏上层传过来的,继续往下调用安卓SDK中的方法,桥接过程
        // opens the player manager screen
        public void dwManagePlayers() {
            _onSDKScreenOpen();
            Debug.Log("[Deepwaterooo] dwManagePlayers()");
            _sdkCalls.ManagePlayers();
        }
    }
}
