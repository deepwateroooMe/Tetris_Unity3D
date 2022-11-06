using deepwaterooo.tetris3d;
using HotFix.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 这个应该是GameViewModel才像,会再修改    
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

        public delegate void BaseBoardSkinChangedDelegate();
        public static BaseBoardSkinChangedDelegate updateBaseCubesSkin;

        // public static int lastTetroIndiScore = 0;
        
        public void OnEnable() {
            Debug.Log(TAG + ": OnEnable()"); 
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            Start();
        }
        public void Start() {
            Debug.Log(TAG + " Start()");
            // EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
        }
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            // if (EventManager.Instance != null) {
            //     EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate);
            // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
            // }
        }
        // public void onActiveTetrominoLand(TetrominoLandEventInfo info) {
        //     Debug.Log(TAG + " onActiveTetrominoLand");
        //     lastTetroIndiScore = ComponentHelper.GetTetroComponent(info.unitGO).score;
        // }
        // public void onActiveTetrominoMove(TetrominoMoveEventInfo info) { 
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

        public System.Collections.IEnumerator DeleteRowCoroutine() {
            Debug.Log(TAG + ": DeleteRowCoroutine()");
            isDeleteRowCoroutineRunning = true;

            for (int y = 0; y < Model.gridHeight; y++) {
                // Debug.Log(TAG + " y: " + y); 
                // if (Model.IsFullFiveInLayerAt(y)) { 
                if ( (!GloData.Instance.isChallengeMode && Model.IsFullFiveInLayerAt(y))
                     // || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel > 2 && GloData.Instance.challengeLevel < 6 &&  Model.IsFullQuadInLayerAt(y)) ) { 
                     || (GloData.Instance.isChallengeMode && Model.IsFullQuadInLayerAt(y)) ) { 

                    Debug.Log(TAG + ": gridOcc[,,] IsFullQuadInLayerAt(y)"); 
                    MathUtilP.printBoard(Model.gridOcc); 

                    Model.isNumberOfRowsThisTurnUpdated = false;
                    // updateScoreEvent(); // commended now

                    Debug.Log(TAG + " (GloData.Instance.isChallengeMode && Model.zoneSum == 4): " + (GloData.Instance.isChallengeMode && Model.zoneSum == 4)); 
                    if (GloData.Instance.isChallengeMode && Model.zoneSum == 4) {
                        DeleteMinoAt(y);
                        ScoreManager.currentScore += GloData.Instance.challengeLayerScore; // 分数再优化一下
                        yield return null;
                    } else {
                        ScoreManager.currentScore += GloData.Instance.layerScore; // 分数再优化一下
                        Debug.Log(TAG + " (!isDeleteMNinoAtCoroutineRunning): " + (!isDeleteMNinoAtCoroutineRunning)); 
                        if (!isDeleteMNinoAtCoroutineRunning) {
                            deleteMinoAtCoroutine = CoroutineHelperP.StartCoroutine(DeleteMinoAtCoroutine(y)); // commented for tmp
                        }
                        yield return deleteMinoAtCoroutine;                        
                    }

                    MoveAllRowsDown(y + 1);
                    --y;

                    hasDeletedMinos = true;
                }
                // yield return null;
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

// TODO: 这里的逻辑还有很多问题,估计后来还会有很多BUG在这里.但暂时改到这里,等熟悉游戏和源码后会更好改        
        public static System.Collections.IEnumerator DeleteMinoAtCoroutine(int y) { 
            // Debug.Log(TAG + ": DeleteMinoAtCoroutine() start");
            isDeleteMNinoAtCoroutineRunning = true;
            
            if (minoPSList.Count > 0)
                minoPSList.Clear();

            yield return CoroutineHelperP.StartCoroutine(displayConnectedParticleEffects(y));
            
            for (int x = 0; x < Model.gridXWidth; x++) {
                for (int  z = 0;  z < Model.gridZWidth;  z++) {
                    if (Model.gridOcc != null && Model.gridOcc[x][y][z] == 2) {
                        Debug.Log("(x,y,z): [" + x + "," + y + "," + z +"]: " + Model.gridOcc[x][y][z]); 
                        if (Model.grid[x][y][z] != null && Model.grid[x][y][z].gameObject != null) { // 一定要
                            if (Model.grid[x][y][z].parent != null) {
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.name: " + Model.grid[x][y][z].parent.name);
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.childCount: " + Model.grid[x][y][z].parent.childCount);
                                
                                if (Model.grid[x][y][z].parent.childCount == 1) {
                                    Transform tmp = Model.grid[x][y][z].parent;
                                    type.Length = 0;
                                    if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>() == null) {
                                        Model.grid[x][y][z].gameObject.AddComponent<MinoType>();
                                        Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString(); // Tetromino
                                        Model.grid[x][y][z].gameObject.name = type.ToString();
                                    }
                                    tmp.GetChild(0).parent = null;
                                    // PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, type.ToString());
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    GameObject.Destroy(tmp.gameObject);
                                    Model.grid[x][y][z] = null;
                                    tmp = null;
                                } else { // childCount > 1
                                    type.Length = 0;
                                    if (Model.grid[x][y][z].gameObject.GetComponent<MinoType>() == null) {
                                        Model.grid[x][y][z].gameObject.AddComponent<MinoType>();
                                        Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type = type.Append("mino" + Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type.Substring(9, 1)).ToString();
                                        Model.grid[x][y][z].gameObject.name = type.ToString();
                                    }
                                    Debug.Log(TAG + " type.ToString(): " + type.ToString());
                                    // PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, type.ToString());
                                    // } else { // 当删除一个小立方体的时候,需要让其父控件知道,这个小立方体已经销毁了
                                    Model.grid[x][y][z].parent = null;
                                    PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                    // }
                                    Model.grid[x][y][z] = null;
                                }
                            } else { // (Model.grid[x][y][z].parent == null)
                                GameObject.Destroy(Model.grid[x][y][z]);
                                Model.grid[x][y][z] = null;
                            }
                        }
                    }
                }
            }
            yield return null;
            isDeleteMNinoAtCoroutineRunning = false;
        }

		static System.Collections.IEnumerator displayConnectedParticleEffects (int y) {
            Debug.Log(TAG + ": displayConnectedParticleEffects()"); 
            for (int x = 0; x < Model.gridXWidth; x++) 
                for (int  z = 0;  z < Model.gridZWidth;  z++)
                    if (Model.gridOcc != null && Model.gridOcc[x][y][z] == 2) {
                        if (Model.grid[x][y][z] != null && Model.grid[x][y][z].childCount == 0) {
                            MathUtilP.print(x, y, z);
                            GameObject connectedEffectTmp = PoolHelper.GetFromPool("minoPS", new Vector3(x, y, z), Quaternion.identity, Vector3.one);
                            connectedEffectTmp.transform.SetParent(Model.grid[x][y][z], false);
                            minoPSList.Add(connectedEffectTmp);
                        }
                    }
// TODO: 感觉这里还有点儿不通,时间不够长,粒子系统展示得不完整            
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
            // hasDeletedMinos = false; 
            bool isFullRowAtY = false;
            for (int y = 0; y < Model.gridHeight; y++) {

                isFullRowAtY = Model.IsFullRowAt(y);
                // Debug.Log(TAG + " isFullRowAtY: " + isFullRowAtY); 
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
                    // m_ExplosionParticles.gameObject.SetActive(true);
                    // m_ExplosionParticles.Play();
                    // m_ExplosionAudio.Play();

                    DeleteMinoAt(y);
                    if (GloData.Instance.gameMode > 0 || (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel < 3))
                       ScoreManager.currentScore += GloData.Instance.layerScore;
                    else
                        ScoreManager.currentScore += GloData.Instance.challengeLayerScore;
                        
                    MoveAllRowsDown(y + 1);
                    --y;
                }
            }

            if (BaseBoardSkin.isSkinChanged) { // debugging
                Debug.Log(TAG + ": Model.gridOcc[,,] aft DeleteRow done"); 
                MathUtilP.printBoard(Model.gridOcc); 
            }
        }
        public static void DeleteMinoAt(int y) {
            Debug.Log(TAG + ": DeleteMinoAt() start");
            for (int x = 0; x < Model.gridXWidth; x++) {
                for (int  z = 0;  z < Model.gridZWidth;  z++) {
                    if (GloData.Instance.gameMode > 0 || (GloData.Instance.gameMode == 0 && GloData.Instance.isChallengeMode)) { // GloData.Instance.gameMode > 0, 进行必要的回收, TODO: 只回收同一层的mino or Tetromino
                        if (Model.gridOcc[x][y][z] == 1 && Model.grid[x][y][z] != null) {
                            if (Model.grid[x][y][z].gameObject != null && Model.grid[x][y][z].parent != null && Model.grid[x][y][z].parent.gameObject != null
                                && !Model.grid[x][y][z].parent.gameObject.CompareTag("InitCubes")
                                && Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().childCnt == Model.grid[x][y][z].parent.childCount
                                && isAllMinoInLayerY(Model.grid[x][y][z].parent, y)) { // 所有的mino都在同一y层
                                // Debug.Log(TAG + ": x, y, z values: ");
                                MathUtilP.print(x, y, z);
                                Transform tmpParentTransform = Model.grid[x][y][z].parent;
                                foreach (Transform mino in Model.grid[x][y][z].parent) {
                                    int i = (int)Mathf.Round(mino.position.x);
                                    int j = (int)Mathf.Round(mino.position.y);
                                    int k = (int)Mathf.Round(mino.position.z);
                                    Model.grid[i][j][k] = null;
                                    if (GloData.Instance.isChallengeMode) {
                                        Model.gridOcc[i][j][k] = 0; 
                                        Model.gridClr[i][j][k] = -1;
                                    }
                                }
                                // PoolHelper.ReturnToPool(Model.grid[x][y][z].parent.gameObject, Model.grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().type);
                                PoolHelper.ReturnToPool(tmpParentTransform.gameObject, tmpParentTransform.gameObject.GetComponent<TetrominoType>().type);
                            } else {
                                MathUtilP.print(x, y, z);
                                PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                                Model.grid[x][y][z] = null;
                                if (GloData.Instance.isChallengeMode) {
                                    Model.gridOcc[x][y][z] = 0; 
                                    Model.gridClr[x][y][z] = -1;
                                }
                            }
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
            Debug.Log(TAG + ": MoveAllRowsDown()"); 
            for (int i = y; i < Model.gridHeight; i++) {
                MoveRowDown(i);
            }
        }
        public static void MoveRowDown(int y) {
            // Debug.Log(TAG + ": MoveRowDown()");
            int cnt = 0;
            if (GloData.Instance.gameMode > 0 ||
                (GloData.Instance.isChallengeMode &&
                 (GloData.Instance.challengeLevel < 3 || GloData.Instance.challengeLevel > 5))) { // challenge mode level 1 2 6 7 8 9 10
                for (int j = 0; j < Model.gridZWidth; j++) {
                    for (int x = 0; x < Model.gridXWidth; x++) {
                        if (Model.grid[x][y][j] != null) {
                            Model.grid[x][y-1][j] = Model.grid[x][y][j];
                            Model.grid[x][y-1][j].position += new Vector3(0, -1, 0);
                            Model.gridOcc[x][y-1][j] = Model.gridOcc[x][y][j];
                            if (GloData.Instance.isChallengeMode) {
                                Model.gridClr[x][y-1][j] = Model.gridClr[x][y][j];
                            }
                            Model.grid[x][y][j] = null;

                            // Debug.Log(TAG + " (y == 1): " + (y == 1));
                            // Debug.Log(TAG + " Model.baseCubes.Length: " + Model.baseCubes.Length); 
                            if (y == 1 && GloData.Instance.isChallengeMode) { // still need to update baseboard skin accordingly
                                Model.baseCubes[x + j * Model.gridXWidth] = Model.gridClr[x][y][j];
                                BaseBoardSkin.isSkinChanged = true;
                            }
                        }
                    }
                }
            } else { // GloData.Instance.gameMode == 0
                for (int x = 0; x < Model.gridXWidth; x++) {
                    for (int z = 0; z < Model.gridZWidth; z++) {
                        if (Model.gridOcc[x][y-1][z] == 2) {
                            ++cnt;
                            Model.gridOcc[x][y-1][z] = Model.gridOcc[x][y][z];
                            if (Model.grid[x][y][z] != null) {
                                Model.grid[x][y-1][z] = Model.grid[x][y][z];
                                Model.grid[x][y][z] = null;
                                Model.grid[x][y-1][z].position += new Vector3(0, -1, 0);
                                if (GloData.Instance.isChallengeMode) {
                                    Model.gridClr[x][y-1][z] = Model.gridClr[x][y][z];

                                    // Debug.Log(TAG + " (y == 1): " + (y == 1));
                                    // Debug.Log(TAG + " Model.baseCubes.Length: " + Model.baseCubes.Length); 
                                    if (y == 1) { // still need to update baseboard skin accordingly
                                        Model.baseCubes[x + z * Model.gridXWidth] = Model.gridClr[x][y][z];
                                        BaseBoardSkin.isSkinChanged = true;
                                    }
                                }
                                Model.grid[x][y][z] = null;
                            }
                            Model.gridOcc[x][y][z] = y == Model.gridHeight - 1 ? 0 : 2;
                        }
                    }   
                }
            } // GloData.Instance.gameMode == 0

            if (y == 1 && BaseBoardSkin.isSkinChanged) { // debug
                updateBaseCubesSkin();
            }
            // Debug.Log(TAG + " cnt: " + cnt); 
        } 
    }
}



