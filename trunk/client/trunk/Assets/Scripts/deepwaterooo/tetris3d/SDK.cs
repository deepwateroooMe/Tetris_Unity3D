using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    // public class SDK : SingletonMono<SDK> {
    public class SDK : MonoBehaviour {
        private const string TAG = "SDK";

        private static SDK _instance;
#if UNITY_ANDROID // && !UNITY_EDITOR
        private AndroidJavaClass androidJavaClass;
        private AndroidJavaObject androidJavaObject;
#endif
        public static SDK Instance {
            get {
                if (Instance == null) {
                    _instance = new SDK();
                    return _instance;
                } else 
                    return _instance;
            }
        }
        private SDK() {
            Init();
        }

        public void Init() {
            Debug.Log(TAG + " Init()");
#if UNITY_ANDROID && !UNITY_EDITOR
            androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            Debug.Log(TAG + " (androidJavaClass != null): " + (androidJavaClass != null));
            Debug.Log(TAG + " (androidJavaObject != null): " + (androidJavaObject != null));
#endif
        }

// 获取设备ID
        public string GetDeviceID() {
#if UNITY_ANDROID && !UNITY_EDITOR
            string s = androidJavaObject.Call<string>("getDeviceId");// 这里是调用Android Activity中注册的接口方法，怎么注册下面会讲到
            Debug.Log(TAG + " GetDeviceID() s: " + s);
            return s;
#else
            return "123456";// 这里是写一个没有SDK的临时返回值，防止一些异常
#endif
        }

        public void ShowSDKLoginTips() {
            Debug.Log(TAG + " ShowSDKLoginTips()");
#if UNITY_ANDROID && !UNITY_EDITOR
            androidJavaObject.Call("showSDKLoginTips");// 这里是接口中的无返回值方法
#endif
        }
    }
}
