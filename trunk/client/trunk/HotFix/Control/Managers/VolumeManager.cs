using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 稍微在安卓平台上测了测,这个方法好像不太管用,不知道哪里没有连通,换个先搭个简单SDK的架测试一下    
	public class VolumeManager : SingletonMono<VolumeManager> { // TODO: TO BE TESTED
        private const string TAG = "VolumeManager"; 
        // AndroidJavaObject UnityActivity;
        // AndroidJavaObject UnityAppContext;

        AndroidJavaClass Context, jc;
        AndroidJavaObject mAudioManager, jo;
        AndroidJavaClass AudioManager;
        int STREAM_MUSIC;

        private int preVol;
        // public int maxVol, curVol, preVol;
        public BindableProperty<int> maxVol;
        public BindableProperty<int> curVol;
        
        public void Start() {
            Debug.Log(TAG + " Start()");

// #if UNITY_ANDROID //&& !UNITY_EDITOR            
            // UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            // UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");        
            // Context = new AndroidJavaClass("android.content.Context");
            // AudioManager = new AndroidJavaClass("android.media.AudioManager");
            // STREAM_MUSIC = AudioManager.GetStatic<int>("STREAM_MUSIC");
            // mAudioManager = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));

            maxVol = new BindableProperty<int>();
            curVol = new BindableProperty<int>();
            maxVol.Value = 0;
            curVol.Value = -1;
            // AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
// 这里的原理有点儿没想明白:为什么必须得调用这个类?先试第一套方案            
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
            AsrEventCallback asrEventCallback = new AsrEventCallback();
            // 设置语音识别回调函数接口
            jo.Call("setCallback", asrEventCallback);
            
            Debug.Log(TAG + " (jc == null): " + (jc == null));
            Debug.Log(TAG + " (jo == null): " + (jo == null));
            // Debug.Log(TAG + " (UnityAppContext == null): " + (UnityAppContext == null));
            // Debug.Log(TAG + " (UnityAppContext == null): " + (UnityAppContext == null));
            // Debug.Log(TAG + " (Context != null): " + (Context != null));
            // Debug.Log(TAG + " (AudioManager != null): " + (AudioManager != null));
// #endif
        }
        
        public int getMaxVolume() {
            Debug.Log(TAG + " getMaxVolume()");
            int val = jo.Call<int>("getMaxVolume"); // 记得,这种调用是需要时间的,需要把结果给返回来
            Debug.Log(TAG + " val: " + val);
            return val;
// // #if UNITY_ANDROID //&& !UNITY_EDITOR            
//             mAudioManager = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));
//             int tmp = mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
//             // return mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
//             Debug.Log(TAG + " tmp: " + tmp);
//             return tmp;
// // #else
// //             return 0;
// // #endif
        }
        int onMaxVolumeReady(int val) {
            Debug.Log(TAG + " onMaxVolumeReady()");
            maxVol.Value = val;
            return val;
        }
        int onCurVolumeReady(int val) {
            Debug.Log(TAG + " onCurVolumeReady()");
            curVol.Value = val;
            return val;
        }
        public int getCurrentVolume() {
            Debug.Log(TAG + " getCurrentVolume()");
            curVol.Value = jo.Call<int>("getCurrentVolume");
            Debug.Log(TAG + " curVol: " + curVol);
            return curVol.Value;
// // #if UNITY_ANDROID //&& !UNITY_EDITOR
//              int tmp =  mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
//              Debug.Log(TAG + " tmp: " + tmp);
//              return tmp;
//             // return mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
// // #else
// //              return 0;
// // #endif
         }
        public void setVolume(int value) {
            Debug.Log(TAG + " setVolume() value: " + value);
            Debug.Log(TAG + " (curVol == null): " + (curVol == null));
            Debug.Log(TAG + " curVol.Value: " + curVol.Value);
            if (curVol.Value == value) return ;
            if (value == 0)
                preVol = curVol.Value;
            else if (curVol.Value == 0 && value == -1)
				value = preVol;
            mAudioManager.Call("setStreamVolume", STREAM_MUSIC, value, AudioManager.GetStatic<int>("FLAG_PLAY_SOUND"));
// TODO: 这里要不要考虑系统设置失败的情况,虽然极少?
            curVol.Value = value;
        }
	}
}

