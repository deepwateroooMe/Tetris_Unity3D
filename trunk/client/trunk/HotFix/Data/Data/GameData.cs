using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.Data.Data {

    [System.Serializable]
    public class GameData {
        private const string TAG = "GameData";
    
        public int gameMode; // 这里把几种不同的游戏模式当作几个不同的(场景)来处理?
        public int score; // 当前游戏进展数据: 得分,级别,消除过的行数等
        public int level;
        public int lines;

        // 这里始终是以字符串来标记游戏场景里可能会存在的各种物件: 游戏面板大方格里的所有数据,下一(两)个方块砖的类型等 
        public string prevPreview;
        public string prevPreview2;
        public string nextTetrominoType;    
        public string previewTetrominoType; 
        public string previewTetromino2Type;
    
        public SerializedTransform cameraData;  // 相机数据
        public TetrominoData nextTetrominoData; // 大方格中的当前方块砖
        public List<MinoData> grid;             // 大方格中的所有先前数据
        public List<TetrominoData> parentList;  // 如果有方块砖链表,那么链表中的方块砖有可能是残缺的(因为游戏过程中的消除行与列等)
        public bool saveForUndo; // 区分教育模式与经典模式 
        
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
