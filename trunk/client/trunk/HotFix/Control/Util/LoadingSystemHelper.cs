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

        public static void LoadDataFromParentList(List<TetrominoData> parentList) {
            Debug.Log(TAG + ": LoadDataFromParentList()"); 
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0, childCounter = 0;
            foreach (TetrominoData parentData in parentList) {
                Debug.Log(TAG + " parentData.name: " + parentData.name);
                Debug.Log(TAG + " parentData.children.Count: " + parentData.children.Count);
                if (isThereAnyExistChild(parentData)) { // 存在
                    // Debug.Log(TAG + " (!gridMatchesSavedParent(tmpParentGO, parentData.children)): " + (!gridMatchesSavedParent(tmpParentGO, parentData.children))); 
                    if (!gridMatchesSavedParent(tmpParentGO, parentData.children)) {  // 先删除多余的，再补全缺失的
                        // foreach (Transform trans in tmpParentGO.transform) { // 先 删除多余的
                        //     MathUtil.print(MathUtil.Round(trans.position));
                        //     // Debug.Log(TAG + " (!myContains(trans, parentData.children)): " + (!myContains(trans, parentData.children))); 
                        //     if (!myContains(trans, parentData.children)) {
                        //         x = (int)Mathf.Round(trans.position.x);
                        //         y = (int)Mathf.Round(trans.position.y);
                        //         z = (int)Mathf.Round(trans.position.z);
                        //         // MathUtil.print(x, y, z); // this one is right
                        //         Model.grid[x, y, z].parent = null;

                        //         // Destroy(Model.grid[x, y, z].gameObject); // todo how
                        //         Model.gridOcc[x, y, z] = 0;
                        //         Model.grid[x, y, z] = null;
                        //     }
                        // }
                        Debug.Log(TAG + " tmpParentGO.transform.childCount (deleted unwanted): " + tmpParentGO.transform.childCount);
                        foreach (MinoData minoData in parentData.children) {
                            Vector3 posA = MathUtil.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform)); 
                            MathUtil.print(posA);
                            x = (int)Mathf.Round(posA.x);
                            y = (int)Mathf.Round(posA.y);
                            z = (int)Mathf.Round(posA.z);

                            Debug.Log(TAG + " (Model.gridw[x][y][z] == null): " + (Model.grid[x][y][z] == null));
                            if (Model.grid[x][y][z] != null) {
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.gameObject.name: " + Model.grid[x][y][z].parent.gameObject.name); 
                                Debug.Log(TAG + " Model.grid[x][y][z].parent.childCount: " + Model.grid[x][y][z].parent.childCount); 
                            }
                            if (Model.grid[x][y][z] == null || (Model.grid[x][y][z].parent.gameObject != tmpParentGO)) {
                                type.Length = 0;
                                GameObject tmpMinoGO = PoolHelper.GetFromPool(type.Append(minoData.type).ToString(), DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                                        DeserializedTransform.getDeserializedTransRot(minoData.transform), minoData.color);
                                
                                tmpMinoGO.transform.parent = tmpParentGO.transform;
                                if (Model.grid[x][y][z] == null) {
                                    Model.grid[x][y][z] = tmpMinoGO.transform; // 当原有，如何升高 add in the position, and move previously one upper
                                    Model.gridOcc[x][y][z] = 1;
                                } else {
                                    FillInMinoAtTargetPosition(x, y, z, tmpMinoGO.transform);
                                }
                            }
                        }
                    }
                    Debug.Log(TAG + " tmpParentGO.transform.childCount (filled needed -- final): " + tmpParentGO.transform.childCount);
                } else { // 重新生成                                           // 空 shapeX Tetromino_X : Universal
                    GameObject tmpGameObject = PoolHelper.GetFromPool("shapeX", DeserializedTransform.getDeserializedTransPos(parentData.transform), 
                                                                                DeserializedTransform.getDeserializedTransRot(parentData.transform), Vector3.one);
                    childCounter = 0;
                    foreach (MinoData minoData in parentData.children) {
                        GameObject tmpMinoGO = PoolHelper.GetFromPool(minoData.type, DeserializedTransform.getDeserializedTransPos(minoData.transform), 
                                                                                DeserializedTransform.getDeserializedTransRot(minoData.transform), minoData.color);
                        tmpMinoGO.transform.parent = tmpGameObject.transform;
                        x = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).x);
                        y = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).y);
                        z = (int)Mathf.Round(DeserializedTransform.getDeserializedTransPos(minoData.transform).z);
                        // fix for bug 5: fill in a Mino into board in y where there are more minos are above the filled in one
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
                MathUtil.printBoard(Model.gridOcc); 
            }
        }

        private static bool gridMatchesSavedParent(GameObject tmpGO, MinoDataCollection<TetrominoData, MinoData> data) {
            // Debug.Log(TAG + ": gridMatchesSavedParent()"); 
            if (tmpGO.GetComponent<TetrominoType>().childCnt == data.Count && tmpGO.transform.childCount == data.Count)
                return true; // 完整的
            else if (tmpGO.transform.childCount != data.Count)
                return false;
            else { // tmpGO.transform.childCount == data.children.Count, potential mismatch ???
                foreach (Transform trans in tmpParentGO.transform) {
                    if (!myContains(trans, data))
                        return false;
                }
                return true;
            }
        }
        
        private static bool myContains(Transform tmp, MinoDataCollection<TetrominoData, MinoData> children) {
            foreach (MinoData data in children)
                if (MathUtil.Round(tmp.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(data.transform)) &&
                    MathUtil.Round(tmp.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(data.transform)))
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
            Debug.Log(TAG + ": isThereAnyExistChild()"); 
            Vector3 pos = Vector3.zero;
            int x = 0, y = 0, z = 0;
            foreach (MinoData mino in parentData.children) {
                pos = DeserializedTransform.getDeserializedTransPos(mino.transform);
                x = (int)Mathf.Round(pos.x);
                y = (int)Mathf.Round(pos.y);
                z = (int)Mathf.Round(pos.z);
                MathUtil.print(x, y, z);
                if (Model.grid[x][y][z] != null && Model.grid[x][y][z].parent != null) { // // make sure parent matches first !
                    if (Model.grid[x][y][z].parent.gameObject.name == parentData.name &&
                        MathUtil.Round(Model.grid[x][y][z].parent.position) == MathUtil.Round(DeserializedTransform.getDeserializedTransPos(parentData.transform)) && 
                        MathUtil.Round(Model.grid[x][y][z].parent.rotation) == MathUtil.Round(DeserializedTransform.getDeserializedTransRot(parentData.transform))) { 
                        tmpParentGO = Model.grid[x][y][z].parent.gameObject;
                        Debug.Log(TAG + " tmpParentGO.name: " + tmpParentGO.name);
                        Debug.Log(TAG + " tmpParentGO.transform.childCount: " + tmpParentGO.transform.childCount); 
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
