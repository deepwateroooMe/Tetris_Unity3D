using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

// 暂时就让它与BtnsCanvasView同一个资源包一起读,暂时仍然让ViewManager 来处理相关逻辑
// TODO: 这个管理器还不能独立于ViewManager之外,需要更多的工作    
    public class PoolManager : Singleton<PoolManager> {
        private const string TAG = "PoolManager";

        public Dictionary<string, GameObject> minosDic = null; // [type, prefab gameObject]
        public Dictionary<string, Stack<GameObject>> pool = null;

        private GameObject tetrosPool = null;
        private GameObject tetroParent = null;

        private Vector3 defaultPos = new Vector3(-100, -100, -100); // 不同类型的起始位置不一样(可否设置在预设里呢>??)

// 这几个换材质的,可以晚点儿再写
        // public Material [] materials; // [red, green, blue, yellow]
        // public Material [] colors;

        // public void Awake() {
        //     Debug.Log(TAG + " Awake()");
        public void Initialize() {
            minosDic = new Dictionary<string, GameObject>();
            pool = new Dictionary<string, Stack<GameObject>>();
        }

        public void fillPool(Transform prefab) {
            string name = prefab.gameObject.name;
            // Debug.Log(TAG + " name: " + name);
            // if (child.gameObject.name.StartsWith("mino"))
            //     type = child.GetComponent<MinoType>();
            // else type = child.GetComponent<TetrominoType>();
            minosDic.Add(name, prefab.gameObject);
            Stack<GameObject> stack = new Stack<GameObject>();
// 这里我写的是手动生成对象池里的缓存对象:并在这里根据不同的类型添加相应的脚本
            bool isTetro = name.StartsWith("Tetromino");
            bool isGhost = name.StartsWith("shadow");
            // Debug.Log(TAG + " isTetro: " + isTetro);
            for (int i = 0; i < 10; i++) {
				GameObject tmp = GameObject.Instantiate(prefab.gameObject);
                tmp.name = name;
// 被劫持了的新的GetComponent()方法: 可能并不支持其.enabled 的属性修改?
                if (isTetro) {
                    ComponentHelper.AddTetroComponent(tmp);
                    Tetromino tetromino = ComponentHelper.GetTetroComponent(tmp);
                    ComponentHelper.GetTetroComponent(tmp).enabled = false;
                } else if (isGhost) {
                    ComponentHelper.AddGhostComponent(tmp);
                    ComponentHelper.GetGhostComponent(tmp).enabled = false;
                }
                tmp.transform.SetParent(tetrosPool.transform, true); // <<<<<<<<<<<<<<<<<<<<  BUG TO BE FIXED
                tmp.SetActive(false);
                stack.Push(tmp);
            }
            pool.Add(name, stack);
        }
// // 从必要的资源包加载预设资源:既然把这个模块独立管理,那么最好就把预设单独打一个资源包ui/view/gameview/prefabs
//             ResourceHelper
//                 .LoadCloneAsyn(
//                     "ui/view/btnscanvasview/prefabs",
//                     "Prefabs", // 这里是有预设的包，读出资源就可以加载
//                     (go) => {
//                         go.name = "Prefabs";
//                         GameObject.DontDestroyOnLoad(go); // 以此为父节点的所有子节点都不会被销毁,包括各种管理类

//                         foreach (Transform child in go.transform) { // go ==> parent 这个破BUG让我找了好久.....只仅仅是实现的时候手误.....
// // 这里顺承从前的用名字代替了类型,但有时候可能会出错了,要再改一下
//                             string name = child.gameObject.name;
//                             minosDic.Add(name, child.gameObject);

//                             Stack<GameObject> stack = new Stack<GameObject>();
//                             bool isTetro = name.StartsWith("Tetromino");
//                             bool isGhost = name.StartsWith("shadow");
//                             for (int i = 0; i < 10; i++) {
//                                 GameObject tmp = GameObject.Instantiate(child.gameObject);
//                                 tmp.name = name;
//                                 if (isTetro) {
//                                     ComponentHelper.AddTetroComponent(tmp);
//                                     Tetromino tetromino = ComponentHelper.GetTetroComponent(tmp);
//                                     ComponentHelper.GetTetroComponent(tmp).enabled = false;
//                                 } else if (isGhost) {
//                                     ComponentHelper.AddGhostComponent(tmp);
//                                     ComponentHelper.GetGhostComponent(tmp).enabled = false;
//                                 }
//                                 tmp.transform.SetParent(tetrosPool.transform, true); // 把它们放在一个容器下面,免得弄得游戏界面乱七八糟的
//                                 tmp.SetActive(false);
//                                 stack.Push(tmp);
//                             }
//                             pool.Add(name, stack);
//                         }
//                         go.SetActive(false); // 或是因为前面的小错误,这里还没有设置成功
//                     }, EAssetBundleUnloadLevel.Never);
//         }
// 这里好像是: 管理层也都需要红过两个域的适配,这里适配的过程中出了问题;
// BUG: 把这个暂时放一下,等晚点儿再回来独立这个模块,继续放在ViewManager里面,先        
        public GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale) {
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
// 这里的逻辑不对,只在非启蒙模式下才立即激活,逻辑还是放游戏视图里去处理            
            // if (type.StartsWith("Tetromino"))
            //     ComponentHelper.GetTetroComponent(objInstance).enabled = true;
            return objInstance;
        }
    
        public void ReturnToPool(GameObject gameObject, string type) {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
                if (pool[type].Count < 10) {
                    gameObject.transform.position = defaultPos;
                    pool[type].Push(gameObject);
                } else GameObject.DestroyImmediate(gameObject);
            } 
        }

        public GameObject GetFromPool(string type) {
            GameObject objInstance = null;
            if (pool.ContainsKey(type) && pool[type].Count > 0) {
                objInstance = pool[type].Pop();
                objInstance.SetActive(true);
            } else  // tmp commented out
                objInstance = GameObject.Instantiate(minosDic[type]);
            return objInstance;
        }

//         public void ReturnToPool(GameObject gameObject, string type, float delay) {
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