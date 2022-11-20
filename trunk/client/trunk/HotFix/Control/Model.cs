using deepwaterooo.tetris3d;
using Framework.MVVM;
using HotFix.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public class Model { 
        private const string TAG = "Model";

        public static int gridWidth;
        public static int gridXWidth;
        public static int gridZWidth;
        
        public static int gridHeight = 12; 
        public static Transform [][][] grid; 
        public static int [][][] gridOcc;
        public static int [][][] gridClr; // color grid
        
        public static int [] baseCubes;
        public static int [] prevSkin;
        public static int [] prevIdx;
        
        public static int numberOfRowsThisTurn = 0; 
        public static bool isNumberOfRowsThisTurnUpdated = false;
        public static bool mcubesInitiated = false;
        
        private static StringBuilder type = new StringBuilder("");
        private static int randomTetromino;

        public static void cleanUpGameBroad() { // 基本能够把面板清理干净: 并没有清除关卡中原设的 障碍立方体
// dealing with currentActiveTetromino & ghostTetromino firrst
            if (ViewManager.nextTetromino != null && ViewManager.nextTetromino.CompareTag("currentActiveTetromino")) { // hang in the air
                Debug.Log(TAG + " (ViewManager.ghostTetromino != null): " + (ViewManager.ghostTetromino != null));  // always true
                if (ViewManager.ghostTetromino != null) 
                    PoolHelper.recycleGhostTetromino();
                PoolHelper.recycleNextTetromino();
            }
            for (int x = 0; x < gridXWidth; x++) 
                for (int y = 0; y < gridHeight; y++) 
                    for (int z = 0; z < gridZWidth; z++) {
                        if (grid[x][y][z] != null) {
                            if (GloData.Instance.isChallengeMode // 因为在不同的游戏模式等下切换,这里也必须得清除掉
                                && grid[x][y][z].parent != null && grid[x][y][z].parent.gameObject != null
                                && grid[x][y][z].parent.gameObject.CompareTag("InitCubes")) {
                                foreach (Transform mino in grid[x][y][z].parent) { // 一次性将这里面所有的小立方体全部清除
                                    int i = (int)Mathf.Round(mino.position.x);
                                    int j = (int)Mathf.Round(mino.position.y);
                                    int k = (int)Mathf.Round(mino.position.z);
                                    if (j >= 0 && j < gridHeight && i >= 0 && i < gridXWidth && k >= 0 && k < gridZWidth) 
                                        if (grid[i][j][k] != null) {
                                            // MathUtilP.print(i, j, k);
                                            grid[i][j][k].parent = null;
                                            if (grid[i][j][k].gameObject != null) 
                                                PoolHelper.ReturnToPool(grid[i][j][k].gameObject, grid[i][j][k].gameObject.GetComponent<MinoType>().type);
                                            grid[i][j][k] = null;
                                            gridOcc[i][j][k] = 0;
                                            if (GloData.Instance.isChallengeMode) gridClr[i][j][k] = -1;
                                        }
                                }
                                continue;
                            }  
// 以 方块砖 为单位回收到资源池里去                            
                            Transform tmpRefParent = null;
                            if (grid[x][y][z].parent != null && grid[x][y][z].parent.gameObject.GetComponent<TetrominoType>().childCnt == grid[x][y][z].parent.childCount) {
                                tmpRefParent = grid[x][y][z].parent;
                                foreach (Transform mino in grid[x][y][z].parent) {
                                    int i = (int)Mathf.Round(mino.position.x);
                                    int j = (int)Mathf.Round(mino.position.y);
                                    int k = (int)Mathf.Round(mino.position.z);
                                    if (j >= 0 && j < gridHeight && i >= 0 && i < gridXWidth && k >= 0 && k < gridZWidth) {
                                        // MathUtilP.print(i, j , k);
                                        if (grid[i][j][k] != null) {
                                            grid[i][j][k] = null;
                                            gridOcc[i][j][k] = 0;
                                            if (GloData.Instance.isChallengeMode) gridClr[i][j][k] = -1;
                                        }
                                    }
                                }
                                PoolHelper.ReturnToPool(tmpRefParent.gameObject, tmpRefParent.gameObject.GetComponent<TetrominoType>().type);
                            } else { // 方块砖的 子立方体 不全,有过消除
                                tmpRefParent = grid[x][y][z].parent; 
                                foreach (Transform mino in grid[x][y][z].parent) {
                                    int i = (int)Mathf.Round(mino.position.x);
                                    int j = (int)Mathf.Round(mino.position.y);
                                    int k = (int)Mathf.Round(mino.position.z);
                                    if (j >= 0 && j < gridHeight && i >= 0 && i < gridXWidth && k >= 0 && k < gridZWidth) // 还是有小立方体 计算中 转到了大立方体之外
                                        if (grid[i][j][k] != null) {
                                            // MathUtilP.print(i, j, k);
                                            grid[i][j][k].parent = null;
                                            if (grid[i][j][k].gameObject != null) 
                                                PoolHelper.ReturnToPool(grid[i][j][k].gameObject, grid[i][j][k].gameObject.GetComponent<MinoType>().type);
                                            grid[i][j][k] = null;
                                            gridOcc[i][j][k] = 0;
                                            if (GloData.Instance.isChallengeMode) gridClr[i][j][k] = -1;
                                        }
                                }
                            }
                            if (tmpRefParent != null) { // 加这个送判断是因为上面有一种情况不不会重置,仍为空,所以需要作非空检测
                                GameObject.Destroy(tmpRefParent.gameObject);
                                tmpRefParent = null; // 这么并没有消除无子立方体的空父件
                            }
// 问题是:为什么我在这里遍历不到,而在再下面的再一次遍历中,可以把它们校正过来?                            
                        } else if (grid[x][y][z] == null && gridOcc[x][y][z] == 1) {
                            Debug.Log(TAG + " ERROR: (Model.grid[x][y][z] == null && Model.gridOcc[x][y][z] == 1) Please check backwards");
                            MathUtilP.print(x, y, z);
                            gridOcc[x][y][z] = 0;
                        }
                    }
            Debug.Log(TAG + " AFTER cleanUpGameBroad() Model.gridOcc[x][y][z]");
            MathUtilP.printBoard(gridOcc);
// 问题是:为什么我在这里遍历不到,而在再下面的再一次遍历中,可以把它们校正过来?                            
            for (int i = 0; i < Model.gridXWidth; i++)
                for (int j = 0; j < Model.gridHeight; j++) 
                    for (int k = 0; k < Model.gridZWidth; k++) 
                        if (gridOcc[i][j][k] == 1) {
                            Debug.Log(TAG + " ERROR: (Model.grid[x][y][z] == null && Model.gridOcc[x][y][z] == 1) Please check backwards");
                            MathUtilP.print(i, j, k);
                            Debug.Log(TAG + " Model.grid[i][j][k].parent.gameObject.name: " + Model.grid[i][j][k].parent.gameObject.name);
                            GameObject.Destroy(Model.grid[i][j][k].gameObject);
                            gridOcc[i][j][k] = 0;
                        }
            Debug.Log(TAG + " AFTER cleanUpGameBroad() Model.gridOcc[x][y][z] AFTER CORRECTION");
            MathUtilP.printBoard(gridOcc);
        }
        
        public static void UpdateGrid(GameObject go) { // update gridOcc, gridClr at the same time
            for (int y = 0; y < gridHeight; y++) 
                for (int z = 0; z < gridZWidth; z++) 
                    for (int x = 0; x < gridXWidth; x++)
                        if (grid[x][y][z] != null && grid[x][y][z].parent == go.transform) {
                            //MathUtilP.print(x, y, z);
                            grid[x][y][z] = null; 
                            gridOcc[x][y][z]= 0;
                            if (GloData.Instance.isChallengeMode)
                                gridClr[x][y][z]= -1; 
                        }
            foreach (Transform mino in go.transform) { // 当挑战模式更新地板砖的时候,这里就是地板砖的着色
                Vector3 pos = MathUtilP.Round(mino.position);
                if (pos.y >= 0 && pos.y < gridHeight && pos.x >= 0 && pos.x < gridXWidth && pos.z >= 0 && pos.z < gridZWidth) { 
                    grid[(int)pos.x][(int)pos.y][(int)pos.z] = mino;
// 把它独立成一个单独的方法,会效率高一点儿;要不然每个方块砖坠落,都需要检查一下
// 对于标签为InitCubes的来说,暂时的游戏逻辑设置为不可以消除,但VALICATE为可接触
                    if (go.CompareTag("InitCubes"))
                        gridOcc[(int)pos.x][(int)pos.y][(int)pos.z] = 8;
                    else
                        gridOcc[(int)pos.x][(int)pos.y][(int)pos.z] = 1;
                    if (GloData.Instance.isChallengeMode) {
                        gridClr[(int)pos.x][(int)pos.y][(int)pos.z] = mino.GetComponent<MinoType>().color;
                        // if ((int)pos.y == 0) // 地板砖的着色,要这个时候更新吗
                        //     baseCubes[(int)x + Model.gridXWidth * (int)pos.z] = gridClr[(int)pos.x][(int)pos.y][(int)pos.z];
                    }
                }
            }
            if (ViewManager.nextTetromino != null)
				Debug.Log(TAG + " UpdateGrid() ViewManager.nextTetromino.name: " + ViewManager.nextTetromino.name);
            // Debug.Log(TAG + " AFTER UpdateGrid() Model.gridOcc[x][y][z]");
            // MathUtilP.printBoard(gridOcc);
        }

        public static bool CheckIsValidPosition() { // check if physically fits into the grid
            // Debug.Log(TAG + " CheckIsValidPosition");
            // MathUtilP.print(ViewManager.nextTetromino.transform.position);
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtilP.Round(mino.position);
                    if (!CheckIsInsideGrid(pos) || CheckIsInsideBarials(pos)) {
                        return false;
                    }
                    if (GetTransformAtGridPosition(pos) != null
					    && GetTransformAtGridPosition(pos).parent != ViewManager.nextTetromino.transform) {
                        return false;
                    }
                }
            }
            return true;
        }
// 这里数的过程中:若是有满足条件(同行同列同对角线全满),会被标记为 2
// BUG TODO: 这里总是最先消除最底层,这是不对的.应该同时消除每一层,再不断往下降        
        public static bool IsFullFiveInLayerAt(int y) { 
            int tmpSum = 0;
            bool isFullFiveInLayer = false;
            numberOfRowsThisTurn = 0;
            for (int x = 0; x < gridXWidth; x++) {
                tmpSum = 0;
                for (int z = 0; z < gridZWidth; z++) {
                    tmpSum += gridOcc[x][y][z] == 0 ? 0 : 1; 
                }
                if (tmpSum == gridZWidth) {
                    numberOfRowsThisTurn++;
                    isFullFiveInLayer = true;
                    for (int z = 0; z < gridZWidth; z++) {
                        gridOcc[x][y][z] += gridOcc[x][y][z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5行
            for (int z = 0; z < gridZWidth; z++) { // 数5行
                tmpSum = 0;
                for (int x = 0; x < gridXWidth; x++) {
                    tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                }
                if (tmpSum == gridXWidth) {
                    numberOfRowsThisTurn++;
                    isFullFiveInLayer = true;
                    for (int x = 0; x < gridXWidth; x++) {
                        gridOcc[x][y][z] += gridOcc[x][y][z] == 1 ? 1 : 0; 
                    }
                }
            } // 数完5列
            tmpSum = 0;
            for (int x = 0; x < gridXWidth; x++) { // 数2对角线
                for (int z = 0; z < gridZWidth; z++) {
                    if (x == z) {
                        tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                    }
                }
            }
            if (tmpSum == gridZWidth) {
                isFullFiveInLayer = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridXWidth; x++) { // 数2对角线
                    for (int o = 0; o < gridZWidth; o++) {
                        if (x == o) {
                            gridOcc[x][y][o] += gridOcc[x][y][o] == 1 ? 1 : 0;  
                        }
                    }
                }
            } // 数完一条对角线
            tmpSum = 0; 
            for (int x = 0; x < gridXWidth; x++) {
                for (int z = 0; z < gridZWidth; z++) {
                    if (z == gridZWidth - 1 - x) {
                        tmpSum += gridOcc[x][y][z] == 2 ? 1 : gridOcc[x][y][z]; // 2
                    }
                }
            }
            if (tmpSum == gridZWidth) {
                isFullFiveInLayer = true;
                numberOfRowsThisTurn++;
                for (int x = 0; x < gridXWidth; x++) {
                    for (int o = 0; o < gridZWidth; o++) {
                        if (o == gridZWidth - 1 - x) {
                            gridOcc[x][y][o] += gridOcc[x][y][o] == 1 ? 1 : 0; 
                        }
                    }
                }
            } // 数完另一条对角线
            // Debug.Log(TAG + ": IsFullFiveInLayerAt(): " + isFullFiveInLayer);
            return isFullFiveInLayer;
        }

        public static bool IsFullRowAt(int y) {
            // Debug.Log(TAG + ": IsFullRowAt()");
            for (int x = 0; x < gridXWidth; x++)
                for (int j = 0; j < gridZWidth; j++) {
                    if (!GloData.Instance.isChallengeMode && grid[x][y][j] == null) return false;
// changle mode
                    if (grid[x][y][j] == null
                        && (gridOcc[x][y][j] == 0 || GloData.Instance.isChallengeMode && gridOcc[x][y][j] == 2)) return false;
                    // if ( (!GloData.Instance.isChallengeMode
                    //       && (grid[x][y][j] == null || grid[x][y][j].parent == ViewManager.ghostTetromino.transform && grid[x][y][j].parent != ViewManager.nextTetromino.transform)) 
                    //      || (GloData.Instance.isChallengeMode && grid[x][y][j] == null && gridOcc[x][y][j] == 0) )
                    //     return false;
                }
            numberOfRowsThisTurn++; // number of layers
            return true;
        }


        public static void resetGridAfterDisappearingNextTetromino(GameObject tetromino) {
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtilP.Round(mino.position);
                if ((int)pos.y >= 0 && (int)pos.y < gridHeight && (int)pos.x >= 0 && (int)pos.x < gridXWidth && (int)pos.z >= 0 && (int)pos.z < gridZWidth) { 
                    grid[(int)pos.x][(int)pos.y][(int)pos.z] = null;
                    gridOcc[(int)pos.x][(int)pos.y][(int)pos.z] = 0;
                }
            }
        }

        public static void resetSkinAfterDisappearingNextTetromino(GameObject tetromino) {
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtilP.Round(mino.position);
                if ((int)pos.y == 0) { 
                    baseCubes[MathUtilP.getIndex(pos)] = prevSkin[ getSkinCubeIdx(MathUtilP.getIndex(pos)) ]; 
                }
            }
        }
        private static int getSkinCubeIdx(int idx) {
            for (int i = 0; i < 4; i++) {
                if (prevIdx[i] == -1) return -1;
                if (prevIdx[i] == idx)
                    return i;
                else continue;
            }
            return -1;
        }
        
        public static void resetGridOccBoard() {
            for (int y = 0; y < gridHeight; y++) {
                for (int x = 0; x < gridXWidth; x++) {
                    for (int z = 0; z < gridZWidth; z++) {
                        gridOcc[x][y][z] = 0;
                    }
                }
            }
        }

        public static bool CheckIsAboveGrid(Tetromino tetromino) {
            for (int x = 0; x < gridXWidth; x++)
                for (int j = 0; j < gridZWidth; j++) 
                    foreach (Transform mino in tetromino.transform) {
                        Vector3 pos = MathUtilP.Round(mino.position);
                        //if (mino.CompareTag("mino" && pos.y > gridHeight - 1)) 
                        //if (mino.CompareTag("mino" && pos.y >= gridHeight - 1))
                        if (pos.y >= gridHeight - 1) // BUG: for game auto ended after first tetromino landing down
                            return true;
                    }
            return false;
        }

        public static Transform GetTransformAtGridPosition(Vector3 pos) {
            if ((int)pos.y > gridHeight - 1 || (int)pos.y < 0 || (int)pos.x < 0 || (int)pos.x >= gridXWidth || (int)pos.z < 0 || (int)pos.z >= gridZWidth) 
                return null;
            else
                return grid[(int)pos.x][(int)pos.y][(int)pos.z];
        }

        public static bool isFullInLayerAt(int y) { // f: gridOcc
            for (int i = 0; i < gridXWidth; i++)
                for (int j = 0; j < gridZWidth; j++) {
                    if (gridOcc[i][y][j] != 1 && gridOcc[i][y][j] != 9) {
                        // Debug.Log(TAG + " isFullInLayerAt():  FALSE");
                        return false;
                    }
                }
            // Debug.Log(TAG + " isFullInLayerAt():  TRUE");
            return true;
        }
        
        public static int zoneSum = 0;
        public static bool IsFullQuadInLayerAt (int y) {
            // Debug.Log(TAG + ": IsFullQuadInLayerAt()");
            int quadSum = 0;
            bool isFullQuadInLayer = false;
// 好像是把每层分成了四个小区,可以每小区单独先降一小区?            
            int [] activeZone = new int[4];
            Vector3 center = Vector3.zero;
            switch (GloData.Instance.challengeLevel.Value) {
            case 3:
                center = new Vector3(3.5f, 0, 3.5f);
                break;
            case 4:
                center = new Vector3(4f, 0, 4f);
                break;
            case 5:                    
                center = new Vector3(4f, 0, 3f);
                break;
            }

            int tmp = 0;
            zoneSum = 0;

// (GloData.Instance.challengeLevel == 3)            
            if (GloData.Instance.challengeLevel.Value == 3) { // 必须单独处理
                Debug.Log(TAG + " (int)center.x: " + (int)center.x); 
                Debug.Log(TAG + " (int)center.z: " + (int)center.z); 
                for (int i = 0; i <= (int)center.x; i++) 
                    for (int j = 0; j <= (int)center.z; j++) {
                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                        ++tmp;
                    }
                if (tmp == 13) {
                    activeZone[0] = 1;
                    ++zoneSum;
                } //
                tmp = 0;
                for (int i = 0; i <= (int)center.x; i++) 
                    for (int j = (int)center.z + 1; j < gridZWidth; j++) {
                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                        ++tmp;
                    }
                if (tmp == 13) {
                    activeZone[1] = 1;
                    ++zoneSum;
                } //
                tmp = 0;
                for (int i = (int)center.x + 1; i < gridXWidth; i++) 
                    for (int j = 0; j <= (int)center.z; j++) {
                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                        ++tmp;
                    }
                if (tmp == 13) {
                    activeZone[2] = 1;
                    ++zoneSum;
                } //
                tmp = 0;
                for (int i = (int)center.x + 1; i < gridXWidth; i++) 
                    for (int j = (int)center.z + 1; j < gridZWidth; j++) {
                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                        ++tmp;
                    }
                if (tmp == 13) {
                    activeZone[3] = 1;
                    ++zoneSum;
                }
                for (int i = 0; i < 4; i++) {
                    if (activeZone[i] == 1) 
                        Debug.Log(TAG + ": activeZone: " + " i: " + i); 
                    Debug.Log(TAG + " zoneSum: " + zoneSum); 
                }
                if (zoneSum >= 2) {
                    for (int x = 0; x < 4; x++) {
                        if (activeZone[x] == 1) {
                            if (x == 0) {
                                for (int i = 0; i <= (int)center.x; i++) {
                                    for (int j = 0; j <= (int)center.z; j++) {
                                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                        gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                                    }
                                }
                            } else if (x == 1) {
                                for (int i = 0; i <= (int)center.x; i++) {
                                    for (int j = (int)center.z + 1; j < gridZWidth; j++) {
                                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                        gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                                    }
                                }
                            } else if (x == 2) {
                                for (int i = (int)center.x + 1; i < gridXWidth; i++) {
                                    for (int j = 0; j <= (int)center.z; j++) {
                                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                        gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                                    }
                                }
                            } else if (x == 3) {
                                for (int i = (int)center.x + 1; i < gridXWidth; i++) {
                                    for (int j = (int)center.z + 1; j < gridZWidth; j++) {
                                        if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                        gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                                    }
                                }
                            }
                        }
                    }
// 分成四个小区，中线横竖都共用，至少有两个小区可以消除，那么就消除他们，同样是把1　＝＝＞2                    
                    Debug.Log(TAG + ": gridOcc[,,] aft detecting IsFullQuadInLayerAt(y)"); 
                    MathUtilP.printBoard(Model.gridOcc); 
                    
                    isFullQuadInLayer = true;
                    return true;
                }
                return false;
            } // level 3 done

// 我昨天晚上还以为我以前已经把这些逻辑写完写好了,原来我只写到了这里.那么这次就把它尽快写完:
// 主要是考虑: onUndoGame的时候如果有消除一块行,还是用原来的逻辑吗?
            
// (GloData.Instance.challengeLevel != 3)            
            Debug.Log(TAG + " (int)center.x: " + (int)center.x); 
            Debug.Log(TAG + " (int)center.z: " + (int)center.z);
// 不知道自己以前是什么神仙逻辑，下面这片代码就是看得头大看不懂在干什么。。。。
            tmp = 0;
            for (int j = 0; j <= (int)center.z; j++) {
                if (gridOcc[(int)center.x][y][j] != 1) continue; // 0 or 9
                ++tmp;
            }
            if (tmp == (int)center.z) { // zone 0 2
                tmp = 0;
                for (int i = 0; i <= (int)center.x; i++) {
                    if (gridOcc[i][y][(int)center.z] != 1) continue;
                    ++tmp;
                }
                if ((GloData.Instance.challengeLevel.Value == 4 && tmp == 4) || (GloData.Instance.challengeLevel.Value == 5 && tmp == 5)) activeZone[0] = 1; // zone 0 
                tmp = 0;
                for (int i = (int)center.x; i < gridXWidth; i++) {
                    if (gridOcc[i][y][(int)center.z] != 1) continue;
                    ++tmp;
                }
                if ((GloData.Instance.challengeLevel.Value == 4 && tmp == 4) || (GloData.Instance.challengeLevel.Value == 5 && tmp == 5)) activeZone[2] = 1; // zone 2
            }
            tmp = 0;
            for (int j = (int)center.z; j < gridZWidth; j++) {
                if (gridOcc[(int)center.x][y][j] != 1) continue; 
                ++tmp;
            }
            if ((GloData.Instance.challengeLevel.Value == 4 && tmp == 5) || (GloData.Instance.challengeLevel.Value == 5 && tmp == 3)) { // zone 1 3
                tmp = 0;
                for (int i = 0; i <= (int)center.x; i++) {
                    if (gridOcc[i][y][(int)center.z] != 1) continue;
                    ++tmp;
                }
                if ((GloData.Instance.challengeLevel.Value == 4 && tmp == 4) || (GloData.Instance.challengeLevel.Value == 5 && tmp == 5)) activeZone[1] = 1; // zone 0 
                tmp = 0;
                for (int i = (int)center.x; i < gridXWidth; i++) {
                    if (gridOcc[i][y][(int)center.z] != 1) continue;
                    ++tmp;
                }
                if ((GloData.Instance.challengeLevel.Value == 4 && tmp == 4) || (GloData.Instance.challengeLevel.Value == 5 && tmp == 5)) activeZone[3] = 1; // zone 2
            }
            zoneSum = 0;
            for (int i = 0; i < 4; i++) {
                zoneSum += activeZone[i];
                if (activeZone[i] == 1)
                    Debug.Log(TAG + " i: " + i); 
            }
            Debug.Log(TAG + " zoneSum: " + zoneSum);
// level 4: 保留了 return true            
            if (zoneSum == 0) return false;
            if (zoneSum == 2 && (!(activeZone[0] + activeZone[1] == 2 || 
                                   activeZone[0] + activeZone[2] == 2 ||
                                   activeZone[2] + activeZone[3] == 2 ||
                                   activeZone[1] + activeZone[3] == 2))) {
                return false;
            }
            // if (zoneSum == 4) { // Bug: TODO, NOT done yet, still need to check if the grids inside are all filled 这也是以前标记的,大概以前也就写到这里了
            //     for (int i = 0; i < 4; i++) {
            //         activeZone[i] = 0;
            //     }
            zoneSum = 0;
            if (GloData.Instance.challengeLevel.Value == 4) {
                if (activeZone[0] == 1) {
                    if (sumUpLeft((int)center.x, y, (int)center.z) == 10) {
                        ++zoneSum;
                    } else
                        activeZone[0] = 0;
                }
                if (activeZone[1] == 1) {
                    if (sumUpRight((int)center.x, y, (int)center.z) == 10) {
                        ++zoneSum;
                    } else 
                        activeZone[1] = 0;
                }
                if (activeZone[2] == 1) {
                    if (sumDownLeft((int)center.x, y, (int)center.z) == 10) {
                        ++zoneSum;
                    } else
                        activeZone[2] = 0;
                }
                if (activeZone[3] == 1) {
                    if (sumDownRight((int)center.x, y, (int)center.z) == 8) { 
                        ++zoneSum;
                    } else
                        activeZone[3] = 0;
                }
// level 4: return true 的情况会返回          
                if (zoneSum == 4) {
                    isFullQuadInLayer = true;
                    return true;
                }
                Debug.Log(TAG + " zoneSum: " + zoneSum); 
            }
            if (GloData.Instance.challengeLevel.Value == 5) {
                if (activeZone[0] == 1) {
                    Debug.Log(TAG + " sumUpLeft((int)center.x, y, (int)center.z): " + sumUpLeft((int)center.x, y, (int)center.z)); 
                    if (sumUpLeft((int)center.x, y, (int)center.z) == 11) {
                        ++zoneSum;
                    } else
                        activeZone[0] = 0;
                }
                if (activeZone[1] == 1) {
                    Debug.Log(TAG + " sumUpRight((int)center.x, y, (int)center.z): " + sumUpRight((int)center.x, y, (int)center.z)); 
                    if (sumUpRight((int)center.x, y, (int)center.z) == 8) {
                        ++zoneSum;
                    } else 
                        activeZone[1] = 0;
                }
                if (activeZone[2] == 1) {
                    Debug.Log(TAG + " sumDownLeft((int)center.x, y, (int)center.z): " + sumDownLeft((int)center.x, y, (int)center.z)); 
                    if (sumDownLeft((int)center.x, y, (int)center.z) == 8) {
                        ++zoneSum;
                    } else
                        activeZone[2] = 0;
                }
                if (activeZone[3] == 1) {
                    Debug.Log(TAG + " sumDownRight((int)center.x, y, (int)center.z): " + sumDownRight((int)center.x, y, (int)center.z)); 
                    if ( sumDownRight((int)center.x, y, (int)center.z) == 11) { 
                        ++zoneSum;
                    } else
                        activeZone[3] = 0;
                }
                Debug.Log(TAG + " zoneSum: " + zoneSum); 
// level 5: return true 的情况会返回          
                if (zoneSum == 4) {
                    isFullQuadInLayer = true;
                    return true;
                } 
            }
            Debug.Log(TAG + " zoneSum: " + zoneSum); 
            if (zoneSum == 0) return false;
            if (zoneSum == 2 && (!(activeZone[0] + activeZone[1] == 2 ||  // 这个限制需要吗？
                                   activeZone[0] + activeZone[2] == 2 ||
                                   activeZone[2] + activeZone[3] == 2 ||
                                   activeZone[1] + activeZone[3] == 2))) {
                return false;
            }
            Debug.Log(TAG + " zoneSum: " + zoneSum); 
            if (zoneSum >= 2 && zoneSum < 4) { // == 2 or 3, 这里有大量重复的工作
                tmp = 0;
                if (activeZone[0] + activeZone[1] == 2) { // 01
                    for (int i = 0; i <= (int)center.x; i++) {
                        for (int j = 0; j < gridZWidth; j++) {
                            if (gridOcc[i][y][j] != 1) continue;
                            ++tmp;
                        }
                    }
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 31 || tmp == 31 - sumLeft((int)center.x, y, (int)center.z) || tmp == 31 - sumRight((int)center.x, y, (int)center.z))) || 
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 28 || tmp == 28 - sumLeft((int)center.x, y, (int)center.z) || tmp == 28 - sumRight((int)center.x, y, (int)center.z)))) {
                        for (int i = 0; i <= (int)center.x; i++) {
                            for (int j = 0; j <= gridZWidth; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        }
                        return true;
                    } else return false;
                } else if (activeZone[0] + activeZone[2] == 2) { // 02
                    for (int i = 0; i < gridXWidth; i++) {
                        for (int j = 0; j <= (int)center.z; j++) {
                            if (gridOcc[i][y][j] != 1) continue;
                            ++tmp;
                        }
                    }
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 30 || tmp == 30 - sumUp((int)center.x, y, (int)center.z) || tmp == 30 - sumDown((int)center.x, y, (int)center.z))) || 
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 30 || tmp == 30 - sumUp((int)center.x, y, (int)center.z) || tmp == 30 - sumDown((int)center.x, y, (int)center.z)))) {
                        for (int i = 0; i < gridXWidth; i++) {
                            for (int j = 0; j <= (int)center.z; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        }
                        return true;
                    } else return false;
                } else if (activeZone[2] + activeZone[3] == 2) { // 23
                    for (int i = (int)center.x; i < gridXWidth; i++) {
                        for (int j = 0; j < gridZWidth; j++) {
                            if (gridOcc[i][y][j] != 1) continue;
                            ++tmp;
                        }
                    }
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 29 || tmp == 29 - sumLeft((int)center.x, y, (int)center.z) || tmp == 29 - sumRight((int)center.x, y, (int)center.z))) || 
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 28 || tmp == 28 - sumLeft((int)center.x, y, (int)center.z) || tmp == 28 - sumRight((int)center.x, y, (int)center.z)))) {
                        for (int i = (int)center.x; i < gridXWidth; i++) {
                            for (int j = 0; j < gridZWidth; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        }
                        return true;
                    } else return false;
                } else if (activeZone[1] + activeZone[3] == 2) { // 13
                    for (int i = 0; i < gridXWidth; i++) {
                        for (int j = (int)center.z; j < gridZWidth; j++) {
                            if (gridOcc[i][y][j] != 1) continue;
                            ++tmp;
                        }
                    }
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 30 || tmp == 30 - sumUp((int)center.x, y, (int)center.z) || tmp == 30 - sumDown((int)center.x, y, (int)center.z))) || 
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 30 || tmp == 30 - sumUp((int)center.x, y, (int)center.z) || tmp == 30 - sumDown((int)center.x, y, (int)center.z)))) {
                        for (int i = 0; i < gridXWidth; i++) {
                            for (int j = (int)center.z; j < gridZWidth; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        }
                    }
                    return true;
                } else return false;
            } else if (zoneSum == 1) {
                tmp = 0;
                if (activeZone[0] == 1) {
                    for (int i = 0; i <= (int)center.x; i++) 
                        for (int j = 0; j <= (int)center.z; j++) 
                            if (gridOcc[i][y][j] != 1)
                                ++tmp;
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 17 && (sumUpRight((int)center.x, y, (int)center.z) == 10 || sumDownLeft((int)center.x, y, (int)center.z) == 10))) ||
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 18 && (sumUpRight((int)center.x, y, (int)center.z) == 8 || sumDownLeft((int)center.x, y, (int)center.z) == 8)))) {
                        for (int i = 0; i <= (int)center.x; i++) 
                            for (int j = 0; j <= (int)center.z; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        if (sumUpRight((int)center.x, y, (int)center.z) >= 8)
                            resetUpRight((int)center.x, y, (int)center.z);
                        else if (sumDownLeft((int)center.x, y, (int)center.z) >= 8)
                            resetDownLeft((int)center.x, y, (int)center.z);
                        return true;
                    } else return false;
                } else if (activeZone[1] == 1) {
                    for (int i = 0; i <= (int)center.x; i++) 
                        for (int j = (int)center.z; j < gridZWidth; j++) 
                            if (gridOcc[i][y][j] != 1)
                                ++tmp;
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 18 && (sumUpLeft((int)center.x, y, (int)center.z) == 10 || sumDownRight((int)center.x, y, (int)center.z) == 8))) ||
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 15 && (sumUpLeft((int)center.x, y, (int)center.z) == 11 || sumDownRight((int)center.x, y, (int)center.z) == 11)))) {
                        for (int i = 0; i <= (int)center.x; i++) 
                            for (int j = (int)center.z; j < gridZWidth; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        if (sumUpLeft((int)center.x, y, (int)center.z) >= 10)
                            resetUpLeft((int)center.x, y, (int)center.z);
                        else if (sumDownRight((int)center.x, y, (int)center.z) >= 8)
                            resetDownRight((int)center.x, y, (int)center.z);
                        return true;
                    } else return false;
                } else if (activeZone[2] == 1) {
                    for (int i = (int)center.x; i < gridXWidth; i++) 
                        for (int j = 0; j <= (int)center.z; j++) 
                            if (gridOcc[i][y][j] != 1)
                                ++tmp;
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 17 && (sumUpLeft((int)center.x, y, (int)center.z) == 10 || sumDownRight((int)center.x, y, (int)center.z) == 8))) ||
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 15 && (sumUpLeft((int)center.x, y, (int)center.z) == 11 || sumDownRight((int)center.x, y, (int)center.z) == 11)))) {
                        for (int i = (int)center.x; i < gridXWidth; i++) 
                            for (int j = 0; j <= (int)center.z; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        if (sumUpLeft((int)center.x, y, (int)center.z) >= 10)
                            resetUpLeft((int)center.x, y, (int)center.z);
                        else if (sumDownRight((int)center.x, y, (int)center.z) >= 8)
                            resetDownRight((int)center.x, y, (int)center.z);
                        return true;
                    } else return false;
                } else {
                    for (int i = (int)center.x; i < gridXWidth; i++) 
                        for (int j = (int)center.z; j < gridZWidth; j++) 
                            if (gridOcc[i][y][j] != 1)
                                ++tmp;
                    Debug.Log(TAG + " tmp: " + tmp); 
                    if ((GloData.Instance.challengeLevel.Value == 4 && (tmp == 16 && (sumUpRight((int)center.x, y, (int)center.z) == 10 || sumDownLeft((int)center.x, y, (int)center.z) == 10))) ||
                        (GloData.Instance.challengeLevel.Value == 5 && (tmp == 18 && (sumUpRight((int)center.x, y, (int)center.z) == 8 || sumDownLeft((int)center.x, y, (int)center.z) == 8)))) {
                        for (int i = (int)center.x; i < gridXWidth; i++) 
                            for (int j = (int)center.z; j < gridZWidth; j++) {
                                if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                                gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                            }
                        if (sumUpRight((int)center.x, y, (int)center.z) >= 8)
                            resetUpRight((int)center.x, y, (int)center.z);
                        else if (sumDownLeft((int)center.x, y, (int)center.z) >= 8)
                            resetDownLeft((int)center.x, y, (int)center.z);
                        return true;
                    } else return false;
                }
            } else return false;
        }

        public static void resetUpLeft(int x, int y, int z) { // reset appeared
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < z; j++) {
                    if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                    gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                }
            }
        }
        public static void resetUpRight(int x, int y, int z) { // reset appeared
            for (int i = 0; i < x; i++) {
                for (int j = z + 1; j < gridZWidth; j++) {
                    if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                    gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                }
            }
        }
        public static void resetDownLeft(int x, int y, int z) { // reset appeared
            for (int i = x + 1; i < gridXWidth; i++) {
                for (int j = 0; j < z; j++) {
                    if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                    gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                }                
            }
        }
        public static void resetDownRight(int x, int y, int z) { // reset appeared
            for (int i = x + 1; i < gridXWidth; i++) {
                for (int j = z + 1; j < gridZWidth; j++) {
                    if (gridOcc[i][y][j] == 0 || gridOcc[i][y][j] == 9) continue; 
                    gridOcc[i][y][j] += gridOcc[i][y][j] == 1 ? 1 : 0;
                }
            }
        }

        public static int sumUpLeft(int x, int y, int z) { // sum appeared
            int res = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < z; j++) {
                    if (gridOcc[i][y][j] == 1)
                        ++res;
                }
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumUpRight(int x, int y, int z) { // sum appeared
            int res = 0;
            for (int i = 0; i < x; i++) {
                for (int j = z + 1; j < gridZWidth; j++) {
                    if (gridOcc[i][y][j] == 1)
                        ++res;
                }
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumDownLeft(int x, int y, int z) { // sum appeared
            int res = 0;
            for (int i = x + 1; i < gridXWidth; i++) {
                for (int j = 0; j < z; j++) {
                    if (gridOcc[i][y][j] == 1)
                        ++res;
                }                
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumDownRight(int x, int y, int z) { // sum appeared
            int res = 0;
            for (int i = x + 1; i < gridXWidth; i++) {
                for (int j = z + 1; j < gridZWidth; j++) {
                    if (gridOcc[i][y][j] == 1)
                        ++res;
                }
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumLeft(int x, int y, int z) { // sum missing
            int res = 0;
            for (int j = 0; j < z; j++) {
                if (gridOcc[x][y][j] != 1)
                    ++res;
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumRight(int x, int y, int z) {
            int res = 0;
            for (int j = z + 1; j < gridZWidth; j++) {
                if (gridOcc[x][y][j] != 1)
                    ++res;
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumUp(int x, int y, int z) {
            int res = 0;
            for (int i = 0; i < x; i++) {
                if (gridOcc[i][y][z] != 1)
                    ++res;
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        public static int sumDown(int x, int y, int z) {
            int res = 0;
            for (int i = x + 1; i < gridXWidth; i++) {
                if (gridOcc[i][y][z] != 1)
                    ++res;
            }
            Debug.Log(TAG + " res: " + res); 
            return res;
        }
        
        public static bool CheckIsInsideGrid(Vector3 pos) {
            return ((int)pos.x >= 0 && (int)pos.x < gridXWidth &&
                    (int)pos.z >= 0 && (int)pos.z < gridZWidth && 
                    (int)pos.y >= 0 && (int)pos.y < gridHeight); 
        }
        
        public static bool CheckIsInsideBarials(Vector3 pos) {
            // Debug.Log(TAG + ": CheckIsInsideBarials()");
            int x = (int)pos.x;
            int y = (int)pos.y;
            int z = (int)pos.z;
            return (gridOcc[x][y][z] == 9);
        }

// 这里:不是很懂什么时候会出现那个菜单 for tmp
        public void UpdateLevel() {
            // if (GameController.startingAtLevelZero || (!GameController.startingAtLevelZero && MainScene_ScoreManager.numLinesCleared / 10 > GameController.startingLevel)) 
            //     GameController.currentLevel = MainScene_ScoreManager.numLinesCleared / 10;
        }
        public void UpdateSpeed() { 
            // GameController.fallSpeed = 1.0f - (float)GameController.currentLevel * 0.1f;
        }

        public static string GetRandomTetromino() { // active Tetromino
            // Debug.Log(TAG + ": GetRandomTetromino()"); 
            if (GloData.Instance.gameMode.Value == 0 && gridWidth == 3)
                randomTetromino = UnityEngine.Random.Range(0, 11);
            else 
                randomTetromino = UnityEngine.Random.Range(0, 12);
			// Debug.Log(TAG + " randomTetromino: " + randomTetromino); 
			type.Length = 0;
            type.Append("Tetromino");
            switch (randomTetromino) {
            case 0: type.Append("0"); break;
            case 1: type.Append("C"); break;
            case 2: type.Append("Z"); break; 
            case 3: type.Append("L"); break;
            case 4: type.Append("S"); break;
            case 5: type.Append("O"); break;
            case 6: type.Append("T"); break;
            case 7: type.Append("B"); break;
            case 8: type.Append("R"); break;
            case 9: type.Append("Y"); break;
            case 10:type.Append("J"); break;
            default: // 
                type.Append("I"); break;
            }
            return type.ToString();
        }
        public static string GetGhostTetrominoType(GameObject gameObject) { // ViewManager.ghostTetromino
            Debug.Log(TAG + ": GetGhostTetrominoType()");
			type.Length = 0;
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            string tmp = gameObject.name.Substring(10, 1);
            switch(tmp) {
            case "0" : type.Append("shadow0"); break;
            case "B" : type.Append("shadowB"); break;
            case "C" : type.Append("shadowC"); break;
            case "I" : type.Append("shadowI"); break;
            case "J" : type.Append("shadowJ"); break;
            case "L" : type.Append("shadowL"); break;
            case "O" : type.Append("shadowO"); break;
            case "R" : type.Append("shadowR"); break;
            case "S" : type.Append("shadowS"); break;
            case "T" : type.Append("shadowT"); break;
            case "Y" : type.Append("shadowY"); break;
            case "Z" : type.Append("shadowZ"); break;
            }
            return type.ToString(); 
        }    
    }
}
