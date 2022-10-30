using UnityEngine;

namespace HotFix.Control {

// 应用中的音效播放管理器:它应该管理游戏中所有声音相关的,存有相关的音频,控制播放与停止等等,后来加上的?感觉代码不完整
// TODO: 这里我忘记了,为什么我必须在热更新程序域里再定义一遍一个应用类Singleton.cs来着?因为两个不同域之间需要适配吗?再测一下(现在还不是一个很好的时间来测这个;今天中午吃撑了下午的脑袋就在打转,等原理再透一些项目再熟悉一些再回来测)    
    public class AudioManager : SingletonMono<AudioManager> { // 感知Mono生命周期
       public const string TAG = "AudioManager";

        public string currentClipName;
        public AudioSource audioSource;

        private AudioClip currentClip = null;

        private AudioClip gameLoop; 
        private AudioClip moveSound;
        private AudioClip rotateSound;
        private AudioClip landSound;
        private AudioClip exploseSound;
        private AudioClip clearLineSound;

        //private EventManager eventManager;

        // void OnEnable () { // 这个方法暂时还没有适配
        //     Debug.Log(TAG + ": OnEnable()");
        public void Awake() {
            Debug.Log(TAG + " Awake()");
            gameObject.AddComponent<AudioSource>();
            audioSource = gameObject.GetComponent<AudioSource>();
            gameLoop = ResourceHelper.LoadAudioClip("ui/view/gameview", "gameloop", EAssetBundleUnloadLevel.Never);
            moveSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "move", EAssetBundleUnloadLevel.Never);
            rotateSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "rotate", EAssetBundleUnloadLevel.Never);
            landSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "land", EAssetBundleUnloadLevel.Never);
            exploseSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "Explosion", EAssetBundleUnloadLevel.Never);
            clearLineSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "linecleared", EAssetBundleUnloadLevel.Never);
            currentClip = gameLoop;
        }

        public void OnStart () {
            Debug.Log(TAG + ": OnStart()"); 
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
// todo: 其它游戏场景的时间播放主背景音乐
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onTetrominoMove);
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }

        void onTetrominoMove(TetrominoMoveEventInfo info) {
            audioSource.PlayOneShot(moveSound);
        }
        void onTetrominoRotate(TetrominoRotateEventInfo info) {
            audioSource.PlayOneShot(rotateSound);
        }
        void onTetrominoLand(TetrominoLandEventInfo info) {
            audioSource.PlayOneShot(landSound);
        }

        public void setCurrentClip(string name) {
            switch (name) {
            case "move":
                currentClip = moveSound;
                break;
            case "rotate":
                currentClip = rotateSound;
                break;
            case "land":
                currentClip = landSound;
                break;
            case "explose":
                currentClip = exploseSound;
                break;
            case "clearline":
                currentClip = clearLineSound;
                break;
            default:
                currentClip = gameLoop;
                break;
            }
        }

        public void PlayAudio(string name) {
            switch (name) {
            case "move":
                currentClip = moveSound;
                break;
            case "rotate":
                currentClip = rotateSound;
                break;
            case "land":
                currentClip = landSound;
                break;
            case "explose":
                currentClip = exploseSound;
                break;
            case "clearline":
                currentClip = clearLineSound;
                break;
            default:
                currentClip = gameLoop;
                break;
            }
            audioSource.PlayOneShot(currentClip);
        }

        public void PlayOneShotAudioClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
        }
        
        public void PlayOneShotAudioClip() {
            audioSource.PlayOneShot(currentClip);
        }

// BUG: 这里好像是只播放一次,想要循环播放        
        public void PlayOneShotGameLoop() {
            audioSource.PlayOneShot(gameLoop);
        }

// 它是不是容易失忆呢?        
        public void Play() {
            Debug.Log(TAG + " Play");
            audioSource.PlayOneShot(currentClip); // 这就是从来开始播放,而不是继续播放
            // audioSource.UnPause();
        }
        
        public void Pause() { 
            audioSource.Pause();
        }

        public void PlayLineClearedSound() { // listen to the event
            audioSource.PlayOneShot(clearLineSound);
        }

            
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
//            Debug.Log(TAG + " gameObject.name: " + gameObject.name);

// com for tmp: 这里的代码写得好奇怪,明明是音频视频管理器,却搅到事件管理器里去了            
            // // Debug.Log(TAG + " (EventManager.Instance != null): " + (EventManager.Instance != null));  // when AudioManager OnDisable(), EventManager = null at the time ?
            // if (EventManager.Instance != null) { // does it hold a reference to the EventManager.cs, and holds it from destroying ?
            //     EventManager.Instance.UnregisterListener<CanvasMovedEventInfo>(onCanvasMoved);
            //     EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            // }
        }        
    }
}
