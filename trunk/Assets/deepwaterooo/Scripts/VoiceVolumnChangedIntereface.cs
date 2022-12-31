using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace deepwaterooo.tetris3d {
    // 监听音量变化的监听接口
    public class VoiceVolumnChangedIntereface : AndroidJavaProxy {

        // 音量变化委托事件
        Action<int> _mVoiceVolumnChanged;
        public VoiceVolumnChangedIntereface(Action<int> VoiceVolumnChanged) : base("com.deepwaterooo.sdk.utils.VoiceVolumnChangedIntereface") {
            _mVoiceVolumnChanged = VoiceVolumnChanged;
        }
        // 监听音量变化的接口
        public void VoiceVolumnChanged(int currentValue) {
            if (_mVoiceVolumnChanged != null) 
                _mVoiceVolumnChanged(currentValue);
            Debug.Log(GetType()+ "/VoiceVolumnChanged()/ 监听音量变化的接口函数执行");
        }
    }
}