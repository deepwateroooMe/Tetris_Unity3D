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

        public string prevPreview;
        public string prevPreview2;
        public string nextTetrominoType;    
        public string previewTetrominoType; 
        public string previewTetromino2Type;
    
        public SerializedTransform cameraData;
        public TetrominoData nextTetrominoData; 
        public List<MinoData> grid;
        public List<TetrominoData> parentList;
        public bool saveForUndo;
        
        public GameData (Game game) { 
            gameMode = game.gameMode;
            score = Game.currentScore;
            level = game.currentLevel;
            lines = game.numLinesCleared;

            if (game.gameMode == 0) {
                prevPreview = game.prevPreview;
                prevPreview2 = game.prevPreview2;
            }
            nextTetrominoType = game.nextTetrominoType;
            previewTetrominoType = game.previewTetrominoType;
            previewTetromino2Type = game.previewTetromino2Type;
            saveForUndo = game.saveForUndo;
            
            grid = new List<MinoData>();
            parentList = new List<TetrominoData>();
            int listSize = Game.gridHeight * Game.gridWidth * Game.gridWidth;
            
            bool isCurrentlyActiveTetromino = false;
            for (int i = 0; i < listSize; i++) {
                MinoData tmpMino = new MinoData(game.tmpTransform);
                tmpMino.reset();
                grid.Add(tmpMino);
            }
        
            //  nextTetromino: May have landed already, may have been destroyed right after undo clicked
            if (Game.nextTetromino != null) {
                isCurrentlyActiveTetromino = Game.nextTetromino.CompareTag("currentActiveTetromino");
                if (Game.nextTetromino != null && isCurrentlyActiveTetromino) { // 没着陆
                    nextTetrominoData = new TetrominoData(Game.nextTetromino.transform, nextTetrominoType, Game.nextTetromino.gameObject.name);
                }
            }
            
            // dealing with Game Data: gird
            int [] pos = new int[3];
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < listSize; i++) {
                pos = MathUtil.getIndex(i);
                x = pos[0];
                y = pos[1];
                z = pos[2];
                if (Game.grid[x, y, z] != null && Game.grid[x, y, z].parent != null 
                    && (Game.ghostTetromino == null || (Game.grid[x, y, z].parent != Game.ghostTetromino.transform))

                    && ((saveForUndo && Game.grid[x, y, z].parent != Game.nextTetromino.transform) ||                                          // for undo, flag
                    // && (Game.grid[x, y, z].parent != Game.nextTetromino.transform || !Game.nextTetromino.CompareTag("currentActiveTetromino")) // for regular game Load
                        (!saveForUndo && (!isCurrentlyActiveTetromino || (Game.nextTetromino != null && Game.grid[x, y, z].parent != Game.nextTetromino.transform)))) // for regular game Load

                        && (!myContains(Game.grid[x, y, z].parent))) {

                        TetrominoData tmp = new TetrominoData(Game.grid[x, y, z].parent,
                                                              new StringBuilder("shape").Append(Game.grid[x, y, z].parent.gameObject.name.Substring(10, 1)).ToString(),
                                                              Game.grid[x, y, z].parent.gameObject.name);
                        
                        Debug.Log(TAG + " Game.grid[x, y, z].parent.gameObject.name (in parentList): " + Game.grid[x, y, z].parent.gameObject.name);
                        Debug.Log(TAG + " Game.grid[x, y, z].parent.childCount: " + Game.grid[x, y, z].parent.childCount); 
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