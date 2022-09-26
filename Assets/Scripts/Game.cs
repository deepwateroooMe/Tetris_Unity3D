using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System;

namespace tetris3d {
    public class Game : MonoBehaviour {
        private static string TAG = "Game";
    
        //public int gridWidth; // = 3;
        public static int gridWidth;
        public static int gridHeight = 12; 
        // public static Transform[,,] grid = new Transform[gridWidth, gridHeight, gridWidth];
        // public static int [,,] gridOcc = new int[gridWidth, gridHeight, gridWidth];
        public static Transform[,,] grid; //= new Transform[gridWidth, gridHeight, gridWidth];
        public static int [,,] gridOcc; //= new int[gridWidth, gridHeight, gridWidth];
        public GameObject baseBoard3;
        public GameObject baseBoard4;
        public GameObject baseBoard5;
        
        public static int currentScore = 0;
        public int scoreOneLine = 40;
        public int scoreTwoLine = 100;
        public int scoreThreeLine = 300;
        public int scoreFourLine = 1200;
    
        public AudioClip clearLineSound;
        public int currentLevel = 0;  
        public static float fallSpeed = 3.0f;
        public static bool startingAtLevelZero;
        public static int startingLevel;
        public static bool isPaused = false;

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
    
        private int numberOfRowsThisTurn = 0;
        private AudioSource audioSource;
        private GameObject cycledPreviewTetromino;
    
        private bool gameStarted = false;

        private Vector3 previewTetrominoPosition = new Vector3(-17f, -5f, -9.81f); 
        private Vector3 previewTetromino2Position = new Vector3(-68.3f, 19.6f, 32.4f); // (-56.3f, -0.1f, 32.4f) (-24.8f, -0.1f, 32.4f);
    
        public int numLinesCleared = 0;

        private int startingHighScore;
        private int startingHighScore2;
        private int startingHighScore3;
    
        private ParticleSystem m_ExplosionParticles;
        private AudioSource m_ExplosionAudio;          

        private bool isMovement = true;
        private int randomTetromino;
    
        private Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f);

        public GameObject emptyGO;
        public Transform tmpTransform;
    
        public int gameMode = 0; 

        public GameObject moveCanvas;
        public GameObject rotateCanvas;

        public GameObject previewTetromino;
        public GameObject previewTetromino2;

        public string prevPreview; // to remember previous spawned choices
        public string prevPreview2;
        public string nextTetrominoType;  
        public string previewTetrominoType; 
        public string previewTetromino2Type;

        private SaveGameEventInfo saveGameInfo;
        private bool hasDeletedMinos = false;

        private GameObject tmpParentGO;

        public GameObject previewSelectionButton;
        public GameObject previewSelectionButton2;
        public GameObject swapPreviewTetrominoButton;
        public GameObject undoButton;
        
        public GameObject pausePanel;
        public GameObject savedGamePanel;
        public GameObject saveGameReminderPanel;
        private bool hasSavedGameAlready = false;
        private bool loadSavedGame = false;

        public static GameObject nextTetromino;
        public static GameObject ghostTetromino; 
        private bool isDuringUndo = false;
        public bool saveForUndo = true;

        private GameObject baseBoard;
        
        // enable disable these buttons work slight better than this, could modify this part later
        private int [] buttonInteractableList = new int[7]{ 1, 1, 1, 1, 1, 1, 1};
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

#region pausePanel Button Handlers
        public void onResumeGame() {
            Time.timeScale = 1.0f;
            isPaused = false;
            pausePanel.SetActive(false);
            audioSource.Play();
        }
        
        void SaveGame(SaveGameEventInfo info) {
            Debug.Log(TAG + ": SaveGame()");
            saveForUndo = false;
            onGameSave();
            hasSavedGameAlready = true;
            savedGamePanel.SetActive(true);
        }
        public void onSavedGamePanelOK() {
            savedGamePanel.SetActive(false);
        }
        public void onBackToMainMenu() {
            if (!hasSavedGameAlready && gameStarted) { // gameStarted
                saveGameReminderPanel.SetActive(true);
            } else {
                cleanUpGameBroad();
                isPaused = false;
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("GameMenu");
            }
        }
        public void onYesToSaveGame() {
            saveForUndo = false;
            onGameSave();
            hasSavedGameAlready = true;
            saveGameReminderPanel.SetActive(false);
            pausePanel.SetActive(false);
            cleanUpGameBroad();
            isPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("GameMenu");
        }
        public void onNoToNotSaveGame() { // 如何才能够延迟加载呢？
            hasSavedGameAlready = false;
            saveGameReminderPanel.SetActive(false);
            pausePanel.SetActive(false);
            cleanUpGameBroad();
            isPaused = false;
            Time.timeScale = 1.0f;
            if (gameMode == 1)
                gameStarted = false;
            // still have to check this due to auto Save
            string path = new StringBuilder(Application.persistentDataPath).Append("/").Append(GameMenuData.Instance.saveGamePathFolderName).Append("/game.save").ToString();
            if (File.Exists(path)) {
                try {
                    File.Delete(path);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }
            }
            SceneManager.LoadScene("GameMenu");
        }
#endregion
        IEnumerator asyncLoadScene() {
            AsyncOperation async = SceneManager.LoadSceneAsync("GameMenu");
            yield return async;
        }
        // void onGameSave(SaveGameEventInfo info) {
        void onGameSave() {
            Debug.Log(TAG + ": onGameSave()");
            if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
                tmpTransform = new GameObject().transform;
            SaveSystem.SaveGame(this);
        }
        public void CheckUserInput() {  // originally pasuseButton & continueButton
            Debug.Log(TAG + ": CheckUserInput()"); 
            if (Time.timeScale == 1.0f) {
                PauseGame();
            } else {
                onResumeGame();
            }
        }

        public void PauseGame() {
            Time.timeScale = 0f;	    
            audioSource.Pause();
            isPaused = true;

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
            UpdateGrid(nextTetromino);

            // Debug.Log(TAG + ": gridOcc[,,] before Land and Save()"); 
            // MathUtil.printBoard(gridOcc); 
            
            recycleGhostTetromino();

            // // SaveGameEventInfo fire here 
            // saveGameInfo = new SaveGameEventInfo();
            // EventManager.Instance.FireEvent(saveGameInfo);
            // change an approach: it is unnessary and do NOT apply delegates and events here
            onGameSave();

            DeleteRow();
            Debug.Log(TAG + " (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>())): " + (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>()))); 
            if (CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>())) {
                GameOver();
            }            
            DisableMoveRotationCanvas();
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            if (gameMode == 0) {
                buttonInteractableList[0] = 1;
                buttonInteractableList[1] = 1;
                buttonInteractableList[2] = 1;
                buttonInteractableList[3] = 1; // undo button
            } else 
                SpawnnextTetromino();  
        }

        void cleanUpGameBroad() {
            Debug.Log(TAG + ": cleanUpGameBroad()");
            // dealing with currentActiveTetromino & ghostTetromino firrst
            if (nextTetromino != null && nextTetromino.CompareTag("currentActiveTetromino")) { // hang in the air
                Debug.Log(TAG + " (ghostTetromino != null): " + (ghostTetromino != null));  // always true
                if (ghostTetromino != null) {
                    recycleGhostTetromino();
                }
                recycleNextTetromino();
            }
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridHeight; j++) {
                    for (int k = 0; k < gridWidth; k++) {
                        if (grid[i, j, k] != null) {
                            if (grid[i, j, k].parent != null && grid[i, j, k].parent.childCount == 4) {
                                if (grid[i, j, k].parent.gameObject.CompareTag("currentActiveTetromino")) 
                                    grid[i, j, k].parent.gameObject.GetComponent<Tetromino>().enabled = false;
                                Transform tmpParentTransform = grid[i, j, k].parent;
                                foreach (Transform transform in grid[i, j, k].parent) {
                                    x = (int)Mathf.Round(transform.position.x);
                                    y = (int)Mathf.Round(transform.position.y);
                                    z = (int)Mathf.Round(transform.position.z);
                                    if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth && z >= 0 && z < gridWidth) {
                                        grid[x, y, z] = null;
                                        gridOcc[x, y, z] = 0;
                                    }
                                }
                                PoolManager.Instance.ReturnToPool(tmpParentTransform.gameObject, tmpParentTransform.gameObject.GetComponent<TetrominoType>().type);
                            } else if (grid[i, j, k].parent != null && grid[i, j, k].parent.childCount < 4) { // parent != null && childCount < 4
                                foreach (Transform transform in grid[i, j, k].parent) {
                                    string type = transform.gameObject.GetComponent<MinoType>() == null ?
                                        new StringBuilder("mino").Append(grid[i, j, k].parent.gameObject.GetComponent<TetrominoType>().type.Substring(5, 1)).ToString()
                                        : transform.gameObject.GetComponent<MinoType>().type;
                                    // grid[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z)] = null;
                                    x = (int)Mathf.Round(transform.position.x);
                                    y = (int)Mathf.Round(transform.position.y);
                                    z = (int)Mathf.Round(transform.position.z);
                                    if (y >= 0 && y < gridHeight && x >= 0 && x < gridWidth && z >= 0 && z < gridWidth) {
                                        grid[x, y, z] = null;
                                        gridOcc[x, y, z] = 0;
                                    }
                                    PoolManager.Instance.ReturnToPool(transform.gameObject, type);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void recycleGhostTetromino() {
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
        
        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            gameMode = GameMenuData.Instance.gameMode;
            fallSpeed = 3.0f; // should be recorded too, here

            if (gameMode == 0)
                resetGridOccBoard();
            SpawnnextTetromino();  

            currentScore = 0;
            hud_score.text = "0";
            currentLevel = startingLevel;
            hud_level.text = currentLevel.ToString();
            hud_lines.text = "0";

            if (gameMode > 0) { // disable some components
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
        void setAllBaseBoardInactive() {
            baseBoard3.SetActive(false);
            baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }
        void Start () {
            Debug.Log(TAG + ": Start()");
            gridWidth = GameMenuData.Instance.gridSize;
            Debug.Log(TAG + " gridWidth: " + gridWidth);
            
            // grid = new Transform[gridWidth, gridHeight, gridWidth];
            // gridOcc = new int[gridWidth, gridHeight, gridWidth];
            grid = new Transform[5, gridHeight, 5];
            gridOcc = new int[5, gridHeight, 5];

            setAllBaseBoardInactive();
            switch (gridWidth) {
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

            // temporatorily don't consider these yet
            string particleType = "particles";
            // m_ExplosionParticles = PoolManager.Instance.GetFromPool(GetSpecificPrefabType(m_ExplosionPrefab)).GetComponent<ParticleSystem>();
            m_ExplosionParticles = PoolManager.Instance.GetFromPool(particleType).GetComponent<ParticleSystem>();
            //m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
            m_ExplosionParticles.gameObject.SetActive(false);
            // 因为实例化粒子特效以后，实际上粒子的脚本就已经完成了初始化的工作，也就是Awake()和OnEnable()方法。然后设置SetActive(false)仅仅是把粒子特效隐藏起来。
        }

        private void SpawnPreviewTetromino() {
            Debug.Log(TAG + ": SpawnPreviewTetromino()"); 
            previewTetromino = PoolManager.Instance.GetFromPool(GetRandomTetromino(), previewTetrominoPosition, Quaternion.identity, previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(previewSet.transform, false);

            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;

            Debug.Log(TAG + " (previewTetromino != null): " + (previewTetromino != null)); 
            Debug.Log(TAG + " previewTetromino.name: " + previewTetromino.name); 
            
            if (gameMode == 0) { // previewTetromino2
                // excepts: undoButton toggleButton fallButton
                buttonInteractableList[3] = 0;
                buttonInteractableList[4] = 0;
                buttonInteractableList[5] = 0;

                previewTetromino2 = PoolManager.Instance.GetFromPool(GetRandomTetromino(), previewTetromino2Position, Quaternion.identity, previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                // previewTetromino2.layer = LayerMask.NameToLayer("UI"); // not working on this RayCast button click right now
            }
        }
        private void SpawnPreviewTetromino(string type1, string type2) {
            previewTetromino = PoolManager.Instance.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(previewSet.transform, false);
            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;

            if (gameMode == 0) { // previewTetromino2
                previewTetromino2 = PoolManager.Instance.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
            }
            buttonInteractableList[3] = 1; // undoButton
        }
        public void playFirstTetromino() {
            Debug.Log(TAG + ": playFirstTetromino()");
            Debug.Log(TAG + " buttonInteractableList[0]: " + buttonInteractableList[0]); 
            if (buttonInteractableList[0] == 0) return;
            prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            preparePreviewTetrominoRecycle(2);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino.transform.localScale -= previewTetrominoScale;
            // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            
            nextTetromino = previewTetromino;
            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();

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
        void printbuttonInteractableList() {
            for (int i = 0; i < 6; i++) 
                Debug.Log(TAG + " buttonInteractableList[i]: i : " + i + ", " + buttonInteractableList[i]); 
        }
        public void playSecondTetromino() {
            Debug.Log(TAG + ": playSecondTetromino()"); 
            Debug.Log(TAG + " buttonInteractableList[1]: " + buttonInteractableList[1]); 
            if (buttonInteractableList[1] == 0) return;
            prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            preparePreviewTetrominoRecycle(1);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino2.transform.localScale -= previewTetrominoScale;
            // previewTetromino2.layer = LayerMask.NameToLayer("Default");
            // previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;

            nextTetromino = previewTetromino2;
            currentActiveTetrominoPrepare();
            gameStarted = true;
            
            SpawnGhostTetromino();  
            moveRotatecanvasPrepare();
            SpawnPreviewTetromino();
            
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
        private void preparePreviewTetrominoRecycle(int i) { 
            cycledPreviewTetromino = i == 1 ? previewTetromino : previewTetromino2;
            // cycledPreviewTetromino.GetComponent<Rotate>().enabled = !cycledPreviewTetromino.GetComponent<Rotate>().enabled; // disable
            cycledPreviewTetromino.transform.localScale -= previewTetrominoScale;
            cycledPreviewTetromino.transform.position = Vector3.zero;
            cycledPreviewTetromino.transform.rotation = Quaternion.identity;
            cycledPreviewTetromino.SetActive(false);
        }
        public void SpawnnextTetromino() {
            Debug.Log(TAG + ": SpawnnextTetromino()");
            if (!gameStarted) {
                if (gameMode == 0) {
                    SpawnPreviewTetromino();
                } else {
                    gameStarted = true;
                    nextTetromino = PoolManager.Instance.GetFromPool(GetRandomTetromino(), new Vector3(2.0f, gridHeight - 1f, 2.0f), Quaternion.identity);
                    currentActiveTetrominoPrepare();
                    moveCanvas.gameObject.SetActive(true);  
                    SpawnGhostTetromino();
                    SpawnPreviewTetromino();
                }
            } else {
                previewTetromino.transform.localScale -= previewTetrominoScale;
                // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;

                nextTetromino = previewTetromino;
                currentActiveTetrominoPrepare();
                
                SpawnGhostTetromino();  
                moveRotatecanvasPrepare();
                SpawnPreviewTetromino();
            }
        }

        public void UpdateGrid(GameObject tetromino) { // update gridOcc at the same time
            Debug.Log(TAG + ": UpdateGrid()");
            for (int y = 0; y < gridHeight; y++) 
                for (int z = 0; z < gridWidth; z++) 
                    for (int x = 0; x < gridWidth; x++)
                        if (grid[x, y, z] != null && grid[x, y, z].parent == tetromino.transform) {
                            grid[x, y, z] = null; 
                            gridOcc[x, y, z]= 0; 
                        }
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (pos.y >= 0 && pos.y < gridHeight && pos.x >= 0 && pos.x < gridWidth && pos.z >= 0 && pos.z < gridWidth) { 
                    grid[(int)pos.x, (int)pos.y, (int)pos.z] = mino;
                    gridOcc[(int)pos.x, (int)pos.y, (int)pos.z] = 1;
                }
            }
            Debug.Log(TAG + " tetromino.name: " + tetromino.name);
        }
        public void resetGridAfterDisappearingNextTetromino(GameObject tetromino) {
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if ((int)pos.y >= 0 && (int)pos.y < gridHeight && (int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.z >= 0 && (int)pos.z < gridWidth) { 
                    grid[(int)pos.x, (int)pos.y, (int)pos.z] = null;
                    gridOcc[(int)pos.x, (int)pos.y, (int)pos.z] = 0;
                }
            }
        }
        private void recycleNextTetromino() {
            Debug.Log(TAG + ": recycleNextTetromino()"); 
            if (nextTetromino != null) {
                nextTetromino.tag = "Untagged";
                nextTetromino.GetComponent<Tetromino>().enabled = false;
                resetGridAfterDisappearingNextTetromino(nextTetromino);  // this one for undo click only ???? Nonono
                if (nextTetromino.transform.childCount == 4) {
                    PoolManager.Instance.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
                } else
                    Destroy(nextTetromino.gameObject);
            }
            // nextTetromino = null;
        }
        private void recycleThreeMajorTetromino() {
            // 回收三样东西：nextTetromino previewTetromino previewTetromino2
            recycleNextTetromino();
            preparePreviewTetrominoRecycle(1);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            preparePreviewTetrominoRecycle(2);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        }

        // void onUndoGame(UndoGameEventInfo info) { // on Reload Saved game
        //     Debug.Log(TAG + ": onUndoGame() void"); 
        // }
        public void onUndoGame() { 
            Debug.Log(TAG + ": onUndoGame()");
            if (buttonInteractableList[3] == 0) return;
            Array.Clear(buttonInteractableList, 0, buttonInteractableList.Length);
            isDuringUndo = true;
            recycleThreeMajorTetromino();

            StringBuilder path = new StringBuilder("");
            // if (!string.IsNullOrEmpty(GameMenuData.Instance.saveGamePathFolderName)) 
                path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game" + ".save");
            // else
            //     path.Append(Application.persistentDataPath + "/game" + ".save");
            GameData gameData = SaveSystem.LoadGame(path.ToString());
            StringBuilder type = new StringBuilder("");
            if (hasDeletedMinos) {
                currentScore = gameData.score;
                currentLevel = gameData.level;
                numLinesCleared = gameData.lines;
                hud_score.text = currentScore.ToString();
                hud_level.text = currentLevel.ToString(); // 这不希望变的
                hud_lines.text = numLinesCleared.ToString();

                // Debug.Log(TAG + ": onUndoGame() current board before respawn"); 
                // MathUtil.printBoard(gridOcc); 
                
                Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                LoadDataFromParentList(gameData.parentList);

                GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            }
            moveCanvas.gameObject.SetActive(false);   // moveCanvas rotateCanvas: SetActive(false)
            rotateCanvas.gameObject.SetActive(false);
            if (gameData.prevPreview != null) { // previewTetromino previewTetromino2
                type.Clear();
                string type2 = gameData.prevPreview2;
                SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2);
            }
            buttonInteractableList[0] = 1; 
            buttonInteractableList[1] = 1; 
            buttonInteractableList[2] = 1; 
            buttonInteractableList[3] = 0; // buttons are supposed to click once at a time only
            isDuringUndo = false;
        }
        
        private void LoadDataFromParentList(List<TetrominoData> parentList) {
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0;
            StringBuilder type = new StringBuilder("");
            foreach (TetrominoData parentData in parentList) {
                // Debug.Log(TAG + " parentData.name: " + parentData.name);
                // Debug.Log(TAG + " parentData.children.Count: " + parentData.children.Count);
                if (isThereAnyExistChild(parentData)) { // 存在
                    if (!gridMatchesSavedParent(tmpParentGO, (List<MinoData>)(parentData.children.collection))) {  // 先删除多余的，再补全缺失的
                        foreach (Transform trans in tmpParentGO.transform) { // 先 删除多余的
                            // MathUtil.print(MathUtil.Round(trans.position));
                            // Debug.Log(TAG + " (!myContains(trans, (List<MinoData>)(parentData.children.collection))): " + (!myContains(trans, (List<MinoData>)(parentData.children.collection)))); 
                            if (!myContains(trans, (List<MinoData>)(parentData.children.collection))) {
                                x = (int)Mathf.Round(trans.position.x);
                                y = (int)Mathf.Round(trans.position.y);
                                z = (int)Mathf.Round(trans.position.z);
                                // MathUtil.print(x, y, z); // this one is right
                                grid[x, y, z].parent = null;
                                Destroy(grid[x, y, z].gameObject);
                                gridOcc[x, y, z] = 0;
                                grid[x, y, z] = null;
                            }
                        }
                        // Debug.Log(TAG + " tmpParentGO.transform.childCount (deleted unwanted): " + tmpParentGO.transform.childCount);
                        foreach (MinoData minoData in parentData.children) {
                            Vector3 posA = MathUtil.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
                            MathUtil.print(posA);
                            x = (int)Mathf.Round(posA.x);
                            y = (int)Mathf.Round(posA.y);
                            z = (int)Mathf.Round(posA.z);
                            if (grid[x, y, z] == null) {
                                // MathUtil.print(x, y, z);
                                type.Clear();
                                GameObject tmpMinoGO = PoolManager.Instance.GetFromPool(type.Append(minoData.type).ToString(), DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                                        DeserializedTransform.getDeserializedTransRot(minoData.transform));
                                grid[x, y, z] = tmpMinoGO.transform;
                                grid[x, y, z].parent = tmpParentGO.transform;
                                gridOcc[x, y, z] = 1;
                            }
                        }
                    }
                    // Debug.Log(TAG + " tmpParentGO.transform.childCount (filled needed -- final): " + tmpParentGO.transform.childCount);
                } else { // 重新生成                                           // 空 shapeX Tetromino_X : Universal
                    GameObject tmpGameObject = PoolManager.Instance.GetFromPool("shapeX", DeserializedTransform.getDeserializedTransPos(parentData.transform), 
                                                                                DeserializedTransform.getDeserializedTransRot(parentData.transform));
                    foreach (MinoData minoData in parentData.children) {
                        GameObject tmpMinoGO = PoolManager.Instance.GetFromPool(minoData.type, DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                                DeserializedTransform.getDeserializedTransRot(minoData.transform));
                        tmpMinoGO.transform.parent = tmpGameObject.transform;
                        x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
                        y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
                        z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
                        // fix for bug 5: fill in a Mino into board in y where there are more minos are above the filled in one
                        if (isColumnFromHereEmpty(x, y, z)) {
                            grid[x, y, z] = tmpMinoGO.transform;
                            gridOcc[x, y, z] = 1;
                        } else {
                            FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform); // update grid accordingly
                        }
                    }
                    tmpGameObject.GetComponent<TetrominoType>().type = parentData.type;
                    tmpGameObject.name = parentData.name;
                    // Debug.Log(TAG + " tmpGameObject.GetComponent<TetrominoType>().type: " + tmpGameObject.GetComponent<TetrominoType>().type); 
                    // Debug.Log(TAG + " tmpGameObject.transform.childCount: " + tmpGameObject.transform.childCount); 
                }
                // Debug.Log(TAG + ": gridOcc[,,] after each deleted mino re-spawn"); 
                // MathUtil.printBoard(gridOcc); 
            }
        }
        void LoadGame(string path) {  // when load Scene load game: according to gameMode
            Debug.Log(TAG + ": LoadGame()");
            if (gameMode == 0)
                resetGridOccBoard(); 
            GameData gameData = SaveSystem.LoadGame(path);
            gameMode = gameData.gameMode;
            
            currentScore = gameData.score;
            currentLevel = gameData.level;
            numLinesCleared = gameData.lines;
            hud_score.text = currentScore.ToString();
            hud_level.text = currentLevel.ToString();
            hud_lines.text = numLinesCleared.ToString();
            
            // hud_canvas.enabled = true; // 这个是需要根据不同的mode 来进行处理的
            if (gameMode > 0) { // disable some components
                previewSelectionButton.SetActive(false);
                previewSelectionButton2.SetActive(false);
                swapPreviewTetrominoButton.SetActive(false);
                undoButton.SetActive(false);
            }

            Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            LoadDataFromParentList(gameData.parentList);

            // currentActiveTetromino: if it has NOT landed yet
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                nextTetromino = PoolManager.Instance.GetFromPool(type.Append(gameData.nextTetrominoData.type).ToString(),
                                                                 DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                                                                 DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform));
                nextTetromino.tag = "currentActiveTetromino";
                // if (defaultContainer == null) // 我不要再管这个东西了
                //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // nextTetromino.transform.SetParent(defaultContainer.transform, false);
                nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
                nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;

                moveCanvas.gameObject.SetActive(true);
                moveCanvas.transform.position = new Vector3(moveCanvas.transform.position.x, nextTetromino.transform.position.y, moveCanvas.transform.position.z);
                // 也需要重新设置 rotateCanvas 的位置
                SpawnGhostTetromino();
            }

            // previewTetromino previewTetromino2
            type.Clear();
            string type2 = gameData.previewTetromino2Type;
            SpawnPreviewTetromino(type.Append(gameData.previewTetrominoType).ToString(), type2);
            if (gameData.prevPreview != null) {
                prevPreview = gameData.prevPreview;
                prevPreview2 = gameData.prevPreview2;
            } 
            // MainCamera rotation
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData);
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            
            if (nextTetromino != null && nextTetromino.CompareTag("currentActiveTetromino")) // Performance Bug: CompareTag()
                gameStarted = true;
            GameMenuData.Instance.loadSavedGame = false;
            loadSavedGame = false;
        }    
        private void currentActiveTetrominoPrepare() {
            Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            nextTetromino.tag = "currentActiveTetromino";
            nextTetromino.transform.rotation = Quaternion.identity;

            if (gameMode == 0 && (gridWidth == 3 || gridWidth == 4)) {
                nextTetromino.transform.localPosition = new Vector3(1.0f, gridHeight - 1f, 1.0f);
            } else 
                nextTetromino.transform.localPosition = new Vector3(2.0f, gridHeight - 1f, 2.0f);
            
            // Debug.Log(TAG + " (defaultContainer == null) before: " + (defaultContainer == null)); 
            // if (defaultContainer == null)
            //     defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
            // Debug.Log(TAG + " (defaultContainer == null) after: " + (defaultContainer == null)); 
            // nextTetromino.transform.SetParent(defaultContainer.transform, false);
            
            nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
            nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;
            Debug.Log(TAG + " nextTetromino.name: " + nextTetromino.name);
        }
        
        private bool isThereAnyExistChild(TetrominoData parentData) {
            // Debug.Log(TAG + ": isThereAnyExistChild()"); 
            Vector3 pos = Vector3.zero;
            int x = 0, y = 0, z = 0;
            foreach (MinoData mino in parentData.children) {
                pos = DeserializedTransform.getDeserializedTransPos(mino.transform);
                x = (int)Mathf.Round(pos.x);
                y = (int)Mathf.Round(pos.y);
                z = (int)Mathf.Round(pos.z);
                MathUtil.print(x, y, z);
                if (grid[x, y, z] != null && grid[x, y, z].parent != null) { // // make sure parent matches first !
                    if (grid[x, y, z].parent.gameObject.name == parentData.name &&
                        MathUtil.Round(grid[x, y, z].parent.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(parentData.transform)) && 
                        MathUtil.Round(grid[x, y, z].parent.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(parentData.transform))) { 
                        tmpParentGO = grid[x, y, z].parent.gameObject; 
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void FillInMinoAtTargetPosition(int x, int y, int z, Transform minoTrans) {
            int o = gridHeight - 1;
            // grid[x, o, z] = 0, otherwose gameover
            while (grid[x, o-1, z] == null) o--;
            for (int i = o; i > y; i--) { // y // if (grid[x, i - 1, z] != null) {
                grid[x, i, z] = grid[x, i-1, z];
                gridOcc[x, i, z] = gridOcc[x, i-1, z];
                if (grid[x, i-1, z] != null) {
                    grid[x, i-1, z] = null;
                    gridOcc[x, i-1, z] = 0;
                    grid[x, i, z].position += new Vector3(0, 1, 0);
                }
            }
            grid[x, y, z] = minoTrans;
            gridOcc[x, y, z] = 1;
        }
        private bool isColumnFromHereEmpty(int x, int y, int z) {
            bool isColumnAboveEmpty = true;
            for (int i = y; i < gridHeight; i++) {
                if (grid[x, y, z] != null)
                    isColumnAboveEmpty = false;
            }
            return isColumnAboveEmpty;
        }
        private bool gridMatchesSavedParent(GameObject tmpGO, List<MinoData> data) {
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
        private bool myContains(Transform tmp, List<MinoData> children) {
            foreach (MinoData data in children)
                if (MathUtil.Round(tmp.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(data.transform)) &&
                    MathUtil.Round(tmp.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(data.transform)))
                    return true;
            return false;
        }
        bool isTheParentChildren(Transform tmpParent, TetrominoData parent) { // compare against SerializedTransform parent
            return (tmpParent.gameObject.name == parent.name &&
                tmpParent.position == DeserializedTransform.getDeserializedTransPos(parent.transform) &&
                    tmpParent.rotation == DeserializedTransform.getDeserializedTransRot(parent.transform));
        }
        bool existInChildren(Transform transform, MinoDataCollection<TetrominoData, MinoData> children) {
            foreach (MinoData data in children) {
                if (transform.position == DeserializedTransform.getDeserializedTransPos(data.transform) &&
                    transform.rotation == DeserializedTransform.getDeserializedTransRot(data.transform)) {
                    return true;
                }
            }
            return false;
        }

        public void onActiveTetrominoMove(TetrominoMoveEventInfo info) { 
            Debug.Log(TAG + ": onTetrominoMove()");
            if (nextTetromino.GetComponent<Tetromino>().IsMoveValid) {
                moveCanvas.transform.position += info.delta;
                if ((int)info.delta.y != 0) {
                    rotateCanvas.transform.position += new Vector3(0, info.delta.y, 0);
                }
                UpdateGrid(nextTetromino);
            }
        }
        public void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            // Debug.Log(TAG + ": onActiveTetrominoRotate()"); 
            if (nextTetromino.GetComponent<Tetromino>().IsRotateValid) {
                UpdateGrid(nextTetromino); 
            }
        }
        
        public void onSwapPreviewTetrominos () {
            Debug.Log(TAG + ": swapPreviewTetrominosFunc()");
            if (buttonInteractableList[2] == 0) return;
            preparePreviewTetrominoRecycle(1); // recycle 1st tetromino first
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            preparePreviewTetrominoRecycle(2); // recycle 2st tetromino then
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            SpawnPreviewTetromino();
        }
        
        // public void onSwapPreviewTetrominos(SwapPreviewsEventInfo swapInfo) {
        //     // Debug.Log(TAG + ": swapPreviewTetrominos()");
        //     if (buttonInteractableList[2] == 0) return;
        //     // Debug.Log(TAG + " swapInfo.tag.ToString(): " + swapInfo.tag.ToString()); 
        //     preparePreviewTetrominoRecycle(1); // recycle 1st tetromino first
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     preparePreviewTetrominoRecycle(2); // recycle 2st tetromino then
        //     PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        //     SpawnPreviewTetromino();
        // }
    
        private void moveRotatecanvasPrepare() {
            // Debug.Log(TAG + ": moveRotatecanvasPrepare()"); 
            moveCanvas.transform.localPosition = new Vector3(2.1f, gridHeight - 1f, 2.1f);     
            rotateCanvas.transform.localPosition = new Vector3(2.1f, gridHeight - 1f, 2.1f);
            isMovement = false;
            toggleButtons(1);
        }

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
        // public void toggleButtons() {
        //     Debug.Log(TAG + ": toggleButtons()");
        //     Debug.Log(TAG + " buttonInteractableList[4]: " + buttonInteractableList[4]); 
        //     if (buttonInteractableList[4] == 0) return;
        //     if (isMovement) { 
        //         isMovement = false;
        //         invertButton.image.overrideSprite = prevImg; // rotation img 这两个图像还需要处理一下
        //         moveCanvas.gameObject.SetActive(false);
        //         rotateCanvas.SetActive(true); 
        //     } else {
        //         isMovement = true;
        //         invertButton.image.overrideSprite = newImg;
        //         moveCanvas.gameObject.SetActive(true);
        //         rotateCanvas.SetActive(false);
        //     }
        // }

        public void toggleButtons(int indicator) {
            Debug.Log(TAG + ": toggleButtons()");
            Debug.Log(TAG + " buttonInteractableList[4]: " + buttonInteractableList[4]);
            Debug.Log(TAG + " indicator: " + indicator); 
            if (gameMode == 0 && buttonInteractableList[4] == 0 && indicator == 0) return;
            if (gameMode > 0 || indicator == 1 || buttonInteractableList[4] == 1) {
                if (isMovement) { 
                    isMovement = false;
                    invertButton.image.overrideSprite = prevImg; // rotation img 这两个图像还需要处理一下
                    moveCanvas.gameObject.SetActive(false);
                    rotateCanvas.SetActive(true); 
                } else {
                    isMovement = true;
                    invertButton.image.overrideSprite = newImg;
                    moveCanvas.gameObject.SetActive(true);
                    rotateCanvas.SetActive(false);
                }
            }
        }

        void resetGridOccBoard() {
            for (int y = 0; y < gridHeight; y++) {
                for (int x = 0; x < gridWidth; x++) {
                    for (int z = 0; z < gridWidth; z++) {
                        gridOcc[x, y, z] = 0;
                    }
                }
            }
        }

        void Update() {
            UpdateScore();
            UpdateUI();
            UpdateLevel();
            UpdateSpeed();
            //CheckUserInput();  // this is a bug need to be fixed, the screen is flashing
        }

        void UpdateLevel() {
            if (startingAtLevelZero || (!startingAtLevelZero && numLinesCleared / 10 > startingLevel)) 
                currentLevel = numLinesCleared / 10;
        }

        public void UpdateSpeed() { 
            fallSpeed = 1.0f - (float)currentLevel * 0.1f;
        }
    
        void UpdateUI() {
            // Debug.Log(TAG + ": UpdateUI()");
            // Debug.Log(TAG + " currentScore: " + currentScore);
            // Debug.Log(TAG + " (hud_score != null): " + (hud_score != null)); 
            hud_score.text = currentScore.ToString();
            hud_level.text = currentLevel.ToString();
            hud_lines.text = numLinesCleared.ToString();
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
                    if (grid[x, y, j] == null ||      // modified here for ghostTetromino
                        (grid[x, y, j].parent == ghostTetromino.transform
                         && grid[x, y, j].parent != nextTetromino.transform)) 
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
                        
                        m_ExplosionParticles.transform.position = new Vector3(2.5f, y, 2.5f); 
                        m_ExplosionParticles.gameObject.SetActive(true);
                        m_ExplosionParticles.Play();
                        m_ExplosionAudio.Play();

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
                            if (gridOcc[x, o, z] == 2) {
                                if (o == gridHeight - 1 ||  gridOcc[x, o+1, z] == 0) // this statement ???
                                    gridOcc[x, o, z] = 0;
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
                    tmpSum += gridOcc[x, y, z] == 0 ? 0 : 1; 
                }
                if (tmpSum == gridWidth) {
                    numberOfRowsThisTurn++;
                    result = true;
                    for (int z = 0; z < gridWidth; z++) {
                        gridOcc[x, y, z] += gridOcc[x, y, z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5列
            for (int z = 0; z < gridWidth; z++) { // 数5行
                tmpSum = 0;
                for (int x = 0; x < gridWidth; x++) {
                    tmpSum += gridOcc[x, y, z] == 2 ? 1 : gridOcc[x, y, z]; // 2
                }
                if (tmpSum == gridWidth) {
                    numberOfRowsThisTurn++;
                    result = true;
                    for (int x = 0; x < gridWidth; x++) {
                        gridOcc[x, y, z] += gridOcc[x, y, z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5行
            tmpSum = 0;
            for (int x = 0; x < gridWidth; x++) { // 数2对角线
                for (int z = 0; z < gridWidth; z++) {
                    if (x == z) {
                        tmpSum += gridOcc[x, y, z] == 2 ? 1 : gridOcc[x, y, z]; // 2
                    }
                }
            }
            if (tmpSum == gridWidth) {
                result = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridWidth; x++) { // 数2对角线
                    for (int o = 0; o < gridWidth; o++) {
                        if (x == o) {
                            gridOcc[x, y, o] += gridOcc[x, y, o] == 1 ? 1 : 0;  
                        }
                    }
                }
            } // 数完一条对角线
            tmpSum = 0; 
            for (int x = 0; x < gridWidth; x++) {
                for (int z = 0; z < gridWidth; z++) {
                    if (z == gridWidth - 1 - x) {
                        tmpSum += gridOcc[x, y, z] == 2 ? 1 : gridOcc[x, y, z]; // 2
                    }
                }
            }
            if (tmpSum == gridWidth) {
                result = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridWidth; x++) {
                    for (int o = 0; o < gridWidth; o++) {
                        if (o == gridWidth - 1 - x) {
                            gridOcc[x, y, o] += gridOcc[x, y, o] == 1 ? 1 : 0; 
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
                        if (gridOcc != null && gridOcc[x, y, z] == 2) {
                            Debug.Log("(x,y,z): [" + x + "," + y + "," + z +"]: " + gridOcc[x, y, z]); 
                            if (grid[x,y,z] != null && grid[x,y,z].gameObject != null) {
                                // Debug.Log(TAG + " (grid[x, y, z].parent != null): " + (grid[x, y, z].parent != null)); 
                                if (grid[x, y, z].parent != null) {
                                    Debug.Log(TAG + " grid[x, y, z].parent.name: " + grid[x, y, z].parent.name);
                                    Debug.Log(TAG + " grid[x, y, z].parent.childCount: " + grid[x, y, z].parent.childCount); 
                                    if (grid[x, y, z].parent.childCount == 1) {
                                        Transform tmp = grid[x, y, z].parent;
                                        tmp.GetChild(0).parent = null;
                                        GameObject.Destroy(grid[x, y, z].gameObject);
                                        Destroy(tmp.gameObject);
                                    } else {
                                        grid[x, y, z].parent = null;
                                        Destroy(grid[x, y, z].gameObject);
                                    }
                                    grid[x, y, z] = null;
                                    // gridOcc[x, y, z] = 0;
                                } else {
                                    // PoolManager.Instance.ReturnToPool(grid[x, y, z].gameObject, GetSpecificPrefabMinoType(grid[x, y, z].gameObject));
                                    Destroy(grid[x, y, z].gameObject);
                                    // gridOcc[x, y, z] = 0; // 暂时还不更新，等要删除的时候才更新
                                    grid[x, y, z] = null; // x ==> z
                                }
                            }
                        } else if (grid[x, y, z] == null && gridOcc[x, y, z] == 2) { // grid[x, y, z] = null
                            // gridOcc[x, y, z] = 0;
                        }
                    } else {
                        Destroy(grid[x, y, z].gameObject);
                        // PoolManager.Instance.ReturnToPool(grid[x, y, z].gameObject, GetSpecificPrefabMinoType(grid[x, y, z].gameObject));
                        grid[x, y, z] = null;
                        gridOcc[x, y, z] = 0; // 其它 mode 好像也不需要这个东西
                    }
                }
        }
        public void MoveRowDown(int y) {
            if (gameMode > 0) {
                for (int j = 0; j < gridWidth; j++)    
                    for (int x = 0; x < gridWidth; x++) {
                        if (grid[x, y, j] != null) {
                            grid[x, y - 1, j] = grid[x, y, j];
                            grid[x, y, j] = null;
                            grid[x, y - 1, j].position += new Vector3(0, -1, 0);
                        }
                    }
            } else { // gameMode == 0
                for (int x = 0; x < gridWidth; x++) {
                    for (int z = 0; z < gridWidth; z++) {
                        if (gridOcc[x, y-1, z] == 2) { // 下面是消毁掉了的，压下去
                            gridOcc[x, y-1, z] = gridOcc[x, y, z];

                            if (grid[x, y, z] != null) {
                                grid[x, y - 1, z] = grid[x, y, z];
                                grid[x, y, z] = null;
                                grid[x, y - 1, z].position += new Vector3(0, -1, 0);
                            }
                            gridOcc[x, y, z] = y == gridHeight - 1 ? 0 : 2;
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
                return grid[(int)pos.x, (int)pos.y, (int)pos.z];
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
            audioSource.PlayOneShot(clearLineSound);
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
    
        public void GameOver() {
            Debug.Log(TAG + ": GameOver()"); 
            UpdateHighScore();
            SceneManager.LoadScene("GameOver");
        }

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
