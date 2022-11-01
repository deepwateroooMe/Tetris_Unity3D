using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d;
using Framework.Util;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

// 因为热更新程序域里静态调用的特殊性,我只能把它放在ViewManager里,或是放在帮助类的静态成员静态方法里,否则无法正常运行
// 这样也基本算是相关功能模块化了.对象池的管理就做到这里算基本结束.必要时会补充方法    
    public class PoolHelper {
        private const string TAG = "PoolHelper";

        public static Dictionary<string, GameObject> minosDic = null; 
        public static Dictionary<string, Stack<GameObject>> pool = null;

        private static GameObject tetrosPool = null;
        private static GameObject tetroParent = null;

        private static Vector3 defaultPos = new Vector3(-100, -100, -100); // 不同类型的起始位置不一样(可否设置在预设里呢>??)
        private static Vector3 previewTetrominoScale = new Vector3(6f, 6f, 6f); // previewTetromino Scale (7,7,7)

        // public Material [] materials; // [red, green, blue, yellow]
        // public Material [] colors;

        public static void Initialize() {
            minosDic = new Dictionary<string, GameObject>();
            pool = new Dictionary<string, Stack<GameObject>>();
        }

        public static void fillPool(Transform prefab) {
            string type, name = prefab.gameObject.name;
            if (name.StartsWith("mino"))
                type = prefab.GetComponent<deepwaterooo.tetris3d.MinoType>().type;
            else type = prefab.GetComponent<TetrominoType>().type;
            minosDic.Add(type, prefab.gameObject);
            Stack<GameObject> stack = new Stack<GameObject>();
            bool isTetro = name.StartsWith("Tetromino");
            bool isGhost = name.StartsWith("shadow");
            for (int i = 0; i < 10; i++) {
				GameObject oneInstance = GameObject.Instantiate(prefab.gameObject).gameObject;
                oneInstance.name = name;
                if (isTetro) {
                    ComponentHelper.AddTetroComponent(oneInstance);
                    Tetromino tetromino = ComponentHelper.GetTetroComponent(oneInstance);
                    ComponentHelper.GetTetroComponent(oneInstance).enabled = false;
                    foreach (Transform oneMino in prefab) {
                        oneMino.gameObject.tag = "mino";
                    }
                } else if (isGhost) {
                    ComponentHelper.AddGhostComponent(oneInstance); 
                }
                oneInstance.transform.SetParent(ViewManager.tetrosPool.transform, true); 
                oneInstance.SetActive(false);
                stack.Push(oneInstance);
            }
            pool.Add(type, stack);
        }

        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale) {
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) 
                objInstance = st.Pop();
            else 
                objInstance = GameObject.Instantiate(minosDic[type]); 
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            if (localScale == null)
                objInstance.transform.localScale = Vector3.one;
            else
                objInstance.transform.localScale = (Vector3)localScale;
            objInstance.SetActive(true);
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
            return objInstance;
        }
    
        public static void ReturnToPool(GameObject gameObject, string type) {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
                if (pool[type].Count < 10) {
                    gameObject.transform.position = defaultPos;
                    pool[type].Push(gameObject);
                } else GameObject.DestroyImmediate(gameObject);
            } 
        }

        public static GameObject GetFromPool(string type) {
            GameObject objInstance = null;
            if (pool.ContainsKey(type) && pool[type].Count > 0) {
                objInstance = pool[type].Pop();
                objInstance.SetActive(true);
            } else  
                objInstance = GameObject.Instantiate(minosDic[type]);
            return objInstance;
        }

        public static void ReturnToPool(GameObject gameObject, string type, float delay) {
// CoroutineHelper这个帮助类对协程的适配做得不到位,这个方法现在还不能用            
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

        public static void preparePreviewTetrominoRecycle(GameObject go) { 
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
            if (ViewManager.ghostTetromino != null) {
                ViewManager.ghostTetromino.tag = "Untagged";
                ReturnToPool(ViewManager.ghostTetromino, ViewManager.ghostTetromino.GetComponent<TetrominoType>().type);
            }
        }
    }
}
