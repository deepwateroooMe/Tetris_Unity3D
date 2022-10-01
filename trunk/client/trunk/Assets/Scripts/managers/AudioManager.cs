using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace tetris3d {

    public class AudioManager : Singleton<AudioManager> { 
        private const string TAG = "AudioManager";

        private static AudioSource audioSource;

        public AudioClip gameLoop; 
        public AudioClip clearLineSound;
        public AudioClip explosion;
        
        public AudioClip moveSound;
        public AudioClip rotateSound;
        public AudioClip landSound;
        private EventManager eventManager;

        void OnEnable () {
            Debug.Log(TAG + ": OnEnable()"); 
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            audioSource = GetComponent<AudioSource>();

            EventManager.Instance.RegisterListener<CanvasMovedEventInfo>(onCanvasMoved);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }
        
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

        void onCanvasMoved(CanvasMovedEventInfo canvasMovedInfo) {
            if (canvasMovedInfo.delta.y != 0) { 
                audioSource.PlayOneShot(rotateSound);
            } else {                            
                audioSource.PlayOneShot(moveSound);
            }
        }

        void onTetrominoLand(TetrominoLandEventInfo info) {
            audioSource.PlayOneShot(landSound);
        }
            
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);

            // Debug.Log(TAG + " (EventManager.Instance != null): " + (EventManager.Instance != null));  // when AudioManager OnDisable(), EventManager = null at the time ?
            if (EventManager.Instance != null) { // does it hold a reference to the EventManager.cs, and holds it from destroying ?
                EventManager.Instance.UnregisterListener<CanvasMovedEventInfo>(onCanvasMoved);
                EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            }
        }        
    }
}