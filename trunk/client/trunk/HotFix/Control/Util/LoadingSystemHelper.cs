using deepwaterooo.tetris3d;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public static class LoadingSystemHelper {
        private const string TAG = "LoadingSystemHelper";

        private static StringBuilder type = new StringBuilder("");
        private static GameObject tmpParentGO;

        private static bool isTheParentChildren(Transform tmpParent, TetrominoData parent) { // compare against SerializedTransform parent
            return (tmpParent.gameObject.name == parent.name &&
                    tmpParent.position == DeserializedTransform.getDeserializedTransPos(parent.transform) &&
                    tmpParent.rotation == DeserializedTransform.getDeserializedTransRot(parent.transform));
        }
        
        private static bool existInChildren(Transform transform, MinoDataCollection<TetrominoData, MinoData> children) {
            foreach (MinoData data in children) {
                if (transform.position == DeserializedTransform.getDeserializedTransPos(data.transform) &&
                    transform.rotation == DeserializedTransform.getDeserializedTransRot(data.transform)) {
                    return true;
                }
            }
            return false; 
        }
        private static bool containsMino(GameObject parent, int i, int j, int k) {
            int x = 0, y = 0, z = 0;
            foreach (Transform mino in parent.transform) {
                x = (int)Mathf.Round(mino.position.x);
                y = (int)Mathf.Round(mino.position.y);
                z = (int)Mathf.Round(mino.position.z);
                if (x == i && y == j && z == k) return true;
            }
            return false;
        }
        public static void LoadDataFromParentList(List<TetrominoData> parentList) {
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0, childCounter = 0;
            foreach (TetrominoData parentData in parentList) {
                Debug.Log(TAG + " LoadDataFromParentList() parentData.name: " + parentData.name);
                Debug.Log(TAG + " parentData.children.Count: " + parentData.children.Count);
                if (isThereAnyExistChild(parentData)) { // 存在
                    Debug.Log(TAG + " (!gridMatchesSavedParent(tmpParentGO, parentData.children)): " + (!gridMatchesSavedParent(tmpParentGO, parentData.children))); 
                    if (!gridMatchesSavedParent(tmpParentGO, parentData.children)) {  // 先删除多余的，再补全缺失的
// // BUG:　这是我这次整合源码,不明白的时候自己又加上的,现去掉,再跑一遍                        
//                         foreach (Transform trans in tmpParentGO.transform) { // 先 删除多余的, 怀疑这一步是可以完全不要的(但是可能的情况下是消除某层后从上面层落下来的)
//                            MathUtilP.print(MathUtilP.Round(trans.position));
//                            // Debug.Log(TAG + " (!myContains(trans, parentData.children)): " + (!myContains(trans, parentData.children))); 
//                            if (!myContains(trans, parentData.children)) {
//                                x = (int)Mathf.Round(trans.position.x);
//                                y = (int)Mathf.Round(trans.position.y);
//                                z = (int)Mathf.Round(trans.position.z);
//                                Model.grid[x][y][z].parent = null;
//                                GameObject.Destroy(Model.grid[x][y][z].gameObject); // todo how
//                                Model.gridOcc[x][y][z] = 0;
//                                Model.grid[x][y][z] = null;
//                            }
//                         }
                        // Debug.Log(TAG + " tmpParentGO.transform.childCount (deleted unwanted): " + tmpParentGO.transform.childCount);
                        foreach (MinoData minoData in parentData.children) {
                            Vector3 posA = MathUtilP.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
                            // MathUtilP.print(posA);
                            x = (int)Mathf.Round(posA.x);
                            y = (int)Mathf.Round(posA.y);
                            z = (int)Mathf.Round(posA.z);
                            MathUtilP.print(x, y, z);
                            
// 这里的写法不合理(容易加多),换一种方法写: 用补的,只补缺的,就不会多出来几粒
                            if (!containsMino(tmpParentGO, x, y, z)) {
                                GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type,
                                                                              DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                              DeserializedTransform.getDeserializedTransRot(minoData.transform),
                                                                              minoData.color);
                                tmpMinoGO.tag = "mino";
                                tmpMinoGO.transform.parent = tmpParentGO.transform;
                            }
                            // if (Model.grid[x][y][z] == null
                            //     ||  Model.grid[x][y][z].parent != null && Model.grid[x][y][z].parent.gameObject != tmpParentGO) { // 当前为空,或是从上面落下来的其它的
                            //     GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type,
                            //                                                   DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                            //                                                   DeserializedTransform.getDeserializedTransRot(minoData.transform),
                            //                                                   minoData.color);
                            //     tmpMinoGO.tag = "mino";
                            //     tmpMinoGO.transform.parent = tmpParentGO.transform;
                            //     if (Model.grid[x][y][z] == null) {
                            //         Model.grid[x][y][z] = tmpMinoGO.transform; // 当原有，如何升高 add in the position, and move previously one upper
                            //         Model.gridOcc[x][y][z] = 1;
                            //     } else {
                            //         FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform);
                            //     }
                            // }
                        }
                    }
                    Debug.Log(TAG + " tmpParentGO.transform.childCount (filled needed -- final): " + tmpParentGO.transform.childCount);
                } else { // 重新生成                                           // 空 shapeX Tetromino_X : Universal
                    GameObject tmpGameObject = PoolHelper.GetFromPool("TetrominoX",
                                                                      DeserializedTransform.getDeserializedTransPos(parentData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(parentData.transform),
                                                                      Vector3.one);
                    childCounter = 0;
                    foreach (MinoData minoData in parentData.children) {
                        GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type,
                                                                      DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(minoData.transform),
                                                                      minoData.color);
                        tmpMinoGO.transform.parent = tmpGameObject.transform;
                        x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
                        y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
                        z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
                        // fix for bug 5: fill in a Mino into board in y where there are more minos are above the filled in one
                        tmpMinoGO.tag = "mino";
                        Debug.Log(TAG + " isColumnFromHereEmpty(x, y, z): " + isColumnFromHereEmpty(x, y, z));
                        if (isColumnFromHereEmpty(x, y, z)) {
                            Model.grid[x][y][z] = tmpMinoGO.transform;
                            Model.gridOcc[x][y][z] = 1;
                        } else {
                            FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform); // update grid accordingly
                        }
                        ++childCounter;
                    }
                    tmpGameObject.GetComponent<TetrominoType>().type = parentData.type;
                    tmpGameObject.GetComponent<TetrominoType>().color = parentData.color;
                    tmpGameObject.GetComponent<TetrominoType>().childCnt = childCounter;
                    tmpGameObject.name = parentData.name;

                    // Debug.Log(TAG + " tmpGameObject.GetComponent<TetrominoType>().type: " + tmpGameObject.GetComponent<TetrominoType>().type); 
                    // Debug.Log(TAG + " tmpGameObject.transform.childCount: " + tmpGameObject.transform.childCount); 
                }
                // Debug.Log(TAG + ": Model.gridOcc[,,] after each deleted mino re-spawn"); 
                MathUtilP.printBoard(Model.gridOcc); 
            }
        }

        private static bool gridMatchesSavedParent(GameObject tmpGO, MinoDataCollection<TetrominoData, MinoData> data) {
            // Debug.Log(TAG + ": gridMatchesSavedParent()"); 
            Debug.Log(TAG + " (tmpGO.transform.childCount != data.Count): " + (tmpGO.transform.childCount != data.Count));
            if (tmpGO.GetComponent<TetrominoType>().childCnt == data.Count && tmpGO.transform.childCount == data.Count)
                return true; // 完整的
            if (tmpGO.transform.childCount != data.Count) 
                return false;
// tmpGO.transform.childCount == data.children.Count, potential mismatch ???
            foreach (Transform trans in tmpParentGO.transform) {
                if (!myContains(trans, data)) {
                    MathUtilP.print(trans.position);
                    MathUtilP.print(trans.rotation);
                    return false;
                }
            }
            return true;
        }
        
        private static bool myContains(Transform tmp, MinoDataCollection<TetrominoData, MinoData> children) {
            foreach (MinoData data in children)
                if (MathUtilP.Round(tmp.position) == MathUtilP.Round(DeserializedTransform.getDeserializedTransPos(data.transform))) 
// 因为实时运行时存在微小转动.这里暂不检查旋转角度
                    // && MathUtilP.Round(tmp.rotation) == MathUtilP.Round(DeserializedTransform.getDeserializedTransRot(data.transform)))
                    return true;
            return false;
        }

        private static void FillInMinoAtTargetPosition(int x, int y, int z, Transform minoTrans) {
            int o = Model.gridHeight - 1;
            // Model.grid[x][o][z] = 0, otherwose gameover
            while (Model.grid[x][o-1][z] == null) o--;
            for (int i = o; i > y; i--) { // y 
                Model.grid[x][i][z] = Model.grid[x][i-1][z];
                Model.gridOcc[x][i][z] = Model.gridOcc[x][i-1][z];
                if (Model.grid[x][i-1][z] != null) {
                    Model.grid[x][i-1][z].parent = null; // 先从父控件中移除
                    Model.grid[x][i-1][z] = null;
                    Model.gridOcc[x][i-1][z] = 0;
                    Model.grid[x][i][z].position += new Vector3(0, 1, 0);
                }
            }
            Model.grid[x][y][z] = minoTrans;
            Model.gridOcc[x][y][z] = 1;
            Debug.Log(TAG + " FillInMinoAtTargetPosition() gridOcc");
            MathUtilP.printBoard(Model.gridOcc);
        }

        private static bool isColumnFromHereEmpty(int x, int y, int z) {
            bool isColumnAboveEmpty = true;
            for (int i = y; i < Model.gridHeight; i++) {
                if (Model.grid[x][y][z] != null)
                    isColumnAboveEmpty = false;
            }
            return isColumnAboveEmpty;
        }

        private static bool isThereAnyExistChild(TetrominoData parentData) {
            // Debug.Log(TAG + ": isThereAnyExistChild()"); 
            Vector3 pos = Vector3.zero;
            int x = 0, y = 0, z = 0;
            MathUtilP.print("isThereAnyExistChild() parentData.transform.pos: ", MathUtilP.Round(DeserializedTransform.getDeserializedTransPos(parentData.transform)));
            foreach (MinoData mino in parentData.children) {
                pos = DeserializedTransform.getDeserializedTransPos(mino.transform);
                x = (int)Mathf.Round(pos.x);
                y = (int)Mathf.Round(pos.y);
                z = (int)Mathf.Round(pos.z);
                MathUtilP.print("isThereAnyExistChild() parentData mino pos:", x, y, z);
                if (Model.grid[x][y][z] != null && Model.grid[x][y][z].parent != null) { // // make sure parent matches first !
                    if (Model.grid[x][y][z].parent.gameObject.name == parentData.name &&
                        MathUtilP.Round(Model.grid[x][y][z].parent.position) == MathUtilP.Round(DeserializedTransform.getDeserializedTransPos(parentData.transform))
						&& MathUtilP.Round(Model.grid[x][y][z].parent.rotation) == MathUtilP.Round(DeserializedTransform.getDeserializedTransRot(parentData.transform))
						) { 
                        tmpParentGO = Model.grid[x][y][z].parent.gameObject;
                        Debug.Log(TAG + " tmpParentGO.name: " + tmpParentGO.name);
                        Debug.Log(TAG + " tmpParentGO.transform.childCount: " + tmpParentGO.transform.childCount);
                        MathUtilP.print(tmpParentGO.transform.position);
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}