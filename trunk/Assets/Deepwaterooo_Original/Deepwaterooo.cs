using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DWInterface = DWater.Deepwaterooo;

namespace DeepwateroooWang {

// unity游戏端(除与安卓SDK交接的桥接层之外,besides)的最底层,相对上层
// 因为安卓SDK中的调用　的关系，我这里不能改变类名，先把命名空间改一下　？    
	public class Deepwaterooo : MonoBehaviour {
        private const string TAG = "DWUpper";

        bool _isInitialized;
        bool _hasActivity = false;
        DWInterface _dw; 

        public System.Action<string> onLoginSuccess;
        public System.Action<string> onSuccessLogoutEvent;
        public System.Action<string> onFinishSDKFlowEvent;
        // public System.Action<string> onFinishSDKUserConfigEvent;

        public static Deepwaterooo instance {
            get;
            private set;
        }
        void Awake() {
            instance = this; // <<<<<<<<<< 
            _isInitialized = false;
        }
// 暂时试一下:　把下面一二三个相互调用的地主连通,测试好
        public void InitializeDW() { // 初始化公司SDK: 方便游戏开始的时候初始化
            Debug.Log(TAG + " InitializeDW()");
            _dw = DWInterface.Instance; // 这里就指向了桥接层,要求进行必要的SDK的初始化相关工作
// 注册几个事件完成的回调, 连接了安卓SDK桥接层,游戏的最底层,和游戏的相对底层游戏公用接口层
            // _dw.OnUserLogin += onUserLogin;
            // _dw.OnUserLogout += OnSuccessLogout; // 这个事件,可能不是这么回调的
            // _dw.OnSettingsSaved += onSettingsSaved; // 如果写成事件,那么就是游戏视图层对某些事件感兴趣
// 那么问题变成是:怎么把unity游戏域里的事件传到热更新域? 网上应该有狠多解法,可是㿝搜一搜,我的项目终于也要用上这个了.....
            
// 安卓SDK流程结束,开始或是恢复游戏
            _dw.unpause += () => { OnFinishSDKFlow(string.Empty); }; 
            // _dw.unpause += () => { OnZPadFinishSDKFlow(string.Empty); }; // <<<<<<<<<< 底层封装回调到这里来
        }

#region Deepwaterooo
#region API
        // This must be called first to initialize the plugin.
        public void Initialize() {
            Debug.Log("[DWUpper] Initialize()"); // 全改成Deepwaterooo
            if (!_isInitialized) {
                _isInitialized = true;
                Debug.Log("[DWUpper] initialized");
            }
        }
        
        // Shows the child chooser and playset selection menus.
        public void DisplaySplash() { // <<<<<<<<<<<<<<<<<<<< 
            Debug.Log("[DWUpper] DisplaySplash()");
            _dw.dwShowLogin();
// #if UNITY_EDITOR
//             // OnDidGetProfileURL(instance._mockData.childInfo.profileImageUrlString);
// #endif
        }
        public bool IsInitialized() {
            Debug.Log("[DWUpper] IsInitialized()");
            return _dw != null;
            // return _dw != null && sql != null; // 两个非空,数据库也非空
        }
#endregion

        public System.Action<string> onFinishSDKUserConfigEvent;
#region CallBacks
// 下面几个主要是SDK 传回来给游戏端的必要的通知        
        private void OnDefaultLoginModeSuccesswithProfile(string message) { // 不是很明白这里说的是什么意思
            Debug.Log("[DWUpper] OnDefaultLoginModeSuccesswithProfile: " + message);
            if (onLoginSuccess != null) {
                onLoginSuccess(message);
            }
        }
        private void OnSuccessLogout(string message) {
            Debug.Log("[DWUpper] OnSuccessLogout: " + message);
            if (onSuccessLogoutEvent != null) {
                onSuccessLogoutEvent(message);
            }
        }
        private void OnFinishSDKFlow(string message) { // [OnZPadFinishSDKFlow, ==>　OnZPadConnected] 前 ==>　后
            Debug.Log("[DWUpper] OnZPadFinishSDKFlow: " + message);
            if (onFinishSDKFlowEvent != null) 
                onFinishSDKFlowEvent(message);
        }

        private void OnZPadFinishSDKFlow(string message) { // [OnZPadFinishSDKFlow, ==>　OnZPadConnected] 前 ==>　后
            // Since OnZPadConnected is triggered AFTER OnZPadFinishSDKFlow,
            // we need to delay this, so we can check if the device is really connected.
            StartCoroutine(OnZPadFinishSDKFlow_CR(message));
        }
        private IEnumerator OnZPadFinishSDKFlow_CR(string message) {
            yield return null; // 这个延迟一个桢, 真的有用吗?
            Debug.Log("[DWUpper] OnZPadFinishSDKFlow: " + message);
            if (onFinishSDKFlowEvent != null) {
                onFinishSDKFlowEvent(message);
            }
        }
        private void OnZPadFinishSDKUserConfig(string message) { // SDKActivity中传过来的通知: 用户完成登录了
            Debug.Log("[DWUpper] OnZPadFinishSDKUserConfig: " + message);
            if (onFinishSDKUserConfigEvent != null) {
                onFinishSDKUserConfigEvent(message);
            }
        }
 // 继承往下调用 安卓SDK的方法:
        public void OpenManagePlayers() {
            Debug.Log("[DWUpper] OpenManagePlayers");
            _dw.dwManagePlayers();
        }
        
#endregion
#endregion
	}
}
