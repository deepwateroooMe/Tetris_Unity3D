using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.deepwaterooo.Scripts {
    public class UnityBroadcastReceiver { // 它的生命周期会不会也狠短: 不适合这个音量变化的广播接收,因为收到了还要再回安卓平台去读一遍当前音量,效率太低了
        private const string TAG = "UnityBroadcastReceiver";

        public static UnityBroadcastReceiver instance = (UnityBroadcastReceiver)Activator.CreateInstance(typeof(UnityBroadcastReceiver));
        private UnityBroadcastProxy broadcastReceiverInterface;
        private AndroidJavaObject UABroadcastReceiver; // <<<<<<<<<< 

        public UnityBroadcastReceiver() {
            Debug.Log(TAG + " UnityBroadcastReceiver() consttructor");
            UABroadcastReceiver = new AndroidJavaObject("com.deepwaterooo.UABroadcastReceiver"); // <<<<<<<<<< 这里可能有个自动完成: 实例化的时候自动完成了游戏端接收代理的设置 ?
        }

        public void initBroadcast() { // 现在是: 项目中自己并不曾 调用和初始化这些
            Debug.Log(TAG + " initBroadcast()");
            broadcastReceiverInterface = new UnityBroadcastProxy();
            broadcastReceiverInterface.onReceiveDelegate = onBroadcastReceive;   // 设置接收后的回调方法代理
            UABroadcastReceiver.Call("setReceiver", broadcastReceiverInterface); // 设置接收者
            try {
                AndroidJavaObject recevierFilter = new AndroidJavaObject("android.content.IntentFilter");
                recevierFilter.Call("addAction", "android.media.VOLUME_CHANGED_ACTION"); // 监听手机硬件调控音量的变化
// 这里极有可能会错: 判断非空, 是为游戏端动态注册安卓广播接收器 拿到必要的 上下文环境
                AndroidJavaObject _UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                Debug.Log(TAG + " (_UnityActivity != null): " + (_UnityActivity != null));
                _UnityActivity.Call<AndroidJavaObject>("registerReceiver", UABroadcastReceiver, recevierFilter);
            }
            catch (Exception e) {
                Debug.Log(TAG + " " + e.Message.ToString());
                throw;
            }
        }

// 需要一个本地存储,来缓存接收到的音量的变化,保持最新最更新的状态
// 安卓系统广播,音量变化的广播里没有数据的变化,不包含当前数据,必须得再去安卓平台再读一遍
// 想测连通这个方法的目的是解偶合,但是感觉这么着来接收广播,再回安卓平台去读当前音量,效率比较低,可能下午的接口回调反而是最快效率比较好的
// 这里只是提供了另一个思路,和可行方法        
        void onBroadcastReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            Debug.Log(TAG + " onBroadcastReceive()");

            AndroidJavaObject action = intent.Call<AndroidJavaObject>("getAction"); // <<<<<<<<<< 
            if (action != null) {
                Debug.Log(TAG + "接收action：" + action.ToString());                

                Debug.Log(TAG + " (action.ToString().Equals('selfDefineReceiver')): " + (action.ToString().Equals("selfDefineReceiver")));
                if (action.ToString().Equals("selfDefineReceiver")) { // <<<<<<<<<< 这里不对,永远进不来
// TODO 这里自己写相应，上面一句可删除
                    string iaction = intent.Call<string>("getAction");
                    //string extra = intent.Call<int>("getIntExtra", "android.media.EXTRA_VOLUME_STREAM_TYPE", -1); // 不知道这里调用会不会对
                    Debug.Log(TAG + " iaction: " + iaction);
                    //Debug.Log(TAG + " extra: " + extra);
                    if (iaction != null && iaction.Equals("android.media.VOLUME_CHANGED_ACTION")) {
                        // && extra == AudioManager.STREAM_MUSIC) // 这个判断,可能会多余,也拿不到安卓平台的相关设置值
                        // TODO:　取出和存储必要的数据: 这里面没有数据,必须再去安卓平台读一遍,这样就不想再测下去了,去写别的急需解决的问题
                        Debug.Log(TAG + " onBroadcastReceive() TODO SOMETHING HERE");
                    }
                }
            }
        }
    }
}
