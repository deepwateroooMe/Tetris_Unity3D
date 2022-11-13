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

                bool isThereAnyExistChildV = isThereAnyExistChild(parentData);
                Debug.Log(TAG + " isThereAnyExistChildV: " + isThereAnyExistChildV);
                if (isThereAnyExistChildV) { // 存在
                // if (isThereAnyExistChild(parentData)) { // 存在

                    Debug.Log(TAG + " (!gridMatchesSavedParent(tmpParentGO, parentData.children)): " + (!gridMatchesSavedParent(tmpParentGO, parentData.children))); 
                    if (!gridMatchesSavedParent(tmpParentGO, parentData.children)) {  // 先删除多余的，再补全缺失的
// 先 删除多余的, 这一步是必要的,因为当有消除父控件的子立方体可能往下掉变形了,需要先删除掉下去了变形了的
// 应该说更好的办法是直接改变这些掉下去了而导致位置变化的小立方体回归原位,但是暂时先这么写吧                        
                        foreach (Transform trans in tmpParentGO.transform) { 
                           if (!myContains(trans, parentData.children)) {
                               x = (int)Mathf.Round(trans.position.x);
                               y = (int)Mathf.Round(trans.position.y);
                               z = (int)Mathf.Round(trans.position.z);
                               Model.grid[x][y][z].parent = null;
                               PoolHelper.ReturnToPool(Model.grid[x][y][z].gameObject, Model.grid[x][y][z].gameObject.GetComponent<MinoType>().type);
                               Model.gridOcc[x][y][z] = 0;
                               Model.grid[x][y][z] = null;
                           }
                        }
                        Debug.Log(TAG + " tmpParentGO.transform.childCount (AFTER deleted unwanted): " + tmpParentGO.transform.childCount);
                        foreach (MinoData minoData in parentData.children) {
                            Vector3 posA = MathUtilP.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
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
                                tmpMinoGO.GetComponent<MinoType>().type = minoData.type;
                                tmpMinoGO.GetComponent<MinoType>().color = minoData.color;
                                Model.grid[x][y][z] = tmpMinoGO.transform;
                                Model.gridOcc[x][y][z] = 1;
                                tmpMinoGO.transform.parent = tmpParentGO.transform; // 在上面没有立方体掉下来的时候,是没有问题的,同一个父控件
                                // tmpMinoGO.transform.SetParent(tmpParentGO.transform, true); // 还是会有那个BUG,并没有从本质上解决这个问题 
                            }
// 这里的写法不合理(容易加多),换上面的方法写: 用补的,只补缺的,就不会多出来几粒:
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
                        Debug.Log(TAG + " tmpParentGO.transform.childCount (AFTER filled disappeared -- final): " + tmpParentGO.transform.childCount);
                    }
                } else { // 重新生成: 这个重新生成,哪怕是上面有东西,仍然是可以的 // 空 shapeX Tetromino_X : Universal
                    GameObject tmpGameObject = PoolHelper.GetFromPool("TetrominoX",
                                                                      DeserializedTransform.getDeserializedTransPos(parentData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(parentData.transform),
                                                                      Vector3.one);
                    tmpGameObject.GetComponent<TetrominoType>().type = parentData.type;
                    tmpGameObject.GetComponent<TetrominoType>().color = parentData.color;
                    tmpGameObject.name = parentData.name;
                    childCounter = 0;
                    foreach (MinoData minoData in parentData.children) {
                        if (minoData.color == -1)
                            Debug.Log(TAG + " (minoData.color == -1) parentData.name: " + parentData.name);
                        GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type,
                                                                      DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                      DeserializedTransform.getDeserializedTransRot(minoData.transform),
                                                                      minoData.color);
                        tmpMinoGO.transform.parent = tmpGameObject.transform;
                        // tmpMinoGO.transform.SetParent(tmpGameObject.transform, true);
                        x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
                        y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
                        z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
                        tmpMinoGO.tag = "mino";
                        tmpMinoGO.GetComponent<MinoType>().type = minoData.type;
                        tmpMinoGO.GetComponent<MinoType>().color = minoData.color;
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

                    Debug.Log(TAG + " tmpGameObject.GetComponent<TetrominoType>().type: " + tmpGameObject.GetComponent<TetrominoType>().type); 
                    Debug.Log(TAG + " tmpGameObject.transform.childCount: " + tmpGameObject.transform.childCount); 
                }
                Debug.Log(TAG + ": Model.gridOcc[,,] after each deleted mino re-spawn"); 
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
            while (Model.grid[x][o-1][z] == null) o--;
            for (int i = o; i > y; i--) { // y 
                Model.grid[x][i][z] = Model.grid[x][i-1][z];
                Model.gridOcc[x][i][z] = Model.gridOcc[x][i-1][z];
                if (Model.grid[x][i-1][z] != null) {
                    // Model.grid[x][i-1][z].parent = null; // BUG: 先从父控件中移除 <<<=== 这是自己这次不懂源码的时候加的,破坏了原当前格与其父控件的父子关系,不需要加这行
                    Model.grid[x][i-1][z] = null;
                    Model.gridOcc[x][i-1][z] = 0;
                    Model.grid[x][i][z].position += new Vector3(0, 1, 0);
                }
            }
            Model.grid[x][y][z] = minoTrans;
            Model.gridOcc[x][y][z] = 1;
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