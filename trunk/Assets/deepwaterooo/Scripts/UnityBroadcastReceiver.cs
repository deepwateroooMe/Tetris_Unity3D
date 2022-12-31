using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.deepwaterooo.Scripts {
    public class UnityBroadcastReceiver { // 它的生命周期会不会也狠短
        private const string TAG = "UnityBroadcastReceiver";

        public static UnityBroadcastReceiver instance = (UnityBroadcastReceiver)Activator.CreateInstance(typeof(UnityBroadcastReceiver));
        private UnityBroadcastProxy broadcastReceiverInterface;
        private AndroidJavaObject UABroadcastReceiver; // <<<<<<<<<< 

        public UnityBroadcastReceiver() {
            Debug.Log(TAG + " UnityBroadcastReceiver() consttructor");
            UABroadcastReceiver = new AndroidJavaObject("com.deepwaterooo.UABroadcastReceiver"); // <<<<<<<<<< 
        }

        public void initBroadcast() {
            Debug.Log(TAG + " initBroadcast()");
            broadcastReceiverInterface = new UnityBroadcastProxy();
            broadcastReceiverInterface.onReceiveDelegate = onBroadcastReceive; // 设置代理
            UABroadcastReceiver.Call("setReceiver", broadcastReceiverInterface);
            try {
                AndroidJavaObject recevierFilter = new AndroidJavaObject("android.content.IntentFilter");
 
                recevierFilter.Call("addAction", "android.media.VOLUME_CHANGED_ACTION"); // 监听手机硬件调控音量的变化
                // recevierFilter.Call("addAction", "android.intent.action.PACKAGE_REMOVED");
                // recevierFilter.Call("addAction", "android.intent.action.PACKAGE_ADDED");
                // recevierFilter.Call("addAction", "android.intent.action.PACKAGE_REPLACED");
                // recevierFilter.Call("addDataScheme", "package");
// 这里极有可能会错: 判断非空, 是为游戏端动态注册安卓广播接收器 拿到必要的 上下文环境
                AndroidJavaObject _UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                Debug.Log(TAG + " (_UnityActivity != null): " + (_UnityActivity != null));

                _UnityActivity.Call<AndroidJavaObject>("registerReceiver", UABroadcastReceiver, recevierFilter);
            }
            catch (Exception e) {
                // AndroidTools.AndroidLogI(e.Message.ToString());
                throw;
            }
        }
// 需要一个本地存储,来缓存接收到的音量的变化,保持最新最更新的状态
        void onBroadcastReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            Debug.Log(TAG + " onBroadcastReceive()");
            AndroidJavaObject action = intent.Call<AndroidJavaObject>("getAction"); // <<<<<<<<<< 
            if (action != null) {
                Debug.Log(TAG + "接收action：" + action.ToString());                
                if (action.ToString().Equals("selfDefineReceiver")) { // <<<<<<<<<< 这里的逻辑转变,还没有想得狠清楚 
// TODO 这里自己写相应，上面一句可删除
                    string iaction = intent.Call<string>("getAction");
                    //string extra = intent.Call<int>("getIntExtra", "android.media.EXTRA_VOLUME_STREAM_TYPE", -1); // 不知道这里调用会不会对
                    Debug.Log(TAG + " iaction: " + iaction);
                    if (iaction != null && iaction.Equals("android.media.VOLUME_CHANGED_ACTION")) {
                        // && extra == AudioManager.STREAM_MUSIC) // 这个判断,可能会多余,也拿不到安卓平台的相关设置值
                        // TODO:　取出和存储必要的数据
                        Debug.Log(TAG + " onBroadcastReceive() TODO SOMETHING HERE");
                    }
                }
            }
        }
    }
}
