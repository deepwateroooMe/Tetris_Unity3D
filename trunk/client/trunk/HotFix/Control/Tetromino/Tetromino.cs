using Framework.MVVM;
using HotFix.UI;
using tetris3d;
using UnityEngine;

namespace HotFix.Control {

    public class Tetromino : MonoBehaviour { 
        private static string TAG = "Tetromino";
    
        public int individualScore = 100;

        private float fall = 0f;
        private float fallSpeed = 1.0f;
    
        private float individualScoreTime;

// TODO: INTO to GameViewModel        
        private bool isMoveValid = false;
        public bool IsMoveValid { get { return isMoveValid; } }
        private bool isRotateValid = false;
        public bool IsRotateValid { get { return isRotateValid; } }

// 如果我不加这些,它掉得太快了,仿佛瞬间从天堂掉到了地狱.....而不是一秒一格地往下掉        
        private float timer = 0.5f;
        Vector3 moveDelta;
        Vector3 rotateDelta;
        
        public void Awake() {
            moveDelta = Vector3.zero;
            rotateDelta = Vector3.zero;
        }
        public void Start () {
            Debug.Log(TAG + ": Start()");
            fallSpeed = ViewManager.GameView.ViewModel.fallSpeed;

            // EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }
        public void OnDisable() { // TODO:方法还不有适配
            Debug.Log(TAG + ": OnDisable()");
            // if (EventManager.Instance != null) {
                // EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onTetrominoMove);
                // EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
                // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            // }
        }        

        public void Update () {
            timer -= Time.deltaTime;
            if (timer > 0) return;
             
            if (!ViewManager.GameView.ViewModel.isPaused) {
               CheckUserInput();
               UpdateIndividualScore();
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

            moveDelta = new Vector3(0, -1, 0);
// TODO: 背景音乐会多放一次            
            EventManager.Instance.FireEvent("move", moveDelta);

            timer = 0.5f;
        }

// TODO: 这里的逻辑需要想一想,好像写成了循环嵌套!!!
// TODO: 下面的这两个方法可以不要了
        public void onTetrominoMove(TetrominoMoveEventInfo info) { 
            isMoveValid = false;
            ViewManager.nextTetromino.transform.position += info.delta;
            if (((GameViewModel)ViewManager.GameView.ViewModel).CheckIsValidPosition()) { 
                isMoveValid = true;
                EventManager.Instance.FireEvent("move", info.delta);
            } else {
                ViewManager.nextTetromino.transform.position -= info.delta;
            }
        }
        public void onTetrominoRotate(TetrominoRotateEventInfo info) {
            ViewManager.nextTetromino.transform.Rotate(info.delta);
            isRotateValid = false;
            if (((GameViewModel)ViewManager.GameView.ViewModel).CheckIsValidPosition()) {
                isRotateValid = true;
                EventManager.Instance.FireEvent("Rotate", info.delta);
            } else { 
                ViewManager.nextTetromino.transform.Rotate(Vector3.zero - info.delta); 
            }
        }

        private void onTetrominoLand() {
            Debug.Log(TAG + ": onTetrominoLand()");
            EventManager.Instance.FireEvent("land");
            ViewManager.nextTetromino.tag = "Untagged";
            ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = false;
            ViewManager.GameView.ViewModel.currentScore.Value += individualScore;            
        }

        public void MoveDown() {
            Debug.Log(TAG + ": MoveDown()"); 
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

            moveDelta = new Vector3(0, -1, 0); // 这种调用太慢了
            MathUtil.print(ViewManager.nextTetromino.transform.position);
            
            ViewManager.nextTetromino.transform.position += moveDelta;
            // (GameViewModel)ViewManager.GameView.ViewModel.MoveDown(); // 直接调用,要两块画布下移一格
            bool isValidPos = CheckIsValidPosition();
            Debug.Log(TAG + " isValidPos: " + isValidPos);
            ViewManager.nextTetromino.transform.position -= moveDelta;
            if (isValidPos) {
                ViewManager.GameView.ViewModel.UpdateGrid(ViewManager.nextTetromino);
                // EventManager.Instance.FireEvent("move", moveDelta); // 这里有个无限循环,会没有声音
// TODO: 这里怎么只在用户有键输入的时候才播放音乐呢?
				// if (Input.GetKey(KeyCode.DownArrow)) ; 
                // // 这里做的事情是播放音乐
            } else { // 往下移,不能再下移了,就是该合理着陆
                // ViewManager.nextTetromino.transform.position += new Vector3(0, 1, 0);
                PoolHelper.recycleGhostTetromino(); // 涉及事件的先后顺序，这里处理比较安全：确保在Tetromino之前处理
                ((GameViewModel)ViewManager.GameView.ViewModel).MoveUp(); // 要它快点儿提上来
                onTetrominoLand(); 
                EventManager.Instance.FireEvent("land");
            }
            fall = Time.time; 
        }
        void CheckUserInput() {
            Debug.Log(TAG + ": CheckUserInput()"); 
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

            // Debug.Log(TAG + " (Time.time - fall >= fallSpeed): " + (Time.time - fall >= fallSpeed)); 
            if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {
                MoveDown();
            }
// #endif
        }

        void UpdateFallSpeed() { 
            // Debug.Log(TAG + ": UpdateFallSpeed()"); 
            fallSpeed = ViewManager.GameView.ViewModel.fallSpeed;
        }
    
        void UpdateIndividualScore() {
            // Debug.Log(TAG + ": UpdateIndividualScore()"); 
            if (individualScoreTime < 1) {
                individualScoreTime += Time.deltaTime;
            } else {
                individualScoreTime = 0;
                individualScore = Mathf.Max(individualScore - 10, 0);
            }
        }

        // public void SlamDown() {
        //     Debug.Log(TAG + ": SlamDown()");
        //     Debug.Log(TAG + " ViewManager.GameView.getSlamDownIndication(): " + ViewManager.GameView.ViewModel.getSlamDownIndication()); 
        //     // if (ViewManager.GameView.buttonInteractableList[5] == 0) return;
        //     if (((MenuViewModel)ViewManager.GameView.ViewModel.ParentViewModel).gameMode == 0 && ViewManager.GameView.ViewModel.getSlamDownIndication() == 0) return;
        //     while (CheckIsValidPosition()) { // 不知道这种极速调用,反应得过来吗?
        //         // ViewManager.nextTetromino.transform.position += new Vector3(0, -1, 0);
        //         // ViewManager.GameView.MoveDown(); 
        //     }
        //     if (!CheckIsValidPosition()) {
        //         ViewManager.nextTetromino.transform.position += new Vector3(0, 1, 0);
        //         PoolHelper.recycleGhostTetromino();
        //         //onTetrominoLand();
        //         EventManager.Instance.FireEvent("land");
		// 	}
		// }
        public bool CheckIsValidPosition() { 
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                    MathUtil.print(pos);
                    if (!ViewManager.GameView.ViewModel.CheckIsInsideGrid(pos)) {
                        return false;
                    }
                    Transform transAtGrid = ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos);
                    // Debug.Log(TAG + " (transAtGrid != null): " + (transAtGrid != null));
                    // if (transAtGrid != null) {
                    //     Debug.Log(TAG + " (transAtGrid.parent != ViewManager.nextTetromino.transform): " + (transAtGrid.parent != ViewManager.nextTetromino.transform));
                    // }
                    if (ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos) != null
                        && ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos).parent != ViewManager.nextTetromino.transform) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}