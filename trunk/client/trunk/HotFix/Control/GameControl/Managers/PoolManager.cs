﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

// 暂时就让它与BtnsCanvasView同一个资源包一起读,暂时仍然让ViewManager 来处理相关逻辑
// 晚点儿项目快结束时,再试着将这一部分的逻辑分出来,统一管理    

// 还有一点儿小问题,接下来三天放假不更新,周六会接着更新    
    public class PoolManager : SingletonMono<PoolManager> {
        private const string TAG = "PoolManager";

        public static Dictionary<string, GameObject> minosDic = null; // [type, prefab gameObject]
        public static Dictionary<string, Stack<GameObject>> pool = null;

        public static GameObject tetrosPool = null;
        public static GameObject tetroParent = null;

        private static Vector3 defaultPos = new Vector3(-100, -100, -100); // 不同类型的起始位置不一样(可否设置在预设里呢>??)

// 这几个换材质的,可以晚点儿再写
        // public Material [] materials; // [red, green, blue, yellow]
        // public Material [] colors;

        public void Awake() {
            Debug.Log(TAG + " Awake()");

            minosDic = new Dictionary<string, GameObject>();
            pool = new Dictionary<string, Stack<GameObject>>();

// 从必要的资源包加载预设资源:既然把这个模块独立管理,那么最好就把预设单独打一个资源包ui/view/gameview/prefabs
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/view/gameview/prefabs",
                    "Prefabs", // 这里是有预设的包，读出资源就可以加载
                    (go) => {
                        go.name = "Prefabs";
                        GameObject.DontDestroyOnLoad(go); // 以此为父节点的所有子节点都不会被销毁,包括各种管理类

                        foreach (Transform child in go.transform) { // go ==> parent 这个破BUG让我找了好久.....只仅仅是实现的时候手误.....
// 这里顺承从前的用名字代替了类型,但有时候可能会出错了,要再改一下
                            string name = child.gameObject.name;
                            minosDic.Add(name, child.gameObject);

                            Stack<GameObject> stack = new Stack<GameObject>();
                            bool isTetro = name.StartsWith("Tetromino");
                            bool isGhost = name.StartsWith("shadow");
                            for (int i = 0; i < 10; i++) {
                                GameObject tmp = GameObject.Instantiate(child.gameObject);
                                tmp.name = name;
                                if (isTetro) {
                                    ComponentHelper.AddTetroComponent(tmp);
                                    Tetromino tetromino = ComponentHelper.GetTetroComponent(tmp);
                                    ComponentHelper.GetTetroComponent(tmp).enabled = false;
                                } else if (isGhost) {
                                    ComponentHelper.AddGhostComponent(tmp);
                                    ComponentHelper.GetGhostComponent(tmp).enabled = false;
                                }
                                tmp.transform.SetParent(tetrosPool.transform, true); // 把它们放在一个容器下面,免得弄得游戏界面乱七八糟的
                                tmp.SetActive(false);
                                stack.Push(tmp);
                            }
                            pool.Add(name, stack);
                        }
                        go.SetActive(false); // 或是因为前面的小错误,这里还没有设置成功
                    }, EAssetBundleUnloadLevel.Never);
        }

        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale) {
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) 
                objInstance = st.Pop();
            else 
                objInstance = GameObject.Instantiate(ViewManager.minosDic[type]); 
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            if (localScale == null)
                objInstance.transform.localScale = Vector3.one;
            else
                objInstance.transform.localScale = (Vector3)localScale;
            objInstance.SetActive(true);
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
// 这里的逻辑不对,只在非启蒙模式下才立即激活,逻辑还是放游戏视图里去处理            
            // if (type.StartsWith("Tetromino"))
            //     ComponentHelper.GetTetroComponent(objInstance).enabled = true;
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
            } else  // tmp commented out
                objInstance = GameObject.Instantiate(minosDic[type]);
            return objInstance;
        }

//         public static void ReturnToPool(GameObject gameObject, string type, float delay) {
// // CoroutineHelper这个帮助类对协程的适配做得不到位,这个方法现在还不能用            
//            CoroutineHelper.StartCoroutine(DelayedReturnToPool(gameObject, type, delay));
//         }

        IEnumerator DelayedReturnToPool(GameObject gameObject, string type, float delayTime) {
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
