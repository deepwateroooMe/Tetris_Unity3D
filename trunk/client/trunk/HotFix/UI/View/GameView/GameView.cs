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
    
    // 还有两个世界坐标系的视图：MoveCanvasView 和RotateCanvasView供其调控
    public class GameView : UnityGuiView {
        private const string TAG = "GameView"; 

        public override string BundleName {
            get {
                return "/view/gameview";
            }
        }
        public override string AssetName {
            get {
                return "GameView";
            }
        }
        public override string ViewName {
            get {
                return "GameView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(GameViewModel).FullName;
            }
        }
        public GameViewModel ViewModel {
            get {
                return (GameViewModel)BindingContext;
            }
        }
        public override bool IsRoot { // 方便调控显示与隐藏
            get {
                return true;
            }
        }

// Five Four ThreeGridView
        // 因为整合到一个大的视图,那么要同时有三种格子,只显示其中的一种,并隐藏剩余的两种而已
        GameObject baseBoard3;
        GameObject baseBoard4;
        GameObject baseBoard5;
// DesView 里的个文本框基本不变，不用管它们        
// ScoreDataView
        Text scoText; // Score Text
        Text lvlText; // Level Text
        Text linText; // Line Text
// StaticBtnsView
        Button pauButton; // pause Game
        Button falButton; // fall fast button
// ToggleBtnView
        int type = 0;
        Button togButton; // Togcation
// EduBtnsView
        Button swaButton; // swap current tetrominos to be a newly generating(it's coming after click) tetromino set
        Button undButton; // undo last selected tetromino landing, revert it back

// ComTetroView
        Button ptoButton; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个
// EduTetroView
        Button pteButton; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个

// TODO: pausePanel: 把 MidMenuView 整合进来,统一管理
        GameObject pausePanel;
        Button savBtn; // SAVE GAME
        Button resBtn; // RESUME GAME
        Button guiBtn; // TUTORIAL
        Button manBtn; // BACK TO MAIN MENU
        Button creBtn; // CREDIT

// 私有的当前视图的元件? 暂时添加上AudioSouce元件顺承原混作一团的逻辑(感觉它不该是被AudioManager管才对吗?)
        AudioSource audioSource;
        AudioSource m_ExplosionAudio;

        public GameObject moveCanvas;    // public ?
        public GameObject rotateCanvas;
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
        public GameObject previewSet;
        public GameObject previewSet2;
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

        // public string prevPreview; // to remember previous spawned choices
        // public string prevPreview2;
        // public string nextTetrominoType;  
        // public string previewTetrominoType; 
        // public string previewTetromino2Type;

        private SaveGameEventInfo saveGameInfo;

        private GameObject tmpParentGO;

        public GameObject previewSelectionButton;
        public GameObject previewSelectionButton2;
        public GameObject swapPreviewTetrominoButton;
        public GameObject undoButton;
        
        public GameObject savedGamePanel;
        public GameObject saveGameReminderPanel;

        private bool isDuringUndo = false;
        public bool saveForUndo = true;

        // private GameObject baseBoard;
        
        public override void OnRevealed() {
            base.OnRevealed();
            // 当当前视图显示结束之后,还是应该之前开始显示的时候呢,调用游戏开始的初始化等相关配置
            Start();
        }

        // 需要有来自ViewModel的数据变化来刷新UI: 观察者模式观察视图模型中数据的变体
        protected override void OnInitialize() {
            base.OnInitialize();
            setAllBaseBoardInactive(); // 重置全部隐藏
            switch (((EducaModesViewModel)BindingContext.ParentViewModel).GridWidth) { // 大方格的类型
            case 3:
                baseBoard3.SetActive(true);
                break;
            case 4:
                baseBoard4.SetActive(true);
                break;
            case 5:
                baseBoard5.SetActive(true);
                break;
            }
            
            scoText = GameObject.FindChildByName("scoTxt").GetComponent<Text>();
            lvlText = GameObject.FindChildByName("lvlTxt").GetComponent<Text>();
            linText = GameObject.FindChildByName("linTxt").GetComponent<Text>();

            pauButton = GameObject.FindChildByName("pauBtn").GetComponent<Button>();
            pauButton.onClick.AddListener(OnClickPauButton);
            falButton = GameObject.FindChildByName("falBtn").GetComponent<Button>();
            falButton.onClick.AddListener(OnClickFalButton);

            togButton = GameObject.FindChildByName("togBtn").GetComponent<Button>();
            togButton.onClick.AddListener(OnClickTogButton);

            swaButton = GameObject.FindChildByName("swaBtn").GetComponent<Button>();
            swaButton.onClick.AddListener(OnClickSwaButton);
            undButton = GameObject.FindChildByName("undBtn").GetComponent<Button>();
            undButton.onClick.AddListener(OnClickUndButton);

            ptoButton = GameObject.FindChildByName("ptoBtn").GetComponent<Button>();
            ptoButton.onClick.AddListener(OnClickPtoButton);
            pteButton = GameObject.FindChildByName("pteBtn").GetComponent<Button>();
            pteButton.onClick.AddListener(OnClickPteButton);

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

            moveCanvas = GameObject.FindChildByName("moveCanvas");
            rotateCanvas = GameObject.FindChildByName("rotateCanvas");
        }

// 新的热更新框架里,游戏是如何开始的呢?
        void Start () { // 感觉这些逻辑放在视图里出很牵强,哪些是可以放在模型里的呢?
            Debug.Log(TAG + ": Start()");

            // check if it is cleaned up first
            Debug.Log(TAG + " (!EventManager.Instance.isCleanedUp()): " + (!EventManager.Instance.isCleanedUp())); 
            if (!EventManager.Instance.isCleanedUp()) {
                EventManager.Instance.cleanUpLists();
            }
            // if (gameMode == 0) {
            // EventManager.Instance.RegisterListener<SwapPreviewsEventInfo>(onSwapPreviewTetrominos); 
            // EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            // EventManager.UndoButtonClicked += onUndoGame;
            // EventManager.SwapButtonClicked += onSwapPreviewTetrominos;
            // }
            EventManager.Instance.RegisterListener<SaveGameEventInfo>(SaveGame); 
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); // 自己重构时commented out for tmp

            // tmpTransform = emptyGO.transform;
            // audioSource = GetComponent<AudioSource>();
            // if (!string.IsNullOrEmpty(GameMenuData.Instance.saveGamePathFolderName)) {
            //     gameMode = GameMenuData.Instane.gameMode;
            //     loadSavedGame = GameMenuData.Instance.loadSavedGame;
            //     StringBuilder path = new StringBuilder("");
            //     if (gameMode > 0)
            //         path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game.save");
            //     else 
            //         path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/grid" + gridWidth + "/game.save");
            //     if (loadSavedGame) {
            //         LoadGame(path.ToString());
            //     } else {
            //         LoadNewGame();
            //     }
            // } else {
            //     LoadNewGame();
            // }
            // currentLevel = startingLevel;
            // startingHighScore = PlayerPrefs.GetInt("highscore");
            // startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            // startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        
            //1.粒子特效的GameObject实例化完毕。
            //2.确保粒子所用到的贴图载入内存
            //3.让粒子进行一次预热（目前预热功能只能在循环的粒子特效里面使用，所以不循环的粒子特效是不能用的）
            // 粒子系统的实例化，何时销毁？
            // 出于性能考虑，其中Update内部的操作也可以移至FixedUpdate中进行以减少更新次数，但是视觉上并不会带来太大的差异

            //// temporatorily don't consider these yet
            //string particleType = "particles";
            //// m_ExplosionParticles = PoolManager.Instance.GetFromPool(GetSpecificPrefabType(m_ExplosionPrefab)).GetComponent<ParticleSystem>();
            //m_ExplosionParticles = PoolManager.Instance.GetFromPool(particleType).GetComponent<ParticleSystem>();
            ////m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
            //m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
            //m_ExplosionParticles.gameObject.SetActive(false);
            //// 因为实例化粒子特效以后，实际上粒子的脚本就已经完成了初始化的工作，也就是Awake()和OnEnable()方法。然后设置SetActive(false)仅仅是把粒子特效隐藏起来。
        }
        
        void OnClickPauButton() { // public void PauseGame()
            ViewModel.PauseGame(); // 游戏暂停

            // Bug: disable all Hud canvas buttons: swap
            pausePanel.SetActive(true);
            ViewManager.MidMenuView.Reveal();
            // Hide(); // 若是隐藏,这里也只会隐藏当前视图- StaticBtnsView一个视图,而不是游戏场景里的所有小视图
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
            audioSource.Play();
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
            //savedGamePanel.SetActive(true);
        }
        public void onSavedGamePanelOK() {
            //savedGamePanel.SetActive(false);
        }
        public void onBackToMainMenu() {
            //if (!ViewModel.hasSavedGameAlready && gameStarted) { // gameStarted
            //    saveGameReminderPanel.SetActive(true);
            //} else {
            //    cleanUpGameBroad(nextTetromino);
            //    ViewModel.isPaused = false;
            //    Time.timeScale = 1.0f;
            //    SceneManager.LoadScene("GameMenu");
            //}
        }
        public void onYesToSaveGame() {
            //ViewModel.saveForUndo = false;
            //onGameSave();
            //ViewModel.hasSavedGameAlready = true;
            //saveGameReminderPanel.SetActive(false);
            //pausePanel.SetActive(false);
            //cleanUpGameBroad(nextTetromino);
            //ViewModel.isPaused = false;
            //Time.timeScale = 1.0f;
            //SceneManager.LoadScene("GameMenu");
        }
        public void onNoToNotSaveGame() { // 如何才能够延迟加载呢？
            //ViewModel.hasSavedGameAlready = false;
            //saveGameReminderPanel.SetActive(false);
            //pausePanel.SetActive(false);
            //cleanUpGameBroad(nextTetromino);
            //ViewModel.isPaused = false;
            //Time.timeScale = 1.0f;
            //if (gameMode == 1)
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
            //SceneManager.LoadScene("GameMenu"); // commented out for tmp
        }
#endregion

        void onGameSave() { // 是指将游戏进度存到本地数据文件
            Debug.Log(TAG + ": onGameSave()");
// 不能直接删:偶合偶合,会影响其它逻辑 !!!            
            //if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事,为什么要生成一个新的空的呢?
            //    // 是因为过程中的某个步骤用到了这个,可是也应该在那个要用的时候重置或是生成一个新的呀,而不是这里,那个时候都是些什么逻辑!!!
            //    tmpTransform = new GameObject().transform;
            SaveSystem.SaveGame(ViewModel);
        }

        void setAllBaseBoardInactive() {
            baseBoard3.SetActive(false);
            baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }
        IEnumerator asyncLoadScene() {
            // AsyncOperation async = SceneManager.LoadSceneAsync("GameMenu");
            // yield return async;
            yield return null;
        }
        // void onGameSave(SaveGameEventInfo info) {
        //void onGameSave() {
        //    Debug.Log(TAG + ": onGameSave()");
        //    if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
        //        tmpTransform = new GameObject().transform;
        //    SaveSystem.SaveGame(this);
        //}
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
        //         PoolManager.Instance.ReturnToPool(ghostTetromino, ghostTetromino.GetComponent<TetrominoType>().type);
        //     }
        // }
        
        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            // gameMode = ((MenuViewModel)ViewModel.ParentViewModel).gameMode;
            // fallSpeed = 3.0f; // should be recorded too, here
            // if (gameMode == 0)
            //     resetGridOccBoard();
            ViewModel.InitializationForNewGame();
            SpawnnextTetromino();  

// 不是说,要把这些都写成是观察者模式的吗?
            // currentScore = 0;
            hud_score.text = "0";
            // currentLevel = startingLevel;
            hud_level.text = ViewModel.currentLevel.ToString();
            hud_lines.text = "0";

            if (ViewModel.gameMode > 0) { // disable some components
                previewSelectionButton.SetActive(false);
                previewSelectionButton2.SetActive(false);
                swapPreviewTetrominoButton.SetActive(false);
                undoButton.SetActive(false);
            }
            rotateCanvas.SetActive(false);
        }
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

        private void SpawnPreviewTetromino() {
            Debug.Log(TAG + ": SpawnPreviewTetromino()"); 
            previewTetromino = PoolManager.Instance.GetFromPool(
                ViewModel.GetRandomTetromino(), previewTetrominoPosition,
                Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(previewSet.transform, false);

            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            ViewModel.previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;

            Debug.Log(TAG + " (previewTetromino != null): " + (previewTetromino != null)); 
            Debug.Log(TAG + " previewTetromino.name: " + previewTetromino.name); 
            
            if (ViewModel.gameMode == 0) { // previewTetromino2
                // excepts: undoButton toggleButton fallButton
                ViewModel.buttonInteractableList[3] = 0;
                ViewModel.buttonInteractableList[4] = 0;
                ViewModel.buttonInteractableList[5] = 0;

                previewTetromino2 = PoolManager.Instance.GetFromPool(
                    ViewModel.GetRandomTetromino(), previewTetromino2Position, 
                    Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                ViewModel.previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                // previewTetromino2.layer = LayerMask.NameToLayer("UI"); // not working on this RayCast button click right now
            }
        }
        private void SpawnPreviewTetromino(string type1, string type2) {
            previewTetromino = PoolManager.Instance.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(previewSet.transform, false);
            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            ViewModel.previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;

            if (ViewModel.gameMode == 0) { // previewTetromino2
                previewTetromino2 = PoolManager.Instance.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                ViewModel.previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
            }
            ViewModel.buttonInteractableList[3] = 1; // undoButton
        }
        public void playFirstTetromino() {
            Debug.Log(TAG + ": playFirstTetromino()");
            Debug.Log(TAG + " ViewModel.buttonInteractableList[0]: " + ViewModel.buttonInteractableList[0]); 
            if (ViewModel.buttonInteractableList[0] == 0) return;
            ViewModel.prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            ViewModel.prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            ViewModel.preparePreviewTetrominoRecycle(previewTetromino2);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
            // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            
            nextTetromino = previewTetromino;
            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();

            // disables: previewSelectionButton previewSelectionButton2 swapPreviewTetrominoButton
            // enables: undoButton toggleButton fallButton
            if (ViewModel.gameMode == 0) {
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
            Debug.Log(TAG + " ViewModel.buttonInteractableList[1]: " + ViewModel.buttonInteractableList[1]); 
            if (ViewModel.buttonInteractableList[1] == 0) return;

            ViewModel.playSecondTetromino(previewTetromino, previewTetromino2, cycledPreviewTetromino);
            // ViewModel.prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            // ViewModel.prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            // ViewModel.preparePreviewTetrominoRecycle(previewTetromino);
            // PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            // previewTetromino2.transform.localScale -= ViewModel.previewTetrominoScale;
            // // previewTetromino2.layer = LayerMask.NameToLayer("Default");
            // // previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;

            nextTetromino = previewTetromino2;
            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
            
            // // disables: previewSelectionButton previewSelectionButton2 swapPreviewTetrominoButton
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
        public void SpawnnextTetromino() {
            Debug.Log(TAG + ": SpawnnextTetromino()");
            if (!gameStarted) {
                if (ViewModel.gameMode == 0) {
                    SpawnPreviewTetromino();
                } else {
                    gameStarted = true;
                    nextTetromino = PoolManager.Instance.GetFromPool(
                        ViewModel.GetRandomTetromino(),
                        new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f),
                        Quaternion.identity);
                    currentActiveTetrominoPrepare();
                    moveCanvas.gameObject.SetActive(true);  
                    SpawnGhostTetromino();
                    SpawnPreviewTetromino();
                }
            } else {
                previewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
                // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;

                nextTetromino = previewTetromino;
                currentActiveTetrominoPrepare();
                
                SpawnGhostTetromino();  
                moveRotatecanvasPrepare();
                SpawnPreviewTetromino();
            }
        }

        // private void recycleNextTetromino(G) {
        //     Debug.Log(TAG + ": recycleNextTetromino()"); 
        //     if (nextTetromino != null) {
        //         nextTetromino.tag = "Untagged";
        //         nextTetromino.GetComponent<Tetromino>().enabled = false;
        //         ViewModel.resetGridAfterDisappearingNextTetromino(nextTetromino);  // this one for undo click only ???? Nonono
        //         if (nextTetromino.transform.childCount == 4) {
        //             PoolManager.Instance.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
        //         } else 
        //             GameObject.Destroy(nextTetromino.gameObject);
        //     }
        //     // nextTetromino = null;
        // }
        // private void recycleThreeMajorTetromino() {
        //     // 回收三样东西：nextTetromino previewTetromino previewTetromino2
        //     recycleNextTetromino();
        //     preparePreviewTetrominoRecycle(previewTetromino1);
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     preparePreviewTetrominoRecycle(previewTetromino2);
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        // }

        public void onUndoGame() { // 分一部分的逻辑到视图模型中去
            Debug.Log(TAG + ": onUndoGame()");
            if (ViewModel.buttonInteractableList[3] == 0) return;
            Array.Clear(ViewModel.buttonInteractableList, 0, ViewModel.buttonInteractableList.Length);
            isDuringUndo = true;
            ViewModel.recycleThreeMajorTetromino(nextTetromino, previewTetromino, previewTetromino2);

            StringBuilder path = new StringBuilder("");
            // if (!string.IsNullOrEmpty(GameMenuData.Instance.saveGamePathFolderName)) 
            path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/game" + ".save");
            // else
            //     path.Append(Application.persistentDataPath + "/game" + ".save");
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            StringBuilder type = new StringBuilder("");
            if (ViewModel.hasDeletedMinos) {
                ViewModel.currentScore = gameData.score; // 这里要改
                ViewModel.currentLevel = gameData.level;
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
            moveCanvas.gameObject.SetActive(false);   // moveCanvas rotateCanvas: SetActive(false)
            rotateCanvas.gameObject.SetActive(false);
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
            if (ViewModel.gameMode == 0)
                ViewModel.resetGridOccBoard(); 
            GameData gameData = SaveSystem.LoadGame(path);
            ViewModel.gameMode = gameData.gameMode;
            
            ViewModel.currentScore = gameData.score;
            ViewModel.currentLevel = gameData.level;
            numLinesCleared = gameData.lines;
            hud_score.text = ViewModel.currentScore.ToString();
            hud_level.text = ViewModel.currentLevel.ToString();
            hud_lines.text = numLinesCleared.ToString();
            
            // hud_canvas.enabled = true; // 这个是需要根据不同的mode 来进行处理的
            if (ViewModel.gameMode > 0) { // disable some components
                previewSelectionButton.SetActive(false);
                previewSelectionButton2.SetActive(false);
                swapPreviewTetrominoButton.SetActive(false);
                undoButton.SetActive(false);
            }

            Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            ViewModel.LoadDataFromParentList(gameData.parentList);

            // currentActiveTetromino: if it has NOT landed yet
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null)
            {
                nextTetromino = PoolManager.Instance.GetFromPool(
                    type.Append(gameData.nextTetrominoData.type).ToString(),
                    gameData.nextTetrominoData.transform,
                    gameData.nextTetrominoData.transform);
                    //DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                    //                                             DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform));
                nextTetromino.tag = "currentActiveTetromino";
                // if (defaultContainer == null) // 我不要再管这个东西了
                //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // nextTetromino.transform.SetParent(defaultContainer.transform, false);
                nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
                ViewModel.nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;

                moveCanvas.gameObject.SetActive(true);
                moveCanvas.transform.position = new Vector3(moveCanvas.transform.position.x, nextTetromino.transform.position.y, moveCanvas.transform.position.z);
                // 也需要重新设置 rotateCanvas 的位置
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
        private void currentActiveTetrominoPrepare() {
            Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            nextTetromino.tag = "currentActiveTetromino";
            nextTetromino.transform.rotation = Quaternion.identity;

            if (ViewModel.gameMode == 0 && (ViewModel.gridWidth == 3 || ViewModel.gridWidth == 4)) {
                nextTetromino.transform.localPosition = new Vector3(1.0f, ViewModel.gridHeight - 1f, 1.0f);
            } else 
                nextTetromino.transform.localPosition = new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f);
            
            // Debug.Log(TAG + " (defaultContainer == null) before: " + (defaultContainer == null)); 
            // if (defaultContainer == null)
            //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
            // Debug.Log(TAG + " (defaultContainer == null) after: " + (defaultContainer == null)); 
            // nextTetromino.transform.SetParent(defaultContainer.transform, false);
            
            nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
            ViewModel.nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;
            Debug.Log(TAG + " nextTetromino.name: " + nextTetromino.name);
        }
        
        public void onActiveTetrominoMove(TetrominoMoveEventInfo info) { 
            Debug.Log(TAG + ": onTetrominoMove()");
            if (nextTetromino.GetComponent<Tetromino>().IsMoveValid) {
                moveCanvas.transform.position += info.delta;
                if ((int)info.delta.y != 0) {
                    rotateCanvas.transform.position += new Vector3(0, info.delta.y, 0);
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
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            ViewModel.preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            SpawnPreviewTetromino();
        }
        
        // public void onSwapPreviewTetrominos(SwapPreviewsEventInfo swapInfo) {
        //     // Debug.Log(TAG + ": swapPreviewTetrominos()");
        //     if (ViewModel.buttonInteractableList[2] == 0) return;
        //     // Debug.Log(TAG + " swapInfo.tag.ToString(): " + swapInfo.tag.ToString()); 
        //     preparePreviewTetrominoRecycle(previewTetromino1); // recycle 1st tetromino first
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     SpawnPreviewTetromino();
        // }
    
        private void moveRotatecanvasPrepare() { // 暂时放在视图,仍然应该是在视图模型中的
            // Debug.Log(TAG + ": moveRotatecanvasPrepare()"); 
            moveCanvas.transform.localPosition = new Vector3(2.1f, ViewModel.gridHeight - 1f, 2.1f);     
            rotateCanvas.transform.localPosition = new Vector3(2.1f, ViewModel.gridHeight - 1f, 2.1f);
            ViewModel.isMovement = false;
            ViewModel.toggleButtons(1);
        }

        public string GetGhostTetrominoType(GameObject gameObject) { // ghostTetromino
            Debug.Log(TAG + ": GetGhostTetrominoType()"); 
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            string tmp = gameObject.name.Substring(10, 1);
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
            moveCanvas.gameObject.SetActive(false);
            rotateCanvas.SetActive(false);
        }
    
        public void SpawnGhostTetromino() {
            //Debug.Log(TAG + ": SpawnGhostTetromino() nextTetromino.tag: " + nextTetromino.tag); 
            GameObject tmpTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            //Debug.Log(TAG + ": SpawnGhostTetromino() (tmpTetromino == null): " + (tmpTetromino == null)); 
            ghostTetromino = PoolManager.Instance.GetFromPool(GetGhostTetrominoType(nextTetromino), nextTetromino.transform.position, nextTetromino.transform.rotation);
            ghostTetromino.GetComponent<GhostTetromino>().enabled = true;
        }

        void Update() {
            ViewModel.UpdateScore();
            UpdateUI();
            ViewModel.UpdateLevel();
            ViewModel.UpdateSpeed();
            //CheckUserInput();  // this is a bug need to be fixed, the screen is flashing
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
            moveCanvas.transform.position += new Vector3(0, -1, 0);
            rotateCanvas.transform.position += new Vector3(0, -1, 0);
        }
        public void MoveUp() {
            Debug.Log(TAG + ": MoveUp()");
            MathUtil.print(nextTetromino.transform.position);
            
            moveCanvas.transform.position += new Vector3(0, 1, 0);
            rotateCanvas.transform.position += new Vector3(0, 1, 0);
        }
   }
}





