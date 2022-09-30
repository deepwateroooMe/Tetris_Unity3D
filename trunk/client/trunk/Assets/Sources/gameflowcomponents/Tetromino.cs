using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace tetris3d {

    public class Tetromino : MonoBehaviour { 
        private static string TAG = "Tetromino";
    
        public int individualScore = 400;
        private float fallSpeed = 3.0f;
        private float fall = 0f;

        private TetrominoLandEventInfo landInfo;
        private CanvasMovedEventInfo canvasMovedInfo;
        
        private float individualScoreTime;
        public static int prevIndividualScore = 400;

        void OnEnable () {
            Debug.Log(TAG + ": OnEnable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            fallSpeed = GameController.fallSpeed;

            landInfo = new TetrominoLandEventInfo();
            canvasMovedInfo = new CanvasMovedEventInfo();
            EventManager.Instance.RegisterListener<FallFastEventInfo>(onFallFast);
        }
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<FallFastEventInfo>(onFallFast);
            }
        }        

        public void MoveDown() {
            Debug.Log(TAG + ": MoveDown()");
            GameController.nextTetromino.transform.position += new Vector3(0, -1, 0);
            MathUtil.print(GameController.nextTetromino.transform.position);
            Debug.Log(TAG + " (Model.CheckIsValidPosition()): " + (Model.CheckIsValidPosition())); 
            if (Model.CheckIsValidPosition()) { // check if physically fits into the grid
                if (canvasMovedInfo == null) {
                    canvasMovedInfo = new CanvasMovedEventInfo();
                }
                canvasMovedInfo.delta = new Vector3(0, -1, 0);
                EventManager.Instance.FireEvent(canvasMovedInfo);
                Model.UpdateGrid(GameController.nextTetromino);
            } else {
                GameController.nextTetromino.transform.position += new Vector3(0, 1, 0); // reach the most bottom layer it could
                if (landInfo == null)
                    landInfo = new TetrominoLandEventInfo();
                EventManager.Instance.FireEvent(landInfo);
                GameController.recycleGhostTetromino(); // 涉及事件的先后顺序，这里处理比较安全：确保在Tetromino之前处理
                onTetrominoLand();
            }
            fall = Time.time; 
        }
        
        public void onFallFast(FallFastEventInfo fallFastInfo) { // public void SlamDown() {
            Debug.Log(TAG + ": onFallFast()"); 
            while (Model.CheckIsValidPosition()) {
                if (canvasMovedInfo == null) { 
                    canvasMovedInfo = new CanvasMovedEventInfo();
                }
                canvasMovedInfo.delta = new Vector3(0, -1, 0);
                EventManager.Instance.FireEvent(canvasMovedInfo);
                GameController.nextTetromino.transform.position += new Vector3(0, -1, 0);
            }
            Debug.Log(TAG + " (!Model.CheckIsValidPosition()): " + (!Model.CheckIsValidPosition())); 
            if (!Model.CheckIsValidPosition()) { 
                GameController.nextTetromino.transform.position += new Vector3(0, 1, 0);
                if (landInfo == null)
                    landInfo = new TetrominoLandEventInfo();
                EventManager.Instance.FireEvent(landInfo);
                GameController.recycleGhostTetromino();
                onTetrominoLand();
            }
        }
        private void onTetrominoLand() {
            GameController.nextTetromino.tag = "Untagged";
            GameController.nextTetromino.GetComponent<Tetromino>().enabled = false;
            MainScene_ScoreManager.currentScore += individualScore;
            prevIndividualScore = individualScore;
        }
        
        void Update () {
            if (!GameController.isPaused) {
                CheckUserInput();
                // UpdateIndividualScore(); // 没必要 ？？？
                UpdateFallSpeed();       // static 1.0f
            } 
//             if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {        
// #if UNITY_ANDROID || UNITY_IPHONE
//                 if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
// #else 
//               if (EventSystem.current.IsPointerOverGameObject())
// #endif
//               text.text = "当前触摸在UI上";
//               else
//               text.text = "当前没有触摸在UI上";
//               }
        }
        
        void UpdateFallSpeed() {
            // Debug.Log(TAG + ": UpdateFallSpeed()"); 
            fallSpeed = GameController.fallSpeed;
        }
        void CheckUserInput() {
            if (Time.time - fall >= fallSpeed) {
                MoveDown();
            }
        }
    
        // void UpdateIndividualScore() {
        //     // Debug.Log(TAG + ": UpdateIndividualScore()"); 
        //     if (individualScoreTime < 1) {
        //         individualScoreTime += Time.deltaTime;
        //         individualScore = Mathf.Max(individualScore + 10, 0);
        //     } else {
        //         individualScoreTime = 0;
        //         // individualScore = Mathf.Max(individualScore - 10, 0);
        //         individualScore = Mathf.Max(individualScore, 0);
        //     }
        // }
    }
}