using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 稍微在安卓平台上测了测,这个方法好像不太管用,不知道哪里没有连通,换个先搭个简单SDK的架测试一下    
	public class VolumeManager : Singleton<VolumeManager> { // TODO: TO BE TESTED
        private const string TAG = "VolumeManager"; 

        static AndroidJavaObject UnityActivity;
        static AndroidJavaObject UnityAppContext;
        static AndroidJavaClass Context;
        static AndroidJavaObject mAudioManager;
        static AndroidJavaClass AudioManager;
        static int STREAM_MUSIC;
        
        public static void Start() {
            Debug.Log(TAG + " Start()");

// #if UNITY_ANDROID //&& !UNITY_EDITOR            
            UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");        
            Context = new AndroidJavaClass("android.content.Context");
            AudioManager = new AndroidJavaClass("android.media.AudioManager");
            STREAM_MUSIC = AudioManager.GetStatic<int>("STREAM_MUSIC");
            mAudioManager = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));
            Debug.Log(TAG + " (UnityAppContext == null): " + (UnityAppContext == null));
            Debug.Log(TAG + " (UnityAppContext == null): " + (UnityAppContext == null));
            Debug.Log(TAG + " (Context != null): " + (Context != null));
            Debug.Log(TAG + " (AudioManager != null): " + (AudioManager != null));
// #endif
        }
        
        public static int getMaxVolume() {
            Debug.Log(TAG + " getMaxVolume()");
// #if UNITY_ANDROID //&& !UNITY_EDITOR            
            mAudioManager = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));
            int tmp = mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
            // return mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
            Debug.Log(TAG + " tmp: " + tmp);
            return tmp;
// #else
//             return 0;
// #endif
        }
        public static int getCurrentVolume() {
// #if UNITY_ANDROID //&& !UNITY_EDITOR
             int tmp =  mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
             Debug.Log(TAG + " tmp: " + tmp);
             return tmp;
            // return mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
// #else
//              return 0;
// #endif
         }
        public static void setVolume(int value) {
            value = 50;
// #if UNITY_ANDROID //&& !UNITY_EDITOR            
            mAudioManager.Call("setStreamVolume", STREAM_MUSIC, value, AudioManager.GetStatic<int>("FLAG_PLAY_SOUND"));
// #endif
        }
	}
}
