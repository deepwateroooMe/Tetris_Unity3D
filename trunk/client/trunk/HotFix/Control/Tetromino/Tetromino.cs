using Framework.MVVM;
using HotFix.UI;
using tetris3d;
using UnityEngine;

namespace HotFix.Control {

    public class Tetromino : MonoBehaviour { 
        private static string TAG = "Tetromino";
    
        // public int individualScore = 100;

        private float fall = 0f;
        private float fallSpeed = 1.0f;
    
        private float individualScoreTime;

// TODO: INTO to GameViewModel        
        private bool isMoveValid = false;
        public bool IsMoveValid { get { return isMoveValid; } }
        private bool isRotateValid = false;
        public bool IsRotateValid { get { return isRotateValid; } }

// 如果我不加这些,它掉得太快了,仿佛瞬间从天堂掉到了地狱.....而不是一秒一格地往下掉        
        Vector3 moveDelta;
        Vector3 rotateDelta;
        
        public void Awake() {
            moveDelta = Vector3.zero;
            rotateDelta = Vector3.zero;
        }
        public void Start () {
            // Debug.Log(TAG + ": Start()");
            fallSpeed = ViewManager.GameView.ViewModel.fallSpeed;

            // EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            // EventManager.Instance.RegisterListener<FallFastEventInfo>(onTetrominoFallFast);
        }
        public void OnDisable() { // TODO:方法还不有适配
            // Debug.Log(TAG + ": OnDisable()");
            // if (EventManager.Instance != null) {
                // EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onTetrominoMove);
                // EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
                // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            // EventManager.Instance.UnregisterListener<FallFastEventInfo>(onTetrominoFallFast);
            // }
        }        

        public void Update () {
            if (!ViewManager.GameView.isPaused) {
               CheckUserInput(); // MoveDown(): 触发事件前检查了事件的有效性
               UpdateIndividualScore();
               // UpdateFallSpeed();       // static 0.5f
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

        private void onTetrominoLandTetromino() {
            Debug.Log(TAG + ": onTetrominoLandTetromino()");
            EventManager.Instance.FireEvent("land");
            ViewManager.nextTetromino.tag = "Untagged";
            ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = false;
        }

        public void MoveDown() {
            // Debug.Log(TAG + ": MoveDown()"); 
            // if (movedImmediateVertical) {
            //     if (buttonDownWaitTimerVertical < buttonDownWaitMax) {
            //         buttonDownWaitTimerVertical += Time.deltaTime;
            //         return;
            //     }
            //     if (verticalTimer < continuousVerticalSpeed) {
            //         verticalTimer += Time.deltaTime;
            //         return;
            //     }
            // }
            // if (!movedImmediateVertical) 
            //     movedImmediateVertical = true;
            // verticalTimer = 0;
            // if (ViewManager.GameView.nextTetromino == null) return;

            moveDelta = new Vector3(0, -1, 0);
            // Debug.Log(TAG + " (ViewManager.nextTetromino == null): " + (ViewManager.nextTetromino == null));
            // Debug.Log(TAG + " (moveDelta == null): " + (moveDelta == null));
            ViewManager.nextTetromino.transform.position += moveDelta; // <<<<<<<<<<<<<<<<<<<< 
            bool isValidPos = CheckIsValidPosition();
            ViewManager.nextTetromino.transform.position -= moveDelta; // <<<<<<<<<<<<<<<<<<<< 
            if (isValidPos) {
                EventManager.Instance.FireEvent("move", moveDelta); // 有效下落,触发事件
            } else { // 往下移,不能再下移了,就是到最底格可以放置了
                PoolHelper.recycleGhostTetromino(); // 涉及事件的先后顺序，这里处理比较安全：确保在Tetromino之前处理
                onTetrominoLandTetromino(); // EventManager.Instance.FireEvent("land");
            }
            fall = Time.time; // 更新现在的时间
        }
        
        void CheckUserInput() {
            // Debug.Log(TAG + ": CheckUserInput()"); 
// #if UNITY_ANDROID // 我暂时就不考虑这些了
//             if (Input.touchCount > 0) {
//                 Touch t = Input.GetTouch(0);
//                 if (t.phase == TouchPhase.Began) {
//                     previousUnitPosition = new Vector2(t.position.x, t.position.y);
//                 } else if (t.phase == TouchPhase.Moved) {
//                     Vector2 touchDeltaPosition = t.deltaPosition;
//                     direction = touchDeltaPosition.normalized;
//                     if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal
//                         && direction.x < 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) { // move left
//                         MoveXNeg(); // MoveLeft();
//                         previousUnitPosition = t.position;
//                         moved = true;
//                     } else if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal
//                                && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) { // move right
//                         MoveXPos(); // MoveRight();
//                         previousUnitPosition = t.position;
//                         moved = true;
//                     } else if (Mathf.Abs(t.position.y - previousUnitPosition.y) >= touchSensitivityVertical &&
//                                direction.y < 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10) {
//                         MoveDown(); // move down
//                         previousUnitPosition = t.position;
//                         moved = true;
//                     }
//                 } else if (t.phase == TouchPhase.Ended) { // double click?
//                     if (!moved && t.position.x > Screen.width / 4) 
//                         Rotate();
//                     moved = false;
//                 }
//             }
//             if (Time.time - fall >= fallSpeed) {
//                 MoveDown();
//             }
// #else 
//             if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {
//                 movedImmediateHorizontal = false;
//                 horizontalTimer = 0;
//                 buttonDownWaitTimerHorizontal = 0;
//             } 
//             if (Input.GetKeyUp(KeyCode.DownArrow)) {
//                 movedImmediateVertical = false;
//                 verticalTimer = 0;
//                 buttonDownWaitTimerVertical = 0;
//             } 
//               // translation, main camera, for checking mouse clicks
//             if (Input.GetKey(KeyCode.W)) MoveZPos();
//             if (Input.GetKey(KeyCode.S)) MoveZNeg();
//             if (Input.GetKey(KeyCode.A)) MoveXNeg();
//             if (Input.GetKey(KeyCode.D)) MoveXPos();
//             if (Input.GetKey(KeyCode.LeftArrow)) RotateZ();     // MoveLeft
//             if (Input.GetKey(KeyCode.RightArrow)) RotateX();    // MoveRight
//             if (Input.GetKeyDown(KeyCode.UpArrow)) RotateY();   // RotateUp
//             if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) MoveDown(); // DOWN
//             // perspective camera
//             // translation, main camera
//             if (Input.GetKey(KeyCode.A)) MoveXNeg();
//             if (Input.GetKey(KeyCode.D)) MoveXPos();
//             if (Input.GetKey(KeyCode.W)) MoveZPos();
//             if (Input.GetKey(KeyCode.S)) MoveZNeg();
//             if (Input.GetKey(KeyCode.LeftArrow)) RotateZ();     // MoveLeft
//             if (Input.GetKey(KeyCode.RightArrow)) RotateX();    // MoveRight
//             if (Input.GetKeyDown(KeyCode.UpArrow)) RotateY();   // RotateUp
//             if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) MoveDown(); // DOWN
//             // tetromino camera
//             if (Input.GetKey(KeyCode.A)) MoveZNeg(); // 左
//             if (Input.GetKey(KeyCode.D)) MoveZPos(); // 右
//             if (Input.GetKey(KeyCode.W)) MoveXNeg(); // 上
//             if (Input.GetKey(KeyCode.S)) MoveXPos(); // 下

            if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {
                MoveDown();
            }
// #endif
        }

        void UpdateFallSpeed() { 
            fallSpeed = ViewManager.GameView.ViewModel.fallSpeed;
        }
    
        void UpdateIndividualScore() {
            if (individualScoreTime < 1) {
                individualScoreTime += Time.deltaTime;
            } else {
                individualScoreTime = 0;
                //individualScore = Mathf.Max(individualScore - 10, 0);
            }
        }

        public bool CheckIsValidPosition() { 
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    mino.rotation = Quaternion.identity;
                    Vector3 pos = MathUtilP.Round(mino.position);
                    if (!Model.CheckIsInsideGrid(pos)) {
                        MathUtilP.print(pos);
                        return false;
                    }
                    if (Model.GetTransformAtGridPosition(pos) != null
                        && Model.GetTransformAtGridPosition(pos).parent != ViewManager.nextTetromino.transform) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}