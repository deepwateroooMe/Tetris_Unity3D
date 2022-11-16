package com.me.tetrissdk;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.media.AudioManager;
import android.os.Bundle;

// 这个类是,unity调用安卓SDK的入口类,必须继承自这个基类
public class MainActivity extends UnityPlayerActivity {

    // 音乐音量
    final AudioManager mAudioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
 
// 获得最大音量
    int max = mAudioManager.getStreamMaxVolume( AudioManager.STREAM_MUSIC );
 
// 获得当前音量
    int current = mAudioManager.getStreamVolume( AudioManager.STREAM_MUSIC );
 
// 修改音量
    mAudioManager.setStreamVolume(AudioManager.STREAM_MUSIC,5,AudioManager.FLAG_SHOW_UI);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }
}