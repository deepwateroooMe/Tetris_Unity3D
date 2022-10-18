﻿using System.IO;
using System.Text;
using Framework.MVVM;
using HotFix.Control;
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

        public override void OnRevealed() {
            base.OnRevealed();
            // 当当前视图显示结束之后,还是应该之前开始显示的时候呢,调用游戏开始的初始化等相关配置
            Start();
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
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);

            tmpTransform = emptyGO.transform;
            audioSource = GetComponent<AudioSource>();

            if (!string.IsNullOrEmpty(GameMenuData.Instance.saveGamePathFolderName)) {
                gameMode = GameMenuData.Instance.gameMode;
                loadSavedGame = GameMenuData.Instance.loadSavedGame;
                StringBuilder path = new StringBuilder("");
                if (gameMode > 0)
                    path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game.save");
                else 
                    path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/grid" + gridWidth + "/game.save");
                if (loadSavedGame) {
                    LoadGame(path.ToString());
                } else {
                    LoadNewGame();
                }
            } else {
                LoadNewGame();
            }

            currentLevel = startingLevel;
            startingHighScore = PlayerPrefs.GetInt("highscore");
            startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        
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

        void OnClickPauButton() { // public void PauseGame()
            Time.timeScale = 0f;	    
            audioSource.Pause(); // ui
            ViewModel.isPaused = true;
            
            // Bug: disable all Hud canvas buttons: swap
            audioSource.Pause(); // ui
            pausePanel.SetActive(true); // ui

            // Bug cleaning: when paused game, if game has NOT started yet, disable Save Button
            if (!ViewModel.gameStarted) {
                
            }

            //ViewModel.PauseGame(); // 游戏暂停
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
        void OnClickResButton() { // RESUME GAME: 隐藏当前游戏过程中的视图,就可以了 // public void onResumeGame();
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
            //    cleanUpGameBroad();
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
            //cleanUpGameBroad();
            //ViewModel.isPaused = false;
            //Time.timeScale = 1.0f;
            //SceneManager.LoadScene("GameMenu");
        }
        public void onNoToNotSaveGame() { // 如何才能够延迟加载呢？
            //ViewModel.hasSavedGameAlready = false;
            //saveGameReminderPanel.SetActive(false);
            //pausePanel.SetActive(false);
            //cleanUpGameBroad();
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

        void setAllBaseBoardInactive() {
            baseBoard3.SetActive(false);
            baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }
    }
}