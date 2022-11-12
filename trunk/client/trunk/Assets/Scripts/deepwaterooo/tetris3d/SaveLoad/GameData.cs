using System.Collections.Generic;
using System.Text;
using Framework.MVVM;
using UnityEngine;

namespace deepwaterooo.tetris3d {

// 当我把含有父子嵌套关系的MinoData, MinoDataCollectio 和 TetrominoData放在主工程中,不会再有IList<T> 接口不适配的问题
// 但是,存在其它问题,有:ILRuntime中默认是不使用BinaryFormator的,这个序列化反序列化包会把ILRuntime扰昏(或者我需要一套BinaryFFormater的源码或是.dll放在热更新工程中,可能还需要必要的适配,如果把BinaryFormater保存系统放在热更新工程中的话)

// 可以试着把GameData和保存模块系统移至主工程,看能否完成解决所有的编译运行时问题出错等(因为先前的保存系统是用BinaryFormater来写的,如果能够连通,)就基本算是最简单的解决思路
    // 这个思路现在下午就已经是能够执行得通的,完全没有问题.局限是因为这套保存系统是定义在主工程中的,那么就是说这套保存系统被限制无法热更新这个模块这套系统
    // 目前不考虑热更新这个模块--保存加载系统
    // 想要具备热更新这个系统的话,可以在热更新工程中使用JsonUntility来序列化与反序列化保存加载系统.也就是说要把原主工程中的BinaryFormater定义保存加载系统改包换库,换为在热更新工程中使用JsonUtility.步及的重构工作量比较大,游戏真正完成之前不考虑热更新这个保存加载系统,就让它放在主工程中

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

        public int prevPreviewColor;
        public int prevPreviewColor2;
        public int previewTetrominoColor; 
        public int previewTetromino2Color;

// 游戏进度数据需要保存的其它条款    
        public SerializedTransform cameraData;  // 相机数据
        public TetrominoData nextTetrominoData; // 大方格中的当前方块砖
        public List<MinoData> grid;             // 大方格中的所有先前数据
        public List<TetrominoData> parentList;  // 如果有方块砖链表,那么链表中的方块砖有可能是残缺的(因为游戏过程中的消除行与列等)

        public bool saveForUndo; // 区分教育模式与经典模式 
        public bool isChallengeMode;

        private TetrominoData curParentData;
		// private List<bool> vis;

		public GameData(bool isChallengeMode, GameObject go, GameObject ghostTetromino, Transform tmpTransform,
                        int gameMode, int currentScore, int currentLevel, int numLinesCleared, int gridWidth,
                        string prevPreview, string prevPreview2,
                        string nextTetrominoType, string previewTetrominoType, string previewTetromino2Type,
                        bool saveForUndo, Transform[][][] gd, int[][][] gridClr,
                        int prevPreviewColor, int prevPreviewColor2, int previewTetrominoColor, int previewTetromino2Color
			) {
			this.gameMode = gameMode;
			this.isChallengeMode = isChallengeMode;
			this.score = currentScore;
			this.level = currentLevel;
			this.lines = numLinesCleared;

			this.saveForUndo = saveForUndo;

			this.nextTetrominoType = nextTetrominoType;
			this.previewTetrominoType = previewTetrominoType;
			this.previewTetromino2Type = previewTetromino2Type;

			if (isChallengeMode) {
				this.previewTetrominoColor = previewTetrominoColor;
				this.previewTetromino2Color = previewTetromino2Color;
			}
			Debug.Log(TAG + " (gameMode == 0): " + (gameMode == 0));
			if (gameMode == 0) {
				this.prevPreview = prevPreview;
				this.prevPreview2 = prevPreview2;
				if (isChallengeMode) {
					this.prevPreviewColor = prevPreviewColor;
					this.prevPreviewColor2 = prevPreviewColor2;
				}
			}

			grid = new List<MinoData>();
			parentList = new List<TetrominoData>();
			// int listSize = Model.gridHeight * Model.gridWidth * Model.gridWidth;
			int listSize = 12 * gridWidth * gridWidth;
            Debug.Log(TAG + " listSize: " + listSize);

			// vis = new bool [gridWidth][][];
			// for (int i = 0; i < gridWidth; i++) {
			//     vis[i] = new bool [12][];
			//     for (int j = 0; j < 12; j++)
			//         vis[i][j] = new bool[gridWidth];
			// }
			// vis = new List<bool> ();
			// for (int i = 0; i < listSize; i++) 
			//     vis.Add(false);

			bool isCurrentlyActiveTetromino = false;
			for (int i = 0; i < listSize; i++) {
				MinoData tmpMino = new MinoData(tmpTransform);
				tmpMino.reset();
				grid.Add(tmpMino);
			}

			int[] pos = new int[3];
			int x = 0, y = 0, z = 0, color = -1;

			//  ViewManager.nextTetromino: May have landed already, may have been destroyed right after undo clicked
			if (go != null) { // go: ViewManager.nextTetromino
				isCurrentlyActiveTetromino = go.CompareTag("currentActiveTetromino");
				if (go != null && isCurrentlyActiveTetromino) { // 没着陆
					foreach (Transform mino in go.transform) {
						if (mino.CompareTag("mino")) {
							x = (int)Mathf.Round(mino.position.x);
							y = (int)Mathf.Round(mino.position.y);
							z = (int)Mathf.Round(mino.position.z);
							color = gridClr[x][y][z];
							// vis[x][y][z] = true;
							// vis[MathUtil.getIndex(x, y, z)] = true;
							break;
						}
					}
					nextTetrominoData = new TetrominoData(go.transform, nextTetrominoType, go.gameObject.name, color);
				}
			}
			// dealing with Game Data: gird
			for (int i = 0; i < listSize; i++) {
				pos = MathUtil.getIndex(i);
				x = pos[0];
				y = pos[1];
				z = pos[2];
				if (gd[x][y][z] != null && gd[x][y][z].parent != null) {
					MathUtil.print(x, y, z);

					if ((ghostTetromino == null || (gd[x][y][z].parent != ghostTetromino.transform))
						&& (go == null || gd[x][y][z].parent != go.transform)) {

						Debug.Log(TAG + " (!myContains(gd[x][y][z].parent)): " + (!myContains(gd[x][y][z].parent)));
						if (!myContains(gd[x][y][z].parent)) {

							// todo: 晚点儿这里会需要更多的逻辑                    
							color = gridClr[x][y][z];
							// Debug.Log(TAG + " color: " + color); 
							TetrominoData tmp = new TetrominoData(gd[x][y][z].parent,
																  gd[x][y][z].parent.gameObject.name,
																  // new StringBuilder("Tetromino").Append(gd[x][y][z].parent.gameObject.name.Substring(9, 1)).ToString(),
																  gd[x][y][z].parent.gameObject.name, color);

							Debug.Log(TAG + " gd[x][y][z].parent.gameObject.name (in parentList): " + gd[x][y][z].parent.gameObject.name);
							Debug.Log(TAG + " gd[x][y][z].parent.childCount: " + gd[x][y][z].parent.childCount); 
							// Debug.Log(TAG + " tmp.children.Count (in saved parent TetrominoData): " + tmp.children.Count); 
							foreach (MinoData mino in tmp.children) {
							    MathUtil.print(MathUtil.getIndex(mino.idx));
							}
							// foreach (Transform mino in gd[x][y][z].parent.transform) {
							//     x = (int)Mathf.Round(mino.position.x);
							//     y = (int)Mathf.Round(mino.position.y);
							//     z = (int)Mathf.Round(mino.position.z);
							//     vis[MathUtil.getIndex(x, y, z)] = true;
							//     // vis[x][y][z] = true;
							// }
							parentList.Add(tmp);
						}
					}
				}
			}
            cameraData = new SerializedTransform(Camera.main.transform);
            
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