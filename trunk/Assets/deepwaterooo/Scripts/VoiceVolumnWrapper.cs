using Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    // 音量相关封装
    // 1、首先初始化 Init　（VolumeCallbackInit()），设置监听事件
    // 2、后面即可获取设置音量值
    // 3、监听音量变化
// TODO: 与现模块化设计不符，要怎么与现安卓SDK游戏SDK统一化管理，方便源码维护与移植　？    
    public class VoiceVolumnWrapper : SingletonMono<VoiceVolumnWrapper> {               
        // 初始化
        public void Init(Action<int> VoiceVolumnChangedListener) {
#if UNITY_EDITOR
#else 
            MAndroidJavaObject.Call("VolumeCallbackInit", new VoiceVolumnChangedIntereface(VoiceVolumnChangedListener)); 
            registerVolumeReceiver();
#endif
        }

        // 获取音量的当前值
        public int GetMusicVoiceCurrentValue() {
            return MAndroidJavaObject.Call<int>("GetMusicVoiceCurrentValue");
        }
        // 设置音量值
        public void SetMusicVoiceVolumn(int value) {
            MAndroidJavaObject.Call("SetMusicVoiceVolumn", value);
        }
        // 获取音量的最大值
        public int GetMusicVoiceMax() {
            return MAndroidJavaObject.Call<int>("GetMusicVoiceMax");
        }
        // 获取音量的最小值
        public int GetMusicVoiceMin() {
            return MAndroidJavaObject.Call<int>("GetMusicVoiceMin"); 
        }

#region 私有方法
        // 注册音量监听
        void registerVolumeReceiver() {
            MAndroidJavaObject.Call("registerVolumeReceiver");
        }
        // 取消注册音量监听
        void unregisterVolumeReceiver() {
            MAndroidJavaObject.Call("unregisterVolumeReceiver");
        }
// TODO: 这个地方会资源泄露,要怎么处理一下        
        protected void OnDestroy() { 
            unregisterVolumeReceiver();
            // base.OnDestroy(); // 我应该是可以不用调用这个就可以了
        }
#endregion
#region 私有变量

// 音量变化委托事件
// 这里的逻辑没有看明白：　它是怎么回调到变化的，在哪里如何通知了谁？ 应用是涉及到一点儿安卓代理的内部原理？
        Action<int> _mVoiceVolumnChanged; 
        
        AndroidJavaObject _mAndroidJavaObject;
        public AndroidJavaObject MAndroidJavaObject {
            get {
                if (_mAndroidJavaObject == null) 
                    _mAndroidJavaObject = new AndroidJavaObject("com.deepwaterooo.sdk.utils.VoiceVolumnUtil");
                return _mAndroidJavaObject;
            }
        }
#endregion
    }
}
