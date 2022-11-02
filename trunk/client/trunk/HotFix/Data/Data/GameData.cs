using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using HotFix.Control;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Data {

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
        public Control.TetrominoData nextTetrominoData; // 大方格中的当前方块砖
        public List<MinoData> grid;             // 大方格中的所有先前数据
        public List<Control.TetrominoData> parentList;  // 如果有方块砖链表,那么链表中的方块砖有可能是残缺的(因为游戏过程中的消除行与列等)
        public bool saveForUndo; // 区分教育模式与经典模式 
        
        public GameData (GameViewModel viewModel) { // viewModel: game
            gameMode = viewModel.gameMode.Value;
            score = viewModel.currentScore.Value;
            level = viewModel.currentLevel.Value;
            lines = viewModel.numLinesCleared.Value;

            if (gameMode == 0) {
                prevPreview = viewModel.prevPreview;
                prevPreview2 = viewModel.prevPreview2;
            }
            nextTetrominoType = viewModel.nextTetrominoType.Value;
            previewTetrominoType = viewModel.previewTetrominoType;
            previewTetromino2Type = viewModel.previewTetromino2Type;
            saveForUndo = viewModel.saveForUndo;
            
            grid = new List<MinoData>();
			parentList = new List<Control.TetrominoData>();
            int listSize = viewModel.gridHeight * viewModel.gridWidth * viewModel.gridWidth;
            
            bool isCurrentlyActiveTetromino = false;
            for (int i = 0; i < listSize; i++) {
                MinoData tmpMino = new MinoData(viewModel.tmpTransform);
                tmpMino.reset();
                grid.Add(tmpMino);
            }
        
            //  nextTetromino: May have landed already, may have been destroyed right after undo clicked
            if (ViewManager.nextTetromino != null) {
                isCurrentlyActiveTetromino = ViewManager.nextTetromino.CompareTag("currentActiveTetromino");
                if (ViewManager.nextTetromino != null && isCurrentlyActiveTetromino) { // 没着陆
					nextTetrominoData = new TetrominoData(ViewManager.nextTetromino.transform, nextTetrominoType, ViewManager.nextTetromino.gameObject.name);
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
                if (viewModel.grid[x][y][z] != null && viewModel.grid[x][y][z].parent != null 
                    && (ViewManager.ghostTetromino == null || (viewModel.grid[x][y][z].parent != ViewManager.ghostTetromino.transform))

                    && ((saveForUndo && viewModel.grid[x][y][z].parent != ViewManager.nextTetromino.transform) ||                                          // for undo, flag
                        // && (viewModel.grid[x][y][z].parent != ViewManager.nextTetromino.transform || !ViewManager.nextTetromino.CompareTag("currentActiveTetromino")) // for regular game Load
                        (!saveForUndo && (!isCurrentlyActiveTetromino || (ViewManager.nextTetromino != null && viewModel.grid[x][y][z].parent != ViewManager.nextTetromino.transform)))) // for regular game Load

                    && (!myContains(viewModel.grid[x][y][z].parent))) {

					Control.TetrominoData tmp = new Control.TetrominoData(viewModel.grid[x][y][z].parent,
                                                          new StringBuilder("shape").Append(viewModel.grid[x][y][z].parent.gameObject.name.Substring(10, 1)).ToString(),
                                                          viewModel.grid[x][y][z].parent.gameObject.name);
                        
                    Debug.Log(TAG + " viewModel.grid[x][y][z].parent.gameObject.name (in parentList): " + viewModel.grid[x][y][z].parent.gameObject.name);
                    Debug.Log(TAG + " viewModel.grid[x][y][z].parent.childCount: " + viewModel.grid[x][y][z].parent.childCount); 
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
           using (List<Control.TetrominoData>.Enumerator enumerator = parentList.GetEnumerator()) {
               while (enumerator.MoveNext()) {
					Control.TetrominoData data = enumerator.Current;
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
