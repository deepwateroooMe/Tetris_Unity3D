using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Util {

    public class AndroidTools {
        private const string TAG = "AndroidTools";

        private static AndroidJavaObject _UnityActivity = null;
        // 获取当前App的Activity
        public static AndroidJavaObject UnityActivity {
            get {
                if (_UnityActivity == null) {
                    _UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _UnityActivity;
            }
        }

        private static AndroidJavaObject _UnityAppContext = null;
        // 获取当前App的Context
        public static AndroidJavaObject UnityAppContext {
            get {
                if (_UnityAppContext == null) {
                    _UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");
                }
                return _UnityAppContext;
            }
        }
// 最简单的两行代码的写法
        // AndroidJavaObject UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        // AndroidJavaObject UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");        
    }
}
