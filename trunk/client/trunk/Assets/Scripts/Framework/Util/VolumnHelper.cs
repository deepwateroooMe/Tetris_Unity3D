using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Util {

// 这里想先用个最简单的做法: 不打安卓JAVA .AAR包,或是C++ JNI DLLSO 包    
    public class VolumnHelper {
        private const string TAG = "VolumnHelper";

// 这里可能还需要再判断一下平台:因为目前我仍是在PC端在运行程序        
        AndroidJavaObject mAudioManager = null;
        AndroidJavaClass AudioManager;

        // private void init() {
        //     AndroidJavaObject UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        //     AndroidJavaObject UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");        
        // }

        int STREAM_MUSIC;

        public int getMaxVolume() {
            Debug.Log(TAG + " getMaxVolume()");
            AndroidJavaClass Context = new AndroidJavaClass("android.content.Context");
            AudioManager = new AndroidJavaClass("android.media.AudioManager");
            STREAM_MUSIC = AudioManager.GetStatic<int>("STREAM_MUSIC");
            mAudioManager = AndroidTools.UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));
            return mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
        }

        public int getCurrentVolume() {
            return mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
        }

        public void setVolume(int value) {
            mAudioManager.Call("setStreamVolume", STREAM_MUSIC, value, AudioManager.GetStatic<int>("FLAG_PLAY_SOUND"));
            // AndroidTools.AndroidLogI(GetCurrentVolume().ToString());
        }
    }
}