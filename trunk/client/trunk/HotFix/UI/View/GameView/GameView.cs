using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
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
// ComTetroView 按钮控件
        GameObject pvBtnOne; // ptoBtn 
// EduTetroView 按钮控件
        GameObject pvBtnTwo; // pteBtn
// ToggleBtnView
        GameObject togBtn; // Togcation
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
        
// 私有的当前视图的元件? 暂时添加上AudioSouce元件顺承原混作一团的逻辑(感觉它不该是被AudioManager管才对吗?)
        public Canvas hud_canvas;
        public Button invertButton;
        public Sprite newImg;
        public Sprite prevImg;

        public GameObject defaultContainer;
        private GameObject cycledPreviewTetromino;
        private bool gameStarted = false;
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

        public Dictionary<GameObject, bool> btnState;
        
        private static GameObject tmpTetro; // TODO: why static ?

        private bool isMoveValid = false;
        private bool isRotateValid = false;
        private Vector3 delta;
        
        public void SpawnnextTetromino() {
            Debug.Log(TAG + ": SpawnnextTetromino()");
            if (!gameStarted) {
                // Debug.Log(TAG + " (ViewModel.gameMode != null): " + (ViewModel.gameMode != null));
                // if (ViewModel.gameMode != null && ViewModel.gameMode.Value == 0) {
                if (ViewModel.gameMode.Value == 0) {
                    SpawnPreviewTetromino();
                } else {
                   gameStarted = true;
                   ViewManager.nextTetromino = PoolHelper.GetFromPool(
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
               ViewManager.nextTetromino = previewTetromino;
               currentActiveTetrominoPrepare();
                
               SpawnGhostTetromino();  
               moveRotatecanvasPrepare();
               SpawnPreviewTetromino();
            }
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
            previewTetromino = PoolHelper.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(ViewManager.tetroParent.transform, false);
            ViewModel.comTetroType.Value = previewTetromino.GetComponent<TetrominoType>().type;
            if (ViewModel.gameMode.Value == 0) { // previewTetromino2
                previewTetromino2 = PoolHelper.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, ViewModel.previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(ViewManager.tetroParent.transform, false);
            }
            ViewModel.buttonInteractableList[3] = 1; // undoBtn
        }

        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            ViewModel.LoadNewGame();

// 感觉这里好鬼魅呀:视图里已经进行这时在了,可是视图模型因为反射调用还没有启动完?!!!
// 这里的问题:应该是用IEnumerator去解决,但暂时这样; 在自己不断修改整理这些重构的代码的过程中,会对这些忘记了的相对陌生的一点点儿捡回来的
// 现在这里要做的就是:打预设的资源包(很简单),从资源包里将不是的预设读出来,存在字典里,供游戏需要的时候来用            
            SpawnnextTetromino();   

            // ViewManager.moveCanvas.SetActive(true);
            // ViewManager.rotateCanvas.SetActive(false);
        }

        public void Start() { // 感觉这些逻辑放在视图里出很牵强,哪些是可以放在模型里的呢?
            Debug.Log(TAG + ": Start()");
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
                    LoadNewGame();
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
            Debug.Log(TAG + ": LoadGame()");
            // ViewModel.InitializationForNewGame(); // com for tmp
            // if (ViewModel.gameMode == 0)
            //     ViewModel.resetGridOccBoard(); 
            GameData gameData = deepwaterooo.tetris3d.SaveSystem.LoadGame(path);
            // ViewModel.gameMode = gameData.gameMode;
            // ViewModel.currentScore.Value  = gameData.score;
            // ViewModel.currentLevel.Value  = gameData.level;
            // numLinesCleared = gameData.lines;
            // scoText.text = ViewModel.currentScore.ToString();
            // lvlText.text = ViewModel.currentLevel.ToString();
            // linText.text = numLinesCleared.ToString();
            
            // hud_canvas.enabled = true; // 这个是需要根据不同的mode 来进行处理的 ?????
// 这部分被观察者模式取代了            
            // if (ViewModel.gameMode > 0) { // disable some components
            //     comTetroView.SetActive(false);
            //     eduTetroView.SetActive(false);
            //     swaBtn.SetActive(false);
            //     undoBtn.SetActive(false);
            // }
            // Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            // ViewModel.LoadDataFromParentList(gameData.parentList);
            // currentActiveTetromino: if it has NOT landed yet
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                ViewManager.nextTetromino = PoolHelper.GetFromPool(
                    type.Append(gameData.nextTetrominoData.type).ToString(),
                    // gameData.nextTetrominoData.transform,
                    // gameData.nextTetrominoData.transform);
                    DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                    DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform), Vector3.one);
                ViewManager.nextTetromino.tag = "currentActiveTetromino";
                // if (defaultContainer == null) // 我不要再管这个东西了
                //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // nextTetromino.transform.SetParent(defaultContainer.transform, false);
                // ViewManager.nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled;
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

        private void currentActiveTetrominoPrepare() {
            // Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            ViewManager.nextTetromino.tag = "currentActiveTetromino";
            ViewManager.nextTetromino.transform.rotation = Quaternion.identity;
            ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = true;
            if (ViewModel.gameMode.Value == 0 && (ViewModel.gridWidth == 3 || ViewModel.gridWidth == 4)) {
                ViewManager.nextTetromino.transform.localPosition = new Vector3(1.0f, ViewModel.gridHeight - 1f, 1.0f);
            } else 
                ViewManager.nextTetromino.transform.localPosition = new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f);
        }
        
        private void moveRotatecanvasPrepare() { 
            ViewManager.moveCanvas.transform.localPosition = new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f);     
            ViewManager.rotateCanvas.transform.localPosition = new Vector3(2.0f, ViewModel.gridHeight - 1f, 2.0f);
            ViewManager.moveCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);   // 规正可能存在的微小转动
            ViewManager.rotateCanvas.transform.rotation = Quaternion.Euler(0, 0, 0); // 规正可能存在的微小转动
            ViewManager.moveCanvas.SetActive(true);
        }

        public void SpawnGhostTetromino() {
            GameObject tmpTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            ViewManager.ghostTetromino = PoolHelper.GetFromPool(GameObjectHelper.GetGhostTetrominoType(ViewManager.nextTetromino),
                                                                ViewManager.nextTetromino.transform.position,
                                                                ViewManager.nextTetromino.transform.rotation, Vector3.one);
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
        }
        public void GameOver() {
            Debug.Log(TAG + ": GameOver()"); 
            ViewModel.UpdateHighScore();
            // SceneManager.LoadScene("GameOver");
        }
#region pausePanel Button Handlers
        // MidMenuView 里的5 个按钮, 以及的瑨延伸的3个按钮的点击回调
        void OnClickSavButton() { // SAVE GAME

        }
        void SaveGame(SaveGameEventInfo info) {
            Debug.Log(TAG + ": SaveGame()");
            ViewModel.saveForUndo = false;
            onGameSave();
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
                ViewModel.cleanUpGameBroad(ViewManager.nextTetromino, ViewManager.ghostTetromino);
                ViewModel.isPaused = false;
                Time.timeScale = 1.0f;
                ViewManager.MenuView.Reveal(); // 现在的简单逻辑
                Hide();
            }
        }
        // void OnClickSaveButton() {
        //     saveGameOrNotPanel.SetActive(false);
        // }
        public void onYesToSaveGame() {
            ViewModel.saveForUndo = false; // ? 这里是什么意思呢
            onGameSave();
            ViewModel.hasSavedGameAlready = true;
            saveGameOrNotPanel.SetActive(false);
            pausePanel.SetActive(false);
            ViewModel.cleanUpGameBroad(ViewManager.nextTetromino, ViewManager.ghostTetromino);
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
            ViewModel.cleanUpGameBroad(ViewManager.nextTetromino, ViewManager.ghostTetromino);
            
            ViewModel.isPaused = false;
            Time.timeScale = 1.0f;
            if (ViewModel.gameMode.Value == 1)
                gameStarted = false;
            // still have to check this due to auto Save
            string path = new StringBuilder(Application.persistentDataPath).Append("/").Append(((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName).Append("/game.save").ToString();
            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }
            }
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

#region GameViewCallbacks
// 游戏主面板中7个按钮的点击回调        
        void OnClickUndButton() { // onUndoGame() 撤销最后一块降落的方块砖(在最后一块降落后有消除的情况下比较复杂一点儿,大量数据的恢复)
            Debug.Log(TAG + ": onUndoGame()");
            if (ViewModel.buttonInteractableList[3] == 0) return;
            // Array.Clear(ViewModel.buttonInteractableList, 0, ViewModel.buttonInteractableList.Length);
            isDuringUndo = true;
            // ViewModel.recycleThreeMajorTetromino(ViewManager.nextTetromino, previewTetromino, previewTetromino2);

            StringBuilder path = new StringBuilder("");
            if (ViewModel.gameMode.Value > 0)
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName + "/game.save");
            else
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ViewModel.ParentViewModel).saveGamePathFolderName
                            + "grid" + ViewModel.gridWidth + "/game.save");
// TODO : 这里的问题是:先前每块方块砖落下的时候都会自动保存,但是我现在的逻辑还没有保存            
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            ViewModel.onUndoGame(gameData);
            
//             if (ViewModel.hasDeletedMinos) {
//                 ViewModel.currentScore.Value  = gameData.score; // 这里要改
//                 ViewModel.currentLevel.Value  = gameData.level;
//                 numLinesCleared = gameData.lines;
//                 // scoText.text = ViewModel.currentScore.ToString();
//                 // lvlText.text = ViewModel.currentLevel.ToString(); // 这不希望变的
//                 // linText.text = numLinesCleared.ToString();
//                 Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
//                 ViewModel.LoadDataFromParentList(gameData.parentList);
// // 不想机载地来写这些东西                 
// // // 相机的位置与旋转:因为是允许用户随意转动的;(可是不是位置没有变吗?为什么要保存位置呢?)
// //                 GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
// //                 GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
//             }
// 这里新添一个根据两种方块砖类型,重新生成两块预览的方块砖
            StringBuilder type = new StringBuilder("");
            if (ViewModel.prevPreview != null) { // previewTetromino previewTetromino2
                type.Length = 0;
                string type2 = ViewModel.prevPreview2;
                SpawnPreviewTetromino(type.Append(ViewModel.prevPreview).ToString(), type2);
            }
            // ViewModel.buttonInteractableList[0] = 1; 
            // ViewModel.buttonInteractableList[1] = 1; 
            // ViewModel.buttonInteractableList[2] = 1; 
            // ViewModel.buttonInteractableList[3] = 0; // buttons are supposed to click once at a time only
            isDuringUndo = false;
        }

        void OnClickPauButton() { 
            Debug.Log(TAG + " OnClickPauButton");
            Time.timeScale = 0f;
            EventManager.Instance.FireEvent("pausegame");
            // ViewModel.PauseGame(); // 游戏暂停
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
            while (ViewModel.CheckIsValidPosition()) { 
                ViewManager.nextTetromino.transform.position -= delta;
                EventManager.Instance.FireEvent("move", delta);
                ViewManager.nextTetromino.transform.position += delta;
            } // 出循环就是不合理位置
            if (!ViewModel.CheckIsValidPosition()) { // 方块砖移到了最底部
                ViewManager.nextTetromino.transform.position -= delta;
                EventManager.Instance.FireEvent("land");
            }
        }
        public void onSwapPreviewTetrominos () { // 这里需要下发指令到视图数据层,并根据随机数生成的新的tetromino来重新刷新UI
            Debug.Log(TAG + " onSwapPreviewTetrominos");
            // EventManager.Instance.FireEvent("swap");
            if (ViewModel.buttonInteractableList[2] == 0) return;
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino); // recycle 1st tetromino first
            PoolHelper.ReturnToPool(previewTetromino, previewTetromino.GetComponent<TetrominoType>().type);
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino2); // recycle 2st tetromino then
            PoolHelper.ReturnToPool(previewTetromino2, previewTetromino2.GetComponent<TetrominoType>().type);
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
            // Debug.Log(TAG + " ViewModel.buttonInteractableList[0]: " + ViewModel.buttonInteractableList[0]); 
            if (ViewModel.buttonInteractableList[0] == 0) return;

            ViewModel.playFirstTetromino(previewTetromino, previewTetromino2, cycledPreviewTetromino);

            currentActiveTetrominoPrepare();
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
        }
        public void playSecondTetromino() { // pvBtnTwo: eduTetroView Button
            // Debug.Log(TAG + " ViewModel.buttonInteractableList[1]: " + ViewModel.buttonInteractableList[1]); 
            if (ViewModel.buttonInteractableList[1] == 0) return;
            ViewModel.playSecondTetromino(previewTetromino, previewTetromino2, cycledPreviewTetromino);

            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
        }
#endregion

// TODO: 这个系统是,4+6个按钮触发事件(会有假阳性,会误播背景音乐),先发送所有事件,再接收后才判断是否合理,只有背景音乐受影响
#region eventsCallbacks
// 因为ViewManager.nextTetromino是静态的,将相关逻辑移到这个类里来管理, 不可以这样      
        void onActiveTetrominoMove(TetrominoMoveEventInfo info) { // MoveDown:所有事件都是有效的
            Debug.Log(TAG + " onActiveTetrominoMove");
            ViewManager.nextTetromino.transform.position += info.delta;
            isMoveValid = ViewModel.CheckIsValidPosition();
            if (isMoveValid)
                EventManager.Instance.FireEvent("validMR", "move");
            else 
                ViewManager.nextTetromino.transform.position -= info.delta;
        }
        void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            // Debug.Log(TAG + ": onActiveTetrominoRotate()");
            ViewManager.nextTetromino.transform.Rotate(info.delta);
            if (ViewModel.CheckIsValidPosition()) {
                isRotateValid = true;
                EventManager.Instance.FireEvent("validMR", "rotate");
            } else {
                ViewManager.nextTetromino.transform.Rotate(Vector3.zero - info.delta);
            }
        }
        public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            ViewModel.onActiveTetrominoLand(info);
// ghostTetromino, nextTetromino 的相关处理
            PoolHelper.recycleGhostTetromino(); // 放这里的主要原因是需要传参数
            ViewManager.nextTetromino.tag = "Untagged";
            Tetromino tetromino = ComponentHelper.GetTetroComponent(ViewManager.nextTetromino);
            ViewModel.currentScore.Value += tetromino.GetComponent<TetrominoType>().score;
            tetromino.enabled = false;
            // ViewManager.GameView.ViewModel.currentScore.Value += tetromino.individualScore;            
// 保存游戏进展 
            // // SaveGameEventInfo fire here 
            // saveGameInfo = new SaveGameEventInfo();
            // EventManager.Instance.FireEvent(saveGameInfo);
            // change an approach: it is unnessary and do NOT apply delegates and events here
            // onGameSave();
            if (((MenuViewModel)ViewModel.ParentViewModel).gameMode != 0) 
                SpawnnextTetromino();  
        }
        // 游戏进程的暂停与恢复: 这么改要改狠久狠久才能够改得完,还是先实现一些基础功能吧,到时对游戏逻辑再熟悉一点儿统一重构效率能高很多的呀
        void onGameSave() { // 是指将游戏进度存到本地数据文件
            ViewModel.onGameSave();
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
            //     // EventManager.Instance.RegisterListener<SwapPreviewsEventInfo>(onSwapPreviewTetrominos); // 也没有音效什么的,白传一遍,暂且如此,将来配点音乐
            //     EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            //     EventManager.UndoButtonClicked += onUndoGame;
            //     EventManager.SwapButtonClicked += onSwapPreviewTetrominos;
            // }
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.RegisterListener<SaveGameEventInfo>(SaveGame); 
        }
        public void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            // if (ViewModel.gameMode.Value == 0) {
            //     // EventManager.Instance.UnregisterListener<SwapPreviewsEventInfo>(onSwapPreviewTetrominos);
            //     EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
            //     EventManager.UndoButtonClicked -= onUndoGame;
            //     EventManager.SwapButtonClicked -= onSwapPreviewTetrominos;
            // }                
            EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove);
            EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.UnregisterListener<SaveGameEventInfo>(SaveGame); 
        }
#endregion

// TODO: 可能更好的办法是去主工程基类中去全局控制,现先这样        
        private bool revealed = false;
#region BindableProperties
        public void OnRevealed() { // 写在这里是因为热更新程序域里回调慢,写早了有时候拿不到返回空
            base.OnRevealed();
            if (revealed) return ;
            ViewModel.currentScore.OnValueChanged += onCurrentScoreChanged;
            ViewModel.currentLevel.OnValueChanged += onCurrentLevelChanged;
            ViewModel.numLinesCleared.OnValueChanged += onNumLinesCleared;
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
// 游戏主场景: 得分等游戏进展框,自动观察视图模型的数据自动,刷新
        void onGameModeChanged(int pre, int cur) {
            // Debug.Log(TAG + " onGameModeChanged");
            if (cur > 0) {
                eduTetroView.SetActive(false); 
                swaBtn.SetActive(false);
                undoBtn.SetActive(false); // 不可撤销(挑战模式是仍可以再考虑)
            }
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
        
#region otherHelpers        
        // void printViewModel.buttonInteractableList() {
        //     for (int i = 0; i < 6; i++) 
        //         Debug.Log(TAG + " ViewModel.buttonInteractableList[i]: i : " + i + ", " + ViewModel.buttonInteractableList[i]); 
        // }
#endregion
// // TODO:不仅仅是当前方块砖的位置/旋转/缩放,还该包括阴影的相关观察
// // 这套系统不工作,太多的适配相关的问题,先把简单的系统逻辑连通
//         // bug to be fixed: 这里的位置是变化的,但是这样的位置变化无法改变沉浸过程,所以热更新里面最好还是用热更新原生的方法
//         void onNextTetroTransChanged(Transform pre, Transform cur) {
//             Debug.Log(TAG + " onNextTetroTransChanged");
//             Debug.Log(TAG + " cur.position: " + cur.position);
//             Utils.print(cur.position);
//             if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.activeSelf) {// 必须当前有正在运行的方块砖
//                 // Helpers.resetTrans(ViewManager.nextTetromino, cur);
//                 ViewManager.nextTetromino.transform.position = cur.position;
//                 Debug.Log(TAG + " ViewManager.nextTetromino.gameObject.transform.position: " + ViewManager.nextTetromino.gameObject.transform.position);
//             }
//         }
//         void onNextTetroPosChanged(Vector3 pre, Vector3 cur) {
//             Debug.Log(TAG + " onNextTetroPosChanged");
//             Utils.print(cur);
//             if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.activeSelf) {// 必须当前有正在运行的方块砖
//                 Utils.print(ViewManager.nextTetromino.gameObject.transform.position);
//                 ViewManager.nextTetromino.transform.position = cur;
//                 Utils.print(ViewManager.nextTetromino.gameObject.transform.position);
//             }
//         }
//         void onNextTetroRotChanged(Quaternion pre, Quaternion cur) {
//             if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.activeSelf) // 必须当前有正在运行的方块砖
//                 ViewManager.nextTetromino.transform.rotation = cur;
//         }
//         void onNextTetroScaChanged(Vector3 pre, Vector3 cur) {
//             if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.activeSelf) // 必须当前有正在运行的方块砖
//                ViewManager.nextTetromino.transform.localScale = cur;
//         }
    }
}
