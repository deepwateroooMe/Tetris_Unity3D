﻿using deepwaterooo.tetris3d;
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
        private const string TAG = "BaseBoardSkin";

        public GameObject [] cubes;
        public static bool isSkinChanged = false;
        public static int [] color;
        
        private int idx = 0;
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            GameView.changeBaseCubesSkin += onChallengeTetrominoLand;
            ModelMono.updateBaseCubesSkin += updateSkin;
            EventManager.Instance.RegisterListener<UndoGameEventInfo>(onUndoGame); 
        }

        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            GameView.changeBaseCubesSkin -= onChallengeTetrominoLand;
            ModelMono.updateBaseCubesSkin -= updateSkin;
            EventManager.Instance.UnregisterListener<UndoGameEventInfo>(onUndoGame); 
        }

        void Start() {
            Model.baseCubes = new int [Model.gridXWidth * Model.gridZWidth];

            int n = Model.gridXWidth * Model.gridZWidth;
            int x = 0, z = 0;
            for (int i = 0; i < n; i++) {
                Model.baseCubes[i] = cubes[i].GetComponent<MinoType>().color;
                x = i % Model.gridXWidth;
                z = i / Model.gridXWidth;
                if (!cubes[i].activeSelf) {
                    // Debug.Log(TAG + " (Model.grid == null): " + (Model.grid == null)); 
                    for (int y = 0; y < Model.gridHeight; y++) {
                        Model.grid[x][y][z] = null;
                        Model.gridOcc[x][y][z] = 9; // magic number, 9 to substitute -1
                    }
                }
            }
        }

        // private int n = Model.gridXWidth * Model.gridZWidth;
        // int x = 0, z = 0, i = 0;
        public void updateSkin() {
            Debug.Log(TAG + ": updateSkin()");
            int n = Model.gridXWidth * Model.gridZWidth;
            for (int i = 0; i < n; i++) {
                if (cubes[i].activeSelf) {
                    if (cubes[i].GetComponent<MinoType>().color != Model.baseCubes[i]) {
                        cubes[i].GetComponent<MinoType>().color = Model.baseCubes[i];
                        cubes[i].gameObject.GetComponent<Renderer>().sharedMaterial = PoolHelper.materials[Model.baseCubes[i]];
                    }
                }
            }
            Debug.Log(TAG + ": baseCubes colors after updateSkin"); 
            MathUtilP.printBoard(Model.baseCubes);
        }

        void onChallengeTetrominoLand() {
            Debug.Log(TAG + ": onChallengeTetrominoLand()");
            Debug.Log(TAG + ": baseCubes colors after nextTetromino landed BEF update:"); 
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

                        cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial = mino.gameObject.GetComponent<Renderer>().sharedMaterial;
                        Model.baseCubes[idx] = mino.gameObject.GetComponent<MinoType>().color;

                        Debug.Log(TAG + " idx: " + idx);
                        Debug.Log(TAG + " Model.prevIdx[i]: " + Model.prevIdx[i]);
                        Debug.Log(TAG + " Model.prevSkin[i]: " + Model.prevSkin[i]);
                        Debug.Log(TAG + " PoolHelper.materials[Model.prevSkin[i]].ToString(): " + PoolHelper.materials[Model.prevSkin[i]].ToString());

                        i++;
                        
                        Debug.Log(TAG + " cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial.ToString(): " + cubes[idx].gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                    }
                }
            }
            Debug.Log(TAG + ": baseCubes colors after nextTetromino landed & updated"); 
            MathUtilP.printBoard(Model.baseCubes);
        }
        
        int getChallengedMaterialIdx(Material material) {
            int n = PoolHelper.materials.Length;
            for (int i = 0; i < n; i++) {
                if (PoolHelper.materials[i] == material)
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
                cubes[Model.prevIdx[i]].gameObject.GetComponent<Renderer>().sharedMaterial = PoolHelper.materials[Model.prevSkin[i]];

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