using UnityEngine;

namespace HotFix.Control {
    // 应用中的音效播放管理器:它应该管理游戏中所有声音相关的,存有相关的音频,控制播放与停止等等,后来加上的?感觉代码不完整
    // public class AudioManager : Singleton<AudioManager> { // 感知Mono生命周期: BUG 
    // public class AudioManager : SingletonMono<AudioManager> { // 感知Mono生命周期
    public class AudioManager : MonoBehaviour { // 感知Mono生命周期 
       public const string TAG = "AudioManager";

// 这里可能需要一个设置功能        
        public AudioSource audioSource;

        private AudioClip gameLoop; 
        private AudioClip moveSound;
        private AudioClip rotateSound;
        private AudioClip landSound;
        private AudioClip exploseSound;
        private AudioClip clearLineSound;

        private AudioClip currentClip = null;
        public string currentClipName;

        // 现在改得丑一点儿就让它丑一点儿,先把它弄运行了再说,把它改成同样例一样   
        private static AudioManager instance;
        public static AudioManager Instance {
            get {
                if (instance == null) {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<AudioManager>();
                    // obj.name = instance.GetType().Name;
                    obj.name = "AudioManager";
                }
                return instance;
            }
        }
        //private EventManager eventManager;

        void OnEnable () {
            instance = this;
            
            Debug.Log(TAG + ": OnEnable()"); 
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            audioSource = gameObject.GetComponent<AudioSource>(); 

// com for tmp: 这里的代码写得好奇怪,明明是音频视频管理器,却搅到事件管理器里去了            
            // EventManager.Instance.RegisterListener<CanvasMovedEventInfo>(onCanvasMoved);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }

        public void InitializeAudioClips() {
            // instance = this;
            gameLoop = ResourceHelper.LoadAudioClip("ui/view/gameview", "gameloop", EAssetBundleUnloadLevel.Never);
            moveSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "move", EAssetBundleUnloadLevel.Never);
            rotateSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "rotate", EAssetBundleUnloadLevel.Never);
            landSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "land", EAssetBundleUnloadLevel.Never);
            exploseSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "Explosion", EAssetBundleUnloadLevel.Never);
            clearLineSound = ResourceHelper.LoadAudioClip("ui/view/gameview", "linecleared", EAssetBundleUnloadLevel.Never);
            currentClip = gameLoop;
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

        public void PlayOneShotAudioClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
        }
        
        public void PlayOneShotAudioClip() {
            audioSource.PlayOneShot(currentClip);
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
