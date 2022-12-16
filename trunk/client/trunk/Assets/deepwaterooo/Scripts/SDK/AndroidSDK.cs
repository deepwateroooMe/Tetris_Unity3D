using UnityEngine;

namespace DWater {
    public class AndroidSDK : ISDK {
        private const string TAG = "AndroidSDK";

        private AndroidJavaClass _javaClassVariable = null;
        private AndroidJavaClass _androidSDK {
            get {
                Debug.Log(TAG + " _androidSDK()");
                if (_javaClassVariable == null) {
                    _javaClassVariable = new AndroidJavaClass("com.deepwaterooo.DWSDK"); 
                    Debug.Log(TAG + " (_javaClassVariable != null): " + (_javaClassVariable != null));
                }
                return _javaClassVariable;
            }
        }

// 公用方法:全部是unity 调用安卓SDK 中的方法        
        public void Init() {
            Debug.Log(TAG + " Init()");
            _androidSDK.CallStatic("Init");
        }

// 这个方法: 显示 SplashScreen, 可以用来测试这个调用的流程.现在把这些都想清楚了,其实很简单,直接相到调用就可以了,几乎可以不用再测试
// 游戏 调用 安卓SDK
        public void ShowLogin () { // ShowSplash()
            _androidSDK.CallStatic("StartSplashScreenActivity");
        }
        // public static void StartSplashScreenActivity() { // 这些是.jar中的桥接调用,写这里不对
        //     PlayerUtil.startSplashScreenActivity(DWUnityActivity.instance);
        // }
        public void Logout () {
            _androidSDK.CallStatic ("Logout");
        }
        public void sendMsg(string s) { // from unity to SDK
            Debug.Log(TAG + " sendMsg()");
            _androidSDK.CallStatic("showToast", s);
        }

        public int add(int x, int y) {
            Debug.Log(TAG + " add()");
            // return _androidSDK.CallStatic("add", x, y); // <<<<<<<<<<<<<<<<<<<< bug: 返回值不能这么传
            return 0;
        }
    }
}