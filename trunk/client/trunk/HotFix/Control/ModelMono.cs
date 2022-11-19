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
                        if (GloData.Instance.gameMode.Value > 0 || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel.Value < 3))
                            ViewManager.GameView.ViewModel.currentScore.Value += GloData.Instance.layerScore;
                        else
                            ViewManager.GameView.ViewModel.currentScore.Value += GloData.Instance.challengeLayerScore;
                        
                        hasDeletedMinos = true;
                        // Debug.Log(TAG + " hasDeletedMinos: " + hasDeletedMinos);
                    }
                }
                if (visDeleteJ != -1) { // 如果这次遍历里最低是在visDeleteJ 层有消除,那么从这层再开始下移和遍历
// TODO:　这里的逻辑不合情理: 启蒙模式下,即便规则 许可 同行 同列 同对角线 如果满了,可是消除它们.但是消除后,对于他们上面的3 4 5 个小立方体,并不是每个都可以下落,仍受小立方体父控件的规则约束,未必一定下落                    
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
            // Debug.Log(TAG + ": DeleteRowCoroutine()");
            isDeleteRowCoroutineRunning = true;
            int visDeleteJ = -1, j = 0;

            for (int y = 0; y < Model.gridHeight; y++) { // 仍然是从 最底层 往上层 process, 但是每层的时候都同时处理多个层,而不是永远只先处理最底层,这不合理
                // Debug.Log(TAG + " DeleteRowCoroutine() y: " + y);
                visDeleteJ = -1;                         
                for (j = y; j < Model.gridHeight; j++) { // 中间同时处理多层 12 层
                    if ( (!GloData.Instance.isChallengeMode && GloData.Instance.gameMode.Value == 0 && Model.IsFullFiveInLayerAt(j))
                         || ((GloData.Instance.isChallengeMode || GloData.Instance.gameMode.Value == 1) && Model.isFullInLayerAt(j)) ) { 
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
                            ViewManager.GameView.ViewModel.numLinesCleared.Value += Model.numberOfRowsThisTurn;
                            ViewManager.GameView.ViewModel.currentScore.Value += GloData.Instance.layerScore; // 分数再优化一下
                            if (!isDeleteMNinoAtCoroutineRunning) 
                                deleteMinoAtCoroutine = CoroutineHelperP.StartCoroutine(DeleteMinoAtCoroutine(j)); // commented for tmp
                            yield return deleteMinoAtCoroutine;                        
                        }

                        hasDeletedMinos = true;
                    }
                }
                Debug.Log(TAG + " visDeleteJ: " + visDeleteJ);
                if (visDeleteJ != -1) { // 如果这次遍历里最低是在visDeleteJ 层有消除,那么从这层再开始下移和遍历
// 根据现在的游戏逻辑定义,当一个方块砖遭受过外力摧毁时,容易变形,变得子立方体可受重力下落,那么不能跳过 当前层, 以及                    
                    // MoveAllRowsDown(visDeleteJ + 1);
                    int redoLayer = visDeleteJ == 0 ? visDeleteJ+1 : Math.Max(visDeleteJ-2, 1);
                    MoveAllRowsDown(redoLayer);
                    y = redoLayer - 2;
                }
                // else break; // 可以直接这里退出吗,还有残余的 2吗?
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
                        Model.gridOcc[x][y][z] = 0; // <<<<<<<<<<<<<<<<<<<< 
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
            int gameMode = GloData.Instance.gameMode.Value;
            Debug.Log(TAG + ": DeleteMinoAt() start");
            for (int x = 0; x < Model.gridXWidth; x++) {
                for (int  z = 0;  z < Model.gridZWidth;  z++) {
// GloData.Instance.gameMode > 0, 进行必要的回收, TODO: 只回收同一层的mino or Tetromino (以前标注的)
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
                                    Model.gridOcc[i][j][k] = 0; 
                                    if (y > 0)
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
                                Model.gridOcc[x][y][z] = 0; // 0 ==> 2
                                if (y > 0) // y == 0 最底层的不要变 
                                    Model.gridClr[x][y][z] = -1;
                            }
                            if (tmpParentTransform.childCount == 0 && tmpParentTransform.gameObject != null)
                                GameObject.Destroy(tmpParentTransform.gameObject);
                        }
                    }
                }
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
            for (int i = y; i < Model.gridHeight; i++) 
                MoveRowDown(i);
            Debug.Log(TAG + ": MoveAllRowsDown() AFTER having moved all row down from layer " + y); 
            MathUtilP.printBoard(Model.gridOcc);
        }
        
// 这里修个BUg: 说当一个方块砖因为内部其它立方体的原因被悬空挂在什么地方;当当前立方体被悬空的其它立方体消除后,当前立方体需要能够自由落体下降; 这块逻辑需要补上
        public static void MoveRowDown(int y) {
            // Debug.Log(TAG + " MoveRowDown() y: " + y);
            int cnt = 0;
// CLASSIC MODE 和挑战模式下整片消除的            
            if (GloData.Instance.gameMode.Value > 0) { // CLASSIC 模式下 
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
                return ;
            } // GloData.Instance.gameMode == 0: 启蒙模式，挑战模式，或是带有undo功能的模式下
// LOGIC MODULE INSERT:
            // 每个 方块砖 有个固定的形状,现逻辑定义为 当且仅当方块砖遭受外外力摧毁,至少消除过一个子立方体之后,其内部结构有可能变形,某个子立方体下落一格;
            // 昨天晚上开始意识到这个问题.目前的定义为符合三维游戏的定义,只是感觉对于小盆友来说,难度太大了点儿,考虑是否回退原实现?暂时保留如些
// 当当前立方体仅且只剩自已(BUG: 只要有消除[或是不曾消除,但是下面一格有消除]就需要检查是否可以继续下落,哪怕只消除了一个立方体,还剩 七 个小立方体),检查是否可以下落
// 是否,在前面有过消除的前提下,前再检查一遍,还是说之后消除呢?
            for (int x = 0; x < Model.gridXWidth; x++) { 
                for (int z = 0; z < Model.gridZWidth; z++) {
                    // if (Model.gridOcc[x][y-1][z] == 2) { // 如果下面一层是需要消除或是曾经消除过的空格,[并非一定能够]将当前非空立方体下移,当前立方体下降与否受其父控件约束
                    if (Model.gridOcc[x][y-1][z] == 2 && Model.gridOcc[x][y][z] == 0) { // 如果下面一层是需要消除或是曾经消除过的空格,[并非一定能够]将当前非空立方体下移,当前立方体下降与否受其父控件约束
                        Model.gridOcc[x][y-1][z] = Model.gridOcc[x][y][z];
                        // if (Model.grid[x][y][z] != null) 
                        //     moveMinoDownOneGridHelper(x,  y, z);
                        // Model.gridOcc[x][y][z] = y == Model.gridHeight - 1 ? 0 : 2;
                        Model.gridOcc[x][y][z] = 0;
                    } else
                        // if (Model.gridOcc[x][y-1][z] == 0 && Model.gridOcc[x][y][z] == 1 // 下一格为 空,上层当前格为 非空,检查是否可以掉落
// 如果以这种逻辑,那么每移动过的/ 下除过的当前格,都仍需要标记以示消除或是移动过,给上层立方体看的                        
                        if ((Model.gridOcc[x][y-1][z] == 0 || Model.gridOcc[x][y-1][z] == 2) // 下面一格为空或是消除过
                            && Model.gridOcc[x][y][z] == 1 && Model.grid[x][y][z] != null    // 下一格为 空,上层当前格为 非空,检查是否可以掉落 Model.gridOcc[x][y-1][z] == 8 ?
                            // && canFallDeeper(Model.grid[x][y][z].parent)) { // 不能以整个父控件为单位,要以各个子立方体为单位.因为父控件不能下移时,其内小立方体仍是可以下移的
                            ) {
                            if (canFallDeeper(Model.grid[x][y][z], x, y, z)) {
                                MathUtilP.print(x, y, z);

                                if (Model.grid[x][y][z].parent != null) {
                                    Debug.Log(TAG + " Model.grid[x][y][z].parent.gameObject.name: " + Model.grid[x][y][z].parent.gameObject.name);
                                    Debug.Log(TAG + " (Model.grid[x][y][z].parent == null): " + (Model.grid[x][y][z].parent == null));
                                }

                                // if (Model.grid[x][y][z].parent == null) { // 独个立方体,只将当前格下移
                                // 如果父方块砖遭受过外力摧毁,可以无形滩软降落,否则受父方块砖约束,要检查(逻辑这么设定,可以好玩吗?)
                                if (Model.grid[x][y][z].parent == null || Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().childCnt != Model.grid[x][y][z].parent.childCount) { // 独个立方体,只将当前格下移
                                    // MathUtilP.print("(Model.grid[x][y][z].parent == null)", x, y, z);
                                    moveMinoDownOneGridHelper(x,  y, z);
                                } else if (Model.grid[x][y][z].parent != null) { // 当将格的父控件可以继续下降一格:
                                    Model.grid[x][y][z].parent.position += new Vector3(0, -1, 0);
                                    Model.UpdateGrid(Model.grid[x][y][z].parent.gameObject);
                                }

                                // Debug.Log(TAG + " MoveRowDown(): AFTER (canFallDeeper(Model.grid[x][y][z], x, y, z))");
                                // MathUtilP.printBoard(Model.gridOcc);
                            }
                        }
                }   
            }
            if (y == 1 && BaseBoardSkin.isSkinChanged) {
                Debug.Log(TAG + " MoveRowDown() (y == 1 && BaseBoardSkin.isSkinChanged)");
                MathUtilP.printBoard(Model.baseCubes);
                EventManager.Instance.FireEvent("cubesMat");
            } // debug
        }
// 标记下降过的当前格为 2        
        private static void moveMinoDownOneGridHelper(int x, int y, int z) {
            Model.grid[x][y-1][z] = Model.grid[x][y][z]; // transform
            Model.grid[x][y][z] = null;
            Model.grid[x][y-1][z].position += new Vector3(0, -1, 0);
            Model.gridOcc[x][y-1][z] = Model.gridOcc[x][y][z]; // 值也要更新
            if (GloData.Instance.isChallengeMode) {
                Model.gridClr[x][y-1][z] = Model.gridClr[x][y][z];
                if (y == 1) { // still need to update baseboard skin accordingly
                    Model.baseCubes[x + z * Model.gridXWidth] = Model.gridClr[x][y][z];
                    BaseBoardSkin.isSkinChanged = true;
                }
            }
            Model.gridOcc[x][y][z] = 0;
            Model.gridClr[x][y][z] = -1;
        }

        // 补充逻辑: 添加检查有过消除后的方块砖是否可以继续下降一格? 仿Tetromino MoveDown()的方法来写        
        private static bool canFallDeeper(Transform p, int i, int j, int k) { // can current parent Transform fall down 1 more layer ?
            if (p.parent == null) return true; // 如果它是独子,下一格为空,就降一格
            Vector3 pos = new Vector3(i, j-1, k);
            if (!Model.CheckIsInsideGrid(pos)) return false;  // 落到格外不行
            if (Model.GetTransformAtGridPosition(pos) != null // 0 || 2 当前格是可以降落的
                && Model.GetTransformAtGridPosition(pos).parent != p.parent) {
                return false;
            }
            // 如果父方块砖遭受过外力摧毁,可以无形滩软降落,否则维护父方块砖的形状,要检查以方块砖为单位,能否下移一格
            if (p.parent.gameObject != null && p.parent.gameObject.GetComponent<TetrominoType>().childCnt == p.parent.childCount) {
                foreach (Transform mino in p.parent) {
                    if (mino.CompareTag("mino")) {
                        Vector3 pp = MathUtilP.Round(mino.position) + new Vector3(0, -1, 0);
                        if (!Model.CheckIsInsideGrid(pp)) return false;  // 落到格外不行
                        if (Model.GetTransformAtGridPosition(pp) != null // 0 || 2 当前格是可以降落的
                            && Model.GetTransformAtGridPosition(pp).parent != p.parent) {
                            return false;
                        }
                    }
                }
            }
            Debug.Log(TAG + " canFallDeeper(): TRUE ");
            return true;
        }
        public void OnEnable() {
            // Debug.Log(TAG + ": OnEnable()"); 
            Start();
        }
        public void Start() {
            // Debug.Log(TAG + " Start()");
        }
        void OnDisable() {
            // Debug.Log(TAG + ": OnDisable()");
        }
    }
}

