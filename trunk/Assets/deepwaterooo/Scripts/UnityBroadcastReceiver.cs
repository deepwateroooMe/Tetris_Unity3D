using DWater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.deepwaterooo.Scripts {

    public class UnityBroadcastReceiver {
        private const string TAG = "UnityBroadcastReceiver";

        public static UnityBroadcastReceiver instance = (UnityBroadcastReceiver)Activator.CreateInstance(typeof(UnityBroadcastReceiver));
        private UnityBroadcastProxy broadcastReceiverInterface;
        private AndroidJavaObject VolumeBroadcastReceiver; 

        public UnityBroadcastReceiver() {
            VolumeBroadcastReceiver = new AndroidJavaObject("com.deepwaterooo.sdk.utils.VolumeBroadcastReceiver");
        }

        public void initBroadcast() {
            Debug.Log(TAG + " initBroadcast()");

            broadcastReceiverInterface = new UnityBroadcastProxy();
            broadcastReceiverInterface.onReceiveDelegate = onBroadcastReceive;       // 设置接收后的回调方法代理
// 这里同样有一个设置的步骤: 但是因为在独立的类中完成,还是相对比较方便的            
            VolumeBroadcastReceiver.Call("setReceiver", broadcastReceiverInterface); // <<<<<<<<<<<<<<<<<<<< 设置接收者
            try {
                AndroidJavaObject recevierFilter = new AndroidJavaObject("android.content.IntentFilter");
                recevierFilter.Call("addAction", "android.media.VOLUME_CHANGED_ACTION"); // 监听手机硬件调控音量的变化
                AndroidJavaObject _UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                _UnityActivity.Call<AndroidJavaObject>("registerReceiver", VolumeBroadcastReceiver, recevierFilter);
            }
            catch (Exception e) {
                Debug.Log(TAG + " " + e.Message.ToString());
                throw; 
            }
        }

        void onBroadcastReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            // AndroidJavaObject action = intent.Call<AndroidJavaObject>("getAction"); // <<<<<<<<<< 参考网络上的,这样写不对!!!
            string action = intent.Call<string>("getAction");
            Debug.Log(TAG + " onBroadcastReceive() action: " + action);
            Debug.Log(TAG + " (action.ToString().Equals('android.media.VOLUME_CHANGED_ACTION')): " + (action.ToString().Equals("android.media.VOLUME_CHANGED_ACTION")));
            if (action != null && action.ToString().Equals("android.media.VOLUME_CHANGED_ACTION")) {
// 把当前音量读出来
                int curVol = intent.Call<int>("getIntExtra", "android.media.EXTRA_VOLUME_STREAM_VALUE", 0);
                Debug.Log(TAG + " onBroadcastReceive() curVol: " + curVol);
                Deepwaterooo.Instance.curVol.Value = curVol;
            }
        }
    }
}