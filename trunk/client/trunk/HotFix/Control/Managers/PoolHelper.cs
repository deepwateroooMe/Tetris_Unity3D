﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.Util;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

    public class PoolHelper {
        private const string TAG = "PoolHelper";

        public static Dictionary<string, GameObject> minosDic = null; 
        public static Dictionary<string, Stack<GameObject>> pool = null;
        // public static Dictionary<string, int> scoreDic = null;

        private static GameObject tetrosPool = null;
        private static GameObject tetroParent = null;

        private static Vector3 defaultPos = new Vector3(-100, -100, -100); 
        private static Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f); 

// // TODO: 这些材质的打包与填充初始化        
//         public static Material [] materials; // [red, green, blue, yellow]
//         public static Material [] colors;    // [blue, colorB, oliveC, purple,  brown, white, red, black,  green, pinkY, Yello]
        //   0      1                3               5           8       9             11
        public static void Initialize() {
            minosDic = new Dictionary<string, GameObject>();
            pool = new Dictionary<string, Stack<GameObject>>();
            // scoreDic = new Dictionary<string, int>();
// 初始化挑战模式下的颜色材质等相关资源
            ViewManager.materials = new Dictionary<int, Material>();
            ViewManager.colors = new Dictionary<int, Material>();
        }

        public static void fillPool(Transform prefab) {
            string type, name = prefab.gameObject.name;
            if (name.StartsWith("mino"))
                type = prefab.GetComponent<deepwaterooo.tetris3d.MinoType>().type;
            else type = prefab.GetComponent<TetrominoType>().type;
            minosDic.Add(type, prefab.gameObject);
            Stack<GameObject> stack = new Stack<GameObject>();
            for (int i = 0; i < 10; i++) {
				GameObject oneInstance = GameObject.Instantiate(prefab.gameObject).gameObject;
                oneInstance.name = name;
                InstantiateNewTetrominoPrepare(oneInstance);
                oneInstance.transform.SetParent(ViewManager.tetrosPool.transform, true); 
                oneInstance.gameObject.SetActive(false);
                stack.Push(oneInstance);
            }
            pool.Add(type, stack);
        }
        
        private static void InstantiateNewTetrominoPrepare(GameObject go) {
            string name = go.gameObject.name;
            bool isTetro = name.StartsWith("Tetromino");
            bool isGhost = name.StartsWith("shadow");
            bool isMino = name.StartsWith("mino") && !name.StartsWith("minoPS");
            if (isTetro) {
                ComponentHelper.AddTetroComponent(go);
                // Tetromino tetromino = ComponentHelper.GetTetroComponent(go);
                ComponentHelper.GetTetroComponent(go).enabled = false;
                foreach (Transform oneMino in go.transform) {
                    oneMino.gameObject.tag = "mino";
                }
// 这里的意思是说,我的预设在序列化与反序列化的过程中,某些数据是没有保存的.
// 可以使用JsonUtility来序列化与反序列化预设中的相关数据,使之不至于丢失?
// 因现在我的预设简单,数据量少,一个标签,一个得分,可以简单地在热更新工程中设定,暂且不补这块儿                    
                TetrominoType itype = go.GetComponent<TetrominoType>();
                itype.score = 0;
                switch (name) {
                case "TetrominoI":
                    itype.score = 300;
                    // scoreDic.Add(name, 300);
                    break;
                case "TetrominoJ":
                    itype.score = 350;
                    // scoreDic.Add(name, 350);
                    break;
                case "TetrominoL":
                    itype.score = 350;
                    // scoreDic.Add(name, 350);
                    break;
                case "TetrominoO":
                    itype.score = 300;
                    // scoreDic.Add(name, 300);
                    break;
                case "TetrominoS":
                    itype.score = 500;
                    // scoreDic.Add(name, 500);
                    break;
                case "TetrominoT":
                    itype.score = 400;
                    // scoreDic.Add(name, 400);
                    break;
                case "TetrominoZ":
                    itype.score = 500;
                    // scoreDic.Add(name, 500);
                    break;
                }
                // scoreDic.Add(name, itype.score);
            } else if (isGhost) {
// 因为BtnsCanvasView最开始读的时候,很多时候容易抛脚本onEnable()生命周期函数回调的空异常,这里仍先把它失活,原本原则上讲应该是可以不必要的
                ComponentHelper.AddGhostComponent(go);
                ComponentHelper.GetGhostComponent(go).enabled = false;
            } else if (isMino) {
                if (go.GetComponent<MinoType>() == null) {
                    go.AddComponent<MinoType>();
                    go.GetComponent<MinoType>().type = name;
                }
            }
        }

        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, int color = 0) {
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) {
                objInstance = st.Pop();
                objInstance.SetActive(true);
            } else {
                objInstance = GameObject.Instantiate(minosDic[type]); 
// 对 tetromino 和 ghostTetromino 进行脚本的加载等相关处理
                InstantiateNewTetrominoPrepare(objInstance);
            }
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;

// 必要的情况下给小立方体更换皮肤材质
            if (GloData.Instance.isChallengeMode
                && type.Substring(0, 4).Equals("mino")
                && !type.Substring(0, 5).Equals("minoP")) { // isChallengeMode = true, gameMode = 0
                objInstance.GetComponent<MinoType>().color = color;
                objInstance.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[color];
            }
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
            return objInstance;
        }

        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale) {
            Debug.Log(TAG + " GetFromPool() type: " + type);
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) {
                objInstance = st.Pop();
                while (objInstance == null && st.Count > 0) {
                    objInstance = st.Pop();
                }
            }
            objInstance.SetActive(true);
            if (objInstance == null) {                
                objInstance = GameObject.Instantiate(minosDic[type]);
                InstantiateNewTetrominoPrepare(objInstance);
            }
// TODO: sometimes, there is a bug here saying it's destroyed exception            
            if (objInstance == null) {
                Debug.Log(TAG + " (objInstance == null): " + (objInstance == null));
                MathUtilP.print(pos);
                MathUtilP.print(rotation);
                MathUtilP.print(localScale);
            }
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;

            if (GloData.Instance.isChallengeMode && type.Substring(0, 9).Equals("Tetromino")) { // isChallengeMode = true, gameMode = 0
                int rand = 0, randomColor1 = 0, randomColor2 = 0, cnt = 0;

                Debug.Log(TAG + " GloData.Instance.challengeLevel: " + GloData.Instance.challengeLevel);
				if (GloData.Instance.isChallengeMode && GloData.Instance.challengeLevel > 10 && GloData.Instance.challengeLevel < 16) { // match 2
                    randomColor1 = UnityEngine.Random.Range(0, 4);
                    Debug.Log(TAG + " randomColor1: " + randomColor1); 
                    randomColor2 = UnityEngine.Random.Range(0, 4);
                    while (randomColor2 == randomColor1)
                        randomColor2 = UnityEngine.Random.Range(0, 4);
                    Debug.Log(TAG + " randomColor2: " + randomColor2); 
                    foreach (Transform child in objInstance.transform) {
                        rand = UnityEngine.Random.Range(0, 2);
                        Debug.Log(TAG + " cnt: " + cnt);
                        Debug.Log(TAG + " (objInstance.GetComponent<TetrominoType>().childCnt - 1): " + (objInstance.GetComponent<TetrominoType>().childCnt - 1)); 
                        if ((rand == 0 && cnt != objInstance.GetComponent<TetrominoType>().childCnt - 1)
                            // || (objInstance.transform.childCount == objInstance.GetComponent<TetrominoType>().childCnt - 1 && cnt == 0))) {
                            || cnt == 0) {
                            ++cnt;
                            child.gameObject.GetComponent<MinoType>().color = randomColor1;
                            child.gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[randomColor1];
                            Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                        } else {
                            child.gameObject.GetComponent<MinoType>().color = randomColor2;
                            child.gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[randomColor2];
                            Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                        }
                    }
                } else { // 1 color per tetromino
                    int randomColor = UnityEngine.Random.Range(0, 4);
                    objInstance.GetComponent<TetrominoType>().color = randomColor;
                    Debug.Log(TAG + " randomColor: " + randomColor); 

                    foreach (Transform child in objInstance.transform) {
                        child.gameObject.GetComponent<MinoType>().color = randomColor;
                        child.gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[randomColor];
                    }
                }
            }
            // if (type == "shapeJ") { // J ==> green
            //     foreach (Transform child in objInstance.transform) {
            //         child.gameObject.GetComponent<Renderer>().material.mainTexture = greenTexture;
            //         // Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
            //     }
            // } // OK
            if (localScale == null)
                objInstance.transform.localScale = Vector3.one;
            else
                objInstance.transform.localScale = (Vector3)localScale;
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
            return objInstance;
        }

        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale, int color) {
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) {
                objInstance = st.Pop();
                objInstance.SetActive(true);
            } else {
                objInstance = GameObject.Instantiate(minosDic[type]); 
                InstantiateNewTetrominoPrepare(objInstance);
            }
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            objInstance.transform.localScale = localScale;

            objInstance.GetComponent<TetrominoType>().color = color;
            foreach (Transform child in objInstance.transform) {
                child.gameObject.GetComponent<MinoType>().color = color;
                child.gameObject.GetComponent<Renderer>().sharedMaterial = ViewManager.materials[color];
            }
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
            return objInstance;
        }

        public static void ReturnToPool(GameObject gameObject, string type) {
            if (gameObject.activeSelf) 
                gameObject.SetActive(false);
            if (pool[type].Count < 10) {
                gameObject.transform.position = defaultPos;
                pool[type].Push(gameObject);
            } else GameObject.DestroyImmediate(gameObject);
        }

        public static void recyclePreviewTetrominos(GameObject go) {
            preparePreviewTetrominoRecycle(go);
            ReturnToPool(go, go.GetComponent<TetrominoType>().type);
        }

        private static void preparePreviewTetrominoRecycle(GameObject go) { 
            go.transform.localScale -= previewTetrominoScale;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.SetActive(false);
        }

        public static void recycleNextTetromino() { // 里面的参数可以去掉
            if (ViewManager.nextTetromino != null) {
                ViewManager.nextTetromino.tag = "Untagged";
                bool isEnabled = ComponentHelper.GetTetroComponent(ViewManager.nextTetromino); 
                ComponentHelper.GetTetroComponent(ViewManager.nextTetromino).enabled = false;
                // resetGridAfterDisappearingNextTetromino(ViewManager.nextTetromino);  // ViewModel.放那里面去
                if (ViewManager.nextTetromino.transform.childCount == 4) {
                    ReturnToPool(ViewManager.nextTetromino, ViewManager.nextTetromino.GetComponent<TetrominoType>().type);
                } else
                    GameObject.Destroy(ViewManager.nextTetromino.gameObject);
            }
        }

        public static void recycleGhostTetromino() {
            Debug.Log(TAG + " recycleGhostTetromino");
            if (ViewManager.ghostTetromino != null) {
                Debug.Log(TAG + " ViewManager.ghostTetromino.name: " + ViewManager.ghostTetromino.name);
                ViewManager.ghostTetromino.tag = "Untagged";
                ComponentHelper.GetGhostComponent(ViewManager.ghostTetromino).enabled = false;
                ReturnToPool(ViewManager.ghostTetromino, ViewManager.ghostTetromino.GetComponent<TetrominoType>().type);
            }
        }
        
// CoroutineHelper这个帮助类对协程的适配做得不到位,这个方法现在还不能用            
        public static void ReturnToPool(GameObject gameObject, string type, float delay) {
            CoroutineHelper.StartCoroutine(DelayedReturnToPool(gameObject, type, delay));
        }
        static IEnumerator DelayedReturnToPool(GameObject gameObject, string type, float delayTime) {
            while (delayTime > 0f) {
                yield return null;
                // If the instance was deactivated while waiting here, just quit
                if (!gameObject.activeInHierarchy) {
                    yield break;
                }
                delayTime -= Time.deltaTime;
            }
            ReturnToPool(gameObject, type);
        }
    }
}
