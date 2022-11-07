using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
using Framework.Util;
using HotFix.Control;
using HotFix.Data;
using tetris3d;
using UnityEngine;

namespace HotFix.UI {

    // 这里是把原混作一团的Game.cs中的应用控制逻辑折分到不同的分模块的视图模型中,尽可能地减少不必要的偶合
    public class GameViewModel : ViewModelBase {
        private const string TAG = "GameViewModel"; 

        public bool isPaused { set; get; }
        public bool saveForUndo { set; get; }
        public bool hasSavedGameAlready { set; get; }
        public bool gameStarted { set; get; }
        public float fallSpeed { set; get; }

        public int gridHeight = 12; 
        public int gridWidth;

        // public Transform [][][] grid; //= new Transform[gridWidth, gridHeight, gridWidth]; // STATIC
        // public int [][][] gridOcc; //= new int[gridWidth, gridHeight, gridWidth];          // static

        public int scoreOneLine = 40;
        public int scoreTwoLine = 100;
        public int scoreThreeLine = 300;
        public int scoreFourLine = 1200;
    
        public BindableProperty<int> currentScore = new BindableProperty<int>();
        public BindableProperty<int> currentLevel = new BindableProperty<int>();
        public BindableProperty<int> numLinesCleared = new BindableProperty<int>();
        public BindableProperty<int> gameMode = new BindableProperty<int>();
// // comTetroType, eduTetroType
        public BindableProperty<string> comTetroType = new BindableProperty<string>();
        public BindableProperty<string> eduTetroType = new BindableProperty<string>();
        public BindableProperty<string> nextTetrominoType = new BindableProperty<string>(); // 这个好像是用来给别人观察的,保存系统 ?
// GameView: nextTetromino position, rotation, localScale
        public BindableProperty<Transform> nextTetroTrans = new BindableProperty<Transform>();
        public BindableProperty<Vector3> nextTetroPos = new BindableProperty<Vector3>();
        public BindableProperty<Quaternion> nextTetroRot = new BindableProperty<Quaternion>();
        public BindableProperty<Vector3> nextTetroSca = new BindableProperty<Vector3>();
// 相机的旋转(因为目前是放在主工程下的,所以其实根本没有必要?)
        public BindableProperty<Vector3> cameraPos = new BindableProperty<Vector3>();
        public BindableProperty<Quaternion> cameraRot = new BindableProperty<Quaternion>();
// for CHALLENGING MODE ONLY: 对于关级中可用的方块砖的数目,撤销次数,以及交换次数进行必要的限制
        public BindableProperty<int> tetrominoCnter = new BindableProperty<int>();
        public BindableProperty<int> undoCnter = new BindableProperty<int>();
        public BindableProperty<int> swapCnter = new BindableProperty<int>();
        private static Coroutine deleteMinoAtCoroutine;
        
        public static bool startingAtLevelZero;
        public static int startingLevel;
    
        private int numberOfRowsThisTurn = 0;

        private int startingHighScore;
        private int startingHighScore2;
        private int startingHighScore3;
     
        // public bool isMovement = true;
        private int randomTetromino;
// TODO: INTO CONST        
        public Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f); // previewTetromino Scale (7,7,7)

        public string prevPreview; // 前预览方块砖类型,记忆方便撤销to remember previous spawned choices
        public string prevPreview2;// 
        public int prevPreviewColor;
        public int prevPreviewColor2;
        public int previewTetrominoColor;
        public int previewTetromino2Color;
        
        // private SaveGameEventInfo saveGameInfo;
        public bool hasDeletedMinos = false;
        public bool loadSavedGame = false;
        public bool isDuringUndo = false;

        public Transform tmpTransform;
        
        private GameObject tmpParentGO; // 在撤销恢复方格的时候会用来填充数据

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe(); 
        }   

        public void onUndoGame(GameData gameData) { 
            Debug.Log(TAG + ": onUndoGame()");
            isDuringUndo = true;

            ++tetrominoCnter.Value;
            --undoCnter.Value;
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            recycleThreeMajorTetromino(ViewManager.GameView.previewTetromino, ViewManager.GameView.previewTetromino2);

            StringBuilder type = new StringBuilder("");
            if (ModelMono.hasDeletedMinos) {
                currentScore.Value  = gameData.score;
                currentLevel.Value  = gameData.level;
                numLinesCleared.Value  = gameData.lines;

                Debug.Log(TAG + ": onUndoGame() current board BEFORE respawn"); 
                MathUtilP.printBoard(Model.gridOcc); 

// 这部分的逻辑独立到一个文件中去了,免得当前文件过大不好管理                 
                Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                LoadingSystemHelper.LoadDataFromParentList(gameData.parentList);
                
                cameraPos.Value = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                cameraRot.Value = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            }
            buttonInteractableList[0] = 1; 
            buttonInteractableList[1] = 1; 
            buttonInteractableList[2] = 1; 
            buttonInteractableList[3] = 0; // buttons are supposed to click once at a time only

            isDuringUndo = false;
        }
        
// 我的那些先前的歪歪斜斜的写法
        // enable disable these button3s work slight better than this, could modify this part later
        public int [] buttonInteractableList = new int[7]{ 1, 1, 1, 1, 1, 1, 1};
        // previewSelectionButton     0
        // previewSelectionButton2    1
        // swapPreviewTetrominoButton 2
        // undoButton                 3
        // toggleButton               4
        // fallButton                 5
        // pauseButton                6
        public int getSlamDownIndication () {
            return buttonInteractableList[5];
        }
        // buttons can be clicked once only each time: disable self whenever got clicked         
        // states:
        // SpawnPreviewTetromino: // undo ?
        // disables: undoButton toggleButton fallButton
        // playFirstTetromino:
        // playSecondTetromino:
        // disables: previewSelectionButton previewSelectionButton2 swapPreviewTetrominoButton
        // enables: undoButton toggleButton fallButton
        // onUndoGame:
        // disableAllButtons();
        // onActiveTetrominoLand: slam down, move down, except undoButton
        // disableAllButtons();
        // enable: undoButton
        void Initialization() {
            this.ParentViewModel = (MenuViewModel)ViewManager.MenuView.BindingContext; // 父视图模型: 菜单视图模型
            gridWidth = ((MenuViewModel)ParentViewModel).gridWidth;
            
            gameMode.Value = ((MenuViewModel)ParentViewModel).gameMode;
            fallSpeed = 0.5f;
            // saveForUndo = true;
            gameStarted = false;

            // numLinesCleared.Value = 0;
            prevPreview = "";
            prevPreview2 = "";
            prevPreviewColor = -1;
            prevPreviewColor2 = -1;
            nextTetroPos.Value = new Vector3(2.0f, 11.0f, 2.0f);
            nextTetroRot.Value = Quaternion.Euler(Vector3.zero);
            nextTetroSca.Value = Vector3.one;

            cameraPos.Value = new Vector3(11.01f, 21.297f, 0.88f);
            cameraRot.Value = Quaternion.Euler(new Vector3(483.091f, -263.118f, -538.141f));

            tetrominoCnter.Value = 0;
            undoCnter.Value = 5;
            swapCnter.Value = 5;
            
            buttonInteractableList = new int [7];
            for (int i = 0; i < 7; i++)
                buttonInteractableList[i] = 1;
        }

        public void Start() {
            Debug.Log(TAG + " Start()");
            // if (!string.IsNullOrEmpty(((MenuViewModel)ParentViewModel).saveGamePathFolderName)) {
            //     gameMode = ((MenuViewModel)ParentViewModel).gameMode;
            //     loadSavedGame = ((MenuViewModel)ParentViewModel).loadSavedGame;
            //     StringBuilder path = new StringBuilder("");
            //     if (gameMode > 0)
            //         path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName + "/game.save");
            //     else 
            //         path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName + "/grid" + gridWidth + "/game.save");
            //     if (loadSavedGame) {
            //         LoadGame(path.ToString());
            //     } else {
            //         LoadNewGame();
            //     }
            // } else {
            LoadNewGame();
            // }
            currentLevel.Value = startingLevel;
            startingHighScore = PlayerPrefs.GetInt("highscore");
            startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        }

// Coroutine: 才是真正解决问题的办法,暂时如此        
        public void OnFinishReveal() {
            Debug.Log(TAG + " OnFinishReveal");
            gameMode.Value = ((MenuViewModel)ParentViewModel).gameMode;
            Debug.Log(TAG + " gameMode.Value: " + gameMode.Value);

            fallSpeed = 0.5f; // should be recorded too, here
            if (gameMode.Value == 0) {
                Model.resetGridOccBoard();
                saveForUndo = true;
            } else saveForUndo = false;
            currentScore.Value = 0;
            currentLevel.Value = startingLevel;
        }

        public void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            gameMode.Value = ((MenuViewModel)ParentViewModel).gameMode;
            fallSpeed = 1.0f; // should be recorded too, here

            if (gameMode.Value == 0)
                Model.resetGridOccBoard();
            currentScore.Value = 0;
            currentLevel.Value = startingLevel;
        }

        void DelegateSubscribe() { } // 这里怎么写成是观察者模式呢?
        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() { } // 这是里是指初始化数据管理,而不是视图层面

        public bool gridMatchesSavedParent(GameObject tmpGO, List<MinoData> data) {
            Debug.Log(TAG + " gridMatchesSavedParent");
            if (tmpGO.transform.childCount == 4 && data.Count == 4) return true;
            if (tmpGO.transform.childCount != data.Count) return false;
// tmpGO.transform.childCount == data.children.Count
            foreach (Transform trans in tmpParentGO.transform) {
                if (!myContains(trans, data))
                    return false;
            }
            return true;
        }
        public bool myContains(Transform tmp, List<MinoData> children) {
            foreach (MinoData data in children)
                if (MathUtil.Round(tmp.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(data.transform))) 
// 因为实时运行时存在微小转动.这里暂不检查旋转角度
                    // && MathUtil.Round(tmp.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(data.transform)))
                    return true;
            return false;
        }

        void Update() {
            UpdateScore();
            UpdateLevel();
            UpdateSpeed();
        }

        public void onGameSave() {
            // Debug.Log(TAG + ": onGameSave()");
// TODO这也是那个时候写得逻辑不对称的乱代码,要归位到真正用它的地方,而不是摆放在这里            
            if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
                tmpTransform = new GameObject().transform;
// TODO: 这里有个游戏数据保存的大版块BUG需要被修复,先把其它游戏逻辑连通
            StringBuilder path = new StringBuilder("");
            Debug.Log(TAG + " onGameSave() ((MenuViewModel)ParentViewModel).saveGamePathFolderName: " + ((MenuViewModel)ParentViewModel).saveGamePathFolderName);
            if (gameMode.Value > 0) {
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName + "game.save"); 
            } else {
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName
                            + "grid" + gridWidth + "/game.save"); 
            }
// TODO:这里还有BUG，也还有先前改过一个,大概撤销之后会生成几套预览的BUG, to be fixed           
            Debug.Log(TAG + " path.ToString(): " + path.ToString());
            Debug.Log(TAG + " prevPreview: " + prevPreview);
            Debug.Log(TAG + " prevPreview2: " + prevPreview2);
            Debug.Log(TAG + " prevPreviewColor: " + prevPreviewColor);
            Debug.Log(TAG + " prevPreviewColor2: " + prevPreviewColor2);
            Debug.Log(TAG + " comTetroType.Value: " + comTetroType.Value);

            GameData gameData = new GameData(GloData.Instance.isChallengeMode, ViewManager.nextTetromino, ViewManager.ghostTetromino, tmpTransform,
                                             gameMode.Value, currentScore.Value, currentLevel.Value, numLinesCleared.Value, gridWidth,
                                             prevPreview, prevPreview2,
                                             nextTetrominoType.Value, comTetroType.Value, eduTetroType.Value,
                                             saveForUndo, Model.grid, Model.gridClr, prevPreviewColor, prevPreviewColor2, previewTetrominoColor, previewTetromino2Color); 
            SaveSystem.SaveGame(path.ToString(), gameData);
        }

        public void printbuttonInteractableList() {
            for (int i = 0; i <= 6; i++) 
                Debug.Log(TAG + " buttonInteractableList[i]: i : " + i + ", " + buttonInteractableList[i]); 
        }

        public void playFirstTetromino(GameObject previewTetromino,
                                       GameObject previewTetromino2) {
// 在生成新的一两预览前将现两个预览保存起来
            prevPreview = comTetroType.Value;
            prevPreview2 = eduTetroType.Value;
            nextTetrominoType.Value = comTetroType.Value; // 记忆功能
            PoolHelper.recyclePreviewTetrominos(previewTetromino2);
// 配置当前方块砖的相关信息
            previewTetromino.transform.localScale -= previewTetrominoScale;
            ViewManager.nextTetromino = previewTetromino;
            ViewManager.nextTetromino.gameObject.transform.position = new Vector3(2.0f, 11.0f, 2.0f);
            ViewManager.nextTetromino.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            ViewManager.nextTetromino.gameObject.transform.localScale = Vector3.one;
            
            gameStarted = true;

            // disables: comTetroView eduTetroView swaBtn
            // enables: undoButton toggleButton fallButton
            if (gameMode.Value == 0) {
                buttonInteractableList[0] = 0;
                buttonInteractableList[1] = 0;
                buttonInteractableList[2] = 0;
                buttonInteractableList[3] = 1;
                buttonInteractableList[4] = 1;
                buttonInteractableList[5] = 1;
            }
        }

        public void playSecondTetromino(GameObject previewTetromino,
                                        GameObject previewTetromino2) {
            prevPreview = comTetroType.Value;
            prevPreview2 = eduTetroType.Value;
            nextTetrominoType.Value = eduTetroType.Value; // 记忆功能
            PoolHelper.recyclePreviewTetrominos(previewTetromino);
// 配置当前方块砖的相关信息
            previewTetromino2.transform.localScale -= previewTetrominoScale;
            Debug.Log(TAG + " (nextTetroRot.Value == null): " + (nextTetroRot.Value == null));
            ViewManager.nextTetromino = previewTetromino2;
            ViewManager.nextTetromino.gameObject.transform.position = new Vector3(2.0f, 11.0f, 2.0f);
            ViewManager.nextTetromino.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            ViewManager.nextTetromino.gameObject.transform.localScale = Vector3.one;
            // previewTetromino2.layer = LayerMask.NameToLayer("Default");
            // previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;

            gameStarted = true; 
            
            // disables: previewSelectionButton previewSelectionButton2 swapPreviewTetrominoButton
            // enables: undoButton toggleButton fallButtononUn
            if (gameMode.Value  == 0) {
                buttonInteractableList[0] = 0;
                buttonInteractableList[1] = 0;
                buttonInteractableList[2] = 0;
                buttonInteractableList[3] = 1;
                buttonInteractableList[4] = 1;
                buttonInteractableList[5] = 1;
            }
        }

        public void UpdateLevel() {
            if (startingAtLevelZero || (!startingAtLevelZero && numLinesCleared.Value  / 10 > startingLevel)) 
                currentLevel.Value  = numLinesCleared.Value  / 10;
        }

        public void UpdateSpeed() { 
            fallSpeed = 1.0f - (float)currentLevel.Value  * 0.1f;
        }
        
        public void recycleNextTetromino() { // 这个折成两部分来写
            Debug.Log(TAG + ": recycleNextTetromino()"); 
            if (ViewManager.nextTetromino != null) {
                Model.resetGridAfterDisappearingNextTetromino(ViewManager.nextTetromino); // check ???
                if (ViewManager.nextTetromino.transform.childCount == 4) {
                    PoolHelper.recycleNextTetromino();
                } else
                    GameObject.Destroy(ViewManager.nextTetromino.gameObject);
            }
        }

        public void recycleThreeMajorTetromino(GameObject previewTetromino, GameObject previewTetromino2) {
// 回收三样东西： nextTetromino previewTetromino previewTetromino2
            recycleNextTetromino();
            PoolHelper.recyclePreviewTetrominos(previewTetromino);
            PoolHelper.recyclePreviewTetrominos(previewTetromino2);
        }

        public string GetRandomTetromino() { // active Tetromino 
            Debug.Log(TAG + ": GetRandomTetromino()"); 

            if (gameMode.Value == 0 && gridWidth == 3)
                randomTetromino = UnityEngine.Random.Range(1, 7);
            else 
                randomTetromino = UnityEngine.Random.Range(1, 8);
            StringBuilder tetrominoType = new StringBuilder("Tetromino");
            switch (randomTetromino) {
            case 1: tetrominoType.Append("J"); break;
            case 2: tetrominoType.Append("Z"); break; 
            case 3: tetrominoType.Append("L"); break;
            case 4: tetrominoType.Append("I"); break;
            case 5: tetrominoType.Append("O"); break;
            case 6: tetrominoType.Append("T"); break;
            default: // 7
                tetrominoType.Append("S"); break; // 需要放一个相对难一点儿的在非启蒙模式下
            }
            return tetrominoType.ToString();
        }
        
#region updatingGameScores        
        public void UpdateScore() {
            Debug.Log(TAG + " UpdateScore");
            Debug.Log(TAG + " numberOfRowsThisTurn: " + numberOfRowsThisTurn);
            if (numberOfRowsThisTurn > 0) {
                if (numberOfRowsThisTurn == 1) 
                    ClearedOneLine();
                else if (numberOfRowsThisTurn == 2) 
                    ClearedTwoLine();
                else if (numberOfRowsThisTurn == 3) 
                    ClearedThreeLine();
                else if (numberOfRowsThisTurn == 4) 
                    ClearedFourLine();
                numberOfRowsThisTurn = 0;
                // 考虑粒子系统是否像是声频管理器一样的统一管理,只在教育模式下使用到粒子系统,但其它模式可以扩展                
                //PlayLineClearedSound();
                //particles = GetComponent<ParticleSystem>();
                //emissionModule = particles.emission;
                //emissionModule.enabled = true;
                //particles.Play();
            }
        }
        public void ClearedOneLine() {
            Debug.Log(TAG + " ClearedOneLine");
            currentScore.Value += scoreOneLine + (currentLevel.Value  + 20);
            numLinesCleared.Value += 1;
            Debug.Log(TAG + " numLinesCleared.Value: " + numLinesCleared.Value);
        }
        public void ClearedTwoLine() {
            currentScore.Value += scoreTwoLine + (currentLevel.Value  + 25);
            numLinesCleared.Value += 2;
        }
        public void ClearedThreeLine() {
            currentScore.Value += scoreThreeLine + (currentLevel.Value  + 30);
            numLinesCleared.Value += 3;
        }
        public void ClearedFourLine() {
            currentScore.Value += scoreFourLine + (currentLevel.Value  + 40);
            numLinesCleared.Value += 4;
        }
        public void UpdateHighScore() {
            if (currentScore.Value  > startingHighScore) {
                PlayerPrefs.SetInt("highscore3", startingHighScore2);
                PlayerPrefs.SetInt("highscore2", startingHighScore);
                PlayerPrefs.SetInt("highscore", currentScore.Value);
            } else if (currentScore.Value  > startingHighScore2) {
                PlayerPrefs.SetInt("highscore3", startingHighScore2);
                PlayerPrefs.SetInt("highscore2", currentScore.Value);
            } else if (currentScore.Value  > startingHighScore3) 
                PlayerPrefs.SetInt("highscore3", currentScore.Value);
        }
#endregion
    }
}




