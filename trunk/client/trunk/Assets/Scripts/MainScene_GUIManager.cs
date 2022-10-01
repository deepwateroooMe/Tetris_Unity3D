using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

namespace tetris3d {

    public class MainScene_GUIManager : MonoBehaviour {
        private const string TAG = "MainScene_GUIManager";

        public static GameObject baseBoard;
        public GameObject goalPanel;
        
        public Text hud_score;
        public Text hud_level;
        public Text hud_lines;
        public Text tetrominoCnter;
        public Text swapCnter;
        public Text undoCnter;
        
        public Sprite newImg; 
        public Sprite prevImg;
        public Button invertButton;

        public GameObject moveCanvas;
        public GameObject rotateCanvas;

        public GameObject baseBoard3;
        public GameObject baseBoard4;
        public GameObject baseBoard5;

        public GameObject hudButtonPanel0; // 0  swapPreviewTetrominoButton
        public GameObject hudButtonPanel;  // 11 undoButton
        public GameObject hudButtonPanel1; // 12 pauseButton
        public GameObject hudButtonPanel2; // 2  toggleButton fallButton

        public Text goalText;
        
        private static bool isMovement = true;
        private bool isScoreUpdated = false;
        private bool isFirstTimeEnable = false;

        public static int [] buttonInteractableList = new int[3]{ 1, 1, 0}; // previewSelectionButton     0  
                                                                            // previewSelectionButton2    1  
                                                                            // undoButton                 2
        private void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawned);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame);

            EventManager.Instance.RegisterListener<PropertyChangedEventInfo>(onPropertyChanged);
            EventManager.Instance.RegisterListener<ToggleActionEventInfo>(onToggleActions);
            EventManager.Instance.RegisterListener<CanvasMovedEventInfo>(onCanvasMoved); 

            hudButtonPanel0.SetActive(true); // 0  swapPreviewTetrominoButton
            hudButtonPanel1.SetActive(true); // 12 pauseButton
        }
        private void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            if (hudButtonPanel0 != null && hudButtonPanel0.activeSelf)
                hudButtonPanel0.SetActive(false); // 0  swapPreviewTetrominoButton
            if (hudButtonPanel1 != null && hudButtonPanel1.activeSelf)
                hudButtonPanel1.SetActive(false); // 12 pauseButton

            EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawned);
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame);

            EventManager.Instance.UnregisterListener<PropertyChangedEventInfo>(onPropertyChanged);
            EventManager.Instance.UnregisterListener<ToggleActionEventInfo>(onToggleActions);
            EventManager.Instance.UnregisterListener<CanvasMovedEventInfo>(onCanvasMoved); 
        }

        // public void InitLevelGoal() {
        //     (goalText as Text).text = new StringBuilder("Get 25000 Points Using ").Append(GloData.Instance.tetrominoCnter + " Blocks").ToString();
        // }

        public void onPropertyChanged(PropertyChangedEventInfo propertyChangedInfo) {
            if (propertyChangedInfo.propertyName.Equals("currentScore")) {
                hud_score.text = MainScene_ScoreManager.currentScore.ToString();
            } else if (propertyChangedInfo.propertyName.Equals("tetrominoCnter")) {
                tetrominoCnter.text = FindObjectOfType<GameController>().tetrominoCnter.ToString();
            } else if (propertyChangedInfo.propertyName.Equals("swapCnter")) {
                swapCnter.text = FindObjectOfType<GameController>().swapCnter.ToString();
            } else if (propertyChangedInfo.propertyName.Equals("undoCnter")) {
                undoCnter.text = FindObjectOfType<GameController>().undoCnter.ToString();
            } 
        }
        public void onActiveTetrominoSpawned(TetrominoSpawnedEventInfo info) { // undoButton deactivated
            Debug.Log(TAG + ": onActiveTetrominoSpawned()");
            buttonInteractableList[0] = 0;
            buttonInteractableList[1] = 0;
            hudButtonPanel0.SetActive(false);
            hudButtonPanel.SetActive(false);
            if (!isFirstTimeEnable) {
                hudButtonPanel1.SetActive(true); // pauseButton enabled
                isFirstTimeEnable = true;
            }
            hudButtonPanel2.SetActive(true);
            moveCanvas.SetActive(true);
            moveRotatecanvasPrepare(info.delta);
        }
        public void onActiveTetrominoLand(TetrominoLandEventInfo info) { // undoButton active
            Debug.Log(TAG + ": onActiveTetrominoLand()");                // pauseButton active anytime
            MoveUp();
            DisableMoveRotationCanvas();
            buttonInteractableList[0] = 1;
            buttonInteractableList[1] = 1;
            buttonInteractableList[2] = 1;
            hudButtonPanel0.SetActive(true); // swapPreviewTetrominoButton
            hudButtonPanel.SetActive(true);  // undoButton
            hudButtonPanel2.SetActive(false);
        }
        public void onUndoGame(UndoGameEventInfo undoInfo) { 
            Debug.Log(TAG + ": onUndoGame()");
            buttonInteractableList[2] = 0;
            hudButtonPanel0.SetActive(true); // swapPreviewTetrominoButton
            hudButtonPanel.SetActive(true);  // 
        }
        
        public void onToggleActions(ToggleActionEventInfo info) { // 回调函数
            Debug.Log(TAG + ": onToggleActions()");
            toggleButtons(1); // 1 ?
        }
        public void onCanvasMoved(CanvasMovedEventInfo canvasMovedInfo) {
            // Debug.Log(TAG + ": onCanvasMoved()");
            moveCanvas.transform.position += canvasMovedInfo.delta;
            rotateCanvas.transform.position += canvasMovedInfo.delta;
            if (canvasMovedInfo.delta.y != 0) // rotateCanvas moves ups and downs only
                rotateCanvas.transform.position += new Vector3(0, canvasMovedInfo.delta.y, 0);
        }

        public void UpdateScore() {
            hud_score.text = MainScene_ScoreManager.currentScore.ToString();
            hud_lines.text = MainScene_ScoreManager.numLinesCleared.ToString();
            isScoreUpdated = false;
        }
        
        public void toggleButtons(int indicator) { // 回调函数
            Debug.Log(TAG + ": toggleButtons()");
            if (GameController.gameMode > 0 || indicator == 1) {
                if (isMovement) { 
                    isMovement = false;
                    Debug.Log(TAG + " (prevImg == null): " + (prevImg == null)); 
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

        private void MoveDown() {
            moveCanvas.transform.position += new Vector3(0, -1, 0);
            rotateCanvas.transform.position += new Vector3(0, -1, 0);
        }
        private void MoveUp() {
            Debug.Log(TAG + ": MoveUp()");
            moveCanvas.transform.position += new Vector3(0, 1, 0);
            rotateCanvas.transform.position += new Vector3(0, 1, 0);
        }

        public void moveRotatecanvasPrepare(Vector3 pos) {
            Debug.Log(TAG + ": moveRotatecanvasPrepare()"); 
            moveCanvas.transform.position = pos;
            rotateCanvas.transform.position = pos;
            isMovement = false;
            toggleButtons(1);
        }
        public void DisableMoveRotationCanvas() {
            Debug.Log(TAG + ": DisableMoveRotationCanvas()"); 
            moveCanvas.SetActive(false);
            rotateCanvas.SetActive(false);
        }
        
        // private void Update() { // control: Update only when needed, NOT every frame, work ????
        //     if (!isScoreUpdated) {
        //         UpdateScore();
        //         isScoreUpdated = true;
        //     }
        // }

        public void UpdateLevel() {
            hud_level.text = GameController.currentLevel.ToString();
        }

        public void setAllBaseBoardInactive() {
            baseBoard3.SetActive(false);
            baseBoard4.SetActive(false);
            baseBoard5.SetActive(false);
        }
    }
}