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
    public class BaseBoardSkin : SingletonMono<BaseBoardSkin> {
    // public class BaseBoardSkin : MonoBehaviour {
        private const string TAG = "BaseBoardSkin";

        public GameObject [] cubes;
        public static bool isSkinChanged = false;
        public static int [] color;

        private int idx = 0;
        
        public void onCubesMaterialsChanged(CubesMaterialEventInfo info) {
            int n = Model.gridXWidth * Model.gridZWidth;
            for (int i = 0; i < n; i++) {
                if (Model.baseCubes[i] < 0 || Model.baseCubes[i] > 11)
                    Debug.Log(TAG + " Model.baseCubes[i]: " + Model.baseCubes[i]);
                if (cubes[i].activeSelf) {
                    if (cubes[i].GetComponent<MinoType>().color != Model.baseCubes[i]) {
                        cubes[i].GetComponent<MinoType>().color = Model.baseCubes[i];
                        cubes[i].gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.colors[Model.baseCubes[i]];
                    }
                } 
            }
            Debug.Log(TAG + " onCubesMaterialsChanged(): baseCubes colors AFTER updateSkin"); 
            MathUtilP.printBoard(Model.baseCubes);
        }

        void onActiveTetrominoLand(TetrominoChallLandInfo info) { 
            // Debug.Log(TAG + " onActiveTetrominoLand() : baseCubes colors after nextTetromino landed BEF update:"); 
            // MathUtilP.printBoard(Model.baseCubes);
            int i = 0;
            resetPrevSkin();
            foreach (Transform mino in ViewManager.nextTetromino.transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtilP.Round(mino.position);
                    if (pos.y == 0) {
                        idx = getMinoPosCubeArrIndex(pos.x, pos.z);
                        Debug.Log(TAG + " onActiveTetrominoLand() idx: " + idx);
// 将当前方块砖落地前的相应位置的 坐标 和 着色 存起来,备用
                        Model.prevIdx[i] = idx;
                        Model.prevSkin[i] = getChallengedMaterialIdx(cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial);
// 将地板板砖的材质更换为当前所接触立方体的材质 
                        cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial = mino.gameObject.GetComponent<Renderer>().sharedMaterial;
                        Model.baseCubes[idx] = mino.gameObject.GetComponent<MinoType>().color;
                        i++;
                    }
                }
            }
            MathUtilP.printSkinArray(Model.prevIdx);
            MathUtilP.printSkinArray(Model.prevSkin);

            Debug.Log(TAG + ": baseCubes colors after nextTetromino landed & UPDATED"); 
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
                cubes[Model.prevIdx[i]].gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[Model.prevSkin[i]];
                Model.baseCubes[Model.prevIdx[i]] = Model.prevSkin[i]; // 这个数据也要回复回来 ?
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

        public void initateBaseCubesColors() { // 这里初始化得狠早,可能早于Model初始化其相关数组,所以这里用到的要自己初始化一下
            Debug.Log(TAG + " initateBaseCubesColors()");
            int n = GloData.Instance.gridXSize * GloData.Instance.gridZSize, idx = 0;
            Debug.Log(TAG + " initateBaseCubesColors() n: " + n);
            cubes = new GameObject[n]; // 地板砖的 gameObject s
            StringBuilder name = new StringBuilder("");
            for (int z = 0; z < GloData.Instance.gridZSize; z++) 
                for (int x = 0; x < GloData.Instance.gridXSize; x++) {
                    name.Length = 0;
                    if (GloData.Instance.gameLevel < 3 || GloData.Instance.gameLevel == 11) // so far 1, 2, 11 three levels named this way
                        name.Append("Cube" + x + z);
                    else name.Append("Cube" + x + "0 (" + z + ")");
                    idx = z * GloData.Instance.gridXSize + x;
                    cubes[idx] = gameObject.FindChildByName(name.ToString());
                    cubes[idx].gameObject.transform.rotation = Quaternion.identity;
                }
            EventManager.Instance.FireEvent(cubes);
        }

        public void OnEnable() {
            Start();
        }
        public void Start() { // 因为它只调用一次,所以帮它确保多调用几次
            initateBaseCubesColors();
            EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
            EventManager.Instance.RegisterListener<TetrominoChallLandInfo>(onActiveTetrominoLand);
            EventManager.Instance.RegisterListener<CubesMaterialEventInfo>(onCubesMaterialsChanged);
        }
        public void OnDisable() {
            EventManager.Instance.UnregisterListener<TetrominoChallLandInfo>(onActiveTetrominoLand); 
            EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
            EventManager.Instance.UnregisterListener<CubesMaterialEventInfo>(onCubesMaterialsChanged);
        }
    }
}
