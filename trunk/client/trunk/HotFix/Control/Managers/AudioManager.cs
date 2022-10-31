using UnityEngine;

namespace HotFix.Control {

// Consider it: WELL DONE, 删除源码中所有其它部分的调用    
    public class AudioManager : SingletonMono<AudioManager> { 
       public const string TAG = "AudioManager";

        public AudioSource audioSource;
        private AudioClip gameLoop;
        
        private AudioClip moveSound;
        private AudioClip rotateSound;
        private AudioClip landSound;

        private AudioClip clearLineSound;
        private AudioClip exploseSound; // TODO

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
            audioSource.clip = gameLoop;
        }

        public void Start () {
            Debug.Log(TAG + ": Start()"); 
            EventManager.Instance.RegisterListener<GameEnterEventInfo>(onEnterGame); 
            EventManager.Instance.RegisterListener<GamePauseEventInfo>(onPauseGame);
            EventManager.Instance.RegisterListener<GameResumeEventInfo>(onResumeGame);
            
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onTetrominoMove);
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }

        void onEnterGame(GameEnterEventInfo info) {
            audioSource.clip = gameLoop;
            audioSource.loop = true;
            audioSource.Play();
        }
        void onPauseGame(GamePauseEventInfo info) {
            audioSource.Pause();
        }
        void onResumeGame(GameResumeEventInfo info) {
            audioSource.Play();
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

        public void PlayOneShotAudioClip(AudioClip clip) {
            audioSource.PlayOneShot(clip);
        }
        
        public void PlayOneShotAudioClip() {
            //audioSource.PlayOneShot(currentClip);
        }

// BUG: 这里好像是只播放一次,想要循环播放        
        public void PlayOneShotGameLoop() {
            audioSource.PlayOneShot(gameLoop);
        }

// 它是不是容易失忆呢?        
        public void Play() {
            Debug.Log(TAG + " Play");
            //audioSource.PlayOneShot(currentClip); // 这就是从来开始播放,而不是继续播放
            // audioSource.UnPause();
        }
        
        public void Pause() { 
            audioSource.Pause();
        }

        public void PlayLineClearedSound() { // listen to the event
            audioSource.PlayOneShot(clearLineSound);
        }

        // // 1.粒子特效的GameObject实例化完毕。
        // // 2.确保粒子所用到的贴图载入内存
        // // 3.让粒子进行一次预热（目前预热功能只能在循环的粒子特效里面使用，所以不循环的粒子特效是不能用的）
        // // 粒子系统的实例化，何时销毁？
        // // 出于性能考虑，其中Update内部的操作也可以移至FixedUpdate中进行以减少更新次数，但是视觉上并不会带来太大的差异
        // // // temporatorily don't consider these yet
        // // string particleType = "particles";
        //  m_ExplosionParticles = ViewManager.GetFromPool(GetSpecificPrefabType(m_ExplosionPrefab)).GetComponent<ParticleSystem>();
        // // m_ExplosionParticles = ViewManager.GetFromPool(particleType).GetComponent<ParticleSystem>();
        // /m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // // m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        // // m_ExplosionParticles.gameObject.SetActive(false);
        //  因为实例化粒子特效以后，实际上粒子的脚本就已经完成了初始化的工作，也就是Awake()和OnEnable()方法。然后设置SetActive(false)仅仅是把粒子特效隐藏起来。
        
        public void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
//            Debug.Log(TAG + " gameObject.name: " + gameObject.name);

// com for tmp: 这里的代码写得好奇怪,明明是音频视频管理器,却搅到事件管理器里去了            
            // // Debug.Log(TAG + " (EventManager.Instance != null): " + (EventManager.Instance != null));  // when AudioManager OnDisable(), EventManager = null at the time ?
            // if (EventManager.Instance != null) { // does it hold a reference to the EventManager.cs, and holds it from destroying ?
            //     EventManager.Instance.UnregisterListener<CanvasMovedEventInfo>(onCanvasMoved);
            //     EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            // }
        }        

// // 好像这两个方法不是很有用
//         public void setCurrentClip(string name) {
//             switch (name) {
//             case "move":
//                 currentClip = moveSound;
//                 break;
//             case "rotate":
//                 currentClip = rotateSound;
//                 break;
//             case "land":
//                 currentClip = landSound;
//                 break;
//             case "explose":
//                 currentClip = exploseSound;
//                 break;
//             case "clearline":
//                 currentClip = clearLineSound;
//                 break;
//             default:
//                 currentClip = gameLoop;
//                 break;
//             }
//         }
//         public void PlayAudio(string name) {
//             switch (name) {
//             case "move":
//                 currentClip = moveSound;
//                 break;
//             case "rotate":
//                 currentClip = rotateSound;
//                 break;
//             case "land":
//                 currentClip = landSound;
//                 break;
//             case "explose":
//                 currentClip = exploseSound;
//                 break;
//             case "clearline":
//                 currentClip = clearLineSound;
//                 break;
//             default:
//                 currentClip = gameLoop;
//                 break;
//             }
//             audioSource.PlayOneShot(currentClip);
//         }
    }
}
