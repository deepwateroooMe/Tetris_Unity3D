using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Collections;
using System;
using System.ComponentModel;

namespace tetris3d {
    public class GameController : MonoBehaviour { 
        private static string TAG = "GameController";

        public GameObject defaultContainer;
        
        //public GameObject m_ExplosionPrefab;

        public static float fallSpeed = 2.0f;
        public static int currentLevel = 0;  

        public static GameObject nextTetromino;
        public static GameObject ghostTetromino; 
        public static int gameMode = 0; 
        public static bool startingAtLevelZero;
        public static int startingLevel;
        public static bool isPaused = false;

        public GameObject previewSet;
        public GameObject previewSet2;
        public GameObject previewTetromino;
        public GameObject previewTetromino2;

        public GameObject emptyGO;
        public Transform tmpTransform;
        public TetrominoSpawnedEventInfo spawnedTetrominoInfo;
        
        public static bool gameStarted = false;
        public static bool isChallengeMode = false;
        public static Vector3 nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1, 2.0f);
        public bool saveForUndo = true;

        private bool hasSavedGameAlready = false;
        private bool loadSavedGame = false;
      
        private Vector3 previewTetrominoPosition = new Vector3(-17f, -5f, -9.81f); 
        private Vector3 previewTetromino2Position = new Vector3(-68.3f, 19.6f, 32.4f); // (-56.3f, -0.1f, 32.4f) (-24.8f, -0.1f, 32.4f);
        private Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f);
        private StringBuilder type = new StringBuilder("");
        private GameObject cycledPreviewTetromino;
        
        public string nextTetrominoType;    // TODO: maintain one copy in GameData only in persistent Scene ?
        public string prevPreview;         
        public string prevPreview2;
        public string previewTetrominoType; 
        public string previewTetromino2Type;
        public int prevPreviewColor;
        public int prevPreviewColor2;
        public int previewTetrominoColor; 
        public int previewTetromino2Color;

        public GameObject pausePanel; // todo
        public GameObject savedGamePanel;
        public GameObject saveGameReminderPanel;

        private ParticleSystem m_ExplosionParticles;

        public ModelMono modelMono;
        public MainScene_GUIManager guiManager; // baseBoard s 3 4 5 UpdateLevel()

        public delegate void TetrominoChallengeLandingDelegate();
        public static TetrominoChallengeLandingDelegate changeBaseCubesSkin;
        private GameObject initCubes;

        private static float levelGoalDisplayTime = 1.2f;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(levelGoalDisplayTime);
        
        private int _tetrominoCnter = 0;
        private int _undoCnter = 5;
        private int _swapCnter = 5;
        public int tetrominoCnter {
            get {
                return _tetrominoCnter;
            }
            set {
                if (_tetrominoCnter != value) {
                    _tetrominoCnter = value;
                    PropertyChangedEventInfo propertyChangedInfo = new PropertyChangedEventInfo();
                    propertyChangedInfo.propertyName = "tetrominoCnter";
                    EventManager.Instance.FireEvent(propertyChangedInfo);
                }
            }
        }
        public int undoCnter {
            get {
                return _undoCnter;
            }
            set {
                if (_undoCnter != value) {
                    _undoCnter = value;
                    PropertyChangedEventInfo propertyChangedInfo = new PropertyChangedEventInfo();
                    propertyChangedInfo.propertyName = "undoCnter";
                    EventManager.Instance.FireEvent(propertyChangedInfo);
                }
            }
        }
        public int swapCnter {
            get {
                return _swapCnter;
            }
            set {
                if (_swapCnter != value) {
                    _swapCnter = value;
                    PropertyChangedEventInfo propertyChangedInfo = new PropertyChangedEventInfo();
                    propertyChangedInfo.propertyName = "swapCnter";
                    EventManager.Instance.FireEvent(propertyChangedInfo);
                }
            }
        }

        public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + ": onActiveTetrominoLand()");
            Model.UpdateGrid(nextTetromino);
            
            Debug.Log(TAG + ": gridOcc[,,] aft Land UpdateGrid(), bef onGameSave()"); 
            MathUtil.printBoard(Model.gridOcc); 
            Debug.Log(TAG + ": gridClr[,,] aft Land UpdateGrid(), bef onGameSave()"); 
            MathUtil.printBoard(Model.gridClr); 
            
            if (isChallengeMode) {
                if (ChallengeRules.isValidLandingPosition()) {
                    changeBaseCubesSkin();
                } else { // print color board
                    Debug.Log(TAG + ": color board before game Over()");
                    MathUtil.printBoard(Model.gridClr);

                    // Debug.Log(TAG + ": Game Over()"); 
                    GameOver();
                }
            }
            
            onGameSave();
            
            if (gameMode > 0 || (isChallengeMode && (GloData.Instance.challengeLevel < 3 || GloData.Instance.challengeLevel > 5))) // 1 2 6 7 8 9 10
                modelMono.DeleteRow();
            else if (((gameMode == 0 && !isChallengeMode) || (isChallengeMode && GloData.Instance.challengeLevel > 2 && GloData.Instance.challengeLevel < 6)) // 3 4 5
                     && !ModelMono.isDeleteRowCoroutineRunning)
                StartCoroutine(modelMono.DeleteRowCoroutine()); // the case

            if (Model.CheckIsAboveGrid(nextTetromino.GetComponent<Tetromino>()) || tetrominoCnter == 0) {
                GameOver();
            }
            
            if (gameMode > 0)
                SpawnnextTetromino();  
        }

        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            EventManager.Instance.RegisterListener<PauseGameEventInfo>(onPauseGame); 
            EventManager.Instance.RegisterListener<SaveGameEventInfo>(onSaveGame); 
            EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
        }
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<PauseGameEventInfo>(onPauseGame); 
                EventManager.Instance.UnregisterListener<SaveGameEventInfo>(onSaveGame); 
                EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
                EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            }
        }

        private void LoadNewGame() {
            Debug.Log(TAG + ": LoadNewGame()");
            gameMode = GloData.Instance.gameMode;
            fallSpeed = 3.0f; // should be recorded too, here

            Debug.Log(TAG + " (gameMode == 0): " + (gameMode == 0)); 
            if (gameMode == 0 && !isChallengeMode)
                Model.resetGridOccBoard();
            else if (isChallengeMode)
                loadInitCubesforChallengeMode();

            gameStarted = false;
            SpawnnextTetromino();  

            MainScene_ScoreManager.currentScore = 0;
            currentLevel = startingLevel;
            // guiManager.UpdateLevel();
        }
        void loadInitCubesforChallengeMode() {
            initCubes = GameObject.FindGameObjectWithTag("InitCubes");
            Debug.Log(TAG + ": cubes positions()");
            if (initCubes != null) {
                Debug.Log(TAG + " initCubes.transform.childCount: " + initCubes.transform.childCount);
                
                Model.UpdateGrid(initCubes);

                Debug.Log(TAG + ": gridOcc()"); 
                MathUtil.printBoard(Model.gridOcc);
                Debug.Log(TAG + ": gridClr()");
                MathUtil.printBoard(Model.gridClr);
            }
        }
        IEnumerator displayChallengeGoal() {
            // guiManager.InitLevelGoal();
            yield return _waitForSeconds;
            guiManager.goalPanel.SetActive(false);
        }
        void Start () {
            Debug.Log(TAG + ": Start()");
            guiManager = FindObjectOfType<MainScene_GUIManager>(); // do NOT want this here
            StartCoroutine(displayChallengeGoal());

            if (GloData.Instance.isChallengeMode) {
                isChallengeMode = true;
                Model.gridXWidth = GloData.Instance.gridXSize;
                Model.gridZWidth = GloData.Instance.gridZSize;
                Model.baseCubes = new int[Model.gridXWidth * Model.gridZWidth];
                Model.prevSkin = new int[4];
                Model.prevIdx = new int[4];
                tetrominoCnter = GloData.Instance.tetrominoCnter;
                Model.grid = new Transform[Model.gridXWidth, Model.gridHeight, Model.gridZWidth];
                Model.gridOcc = new int[Model.gridXWidth, Model.gridHeight, Model.gridZWidth];
                Model.gridClr = new int[Model.gridXWidth, Model.gridHeight, Model.gridZWidth];
                MathUtil.resetColorBoard();
                
                loadInitCubesforChallengeMode();
            } else {
                Model.gridWidth = GloData.Instance.gridSize;
                Model.gridXWidth = GloData.Instance.gridSize;
                Model.gridZWidth = GloData.Instance.gridSize;
                
                Model.grid = new Transform[5, Model.gridHeight, 5];
                Model.gridOcc = new int[5, Model.gridHeight, 5];

                Model.gridClr = new int[5, Model.gridHeight, 5];
                MathUtil.resetColorBoard(); 
            }
            
            if (gameMode == 0 && !isChallengeMode) {
                guiManager.setAllBaseBoardInactive();
                switch (Model.gridWidth) {
                    case 3:
                        guiManager.baseBoard3.SetActive(true);
                        MainScene_GUIManager.baseBoard = guiManager.baseBoard3;
                        break;
                    case 4:
                        guiManager.baseBoard4.SetActive(true);
                        MainScene_GUIManager.baseBoard = guiManager.baseBoard4;
                        break;
                    case 5:
                        guiManager.baseBoard5.SetActive(true);
                        MainScene_GUIManager.baseBoard = guiManager.baseBoard5;
                        break;
                }
            }
            tmpTransform = emptyGO.transform;

            if (!string.IsNullOrEmpty(GloData.Instance.saveGamePathFolderName)) {
                // if (!GloData.Instance.isChallengeMode)
                gameMode = GloData.Instance.gameMode;
                // else
                //     gameMode = 1;
                loadSavedGame = GloData.Instance.loadSavedGame;
                type.Clear();
                if (gameMode > 0 || isChallengeMode) // clear flag
                    type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/game.save");
                else 
                    type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/grid" + Model.gridWidth + "/game.save");
                if (loadSavedGame) {
                    LoadGame(type.ToString());
                } else {
                    LoadNewGame();
                }
            } else {
                LoadNewGame();
            }
            // currentLevel = startingLevel;
            // startingHighScore = PlayerPrefs.GetInt("highscore");
            // startingHighScore2 = PlayerPrefs.GetInt("highscore2");
            // startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        
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
            m_ExplosionParticles.gameObject.SetActive(false);
            // 因为实例化粒子特效以后，实际上粒子的脚本就已经完成了初始化的工作，也就是Awake()和OnEnable()方法。然后设置SetActive(false)仅仅是把粒子特效隐藏起来。
        }

        public void onUndoGame(UndoGameEventInfo undoInfo) { 
            Debug.Log(TAG + ": onUndoGame()");
            if (MainScene_GUIManager.buttonInteractableList[2] == 0 || undoCnter == 0) return;
            ++tetrominoCnter;
            MainScene_ScoreManager.currentScore -= Tetromino.prevIndividualScore;
            
            Array.Clear(MainScene_GUIManager.buttonInteractableList, 0, MainScene_GUIManager.buttonInteractableList.Length);
            recycleThreeMajorTetromino();

            type.Clear();
            if (gameMode == 0 && !isChallengeMode)
                type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/grid" + Model.gridWidth + "/game" + ".save");
            else 
                type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/game" + ".save");
            GameData gameData = SaveSystem.LoadGame(type.ToString());
            if (ModelMono.hasDeletedMinos) {
                 MainScene_ScoreManager.currentScore = gameData.score; // todo score
                 MainScene_ScoreManager.numLinesCleared = gameData.lines;
                 // MainScene_ScoreManager.updateScoreEvent();
                 currentLevel = gameData.level;
                 // guiManager.UpdateLevel(); // for tmp

                 Debug.Log(TAG + ": onUndoGame() current board before respawn"); 
                 MathUtil.printBoard(Model.gridOcc); 
                
                 Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count);
                 LoadingSystemHelper.LoadDataFromParentList(gameData.parentList);

                 GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData); // MainCamera
                 GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            }
            if (gameData.prevPreview != null) { 
                type.Clear();
                string type2 = gameData.prevPreview2;
                if (gameData.isChallengeMode) {
                    SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2, gameData.prevPreviewColor, gameData.prevPreviewColor2);
                } else 
                    SpawnPreviewTetromino(type.Append(gameData.prevPreview).ToString(), type2);
            }
            MainScene_GUIManager.buttonInteractableList[0] = 1;
            MainScene_GUIManager.buttonInteractableList[1] = 1;
            --undoCnter;
        }

        private void SpawnPreviewTetromino(string type1, string type2, int color1, int color2) {
            previewTetromino = PoolManager.Instance.GetFromPool(type1, previewTetrominoPosition, Quaternion.identity, previewTetrominoScale + Vector3.one, color1);
            previewTetromino.transform.SetParent(previewSet.transform, false);
            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;
            previewTetrominoColor = previewTetromino.GetComponent<TetrominoType>().color;

            if (gameMode == 0) { // previewTetromino2
                previewTetromino2 = PoolManager.Instance.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, previewTetrominoScale + Vector3.one, color2);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                previewTetromino2Color = previewTetromino2.GetComponent<TetrominoType>().color;
            }
        }

        private void SpawnPreviewTetromino() {
            Debug.Log(TAG + ": SpawnPreviewTetromino()"); 
            previewTetromino = PoolManager.Instance.GetFromPool(Model.GetRandomTetromino(), previewTetrominoPosition, Quaternion.identity, previewTetrominoScale + Vector3.one);
            previewTetromino.transform.SetParent(previewSet.transform, false);

            // if (previewTetromino.GetComponent<Rotate>() != null)
            //     previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            // else
            //     previewTetromino.AddComponent<Rotate>();
            previewTetrominoType = previewTetromino.GetComponent<TetrominoType>().type;
            previewTetrominoColor = previewTetromino.GetComponent<TetrominoType>().color;

            // Debug.Log(TAG + " (previewTetromino != null): " + (previewTetromino != null)); 
            Debug.Log(TAG + " previewTetromino.name: " + previewTetromino.name); 
            
            if (gameMode == 0) { // previewTetromino2
                previewTetromino2 = PoolManager.Instance.GetFromPool(Model.GetRandomTetromino(), previewTetromino2Position, Quaternion.identity, previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                previewTetromino2Color = previewTetromino2.GetComponent<TetrominoType>().color;
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
            previewTetrominoColor = previewTetromino.GetComponent<TetrominoType>().color;

            if (gameMode == 0) { // previewTetromino2
                previewTetromino2 = PoolManager.Instance.GetFromPool(type2, previewTetromino2Position, Quaternion.identity, previewTetrominoScale + Vector3.one);
                previewTetromino2.transform.SetParent(previewSet2.transform, false);
                // if (previewTetromino2.GetComponent<Rotate>() != null)
                //     previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;
                // else
                //     previewTetromino2.AddComponent<Rotate>();
                previewTetromino2Type = previewTetromino2.GetComponent<TetrominoType>().type;
                previewTetromino2Color = previewTetromino2.GetComponent<TetrominoType>().color;
            }
        }

#region pausePanel Button Handlers
        public void onResumeGame() {
            Time.timeScale = 1.0f;
            isPaused = false;
            pausePanel.SetActive(false);
            AudioManager.Instance.PlayOneShotGameLoop();

        }
        
        public void onSaveGame(SaveGameEventInfo info) {
            Debug.Log(TAG + ": onSaveGame()");
            saveForUndo = false;
            onGameSave();
            hasSavedGameAlready = true;
            savedGamePanel.SetActive(true);
        }
        public void onSavedGamePanelOK() {
            savedGamePanel.SetActive(false);
        }
        public void onBackToMainMenu() {
            Debug.Log(TAG + ": onBackToMainMenu()"); 
            if (!hasSavedGameAlready && gameStarted) { 
                saveGameReminderPanel.SetActive(true);
            } else {
                Model.cleanUpGameBroad();

                if (MainScene_GUIManager.baseBoard != null)
                    MainScene_GUIManager.baseBoard.SetActive(false); // todo
                
                isPaused = false;
                Time.timeScale = 1.0f;
                if (isChallengeMode)
                    isChallengeMode = false;
                SceneManager.LoadScene("GameMenu");
            }
        }
        public void onYesToSaveGame() {
            saveForUndo = false;
            onGameSave();
            hasSavedGameAlready = true;
            saveGameReminderPanel.SetActive(false);
            pausePanel.SetActive(false);
            Model.cleanUpGameBroad();
            isPaused = false;
            Time.timeScale = 1.0f;
            if (isChallengeMode)
                isChallengeMode = false;
            SceneManager.LoadScene("GameMenu");
        }
        public void onNoToNotSaveGame() {
            hasSavedGameAlready = false;
            saveGameReminderPanel.SetActive(false);
            pausePanel.SetActive(false);
            Model.cleanUpGameBroad();
            isPaused = false;
            Time.timeScale = 1.0f;
            if (gameMode == 1)
                gameStarted = false;
            type.Clear();
            type.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/game.save").ToString();
            if (File.Exists(type.ToString())) {
                try {
                    File.Delete(type.ToString());
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }
            }
            if (isChallengeMode)
                isChallengeMode = false;
            SceneManager.LoadScene("GameMenu");
        }
#endregion
        // IEnumerator asyncLoadScene() {
        //     AsyncOperation async = SceneManager.LoadSceneAsync("GameMenu");
        //     yield return async;
        // }
        void onGameSave() {
            Debug.Log(TAG + ": onGameSave()");
            if (tmpTransform == null) // Bug: 再检查一下这个到底是怎么回事
                tmpTransform = new GameObject().transform;
            SaveSystem.SaveGame(this);
        }
        public void onPauseGame(PauseGameEventInfo pauseInfo) {  // originally pasuseButton & continueButton
            Debug.Log(TAG + ": onPauseGame()"); 
            if (Time.timeScale == 1.0f) {
                PauseGame();
            } else {
                onResumeGame();
            }
        }
        public void CheckUserInput() { // for resume button in SaveResumePanel
            Debug.Log(TAG + ": CheckUserInput()"); 
            if (Time.timeScale == 1.0f) {
                PauseGame();
            } else {
                onResumeGame();
            }
        }

        public void PauseGame() {
            Time.timeScale = 0f;	    
            AudioManager.Instance.Pause();
            isPaused = true;

            // Bug: disable all Hud canvas buttons: swap
            pausePanel.SetActive(true);
            // Bug TODO: when paused game, if game has NOT started yet, disable Save Button
            if (!gameStarted) {
            }
        }

        public static void recycleNextTetromino() {
            Debug.Log(TAG + ": recycleNextTetromino()"); 
            if (nextTetromino != null) {
                nextTetromino.tag = "Untagged";
                nextTetromino.GetComponent<Tetromino>().enabled = false;
                Model.resetGridAfterDisappearingNextTetromino(nextTetromino);
                if (isChallengeMode)
                    Model.resetSkinAfterDisappearingNextTetromino(nextTetromino);
                if (nextTetromino.transform.childCount == 4) {
                    PoolManager.Instance.ReturnToPool(nextTetromino, nextTetromino.GetComponent<TetrominoType>().type);
                } else
                    Destroy(nextTetromino.gameObject);
            }
            nextTetromino = null;
        }
        private void recycleThreeMajorTetromino() { // 回收三样东西：nextTetromino previewTetromino previewTetromino2
            recycleNextTetromino();
            preparePreviewTetrominoRecycle(1);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            preparePreviewTetrominoRecycle(2);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
        }

        public static void recycleGhostTetromino() {
            Debug.Log(TAG + ": recycleGhostTetromino()");
            Debug.Log(TAG + " ghostTetromino.name: " + ghostTetromino.name); 
            if (ghostTetromino != null) {
                ghostTetromino.tag = "Untagged";
                if (isChallengeMode && GloData.Instance.challengeLevel > 10) {
                    foreach (Transform mino in ghostTetromino.transform) {
                        PoolManager.Instance.ReturnToPool(mino.gameObject, mino.gameObject.GetComponent<MinoType>().type);
                    }
                }
                // else 
                PoolManager.Instance.ReturnToPool(ghostTetromino, ghostTetromino.GetComponent<TetrominoType>().type);
            }
        }
        
        private void FireSpawnedTetrominoEvent(Vector3 pos) {
            Debug.Log(TAG + ": FireSpawnedTetrominoEvent()");
            if (spawnedTetrominoInfo == null) {
                spawnedTetrominoInfo = new TetrominoSpawnedEventInfo();
                spawnedTetrominoInfo.delta = pos;
            }
            EventManager.Instance.FireEvent(spawnedTetrominoInfo);
        }
        private void currentActiveTetrominoPrepare() {
            Debug.Log(TAG + ": currentActiveTetrominoPrepare()");
            nextTetromino.tag = "currentActiveTetromino";
            nextTetromino.transform.rotation = Quaternion.identity;
            
            if (gameMode == 0 && (Model.gridWidth == 3 || Model.gridWidth == 4)) {
                nextTetromino.transform.localPosition = new Vector3(1.0f, Model.gridHeight - 1f, 1.0f);
            } else 
                nextTetromino.transform.localPosition = nextTetrominoSpawnPos;

            Debug.Log(TAG + " (defaultContainer == null) before: " + (defaultContainer == null));  // Bug source: it will introduce classic mode ghostTetromino bug, but so far let it be this way for tmp;
            if (defaultContainer == null)
                defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
            Debug.Log(TAG + " (defaultContainer == null) after: " + (defaultContainer == null)); 
            nextTetromino.transform.SetParent(defaultContainer.transform, false);

            
            nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
            nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;
            Debug.Log(TAG + " nextTetromino.name: " + nextTetromino.name);
            --tetrominoCnter;
            Debug.Log(TAG + " tetrominoCnter: " + tetrominoCnter); 
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
                    // nextTetromino = PoolManager.Instance.GetFromPool(Model.GetRandomTetromino(), new Vector3(2.0f, Model.gridHeight - 1f, 2.0f), Quaternion.identity, Vector3.one);
                    nextTetromino = PoolManager.Instance.GetFromPool(Model.GetRandomTetromino(), nextTetrominoSpawnPos, Quaternion.identity, Vector3.one);
                    currentActiveTetrominoPrepare();
                    SpawnGhostTetromino();
                    FireSpawnedTetrominoEvent(nextTetrominoSpawnPos);            
                    SpawnPreviewTetromino();
                }
            } else {
                previewTetromino.transform.localScale -= previewTetrominoScale;
                // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
                nextTetromino = previewTetromino;
                currentActiveTetrominoPrepare();
                SpawnGhostTetromino();  
                FireSpawnedTetrominoEvent(nextTetrominoSpawnPos);            
                SpawnPreviewTetromino();
            }
        }

        void LoadGame(string path) {  // when load Scene load game: according to gameMode
            Debug.Log(TAG + ": LoadGame()");
            if (gameMode == 0)
                Model.resetGridOccBoard(); 
            GameData gameData = SaveSystem.LoadGame(path);
            
            MainScene_ScoreManager.currentScore = gameData.score;
            MainScene_ScoreManager.numLinesCleared = gameData.lines;
            currentLevel = gameData.level;
            // guiManager.UpdateLevel();

            Debug.Log(TAG + " gameData.parentList.Count: " + gameData.parentList.Count); 
            LoadingSystemHelper.LoadDataFromParentList(gameData.parentList);

            // currentActiveTetromino: if it has NOT landed yet
            Debug.Log(TAG + " (gameData.nextTetrominoData != null): " + (gameData.nextTetrominoData != null)); 
            if (gameData.nextTetrominoData != null) {
                nextTetromino = PoolManager.Instance.GetFromPool(type.Append(gameData.nextTetrominoData.type).ToString(),
                                                                 DeserializedTransform.getDeserializedTransPos(gameData.nextTetrominoData.transform),
                                                                 DeserializedTransform.getDeserializedTransRot(gameData.nextTetrominoData.transform), Vector3.one);
                nextTetromino.tag = "currentActiveTetromino";
                
                if (defaultContainer == null)  // Bug source: as describe above C-s search for defaultContainer
                    defaultContainer = GameObject.FindGameObjectWithTag("defaultContainer");
                // Debug.Log(TAG + " (defaultContainer == null): " + (defaultContainer == null)); 
                nextTetromino.transform.SetParent(defaultContainer.transform, false);
                
                nextTetromino.GetComponent<Tetromino>().enabled = !nextTetromino.GetComponent<Tetromino>().enabled; 
                nextTetrominoType = nextTetromino.GetComponent<TetrominoType>().type;

                SpawnGhostTetromino();
                FireSpawnedTetrominoEvent(new Vector3(2.0f, nextTetromino.transform.position.y, 2.0f));
            }

            // previewTetromino previewTetromino2
            type.Clear();
            string type2 = gameData.previewTetromino2Type;
            if (gameData.isChallengeMode) {
                SpawnPreviewTetromino(type.Append(gameData.previewTetrominoType).ToString(), type2, gameData.previewTetrominoColor, gameData.previewTetromino2Color);
            } else 
                SpawnPreviewTetromino(type.Append(gameData.previewTetrominoType).ToString(), type2);
            if (gameData.prevPreview != null) {
                prevPreview = gameData.prevPreview;
                prevPreview2 = gameData.prevPreview2;
                if (gameData.isChallengeMode) {
                    prevPreviewColor = gameData.prevPreviewColor;
                    prevPreviewColor2 = gameData.prevPreviewColor2;
                }
            } 
            // MainCamera rotation
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = DeserializedTransform.getDeserializedTransPos(gameData.cameraData);
            GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = DeserializedTransform.getDeserializedTransRot(gameData.cameraData);
            
            if (gameMode == 1 && (nextTetromino == null || !nextTetromino.CompareTag("currentActiveTetromino"))) {
                // spawn currentActiveTetromino && ghostTetromino only, not previewTetromino
				nextTetromino = PoolManager.Instance.GetFromPool(Model.GetRandomTetromino(), new Vector3(2.0f, Model.gridHeight - 1f, 2.0f), Quaternion.identity, Vector3.one);
                currentActiveTetrominoPrepare();
                SpawnGhostTetromino();
                FireSpawnedTetrominoEvent(nextTetrominoSpawnPos);            
            }
            gameStarted = true;
        
            GloData.Instance.loadSavedGame = false;
            // loadSavedGame = false;
            GloData.Instance.loadSavedGame = false;
            // GloData.Instance.isChallengeMode = false; // ToDo Bug: clear flag
            
            // delete loaded saved game file
            try {
                File.Delete(path);
            } catch (System.Exception ex) {
                Debug.LogException(ex);
            }            
        }    
        
        public void onSwapPreviewTetrominos () {
            if (swapCnter == 0) return;
            Debug.Log(TAG + ": swapPreviewTetrominosFunc()");
            preparePreviewTetrominoRecycle(1); // recycle 1st tetromino first
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            preparePreviewTetrominoRecycle(2); // recycle 2st tetromino then
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            SpawnPreviewTetromino();
            swapCnter--;
            // Debug.Log(TAG + " swapCnter: " + swapCnter); 
        }
        
        public void SpawnGhostTetromino() {
            GameObject tmpTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            if (gameMode > 0 || (isChallengeMode && GloData.Instance.challengeLevel < 11))
                ghostTetromino = PoolManager.Instance.GetFromPool(Model.GetGhostTetrominoType(nextTetromino), nextTetromino.transform.position, nextTetromino.transform.rotation, Vector3.one);
            else { // colorful ghost tetrominos
                ghostTetromino = PoolManager.Instance.GetFromPool("shadowX", nextTetromino.transform.position, nextTetromino.transform.rotation, Vector3.one);
                int cnter = 0;
                foreach (Transform mino in nextTetromino.transform) {
                    if (ghostTetromino.transform.childCount == cnter) { // cnter == 0 || 
                        GameObject gost = PoolManager.Instance.GetFromPool(getGostMinoType(mino.gameObject.GetComponent<MinoType>().color), mino.transform.position, mino.transform.rotation, Vector3.one);
                        gost.transform.parent = ghostTetromino.transform;
                    }
                    ++cnter;
                }
                type.Clear();
                type.Append("shadow_").Append(nextTetromino.name.Substring(10, 1)).ToString();
                ghostTetromino.name = type.ToString();
                type.Clear();
                type.Append("shadow").Append(nextTetromino.name.Substring(10, 1)).ToString();
                ghostTetromino.GetComponent<TetrominoType>().type = type.ToString();
                ghostTetromino.GetComponent<TetrominoType>().childCnt = nextTetromino.GetComponent<TetrominoType>().childCnt;
            }
            ghostTetromino.GetComponent<GhostTetromino>().enabled = true;
        }
        string getGostMinoType(int color) {
            switch (color) {
                case 0:
                    return "gostR";
                case 1:
                    return "gostG";
                case 2:
                    return "gostB";
                case 3:
                    return "gostY";
            }
            return null;
        }
        public void GameOver() {
            Debug.Log(TAG + ": GameOver()"); 
			// EntryScene_ScoreManager.UpdateHighScore();
            SceneManager.LoadScene("GameOver");
        }
        
        public void playFirstTetromino() {
            Debug.Log(TAG + ": playFirstTetromino()");
            if (MainScene_GUIManager.buttonInteractableList[0] == 0) return;

            prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            if (isChallengeMode) {
                prevPreviewColor = previewTetromino.GetComponent<TetrominoType>().color;
                prevPreviewColor2 = previewTetromino2.GetComponent<TetrominoType>().color;
            }

            preparePreviewTetrominoRecycle(2);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino.transform.localScale -= previewTetrominoScale;
            // previewTetromino.GetComponent<Rotate>().enabled = !previewTetromino.GetComponent<Rotate>().enabled;
            
            nextTetromino = previewTetromino;
            gameStarted = true;
            currentActiveTetrominoPrepare();
            SpawnGhostTetromino();
            FireSpawnedTetrominoEvent(nextTetrominoSpawnPos);
            SpawnPreviewTetromino();
        }
        
        public void playSecondTetromino() {
            Debug.Log(TAG + ": playSecondTetromino()"); 
            if (MainScene_GUIManager.buttonInteractableList[1] == 0) return;
            
            prevPreview = previewTetromino.GetComponent<TetrominoType>().type;   
            prevPreview2 = previewTetromino2.GetComponent<TetrominoType>().type;
            if (isChallengeMode) {
                prevPreviewColor = previewTetromino.GetComponent<TetrominoType>().color;
                prevPreviewColor2 = previewTetromino2.GetComponent<TetrominoType>().color;
            }
            preparePreviewTetrominoRecycle(1);
            PoolManager.Instance.ReturnToPool(cycledPreviewTetromino, cycledPreviewTetromino.GetComponent<TetrominoType>().type);
            previewTetromino2.transform.localScale -= previewTetrominoScale;
            // previewTetromino2.layer = LayerMask.NameToLayer("Default");
            // previewTetromino2.GetComponent<Rotate>().enabled = !previewTetromino2.GetComponent<Rotate>().enabled;

            nextTetromino = previewTetromino2;
            gameStarted = true;
            currentActiveTetrominoPrepare();            
            SpawnGhostTetromino();
            FireSpawnedTetrominoEvent(nextTetrominoSpawnPos);            
            SpawnPreviewTetromino();
        }
    }
}
