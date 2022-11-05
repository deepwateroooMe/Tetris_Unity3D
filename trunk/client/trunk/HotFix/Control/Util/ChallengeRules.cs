using deepwaterooo.tetris3d;
using Framework.MVVM;
using HotFix.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {
    public static class ChallengeRules {
        private const string TAG = "challengeRules";

        private static int n = Model.gridHeight;
        
        private static int [] bottomIdx = new int[5];
        // private static int [] bottomIdx = new int[];
        
        private static int matchingCnter = 0;
        private static bool isSolo = false;
        private static Vector3 [] rotations = new Vector3[12]{
            new Vector3(0f, 90f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0f, -90f, 0f), new Vector3(0f, -180f, 0f), // y
            new Vector3(90f, 0f, 0f), new Vector3(180f, 0f, 0f), new Vector3(-90f, 0f, 0f), new Vector3(-180f, 0f, 0f), // x
            new Vector3(0f, 0f, 90f), new Vector3(0f, 0f, 180f), new Vector3(0f, 0f, -90f), new Vector3(0f, 0f, -180f)  // z   
        };
        
        public static bool isValidLandingPosition() {
            Debug.Log(TAG + ": isValidLandingPosition()");
            if (ViewManager.nextTetromino.transform.childCount == 1)
                isSolo = true;

            matchingCnter = 0;
            if (isTetrominoMatchingNeighbour()) return true;

            // check bottom layer if bottom minos color match
            if (isBottomLayerSkinMatches()) {
                return true;
            }
            if (GloData.Instance.challengeLevel < 11 && !isColorExistOnContactableBoard(ViewManager.nextTetromino.GetComponent<TetrominoType>().color))
                return true;
            else if (GloData.Instance.challengeLevel > 10 && !isThereSolutionOnBoard()) // 搜索确认是否 存在合理解
                return true;
            return false;
        }

        public static bool isTetrominoMatchingNeighbour() {
            Debug.Log(TAG + ": isTetrominoMatchingNeighbour()");
            bool isMatching = false;
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                isMatching = false;
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                    int x = (int)Mathf.Round(pos.x);
                    int y = (int)Mathf.Round(pos.y);
                    int z = (int)Mathf.Round(pos.z);
                    Debug.Log(TAG + " x: " + x); 
                    Debug.Log(TAG + " y: " + y); 
                    Debug.Log(TAG + " z: " + z);
                    Debug.Log(TAG + " (!isNeighboursExist(x, y, z)): " + (!isNeighboursExist(x, y, z)));
                    if (!isNeighboursExist(x, y, z)) continue; // 这个格有效，检查下一个格
                    isMatching = isMatchingAnyNeighbour(x, y, z);
                    Debug.Log(TAG + " isMatchingAnyNeighbour(): " + isMatching); 
                    if (isMatching) {
                        ++matchingCnter;

                        Debug.Log(TAG + " matchingCnter: " + matchingCnter);
                        Debug.Log(TAG + " GloData.Instance.challengeLevel: " + GloData.Instance.challengeLevel);
                        Debug.Log(TAG + " isSolo: " + isSolo);
                        Debug.Log(TAG + " (GloData.Instance.challengeLevel > 10 && matchingCnter >= 2): " + (GloData.Instance.challengeLevel > 10 && matchingCnter >= 2)); 

                        if (GloData.Instance.challengeLevel < 11 || isSolo
                            || (GloData.Instance.challengeLevel > 10 && matchingCnter >= 2)) {
                            Debug.Log(TAG + ": return true"); 
                            return true;
                        }
                    } 
                }
            }
            return false;
        }
        
        public static bool isThereSolutionOnBoard() { // 暴力 查找 合理解
            Debug.Log(TAG + ": isThereSolutionOnBoard()"); 
            int x = 0, y = 0, z = 0;
            x = (int)Mathf.Round(ViewManager.nextTetromino.transform.position.x);
            y = (int)Mathf.Round(ViewManager.nextTetromino.transform.position.y);
            z = (int)Mathf.Round(ViewManager.nextTetromino.transform.position.z);
            bool isMatching = false;
            Vector3 delta = Vector3.zero;

            for (int i = 0; i < Model.gridXWidth; i++) {
                for (int j = 0; j < Model.gridHeight; j++) {
                    for (int k = 0; k < Model.gridZWidth; k++) {
                        isMatching = false;
                        matchingCnter = 0;
                        delta = new Vector3(i-x, j-y, k-z);
                        if (delta != Vector3.zero) {
                            ViewManager.nextTetromino.transform.position += delta; // 平移
                            if (Model.CheckIsValidPosition()) {
                                if (isTetrominoMatchingNeighbour()) {// y-1 >= 0
                                    ViewManager.nextTetromino.transform.position -= delta; // 平移
                                    return true;
                                }
                                getBottomLayerMinosIdx();
                                if (getMinYLayerIdx() == 0) {  // 检查最底层: 特殊状态优先处理
                                    if (isExistInBaseCubes()) { // currentAcitiveTetromino: 无平移 无旋转
                                        ViewManager.nextTetromino.transform.position -= delta; // 平移
                                        return true;
                                    }
                                }
                            }
                            // 再进行旋转查找
                            if (isTetrominoMatchingNeighbourAfterRotation()) // 12
                                return true;
                            ViewManager.nextTetromino.transform.position -= delta;
                        }
                    }
                }
            }
            Debug.Log(TAG + ": no solution"); 
            return false;
        }
        
        public static bool isTetrominoMatchingNeighbourAfterRotation() { // 旋转
            Debug.Log(TAG + ": isTetrominoMatchingNeighbourAfterRotation()");
            for (int i = 0; i < rotations.Length; i++) { // todo: 这个最好对每个 tetromino 进行优化处理，而不是每个都检查12次
                matchingCnter = 0;
                ViewManager.nextTetromino.transform.Rotate(rotations[i]); // 旋转有大量浪费计算，还需要考虑平移: 但仍然没有包括所有的landing可能性
                if (!Model.CheckIsValidPosition()) {
                    ViewManager.nextTetromino.transform.Rotate(-rotations[i]);
                    continue;
                } else {
                    if (isTetrominoMatchingNeighbour()) return true; 

                    getBottomLayerMinosIdx();
                    if (getMinYLayerIdx() == 0) {  // 检查最底层: 特殊状态优先处理
                        if (isExistInBaseCubes()) { // currentAcitiveTetromino: 无平移 无旋转
                            ViewManager.nextTetromino.transform.Rotate(-rotations[i]);
                            return true;
                        }
                    }
                    ViewManager.nextTetromino.transform.Rotate(-rotations[i]);
                }
            }
            return false;
        }

        public static bool isBottomLayerSkinMatches () {
            Debug.Log(TAG + ": isBottomLayerSkinMatches()"); 
            getBottomLayerMinosIdx();
            for (int i = 0; i < 4; i++) {
                if (bottomIdx[i] == -1) {
                    // if (GloData.Instance.challengeLevel < 11)
                    return false;
                    // else continue;
                } 
                int [] pos = MathUtil.getIndex(bottomIdx[i]);
                MathUtil.print(pos);
                if (pos[1] == 0) {
                    Debug.Log(TAG + " (Model.baseCubes[getMinoPosCubeArrIndex(pos[0], pos[2])] == Model.grid[pos[0]][pos[1]][pos[2]].gameObject.GetComponent<MinoType>().color): "
                              + (Model.baseCubes[getMinoPosCubeArrIndex(pos[0], pos[2])] == Model.grid[pos[0]][pos[1]][pos[2]].gameObject.GetComponent<MinoType>().color)); 
                    if (Model.baseCubes[getMinoPosCubeArrIndex(pos[0], pos[2])] == Model.grid[pos[0]][pos[1]][pos[2]].gameObject.GetComponent<MinoType>().color) {
                        ++matchingCnter;
                        Debug.Log(TAG + " matchingCnter: " + matchingCnter); 
                        if (GloData.Instance.challengeLevel < 11 || isSolo || (GloData.Instance.challengeLevel > 10 && matchingCnter >= 2)) {
                            return true;
                        }
                    }
                }
                // else if (pos[1] > 0 && Model.grid[pos[0]][pos[1]][pos[2]].gameObject.GetComponent<MinoType>().color == Model.grid[pos[0], pos[1]-1, pos[2]].gameObject.GetComponent<MinoType>().color)
                //     return true;
            }
            Debug.Log(TAG + " matchingCnter: " + matchingCnter); 
            // 当 board 中第四种色彩时，还需要进一步再检测
            return false;
        }

        public static bool isExistInBaseCubes() {
            int n = Model.gridXWidth * Model.gridZWidth;
            int x = 0, y = 0, z = 0, cnt = 0;
            for (int i = 0; i < 5; i++) {
                if (bottomIdx[i] > -1) { 
                    x = bottomIdx[i] % Model.gridXWidth;
                    z = bottomIdx[i] / Model.gridXWidth;
                    if (getMinoColorFromRotatedTetromino(x, 0, z) == Model.baseCubes[x + Model.gridXWidth * z]) {
                        ++cnt;
                        if (cnt >= 2) return true;
                    }
                }
            }
            return false;
        }
        public static int getMinoColorFromRotatedTetromino(int x, int y, int z) {
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (Mathf.Round(mino.position.x) == x && Mathf.Round(mino.position.y) == y && Mathf.Round(mino.position.z) == z)
                    return mino.gameObject.GetComponent<MinoType>().color;
                
            }
            return -1;
        }
        
        public static void getBottomLayerMinosIdx() {
            // Debug.Log(TAG + ": getBottomLayerMinosIdx()"); 
            clearBottomIdxArray();
            int minY = getMinYLayerIdx();
            Debug.Log(TAG + " minY: " + minY); 
            int i = 0;
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                    if ((int)pos.y == minY) {
                        // bottomIdx[i++] = MathUtil.getIndex(pos);
                        bottomIdx[i] = MathUtil.getIndex(pos);
                        Debug.Log(TAG + " bottomIdx[i]: " + bottomIdx[i]);
                        i++;
                    }
                }
            }
        }
        public static int getMinYLayerIdx() {
            Debug.Log(TAG + ": getMinYLayerIdx()"); 
            int minY = Model.gridHeight - 1;
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (Mathf.Round(mino.position.y) < minY) 
                    minY = (int)Mathf.Round(mino.position.y);
            }
            return minY;
        }
        public static void clearBottomIdxArray() {
            Debug.Log(TAG + ": clearBottomIdxArray()"); 
            if (bottomIdx != null) {
                Array.Clear(bottomIdx, 0, 5);
                for (int i = 0; i < 5; i++) {
                    bottomIdx[i] = -1;
                }
            }
        }
        
        public static bool isColorExistOnContactableBoard(int color) {
            Debug.Log(TAG + ": isColorExistOnContactableBoard()");
            Debug.Log(TAG + " color: " + color);
            
            getBottomLayerMinosIdx();

            // check if exist in baseCubes
            Debug.Log(TAG + " isExistInBaseCubes(color): " + isExistInBaseCubes(color)); 
            if (bottomIdx[0] == 0 && isExistInBaseCubes(color)) return true;

            // check if exist in contactable Higher layers: todo
            Debug.Log(TAG + " isExistInHigherLayers(color): " + isExistInHigherLayers(color)); 
            if (bottomIdx[0] > 0 && isExistInHigherLayers(color))
                return true;
            // else {
            //     // todo: according to challenge level, permit landing or not
            // }
            return false;
        }
        public static bool isExistInHigherLayers(int color) { 
            int j = Model.gridHeight - 1;
            for (int i = 0; i < Model.gridXWidth; ++i) {
                for (int k = 0; k < Model.gridZWidth; k++) {
                    j = Model.gridHeight - 1;
                    // Model.grid[i, j, k] = 0, otherwose gameover
                    while (j > 0 && Model.grid[i][j][k] == null) j--;
                    if (j > 0 && Model.gridClr[i][j][k] == color && Model.grid[i][j][k].parent.gameObject != ViewManager.nextTetromino) {
                        Debug.Log(TAG + ": i, j, k values: "); 
                        MathUtil.print(i, j, k);
                        // Debug.Log(TAG + " j: " + j); 
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static bool isExistInBaseCubes(int color) {
            int n = Model.gridXWidth * Model.gridZWidth;
            for (int i = 0; i < n; i++) {
                if (Model.baseCubes[i] == color)
                    return true;
            }
            return false;
        }

        public static bool isMatchingAnyNeighbour(int x, int y, int z) { // getting the values through a hard way, get from board faster ?
            Debug.Log(TAG + ": isMatchingAnyNeighbour()"); 
            
            if (x-1 >= 0 && Model.grid[x-1][y][z] != null
                && Model.grid[x-1][y][z].parent.gameObject != ViewManager.nextTetromino) {
                // if (x-1 >= 0 && Model.GetTransformAtGridPosition(new Vector3(x-1, y, z)) != null
                // && Model.GetTransformAtGridPosition(new Vector3(x-1, y, z)).parent.gameObject != ViewManager.nextTetromino) {
                // if (Model.GetTransformAtGridPosition(new Vector3(x-1, y, z)).gameObject.GetComponent<MinoType>().color
                if (Model.grid[x-1][y][z].gameObject.GetComponent<MinoType>().color
                    == Model.grid[x][y][z].gameObject.GetComponent<MinoType>().color)
                    return true;
            }
            if (x+1 < Model.gridXWidth && Model.grid[x+1][y][z] != null
                && Model.grid[x+1][y][z].parent.gameObject != ViewManager.nextTetromino) {
                if (Model.grid[x+1][y][z].gameObject.GetComponent<MinoType>().color
                    == Model.grid[x][y][z].gameObject.GetComponent<MinoType>().color)
                    return true;
            }
            if (z-1 >= 0 && Model.grid[x][y][z-1] != null
                && Model.grid[x][y][z-1].parent.gameObject != ViewManager.nextTetromino) {
                if (Model.grid[x][y][z-1].gameObject.GetComponent<MinoType>().color
                    == Model.grid[x][y][z].gameObject.GetComponent<MinoType>().color)
                    return true;
            }
            if (z+1 < Model.gridZWidth && Model.grid[x][y][z+1] != null
                && Model.grid[x][y][z+1].parent.gameObject != ViewManager.nextTetromino) {
                if (Model.grid[x][y][z+1].gameObject.GetComponent<MinoType>().color
                    == Model.grid[x][y][z].gameObject.GetComponent<MinoType>().color)
                    return true;
            }
            if (y-1 >= 0 && Model.grid[x][y-1][z] != null
                && Model.grid[x][y-1][z].parent.gameObject != ViewManager.nextTetromino) {
                if (Model.grid[x][y-1][z].gameObject.GetComponent<MinoType>().color
                    == Model.grid[x][y][z].gameObject.GetComponent<MinoType>().color)
                    return true;
            }
            return false;
        }
        
        public static bool isNeighboursExist(int x, int y, int z) {
            if (x-1 >= 0 && Model.GetTransformAtGridPosition(new Vector3(x-1, y, z)) != null
                && Model.GetTransformAtGridPosition(new Vector3(x-1, y, z)).parent.gameObject != ViewManager.nextTetromino)
                return true;
            if (x+1 < Model.gridXWidth && Model.GetTransformAtGridPosition(new Vector3(x+1, y, z)) != null
                && Model.GetTransformAtGridPosition(new Vector3(x+1, y, z)).parent.gameObject != ViewManager.nextTetromino)
                return true;
            if (z-1 >= 0 && Model.GetTransformAtGridPosition(new Vector3(x, y, z-1)) != null
                && Model.GetTransformAtGridPosition(new Vector3(x, y, z-1)).parent.gameObject != ViewManager.nextTetromino)
                return true;
            if (z+1 < Model.gridZWidth && Model.GetTransformAtGridPosition(new Vector3(x, y, z+1)) != null
                && Model.GetTransformAtGridPosition(new Vector3(x, y, z+1)).parent.gameObject != ViewManager.nextTetromino)
                return true;
            if (y-1 >= 0 && Model.GetTransformAtGridPosition(new Vector3(x, y-1, z)) != null
                && Model.GetTransformAtGridPosition(new Vector3(x, y-1, z)).parent.gameObject != ViewManager.nextTetromino)
                return true;
            return false;
        }

        static int getMinoPosCubeArrIndex(float x, float z) {
            return (int)(Mathf.Round(x) + Model.gridXWidth * Mathf.Round(z)); // y = 0
        }
    }
}
