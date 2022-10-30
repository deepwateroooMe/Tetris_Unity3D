using Framework.MVVM;
using HotFix.UI;
using tetris3d;
using UnityEngine;

namespace HotFix.Control {

    public class Tetromino : MonoBehaviour { 
        private static string TAG = "Tetromino";
    
        public int individualScore = 100;
        // public AudioClip moveSound;
        // public AudioClip rotateSound;
        // public AudioClip landSound;

        private float fall = 0f;
        private float fallSpeed = 3.0f;
    
        private float individualScoreTime;
        private AudioSource audioSource;

        // public bool limitRotation = false; // No-Limited Rotation Control
        // public bool noRotationX = false;
        // public bool noRotationY = false;
        // public bool noRotationZ = false;
        // private float continuousVerticalSpeed = 0.05f; // the speed at which the tetromino will move when the down button is held down
        // private float continuousHorizontalSpeed = 0.1f;// the speed at which the tetromino will move when the left or right buttons are held down
        // private float buttonDownWaitMax = 0.2f;        // how long to wait before the tetromino recognizeds that a button is being held down
        // private float verticalTimer = 0;
        // private float horizontalTimer = 0;
        // private float buttonDownWaitTimerHorizontal = 0;
        // private float buttonDownWaitTimerVertical = 0;
        // private bool movedImmediateHorizontal = false;
        // private bool movedImmediateVertical = false;
        // private int touchSensitivityHorizontal = 8;
        // private int touchSensitivityVertical = 4;
        // private Vector3 previousUnitPosition = Vector3.zero; // 不受影响？
        // private Vector3 direction = Vector3.zero;
        // private bool moved = false; // this one used for android touch

        private TetrominoLandEventInfo info;

// TODO: INTO to GameViewModel        
        private bool isMoveValid = false;
        public bool IsMoveValid { get { return isMoveValid; } }
        private bool isRotateValid = false;
        public bool IsRotateValid { get { return isRotateValid; } }

// 如果我不加这些,它掉得太快了,仿佛瞬间从天堂掉到了地狱.....而不是一秒一格地往下掉        
        private float timer = 1.0f;
        private Vector3 delta; // for MOVE ROTATE delta

        public void Awake() {
            Debug.Log(TAG + " Awake");
            delta = Vector3.zero;
        }
        
        public void Start () {
            Debug.Log(TAG + ": Start()");
            fallSpeed = ViewManager.GameView.ViewModel.fallSpeed;

            info = new TetrominoLandEventInfo();
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }

        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onTetrominoMove);
                EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onTetrominoRotate);
                // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
            }
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

            // Vector3 cur = ((GameViewModel)ViewManager.GameView.BindingContext).nextTetroPos.Value;
            // ((GameViewModel)ViewManager.GameView.BindingContext).nextTetroPos.Value = cur + new Vector3(0, -1, 0);
            delta = new Vector3(0, -1, 0);
            EventManager.Instance.FireEvent("move", delta);
            // ViewManager.nextTetromino.gameObject.transform.position += new Vector3(0, -1, 0);

            timer = 1.0f;
        }
        
        public void onTetrominoMove(TetrominoMoveEventInfo eventInfo) { // 不使用 _command_
            Debug.Log(TAG + ": onTetrominoMove()"); 
            isMoveValid = false;
            ViewManager.nextTetromino.transform.position += eventInfo.delta;
            if (CheckIsValidPosition()) { // 这里下移相比于平移可以再简化优化一下?
                isMoveValid = true;
				AudioManager.Instance.PlayAudio("move");
            } else {
                ViewManager.nextTetromino.transform.position -= eventInfo.delta;
            }
        }
        // public void RotateXPos() {
        //     ViewManager.nextTetromino.transform.Rotate(90, 0, 0);
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         ViewManager.nextTetromino.transform.Rotate(-90, 0, 0); 
        //     }
        // }
        public void onTetrominoRotate(TetrominoRotateEventInfo eventInfo) {
            // Debug.Log(TAG + ": onTetrominoRotate()"); 
            ViewManager.nextTetromino.transform.Rotate(eventInfo.delta);
            isRotateValid = false;
            if (CheckIsValidPosition()) {
                isRotateValid = true;
                AudioManager.Instance.PlayAudio("rotate");
            } else { 
                ViewManager.nextTetromino.transform.Rotate(Vector3.zero - eventInfo.delta); 
            }
        }
        // public void onTetrominoLand(TetrominoLandEventInfo eventInfo) {
        private void onTetrominoLand() {
            Debug.Log(TAG + ": onTetrominoLand()");
			AudioManager.Instance.PlayAudio("land");
            ViewManager.GameView.nextTetromino.tag = "Untagged";
            ViewManager.GameView.nextTetromino.GetComponent<Tetromino>().enabled = false;
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

            // ((GameViewModel)ViewManager.GameView.BindingContext).nextTetroPos.Value += new Vector3(0, -1, 0); // 不适用了,没再用这套系统
            ViewManager.nextTetromino.transform.position += new Vector3(0, -1, 0);

// 这里写的都是些什么乱七八糟的代码?完全是牛头不对马尾            
            // ViewManager.GameView.MoveDown(); // 怎么会这样呢?什么时候会写出这种.....
            // Debug.Log(TAG + " CheckIsValidPosition(): " + CheckIsValidPosition()); 
            if (CheckIsValidPosition()) {
                ViewManager.GameView.ViewModel.UpdateGrid(ViewManager.GameView.nextTetromino);
                if (Input.GetKey(KeyCode.DownArrow)) 
                    AudioManager.Instance.PlayAudio("move");
            } else {
                ViewManager.nextTetromino.transform.position += new Vector3(0, 1, 0);
                ViewManager.GameView.ViewModel.recycleGhostTetromino(ViewManager.GameView.ghostTetromino); // 涉及事件的先后顺序，这里处理比较安全：确保在Tetromino之前处理
                onTetrominoLand();
                if (info == null)
                    info = new TetrominoLandEventInfo();
                //info.unitGO = gameObject;
// cannot convert from Hotfix.control.eventinfo to System.Reflection.EventInfo: 就是把事件管理器放到主工程后,与热更新工程之间交通需要适配一下
                //EventManager.Instance.FireEvent(info);
            }
            fall = Time.time; 
        }
        public void SlamDown() {
            Debug.Log(TAG + ": SlamDown()");
            Debug.Log(TAG + " ViewManager.GameView.getSlamDownIndication(): " + ViewManager.GameView.ViewModel.getSlamDownIndication()); 
            // if (ViewManager.GameView.buttonInteractableList[5] == 0) return;
            if (((MenuViewModel)ViewManager.GameView.ViewModel.ParentViewModel).gameMode == 0 && ViewManager.GameView.ViewModel.getSlamDownIndication() == 0) return;
            while (CheckIsValidPosition()) {
                ViewManager.nextTetromino.transform.position += new Vector3(0, -1, 0);
                ViewManager.GameView.MoveDown(); 
            }
            if (!CheckIsValidPosition()) {
                ViewManager.nextTetromino.transform.position += new Vector3(0, 1, 0);
                ViewManager.GameView.ViewModel.recycleGhostTetromino(ViewManager.GameView.ghostTetromino);
                onTetrominoLand();
                if (info == null)
                    info = new TetrominoLandEventInfo();
                //info.unitGO = gameObject;
                // cannot convert from Hotfix.control.eventinfo to System.Reflection.EventInfo: 就是把事件管理器放到主工程后,与热更新工程之间交通需要适配一下
                //EventManager.Instance.FireEvent(info);
			}
		}

        public bool CheckIsValidPosition() { 
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    // Vector3 pos = ViewManager.GameView.Round(mino.position);
                    Vector3 pos = MathUtil.Round(mino.position);
                    bool isInsideGrid = ViewManager.GameView.ViewModel.CheckIsInsideGrid(pos);
                    Debug.Log(TAG + " isInsideGrid: " + isInsideGrid);
                    // if (!ViewManager.GameView.ViewModel.CheckIsInsideGrid(pos)) {
                    if (!isInsideGrid)
                        return false;
                    // }
                    Transform transAtGrid = ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos);
                    Debug.Log(TAG + " (transAtGrid != null): " + (transAtGrid != null));
                    Debug.Log(TAG + " (transAtGrid.parent != ViewManager.nextTetromino.transform): " + (transAtGrid.parent != ViewManager.nextTetromino.transform));
                    if (ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos) != null
                        && ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos).parent != ViewManager.nextTetromino.transform) {
                        return false;
                    }
                }
            }
            return true;
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

        public void MoveZNeg() {  
            // if (movedImmediateHorizontal) {
            //     if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
            //         buttonDownWaitTimerHorizontal += Time.deltaTime;
            //         return;
            //     }
            //     if (horizontalTimer < continuousHorizontalSpeed) {
            //         horizontalTimer += Time.deltaTime;
            //         return;
            //     }
            // }
            // if (!movedImmediateHorizontal) 
            //     movedImmediateHorizontal = true;
            // horizontalTimer = 0;
            ViewManager.nextTetromino.transform.position += new Vector3(0, 0, -1); 
            // FindObjectOfType<Game>().MoveZNeg(); // moveCanvas moves too

            if (CheckIsValidPosition()) {
                ViewManager.GameView.ViewModel.UpdateGrid(ViewManager.GameView.nextTetromino);
                AudioManager.Instance.PlayAudio("move");
            } else {
                ViewManager.nextTetromino.transform.position += new Vector3(0, 0, 1);
                // ViewManager.GameView.MoveZPos(); // moveCanvas moves too
            }
        }
        
        //public void MoveDelta(Vector3 delta) {     // 使用 _command_
        //    //     if (delta != Vector3.zero) {
        //    //         moveCommand = new MoveCommand(this, delta); // this: Tetromino as IEntity
        //    //         _commandProcessor.ExecuteCommand(moveCommand);
        //    //     }
        //}

        //public void RotateDelta(Vector3 delta) {
        //    // if (delta != Vector3.zero) {
        //    //     rotateCommand = new RotateCommand(this, delta); // this: Tetromino as IEntity
        //    //     _commandProcessor.ExecuteCommand(rotateCommand);
        //    // }
        //}
        
        // public void MoveXPos() { 
        //     if (movedImmediateHorizontal) {
        //         if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
        //             buttonDownWaitTimerHorizontal += Time.deltaTime;
        //             return;
        //         }
        //         if (horizontalTimer < continuousHorizontalSpeed) {
        //             horizontalTimer += Time.deltaTime;
        //             return;
        //         }
        //     }
        //     if (!movedImmediateHorizontal) 
        //         movedImmediateHorizontal = true;
        //     horizontalTimer = 0;
        //     //transform.position += new Vector3(1, 0, 0);
        // }
        // public void MoveXNeg() { 
        //     if (movedImmediateHorizontal) {
        //         if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
        //             buttonDownWaitTimerHorizontal += Time.deltaTime;
        //             return;
        //         }
        //         if (horizontalTimer < continuousHorizontalSpeed) {
        //             horizontalTimer += Time.deltaTime;
        //             return;
        //         }
        //     }
        //     if (!movedImmediateHorizontal) 
        //         movedImmediateHorizontal = true;
        //     horizontalTimer = 0;
        // }
        // public void MoveZPos() { 
        //     if (movedImmediateHorizontal) {
        //         if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
        //             buttonDownWaitTimerHorizontal += Time.deltaTime;
        //             return;
        //         }
        //         if (horizontalTimer < continuousHorizontalSpeed) {
        //             horizontalTimer += Time.deltaTime;
        //             return;
        //         }
        //     }
        //     if (!movedImmediateHorizontal) 
        //         movedImmediateHorizontal = true;
        //     horizontalTimer = 0;
        
        // }

        // public void RotateXPos() { 
        //     if (noRotationX) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.x >= 90) {
        //             transform.Rotate(-90, 0, 0);
        //             rotateMinoOnly(-90, 0, 0);
        //         } else {
        //             transform.Rotate(90, 0, 0);
        //             rotateMinoOnly(90, 0, 0);
        //         }
        //     } else {
        //         transform.Rotate(90, 0, 0);
        //         rotateMinoOnly(90, 0, 0);
        //     }
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.x >= 90) {
        //                 transform.Rotate(-90, 0, 0);
        //                 rotateMinoOnly(-90, 0, 0);
        //             } else {
        //                 transform.Rotate(90, 0, 0);
        //                 rotateMinoOnly(90, 0, 0);
        //             }
        //         } else {
        //             transform.Rotate(-90, 0, 0); // i changed here from 90 to -90 for fixing movement buttons
        //             rotateMinoOnly(-90, 0, 0);
        //         }
        //     }
        // }
        // public void RotateXNeg() { // X axis
        //     if (noRotationX) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.x >= 90) {
        //             transform.Rotate(90, 0, 0);
        //             rotateMinoOnly(90, 0, 0);
        //         } else {
        //             transform.Rotate(-90, 0, 0);
        //             rotateMinoOnly(-90, 0, 0);
        //         }
        //     } else {
        //         transform.Rotate(-90, 0, 0);
        //         rotateMinoOnly(-90, 0, 0);
        //     }
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.x >= 90) {
        //                 transform.Rotate(90, 0, 0);
        //                 rotateMinoOnly(90, 0, 0);
        //             } else {
        //                 transform.Rotate(-90, 0, 0);
        //                 rotateMinoOnly(-90, 0, 0);
        //             }
        //         } else {
        //             transform.Rotate(90, 0, 0); // -90
        //             rotateMinoOnly(90, 0, 0);
        //         }
        //     }
        // }
        // public void RotateYPos() { 
        //     if (noRotationY) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.y >= 90) {
        //             transform.Rotate(0, 90, 0);
        //             rotateMinoOnly(0, 90, 0);
        //         } else {
        //             transform.Rotate(0, -90, 0);
        //             rotateMinoOnly(0, -90, 0);
        //         }
        //     } else {
        //         transform.Rotate(0, -90, 0);
        //         rotateMinoOnly(0, -90, 0);
        //     }
		
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.y >= 90) {
        //                 transform.Rotate(0, 90, 0);
        //                 rotateMinoOnly(0, 90, 0);
        //             } else {
        //                 transform.Rotate(0, -90, 0);
        //                 rotateMinoOnly(0, -90, 0);
        //             }
        //         } else {
        //             transform.Rotate(0, 90, 0);  //-90
        //             rotateMinoOnly(0, 90, 0);
        //         }
        //     }
        // }
        // public void RotateYNeg() { 
        //     if (noRotationY) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.y >= 90) {
        //             transform.Rotate(0, -90, 0);
        //             rotateMinoOnly(0, -90, 0);
        //         } else {
        //             transform.Rotate(0, 90, 0);
        //             rotateMinoOnly(0, 90, 0);
        //         }
        //     } else {
        //         transform.Rotate(0, 90, 0);
        //         rotateMinoOnly(0, 90, 0);
        //     }
		
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.y >= 90) {
        //                 transform.Rotate(0, -90, 0);
        //                 rotateMinoOnly(0, -90, 0);
        //             } else {
        //                 transform.Rotate(0, 90, 0);
        //                 rotateMinoOnly(0, 90, 0);
        //             }
        //         } else {
        //             transform.Rotate(0, -90, 0); 
        //             rotateMinoOnly(0, -90, 0);
        //         }
        //     }
        // }
        // public void RotateZPos() { 
        //     if (noRotationZ) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.z >= 90) {
        //             transform.Rotate(0, 0, -90);
        //             rotateMinoOnly(0, 0, -90);
        //         } else {
        //             transform.Rotate(0, 0, 90);
        //             rotateMinoOnly(0, 0, 90);
        //         }
        //     } else {
        //         transform.Rotate(0, 0, 90);
        //         rotateMinoOnly(0, 0, 90);
        //     }
		
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.z >= 90) {
        //                 transform.Rotate(0, 0, -90);
        //                 rotateMinoOnly(0, 0, -90);
        //             } else {
        //                 transform.Rotate(0, 0, 90);
        //                 rotateMinoOnly(0, 0, 90);
        //             }
        //         } else {
        //             transform.Rotate(0, 0, -90); 
        //             rotateMinoOnly(0, 0, -90);
        //         }
        //     }
        // }
        // public void RotateZNeg() { // Z axis
        //     if (noRotationZ) return;
        //     if (limitRotation) {
        //         if (transform.rotation.eulerAngles.z >= 90) {
        //             transform.Rotate(0, 0, 90);
        //             rotateMinoOnly(0, 0, 90);
        //         } else {
        //             transform.Rotate(0, 0, -90);
        //             rotateMinoOnly(0, 0, -90);
        //         }
        //     } else {
        //         transform.Rotate(0, 0, -90);
        //         rotateMinoOnly(0, 0, -90);
        //     }
		
        //     if (CheckIsValidPosition()) {
        //         ViewManager.GameView.UpdateGrid(ViewManager.GameView.nextTetromino);
        //         PlayRotateAudio();
        //     } else { 
        //         if (limitRotation) {
        //             if (transform.rotation.eulerAngles.z >= 90) {
        //                 transform.Rotate(0, 0, 90);
        //                 rotateMinoOnly(0, 0, 90);
        //             } else {
        //                 transform.Rotate(0, 0, -90);
        //                 rotateMinoOnly(0, 0, -90);
        //             }
        //         } else {
        //             transform.Rotate(0, 0, 90); // -90
        //             rotateMinoOnly(0, 0, 90);
        //         }
        //     }
        // }

        // IEnumerator MyUpdate() {
        //     while (true) {
        //         yield return new WaitForSeconds(2.0f * fallSpeed);

        //         Debug.Log("Tetromino MyUpdate: ");
        //         if (!ViewManager.GameView.ViewModel.isPaused) {
        //             CheckUserInput();
        //         }
        //     }
        // }     
    }
}