using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Util;
using UnityEngine;

namespace HotFix.Control {
    // 应用中的音效播放管理器:它应该管理游戏中所有声音相关的,存有相关的音频,控制播放与停止等等,后来加上的?感觉代码不完整
    public class AudioManager : Singleton<AudioManager> { // 感知Mono生命周期
        public const string TAG = "AudioManager";

// 这里可能需要一个设置功能        
        public AudioSource audioSource;

        public AudioClip gameLoop; 

        public AudioClip moveSound;
        public AudioClip rotateSound;
        public AudioClip landSound;
        public AudioClip exploseSound;
        public AudioClip clearLineSound;
        
        //private EventManager eventManager;

//         void OnEnable () {
//             Debug.Log(TAG + ": OnEnable()"); 
//             // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
//             audioSource = gameObject.GetComponent<AudioSource>(); 

// // com for tmp: 这里的代码写得好奇怪,明明是音频视频管理器,却搅到事件管理器里去了            
//             // EventManager.Instance.RegisterListener<CanvasMovedEventInfo>(onCanvasMoved);
//             // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
//         }

        public void PlayOneShotAudioClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
        }
        
        public void PlayOneShotGameLoop() {
            audioSource.PlayOneShot(gameLoop);
        }

        public void Pause() { 
            audioSource.Pause();
        }

        public void PlayLineClearedSound() { // listen to the event
            audioSource.PlayOneShot(clearLineSound);
        }

        // void onCanvasMoved(CanvasMovedEventInfo canvasMovedInfo) {
        //     // if (canvasMovedInfo.delta.y != 0) { 
        //     //     audioSource.PlayOneShot(rotateSound);
        //     // } else {                            
        //     //     audioSource.PlayOneShot(moveSound);
        //     // }
        // }

        // void onTetrominoLand(TetrominoLandEventInfo info) {
        //     // audioSource.PlayOneShot(landSound);
        // }
            
//         void OnDisable() {
//             Debug.Log(TAG + ": OnDisable()");
// //            Debug.Log(TAG + " gameObject.name: " + gameObject.name);

// // com for tmp: 这里的代码写得好奇怪,明明是音频视频管理器,却搅到事件管理器里去了            
//             // // Debug.Log(TAG + " (EventManager.Instance != null): " + (EventManager.Instance != null));  // when AudioManager OnDisable(), EventManager = null at the time ?
//             // if (EventManager.Instance != null) { // does it hold a reference to the EventManager.cs, and holds it from destroying ?
//             //     EventManager.Instance.UnregisterListener<CanvasMovedEventInfo>(onCanvasMoved);
//             //     EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
//             // }
//         }        
    }
}
