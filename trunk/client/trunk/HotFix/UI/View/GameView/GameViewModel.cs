using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
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

        public Transform [][][] grid; //= new Transform[gridWidth, gridHeight, gridWidth]; // STATIC
        public int [][][] gridOcc; //= new int[gridWidth, gridHeight, gridWidth];          // static

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
// 相机的旋转
        public BindableProperty<Vector3> cameraPos = new BindableProperty<Vector3>();
        public BindableProperty<Quaternion> cameraRot = new BindableProperty<Quaternion>();
        
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

        // private SaveGameEventInfo saveGameInfo;
        public bool hasDeletedMinos = false;
        public bool loadSavedGame = false;
        public bool isDuringUndo = false;

        public Transform tmpTransform;
        
        private GameObject tmpParentGO; // 在撤销恢复方格的时候会用来填充数据

        public bool isGameModel = true;
        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe(); 
        }   

        public void onUndoGame(GameData gameData) { 
            Debug.Log(TAG + ": onUndoGame()");
            // if (buttonInteractableList[3] == 0) return;
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            isDuringUndo = true;
            recycleThreeMajorTetromino(ViewManager.nextTetromino, ViewManager.GameView.previewTetromino, ViewManager.GameView.previewTetromino2);

            StringBuilder type = new StringBuilder("");
            if (hasDeletedMinos) {
                currentScore.Value  = gameData.score;
                currentLevel.Value  = gameData.level;
                numLinesCleared.Value  = gameData.lines;
                
                Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                LoadDataFromParentList(gameData.parentList);

                cameraPos.Value = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                cameraRot.Value = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
                // GameObject.FindGameObjectWithTag("MainCamera").transform.position = 
                // GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = 
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
            
// 这里好像是需要解决一下多维数组在ILRuntime热更新程序域中的适配问题???
            // grid = new Transform[5, gridHeight, 5]; // BUGGY BUGGY BUGGY multidimensional array.....
            grid = new Transform [5][][]; // BUGGY BUGGY BUGGY multidimensional array.....
            for (int i = 0; i < 5; i++) {
                grid[i] = new Transform[gridHeight][];
                for (int j = 0; j < gridHeight; j++) 
                    grid[i][j] = new Transform[5];
            }
                
            // gridOcc = new int[5, gridHeight, 5];
            gridOcc = new int[5][][];
            for (int i = 0; i < 5; i++) {
                gridOcc[i] = new int[gridHeight][];
                for (int j = 0; j < gridHeight; j++) 
                    gridOcc[i][j] = new int[5];
            }
            gameMode.Value = ((MenuViewModel)ParentViewModel).gameMode;
            fallSpeed = 0.5f;
            // saveForUndo = true;
            gameStarted = false;

            // numLinesCleared.Value = 0;
            prevPreview = "";
            prevPreview2 = "";
            nextTetroPos.Value = new Vector3(2.0f, 11.0f, 2.0f);
            nextTetroRot.Value = Quaternion.Euler(Vector3.zero);
            nextTetroSca.Value = Vector3.one;

            cameraPos.Value = new Vector3(11.01f, 21.297f, 0.88f);
            cameraRot.Value = Quaternion.Euler(new Vector3(483.091f, -263.118f, -538.141f));
            
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
                resetGridOccBoard();
                saveForUndo = true;
            } else saveForUndo = false;
            currentScore.Value = 0;
            currentLevel.Value = startingLevel;
        }

        public void LoadGame(string path) {  // when load Scene load game: according to gameMode
            Debug.Log(TAG + ": LoadGame()");
            if (gameMode.Value  == 0)
                resetGridOccBoard(); 
            GameData gameData = SaveSystem.LoadGame(path);
            gameMode.Value  = gameData.gameMode;
            
            currentScore.Value  = gameData.score;
            currentLevel.Value  = gameData.level;
            numLinesCleared.Value  = gameData.lines;
            
            Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            LoadDataFromParentList(gameData.parentList);
// 下面这部分被自己跳过去了,暂时没再读
            // currentActiveTetromino: if it has NOT landed yet
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                ViewManager.nextTetromino = PoolHelper.GetFromPool(
                    type.Append(gameData.nextTetrominoData.type).ToString(),
                    // gameData.ViewManager.nextTetrominoData.transform,
                    // gameData.ViewManager.nextTetrominoData.transform);
                    DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                    DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform), Vector3.one);
                ViewManager.nextTetromino.tag = "currentActiveTetromino";
                // if (defaultContainer == null) // 我不要再管这个东西了
                //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // ViewManager.nextTetromino.transform.SetParent(defaultContainer.transform, false);
                ViewManager.nextTetromino.GetComponent<Tetromino>().enabled = !ViewManager.nextTetromino.GetComponent<Tetromino>().enabled; 
                nextTetrominoType.Value = ViewManager.nextTetromino.GetComponent<TetrominoType>().type;

                ViewManager.moveCanvas.gameObject.SetActive(true);
                ViewManager.moveCanvas.transform.position = new Vector3(ViewManager.moveCanvas.transform.position.x, ViewManager.nextTetromino.transform.position.y, ViewManager.moveCanvas.transform.position.z);
                // 也需要重新设置 ViewManager.rotateCanvas 的位置
                //SpawnGhostTetromino();
            }

            // previewTetromino previewTetromino2
            type.Length = 0;
			string type2 = eduTetroType.Value;
            //SpawnPreviewTetromino(type.Append(previewTetrominoType).ToString(), type2);
// 这里再检查一下,感觉道理不通            
            if (prevPreview != null) {
                prevPreview = prevPreview;
                prevPreview2 = prevPreview2;
            } 
            // MainCamera rotation
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData);
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            
            if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.CompareTag("currentActiveTetromino")) // Performance Bug: CompareTag()
                gameStarted = true;
            loadSavedGame = false;
            loadSavedGame = false;
        }    

       public void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            gameMode.Value = ((MenuViewModel)ParentViewModel).gameMode;
            fallSpeed = 1.0f; // should be recorded too, here

            if (gameMode.Value == 0)
                resetGridOccBoard();
            currentScore.Value = 0;
            currentLevel.Value = startingLevel;
        }
        
        void DelegateSubscribe() { // 这里怎么写成是观察者模式呢?
        }
        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() { // 这是里是指初始化数据管理,而不是视图层面

        }

        public void LoadDataFromParentList(List<TetrominoData> parentList) {
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0;
            StringBuilder type = new StringBuilder("");
            foreach (TetrominoData parentData in parentList) {
                // Debug.Log(TAG + " parentData.name: " + parentData.name);
                // Debug.Log(TAG + " parentData.children.Count: " + parentData.children.Count);
                // bool isThereAnyExistChil = isThereAnyExistChild(parentData);
                // Debug.Log(TAG + " isThereAnyExistChil: " + isThereAnyExistChil);
                if (isThereAnyExistChild(parentData)) { // 存在
                // if (isThereAnyExistChil) { // 如果某块方块砖只是消除了几个格,并且还有剩余,那么只是填补消除掉的小立方体
                    // bool gridMatchesSavedParentBool = gridMatchesSavedParent(tmpParentGO, (List<MinoData>)(parentData.children.collection));
                    // Debug.Log(TAG + " gridMatchesSavedParentBool: " + gridMatchesSavedParentBool);
                    if (!gridMatchesSavedParent(tmpParentGO, (List<MinoData>)(parentData.children.collection))) {  // 先删除多余的，再补全缺失的
                    // if (!gridMatchesSavedParentBool) {
                        foreach (Transform trans in tmpParentGO.transform) { // 先 删除多余的: 很多情况下应该是一个也不多才对(除外的比如,从上一层顶层掉下来的?所以有这种可能)
                            // MathUtil.print(MathUtil.Round(trans.position));
                            // Debug.Log(TAG + " (!myContains(trans, (List<MinoData>)(parentData.children.collection))): " + (!myContains(trans, (List<MinoData>)(parentData.children.collection)))); 
                            if (!myContains(trans, (List<MinoData>)(parentData.children.collection))) {
                                x = (int)Mathf.Round(trans.position.x);
                                y = (int)Mathf.Round(trans.position.y);
                                z = (int)Mathf.Round(trans.position.z);
                                // MathUtil.print(x, y, z); // this one is right
                                grid[x][y][z].parent = null;
                                GameObject.Destroy(grid[x][y][z].gameObject);
                                gridOcc[x][y][z] = 0;
                                grid[x][y][z] = null;
                            }
                        }
                        // Debug.Log(TAG + " tmpParentGO.transform.childCount (deleted unwanted): " + tmpParentGO.transform.childCount);
                        foreach (MinoData minoData in parentData.children) { // 再补全仍然缺失的
                            Vector3 posA = MathUtil.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
                            MathUtil.print(posA);
                            x = (int)Mathf.Round(posA.x);
                            y = (int)Mathf.Round(posA.y);
                            z = (int)Mathf.Round(posA.z);
                            if (grid[x][y][z] == null) {
                                MathUtil.print(x, y, z);
                                type.Length = 0;
                                GameObject tmpMinoGO = PoolHelper.GetFromPool(type.Append(minoData.type).ToString(),
                                                                              DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                              DeserializedTransform.getDeserializedTransRot(minoData.transform),
                                    Vector3.one);
                                tmpMinoGO.tag = "mino"; // bug fixed: 这里需要TAG,不标记TAG,不被父控件认定为子控件(限制在TetrominoData里)
                                grid[x][y][z] = tmpMinoGO.transform;
                                grid[x][y][z].parent = tmpParentGO.transform;
                                gridOcc[x][y][z] = 1;
                            }
                        }
                    }
                    Debug.Log(TAG + " tmpParentGO.transform.childCount (filled needed -- final): " + tmpParentGO.transform.childCount);
// TODO:下面这个分支的逻辑没有检查,                    
                } else { // 重新生成                // 空 shapeX Tetromino_X : Universal
                GameObject tmpGameObject = PoolHelper.GetFromPool("TetrominoX", DeserializedTransform.getDeserializedTransPos(parentData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(parentData.transform),
                                                                      Vector3.one);
                    foreach (MinoData minoData in parentData.children) {
                        GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type, DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(minoData.transform),
                                                                      Vector3.one);
                        tmpMinoGO.transform.parent = tmpGameObject.transform;
                        x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
                        y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
                        z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
                        tmpMinoGO.tag = "mino";
                        if (isColumnFromHereEmpty(x, y, z)) {
                            grid[x][y][z] = tmpMinoGO.transform;
                            gridOcc[x][y][z] = 1;
                        } else {
                            FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform); // update grid accordingly
                        }
                    }
// TODO: 这里仍然涉及MINO TAG, TETROMINO SCORE等的记忆, TetrominoData里也要作相应的修改                    
                    tmpGameObject.GetComponent<TetrominoType>().type = parentData.type; // 类型 
                    // tmpGameObject.GetComponent<TetrominoType>().score = PoolHelper.scoreDic[parentData.type]; // 得分 TODO
                    tmpGameObject.name = parentData.name;
                    // Debug.Log(TAG + " tmpGameObject.GetComponent<TetrominoType>().type: " + tmpGameObject.GetComponent<TetrominoType>().type); 
                    // Debug.Log(TAG + " tmpGameObject.transform.childCount: " + tmpGameObject.transform.childCount);
                    ComponentHelper.AddTetroComponent(tmpGameObject);
                }
                // Debug.Log(TAG + ": gridOcc[,,] after each deleted mino re-spawn"); 
                MathUtil.printBoard(gridOcc); 
            }
        }
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
        public bool isThereAnyExistChild(TetrominoData parentData) {
            Debug.Log(TAG + ": isThereAnyExistChild()"); 
            Vector3 pos = Vector3.zero;
            int x = 0, y = 0, z = 0;
            foreach (MinoData mino in parentData.children) {
                pos = DeserializedTransform.getDeserializedTransPos(mino.transform);
                x = (int)Mathf.Round(pos.x);
                y = (int)Mathf.Round(pos.y);
                z = (int)Mathf.Round(pos.z);
                MathUtil.print(x, y, z);
                if (grid[x][y][z] != null && grid[x][y][z].parent != null) { // // make sure parent matches first !
                    if (grid[x][y][z].parent.gameObject.name == parentData.name &&
                        MathUtil.Round(grid[x][y][z].parent.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(parentData.transform)) && 
                        MathUtil.Round(grid[x][y][z].parent.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(parentData.transform))) {
                        tmpParentGO = grid[x][y][z].parent.gameObject;  // 这里有保存的数据
                        Debug.Log(TAG + " tmpParentGO.gameObject.name: " + tmpParentGO.gameObject.name);
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateGrid(GameObject tetromino) { // update gridOcc at the same time
            Debug.Log(TAG + ": UpdateGrid()");
            for (int y = 0; y < gridHeight; y++) 
                for (int z = 0; z < gridWidth; z++) 
                    for (int x = 0; x < gridWidth; x++)
                        if (grid[x][y][z] != null && grid[x][y][z].parent == tetromino.transform) {
                            grid[x][y][z] = null; 
                            gridOcc[x][y][z]= 0; 
                        }
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (pos.y >= 0 && pos.y < gridHeight && pos.x >= 0 && pos.x < gridWidth && pos.z >= 0 && pos.z < gridWidth) { 
                    grid[(int)pos.x][(int)pos.y][(int)pos.z] = mino;
                    gridOcc[(int)pos.x][(int)pos.y][(int)pos.z] = 1;
                }
            }
            Debug.Log(TAG + " tetromino.name: " + tetromino.name);
        }

        public void resetGridAfterDisappearingNextTetromino(GameObject tetromino) {
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if ((int)pos.y >= 0 && (int)pos.y < gridHeight && (int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.z >= 0 && (int)pos.z < gridWidth) { 
                    grid[(int)pos.x][(int)pos.y][(int)pos.z] = null;
                    gridOcc[(int)pos.x][(int)pos.y][(int)pos.z] = 0;
                }
            }
        }

        public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            UpdateGrid(ViewManager.nextTetromino);
            // // SaveGameEventInfo fire here 
            // saveGameInfo = new SaveGameEventInfo();
            // EventManager.Instance.FireEvent(saveGameInfo);
            // change an approach: it is unnessary and do NOT apply delegates and events here
            if (gameMode.Value == 0) // 只在启蒙模式下才保存
                onGameSave();

            DeleteRow();
            Update();
            if (CheckIsAboveGrid(ViewManager.nextTetromino.GetComponent<Tetromino>())) { // 检查游戏是否结束,最后一个方块砖是否放到顶了
                Debug.Log(TAG + " TODO: Game Over");
                // GameOver(); // for tmp
            }            
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            if (gameMode.Value  == 0) {
                buttonInteractableList[0] = 1;
                buttonInteractableList[1] = 1;
                buttonInteractableList[2] = 1;
                buttonInteractableList[3] = 1; // undo button
            }
        }
        
        void Update() {
            UpdateScore();
            UpdateLevel();
            UpdateSpeed();
        }

        public void onGameSave() {
            Debug.Log(TAG + ": onGameSave()");
// TODO这也是那个时候写得逻辑不对称的乱代码,要归位到真正用它的地方,而不是摆放在这里            
            if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
                tmpTransform = new GameObject().transform;
// TODO: 这里有个游戏数据保存的大版块BUG需要被修复,先把其它游戏逻辑连通
            StringBuilder path = new StringBuilder("");
            if (gameMode.Value > 0) {
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName + "/game.save"); 
            } else {
                path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName
                            + "grid" + gridWidth + "/game.save"); 
            }
            GameData gameData = new GameData(ViewManager.nextTetromino, ViewManager.ghostTetromino, tmpTransform,
                                             gameMode.Value, currentScore.Value, currentLevel.Value, numLinesCleared.Value, gridWidth,
                                             prevPreview, prevPreview2,
                                             nextTetrominoType.Value, comTetroType.Value, eduTetroType.Value,
                                             saveForUndo, grid); 
            SaveSystem.SaveGame(path.ToString(), gameData);
        }

        public void cleanUpGameBroad(GameObject nextTetromino, GameObject ghostTetromino) {
            Debug.Log(TAG + ": cleanUpGameBroad()");
            // dealing with currentActiveTetromino & ghostTetromino firrst
            if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.CompareTag("currentActiveTetromino")) { // hang in the air
                Debug.Log(TAG + " (ghostTetromino != null): " + (ghostTetromino != null));  // always true
                if (ViewManager.ghostTetromino != null) {
                    PoolHelper.recycleGhostTetromino();
                }
                recycleNextTetromino(ViewManager.nextTetromino);
            }
            prevPreview = "";
            prevPreview2 = "";
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridHeight; j++) {
                    for (int k = 0; k < gridWidth; k++) {
                        if (grid[i][j][k] != null) {
                            if (grid[i][j][k].parent != null && grid[i][j][k].parent.childCount == 4) {
                                if (grid[i][j][k].parent.gameObject.CompareTag("currentActiveTetromino")) 
                                    grid[i][j][k].parent.gameObject.GetComponent<Tetromino>().enabled = false;
                                Transform tmpParentTransform = grid[i][j][k].parent;
                                foreach (Transform transform in grid[i][j][k].parent) {
                                    x = (int)Mathf.Round(transform.position.x);
                                    y = (int)Mathf.Round(transform.position.y);
                                    z = (int)Mathf.Round(transform.position.z);
                                    if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth && z >= 0 && z < gridWidth) {
                                        grid[x][y][z] = null;
                                        gridOcc[x][y][z] = 0;
                                    }
                                }
                                PoolHelper.ReturnToPool(tmpParentTransform.gameObject, tmpParentTransform.gameObject.GetComponent<TetrominoType>().type);
                            } else if (grid[i][j][k].parent != null && grid[i][j][k].parent.childCount < 4) { // parent != null && childCount < 4
                                foreach (Transform transform in grid[i][j][k].parent) {
                                    string type = transform.gameObject.GetComponent<MinoType>() == null ?
                                        new StringBuilder("mino").Append(grid[i][j][k].parent.gameObject.GetComponent<TetrominoType>().type.Substring(5, 1)).ToString()
                                        : transform.gameObject.GetComponent<MinoType>().type;
                                    // grid[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z)] = null;
                                    x = (int)Mathf.Round(transform.position.x);
                                    y = (int)Mathf.Round(transform.position.y);
                                    z = (int)Mathf.Round(transform.position.z);
                                    if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth && z >= 0 && z < gridWidth) {
                                        grid[x][y][z] = null;
                                        gridOcc[x][y][z] = 0;
                                    }
                                    PoolHelper.ReturnToPool(transform.gameObject, type);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void printbuttonInteractableList() {
            for (int i = 0; i <= 6; i++) 
                Debug.Log(TAG + " buttonInteractableList[i]: i : " + i + ", " + buttonInteractableList[i]); 
        }

        public void playFirstTetromino(GameObject previewTetromino,
                                        GameObject previewTetromino2,
                                        GameObject cycledPreviewTetromino) {
// 在生成新的一两预览前将现两个预览保存起来
            prevPreview = comTetroType.Value;
            prevPreview2 = eduTetroType.Value;
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino2); // 用第一个,回收第二个
            nextTetrominoType.Value = comTetroType.Value; // 记忆功能
            PoolHelper.ReturnToPool(previewTetromino2, previewTetromino2.GetComponent<TetrominoType>().type);
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
                                        GameObject previewTetromino2,
                                        GameObject cycledPreviewTetromino) {
            prevPreview = comTetroType.Value;
            prevPreview2 = eduTetroType.Value;
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino);
            cycledPreviewTetromino = previewTetromino;
            nextTetrominoType.Value = eduTetroType.Value; // 记忆功能
            PoolHelper.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type); // 回收一个方块砖
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
            // enables: undoButton toggleButton fallButton
            if (gameMode.Value  == 0) {
                buttonInteractableList[0] = 0;
                buttonInteractableList[1] = 0;
                buttonInteractableList[2] = 0;
                buttonInteractableList[3] = 1;
                buttonInteractableList[4] = 1;
                buttonInteractableList[5] = 1;
            }
        }

        public void FillInMinoAtTargetPosition(int x, int y, int z, Transform minoTrans) {
            int o = gridHeight - 1;
            // grid[x, o, z] = 0, otherwose gameover
            while (grid[x][o-1][z] == null) o--;
            for (int i = o; i > y; i--) { // y // if (grid[x] i - 1,[z] != null) {
                grid[x][i][z] = grid[x][i-1][z];
                gridOcc[x][i][z] = gridOcc[x][i-1][z];
                if (grid[x][i-1][z] != null) {
                    grid[x][i-1][z] = null;
                    gridOcc[x][i-1][z] = 0;
                    grid[x][i][z].position += new Vector3(0, 1, 0);
                }
            }
            grid[x][y][z] = minoTrans;
            gridOcc[x][y][z] = 1;
        }
        public bool isColumnFromHereEmpty(int x, int y, int z) {
            bool isColumnAboveEmpty = true;
            for (int i = y; i < gridHeight; i++) {
                if (grid[x][y][z] != null)
                    isColumnAboveEmpty = false;
            }
            return isColumnAboveEmpty;
        }
        public bool isTheParentChildren(Transform tmpParent, TetrominoDataCon parent) { // compare against SerializedTransform parent
            return (tmpParent.gameObject.name == parent.name &&
                    tmpParent.position == DeserializedTransform.getDeserializedTransPos(parent.transform) &&
                    tmpParent.rotation == DeserializedTransform.getDeserializedTransRot(parent.transform));
        }

        // public bool existInChildren(Transform transform, MinoDataCollection<TetrominoData, MinoData> children) {
        //     foreach (MinoData data in children) {
        //         if (transform.position == DeserializedTransform.getDeserializedTransPos(data.transform) &&
        //             transform.rotation == DeserializedTransform.getDeserializedTransRot(data.transform)) {
        //             return true;
        //         }
        //     }
        //     return false;
        // }

        // public void moveRotatecanvasPrepare() {
        //     // Debug.Log(TAG + ": moveRotatecanvasPrepare()"); 
        //     moveCanvas.transform.localPosition = new Vector3(2.1f, gridHeight - 1f, 2.1f);     
        //     rotateCanvas.transform.localPosition = new Vector3(2.1f, gridHeight - 1f, 2.1f);
        //     isMovement = false;
        //     toggleButtons(1);
        // }

        public void resetGridOccBoard() {
            for (int y = 0; y < gridHeight; y++) {
                for (int x = 0; x < gridWidth; x++) {
                    for (int z = 0; z < gridWidth; z++) {
                        gridOcc[x][y][z] = 0;
                    }
                }
            }
        }

        public void UpdateLevel() {
            if (startingAtLevelZero || (!startingAtLevelZero && numLinesCleared.Value  / 10 > startingLevel)) 
                currentLevel.Value  = numLinesCleared.Value  / 10;
        }

        public void UpdateSpeed() { 
            fallSpeed = 1.0f - (float)currentLevel.Value  * 0.1f;
        }
    
        public bool CheckIsAboveGrid(Tetromino tetromino) {
            // public bool CheckIsAboveGrid(Transform transform) {
            for (int x = 0; x < gridWidth; x++)
                for (int j = 0; j < gridWidth; j++) 
                    foreach (Transform mino in tetromino.transform) {
                        Vector3 pos = MathUtil.Round(mino.position);
                        //if (mino.CompareTag("mino" && pos.y > gridHeight - 1)) 
                        //if (mino.CompareTag("mino" && pos.y >= gridHeight - 1))
                        if (pos.y >= gridHeight - 1) // BUG: for game auto ended after first tetromino landing down
                            return true;
                    }
            return false;
        }

        public bool IsFullRowAt(int y) {
            for (int x = 0; x < gridWidth; x++)
                for (int j = 0; j < gridWidth; j++)
// 下面的两句话:临时先写成ViewManager.GameView.ghostTetromino.transform ,之后再重构                    
                    if (grid[x][y][j] == null ||      // modified here for ghostTetromino
                        (grid[x][y][j].parent == ViewManager.ghostTetromino.transform
                         && grid[x][y][j].parent != ViewManager.nextTetromino.transform)) 
                        return false;
            numberOfRowsThisTurn++;
            return true;
        }
        public void DeleteRow() { // 算法上仍然需要优化
            Debug.Log(TAG + ": DeleteRow() start");
            hasDeletedMinos = false;
            for (int y = 0; y < gridHeight; y++) {
                if (gameMode.Value  > 0) {
                    if (IsFullRowAt(y)) {
                        // 一定要控制同屏幕同时播放的粒子数量
                        // 1.同屏的粒子数量一定要控制在200以内，每个粒子的发射数量不要超过50个。
                        // 2.尽量减少粒子的面积，面积越大就会越卡。
                        // 3.粒子最好不要用Alfa Test（但是有的特效又不能不用，这个看美术吧）
                        //   粒子的贴图用黑底的这种，然后用Particles/Additive 这种Shader，贴图必须要2的幂次方，这样渲染的效率会高很多。个人建议 粒子特效的贴图在64左右，千万不要太大。
                        // // 让游戏中真正要播放粒子特效的时候，粒子不用在载入它的贴图，也不用实例化，仅仅是执行一下SetActive(true)。
                        // SetActive(true)的时候就不会执行粒子特效的Awake()方法，但是它会执行OnEnable方法。
                        hasDeletedMinos = true;
                        
                        // m_ExplosionParticles.transform.position = new Vector3(2.5f, y, 2.5f); 
                        // m_ExplosionParticles.gameObject.SetActive(true);
                        // m_ExplosionParticles.Play();
                        // m_ExplosionAudio.Play();

                        DeleteMinoAt(y);
                        MoveAllRowsDown(y + 1);
                        --y;
                    }
                } else { // easy mode
                    if (IsFullFiveInLayerAt(y)) { // 有可能是 bug 的存在
                        hasDeletedMinos = true;

                        DeleteMinoAt(y);
                        // MoveRowDown(y + 1);
                        MoveAllRowsDown(y + 1);
                        --y;
                    }
                }
            }
            if (gameMode.Value  == 0) {
                // clean top layer 2 out: all 2s to be 0s
                for (int o = gridHeight - 1; o >= 0; o--) {
                    for (int x = 0; x < gridWidth; x++) {
                        for (int z = 0; z < gridWidth; z++) {
                            if (gridOcc[x][o][z] == 2) {
                                if (o == gridHeight - 1 ||  gridOcc[x][o+1][z] == 0) // this statement ???
                                    gridOcc[x][o][z] = 0;
                            }
                        }
                    }
                } 
            }
            // Debug.Log(TAG + ": After all DeleteRow() finish");
            // MathUtil.printBoard(gridOcc); 
        }
        public bool IsFullFiveInLayerAt(int y) {
            // Debug.Log(TAG + ": IsFullFiveInLayerAt()");
            int tmpSum = 0;
            bool result = false;
            for (int x = 0; x < gridWidth; x++) {
                tmpSum = 0;
                for (int z = 0; z < gridWidth; z++) {
                    tmpSum += gridOcc[x][y][z] == 0 ? 0 : 1; 
                }
                if (tmpSum == gridWidth) {
                    numberOfRowsThisTurn++;
                    result = true;
                    for (int z = 0; z < gridWidth; z++) {
                        gridOcc[x][y][z] += gridOcc[x][y][z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5列
            for (int z = 0; z < gridWidth; z++) { // 数5行
                tmpSum = 0;
                for (int x = 0; x < gridWidth; x++) {
                    tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                }
                if (tmpSum == gridWidth) {
                    numberOfRowsThisTurn++;
                    result = true;
                    for (int x = 0; x < gridWidth; x++) {
                        gridOcc[x][y][z] += gridOcc[x][y][z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5行
            tmpSum = 0;
            for (int x = 0; x < gridWidth; x++) { // 数2对角线
                for (int z = 0; z < gridWidth; z++) {
                    if (x == z) {
                        tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                    }
                }
            }
            if (tmpSum == gridWidth) {
                result = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridWidth; x++) { // 数2对角线
                    for (int o = 0; o < gridWidth; o++) {
                        if (x == o) {
                            gridOcc[x][y][o] += gridOcc[x][y][o] == 1 ? 1 : 0;  
                        }
                    }
                }
            } // 数完一条对角线
            tmpSum = 0; 
            for (int x = 0; x < gridWidth; x++) {
                for (int z = 0; z < gridWidth; z++) {
                    if (z == gridWidth - 1 - x) {
                        tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                    }
                }
            }
            if (tmpSum == gridWidth) {
                result = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridWidth; x++) {
                    for (int o = 0; o < gridWidth; o++) {
                        if (o == gridWidth - 1 - x) {
                            gridOcc[x][y][o] += gridOcc[x][y][o] == 1 ? 1 : 0; 
                        }
                    }
                }
            } // 数完另一条对角线
            // MathUtil.printBoard(gridOcc); // 太多打印的了
            if (result) {
                return true;
            }
            return false;
        }

        public void DeleteMinoAt(int y) {
            Debug.Log(TAG + ": DeleteMinoAt() start"); 
            for (int x = 0; x < gridWidth; x++) 
                for (int  z = 0;  z < gridWidth;  z++) {
                    if (gameMode.Value  == 0) { // 只删除合格 行 列 对角线 占有的格
                        if (gridOcc != null && gridOcc[x][y][z] == 2) {
                            if (grid[x][y][z] != null && grid[x][y][z].gameObject != null) {
                                // Debug.Log(TAG + " (grid[x][y][z].parent != null): " + (grid[x][y][z].parent != null)); 
                                if (grid[x][y][z].parent != null) {
                                    // Debug.Log(TAG + " grid[x][y][z].parent.name: " + grid[x][y][z].parent.name);
                                    // Debug.Log(TAG + " grid[x][y][z].parent.childCount: " + grid[x][y][z].parent.childCount); 
                                    if (grid[x][y][z].parent.childCount == 1) {
                                        Transform tmp = grid[x][y][z].parent;
                                        tmp.GetChild(0).parent = null;
                                        GameObject.Destroy(grid[x][y][z].gameObject);
                                        GameObject.Destroy(tmp.gameObject);
                                    } else {
                                        grid[x][y][z].parent = null;
                                        GameObject.Destroy(grid[x][y][z].gameObject);
                                    }
                                    grid[x][y][z] = null;
                                    // gridOcc[x][y][z] = 0;
                                } else {
                                    // PoolManager.ReturnToPool(grid[x][y][z].gameObject, GetSpecificPrefabMinoType(grid[x][y][z].gameObject));
                                    GameObject.Destroy(grid[x][y][z].gameObject);
                                    // gridOcc[x][y][z] = 0; // 暂时还不更新，等要删除的时候才更新
                                    grid[x][y][z] = null; // x ==> z
                                }
                            }
                        } else if (grid[x][y][z] == null && gridOcc[x][y][z] == 2) { // grid[x][y][z] = null
                            // gridOcc[x][y][z] = 0;
                        }
                    } else {
                        GameObject.Destroy(grid[x][y][z].gameObject);
                        // PoolManager.ReturnToPool(grid[x][y][z].gameObject, GetSpecificPrefabMinoType(grid[x][y][z].gameObject));
                        grid[x][y][z] = null;
                        gridOcc[x][y][z] = 0; // 其它 mode 好像也不需要这个东西
                    }
                }
        }
        public void MoveRowDown(int y) {
            if (gameMode.Value  > 0) {
                for (int j = 0; j < gridWidth; j++)    
                    for (int x = 0; x < gridWidth; x++) {
                        if (grid[x][y][j] != null) {
                            grid[x][y-1][j] = grid[x][y][j];
                            grid[x][y][j] = null;
                            grid[x][y-1][j].position += new Vector3(0, -1, 0);
                        }
                    }
            } else { // gameMode.Value  == 0
                for (int x = 0; x < gridWidth; x++) {
                    for (int z = 0; z < gridWidth; z++) {
                        if (gridOcc[x][y-1][z] == 2) { // 下面是消毁掉了的，压下去
                            gridOcc[x][y-1][z] = gridOcc[x][y][z];

                            if (grid[x][y][z] != null) {
                                grid[x][y-1][z] = grid[x][y][z];
                                grid[x][y][z] = null;
                                grid[x][y-1][z].position += new Vector3(0, -1, 0);
                            }
                            gridOcc[x][y][z] = y == gridHeight - 1 ? 0 : 2;
                        }
                    }   
                }
            } // gameMode.Value  == 0
        } 

        public void MoveAllRowsDown(int y) {
            for (int i = y; i < gridHeight; i++) 
                MoveRowDown(i);
        }
    
        public bool CheckIsInsideGrid(Vector3 pos) {
            return ((int)pos.x >= 0 && (int)pos.x < gridWidth &&
                    (int)pos.z >= 0 && (int)pos.z < gridWidth && 
                    (int)pos.y >= 0 && (int)pos.y < gridHeight); 
        }

        public Transform GetTransformAtGridPosition(Vector3 pos) {
            if (pos.y > gridHeight - 1) 
                return null;
            else
                return grid[(int)pos.x][(int)pos.y][(int)pos.z];
        }
        public bool CheckIsValidPosition() { // TODO: 写了两遍 Tetromino GameViewModel
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                    if (!CheckIsInsideGrid(pos)) 
                        return false;
                    if (GetTransformAtGridPosition(pos) != null
                        && GetTransformAtGridPosition(pos).parent != ViewManager.nextTetromino.transform) {
                        return false;
                    }
                }
            }
            return true;
        }
        
        public void recycleNextTetromino(GameObject nextTetromino) { // 这个折成两部分来写
            Debug.Log(TAG + ": recycleNextTetromino()"); 
            if (nextTetromino != null) {
                PoolHelper.recycleNextTetromino();
                // nextTetromino.tag = "Untagged";
                // nextTetromino.GetComponent<Tetromino>().enabled = false;
                resetGridAfterDisappearingNextTetromino(ViewManager.nextTetromino); // check ???
                // if (nextTetromino.transform.childCount == 4) {
                //     PoolHelper.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
                // } else
                //     GameObject.Destroy(nextTetromino.gameObject);
            }
        }
        public void recycleThreeMajorTetromino(
            GameObject nextTetromino,
            GameObject previewTetromino,
            GameObject previewTetromino2) {
            // 回收三样东西： nextTetromino previewTetromino previewTetromino2
            recycleNextTetromino(nextTetromino);
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino);
            PoolHelper.ReturnToPool(previewTetromino, previewTetromino.GetComponent<TetrominoType>().type);
            PoolHelper.preparePreviewTetrominoRecycle(previewTetromino2);
            PoolHelper.ReturnToPool(previewTetromino2, previewTetromino2.GetComponent<TetrominoType>().type);
        }

//　操纵两画面的上下移动
       public void MoveDown() {
           ViewManager.moveCanvas.transform.position += new Vector3(0, -1, 0);
           ViewManager.rotateCanvas.transform.position += new Vector3(0, -1, 0);
       }
       public void MoveUp() {
           ViewManager.moveCanvas.transform.position += new Vector3(0, 1, 0);
           ViewManager.rotateCanvas.transform.position += new Vector3(0, 1, 0);
       }

        // public string GetRandomTetromino(BindableProperty<String> tetroTypeBindableProperty) { // active Tetromino
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
            // if (gameMode.Value == 0 && gridWidth == 5)
            //     randomTetromino = UnityEngine.Random.Range(15, 21);
            // else 
            //     randomTetromino = UnityEngine.Random.Range(7, 14);
            // Debug.Log(TAG + " Generated randomTetromino: " + randomTetromino); 
            // StringBuilder tetrominoType = new StringBuilder("Tetromino");
            // switch (randomTetromino) {
            // case 15: tetrominoType.Append("I"); break;
            // case 16: tetrominoType.Append("J"); break; 
            // case 17: tetrominoType.Append("L"); break;
            // case 18: tetrominoType.Append("O"); break;
            // case 19: tetrominoType.Append("S"); break;
            // case 20: tetrominoType.Append("T"); break;
            // default:
            //     tetrominoType.Append("Z"); break;
            // }
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
//         public void toggleButtons(int indicator) { // 留个残影作最后的参考
//             Debug.Log(TAG + ": toggleButtons()");
//             Debug.Log(TAG + " buttonInteractableList[4]: " + buttonInteractableList[4]);
//             Debug.Log(TAG + " indicator: " + indicator); 
//             if (gameMode.Value  == 0 && buttonInteractableList[4] == 0 && indicator == 0) return;
//             if (gameMode.Value  > 0 || indicator == 1 || buttonInteractableList[4] == 1) {
//                 if (isMovement) { 
//                     isMovement = false;
// // 那个按钮的图片切换,暂时不管                    , 空空 NULL NULL
//                     // invertButton.image.overrideSprite = prevImg; // rotation img 这两个图像还需要处理一下
//                     ViewManager.moveCanvas.gameObject.SetActive(false);
//                     ViewManager.rotateCanvas.SetActive(true); 
//                 } else {
//                     isMovement = true;
//                     // invertButton.image.overrideSprite = newImg;
//                     ViewManager.moveCanvas.gameObject.SetActive(true);
//                     ViewManager.rotateCanvas.SetActive(false);
//                 }
//             }
//         }

    }
}
