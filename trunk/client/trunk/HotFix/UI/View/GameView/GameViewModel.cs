using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.MVVM;
using HotFix.Control;
using HotFix.Data;
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
        public int currentScore { set; get; }

        public int gridHeight = 12; 
        public int gridWidth;

        public static Transform [][] tmpTest;
        public static Transform [][][] grid; //= new Transform[gridWidth, gridHeight, gridWidth];
        public static int [][][] gridOcc; //= new int[gridWidth, gridHeight, gridWidth];
        public int scoreOneLine = 40;
        public int scoreTwoLine = 100;
        public int scoreThreeLine = 300;
        public int scoreFourLine = 1200;
    
        public int currentLevel = 0;  
        public static bool startingAtLevelZero;
        public static int startingLevel;
    
        private int numberOfRowsThisTurn = 0;

        // private Vector3 previewTetrominoPosition = new Vector3(-17f, -5f, -9.81f); 
        // private Vector3 previewTetromino2Position = new Vector3(-68.3f, 19.6f, 32.4f); // (-56.3f, -0.1f, 32.4f) (-24.8f, -0.1f, 32.4f);
    
        public int numLinesCleared = 0;

        private int startingHighScore;
        private int startingHighScore2;
        private int startingHighScore3;
    
        public bool isMovement = true;
        private int randomTetromino;
    
        public Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f);

        public int gameMode = 0; 

        public string prevPreview; // to remember previous spawned choices
        public string prevPreview2;
        public string nextTetrominoType;  
        public string previewTetrominoType; 
        public string previewTetromino2Type;

        // private SaveGameEventInfo saveGameInfo;
        public bool hasDeletedMinos = false;
        public bool loadSavedGame = false;
        public bool isDuringUndo = false;

    // 不知道临时拿了这个作了什么用,一定要用上这个?
        private GameObject tmpParentGO;

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe(); //
        }

        void Initialization() {
            this.ParentViewModel = (MenuViewModel)ViewManager.MenuView.BindingContext; // 父视图模型: 菜单视图模型
            gridWidth = ((MenuViewModel)ParentViewModel).gridWidth;

            Debug.Log("Initialization bef arrays");
            tmpTest = new Transform [5][];
            for (int i = 0; i < 5; i++) 
                tmpTest[i] = new Transform[5];
                
            Debug.Log("Initialization after [] arrays");

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
            
            Debug.Log("Initialization aft arrays");
            fallSpeed = 3.0f;
            saveForUndo = true;
        }

        void DelegateSubscribe() { // 这里怎么写成是观察者模式呢?
            
        }

        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() { // 这是里是指初始化数据管理,而不是视图层面

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

        public void LoadDataFromParentList(List<TetrominoData> parentList) {
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0;
            StringBuilder type = new StringBuilder("");
            foreach (TetrominoData parentData in parentList) {
                // Debug.Log(TAG + " parentData.name: " + parentData.name);
                // Debug.Log(TAG + " parentData.children.Count: " + parentData.children.Count);
// // 下面这里要真正的重构:因为无法拿到子立方体的集合数据                
//                 if (isThereAnyExistChild(parentData)) { // 存在
//                     if (!gridMatchesSavedParent(tmpParentGO, (List<MinoData>)(parentData.children.collection))) {  // 先删除多余的，再补全缺失的
//                         foreach (Transform trans in tmpParentGO.transform) { // 先 删除多余的
//                             // MathUtil.print(MathUtil.Round(trans.position));
//                             // Debug.Log(TAG + " (!myContains(trans, (List<MinoData>)(parentData.children.collection))): " + (!myContains(trans, (List<MinoData>)(parentData.children.collection)))); 
//                             if (!myContains(trans, (List<MinoData>)(parentData.children.collection))) {
//                                 x = (int)Mathf.Round(trans.position.x);
//                                 y = (int)Mathf.Round(trans.position.y);
//                                 z = (int)Mathf.Round(trans.position.z);
//                                 // MathUtil.print(x, y, z); // this one is right
//                                 grid[x, y, z].parent = null;
//                                 GameObject.Destroy(grid[x, y, z].gameObject);
//                                 gridOcc[x, y, z] = 0;
//                                 grid[x, y, z] = null;
//                             }
//                         }
//                         // Debug.Log(TAG + " tmpParentGO.transform.childCount (deleted unwanted): " + tmpParentGO.transform.childCount);
//                         foreach (MinoData minoData in parentData.children) {
//                             Vector3 posA = MathUtil.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
//                             MathUtil.print(posA);
//                             x = (int)Mathf.Round(posA.x);
//                             y = (int)Mathf.Round(posA.y);
//                             z = (int)Mathf.Round(posA.z);
//                             if (grid[x, y, z] == null) {
//                                 // MathUtil.print(x, y, z);
//                                 type.Length = 0;
//                                 GameObject tmpMinoGO = PoolManager.Instance.GetFromPool(type.Append(minoData.type).ToString(), DeserializedTransform.getDeserializedTransPos(minoData.transform), 
//                                                                                         DeserializedTransform.getDeserializedTransRot(minoData.transform));
//                                 grid[x, y, z] = tmpMinoGO.transform;
//                                 grid[x, y, z].parent = tmpParentGO.transform;
//                                 gridOcc[x, y, z] = 1;
//                             }
//                         }
//                     }
//                     // Debug.Log(TAG + " tmpParentGO.transform.childCount (filled needed -- final): " + tmpParentGO.transform.childCount);
//                 } else { // 重新生成                                           // 空 shapeX Tetromino_X : Universal
//                     GameObject tmpGameObject = PoolManager.Instance.GetFromPool("shapeX", DeserializedTransform.getDeserializedTransPos(parentData.transform), 
//                                                                                 DeserializedTransform.getDeserializedTransRot(parentData.transform));
//                     foreach (MinoData minoData in parentData.children) {
//                         GameObject tmpMinoGO = PoolManager.Instance.GetFromPool(minoData.type, DeserializedTransform.getDeserializedTransPos(minoData.transform), 
//                                                                                 DeserializedTransform.getDeserializedTransRot(minoData.transform));
//                         tmpMinoGO.transform.parent = tmpGameObject.transform;
//                         x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
//                         y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
//                         z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
//                         // fix for bug 5: fill in a Mino into board in y where there are more minos are above the filled in one
//                         if (isColumnFromHereEmpty(x, y, z)) {
//                             grid[x, y, z] = tmpMinoGO.transform;
//                             gridOcc[x, y, z] = 1;
//                         } else {
//                             FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform); // update grid accordingly
//                         }
//                     }
//                     tmpGameObject.GetComponent<TetrominoType>().type = parentData.type;
//                     tmpGameObject.name = parentData.name;
//                     // Debug.Log(TAG + " tmpGameObject.GetComponent<TetrominoType>().type: " + tmpGameObject.GetComponent<TetrominoType>().type); 
//                     // Debug.Log(TAG + " tmpGameObject.transform.childCount: " + tmpGameObject.transform.childCount); 
//                 }
//                 // Debug.Log(TAG + ": gridOcc[,,] after each deleted mino re-spawn"); 
//                 // MathUtil.printBoard(gridOcc); 
            }
        }

        public void recycleGhostTetromino(GameObject ghostTetromino) {
            Debug.Log(TAG + ": recycleGhostTetromino()");
            Debug.Log(TAG + " ghostTetromino.name: " + ghostTetromino.name); 
            // Debug.Log(TAG + " (ghostTetromino == null): " + (ghostTetromino == null));
            // Debug.Log(TAG + " ghostTetromino.tag: " + ghostTetromino.tag); 
            // Debug.Log(TAG + " ghostTetromino.CompareTag(\"currentGhostTetromino\"): " + ghostTetromino.CompareTag("currentGhostTetromino")); 
            if (ghostTetromino != null) {
                ghostTetromino.tag = "Untagged";
                PoolManager.Instance.ReturnToPool(ghostTetromino, ghostTetromino.GetComponent<TetrominoType>().type);
            }
        }
        public void recycleNextTetromino(GameObject nextTetromino) {
            Debug.Log(TAG + ": recycleNextTetromino()"); 
            if (nextTetromino != null) {
                nextTetromino.tag = "Untagged";
                nextTetromino.GetComponent<Tetromino>().enabled = false;
                resetGridAfterDisappearingNextTetromino(nextTetromino);  // this one for undo click only ???? Nonono
                if (nextTetromino.transform.childCount == 4) {
                    PoolManager.Instance.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
                } else
                    GameObject.Destroy(nextTetromino.gameObject);
            }
            // nextTetromino = null;
        }
        public void recycleThreeMajorTetromino(
            GameObject nextTetromino,
            GameObject previewTetromino,
            GameObject previewTetromino2) {
            // 回收三样东西：nextTetromino previewTetromino previewTetromino2
            recycleNextTetromino(nextTetromino);
            // preparePreviewTetrominoRecycle(1);
            preparePreviewTetrominoRecycle(previewTetromino);
// // 不知道这里的写法算是怎么回事,忘记了,回来再来检查            
//             PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
//             // preparePreviewTetrominoRecycle(2);
//             preparePreviewTetrominoRecycle(previewTetromino2);
//             PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        }

        public void preparePreviewTetrominoRecycle(GameObject cycledPreviewTetromino) { 
            // cycledPreviewTetromino = i == 1 ? previewTetromino : previewTetromino2;
            // cycledPreviewTetromino.GetComponent<Rotate>().enabled = !cycledPreviewTetromino.GetComponent<Rotate>().enabled; // disable
            cycledPreviewTetromino.transform.localScale -= previewTetrominoScale;
            cycledPreviewTetromino.transform.position = Vector3.zero;
            cycledPreviewTetromino.transform.rotation = Quaternion.identity;
            cycledPreviewTetromino.SetActive(false);
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

        // void onGameSave(SaveGameEventInfo info) {
        public void onGameSave() {
            Debug.Log(TAG + ": onGameSave()");
// // 这也是那个时候写得逻辑不对称的乱代码,要归位到真正用它的地方,而不是摆放在这里            
//             if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
//                 tmpTransform = new GameObject().transform;
            SaveSystem.SaveGame(this);
        }
        public void CheckUserInput() {  // originally pasuseButton & continueButton
            Debug.Log(TAG + ": CheckUserInput()"); 
            if (Time.timeScale == 1.0f) {
                PauseGame();
            } else {
// // 这里需要补充成视图模型里的恢复游戏的逻辑,而不是视图层面的                
//                 onResumeGame();
            }
        }

        public void PauseGame() {
            Time.timeScale = 0f;	    
            AudioManager.Instance.Pause();
            isPaused = true;
            // Bug cleaning: when paused game, if game has NOT started yet, disable Save Button
            if (!gameStarted) {
                
            }
        }

        public void onActiveTetrominoLand(TetrominoLandEventInfo info,
                                          GameObject nextTetromino) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            UpdateGrid(nextTetromino);

            // Debug.Log(TAG + ": gridOcc[,,] before Land and Save()"); 
            // MathUtil.printBoard(gridOcc); 

            // recycleGhostTetromino();

            // // SaveGameEventInfo fire here 
            // saveGameInfo = new SaveGameEventInfo();
            // EventManager.Instance.FireEvent(saveGameInfo);
            // change an approach: it is unnessary and do NOT apply delegates and events here
            onGameSave();

            DeleteRow();
            Debug.Log(TAG + " (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>())): " + (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>()))); 
            if (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>())) {
                // GameOver(); // for tmp
            }            
            // DisableMoveRotationCanvas();
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            if (gameMode == 0) {
                buttonInteractableList[0] = 1;
                buttonInteractableList[1] = 1;
                buttonInteractableList[2] = 1;
                buttonInteractableList[3] = 1; // undo button
            }
            // else 
            //     SpawnnextTetromino();  
        }

        public void cleanUpGameBroad(GameObject nextTetromino, GameObject ghostTetromino) {
            Debug.Log(TAG + ": cleanUpGameBroad()");
            // dealing with currentActiveTetromino & ghostTetromino firrst
            if (nextTetromino != null && nextTetromino.CompareTag("currentActiveTetromino")) { // hang in the air
                Debug.Log(TAG + " (ghostTetromino != null): " + (ghostTetromino != null));  // always true
                if (ghostTetromino != null) {
                    recycleGhostTetromino(ghostTetromino);
                }
                recycleNextTetromino(nextTetromino);
            }
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
                                PoolManager.Instance.ReturnToPool(tmpParentTransform.gameObject, tmpParentTransform.gameObject.GetComponent<TetrominoType>().type);
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
                                    PoolManager.Instance.ReturnToPool(transform.gameObject, type);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void InitializationForNewGame() {
            gameMode = ((MenuViewModel)ParentViewModel).gameMode;
            fallSpeed = 3.0f; // should be recorded too, here
            if (gameMode == 0)
                resetGridOccBoard();
            currentScore = 0;
            currentLevel = startingLevel;
        }
        
        public void printbuttonInteractableList() {
            for (int i = 0; i < 6; i++) 
                Debug.Log(TAG + " buttonInteractableList[i]: i : " + i + ", " + buttonInteractableList[i]); 
        }

        public void playSecondTetromino(GameObject previewTetromino,
                                        GameObject previewTetromino2,
                                        GameObject cycledPreviewTetromino) {
            prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            preparePreviewTetrominoRecycle(previewTetromino);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino2.transform.localScale -= previewTetrominoScale;
            // previewTetromino2.layer = LayerMask.NameToLayer("Default");
            // previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;

            // nextTetromino = previewTetromino2;
            // currentActiveTetrominoPrepare();
            gameStarted = true;
            
            // SpawnGhostTetromino();  
            // moveRotatecanvasPrepare();
            // SpawnPreviewTetromino();
            
            // disables: previewSelectionButton previewSelectionButton2 swapPreviewTetrominoButton
            // enables: undoButton toggleButton fallButton
            if (gameMode == 0) {
                buttonInteractableList[0] = 0;
                buttonInteractableList[1] = 0;
                buttonInteractableList[2] = 0;
                buttonInteractableList[3] = 1;
                buttonInteractableList[4] = 1;
                buttonInteractableList[5] = 1;
            }
            // printbuttonInteractableList();
        }
        public void onUndoGame() { 
            Debug.Log(TAG + ": onUndoGame()");
            if (buttonInteractableList[3] == 0) return;
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            isDuringUndo = true;
            // recycleThreeMajorTetromino();

            StringBuilder path = new StringBuilder("");
            // if (!string.IsNullOrEmpty(GameMenuData.Instance.saveGamePathFolderName)) 
            path.Append(Application.persistentDataPath + "/" + ((MenuViewModel)ParentViewModel).saveGamePathFolderName + "/game" + ".save");
            // else
            //     path.Append(Application.persistentDataPath + "/game" + ".save");
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            StringBuilder type = new StringBuilder("");
            if (hasDeletedMinos) {
                currentScore = gameData.score;
                currentLevel = gameData.level;
                numLinesCleared = gameData.lines;
                // hud_score.text = currentScore.ToString();
                // hud_level.text = currentLevel.ToString(); // 这不希望变的
                // hud_lines.text = numLinesCleared.ToString();

                // Debug.Log(TAG + ": onUndoGame() current board before respawn"); 
                // MathUtil.printBoard(gridOcc); 
                
                Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                LoadDataFromParentList(gameData.parentList);

                GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            }
            // moveCanvas.gameObject.SetActive(false);   // moveCanvas rotateCanvas: SetActive(false)
            // rotateCanvas.gameObject.SetActive(false);
            if (gameData.prevPreview != null) { // previewTetromino previewTetromino2
                type.Length = 0;
                string type2 = gameData.prevPreview2;
                // SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2);
            }
            buttonInteractableList[0] = 1; 
            buttonInteractableList[1] = 1; 
            buttonInteractableList[2] = 1; 
            buttonInteractableList[3] = 0; // buttons are supposed to click once at a time only
            isDuringUndo = false;
        }
        
        public bool isThereAnyExistChild(TetrominoData parentData) {
            // Debug.Log(TAG + ": isThereAnyExistChild()"); 
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
// 这么写是不通的,所以要用观察者模式,视图观察视图模型里的TRANSFORM变化                        
                        // ViewManager.GameView.tmpParentGO = grid[x][y][z].parent.gameObject;  // for tmp
                        return true;
                    }
                }
            }
            return false;
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
        public bool gridMatchesSavedParent(GameObject tmpGO, List<MinoData> data) {
            if (tmpGO.transform.childCount == 4 && data.Count == 4) return true;
            else if (tmpGO.transform.childCount != data.Count) return false;
            else { // tmpGO.transform.childCount == data.children.Count
                foreach (Transform trans in tmpParentGO.transform) {
                    if (!myContains(trans, data))
                        return false;
                }
                return true;
            }
        }
        public bool myContains(Transform tmp, List<MinoData> children) {
            foreach (MinoData data in children)
                if (MathUtil.Round(tmp.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(data.transform)) &&
                    MathUtil.Round(tmp.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(data.transform)))
                    return true;
            return false;
        }

        public bool isTheParentChildren(Transform tmpParent, TetrominoData parent) { // compare against SerializedTransform parent
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

        public string GetRandomTetromino() { // active Tetromino
            Debug.Log(TAG + ": GetRandomTetromino()"); 
            if (gameMode == 0 && gridWidth == 3)
                randomTetromino = UnityEngine.Random.Range(1, 7);
            else 
                randomTetromino = UnityEngine.Random.Range(1, 8);
            Debug.Log(TAG + " randomTetromino: " + randomTetromino); 
            StringBuilder tetrominoType = new StringBuilder("shape");
            switch (randomTetromino) {
            case 1: tetrominoType.Append("J"); break;
            case 2: tetrominoType.Append("Z"); break; 
            case 3: tetrominoType.Append("L"); break;
            case 4: tetrominoType.Append("S"); break;
            case 5: tetrominoType.Append("O"); break;
            case 6: tetrominoType.Append("T"); break;
                // case 7:
            default:
                tetrominoType.Append("I"); break;
            }
            return tetrominoType.ToString();
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

        public void toggleButtons(int indicator) {
            Debug.Log(TAG + ": toggleButtons()");
            Debug.Log(TAG + " buttonInteractableList[4]: " + buttonInteractableList[4]);
            Debug.Log(TAG + " indicator: " + indicator); 
            if (gameMode == 0 && buttonInteractableList[4] == 0 && indicator == 0) return;
            if (gameMode > 0 || indicator == 1 || buttonInteractableList[4] == 1) {
                if (isMovement) { 
                    isMovement = false;
                    // invertButton.image.overrideSprite = prevImg; // rotation img 这两个图像还需要处理一下
                    // moveCanvas.gameObject.SetActive(false);
                    // rotateCanvas.SetActive(true); 
                } else {
                    isMovement = true;
                    // invertButton.image.overrideSprite = newImg;
                    // moveCanvas.gameObject.SetActive(true);
                    // rotateCanvas.SetActive(false);
                }
            }
        }

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
            if (startingAtLevelZero || (!startingAtLevelZero && numLinesCleared / 10 > startingLevel)) 
                currentLevel = numLinesCleared / 10;
        }

        public void UpdateSpeed() { 
            fallSpeed = 1.0f - (float)currentLevel * 0.1f;
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
                        (grid[x][y][j].parent == ViewManager.GameView.ghostTetromino.transform
                         && grid[x][y][j].parent != ViewManager.GameView.nextTetromino.transform)) 
                        return false;
            numberOfRowsThisTurn++;
            return true;
        }
        public void DeleteRow() { // 算法上仍然需要优化
            Debug.Log(TAG + ": DeleteRow() start");
            hasDeletedMinos = false;
            for (int y = 0; y < gridHeight; y++) {
                if (gameMode > 0) {
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
            if (gameMode == 0) {
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
                    if (gameMode == 0) { // 只删除合格 行 列 对角线 占有的格
                        if (gridOcc != null && gridOcc[x][y][z] == 2) {
                            Debug.Log("(x,y,z): [" + x + "," + y + "," + z +"]: " + gridOcc[x][y][z]); 
                            if (grid[x][y][z] != null && grid[x][y][z].gameObject != null) {
                                // Debug.Log(TAG + " (grid[x][y][z].parent != null): " + (grid[x][y][z].parent != null)); 
                                if (grid[x][y][z].parent != null) {
                                    Debug.Log(TAG + " grid[x][y][z].parent.name: " + grid[x][y][z].parent.name);
                                    Debug.Log(TAG + " grid[x][y][z].parent.childCount: " + grid[x][y][z].parent.childCount); 
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
                                    // PoolManager.Instance.ReturnToPool(grid[x][y][z].gameObject, GetSpecificPrefabMinoType(grid[x][y][z].gameObject));
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
                        // PoolManager.Instance.ReturnToPool(grid[x][y][z].gameObject, GetSpecificPrefabMinoType(grid[x][y][z].gameObject));
                        grid[x][y][z] = null;
                        gridOcc[x][y][z] = 0; // 其它 mode 好像也不需要这个东西
                    }
                }
        }
        public void MoveRowDown(int y) {
            if (gameMode > 0) {
                for (int j = 0; j < gridWidth; j++)    
                    for (int x = 0; x < gridWidth; x++) {
                        if (grid[x][y][j] != null) {
                            grid[x][y-1][j] = grid[x][y][j];
                            grid[x][y][j] = null;
                            grid[x][y-1][j].position += new Vector3(0, -1, 0);
                        }
                    }
            } else { // gameMode == 0
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
            } // gameMode == 0
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

        public void UpdateScore() {
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
                PlayLineClearedSound();
// 考虑粒子系统是否像是声频管理器一样的统一管理,只在教育模式下使用到粒子系统,但其它模式可以扩展                
                //particles = GetComponent<ParticleSystem>();
                //emissionModule = particles.emission;
                //emissionModule.enabled = true;
                //particles.Play();
            }
        }

        public void ClearedOneLine() {
            currentScore += scoreOneLine + (currentLevel + 20);
            numLinesCleared += 1;
        }
        public void ClearedTwoLine() {
            currentScore += scoreTwoLine + (currentLevel + 25);
            numLinesCleared += 2;
        }
        public void ClearedThreeLine() {
            currentScore += scoreThreeLine + (currentLevel + 30);
            numLinesCleared += 3;
        }
        public void ClearedFourLine() {
            currentScore += scoreFourLine + (currentLevel + 40);
            numLinesCleared += 4;
        }

        public void PlayLineClearedSound() {
            AudioManager.Instance.PlayLineClearedSound();
        }

        public void UpdateHighScore() {
            if (currentScore > startingHighScore) {
                PlayerPrefs.SetInt("highscore3", startingHighScore2);
                PlayerPrefs.SetInt("highscore2", startingHighScore);
                PlayerPrefs.SetInt("highscore", currentScore);
            } else if (currentScore > startingHighScore2) {
                PlayerPrefs.SetInt("highscore3", startingHighScore2);
                PlayerPrefs.SetInt("highscore2", currentScore);
            } else if (currentScore > startingHighScore3) 
                PlayerPrefs.SetInt("highscore3", currentScore);
        }
    }
}

