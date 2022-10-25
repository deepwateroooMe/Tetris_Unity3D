using System;
using System.Collections;
using System.IO;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
using Framework.Util;
using HotFix.Control;
using HotFix.Data;
using UnityEngine;
using UnityEngine.UI;
namespace HotFix.UI {
    
    // 世界坐标系视图BtnsCanvasView：
    // MoveCanvasView 和RotateCanvasView供其调控,但是他们位于世界坐标系下，区别于其它任何视图
    // ComTetroCamera, EduTetroCamera: 用来照两个预览的相机,也摆在世界坐标系下,与当前视图GameView交互
// 这是目前的设计下: 游戏过程中的所有的方块砖也将位于世界坐标系下
// 游戏视图里需要测一个更为重要的逻辑就是Update()函数,因为经典模式下的自动更新,需要把这个连通测一下
    // 因为架构中已经有了MonoBehaviourAdapter,适配了Update()方法,应该是没有问题的.但是要连通后测一下确保主要逻辑没问题

// TODO: 当用户点击返回主菜单,如果游戏还没有保存,需要有提示窗口提醒用户,是否需要保存游戏    
    public class GameView : UnityGuiView {
        private const string TAG = "GameView";
        public override string BundleName { get { return "ui/view/gameview"; } }
        public override string AssetName { get { return "GameView"; } }
        public override string ViewName { get { return "GameView"; } }
        public override string ViewModelTypeName { get { return typeof(GameViewModel).FullName; } }
        public GameViewModel ViewModel { get { return (GameViewModel)BindingContext; } }
        public override bool IsRoot { get { return true; } }

        GameObject managers;
		//PoolManager poolManager;
		Control.AudioManager audioManager;
        // EventManager eventManager;
        
// 基础游戏大方格
        // GameObject baseBoard3; // 这些控件还没有做,哪天头脑昏昏的时候才再去做
        // GameObject baseBoard4;
        GameObject baseBoard5;
// DesView 里的个文本框基本不变，不用管它们        
// ScoreDataView
        Text scoText; // Score Text
        Text lvlText; // Level Text
        Text linText; // Line Text
// StaticBtnsView
        Button pauBtn; // pause Game
        Button falBtn; // fall fast button
// ToggleBtnView
        int type = 0;
        Button togBtn; // Togcation
// EduBtnsView
        Button swaBtn; // swap current tetrominos to be a newly generating(it's coming after click) tetromino set
        Button undBtn; // undo last selected tetromino landing, revert it back
// ComTetroView
        Button ptoBtn; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个
// EduTetroView
        Button pteBtn; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个
// pausePanel: 把 MidMenuView 整合进来,统一管理
        GameObject pausePanel;
        Button savBtn; // SAVE GAME
        Button resBtn; // RESUME GAME
        Button guiBtn; // TUTORIAL
        Button manBtn; // BACK TO MAIN MENU
        Button creBtn; // CREDIT

// 私有的当前视图的元件? 暂时添加上AudioSouce元件顺承原混作一团的逻辑(感觉它不该是被AudioManager管才对吗?)
        AudioSource audioSource;
        AudioSource m_ExplosionAudio;
        public GameObject nextTetromino; // 这里把原本的静态标志STATIC给移掉了
        public GameObject ghostTetromino;
        public AudioClip clearLineSound;
        public Text hud_score;
        public Text hud_level;
        public Text hud_lines;
        public Canvas hud_canvas;
        public GameObject m_ExplosionPrefab;
        public Button invertButton;
        public Sprite newImg;
        public Sprite prevImg;

        public GameObject defaultContainer;
        // private int numberOfRowsThisTurn = 0;
        private GameObject cycledPreviewTetromino;
        private bool gameStarted = false;
        private Vector3 previewTetrominoPosition = new Vector3(-17f, -5f, -9.81f);
        private Vector3 previewTetromino2Position = new Vector3(-68.3f, 19.6f, 32.4f); // (-56.3f, -0.1f, 32.4f) (-24.8f, -0.1f, 32.4f);
        public int numLinesCleared = 0;
        public GameObject emptyGO;
        public Transform tmpTransform;
        public GameObject previewTetromino;
        public GameObject previewTetromino2;

// 这组移去模型里去了        
        // public string prevPreview; // to remember previous spawned choices
        // public string prevPreview2;
        // public string nextTetrominoType;  
        // public string previewTetrominoType; 
        // public string previewTetromino2Type;

        private SaveGameEventInfo saveGameInfo;
        // private GameObject tmpParentGO;
        
        private GameObject comTetroView; // 这里更多的应该是指 ComTetroView EduTetroView组合组件(会激或是失活某组组件)
        private GameObject eduTetroView;
        private Button pvBtnOne; // 这两个就指里面的按钮组件
        private Button pvBtnTwo;
        
		// swapPreviewTetrominoButton;        
		public GameObject undoButton;
        public GameObject savedGamePanel;
        public GameObject saveGameReminderPanel;
        private bool isDuringUndo = false;
        public bool saveForUndo = true;

        void onGameModeChanged(int pre, int cur) {
            Debug.Log(TAG + " onGameModeChanged");
            if (cur == 0) { // 启蒙模式,自动生成两个tetromino方块砖摆上去
                SpawnPreviewTetromino(); // 生成预览的两块方块砖,完全没有问题了!!!
            } else { // 非启蒙模式, 这里需要添加
                comTetroView.SetActive(false); // 这里最是设置为移动位置(方便期间也可以设置三套,失活与激活另经典模式下的第三套)
                eduTetroView.SetActive(false); 
                //swaBtn.SetActive(false);
                undoButton.SetActive(false); // 不可撤销(挑战模式是仍可以再考虑)
            }
            // 还有其它逻辑吗?
        }
        
        public void SpawnnextTetromino() {
            Debug.Log(TAG + ": SpawnnextTetromino()");
            if (!gameStarted) {
                Debug.Log(TAG + " (ViewModel.gameMode != null): " + (ViewModel.gameMode != null));
                if (ViewModel.gameMode != null && ViewModel.gameMode.Value == 0) {
                   SpawnPreviewTetromino();
               } else {
                   gameStarted = true;
                   nextTetromino = ViewManager.GetFromPool(
                       ViewModel.GetRandomTetromino(),
                       new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f),
                       Quaternion.identity, Vector3.one);
                   currentActiveTetrominoPrepare();
                   ViewManager.moveCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);
                   ViewManager.moveCanvas.gameObject.SetActive(true);  
                   SpawnGhostTetromino();
                   SpawnPreviewTetromino();
               }
            } else {
               previewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
               // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled; // 忘记了这里为什么需要旋转一下了
               nextTetromino = previewTetromino;
               currentActiveTetrominoPrepare();
                
               SpawnGhostTetromino();  
               moveRotatecanvasPrepare();
               SpawnPreviewTetromino();
            }
        }

        private void SpawnPreviewTetromino() {
            Debug.Log(TAG + ": SpawnPreviewTetromino()");
// 这里仍旧是写成观察者模式,视图观察视图模型的数据变化            
            previewTetromino = ViewManager.GetFromPool(
                ViewModel.GetRandomTetromino(), previewTetrominoPosition,
                Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();

// 这里是废话,方向反了,模型里的数据驱动的
            // ViewModel.previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;

            Debug.Log(TAG + " (previewTetromino != null): " + (previewTetromino != null)); 
            Debug.Log(TAG + " previewTetromino.name: " + previewTetromino.name); 
            
            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                // excepts: undoButton toggleButton fallButton
                ViewModel.buttonInteractableList[3] = 0;
                ViewModel.buttonInteractableList[4] = 0;
                ViewModel.buttonInteractableList[5] = 0;
                previewTetromino2 = ViewManager.GetFromPool(
                    ViewModel.GetRandomTetromino(), previewTetromino2Position, 
                    Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(ViewManager.tetroParent.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();

// 这里是废话,方向反了,模型里的数据驱动的
                // ViewModel.previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                // previewTetromino2.layer = LayerMask.NameToLayer("UI"); // not working on this RayCast button click right now
            }
        }

        private void SpawnPreviewTetromino(string type1, string type2) {
            previewTetromino = ViewManager.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            ViewModel.previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;
            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                previewTetromino2 = ViewManager.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(ViewManager.tetroParent.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                // ViewModel.previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type; // 这些都不需要了
            }
            ViewModel.buttonInteractableList[3] = 1; // undoButton
        }

        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            // gameMode = ((MenuViewModel)ViewModel.ParentViewModel).gameMode;
            // fallSpeed = 3.0f; // should be recorded too, here
            // if (gameMode == 0)
            //     resetGridOccBoard();
// 感觉这里好鬼魅呀:视图里已经进行这时在了,可是视图模型因为反射调用还没有启动完?!!!
// 这里的问题:应该是用IEnumerator去解决,但暂时这样; 在自己不断修改整理这些重构的代码的过程中,会对这些忘记了的相对陌生的一点点儿捡回来的
            // ViewModel.InitializationForNewGame(); // 这里不要视图层调用,视图模型自己处理
// 现在这里要做的就是:打预设的资源包(很简单),从资源包里将不是的预设读出来,存在字典里,供游戏需要的时候来用            
            SpawnnextTetromino();   // cmp for tmp
            // if (ViewModel.gameMode > 0) { // disable some components
            //     comTetroView.SetActive(false);
            //     eduTetroView.SetActive(false);
            //     swaBtn.SetActive(false);
            //     undoButton.SetActive(false);
            // }
            // ViewManager.moveCanvas.SetActive(true);
            // ViewManager.rotateCanvas.SetActive(false);
        }


        private static GameObject tmp;

        
        static void Start () { // 感觉这些逻辑放在视图里出很牵强,哪些是可以放在模型里的呢?
            Debug.Log(TAG + ": Start()");
           ComponentHelper.AddTetroComponent(tmp);
           Tetromino tetro = ComponentHelper.GetComponent(tmp);
           tetro.Awake();
           
            // // check if it is cleaned up first 这里有个热更新程序域EventManager适配的大BUG
            // Debug.Log(TAG + " (!EventManager.Instance.isCleanedUp()): " + (!EventManager.Instance.isCleanedUp())); 
            // if (!EventManager.Instance.isCleanedUp()) {
            //     EventManager.Instance.cleanUpLists();
            // }
            // // if (gameMode == 0) {
            // // EventManager.Instance.RegisterListener<SwapPreviewsEventInfo>(onSwapPreviewTetrominos); 
            // // EventManager.Instance.RegisterListener<UendoGameEventInfo>(onUndoGame); 
            // // EventManager.UndoButtonClicked += onUndoGame;
            // // EventManager.SwapButtonClicked += onSwapPreviewTetrominos;
            // // }
            // EventManager.Instance.RegisterListener<SaveGameEventInfo>(SaveGame); 
            // EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); // 自己重构时commented out for tmp
            // tmpTransform = emptyGO.transform; //  原本的
// // klfdjfldk            
//             tmpTransform = new GameObject().transform; // 这里暂时这么写,容易生成很多个控件,暂时这么写
            // audioSource = GetComponent<AudioSource>();
            // if (!string.IsNullOrEmpty(((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName)) {
            //     gameMode = ((MenuViewModel)ViewModel.ParentViewModel).gameMode;
            //     loadSavedGame = ((MenuViewModel)ViewModel.ParentViewModel).loadSavedGame;
            //     StringBuilder path = new StringBuilder("");
            //     if (gameMode > 0)
            //         path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/game.save");
            //     else 
            //         path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/grid" + gridWidth + "/game.save");
            //     if (loadSavedGame) {
            //         LoadGame(path.ToString());
            //     } else {
            //         LoadNewGame();
            //     }
            // } else {
// // dflkdjfdlkfjkk            
//             LoadNewGame();
            // }
            // currentLevel = startingLevel;
            // startingHighScore = PlayerPrefs.GetInt("highscore");
            // startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            // startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        
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
        }
        
        void OnClickPauButton() { // public void PauseGame()
            ViewModel.PauseGame(); // 游戏暂停
            // Bug: disable all Hud canvas buttons: swap
            pausePanel.SetActive(true);
        }
        void OnClickFalButton() {
        }
        void OnClickTogButton() {
            // 当前的视图需要隐藏起来吗? 检查一下逻辑
            type ^= 1;
// 根据当前需要显示的按钮组的值的不同,来显示平移组或是旋转组
// 这两个组的按钮也需要分别制作成视图,两个不同的视图
            // if (type == 0)
            // else
            Hide();
        }
        void OnClickPtoButton() { // preview Tetromino Button COMMON BUTTON
        }
        void OnClickPteButton() { // preview Tetromino Button EDUCATION BUTTON
        }
        void OnClickSwaButton() {
            // 这里需要下发指令到视图数据层,并根据随机数生成的新的tetromino来重新刷新UI
        }
        void OnClickUndButton() {
            // 类似的逻辑下发数据,并由数据驱动刷新UI
        }
// 游戏进程的暂停与恢复: 这么改要改狠久狠久才能够改得完,还是先实现一些基础功能吧,到时对游戏逻辑再熟悉一点儿统一重构效率能高很多的呀
#region pausePanel Button Handlers
        // MidMenuView 里的5 个按钮
        void OnClickSavButton() { // SAVE GAME
        }
        void OnClickResButton() { // RESUME GAME: 隐藏当前游戏过程中的视图,就可以了 // public void OnClickResButton();
            Time.timeScale = 1.0f;
            ViewModel.isPaused = false;
            pausePanel.SetActive(false);
            AudioManager.Instance.audioSource.Play();
            // Hide(); // 隐藏当前视图,游戏的其它相关逻辑呢?
        }
        void OnClickGuiButton() { // 可以视频GUIDE吗?
            // ViewManager.DesginView.Reveal();
        }
        void OnClickManButton() { // BACK TO MAIN MENU
            ViewManager.MenuView.Reveal();
            // 这里需要隐藏所有的游戏视图相关界面,有很多个;请视图管理者来关
            // ViewManager.CloseOtherRootViews("MenuView"); // 待测试: 在同一个视图中,这里可以不用隐藏游戏背景,方便提醒玩家游戏正在进行中,要保存吗要恢复吗
            Hide();
        }
        void OnClickCreButton() {
            // ViewManager.DesginView.Reveal();
        }
        
        void SaveGame(SaveGameEventInfo info) {
            Debug.Log(TAG + ": SaveGame()");
            ViewModel.saveForUndo = false;
            onGameSave();
            ViewModel.hasSavedGameAlready = true;
            // savedGamePanel.SetActive(true);
        }
        public void onSavedGamePanelOK() {
            // savedGamePanel.SetActive(false);
        }
        public void onBackToMainMenu() {
            // if (!ViewModel.hasSavedGameAlready && gameStarted) { // gameStarted
            //    saveGameReminderPanel.SetActive(true);
            // } else {
            //    cleanUpGameBroad(nextTetromino);
            //    ViewModel.isPaused = false;
            //    Time.timeScale = 1.0f;
            //    SceneManager.LoadScene("GameMenu");
            // }
        }
        public void onYesToSaveGame() {
            // ViewModel.saveForUndo = false;
            // onGameSave();
            // ViewModel.hasSavedGameAlready = true;
            // saveGameReminderPanel.SetActive(false);
            // pausePanel.SetActive(false);
            // cleanUpGameBroad(nextTetromino);
            // ViewModel.isPaused = false;
            // Time.timeScale = 1.0f;
            // SceneManager.LoadScene("GameMenu");
        }
        public void onNoToNotSaveGame() { // 如何才能够延迟加载呢？
            // ViewModel.hasSavedGameAlready = false;
            // saveGameReminderPanel.SetActive(false);
            // pausePanel.SetActive(false);
            // cleanUpGameBroad(nextTetromino);
            // ViewModel.isPaused = false;
            // Time.timeScale = 1.0f;
            // if (gameMode == 1)
            //    gameStarted = false;
            // still have to check this due to auto Save
            string path = new StringBuilder(Application.persistentDataPath).Append("/").Append(((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName).Append("/game.save").ToString();
            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }
            }
            // SceneManager.LoadScene("GameMenu"); // commented out for tmp
        }
#endregion
            void onGameSave() { // 是指将游戏进度存到本地数据文件
                Debug.Log(TAG + ": onGameSave()");
// 不能直接删:偶合偶合,会影响其它逻辑 !!!            
                // if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事,为什么要生成一个新的空的呢?
                //    // 是因为过程中的某个步骤用到了这个,可是也应该在那个要用的时候重置或是生成一个新的呀,而不是这里,那个时候都是些什么逻辑!!!
                //    tmpTransform = new GameObject().transform;
                SaveSystem.SaveGame(ViewModel);
            }
        void setAllBaseBoardInactive() {
            // baseBoard3.SetActive(false);
            // baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }
        IEnumerator asyncLoadScene() {
            // AsyncOperation async = SceneManager.LoadSceneAsync("GameMenu");
            // yield return async;
            yield return null;
        }
        // void onGameSave(SaveGameEventInfo info) {
        // void onGameSave() {
        //    Debug.Log(TAG + ": onGameSave()");
        //    if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
        //        tmpTransform = new GameObject().transform;
        //    SaveSystem.SaveGame(this);
        // }
        public void CheckUserInput() {  // originally pasuseButton & continueButton
            Debug.Log(TAG + ": CheckUserInput()"); 
            if (Time.timeScale == 1.0f) {
                PauseGame();
            } else {
                OnClickResButton();
            }
        }
        public void PauseGame() {
            Time.timeScale = 0f;        
            audioSource.Pause();
            ViewModel.isPaused = true;
            // Bug: disable all Hud canvas buttons: swap
            audioSource.Pause();
            pausePanel.SetActive(true);
            // Bug cleaning: when paused game, if game has NOT started yet, disable Save Button
            if (!gameStarted) {
                
            }
        }
        public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            MoveUp();
            ViewModel.onActiveTetrominoLand(info, nextTetromino);
// 更好的办法应该是前面定义过的 BindableProperty<IType>,对方块砖的基类和扩展类分别作出不同的实现,来解除偶合            
            ViewModel.recycleGhostTetromino(ghostTetromino); // 放这里的主要原因是需要传参数
            // // SaveGameEventInfo fire here 
            // saveGameInfo = new SaveGameEventInfo();
            // EventManager.Instance.FireEvent(saveGameInfo);
            // change an approach: it is unnessary and do NOT apply delegates and events here
            // onGameSave();
            DisableMoveRotationCanvas();
            if (((MenuViewModel)ViewModel.ParentViewModel).gameMode != 0) 
                SpawnnextTetromino();  
        }
        // public void recycleGhostTetromino(GameObject ghostTetromino) {
        //     Debug.Log(TAG + ": recycleGhostTetromino()");
        //     Debug.Log(TAG + " ghostTetromino.name: " + ghostTetromino.name); 
        //     // Debug.Log(TAG + " (ghostTetromino == null): " + (ghostTetromino == null));
        //     // Debug.Log(TAG + " ghostTetromino.tag: " + ghostTetromino.tag); 
        //     // Debug.Log(TAG + " ghostTetromino.CompareTag(\"currentGhostTetromino\"): " + ghostTetromino.CompareTag("currentGhostTetromino")); 
        //     if (ghostTetromino != null) {
        //         ghostTetromino.tag = "Untagged";
        //         ViewManager.ReturnToPool(ghostTetromino, ghostTetromino.GetComponent<TetrominoType>().type);
        //     }
        // }
        
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            if (EventManager.Instance != null) {
                // if (gameMode == 0) {
                // EventManager.Instance.UnregisterListener<SwapPreviewsEventInfo>(onSwapPreviewTetrominos);
                // EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
                // EventManager.UndoButtonClicked -= onUndoGame;
                // EventManager.SwapButtonClicked -= onSwapPreviewTetrominos;
                // }                
                EventManager.Instance.UnregisterListener<SaveGameEventInfo>(SaveGame); 
                EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove);
                EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
                EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            }
        }
        public void playFirstTetromino() {
            Debug.Log(TAG + ": playFirstTetromino()");
            Debug.Log(TAG + " ViewModel.buttonInteractableList[0]: " + ViewModel.buttonInteractableList[0]); 
            if (ViewModel.buttonInteractableList[0] == 0) return;
            ViewModel.playFirstTetromino(previewTetromino, previewTetromino2, cycledPreviewTetromino);
            previewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
            // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            
            ViewManager.nextTetromino.Value = previewTetromino;
            currentActiveTetrominoPrepare();
            // gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
            // disables: comTetroView eduTetroView swaBtn
            // enables: undoButton toggleButton fallButton
            if (ViewModel.gameMode.Value == 0) {
                ViewModel.buttonInteractableList[0] = 0;
                ViewModel.buttonInteractableList[1] = 0;
                ViewModel.buttonInteractableList[2] = 0;
                ViewModel.buttonInteractableList[3] = 1;
                ViewModel.buttonInteractableList[4] = 1;
                ViewModel.buttonInteractableList[5] = 1;
            }
            // printViewModel.buttonInteractableList();
        }
        // void printViewModel.buttonInteractableList() {
        //     for (int i = 0; i < 6; i++) 
        //         Debug.Log(TAG + " ViewModel.buttonInteractableList[i]: i : " + i + ", " + ViewModel.buttonInteractableList[i]); 
        // }
        public void playSecondTetromino() {
            Debug.Log(TAG + ": playSecondTetromino()"); 
            // Debug.Log(TAG + " ViewModel.buttonInteractableList[1]: " + ViewModel.buttonInteractableList[1]); 
            if (ViewModel.buttonInteractableList[1] == 0) return;
            ViewModel.playSecondTetromino(previewTetromino, previewTetromino2, cycledPreviewTetromino);

            Debug.Log(TAG + " (previewTetromino2 != null): " + (previewTetromino2 != null));
            ViewManager.nextTetromino.Value = previewTetromino2;

            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
            
            // // disables: comTetroView eduTetroView swaBtn
            // // enables: undoButton toggleButton fallButton
            // if (ViewModel.gameMode == 0) {
            //     ViewModel.buttonInteractableList[0] = 0;
            //     ViewModel.buttonInteractableList[1] = 0;
            //     ViewModel.buttonInteractableList[2] = 0;
            //     ViewModel.buttonInteractableList[3] = 1;
            //     ViewModel.buttonInteractableList[4] = 1;
            //     ViewModel.buttonInteractableList[5] = 1;
            // }
            // // printViewModel.buttonInteractableList();
        }
        // private void preparePreviewTetrominoRecycle(previewTetrominoint i) { 
        //     cycledPreviewTetromino = i == 1 ? previewTetromino : previewTetromino2;
        //     // cycledPreviewTetromino.GetComponent<Rotate>().enabled = !cycledPreviewTetromino.GetComponent<Rotate>().enabled; // disable
        //     cycledPreviewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
        //     cycledPreviewTetromino.transform.position = Vector3.zero;
        //     cycledPreviewTetromino.transform.rotation = Quaternion.identity;
        //     cycledPreviewTetromino.SetActive(false);
        // }
        // private void recycleNextTetromino(G) {
        //     Debug.Log(TAG + ": recycleNextTetromino()"); 
        //     if (nextTetromino != null) {
        //         nextTetromino.tag = "Untagged";
        //         nextTetromino.GetComponent<Tetromino>().enabled = false;
        //         ViewModel.resetGridAfterDisappearingNextTetromino(nextTetromino);  // this one for undo click only ???? Nonono
        //         if (nextTetromino.transform.childCount == 4) {
        //             ViewManager.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
        //         } else 
        //             GameObject.Destroy(nextTetromino.gameObject);
        //     }
        //     // nextTetromino = null;
        // }
        // private void recycleThreeMajorTetromino() {
        //     // 回收三样东西：nextTetromino previewTetromino previewTetromino2
        //     recycleNextTetromino();
        //     preparePreviewTetrominoRecycle(previewTetromino1);
        //     ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     preparePreviewTetrominoRecycle(previewTetromino2);
        //     ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        // }
        public void onUndoGame() { // 分一部分的逻辑到视图模型中去
            Debug.Log(TAG + ": onUndoGame()");
            if (ViewModel.buttonInteractableList[3] == 0) return;
            Array.Clear(ViewModel.buttonInteractableList, 0, ViewModel.buttonInteractableList.Length);
            isDuringUndo = true;
            ViewModel.recycleThreeMajorTetromino(nextTetromino, previewTetromino, previewTetromino2);
            StringBuilder path = new StringBuilder("");
            // if (!string.IsNullOrEmpty(((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName)) 
            path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/game" + ".save");
            // else
            //     path.Append(Application.persistentDataPath + "/game" + ".save");
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            StringBuilder type = new StringBuilder("");
            if (ViewModel.hasDeletedMinos) {
                ViewModel.currentScore.Value  = gameData.score; // 这里要改
                ViewModel.currentLevel.Value  = gameData.level;
                numLinesCleared = gameData.lines;
                hud_score.text = ViewModel.currentScore.ToString();
                hud_level.text = ViewModel.currentLevel.ToString(); // 这不希望变的
                hud_lines.text = numLinesCleared.ToString();
                // Debug.Log(TAG + ": onUndoGame() current board before respawn"); 
                // MathUtil.printBoard(gridOcc); 
                
                Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                ViewModel.LoadDataFromParentList(gameData.parentList);
                GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            }
            ViewManager.moveCanvas.gameObject.SetActive(false);   // ViewManager.moveCanvas ViewManager.rotateCanvas: SetActive(false)
            ViewManager.rotateCanvas.gameObject.SetActive(false);
            if (ViewModel.prevPreview != null) { // previewTetromino previewTetromino2
                type.Length = 0;
                string type2 = ViewModel.prevPreview2;
                SpawnPreviewTetromino(type.Append(ViewModel.prevPreview).ToString(), type2);
            }
            ViewModel.buttonInteractableList[0] = 1; 
            ViewModel.buttonInteractableList[1] = 1; 
            ViewModel.buttonInteractableList[2] = 1; 
            ViewModel.buttonInteractableList[3] = 0; // buttons are supposed to click once at a time only
            isDuringUndo = false;
        }
        
        void LoadGame(string path) {  // when load Scene load game: according to gameMode
            Debug.Log(TAG + ": LoadGame()");
            // ViewModel.InitializationForNewGame(); // com for tmp
            // if (ViewModel.gameMode == 0)
            //     ViewModel.resetGridOccBoard(); 
            GameData gameData = SaveSystem.LoadGame(path);
            // ViewModel.gameMode = gameData.gameMode;
            // ViewModel.currentScore.Value  = gameData.score;
            // ViewModel.currentLevel.Value  = gameData.level;
            // numLinesCleared = gameData.lines;
            // hud_score.text = ViewModel.currentScore.ToString();
            // hud_level.text = ViewModel.currentLevel.ToString();
            // hud_lines.text = numLinesCleared.ToString();
            
            // hud_canvas.enabled = true; // 这个是需要根据不同的mode 来进行处理的 ?????
// 这部分被观察者模式取代了            
            // if (ViewModel.gameMode > 0) { // disable some components
            //     comTetroView.SetActive(false);
            //     eduTetroView.SetActive(false);
            //     swaBtn.SetActive(false);
            //     undoButton.SetActive(false);
            // }
            // Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            // ViewModel.LoadDataFromParentList(gameData.parentList);
            // currentActiveTetromino: if it has NOT landed yet
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                nextTetromino = ViewManager.GetFromPool(
                    type.Append(gameData.nextTetrominoData.type).ToString(),
                    // gameData.nextTetrominoData.transform,
                    // gameData.nextTetrominoData.transform);
                    DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                    DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform), Vector3.one);
                nextTetromino.tag = "currentActiveTetromino";
                // if (defaultContainer == null) // 我不要再管这个东西了
                //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // nextTetromino.transform.SetParent(defaultContainer.transform, false);
                nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
                ViewModel.nextTetrominoType.Value = nextTetromino.GetComponent<TetrominoType>().type;
                ViewManager.moveCanvas.gameObject.SetActive(true);
                ViewManager.moveCanvas.transform.position = new Vector3(ViewManager.moveCanvas.transform.position.x, nextTetromino.transform.position.y, ViewManager.moveCanvas.transform.position.z);
                // 也需要重新设置 ViewManager.rotateCanvas 的位置
                SpawnGhostTetromino();
            }
            // previewTetromino previewTetromino2
            type.Length = 0;
            string type2 = ViewModel.previewTetromino2Type;
            SpawnPreviewTetromino(type.Append(ViewModel.previewTetrominoType).ToString(), type2);
            if (ViewModel.prevPreview != null) {
                ViewModel.prevPreview = ViewModel.prevPreview;
                ViewModel.prevPreview2 = ViewModel.prevPreview2;
            } 
            // MainCamera rotation
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData);
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            
            if (nextTetromino != null && nextTetromino.CompareTag("currentActiveTetromino")) // Performance Bug: CompareTag()
                gameStarted = true;
            ViewModel.loadSavedGame = false;
            ViewModel.loadSavedGame = false;
        }    
        private static void currentActiveTetrominoPrepare() {
            Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            ViewManager.nextTetromino.Value.tag = "currentActiveTetromino";
            ViewManager.nextTetromino.Value.transform.rotation = Quaternion.identity;
            ViewManager.nextTetromino.Value.AddComponent<Tetromino>();
            Debug.Log(TAG + " (ViewManager.nextTetromino.Value.GetComponent<Tetromino>() != null): " + (ViewManager.nextTetromino.Value.GetComponent<Tetromino>() != null));

            //if (ViewModel.gameMode.Value == 0 && (ViewModel.gridWidth == 3 || ViewModel.gridWidth == 4)) {
            //    ViewManager.nextTetromino.Value.transform.localPosition = new Vector3(1.0f, ViewModel.gridHeight - 1f, 1.0f);
            //} else 
            //    ViewManager.nextTetromino.Value.transform.localPosition = new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f);
            //Tetromino tmp = ViewManager.nextTetromino.Value.GetComponent<Tetromino>();
            //Debug.Log(TAG + " (tmp != null): " + (tmp != null));
            //if (tmp != null)
            //    Debug.Log(TAG + " tmp.activeSelf: " + tmp.enabled);

            // Debug.Log(TAG + " (defaultContainer == null) before: " + (defaultContainer == null)); 
            // if (defaultContainer == null)
            //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
            // Debug.Log(TAG + " (defaultContainer == null) after: " + (defaultContainer == null)); 
            // nextTetromino.transform.SetParent(defaultContainer.transform, false);
// 下面这里跟以前的预设做得不一样:需要对象池第一次创建的时候根据不同的类型来添加脚本            
            // nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
// // 下面这行是被我弄掉了
//             nextTetromino.AddComponent<Tetromino>();
            // ViewModel.nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type; // 可以去掉了
            //Debug.Log(TAG + " nextTetromino.name: " + nextTetromino.name);
        }
        
        public void onActiveTetrominoMove(TetrominoMoveEventInfo info) { 
            Debug.Log(TAG + ": onTetrominoMove()");
            if (nextTetromino.GetComponent<Tetromino>().IsMoveValid) {
                ViewManager.moveCanvas.transform.position += info.delta;
                if ((int)info.delta.y != 0) {
                    ViewManager.rotateCanvas.transform.position += new Vector3(0, info.delta.y, 0);
                }
                ViewModel.UpdateGrid(nextTetromino);
            }
        }
        public void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            // Debug.Log(TAG + ": onActiveTetrominoRotate()"); 
            if (nextTetromino.GetComponent<Tetromino>().IsRotateValid) {
                ViewModel.UpdateGrid(nextTetromino); 
            }
        }
        
        public void onSwapPreviewTetrominos () {
            Debug.Log(TAG + ": swapPreviewTetrominosFunc()");
            if (ViewModel.buttonInteractableList[2] == 0) return;
            ViewModel.preparePreviewTetrominoRecycle(previewTetromino); // recycle 1st tetromino first
           ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            ViewModel.preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
            ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            SpawnPreviewTetromino();
        }
        
        // public void onSwapPreviewTetrominos(SwapPreviewsEventInfo swapInfo) {
        //     // Debug.Log(TAG + ": swapPreviewTetrominos()");
        //     if (ViewModel.buttonInteractableList[2] == 0) return;
        //     // Debug.Log(TAG + " swapInfo.tag.ToString(): " + swapInfo.tag.ToString()); 
        //     preparePreviewTetrominoRecycle(previewTetromino1); // recycle 1st tetromino first
        //     ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
        //     ViewManager.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     SpawnPreviewTetromino();
        // }
    
        private void moveRotatecanvasPrepare() { // 暂时放在视图,仍然应该是在视图模型中的
            // Debug.Log(TAG + ": moveRotatecanvasPrepare()"); 
            ViewManager.moveCanvas.transform.localPosition = new Vector3(2.1f, ViewModel.gridHeight - 1f, 2.1f);     
            ViewManager.rotateCanvas.transform.localPosition = new Vector3(2.1f, ViewModel.gridHeight - 1f, 2.1f);

            ViewManager.moveCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);

			ViewModel.isMovement = false;
            ViewModel.toggleButtons(1);
        }
        public string GetGhostTetrominoType(GameObject gameObject) { // ghostTetromino
            Debug.Log(TAG + ": GetGhostTetrominoType()"); 
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            string tmp = gameObject.name.Substring(9, 1);
            switch(tmp) {
            case "T" : type.Append("shadowT"); break;
            case "I" : type.Append("shadowI"); break;
            case "J" : type.Append("shadowJ"); break;
            case "L" : type.Append("shadowL"); break;
            case "O" : type.Append("shadowO"); break;
            case "S" : type.Append("shadowS"); break;
            case "Z" : type.Append("shadowZ"); break;
            }
            return type.ToString(); 
        }    
        public void DisableMoveRotationCanvas() {
            Debug.Log(TAG + ": DisableMoveRotationCanvas()"); 
            ViewManager.moveCanvas.gameObject.SetActive(false);
            ViewManager.rotateCanvas.SetActive(false);
        }
    
        public void SpawnGhostTetromino() {
            // Debug.Log(TAG + ": SpawnGhostTetromino() nextTetromino.tag: " + nextTetromino.tag); 
            GameObject tmpTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            // Debug.Log(TAG + ": SpawnGhostTetromino() (tmpTetromino == null): " + (tmpTetromino == null)); 
            ViewManager.ghostTetromino.Value = ViewManager.GetFromPool(GetGhostTetrominoType(ViewManager.nextTetromino.Value), ViewManager.nextTetromino.Value.transform.position, ViewManager.nextTetromino.Value.transform.rotation, Vector3.one);
// 因为暂时还不想写协程相关的代码,这里先现加再载载,绕道,晚点儿重构时再一一改回来
            // ghostTetromino.GetComponent<GhostTetromino>().enabled = true;
            ViewManager.ghostTetromino.Value.AddComponent<GhostTetromino>();
        }
        void Update() {
            ViewModel.UpdateScore();
            UpdateUI();
            ViewModel.UpdateLevel();
            ViewModel.UpdateSpeed();
            // CheckUserInput();  // this is a bug need to be fixed, the screen is flashing
        }
        void UpdateUI() {
            // Debug.Log(TAG + ": UpdateUI()");
            // Debug.Log(TAG + " ViewModel.currentScore: " + ViewModel.currentScore);
            // Debug.Log(TAG + " (hud_score != null): " + (hud_score != null)); 
            hud_score.text = ViewModel.currentScore.ToString();
            hud_level.text = ViewModel.currentLevel.ToString();
            hud_lines.text = numLinesCleared.ToString();
        }
        public void GameOver() {
            Debug.Log(TAG + ": GameOver()"); 
            ViewModel.UpdateHighScore();
            // SceneManager.LoadScene("GameOver");
        }
        // 平移与旋转两套按钮的上下移动: 应该是放在视图里的吧;最好是写成观察者模式,UI观察数据的变化,UI事件触发下发更新数据指令
        public void MoveDown() {
            ViewManager.moveCanvas.transform.position += new Vector3(0, -1, 0);
            ViewManager.rotateCanvas.transform.position += new Vector3(0, -1, 0);
        }
        public void MoveUp() {
            Debug.Log(TAG + ": MoveUp()");
            MathUtil.print(nextTetromino.transform.position);
            
            ViewManager.moveCanvas.transform.position += new Vector3(0, 1, 0);
            ViewManager.rotateCanvas.transform.position += new Vector3(0, 1, 0);
        }
        // 需要有来自ViewModel的数据变化来刷新UI: 观察者模式观察视图模型中数据的变体
        protected override void OnInitialize() {
            base.OnInitialize();

            tmp = GameObject.FindChildByName("TetrominoL");

			managers = GameObject.FindChildByName("managers");
            //poolManager = managers.GetComponent<PoolManager>();
            //Debug.Log(TAG + " (poolManager != null): " + (poolManager != null));

// // 我并不需要真的来加空上东西,有这具脚本或是说游戏应用里有这个管理器帮助模块化管理应用就可以了,它不是必须要依附在控件上的
            // 以另一种方式热更新:把脚本加进了预制里去了
//             poolRoot.AddComponent<PoolManager>(); 

            baseBoard5 = GameObject.FindChildByName("BaseBoard5");
            setAllBaseBoardInactive();
// 当视图想要通过视图模型来获取父视图模型的数据时,实际上当前视图模型还没能启动好,还没有设置好其父视图模型,所以会得到空,要换种写法
            // switch (((MenuViewModel)BindingContext.ParentViewModel).gridWidth) { // 大方格的类型
            switch (((MenuViewModel)ViewManager.MenuView.BindingContext).gridWidth) {
                // 大方格的类型
            case 3:
                //     baseBoard3.SetActive(true);
                //     break;
            case 4:
                //     baseBoard4.SetActive(true);
                //     break;
            case 5:
                baseBoard5.SetActive(true);
                break;
            }
// 测试 moveCanvas rotateCanvas SetActive(true);
            // ViewManager.moveCanvas.SetActive(true);
            // ViewManager.rotateCanvas.SetActive(true);

// 预览两块方块砖的按钮
            comTetroView = GameObject.FindChildByName("ComTetroView");
            eduTetroView = GameObject.FindChildByName("EduTetroView");
// 这里按键的检测本身是没有问题的,出问题的只是游戏逻辑中视图模型里某些变量的设置;
            pvBtnOne = GameObject.FindChildByName("ptoBtn").GetComponent<Button>();
            pvBtnOne.onClick.AddListener(playFirstTetromino); 
            pvBtnTwo = GameObject.FindChildByName("pteBtn").GetComponent<Button>();
            pvBtnTwo.onClick.AddListener(playSecondTetromino);

            scoText = GameObject.FindChildByName("scoTxt").GetComponent<Text>();
            lvlText = GameObject.FindChildByName("lvlTxt").GetComponent<Text>();
            linText = GameObject.FindChildByName("linTxt").GetComponent<Text>();
            pauBtn = GameObject.FindChildByName("pauBtn").GetComponent<Button>();
            pauBtn.onClick.AddListener(OnClickPauButton);
            falBtn = GameObject.FindChildByName("falBtn").GetComponent<Button>();
            falBtn.onClick.AddListener(OnClickFalButton);
            togBtn = GameObject.FindChildByName("togBtn").GetComponent<Button>();
            // togBtn.onClick.AddListener(OnClickTogButton); // toggle moveCanvas rotateCanvas
            togBtn.onClick.AddListener(ViewModel.toggleButtons); // toggle moveCanvas rotateCanvas
            swaBtn = GameObject.FindChildByName("swaBtn").GetComponent<Button>();
            // swaBtn.onClick.AddListener(OnClickSwaButton);
            swaBtn.onClick.AddListener(onSwapPreviewTetrominos);
            undBtn = GameObject.FindChildByName("undBtn").GetComponent<Button>();
            undBtn.onClick.AddListener(OnClickUndButton);
            ptoBtn = GameObject.FindChildByName("ptoBtn").GetComponent<Button>();
            ptoBtn.onClick.AddListener(OnClickPtoButton);
            pteBtn = GameObject.FindChildByName("pteBtn").GetComponent<Button>();
            pteBtn.onClick.AddListener(OnClickPteButton);
// pausePanel里的5个按钮
            pausePanel = GameObject.FindChildByName("MidMenuView"); // MidMenuView: 它仍然是空父节点GameView的众多子节点之一
            savBtn = GameObject.FindChildByName("savBtn").GetComponent<Button>();
            savBtn.onClick.AddListener(OnClickSavButton);
            resBtn = GameObject.FindChildByName("resBtn").GetComponent<Button>();
            resBtn.onClick.AddListener(OnClickResButton);
            guiBtn = GameObject.FindChildByName("guiBtn").GetComponent<Button>();
            guiBtn.onClick.AddListener(OnClickGuiButton);
            manBtn = GameObject.FindChildByName("manBtn").GetComponent<Button>();
            manBtn.onClick.AddListener(OnClickManButton);
            creBtn = GameObject.FindChildByName("creBtn").GetComponent<Button>();
            creBtn.onClick.AddListener(OnClickCreButton);

            cycledPreviewTetromino = new GameObject();
// 四个平移方向
            ViewManager.leftBtn.onClick.AddListener(OnClickLeftButton);
            ViewManager.rightBtn.onClick.AddListener(OnClickRightButton);
            ViewManager.upBtn.onClick.AddListener(OnClickUpButton);
            ViewManager.downBtn.onClick.AddListener(OnClickDownButton);
// 六个旋转方向
            ViewManager.XPosBtn.onClick.AddListener(OnClickXPosButton);
            ViewManager.XNegBtn.onClick.AddListener(OnClickXNegButton);
            ViewManager.YPosBtn.onClick.AddListener(OnClickYPosButton);
            ViewManager.YNegBtn.onClick.AddListener(OnClickYNegButton);
            ViewManager.ZPosBtn.onClick.AddListener(OnClickZPosButton);
            ViewManager.ZNegBtn.onClick.AddListener(OnClickZNegButton);

// // 这里写得有点儿牛头不对马尾,要再改            
//             managers = GameObject.FindChildByName("managers");
//             managers.AddComponent<AudioSource>();
// // AudioManager 单例模式
//             AudioManager.Instance.audioSource = managers.GetComponent<AudioSource>();
//             AudioManager.Instance.InitializeAudioClips();
        }
        // void OnClickTogButton() {
        // }
// 现在的这个四个方向都是对的了，可以如此解决其它旋转6个方向的问题
// 要再回去研究一下方块砖的旋转是如何实现的        
        void OnClickXPosButton() {
            
        }
        void OnClickXNegButton() {
            
        }
        void OnClickYPosButton() {
            
        }
        void OnClickYNegButton() {
            
        }
        void OnClickZPosButton() {
            
        }
        void OnClickZNegButton() {
            
        }
        void OnClickLeftButton() {
            ViewModel.nextTetroPos.Value += new Vector3(-1, 0, 0);
        }
        void OnClickRightButton() {
            ViewModel.nextTetroPos.Value += new Vector3(1, 0, 0);
        }
        void OnClickUpButton() {
            ViewModel.nextTetroPos.Value += new Vector3(0, 0, 1);
        }
        void OnClickDownButton() {
            ViewModel.nextTetroPos.Value += new Vector3(0, 0, -1);
        }
        
        // 想找一个更为合适的地方来写上面的观察者模式监听回调
        public void OnRevealed() {
            base.OnRevealed();
            // 注册对视图模型数据的监听观察回调等
// 观察监听当前方块砖的位置/平移旋转缩放的变化
            ViewModel.currentScore.OnValueChanged += onCurrentScoreChanged;
            ViewModel.currentLevel.OnValueChanged += onCurrentLevelChanged;
            ViewModel.numLinesCleared.OnValueChanged += onNumLinesCleared;
// 再测试一下这个: 还仍然是不过
            // ViewModel.gameMode.OnValueChanged += onGameModeChanged; // 运行时仍为空抛异常,因为它的初始化的过程会相对复杂那么一点点儿

// // 不想要游戏视图来观察,要对象池来观察,只想游戏视图中持一个对象池的引用            
// // 预览中的两个方块砖类型变了:要生成实例并刷新
//             ViewModel.comTetroTyep.OnValueChanged += onComTetroTypeChanged;
//             ViewModel.eduTetroTyep.OnValueChanged += onEduTetroTypeChanged;

// GameView: nextTetromino position, rotation, localScale        
            ViewModel.nextTetroPos.OnValueChanged += onNextTetroPosChanged;
            ViewModel.nextTetroRot.OnValueChanged += onNextTetroRotChanged;
            ViewModel.nextTetroSca.OnValueChanged += onNextTetroScaChanged;

            // 对于启蒙模式下,这里的游戏逻辑就是说,在加载视图的时候就需要去实例化两个方块砖,并在显示视图的时候显示出来给看
            Start();
        }
// TODO:不仅仅是当前方块砖的位置/旋转/缩放,还该包括阴影的相关观察
        void onNextTetroPosChanged(Vector3 pre, Vector3 cur) {
            if (ViewManager.nextTetromino.Value.activeSelf) // 必须当前有正在运行的方块砖
                ViewManager.nextTetromino.Value.transform.position = cur;
        }
        void onNextTetroRotChanged(Quaternion pre, Quaternion cur) {
            if (ViewManager.nextTetromino.Value.activeSelf) // 必须当前有正在运行的方块砖
                ViewManager.nextTetromino.Value.transform.rotation = cur;
        }
        void onNextTetroScaChanged(Vector3 pre, Vector3 cur) {
            if (ViewManager.nextTetromino.Value.activeSelf) // 必须当前有正在运行的方块砖
                ViewManager.nextTetromino.Value.transform.localScale = cur;
        }
        void onCurrentScoreChanged(int pre, int cur) {
            hud_score.text = cur.ToString();
        }
        void onCurrentLevelChanged(int pre, int cur) {
            hud_level.text = cur.ToString();
        }
        void onNumLinesCleared(int pre, int cur) {
            hud_lines.text = cur.ToString();
        }
    }
}

