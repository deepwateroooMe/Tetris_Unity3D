using Framework.MVVM;
using HotFix.Control;
using UnityEngine;

namespace HotFix.UI {

    // 这里是把原混作一团的Game.cs中的应用控制逻辑折分到不同的分模块的视图模型中,尽可能地减少不必要的偶合
    public class GameViewModel : ViewModelBase {
        private const string TAG = "GameViewModel"; 

        public bool isPaused { set; get; }
        public bool saveForUndo { set; get; }
        public bool hasSavedGameAlready { set; get; }
        public bool gameStarted { set; get; }
        public float fallSpeed { set; get; }
        public int currentScore { set; get; }

        public int gridHeight = 12; 
        int gridWidth;

        public static Transform[,,] grid; //= new Transform[gridWidth, gridHeight, gridWidth];
        public static int [,,] gridOcc; //= new int[gridWidth, gridHeight, gridWidth];

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe(); //
        }

        void Initialization() {
            this.ParentViewModel = (MenuViewModel)ViewManager.MenuView.BindingContext; // 父视图模型: 菜单视图模型
            gridWidth = ((MenuViewModel)ParentViewModel).gridWidth;

            grid = new Transform[5, gridHeight, 5];
            gridOcc = new int[5, gridHeight, 5];

        }

        void DelegateSubscribe() { // 这里怎么写成是观察者模式呢?
        }

        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() { // 这是里是指初始化数据管理,而不是视图层面
        }

        public bool CheckIsInsideGrid(Vector3 pos) {
            return ((int)pos.x >= 0 && (int)pos.x < gridWidth &&
                    (int)pos.z >= 0 && (int)pos.z < gridWidth && 
                    (int)pos.y >= 0 && (int)pos.y < gridHeight); 
        }

        public void UpdateGrid(GameObject tetromino) { // update gridOcc at the same time
            Debug.Log(TAG + ": UpdateGrid()");
            for (int y = 0; y < gridHeight; y++) 
                for (int z = 0; z < gridWidth; z++) 
                    for (int x = 0; x < gridWidth; x++)
                        if (grid[x, y, z] != null && grid[x, y, z].parent == tetromino.transform) {
                            grid[x, y, z] = null; 
                            gridOcc[x, y, z]= 0; 
                        }
            foreach (Transform mino in tetromino.transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (pos.y >= 0 && pos.y < gridHeight && pos.x >= 0 && pos.x < gridWidth && pos.z >= 0 && pos.z < gridWidth) { 
                    grid[(int)pos.x, (int)pos.y, (int)pos.z] = mino;
                    gridOcc[(int)pos.x, (int)pos.y, (int)pos.z] = 1;
                }
            }
            Debug.Log(TAG + " tetromino.name: " + tetromino.name);
        }

    }
}
