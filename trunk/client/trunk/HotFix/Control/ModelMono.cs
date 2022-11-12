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
        
        public System.Collections.IEnumerator DeleteRowCoroutine() {
            Debug.Log(TAG + ": DeleteRowCoroutine()");
            isDeleteRowCoroutineRunning = true;

            for (int y = 0; y < Model.gridHeight; y++) {

                if ( (!GloData.Instance.isChallengeMode && GloData.Instance.gameMode == 0 && Model.IsFullFiveInLayerAt(y))
                     || ((GloData.Instance.isChallengeMode || GloData.Instance.gameMode == 1) && Model.isFullInLayerAt(y)) ) { 
                    // || (GloData.Instance.isChallengeMode && Model.IsFullQuadInLayerAt(y)) ) { // 以前的分四个小区的

                    Debug.Log(TAG + ": gridOcc[,,] IsFullQuadInLayerAt(y)"); 
                    MathUtilP.printBoard(Model.gridOcc); 

                    Model.isNumberOfRowsThisTurnUpdated = false;
                    // updateScoreEvent(); // commended now

// 这里不再细化这么四个分区了,就按最传统原始的一层一层地消除,每层的分数加多
// 系统逻辑: 这里是不是得把1 ==>　2？ 暂时不考虑这些 

                    if (GloData.Instance.isChallengeMode) {
                        DeleteMinoAt(y);
                        ScoreManager.currentScore += GloData.Instance.challengeLayerScore; // 分数再优化一下
                        yield return null;
                    } else {
                        ScoreManager.currentScore += GloData.Instance.layerScore; // 分数再优化一下
                        if (!isDeleteMNinoAtCoroutineRunning) 
                            deleteMinoAtCoroutine = CoroutineHelperP.StartCoroutine(DeleteMinoAtCoroutine(y)); // commented for tmp
                        yield return deleteMinoAtCoroutine;                        
                    }
// 下面是以前的分小区的不完整未完成的代码
                    // if (GloData.Instance.isChallengeMode && Model.zoneSum == 4) { 
                    //     DeleteMinoAt(y);
                    //     ScoreManager.currentScore += GloData.Instance.challengeLayerScore; // 分数再优化一下
                    //     yield return null;
                    // } else {
                    //     ScoreManager.currentScore += GloData.Instance.layerScore; // 分数再优化一下
                    //     if (!isDeleteMNinoAtCoroutineRunning) 
                    //         deleteMinoAtCoroutine = CoroutineHelperP.StartCoroutine(DeleteMinoAtCoroutine(y)); // commented for tmp
                    //     yield return deleteMinoAtCoroutine;                        
                    // }

                    MoveAllRowsDown(y + 1);
                    --y;

                    hasDeletedMinos = true;
                }
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
//                                     if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>() == null) {
//                                         Model.grid[x][y][z].gameObject.AddComponent<MinoType>();
//                                         Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString(); // Tetromino
//                                         Model.grid[x][y][z].gameObject.name = type.ToString();
//                                     } else if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type.Equals(""))
//                                         Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString();
// // 这里很容易报空异常,因为我的预设里可能忘记给小立方体加类型了,或是运行的过程中怎样                                    
                                    tmp.GetChild(0).parent = null;
                                    // PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, type.ToString());
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    GameObject.Destroy(tmp.gameObject);
                                    Model.grid[x][y][z] = null;
                                    tmp = null;
                                } else { // childCount > 1
                                    type.Length = 0;
                                    // if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>() == null) {
                                    //     Model.grid[x][y][z].gameObject.AddComponent<MinoType>();
                                    //     Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString();
                                    //     Model.grid[x][y][z].gameObject.name = type.ToString();
                                    // } else if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type.Equals(""))
                                    //     Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString();
                                        
                                    Debug.Log(TAG + " type.ToString(): " + type.ToString());
                                    // PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, type.ToString());
                                    // } else { // 当删除一个小立方体的时候,需要让其父控件知道,这个小立方体已经销毁了
                                    Model.grid[x][y][z].parent = null;
// 这里很容易报空异常,因为我的预设里可能忘记给小立方体加类型了,或是运行的过程中怎样                                    
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    // PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.name);
                                    // }
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

        public static void DeleteRow() { // 算法上仍然需要优化
            Debug.Log(TAG + ": DeleteRow() start");
            hasDeletedMinos = false; 
            bool isFullRowAtY = false;
            for (int y = 0; y < Model.gridHeight; y++) {

                isFullRowAtY = Model.IsFullRowAt(y);
                Debug.Log(TAG + " isFullRowAtY: " + isFullRowAtY); 
                // if (IsFullRowAt(y)) {
                if (isFullRowAtY) {
                    // 一定要控制同屏幕同时播放的粒子数量
                    // 1.同屏的粒子数量一定要控制在200以内，每个粒子的发射数量不要超过50个。
                    // 2.尽量减少粒子的面积，面积越大就会越卡。
                    // 3.粒子最好不要用Alfa Test（但是有的特效又不能不用，这个看美术吧）
                    //   粒子的贴图用黑底的这种，然后用Particles/Additive 这种Shader，贴图必须要2的幂次方，这样渲染的效率会高很多。个人建议 粒子特效的贴图在64左右，千万不要太大。
                    // // 让游戏中真正要播放粒子特效的时候，粒子不用在载入它的贴图，也不用实例化，仅仅是执行一下SetActive(true)。
                    // SetActive(true)的时候就不会执行粒子特效的Awake()方法，但是它会执行OnEnable方法。
                    // hasDeletedMinos = true; // Bug: I commented this out, supposedly it's for DeleteRowCoroutine only                    
                        
                    // m_ExplosionParticles.transform.position = new Vector3(2.5f, y, 2.5f); 
                    // m_ExplosionParticles.gameObject.SetActive(true); ...
                    // m_ExplosionParticles.Play();
                    // m_ExplosionAudio.Play(); ...

// CHALLENGE　MODE: 因为有五次撤销功能,所以这里标记一下:就是1　==>　2
                    if (GloData.Instance.isChallengeMode && y == 0)
                        markLayerIn(y);
                    DeleteMinoAt(y);
                    if (GloData.Instance.gameMode > 0 || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel < 3))
                       ScoreManager.currentScore += GloData.Instance.layerScore;
                    else
                        ScoreManager.currentScore += GloData.Instance.challengeLayerScore;
                        
                    MoveAllRowsDown(y + 1);
                    --y;

                    hasDeletedMinos = true;
                    Debug.Log(TAG + " hasDeletedMinos: " + hasDeletedMinos);
                }
            }

            if (BaseBoardSkin.isSkinChanged) { // debugging
                Debug.Log(TAG + ": Model.gridOcc[,,] aft DeleteRow done"); 
                MathUtilP.printBoard(Model.gridOcc); 
            }
        }
        static void markLayerIn(int y) {
            for (int i = 0; i < Model.gridXWidth; i++) 
                for (int j = 0; j < Model.gridZWidth; j++) 
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
                        && (gameMode > 0 && Model.gridOcc[x][y][z] == 1 || gameMode == 0 && Model.gridOcc[x][y][z] == 2)) {
                        // MathUtilP.print(x, y, z);
                        if (Model.grid[x][y][z].gameObject != null && Model.grid[x][y][z].parent != null && Model.grid[x][y][z].parent.gameObject != null
                            && !Model.grid[x][y][z].parent.gameObject.CompareTag("InitCubes") // 这些不能消除
                            && Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().childCnt == Model.grid[x][y][z].parent.childCount // 父控件是完整的
                            && isAllMinoInLayerY(Model.grid[x][y][z].parent, y)) { // 这个小方格的 父控件 的所有子立方体全部都在这一y层,就是,可以直接回收到资源池
                            // MathUtilP.print(x, y, z);
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
                        } else { // 当前立方体的 父控件 的某个或某些子立方体不在当前层,仅只回收当前小立方体到资源池
                            // MathUtilP.print(x, y, z);
// 在做预设的时候,有时候我的那些预设里的小立方体并没有标注清楚是什么类型,
                            Model.grid[x][y][z].parent = null; // 需要先解除这个父子控件关系,否则父控件永远以为这个子立方体存在存活
                            PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                            Model.grid[x][y][z] = null;
                            if (GloData.Instance.isChallengeMode) {
                                Model.gridOcc[x][y][z] = (y == Model.gridHeight-1 ? 0 : 2); // 0 ==> 2
                                Model.gridClr[x][y][z] = -1;
                            }
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
// TODO BUG:　LOGIC MODULE:                
// 当当前立方体仅且只剩自已(BUG TODO: 只要有消除就需要检查是否可以继续下落,哪怕只消除了一个立方体,还剩 七 个小立方体,先前想到的逻辑不完整),检查是否可以下落
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
                            if (Model.grid[x][y][z].parent == null) // 独个立方体,只将当前格下移
                                moveMinoDownOneGridHelper(x,  y, z);
                            else { // 当将格的父控件可以继续下降一格: 这里应该可以换个方法(以父控件整体来)写,但是暂时如此吧
                                foreach (Transform mino in Model.grid[x][y][z].parent) {
                                    if (mino.CompareTag("mino")) {
                                        Vector3 pos = MathUtilP.Round(mino.position);
                                        int i = (int)Math.Round(mino.position.x);
                                        int j = (int)Math.Round(mino.position.y);
                                        int k = (int)Math.Round(mino.position.z);
                                        moveMinoDownOneGridHelper(x,  y, z);
                                    }
                                }
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
            p.position += new Vector3 (0, -1, 0);
            foreach (Transform mino in p) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtilP.Round(mino.position);
                    int x = (int)Math.Round(mino.position.x);
                    int y = (int)Math.Round(mino.position.y);
                    int z = (int)Math.Round(mino.position.z);
                    if (!Model.CheckIsInsideGrid(pos)) return false;  // 落到格外不行
                    if (Model.GetTransformAtGridPosition(pos) != null // 0 || 2 当前格是可以降落的
                        && Model.GetTransformAtGridPosition(pos).parent != p) {
                        return false;
                    }
                }
            }
            p.position -= new Vector3 (0, -1, 0);
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
