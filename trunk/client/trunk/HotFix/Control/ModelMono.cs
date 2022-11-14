using deepwaterooo.tetris3d;
using Framework.Util;
using HotFix.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public class ModelMono : SingletonMono<ModelMono> {
        private const string TAG = "ModelMono";

        public delegate void UpdateScoreDelegate();
        public static event UpdateScoreDelegate updateScoreEvent;

        public static bool hasDeletedMinos = false; 
        public static bool isDeleteRowCoroutineRunning = false;
        public static bool isDeleteMNinoAtCoroutineRunning = false;

        private static StringBuilder type = new StringBuilder("");
        private static List<GameObject> minoPSList = new List<GameObject>(); 
        private static Coroutine deleteMinoAtCoroutine;

        private static float connectionEffectDisplayTime = 2.0f; // ori: 1.1f
        private static WaitForSeconds _waitForSeconds = new WaitForSeconds(connectionEffectDisplayTime);
        private CanvasMovedEventInfo canvasMovedInfo;
        
        public static void DeleteRow() { // 算法上仍然需要优化
            Debug.Log(TAG + ": DeleteRow() start");
            hasDeletedMinos = false; 
            bool isFullRowAtY = false;
            int visDeleteJ = -1;

            for (int y = 0; y < Model.gridHeight; y++) {
                visDeleteJ = -1;

                for (int j = y; j < Model.gridHeight; j++) { // 中间同时处理多层 12 层

                    isFullRowAtY = Model.IsFullRowAt(j);
                    Debug.Log(TAG + " isFullRowAtY: " + isFullRowAtY); 
                    // if (IsFullRowAt(j)) {
                    if (isFullRowAtY) {
                        if (visDeleteJ == -1)
                            visDeleteJ = j;

                        if (GloData.Instance.isChallengeMode && j == 0) // isChallengeMode 有撤销功能
                            markLayerIn(j);

                        Debug.Log(TAG + ": gridOcc[,,] IsFullFiveInLayerAt(j)_EDU || isFullInLayerAt(j)_OTHER"); 
                        MathUtilP.printBoard(Model.gridOcc); 

                        DeleteMinoAt(j);
                        if (GloData.Instance.gameMode > 0 || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel < 3))
                            ScoreManager.currentScore += GloData.Instance.layerScore;
                        else
                            ScoreManager.currentScore += GloData.Instance.challengeLayerScore;
                        
                        hasDeletedMinos = true;
                        // Debug.Log(TAG + " hasDeletedMinos: " + hasDeletedMinos);
                    }
                }
                if (visDeleteJ != -1) { // 如果这次遍历里最低是在visDeleteJ 层有消除,那么从这层再开始下移和遍历
                    MoveAllRowsDown(visDeleteJ + 1);
                    y = visDeleteJ - 1;
                } else break; // 可以直接这里退出吗,还有残余的 2吗?
            }

            if (BaseBoardSkin.isSkinChanged) { // debugging
                Debug.Log(TAG + ": Model.gridOcc[,,] aft DeleteRow done"); 
                MathUtilP.printBoard(Model.gridOcc); 
            }
        }
        
        public System.Collections.IEnumerator DeleteRowCoroutine() {
            Debug.Log(TAG + ": DeleteRowCoroutine()");
            isDeleteRowCoroutineRunning = true;
            int visDeleteJ = -1;

            for (int y = 0; y < Model.gridHeight; y++) { // 仍然是从 最底层 往上层 process, 但是每层的时候都同时处理多个层,而不是永远只先处理最底层,这不合理
                // Debug.Log(TAG + " DeleteRowCoroutine() y: " + y);
                visDeleteJ = -1;

                for (int j = y; j < Model.gridHeight; j++) { // 中间同时处理多层 12 层
                    // Debug.Log(TAG + "  DeleteRowCoroutine() j: " + j);

                    if ( (!GloData.Instance.isChallengeMode && GloData.Instance.gameMode == 0 && Model.IsFullFiveInLayerAt(j))
                         || ((GloData.Instance.isChallengeMode || GloData.Instance.gameMode == 1) && Model.isFullInLayerAt(j)) ) { 
                        if (visDeleteJ == -1)
                            visDeleteJ = j;

                        Debug.Log(TAG + ": gridOcc[,,] IsFullFiveInLayerAt(j)_EDU || isFullInLayerAt(j)_OTHER"); 
                        MathUtilP.printBoard(Model.gridOcc); 

                        Model.isNumberOfRowsThisTurnUpdated = false;

                        if (GloData.Instance.isChallengeMode) {
                            DeleteMinoAt(j);
                            ViewManager.GameView.ViewModel.currentScore.Value += GloData.Instance.challengeLayerScore; // 分数再优化一下
                            yield return null;
                        } else {
                            ViewManager.GameView.ViewModel.currentScore.Value += GloData.Instance.layerScore; // 分数再优化一下
                            if (!isDeleteMNinoAtCoroutineRunning) 
                                deleteMinoAtCoroutine = CoroutineHelperP.StartCoroutine(DeleteMinoAtCoroutine(j)); // commented for tmp
                            yield return deleteMinoAtCoroutine;                        
                        }

                        hasDeletedMinos = true;
                    }
                }
                if (visDeleteJ != -1) { // 如果这次遍历里最低是在visDeleteJ 层有消除,那么从这层再开始下移和遍历
                    MoveAllRowsDown(visDeleteJ + 1);
                    y = visDeleteJ - 1;
                } else break; // 可以直接这里退出吗,还有残余的 2吗?
            }

            Debug.Log(TAG + " hasDeletedMinos: " + hasDeletedMinos); 
            if (hasDeletedMinos) { // clean top layer 2 out: all 2s to be 0s
                for (int o = Model.gridHeight - 1; o >= 0; o--) {
                    for (int x = 0; x < Model.gridXWidth; x++) {
                        for (int z = 0; z < Model.gridZWidth; z++) {
                            if (Model.gridOcc[x][o][z] == 2) {
                                if (o == Model.gridHeight - 1 ||  Model.gridOcc[x][o+1][z] == 0) 
                                    Model.gridOcc[x][o][z] = 0;
                            }
                        }
                    }
                } 
            }
            Debug.Log(TAG + ": Model.gridOcc[][][] AFTER DeleteRowCoroutine()"); 
            MathUtilP.printBoard(Model.gridOcc);

            isDeleteRowCoroutineRunning = false;
            // yield return null;
        }

// 这里是要求要被消除的小立方体被标记为 2        
// TODO: 这里的逻辑还有很多问题,估计后来还会有很多BUG在这里.但暂时改到这里,等熟悉游戏和源码后会更好改
// TODO:　当有两层底层需要同时消除的时候,有一个BUG这里两行都没有消除        
        public static System.Collections.IEnumerator DeleteMinoAtCoroutine(int y) { 
            // Debug.Log(TAG + ": DeleteMinoAtCoroutine() start");
            isDeleteMNinoAtCoroutineRunning = true;
            
            if (minoPSList.Count > 0)
                minoPSList.Clear();
// 这里只是想要展示有这些方格需要消除,但仍然还没有消息
            yield return CoroutineHelperP.StartCoroutine(displayConnectedParticleEffects(y));
            
            for (int x = 0; x < Model.gridXWidth; x++) {
                for (int  z = 0;  z < Model.gridZWidth;  z++) {
                    if (Model.gridOcc != null && Model.gridOcc[x][y][z] == 2) {
                        // Debug.Log("(x,y,z): [" + x + "," + y + "," + z +"]: " + Model.gridOcc[x][y][z]); 
                        if (Model.grid[x][y][z] != null && Model.grid[x][y][z].gameObject != null) { // 一定要
                            if (Model.grid[x][y][z].parent != null) {
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.name: " + Model.grid[x][y][z].parent.name);
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.childCount: " + Model.grid[x][y][z].parent.childCount);
                                
                                if (Model.grid[x][y][z].parent.childCount == 1) {
                                    Transform tmp = Model.grid[x][y][z].parent;
                                    type.Length = 0;
                                    tmp.GetChild(0).parent = null;
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    GameObject.Destroy(tmp.gameObject);
                                    Model.grid[x][y][z] = null;
                                    tmp = null;
                                } else { // childCount > 1
                                    type.Length = 0;
                                    Model.grid[x][y][z].parent = null;
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    Model.grid[x][y][z] = null;
                                    Debug.Log(TAG + " (Model.grid[x][y][z] == null): " + (Model.grid[x][y][z] == null));
                                }
                            } else { // (Model.grid[x][y][z].parent == null)
                                GameObject.Destroy(Model.grid[x][y][z]);
                                Model.grid[x][y][z] = null;
                            }
                        }
                    }
                }
            }
            isDeleteMNinoAtCoroutineRunning = false;
            yield return null;
        }

		static System.Collections.IEnumerator displayConnectedParticleEffects (int y) {
            Debug.Log(TAG + ": displayConnectedParticleEffects()"); 
            for (int x = 0; x < Model.gridXWidth; x++) 
                for (int  z = 0;  z < Model.gridZWidth;  z++)
                    if (Model.gridOcc != null && Model.gridOcc[x][y][z] == 2) {
                        if (Model.grid[x][y][z] != null && Model.grid[x][y][z].childCount == 0) {
                            MathUtilP.print(x, y, z);
                            GameObject connectedEffectTmp = PoolHelper.GetFromPool("minoPS", new Vector3(x, y, z), Quaternion.identity, Vector3.one);
                            connectedEffectTmp.transform.SetParent(ViewManager.tetroParent.transform, false);
                            connectedEffectTmp.transform.rotation = Quaternion.identity; // 运行时它仍然有微小转动
                            MathUtilP.print(connectedEffectTmp.transform.position);
                            minoPSList.Add(connectedEffectTmp);
                        }
                    }
// TODO: 感觉这里还有点儿不通,时间不够长,粒子系统展示得不完整: 这里仍然不工作,得想个办法
            // yield return new WaitForSeconds(3);
            // yield return CoroutineHelper.WaitForOneSecond;
            // yield return GameObject.FindChild  IEnumeratorTools.WaitForOneSecond;
            yield return _waitForSeconds;
            
            // Debug.Log(TAG + "  minoPSList.Count: " +  minoPSList.Count);
            for (int i = minoPSList.Count - 1; i >= 0; i--) {
                minoPSList[i].transform.parent = null;
                PoolHelper.ReturnToPool(minoPSList[i], "minoPS");
                minoPSList.RemoveAt(i);
            }
            // Debug.Log(TAG + "  minoPSList.Count: " +  minoPSList.Count);
            yield return null;
        }

        static void markLayerIn(int y) {
            for (int i = 0; i < Model.gridXWidth; i++) 
                for (int j = 0; j < Model.gridZWidth; j++)
                    if (Model.gridOcc[i][y][j] == 1) // Model.grid[i][y][j] = 8 InitCubes 保留不变
						Model.gridOcc[i][y][j] = 2;
        }

// gameMode == 1 经典模式:
// gameMode == 0 && isChallengeMode : 标记2并需要能够undo        
        public static void DeleteMinoAt(int y) {
            int gameMode = GloData.Instance.gameMode;
            Debug.Log(TAG + ": DeleteMinoAt() start");
            for (int x = 0; x < Model.gridXWidth; x++) {
                for (int  z = 0;  z < Model.gridZWidth;  z++) {
// GloData.Instance.gameMode > 0, 进行必要的回收, TODO: 只回收同一层的mino or Tetromino (以前标注的)
                    // if (gameMode > 0 || (GloData.Instance.gameMode == 0 && GloData.Instance.isChallengeMode)) { // 这个条件这里不要了 ?
                    // if (Model.gridOcc[x][y][z] == 1 && Model.grid[x][y][z] != null) {
                    if (Model.grid[x][y][z] != null
                        && (gameMode > 0 && Model.gridOcc[x][y][z] == 1 || gameMode == 0 && Model.gridOcc[x][y][z] == 2)) { // 这些不能消除 Model.grid[x][y][z] = 8
                        if (Model.grid[x][y][z].gameObject != null && Model.grid[x][y][z].parent != null && Model.grid[x][y][z].parent.gameObject != null
                            && Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().childCnt == Model.grid[x][y][z].parent.childCount // 父控件是完整的
                            && isAllMinoInLayerY(Model.grid[x][y][z].parent, y)) { // 这个小方格的 父控件 的所有子立方体全部都在这一y层,就是,可以直接回收到资源池
                            Transform tmpParentTransform = Model.grid[x][y][z].parent;
                            foreach (Transform mino in Model.grid[x][y][z].parent) {
                                int i = (int)Mathf.Round(mino.position.x);
                                int j = (int)Mathf.Round(mino.position.y);
                                int k = (int)Mathf.Round(mino.position.z);
                                Model.grid[i][j][k] = null;
                                if (GloData.Instance.isChallengeMode) {
                                    Model.gridOcc[i][j][k] = (y == Model.gridHeight-1 ? 0 : 2); // 0 ==> 2
                                    Model.gridClr[i][j][k] = -1;
                                }
                            }
                            PoolHelper.ReturnToPool(tmpParentTransform.gameObject, tmpParentTransform.gameObject.GetComponent<TetrominoType>().type);
// 当前立方体的 父控件 的某个或某些子立方体不在当前层,仅只回收当前小立方体到资源池
// TODO: 这里仍然要区分是否有父控件,是否为父控件的最后一个立方体,因为它负有消除父控件的责任.要不然,残余一堆父控件在那里
                        } else {
                            Transform tmpParentTransform = Model.grid[x][y][z].parent;
                            Model.grid[x][y][z].parent = null; // 需要先解除这个父子控件关系,否则父控件永远以为这个子立方体存在存活
                            PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                            Model.grid[x][y][z] = null;
                            if (GloData.Instance.isChallengeMode) {
                                Model.gridOcc[x][y][z] = (y == Model.gridHeight-1 ? 0 : 2); // 0 ==> 2
                                Model.gridClr[x][y][z] = -1;
                            }
                            if (tmpParentTransform.childCount == 0 && tmpParentTransform.gameObject != null)
                                GameObject.Destroy(tmpParentTransform.gameObject);
                        }
                    }
                }
                // }
            }
        }

        private static bool isAllMinoInLayerY(Transform parent, int y) {
            foreach (Transform mino in parent) {
                if (Mathf.Round(mino.position.y) != y)
                    return false;
            }
            return true;
        }
        public static void MoveAllRowsDown(int y) {
            Debug.Log(TAG + ": MoveAllRowsDown()"); 
            for (int i = y; i < Model.gridHeight; i++) 
                MoveRowDown(i);
            MathUtilP.printBoard(Model.gridOcc);
        }
        
// 这里修个BUg: 说当一个方块砖因为内部其它立方体的原因被悬空挂在什么地方;当当前立方体被悬空的其它立方体消除后,当前立方体需要能够自由落体下降; 这块逻辑需要补上
        public static void MoveRowDown(int y) {
            // Debug.Log(TAG + ": MoveRowDown()");
            int cnt = 0;
// CLASSIC MODE 和挑战模式下整片消除的            
            // if (GloData.Instance.gameMode > 0 ||
            //     (GloData.Instance.isChallengeMode &&
            //      (GloData.Instance.challengeLevel < 3 || GloData.Instance.challengeLevel > 5))) { // challenge mode level 1 2 6 7 8 9 10
            if (GloData.Instance.gameMode > 0) { // CLASSIC 模式下 
                for (int j = 0; j < Model.gridZWidth; j++) {
                    for (int x = 0; x < Model.gridXWidth; x++) {
                        if (Model.grid[x][y][j] != null) {
                            Model.grid[x][y-1][j] = Model.grid[x][y][j];
                            Model.grid[x][y-1][j].position += new Vector3(0, -1, 0);
                            Model.gridOcc[x][y-1][j] = Model.gridOcc[x][y][j];　// BUG　TODO这里对于被压下来的上一层如何处理没有标明
                            Model.gridOcc[x][y][j] = 0; // 我现加的，最主要是考虑最顶层的会造成什么BUG
                            Model.grid[x][y][j] = null;
                        }
                    }
                }
            } else { // GloData.Instance.gameMode == 0: 启蒙模式，挑战模式，或是带有undo功能的模式下
// LOGIC MODULE INSERT              
// 当当前立方体仅且只剩自已(BUG: 只要有消除就需要检查是否可以继续下落,哪怕只消除了一个立方体,还剩 七 个小立方体,先前想到的逻辑不完整),检查是否可以下落
// 是否,在前面有过消除的前提下,前再检查一遍,还是说之后消除呢?
                for (int x = 0; x < Model.gridXWidth; x++) {
                    for (int z = 0; z < Model.gridZWidth; z++) {
                        if (Model.gridOcc[x][y-1][z] == 2) { // 如果下面一层是需要消除或是曾经消除过的空格,将当前非空立方体下移
                            Model.gridOcc[x][y-1][z] = Model.gridOcc[x][y][z];
                            if (Model.grid[x][y][z] != null) 
                                moveMinoDownOneGridHelper(x,  y, z);
                            Model.gridOcc[x][y][z] = y == Model.gridHeight - 1 ? 0 : 2;
                        }
                        else if (Model.gridOcc[x][y-1][z] == 0 && Model.gridOcc[x][y][z] == 1 // 下一格为 空,上层当前格为 非空,检查是否可以掉落
                                 && Model.grid[x][y][z] != null && canFallDeeper(Model.grid[x][y][z].parent)) {

                            Debug.Log(TAG + " Model.grid[x][y][z].parent.gameObject.name: " + Model.grid[x][y][z].parent.gameObject.name);
                            Debug.Log(TAG + " (Model.grid[x][y][z].parent == null): " + (Model.grid[x][y][z].parent == null));

                            if (Model.grid[x][y][z].parent == null) { // 独个立方体,只将当前格下移
                                MathUtilP.print("(Model.grid[x][y][z].parent == null)", x, y, z);
                                moveMinoDownOneGridHelper(x,  y, z);
                            } else if (Model.grid[x][y][z].parent != null) { // 当将格的父控件可以继续下降一格:
// TODO: BUG: 这里出过报空的BUG                                
                                // 这里应该可以换个方法(以父控件整体来)写,但是暂时如此吧, 感觉下面这个:把父控件里的每个立方体住下移动一格,既低效又bug百出
                                foreach (Transform mino in Model.grid[x][y][z].parent) {
                                    if (mino.CompareTag("mino")) {
                                        Vector3 pos = MathUtilP.Round(mino.position);
                                        int i = (int)Math.Round(mino.position.x);
                                        int j = (int)Math.Round(mino.position.y);
                                        int k = (int)Math.Round(mino.position.z);
                                        MathUtilP.print("(Model.grid[x][y][z].parent != null)", i, j, k);
                                        moveMinoDownOneGridHelper(i, j, k);
                                    }
                                }
                                // Model.grid[x][y][z].parent.position += new Vector3(0, -1, 0);
                                // Model.UpdateGrid(Model.grid[x][y][z].parent.gameObject);
                            }
                        }
                    }   
                }
            } // GloData.Instance.gameMode == 0
            if (y == 1 && BaseBoardSkin.isSkinChanged)  // debug
                EventManager.Instance.FireEvent("cubesMat");
        }
        
        private static void moveMinoDownOneGridHelper(int x, int y, int z) {
            Model.grid[x][y-1][z] = Model.grid[x][y][z];
            Model.grid[x][y][z] = null;
            Model.grid[x][y-1][z].position += new Vector3(0, -1, 0);
            if (GloData.Instance.isChallengeMode) {
                Model.gridClr[x][y-1][z] = Model.gridClr[x][y][z];
                if (y == 1) { // still need to update baseboard skin accordingly
                    Model.baseCubes[x + z * Model.gridXWidth] = Model.gridClr[x][y][z];
                    BaseBoardSkin.isSkinChanged = true;
                }
            }
            Model.grid[x][y][z] = null;
        }

        // 补充逻辑: 添加检查有过消除后的方块砖是否可以继续下降一格? 仿Tetromino MoveDown()的方法来写        
        private static bool canFallDeeper(Transform p) { // can current parent Transform fall down 1 more layer ?
            if (p == null) return true;
            foreach (Transform mino in p) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtilP.Round(mino.position);
                    pos += new Vector3(0, -1, 0);
                    if (!Model.CheckIsInsideGrid(pos)) return false;  // 落到格外不行
                    if (Model.GetTransformAtGridPosition(pos) != null // 0 || 2 当前格是可以降落的
                        && Model.GetTransformAtGridPosition(pos).parent != p) {
                        return false;
                    }
                }
            }
            Debug.Log(TAG + " canFallDeeper() : TRUE");
            return true;
        }
        
        public void OnEnable() {
            // Debug.Log(TAG + ": OnEnable()"); 
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            Start();
        }
        public void Start() {
            // Debug.Log(TAG + " Start()");
            // EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
        }
        void OnDisable() {
            // Debug.Log(TAG + ": OnDisable()");
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            // if (EventManager.Instance != null) {
            //     EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            // }
        }
        // public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
        //     Debug.Log(TAG + " onActiveTetrominoLand");
        //     lastTetroIndiScore = ComponentHelper.GetTetroComponent(info.unitGO).score;
        // }
        // public void onActiveTetrominoMove(TetrominoMoveEventInfo info) { 4
        //     Debug.Log(TAG + ": onActiveTetrominoMove()");
        //     ViewManager.nextTetromino.transform.position += info.delta; // -11 0 -11, 0 -11 0
        //     if (Model.CheckIsValidPosition()) {
        //         if (canvasMovedInfo == null) {
        //             canvasMovedInfo = new CanvasMovedEventInfo();
        //         }
        //         canvasMovedInfo.delta = info.delta;
        //         EventManager.Instance.FireEvent(canvasMovedInfo);
        //         Model.UpdateGrid(ViewManager.nextTetromino);
        //     } else {
        //         ViewManager.nextTetromino.transform.position -= info.delta;
        //     }
        // }
        // public void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
        //     Debug.Log(TAG + ": onActiveTetrominoRotate()");
        //     ViewManager.nextTetromino.transform.Rotate(info.delta);
        //     if (Model.CheckIsValidPosition()) {
        //         Model.UpdateGrid(ViewManager.nextTetromino); 
        //     } else
        //         ViewManager.nextTetromino.transform.Rotate(Vector3.zero - info.delta); 
        // }
    }
}
