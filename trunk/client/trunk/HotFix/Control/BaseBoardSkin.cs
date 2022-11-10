using deepwaterooo.tetris3d;
using Framework.MVVM;
using HotFix.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    // 挑战模式: 每一关的底板的皮肤是动态更换的
    // public class BaseBoardSkin : SingletonMono<BaseBoardSkin> {
    public class BaseBoardSkin : MonoBehaviour {
        private const string TAG = "BaseBoardSkin";

        public GameObject [] cubes;
        public static bool isSkinChanged = false;
        public static int [] color;
        
        private int idx = 0;

        public void Awake() {
            Debug.Log(TAG + " Awake()");
            Start();
        }
        public void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            Start();
        }

        public void OnDisable() {
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            EventManager.Instance.UnregisterListener<TetrominoChallLandInfo>(onActiveTetrominoLand); 
            EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
            EventManager.Instance.UnregisterListener<CubesMaterialEventInfo>(onCubesMaterialsChanged);
        }

        public void Start() {
            Debug.Log(TAG + " Start()");
            EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            EventManager.Instance.RegisterListener<TetrominoChallLandInfo>(onActiveTetrominoLand);
            EventManager.Instance.RegisterListener<CubesMaterialEventInfo>(onCubesMaterialsChanged);

// 初始化: 我记得当有空白的地方是不允许移动的,要把那个地方的逻辑给找出来            
            Model.baseCubes = new int [Model.gridXWidth * Model.gridZWidth]; // <<<<<<<<<<<<<<<<<<<< Model.baseCubes: int [], 地板砖的着色
            int n = Model.gridXWidth * Model.gridZWidth;
            cubes = new GameObject[n]; // 地板砖的 gameObject s

// TODO: BUG FOR deffient levels, because I did named them differenly last night                    
            StringBuilder name = new StringBuilder("");
            for (int j = 0; j < Model.gridZWidth; j++) 
                for (int a = 0; a < Model.gridXWidth; a++) {// 这里字符串的使用太恐怖了
                    name.Length = 0;
                    if (GloData.Instance.gameLevel < 3)
                        name.Append("Cube" + a + j);
                    else name.Append("Cube" + a + "0 (" + j + ")"); 
                    cubes[j * Model.gridXWidth + a] = gameObject.FindChildByName(name.ToString());
                }
            
            int x = 0, z = 0;
            for (int i = 0; i < n; i++) {
                Model.baseCubes[i] = cubes[i].GetComponent<MinoType>().color;
                x = i % Model.gridXWidth;
                z = i / Model.gridXWidth;
                if (!cubes[i].activeSelf) { // 如果某一个方格失活,那么整个竖列都是不能穿过的
                    for (int y = 0; y < Model.gridHeight; y++) {
                        Model.grid[x][y][z] = null;
                        Model.gridOcc[x][y][z] = 9; // magic number, 9 to substitute -1
                    }
                }
            }
        }

        public void onCubesMaterialsChanged(CubesMaterialEventInfo info) {
        // private void updateSkin() {
            Debug.Log(TAG + ": updateSkin()");
            int n = Model.gridXWidth * Model.gridZWidth;
            for (int i = 0; i < n; i++) {
                if (cubes[i].activeSelf) {
                    if (cubes[i].GetComponent<MinoType>().color != Model.baseCubes[i]) {
                        cubes[i].GetComponent<MinoType>().color = Model.baseCubes[i];
                        cubes[i].gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[Model.baseCubes[i]];
                    }
                }
            }
            Debug.Log(TAG + ": baseCubes colors after updateSkin"); 
            MathUtilP.printBoard(Model.baseCubes);
        }

        void onActiveTetrominoLand(TetrominoChallLandInfo info) { 
            Debug.Log(TAG + " onActiveTetrominoCHALLENGINGLand() : baseCubes colors after nextTetromino landed BEF update:"); 
            MathUtilP.printBoard(Model.baseCubes);
            
            int i = 0;
            resetPrevSkin();
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                    if (pos.y == 0) {
                        idx = getMinoPosCubeArrIndex(pos.x, pos.z);
                        
                        Model.prevIdx[i] = idx;
                        Model.prevSkin[i] = getChallengedMaterialIdx(cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial);
// 将地板板砖的材质更换为当前所接触立方体的材质 
                        cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial = mino.gameObject.GetComponent<Renderer>().sharedMaterial;
                        Model.baseCubes[idx] = mino.gameObject.GetComponent<MinoType>().color;
                        // Debug.Log(TAG + " idx: " + idx);
                        // Debug.Log(TAG + " Model.prevIdx[i]: " + Model.prevIdx[i]);
                        // Debug.Log(TAG + " Model.prevSkin[i]: " + Model.prevSkin[i]);
                        // Debug.Log(TAG + " ViewManager.materials[Model.prevSkin[i]].ToString(): " + ViewManager.materials[Model.prevSkin[i]].ToString());
                        i++;
                        Debug.Log(TAG + " cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial.ToString(): " + cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                    }
                }
            }
            Debug.Log(TAG + ": baseCubes colors after nextTetromino landed & updated"); 
            MathUtilP.printBoard(Model.baseCubes);
        }
        
        int getChallengedMaterialIdx(Material material) {
            int n = ViewManager.materials.Count;
            for (int i = 0; i < n; i++) {
                if (ViewManager.materials[i] == material)
                    return i;
                else continue;
            }
            return -1;
        }
        
        void onUndoGame(UndoGameEventInfo undoInfo) {
            Debug.Log(TAG + ": onUndoGame()"); 
            for (int i = 0; i < 4; i++) {
                if (Model.prevIdx[i] == -1) return;
                Debug.Log(TAG + " Model.prevIdx[i]: " + Model.prevIdx[i]);
                cubes[Model.prevIdx[i]].gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[Model.prevSkin[i]];

                Debug.Log(TAG + " Model.prevIdx[i]: " + Model.prevIdx[i]);
                Debug.Log(TAG + " Model.prevSkin[i]: " + Model.prevSkin[i]); 
                Debug.Log(TAG + " cubes[Model.prevIdx[i]].gameObject.GetComponent<Renderer>().sharedMaterial.ToString(): " + cubes[Model.prevIdx[i]].gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
            }
        }
        
        void resetPrevSkin() {
            for (int i = 0; i < 4; i++) {
                Model.prevIdx[i] = -1;
                Model.prevSkin[i] = -1;
            }
        }
        
        int getMinoPosCubeArrIndex(float x, float z) {
            return (int)(Mathf.Round(x) + GloData.Instance.gridXSize * Mathf.Round(z)); // y = 0
        }
    }
}
