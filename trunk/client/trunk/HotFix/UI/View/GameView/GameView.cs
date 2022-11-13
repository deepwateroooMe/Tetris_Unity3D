using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
using Framework.Util;
using HotFix.Control;
using HotFix.Data;
using tetris3d;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {
    
    public class GameView : UnityGuiView {
        private const string TAG = "GameView";
        public override string BundleName { get { return "ui/view/gameview"; } }
        public override string AssetName { get { return "GameView"; } }
        public override string ViewName { get { return "GameView"; } }
        public override string ViewModelTypeName { get { return typeof(GameViewModel).FullName; } }
        public GameViewModel ViewModel { get { return (GameViewModel)BindingContext; } }
        public override bool IsRoot { get { return true; } }
        
// 基础游戏大方格
        // GameObject baseBoard3; // 这些控件还没有做,哪天头脑昏昏的时候才再去做
        // GameObject baseBoard4;
        GameObject baseBoard5;
// DesView 里的个文本框基本不变，不用管它们        
// ScoreDataView 
        Text scoText; // Score Text
        Text lvlText; // Level Text
        Text linText; // Line Text
        GameObject linTextDes; // Line Text Description: LINE
// ComTetroView 按钮控件
        GameObject pvBtnOne; // ptoBtn 
// EduTetroView 按钮控件
        GameObject pvBtnTwo; // pteBtn
// ToggleBtnView
        public GameObject togBtn; // Togcation
// StaticBtnsView
        GameObject pauBtn; // pause Game
        GameObject falBtn; // fall fast button
// EduBtnsView
        GameObject swaBtn; // swap current tetrominos to be a newly generating(it's coming after click) tetromino set
        GameObject undoBtn; // UNDO last selected tetromino landing, revert it back
// pausePanel: 
        GameObject pausePanel;
        Button savBtn; // SAVE GAME
        Button resBtn; // RESUME GAME 
        Button guiBtn; // TUTORIAL
        Button manBtn; // BACK TO MAIN MENU
        Button creBtn; // CREDIT
// Save current game or not panel
        GameObject saveGameOrNotPanel;
        Button saveBtn; // save
        Button nosvBtn; // not saving
        Button cancBtn; // cancel: back to pause Panel
// GAME OVER TMP PANEL;
        GameObject gameOverPanel;
        
// isChallengeMode:
        GameObject comLevelView;
        GameObject goalPanel;
        GameObject initCubes;
        Text swapCnter;
        Text undoCnter;
        Text tetroCnter;
        
// 私有的当前视图的元件? 暂时添加上AudioSouce元件顺承原混作一团的逻辑(感觉它不该是被AudioManager管才对吗?)
        public Canvas hud_canvas;
        public Button invertButton;
        public Sprite newImg;
        public Sprite prevImg;

        public GameObject defaultContainer;
        private GameObject cycledPreviewTetromino;
        private bool gameStarted = false;

        public static Vector3 nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1, 2.0f);
        private Vector3 previewTetrominoPosition = new Vector3(-17f, -5f, -9.81f);
        private Vector3 previewTetromino2Position = new Vector3(-68.3f, 19.6f, 32.4f); 
        public int numLinesCleared = 0;
        public GameObject emptyGO;
        public GameObject previewTetromino;
        public GameObject previewTetromino2;
        
        private GameObject comTetroView; // 这里更多的应该是指 ComTetroView EduTetroView组合组件(会激或是失活某组组件)
        private GameObject eduTetroView;
        
        private bool isDuringUndo = false;
        public bool saveForUndo = true;
     
        private static GameObject tmpTetro; // TODO: why static ?

        private bool isMoveValid = false;
        private bool isRotateValid = false;
        private Vector3 delta;
        private StringBuilder type = new StringBuilder("");

        public ModelMono modelMono;

// TODO:对按钮,更该用状态来管理,而不是彻底看不见,应该设置为可见不可点击回调        
        public Dictionary<GameObject, bool> btnState;

        // 游戏主场景: 得分等游戏进展框,自动观察视图模型的数据自动,刷新
        void onGameModeChanged(int pre, int cur) {
            Debug.Log(TAG + " onGameModeChanged(): " + " cur: " + cur);
            if (cur > 0) {
                eduTetroView.SetActive(false); 
                swaBtn.SetActive(false);
                undoBtn.SetActive(false); // 不可撤销(挑战模式是仍可以再考虑)
            } else if (cur == 0 && GloData.Instance.isChallengeMode) { // 挑战模式 下
                linText.gameObject.SetActive(false);
                linTextDes.SetActive(false); // LINE 
                initializeChallengingMode();
            }
        }

        void initializeChallengingMode() {
            comLevelView.SetActive(true);
            goalPanel.SetActive(true);
            lvlText.text = GloData.Instance.gameLevel.ToString();
// 每个层级的底坐: 这里可能还需要更多的控件索引,因为底座需要能够更新材质                
            ViewManager.basePlane.SetActive(true);
            
        }

        public void SpawnnextTetromino() {
            Debug.Log(TAG + ": SpawnnextTetromino()");
            Debug.Log(TAG + " gameStarted: " + gameStarted);
            if (!gameStarted) {
                if (ViewModel.gameMode.Value == 0) {
                    SpawnPreviewTetromino();
                } else {
                   gameStarted = true;
                   ViewModel.nextTetrominoType.Value = ViewModel.GetRandomTetromino();
                   ViewManager.nextTetromino = PoolHelper.GetFromPool(
                       ViewModel.nextTetrominoType.Value,
                       nextTetrominoSpawnPos,
                       Quaternion.identity, Vector3.one);
                   currentActiveTetrominoPrepare();
                   // if (ViewModel.gameMode.Value == 0) {
                   ViewManager.moveCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);
                   ViewManager.moveCanvas.gameObject.SetActive(true);  
                   // }                   
                   SpawnGhostTetromino();
                   SpawnPreviewTetromino();
               }
            } else {
               previewTetromino.transform.localScale -= ViewModel.previewTetrominoScale;
               ViewManager.nextTetromino = previewTetromino;
               currentActiveTetrominoPrepare();
                
               SpawnGhostTetromino();
               if (ViewModel.gameMode.Value == 0)
                   moveRotatecanvasPrepare();
               SpawnPreviewTetromino();
            }
        }

        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            ViewModel.LoadNewGame();

// 感觉这里好鬼魅呀:视图里已经进行这时在了,可是视图模型因为反射调用还没有启动完?!!!
// 这里的问题:应该是用IEnumerator去解决,但暂时这样; 在自己不断修改整理这些重构的代码的过程中,会对这些忘记了的相对陌生的一点点儿捡回来的
// 现在这里要做的就是:打预设的资源包(很简单),从资源包里将不是的预设读出来,存在字典里,供游戏需要的时候来用            
            SpawnnextTetromino();   
        }

        private static float levelGoalDisplayTime = 1.2f;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(levelGoalDisplayTime);
        IEnumerator displayChallengeGoal() {
            Debug.Log(TAG + " displayChallengeGoal()");
// TODO: 这里还有点儿准备工作是说,面板里的内容需要更改一下
			//guiManager.InitLevelGoal(); 
			yield return _waitForSeconds;
			goalPanel.SetActive(false);
        }
        void loadInitCubesforChallengeMode() {
            initCubes = GameObject.FindGameObjectWithTag("InitCubes");
            Debug.Log(TAG + " (initCubes != null): " + (initCubes != null));
            if (initCubes != null) {
                Debug.Log(TAG + " loadInitCubesforChallengeMode() initCubes.transform.childCount: " + initCubes.transform.childCount);
                Model.UpdateGrid(initCubes); // parent GameObject

                Debug.Log(TAG + ": gridOcc()"); // 这里不知道是怎么回事,想要它打印三维数据的时候,它还没有处理完,总是打不出来
                MathUtilP.printBoard(Model.gridOcc);
                Debug.Log(TAG + ": gridClr()");
                MathUtilP.printBoard(Model.gridClr);
            } else initCubes = new GameObject();
        }
        public GameObject [] cubes; // baseCubesGO;
        public void Start() { // 感觉这些逻辑放在视图里出很牵强,哪些是可以放在模型里的呢?
            Debug.Log(TAG + ": Start()");
            
// TODO: 在这里初始化模型数据 ? 这个显示不成功, 就是coroutine的那个等一秒没有执行成功
            CoroutineHelperP.StartCoroutine(displayChallengeGoal());

            if (ViewModel.isChallengeMode)
                loadInitCubesforChallengeMode();
            else initCubes = new GameObject();
            
            // if (gameMode == 0 && !isChallengeMode) { 
            //     // guiManager.setAllBaseBoardInactive();
            //     switch (Model.gridWidth) {
            //     case 3:
            //         guiManager.baseBoard3.SetActive(true);
            //         MainScene_GUIManager.baseBoard = guiManager.baseBoard3;
            //         break;
            //     case 4:
            //         guiManager.baseBoard4.SetActive(true);
            //         MainScene_GUIManager.baseBoard = guiManager.baseBoard4;
            //         break;
            //     case 5:
            //         guiManager.baseBoard5.SetActive(true);
            //         MainScene_GUIManager.baseBoard = guiManager.baseBoard5;
            //         break;
            //     }
            // }
            // tmpTransform = emptyGO.transform;

            if (!string.IsNullOrEmpty(GloData.Instance.saveGamePathFolderName)) {
// // cmt for tmp
//                 // if (!GloData.Instance.isChallengeMode)
//                 gameMode = GloData.Instance.gameMode;
//                 // else
//                 //     gameMode = 1;
                // loadSavedGame = GloData.Instance.loadSavedGame;
                // type.Length = 0;
                // if (ViewModel.gameMode.Value > 0 || GloData.Instance.isChallengeMode) // clear flag
                //     type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/game.save");
                // else 
                //     type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/grid" + Model.gridWidth + "/game.save");
                if (GloData.Instance.loadSavedGame) {
                    // LoadGame(type.ToString());
                } else {
                    LoadNewGame();
                }
            } else {
                LoadNewGame();
            }
            
// TODO: 这里有个加载原保存过的游戏的过程            
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
// // cmt for tmp
//             LoadNewGame(); // <<<<<<<<<<<<<<<<<<<< 
            //     }
            // } else {
            //     LoadNewGame();
            // }
            // currentLevel = startingLevel;
            // startingHighScore = PlayerPrefs.GetInt("highscore");
            // startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            // startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        }
// PauseGame, ResumeGame        
        void OnClickResButton() { // RESUME GAME: 隐藏当前游戏过程中的视图,就可以了 // public void OnClickResButton();
            Debug.Log(TAG + " OnClickResButton");
            Time.timeScale = 1.0f;
            ViewModel.isPaused = false;
            pausePanel.SetActive(false);
            EventManager.Instance.FireEvent("resumegame"); // for Audio
        }
        
        void setAllBaseBoardInactive() {
            // baseBoard3.SetActive(false);
            // baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }

        public void CheckUserInput() {  // originally pasuseButton & continueButton
            Debug.Log(TAG + ": CheckUserInput()"); 
            if (Time.timeScale == 1.0f) {
                // PauseGame(); 不知道这里说的是哪个暂停游戏方法
                OnClickPauButton();
            } else {
                OnClickResButton();
            }
        }

        void LoadGame(string path) {  // when load Scene load game: according to gameMode
            // Debug.Log(TAG + ": LoadGame()");
            GameData gameData = deepwaterooo.tetris3d.SaveSystem.LoadGame(path);
            StringBuilder type = new StringBuilder("");
            // Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                ViewManager.nextTetromino = PoolHelper.GetFromPool(
                    type.Append(gameData.nextTetrominoData.type).ToString(),
                    DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                    DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform), Vector3.one);
                ViewManager.nextTetromino.tag = "currentActiveTetromino";
                bool isEnabled = ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled;
                ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = !isEnabled;
                ViewModel.nextTetrominoType.Value = ViewManager.nextTetromino.GetComponent<TetrominoType>().type;
                ViewManager.moveCanvas.gameObject.SetActive(true);
                ViewManager.moveCanvas.transform.position = new Vector3(ViewManager.moveCanvas.transform.position.x, ViewManager.nextTetromino.transform.position.y, ViewManager.moveCanvas.transform.position.z);
                // 也需要重新设置 ViewManager.rotateCanvas 的位置
                SpawnGhostTetromino();
            }
            // previewTetromino previewTetromino2
            type.Length = 0;
			string type2 = ViewModel.eduTetroType.Value;
            SpawnPreviewTetromino(type.Append(ViewModel.comTetroType.Value).ToString(), type2);
            if (ViewModel.prevPreview != null) {
                ViewModel.prevPreview = ViewModel.prevPreview;
                ViewModel.prevPreview2 = ViewModel.prevPreview2;
            } 
            // MainCamera rotation
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData);
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            
            if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.CompareTag("currentActiveTetromino")) // Performance Bug: CompareTag()
                gameStarted = true;
            ViewModel.loadSavedGame = false;
            ViewModel.loadSavedGame = false;
        }    

        // TODO: 这个系统是,4+6个按钮触发事件(会有假阳性,会误播背景音乐),先发送所有事件,再接收后才判断是否合理,只有背景音乐受影响
#region eventsCallbacks
        public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            btnState[togBtn] = false;

// 最近一个月刚开始重做这个项目的时候没有拿到更原始功能更多的版本,所以最开始的缺少了很多后来挑战模块下的源码和逻辑,以及粒子系统等
// 在适配进热更新工程后,现因要整合挑战模块,把原本也只修改了几个BUG的前版本就不要了,直接用现在抽象独立出来的Model ModelMono (原本被我全写在GameViewModel里)
// 这只是众多重构过程中的一个小节,无关任何其它. 爱表哥,爱生活!!!            
            // ViewModel.UpdateGrid(ViewManager.nextTetromino); 
            
            Debug.Log(TAG + ": gridOcc[][][] BEFORE Land BEFORE UpdateGrid()"); 
            MathUtilP.printBoard(Model.gridOcc);  // Model.

            Model.UpdateGrid(ViewManager.nextTetromino); 

            Debug.Log(TAG + ": gridOcc[][][] AFTER Land  Update AFTER UpdteGrid(); BEFORE onGameSave()"); 
 
            MathUtilP.printBoard(Model.gridOcc);  // Model.
            // Debug.Log(TAG + ": gridClr[,,] aft Land UpdateGrid(), bef onGameSave()"); 
            // MathUtilP.printBoard(gridClr);  // Model.


            if (GloData.Instance.isChallengeMode) {
                if (ChallengeRules.isValidLandingPosition()) {
                    EventManager.Instance.FireEvent("challLand");
                } else { // print color board
                    Debug.Log(TAG + ": color board before GAME OVER ");
                    MathUtilP.printBoard(Model.gridClr);
                    // Debug.Log(TAG + ": Game Over()"); 
                    Debug.Log(TAG + " game over");
                    GameOver(); // 为什么这里是game over ? 因为当前方块砖放错位置了,一定要适应周围小立方体的材质
                }
            }

            if (ViewModel.isChallengeMode)
                ViewModel.onGameSave(initCubes.transform);
            else ViewModel.onGameSave(null);

            Debug.Log(TAG + " (ViewModel.gameMode.Value == 0 && !GloData.Instance.isChallengeMode): " + (ViewModel.gameMode.Value == 0 && !GloData.Instance.isChallengeMode));

            
            // if (ViewModel.gameMode.Value > 0 || (GloData.Instance.isChallengeMode && (GloData.Instance.challengeLevel < 3 || GloData.Instance.challengeLevel > 5))) // 1 2 6 7 8 9 10
            // else if (((ViewModel.gameMode.Value == 0 && !GloData.Instance.isChallengeMode) || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel > 2 && GloData.Instance.challengeLevel < 6)) // 3 4 5
            if (ViewModel.gameMode.Value > 0 || GloData.Instance.isChallengeMode) // 需要整平面消除的
                ModelMono.DeleteRow();
            else if (ViewModel.gameMode.Value == 0 && !GloData.Instance.isChallengeMode // 启蒙模式,想要带点儿粒子特效的
                     && !ModelMono.isDeleteRowCoroutineRunning)
                CoroutineHelperP.StartCoroutine(ModelMono.Instance.DeleteRowCoroutine()); // the case
            
// 后面一部分,只在持战模式下,当剩余的方块砖的数目为0时才游戏结束
            bool tmp = Model.CheckIsAboveGrid(ComponentHelper.GetTetroComponent(ViewManager.nextTetromino));
            Debug.Log(TAG + " Model.CheckIsAboveGrid(ComponentHelper.GetTetroComponent(ViewManager.nextTetromino)): " + tmp);
            if (Model.CheckIsAboveGrid(ComponentHelper.GetTetroComponent(ViewManager.nextTetromino)) || GloData.Instance.isChallengeMode && ViewModel.tetroCnter.Value == 0) {
                Debug.Log(TAG + " (ViewModel.tetroCnter.Value == 0): " + (ViewModel.tetroCnter.Value == 0));
                GameOver();
            }

// 这是被自己控制过的逻辑,要补完整            
            Array.Clear(ViewModel.buttonInteractableList, 0, ViewModel.buttonInteractableList.Length);
            if (ViewModel.gameMode.Value  == 0) {
                ViewModel.buttonInteractableList[0] = 1;
                ViewModel.buttonInteractableList[1] = 1;
                ViewModel.buttonInteractableList[2] = 1;
                ViewModel.buttonInteractableList[3] = 1; // undo button
            }
            
// TODO那么,这下面的逻辑是放在哪里处理的呢?            
            
// nextTetromino 的相关处理: 
// ViewManager.nextTetromino: 在接下来消除的过程中，它的部分或是全部立方体可能会被消除，所以要早点儿处理
            ViewManager.nextTetromino.tag = "Untagged";
            Tetromino tetromino = ComponentHelper.GetTetroComponent(ViewManager.nextTetromino);
            ViewModel.currentScore.Value += ViewManager.scoreDic[ViewManager.nextTetromino.name];
            // ViewModel.currentScore.Value += tetromino.GetComponent<TetrominoType>().score;
            tetromino.enabled = false;

            PoolHelper.recycleGhostTetromino();

            if (((MenuViewModel)ViewModel.ParentViewModel).gameMode != 0) 
                SpawnnextTetromino();
        }

// 因为ViewManager.nextTetromino是静态的,将相关逻辑移到这个类里来管理, 不可以这样      
        void onActiveTetrominoMove(TetrominoMoveEventInfo info) { // MoveDown:所有事件都是有效的
            ViewManager.nextTetromino.transform.position += info.delta;
            isMoveValid = Model.CheckIsValidPosition();
            // Debug.Log(TAG + " isMoveValid: " + isMoveValid);
            // MathUtilP.print(ViewManager.nextTetromino.transform.position);
            if (isMoveValid)
                EventManager.Instance.FireEvent("validMR", "move", info.delta);
            else 
                ViewManager.nextTetromino.transform.position -= info.delta;
            // MathUtilP.print(ViewManager.nextTetromino.transform.position);
        }
        void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            ViewManager.nextTetromino.transform.Rotate(info.delta);
            if (Model.CheckIsValidPosition()) {
                isRotateValid = true;
                EventManager.Instance.FireEvent("validMR", "rotate", info.delta);
            } else {
                ViewManager.nextTetromino.transform.Rotate(Vector3.zero - info.delta);
            }
        }
#endregion        

#region GameViewCallbacks
// 游戏主面板中7个按钮的点击回调
        public void OnClickUndButton() {
            EventManager.Instance.FireEvent("undo");
        }
        public void onUndoGame(UndoLastTetrominoInfo info) { // 新系统,需要适配这套系统
            Debug.Log(TAG + " onUndoGame() isDuringUndo: " + isDuringUndo);
// TODO: THERE IS A onUndoGame() irresponsible bug here to be fixed            
            if (isDuringUndo) return ;
// TODO: 对行列场景的处理
            if (gameOverPanel.activeSelf)
                gameOverPanel.SetActive(false);
            Debug.Log(TAG + " (ViewModel.buttonInteractableList[2] == 0): " + (ViewModel.buttonInteractableList[2] == 0));
            Debug.Log(TAG + " (ViewModel.buttonInteractableList[2] == 0 || GloData.Instance.isChallengeMode && ViewModel.undoCnter.Value == 0): " + (ViewModel.buttonInteractableList[2] == 0 || GloData.Instance.isChallengeMode && ViewModel.undoCnter.Value == 0));
            if (ViewModel.buttonInteractableList[2] == 0 || GloData.Instance.isChallengeMode && ViewModel.undoCnter.Value == 0) return;
            isDuringUndo = true;

            Debug.Log(TAG + ": gridOcc[][][] BEFORE ViewModel.onUndoGame(gameData)"); 
            MathUtilP.printBoard(Model.gridOcc);  
            
            StringBuilder path = new StringBuilder("");
// TODO: 关于保存路径的BUG
            if (ViewModel.gameMode.Value > 0)
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/game.save");
            else
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName
                            + (GloData.Instance.isChallengeMode ? "challenge/level" + GloData.Instance.gameLevel.ToString() : "educational/grid" + ViewModel.gridSize.ToString())
                            + "/game.save");
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            ViewModel.onUndoGame(gameData);

            Debug.Log(TAG + " (gameData.prevPreview != null): " + (gameData.prevPreview != null));
            if (gameData.prevPreview != null) { // 生成早了,会被视图模型又回收走了?
                type.Length = 0;
                string type2 = gameData.prevPreview2;
                if (gameData.isChallengeMode) {
                    SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2, gameData.prevPreviewColor, gameData.prevPreviewColor2);
                } else 
                    SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2);
            }
            isDuringUndo = false;
        }

        void OnClickPauButton() { 
            Debug.Log(TAG + " OnClickPauButton");
            Time.timeScale = 0f;
            EventManager.Instance.FireEvent("pausegame");
            // ViewModel.PauseGame(); // 游戏暂停Reg
            pausePanel.SetActive(true);
// TODO : Bug: disable all Hud canvas buttons: swap etc
        }
        void OnClickFalButton() { // SlamDown FallFast
            Debug.Log(TAG + " OnClickFallFastButton");
// // TODO: 这个数组没有管理好,为的是一个防止用户防连击的效果(自动过滤掉处理回调过程中的后续连续点击)            
//             ViewModel.printbuttonInteractableList();
//             if (ViewModel.gameMode.Value == 0 && ViewModel.getSlamDownIndication() == 0) return; // 防用户连击
//             ViewModel.buttonInteractableList[5] = 0;
            delta = new Vector3(0, -1, 0);
            ViewManager.nextTetromino.transform.position += delta;
            while (Model.CheckIsValidPosition()) { 
                ViewManager.nextTetromino.transform.position -= delta;
                EventManager.Instance.FireEvent("move", delta);
                ViewManager.nextTetromino.transform.position += delta;
            } // 出循环就是不合理位置
            if (!Model.CheckIsValidPosition()) { // 方块砖移到了最底部
                ViewManager.nextTetromino.transform.position -= delta;
                EventManager.Instance.FireEvent("land");
            }
        }
// TODO: BUG 这个计数没能及时反映到UI上的刷新        
        public void onSwapPreviewTetrominos () { // 这里需要下发指令到视图数据层,并根据随机数生成的新的tetromino来重新刷新UI
            Debug.Log(TAG + " onSwapPreviewTetrominos");
            if (ViewModel.buttonInteractableList[2] == 0) return;
            Debug.Log(TAG + " ViewModel.swapCnter.Value: " + ViewModel.swapCnter.Value);
            --ViewModel.swapCnter.Value;
            // ViewModel.onSwapPreviewTetromino();
            
            // 当 ViewModel.swapCnter.Value == 1的时候点击,就将这个按钮短暂失活,直到重新游戏或是下一关卡
            if (ViewModel.swapCnter.Value == 0 && GloData.Instance.isChallengeMode) // 
                swaBtn.SetActive(false);
            Debug.Log(TAG + " ViewModel.swapCnter.Value: " + ViewModel.swapCnter.Value);
            PoolHelper.recyclePreviewTetrominos(previewTetromino);
            PoolHelper.recyclePreviewTetrominos(previewTetromino2);
            SpawnPreviewTetromino();
        }
// TODO: 当且仅当要为换方块砖配置音效的时候,才使用这个方法swap preview tetrominos, for Educational mode only
        // public void onSwapPreviewTetrominos(SwapPreviewsEventInfo swapInfo) { 
        //     Debug.Log(TAG + ": swapPreviewTetrominos()");
        //     if (ViewModel.buttonInteractableList[2] == 0) return;
        //     PoolHelper.preparePreviewTetrominoRecycle(previewTetromino); // recycle 1st tetromino first
        //     PoolHelper.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     PoolHelper.preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
        //     PoolHelper.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     SpawnPreviewTetromino();
        // }
        void OnClickTogButton() { // toggle moveCanvas rotateCanvas
			btnState[togBtn] = false;
            EventManager.Instance.FireEvent("canvas");
        }
        public void playFirstTetromino() { // pvBtnOne: comTetroView Button
            Debug.Log(TAG + " playFirstTetromino");
            // Debug.Log(TAG + " ViewModel.buttonInteractableList[0]: " + ViewModel.buttonInteractableList[0]); 
            if (ViewModel.buttonInteractableList[0] == 0) return;
            if (GloData.Instance.isChallengeMode) {
                ViewModel.prevPreviewColor = previewTetromino.GetComponent<TetrominoType>().color;
                ViewModel.prevPreviewColor2 = previewTetromino2.GetComponent<TetrominoType>().color;
            }
            ViewModel.playFirstTetromino(previewTetromino, previewTetromino2);

            currentActiveTetrominoPrepare();
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare(); // 
            SpawnPreviewTetromino();
            btnState[togBtn] = true;
        }
        public void playSecondTetromino() { // pvBtnTwo: eduTetroView Button
            Debug.Log(TAG + " playSecondTetromino");
            Debug.Log(TAG + " ViewModel.buttonInteractableList[1]: " + ViewModel.buttonInteractableList[1]); 
            if (ViewModel.buttonInteractableList[1] == 0) return;
            if (GloData.Instance.isChallengeMode) {
                ViewModel.prevPreviewColor = previewTetromino.GetComponent<TetrominoType>().color;
                ViewModel.prevPreviewColor2 = previewTetromino2.GetComponent<TetrominoType>().color;
            }
            ViewModel.playSecondTetromino(previewTetromino, previewTetromino2);

            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
            btnState[togBtn] = true;
        }
#endregion

#region pausePanel Button Handlers
        // MidMenuView 里的5 个按钮, 以及的瑨延伸的3个按钮的点击回调
        void OnClickSavButton() { // SAVE GAME

        }
        void SaveGame(SaveGameEventInfo info) {
            Debug.Log(TAG + ": SaveGame()");
            ViewModel.saveForUndo = false;
            ViewModel.onGameSave(initCubes.transform);
            ViewModel.hasSavedGameAlready = true;
        }
        // public void onSavedGamePanelOK() {
        // }
        void OnClickManButton() { // back to MAIN menu
// TODO: reminder FOR USER if game is NOT saved            
            pausePanel.SetActive(false); 
            if (!ViewModel.hasSavedGameAlready && gameStarted) { // gameStarted
                saveGameOrNotPanel.SetActive(true);
            } else {
                Model.cleanUpGameBroad();
                ViewModel.isPaused = false;
                Time.timeScale = 1.0f;
                ViewManager.MenuView.Reveal(); // 现在的简单逻辑
                // Hide(); // TOGO, BUG: 这里还没能真正切换到主游戏菜单去
                ViewManager.GameView.Hide();
            }
        }
        public void onYesToSaveGame() {
            ViewModel.saveForUndo = false; // ? 这里是什么意思呢
            ViewModel.onGameSave(initCubes.transform);
            ViewModel.hasSavedGameAlready = true;
            saveGameOrNotPanel.SetActive(false);
            pausePanel.SetActive(false);
            Model.cleanUpGameBroad();
            ViewModel.isPaused = false;
            Time.timeScale = 1.0f;
// TODO: 检查一下这里还需要做哪些清理工作?
            ViewManager.MenuView.Reveal();
            Hide();
        }
        public void onNoToNotSaveGame() { // 因为每块方块砖落地时的自动保存,这里用户不需要保存时,要清除保存过的文件
            ViewModel.hasSavedGameAlready = false;
            saveGameOrNotPanel.SetActive(false);
            pausePanel.SetActive(false);

            PoolHelper.ReturnToPool(previewTetromino, previewTetromino.GetComponent<TetrominoType>().type);
            if (ViewModel.gameMode.Value == 0) 
                PoolHelper.ReturnToPool(previewTetromino2, previewTetromino2.GetComponent<TetrominoType>().type);
            Model.cleanUpGameBroad();
            
            ViewModel.isPaused = false;
            Time.timeScale = 1.0f;
            if (ViewModel.gameMode.Value == 1)
                gameStarted = false;
// TODO: BUG: still have to check this due to auto Save
            string path = new StringBuilder(Application.persistentDataPath).Append("/").Append(((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName).Append("/game.save").ToString();
            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }
            }
            ViewManager.MenuView.Reveal();
            Hide();
        }
        void OnClickCancButton() { 
            saveGameOrNotPanel.SetActive(false);
            pausePanel.SetActive(true);
        }
        void OnClickGuiButton() { // 可以视频GUIDE吗?
        }
        void OnClickCreButton() {
        }
#endregion

        private void currentActiveTetrominoPrepare() {
            // Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            ViewManager.nextTetromino.tag = "currentActiveTetromino";
            ViewManager.nextTetromino.transform.rotation = Quaternion.identity;
            ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = true;
            if (ViewModel.gameMode.Value == 0 && (ViewModel.gridSize == 3 || ViewModel.gridSize == 4)) {
                ViewManager.nextTetromino.transform.localPosition = new Vector3(1.0f, ViewModel.gridHeight - 1f, 1.0f);
            } else 
                ViewManager.nextTetromino.transform.localPosition = nextTetrominoSpawnPos;
            EventManager.Instance.FireEvent("spawned", ViewManager.nextTetromino.transform.localPosition);
        }
        
        private void moveRotatecanvasPrepare() { 
            ViewManager.moveCanvas.transform.localPosition = nextTetrominoSpawnPos; 
            ViewManager.rotateCanvas.transform.localPosition = nextTetrominoSpawnPos; 
            ViewManager.moveCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);   // 规正可能存在的微小转动
            ViewManager.rotateCanvas.transform.rotation = Quaternion.Euler(0, 0, 0); // 规正可能存在的微小转动
            ViewManager.moveCanvas.SetActive(true);
        }

        public void SpawnGhostTetromino() {
            Debug.Log(TAG + " SpawnGhostTetromino");
            // GameObject tmpTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            ViewManager.ghostTetromino = PoolHelper.GetFromPool(GetGhostTetrominoType(ViewManager.nextTetromino),
                                                                ViewManager.nextTetromino.transform.position,
                                                                ViewManager.nextTetromino.transform.rotation, Vector3.one);
            ComponentHelper.GetGhostComponent(ViewManager.ghostTetromino).enabled = true;
        }
        public string GetGhostTetrominoType(GameObject gameObject) { // ghostTetromino
            Debug.Log(TAG + ": GetGhostTetrominoType()"); 
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            string tmp = gameObject.name.Substring(9, 1);
            switch(tmp) {
            case "0" : type.Append("shadow0"); break;
            case "B" : type.Append("shadowB"); break;
            case "C" : type.Append("shadowC"); break;
            case "I" : type.Append("shadowI"); break;
            case "J" : type.Append("shadowJ"); break;
            case "L" : type.Append("shadowL"); break;
            case "O" : type.Append("shadowO"); break;
            case "R" : type.Append("shadowR"); break;
            case "S" : type.Append("shadowS"); break;
            case "T" : type.Append("shadowT"); break;
            case "Y" : type.Append("shadowY"); break;
            case "Z" : type.Append("shadowZ"); break;
            }
            return type.ToString(); 
        }
        
        public void GameOver() {
            Debug.Log(TAG + ": GameOver()"); 
            ViewModel.UpdateHighScore();
            // SceneManager.LoadScene("GameOver");
// TODO: 这里需要一个方便自己先看的临时有物面板
            gameOverPanel.SetActive(true);
        }
        void Update() {
            ViewModel.UpdateScore();
            UpdateUI(); // TODO COMMENT掉
            ViewModel.UpdateLevel();
            ViewModel.UpdateSpeed();
            // CheckUserInput();  // this is a bug need to be fixed, the screen is flashing
        }
        void UpdateUI() {
            scoText.text = ViewModel.currentScore.ToString();
            lvlText.text = ViewModel.currentLevel.ToString();
            linText.text = ViewModel.numLinesCleared.ToString();
            swapCnter.text = ViewModel.swapCnter.Value.ToString();
        }

        private void SpawnPreviewTetromino() {
            Debug.Log(TAG + ": SpawnPreviewTetromino()");
// 这里仍旧是写成观察者模式,视图观察视图模型的数据变化
            ViewModel.comTetroType.Value = ViewModel.GetRandomTetromino();
            Debug.Log(TAG + " ViewModel.comTetroType.Value: " + ViewModel.comTetroType.Value);
            previewTetromino = PoolHelper.GetFromPool(
                ViewModel.comTetroType.Value, previewTetrominoPosition,
                Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            
            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                // excepts: undoBtn toggleButton fallButton
                ViewModel.buttonInteractableList[3] = 0;
                ViewModel.buttonInteractableList[4] = 0;
                ViewModel.buttonInteractableList[5] = 0;
                ViewModel.eduTetroType.Value = ViewModel.GetRandomTetromino();
                Debug.Log(TAG + " ViewModel.eduTetroType.Value: " + ViewModel.eduTetroType.Value);
                previewTetromino2 = PoolHelper.GetFromPool(
                    ViewModel.eduTetroType.Value, previewTetromino2Position, 
                    Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(ViewManager.tetroParent.transform, false);
            }
        }

        private void SpawnPreviewTetromino(string type1, string type2) {
            Debug.Log(TAG + " SpawnPreviewTetromino() type1 type2");
            Debug.Log(TAG + " type1: " + type1);
            Debug.Log(TAG + " type2: " + type2);

            previewTetromino = PoolHelper.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            // ViewModel.comTetroType.Value = previewTetromino.GetComponent<TetrominoType>().type;
            ViewModel.comTetroType.Value = type1;
            ViewModel.previewTetrominoColor = previewTetromino.GetComponent<TetrominoType>().color;
            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                previewTetromino2 = PoolHelper.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(ViewManager.tetroParent.transform, false);
                ViewModel.eduTetroType.Value = type2;
                ViewModel.previewTetromino2Color = previewTetromino2.GetComponent<TetrominoType>().color;
            }
            ViewModel.buttonInteractableList[3] = 1; // undoBtn
        }

        private void SpawnPreviewTetromino(string type1, string type2, int color1, int color2) {
            previewTetromino = PoolHelper.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one, color1);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            ViewModel.comTetroType.Value = previewTetromino.GetComponent<TetrominoType>().type;
            ViewModel.previewTetrominoColor = previewTetromino.GetComponent<TetrominoType>().color;

            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                previewTetromino2 = PoolHelper.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one, color2);
                previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
                ViewModel.eduTetroType.Value = previewTetromino2.GetComponent<TetrominoType>().type;
                ViewModel.previewTetromino2Color = previewTetromino2.GetComponent<TetrominoType>().color;
            }
        }

// TODO: 可能更好的办法是去主工程基类中去全局控制,现先这样        
        private bool revealed = false;
#region BindableProperties
        public void OnRevealed() { // 写在这里是因为热更新程序域里回调慢,写早了有时候拿不到返回空
            base.OnRevealed();
            if (revealed) return ;
            ViewModel.currentScore.OnValueChanged += onCurrentScoreChanged;
            ViewModel.currentLevel.OnValueChanged += onCurrentLevelChanged;
            ViewModel.numLinesCleared.OnValueChanged += onNumLinesCleared;
// CHALLENGE MODE: COUNTERS
            ViewModel.tetroCnter.OnValueChanged += onTetroCnterChanged;
            ViewModel.swapCnter.OnValueChanged += onSwapCnterChanged;
            ViewModel.undoCnter.OnValueChanged += onUndoCnterChanged;
// TODO: 为了触发第一次的回调,稍微绕了一下,需要更为优雅的设置方法
            ViewManager.MenuView.ViewModel.mgameMode.OnValueChanged += onGameModeChanged; 
            if (ViewModel.gameMode.Value != ViewManager.MenuView.ViewModel.mgameMode.Value)
                ViewManager.MenuView.ViewModel.mgameMode.Value = ViewModel.gameMode.Value;
// 不想要游戏视图来观察,要对象池来观察[对象池的帮助方法都只能静态调用,可以观察吗?]暂先放这里
            //ViewModel.comTetroType.OnValueChanged += onComTetroTypeChanged;
            //ViewModel.eduTetroType.OnValueChanged += onEduTetroTypeChanged;
// 相机的位置变化:主要用于启蒙模式,用户㧤撤销某块方块砖的时候 ?
            ViewModel.cameraPos.OnValueChanged += onCameraPosChanged;
            ViewModel.cameraRot.OnValueChanged += onCameraRotChanged;

            Start(); // 开始游戏
            revealed = true;
        }
        void onTetroCnterChanged(int pre, int cur) {
            tetroCnter.text = cur.ToString();
        }
        void onSwapCnterChanged(int pre, int cur) {
            Debug.Log(TAG + " onSwapCnterChanged");
// TODO: 这里的值是变化了,但是UI没有刷新
            swapCnter.text = cur.ToString();
            Debug.Log(TAG + " swapCnter.text: " + swapCnter.text);
        }
        void onUndoCnterChanged(int pre, int cur) {
            undoCnter.text = cur.ToString();
        }
        void onCameraPosChanged(Vector3 pre, Vector3 cur) {
            Debug.Log(TAG + " onCameraPosChanged");
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = cur;
        }
        void onCameraRotChanged(Quaternion pre, Quaternion cur) {
            Debug.Log(TAG + " onCameraRotChanged");
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = cur;
        }
        void onComTetroTypeChanged(string pre, string cur) {
            Debug.Log(TAG + " onComTetroTypeChanged");
// TODO: 这里的逻辑需要的后来的整合优化            
            // PoolHelper.GetFromPool(cur);
        }
        void onEduTetroTypeChanged(String pre, string cur) {
            Debug.Log(TAG + " onEduTetroTypeChanged");
        }
        void onCurrentScoreChanged(int pre, int cur) {
            scoText.text = cur.ToString();
        }
        void onCurrentLevelChanged(int pre, int cur) {
            lvlText.text = cur.ToString();
        }
        void onNumLinesCleared(int pre, int cur) {
            Debug.Log(TAG + " onNumLinesCleared");
            Debug.Log(TAG + " cur: " + cur);
            linText.text = cur.ToString();
        }
#endregion
        
#region Initialize
        protected override void OnInitialize() {
            base.OnInitialize();
            RegisterListeners();

            baseBoard5 = GameObject.FindChildByName("BaseBoard5");
            setAllBaseBoardInactive();
// 当视图想要通过视图模型来获取父视图模型的数据时,实际上当前视图模型还没能启动好,还没有设置好其父视图模型,所以会得到空,要换种写法
            switch (ViewManager.MenuView.ViewModel.gridWidth) { // 大方格的类型
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
// 游戏得分文本框
            linTextDes = GameObject.FindChildByName("linTxtDes");
            scoText = GameObject.FindChildByName("scoTxt").GetComponent<Text>();
            lvlText = GameObject.FindChildByName("lvlTxt").GetComponent<Text>();
            linText = GameObject.FindChildByName("linTxt").GetComponent<Text>();
// 预览两块方块砖的按钮
            comTetroView = GameObject.FindChildByName("ComTetroView");
            eduTetroView = GameObject.FindChildByName("EduTetroView");
            pvBtnOne = GameObject.FindChildByName("ptoBtn");
            pvBtnOne.GetComponent<Button>().onClick.AddListener(playFirstTetromino); 
            pvBtnTwo = GameObject.FindChildByName("pteBtn");
            pvBtnTwo.GetComponent<Button>().onClick.AddListener(playSecondTetromino);
// 游戏主界面面板上的几个按钮            
            togBtn = GameObject.FindChildByName("togBtn");
            togBtn.GetComponent<Button>().onClick.AddListener(OnClickTogButton); // toggle moveCanvas rotateCanvas
            pauBtn = GameObject.FindChildByName("pauBtn");
            pauBtn.GetComponent<Button>().onClick.AddListener(OnClickPauButton);
            swaBtn = GameObject.FindChildByName("swaBtn");
            swaBtn.GetComponent<Button>().onClick.AddListener(onSwapPreviewTetrominos);

            falBtn = GameObject.FindChildByName("falBtn");
            falBtn.GetComponent<Button>().onClick.AddListener(OnClickFalButton);
            undoBtn = GameObject.FindChildByName("undBtn");
            undoBtn.GetComponent<Button>().onClick.AddListener(OnClickUndButton);
// pausePanel里的5个按钮
            pausePanel = GameObject.FindChildByName("MidMenuView"); // MidMenuView: 它仍然是空父节点GameView的众多子节点之一
            resBtn = GameObject.FindChildByName("resBtn").GetComponent<Button>();
            resBtn.onClick.AddListener(OnClickResButton);
            manBtn = GameObject.FindChildByName("manBtn").GetComponent<Button>();
            manBtn.onClick.AddListener(OnClickManButton);

            savBtn = GameObject.FindChildByName("savBtn").GetComponent<Button>();
            savBtn.onClick.AddListener(OnClickSavButton);
            guiBtn = GameObject.FindChildByName("guiBtn").GetComponent<Button>();
            guiBtn.onClick.AddListener(OnClickGuiButton);
            creBtn = GameObject.FindChildByName("creBtn").GetComponent<Button>();
            creBtn.onClick.AddListener(OnClickCreButton);
// Save current game or not panel
            saveGameOrNotPanel = GameObject.FindChildByName("SaveGameOrNotPanel");
            saveBtn = GameObject.FindChildByName("saveBtn").GetComponent<Button>(); // save
            // saveBtn.onClick.AddListener(OnClickSaveButton);
            saveBtn.onClick.AddListener(onYesToSaveGame);
            nosvBtn = GameObject.FindChildByName("nosvBtn").GetComponent<Button>(); // not saving
            nosvBtn.onClick.AddListener(onNoToNotSaveGame);
            cancBtn = GameObject.FindChildByName("cancBtn").GetComponent<Button>(); // cancel: back to pause Panel
            cancBtn.onClick.AddListener(OnClickCancButton);

// isChallengeMode: 这里先前的问题是,当不把这个面板及时加进去,感觉像是没有在UI Canvas绘制系统中注册,当swapCnter值变化的时候,不能刷新UI LOVE MY DEAR COUSIN !!!
            comLevelView = GameObject.FindChildByName("comLevelTexts");
            comLevelView.SetActive(false);
            goalPanel = GameObject.FindChildByName("goalPanel");
            swapCnter = GameObject.FindChildByName("swapCnter").GetComponent<Text>();
            undoCnter = GameObject.FindChildByName("undoCnter").GetComponent<Text>();
            tetroCnter = GameObject.FindChildByName("tetroCnter").GetComponent<Text>();
            gameOverPanel = GameObject.FindChildByName("gameOverPanel");
            
            cycledPreviewTetromino = new GameObject();
            delta = Vector3.zero;

// 启动并控制游戏主场景面板上几个按钮的是否可点击状态
            btnState = new Dictionary<GameObject, bool>() {
                {pvBtnOne, true}, // comTetroView button
                {pvBtnTwo, true}, // eduTetroView button
                {swaBtn, true},   // swap
                {undoBtn, true},  // undo
                {togBtn, true},   // toggle
                {falBtn, true},   // fall fast
                {pauBtn, true}    // pause game
            };
        }
        void RegisterListeners() {
            Debug.Log(TAG + " RegisterListeners");
            // if (ViewModel.gameMode.Value == 0) { // 启蒙模式下特有的几个物件
            //     EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            //     EventManager.UndoButtonClicked += onUndoGame;
            //     EventManager.SwapButtonClicked += onSwapPreviewTetrominos;
            // }
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.RegisterListener<UndoLastTetrominoInfo>(onUndoGame); 
            EventManager.Instance.RegisterListener<SaveGameEventInfo>(SaveGame); 
        }
        public void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            // if (ViewModel.gameMode.Value == 0) {
            //     EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
            //     EventManager.UndoButtonClicked -= onUndoGame;
            //     EventManager.SwapButtonClicked -= onSwapPreviewTetrominos;
            // }                
            EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove);
            EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.UnregisterListener<SaveGameEventInfo>(SaveGame); 
            EventManager.Instance.UnregisterListener<UndoLastTetrominoInfo>(onUndoGame); 
        }
#endregion

#region otherHelpers        
        // void printViewModel.buttonInteractableList() {
        //     for (int i = 0; i < 6; i++) 
        //         Debug.Log(TAG + " ViewModel.buttonInteractableList[i]: i : " + i + ", " + ViewModel.buttonInteractableList[i]); 
        // }
#endregion
    }
}