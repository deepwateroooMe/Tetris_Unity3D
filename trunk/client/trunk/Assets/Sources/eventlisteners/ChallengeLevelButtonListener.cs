using UnityEngine;
using UnityEngine.SceneManagement;

namespace tetris3d {
    public class ChallengeLevelButtonListener : MonoBehaviour {
        private const string TAG = "ChallengeLevelButtonListener";

        public GameObject [] levelButtons; 
        public GameObject squaresEdgesCornersPanel;
        public GameObject matchingColorsPanel;
        public GameObject left;
        public GameObject right;
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            EventManager.Instance.RegisterListener<LevelButtonClickEventInfo>(onLevelButtonClicked); 
        }

        void onLevelButtonClicked(LevelButtonClickEventInfo info) { 
            Debug.Log(TAG + ": onLevelButtonClicked()");
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 

            squaresEdgesCornersPanel.SetActive(false);
            if (info.unitGO.name == "left") {
                matchingColorsPanel.SetActive(false);
                squaresEdgesCornersPanel.SetActive(true);
            } else if (info.unitGO.name == "right") {
                squaresEdgesCornersPanel.SetActive(false);
                matchingColorsPanel.SetActive(true);
            }
            if (info.unitGO.name == "LevelTrials1")  {
                GloData.Instance.challengeLevel = 1;
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetrominoCnter = 22;
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials2") {
                GloData.Instance.challengeLevel = 2;
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            }  else if (info.unitGO.name == "LevelTrials3") {
                GloData.Instance.challengeLevel = 3;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials4") { 
                GloData.Instance.challengeLevel = 4;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials5") {
                GloData.Instance.challengeLevel = 5;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials6")  {
                GloData.Instance.challengeLevel = 6;
                GloData.Instance.gridSize = 9;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetrominoCnter = 42;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials7") {
                GloData.Instance.challengeLevel = 7;
                // GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 10;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetrominoCnter = 42;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            }  else if (info.unitGO.name == "LevelTrials8") {
                GloData.Instance.challengeLevel = 8;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetrominoCnter = 42;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials9") { 
                GloData.Instance.challengeLevel = 9;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetrominoCnter = 42;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials10") {
                GloData.Instance.challengeLevel = 10;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetrominoCnter = 42;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials11")  {
                GloData.Instance.challengeLevel = 11;
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetrominoCnter = 22;
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials12") {
                GloData.Instance.challengeLevel = 12;
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            }  else if (info.unitGO.name == "LevelTrials13") {
                GloData.Instance.challengeLevel = 13;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials14") { 
                GloData.Instance.challengeLevel = 14;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } else if (info.unitGO.name == "LevelTrials15") {
                GloData.Instance.challengeLevel = 15;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetrominoCnter = 22;
                GameController.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                LoadScene(info.unitGO.name);
            } 
        }

        private void LoadScene(string scene) {
            SceneManager.LoadSceneAsync(scene);
        }

        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<LevelButtonClickEventInfo>(onLevelButtonClicked); 
            }
        }
    }
}