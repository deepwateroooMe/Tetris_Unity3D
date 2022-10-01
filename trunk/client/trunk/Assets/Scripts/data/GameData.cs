using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text;

namespace tetris3d {

    [System.Serializable]
    public class GameData {
        private const string TAG = "GameData";
    
        public int gameMode;
        public int score;
        public int level;
        public int lines;

        public string nextTetrominoType;    
        public string prevPreview;
        public string prevPreview2;
        public string previewTetrominoType; 
        public string previewTetromino2Type;
        public int prevPreviewColor;
        public int prevPreviewColor2;
        public int previewTetrominoColor; 
        public int previewTetromino2Color;
    
        public SerializedTransform cameraData;
        public TetrominoData nextTetrominoData; 
        public List<MinoData> grid;
        public List<TetrominoData> parentList;

        public bool saveForUndo;
        public bool isChallengeMode;
        
        public GameData (GameController gameController) {
            gameMode = GameController.gameMode;
            isChallengeMode = GameController.isChallengeMode;
            
            score = MainScene_ScoreManager.currentScore;
            level = MainScene_ScoreManager.currentLevel;
            lines = MainScene_ScoreManager.numLinesCleared;

            saveForUndo = gameController.saveForUndo;

            nextTetrominoType = gameController.nextTetrominoType;
            previewTetrominoType = gameController.previewTetrominoType;
            previewTetromino2Type = gameController.previewTetromino2Type;
            if (isChallengeMode) {
                previewTetrominoColor = gameController.previewTetrominoColor;
                previewTetromino2Color = gameController.previewTetromino2Color;
            }
            if (GameController.gameMode == 0) {
                prevPreview = gameController.prevPreview;
                prevPreview2 = gameController.prevPreview2;
                if (isChallengeMode) {
                    prevPreviewColor = gameController.prevPreviewColor;
                    prevPreviewColor2 = gameController.prevPreviewColor2;
                }
            }
            
            grid = new List<MinoData>();
            parentList = new List<TetrominoData>();
            int listSize = Model.gridHeight * Model.gridWidth * Model.gridWidth;
            
            bool isCurrentlyActiveTetromino = false;
            for (int i = 0; i < listSize; i++) {
                MinoData tmpMino = new MinoData(gameController.tmpTransform); 
                tmpMino.reset();
                grid.Add(tmpMino);
            }
        
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0, color = -1;

            //  nextTetromino: May have landed already, may have been destroyed right after undo clicked
            if (GameController.nextTetromino != null) {
                isCurrentlyActiveTetromino = GameController.nextTetromino.CompareTag("currentActiveTetromino");
                if (GameController.nextTetromino != null && isCurrentlyActiveTetromino) { // 没着陆
                    foreach (Transform mino in GameController.nextTetromino.transform) {
                        if (mino.CompareTag("mino")) {
                            x = (int)Mathf.Round(mino.position.x);
                            y = (int)Mathf.Round(mino.position.y);
                            z = (int)Mathf.Round(mino.position.z);
                            color = Model.gridClr[x, y, z];
                            break;
                        }
                    }
                    nextTetrominoData = new TetrominoData(GameController.nextTetromino.transform, nextTetrominoType, GameController.nextTetromino.gameObject.name, color);
                }
            }
            
            // dealing with Game Data: gird
            for (int i = 0; i < listSize; i++) {
                pos = MathUtil.getIndex(i);
                x = pos[0];
                y = pos[1];
                z = pos[2];
                if (Model.grid[x, y, z] != null && Model.grid[x, y, z].parent != null 
                    && (GameController.ghostTetromino == null || (Model.grid[x, y, z].parent != GameController.ghostTetromino.transform))

                    && ((saveForUndo && Model.grid[x, y, z].parent != GameController.nextTetromino.transform) ||                                          // for undo, flag
                        // && (GameController.grid[x, y, z].parent != GameController.nextTetromino.transform || !GameController.nextTetromino.CompareTag("currentActiveTetromino")) // for regular game Load
                        (!saveForUndo && (!isCurrentlyActiveTetromino || (GameController.nextTetromino != null && Model.grid[x, y, z].parent != GameController.nextTetromino.transform)))) // for regular game Load

                    && (!myContains(Model.grid[x, y, z].parent))) {

                    color = Model.gridClr[x, y, z];
                    Debug.Log(TAG + " color: " + color); 
                    TetrominoData tmp = new TetrominoData(Model.grid[x, y, z].parent,
                                                          new StringBuilder("shape").Append(Model.grid[x, y, z].parent.gameObject.name.Substring(10, 1)).ToString(),
                                                          Model.grid[x, y, z].parent.gameObject.name, color);
                        
                    Debug.Log(TAG + " Model.grid[x, y, z].parent.gameObject.name (in parentList): " + Model.grid[x, y, z].parent.gameObject.name);
                    Debug.Log(TAG + " Model.grid[x, y, z].parent.childCount: " + Model.grid[x, y, z].parent.childCount); 
                    Debug.Log(TAG + " tmp.children.Count (in saved parent TetrominoData): " + tmp.children.Count); 
                    foreach (MinoData mino in tmp.children) {
                        MathUtil.print(MathUtil.getIndex(mino.idx));
                    }
                    parentList.Add(tmp);
                }
                cameraData = new SerializedTransform(Camera.main.transform);
            }
        }        
        
        private bool myContains(Transform tmp) { // 目前只比较了parent，later是有可能需要比较children的
            using (List<TetrominoData>.Enumerator enumerator = parentList.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    TetrominoData data = enumerator.Current;
                    if (tmp.gameObject.name == data.name & // 不同名字、形状的两个Tetromino 可以有相同的 pos rot
                        tmp.position == DeserializedTransform.getDeserializedTransPos(data.transform) &&
                        tmp.rotation == DeserializedTransform.getDeserializedTransRot(data.transform))
                        return true;
                }
            }
            return false;
        }
    }   
}