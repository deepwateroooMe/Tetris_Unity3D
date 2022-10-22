using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Util;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    public class PoolInfo { // 不需要弄得太复杂,反正就每个存10个备用就行了 
        public string type;
        public int amount = 10;
        public GameObject prefab;
        public GameObject container;
        // [HideInInspector]
        // public Stack<GameObject> pool = new Stack<GameObject>(); // 换成栈,好像unity的内存特性里栈比较方便? 
        public List<GameObject> pool = new List<GameObject>(); // 换成栈,好像unity的内存特性里栈比较方便? 
    }

    // 这个管理器类:  执行效率太低,可以简化,可以整合到ViewManager中去
    public class PoolManager : SingletonMono<PoolManager> { // 单例模式
        private const string TAG = "PoolManager";

        private Vector3 defaultPos = new Vector3(-100, -100, -100); // 不同类型的起始位置不一样(可否设置在预设里呢>??)

        public List<PoolInfo> dic; // 为什么使用链表呢,查询效率不是太低了吗? 怎么也得用个字典才对的呀?
        // public Dictionary<string, PoolInfo> dic = new Dictionary<string, PoolInfo>();
        
        public GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3? localScale = null) {
            PoolInfo selected = GetPoolByType(type);
            List<GameObject> pool = selected.pool;
            // Stack<GameObject> pool = dic.get(type).pool;
            GameObject objInstance = null;
            if (pool.Count > 0) {
                objInstance = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
            }
            // else // tmp commented out
            //     objInstance = GameObject.Instantiate(Resources.Load(GetRandomTetrominoPath(type), typeof(GameObject))); // <<<<<<<<<<<<<<<<<<<< 
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            if (localScale == null)
                objInstance.transform.localScale = Vector3.one;
            else
                objInstance.transform.localScale = (Vector3)localScale;
            objInstance.SetActive(true);
            objInstance.transform.SetParent(selected.container.transform, false); // default set here 吧
            return objInstance;
        }
    
        public void ReturnToPool(GameObject gameObject, string type) {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
                gameObject.transform.position = defaultPos;
                PoolInfo selected = GetPoolByType(type);
                gameObject.transform.SetParent(selected.container.transform, false);
                List<GameObject> pool = selected.pool;
                pool.Add(gameObject);
            } 
        }

        void Start() {
            InitPool();
        }
        public void InitPool() {
            for (int i = 0; i < dic.Count; ++i) {
                FillPool(dic[i]);
            }
        }
        private void FillPool(PoolInfo info) {
            for (int i = 0; i < info.amount; i++) {
                GameObject objInstance = null;
                objInstance = GameObject.Instantiate(info.prefab, info.container.transform);
                objInstance.gameObject.SetActive(false);
                objInstance.transform.position = defaultPos;
                info.pool.Add(objInstance);
            }
        }

        public GameObject GetFromPool(string type) {
            PoolInfo selected = GetPoolByType(type);
            List<GameObject> pool = selected.pool;
            GameObject objInstance = null;
            if (pool.Count > 0) {
                objInstance = pool[pool.Count - 1];
                pool.Remove(objInstance);
            }
            // else  // tmp commented out
            //     objInstance = (GameObject)Instantiate(Resources.Load(GetRandomTetrominoPath(type), typeof(GameObject))); // <<<<<<<<<<<<<<<<<<<< 
            objInstance.SetActive(true);
            return objInstance;
        }

        public void ReturnToPool(GameObject gameObject, string type, float delay) {
            CoroutineHelper.StartCoroutine(DelayedReturnToPool(gameObject, type, delay));
        }

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

        private PoolInfo GetPoolByType(string type) {
            for (int i = 0; i < dic.Count; i++) { // 这里用了个链表,查询效率太低,字典快很多
                if (type == dic[i].type) {
                    return dic[i];
                }
            }
            return null;
        }

// // 原逻辑: 根据方块砖的类型来确定不同方块砖的预设的系统存放位置,以便实例化,但热更新的加载逻辑显然不是这样的了        
//         private string GetRandomTetrominoPath(string type) {
//             StringBuilder randomTetrominoName = new StringBuilder("Prefabs/");
//             switch(type) {            
//             case "shapeT": randomTetrominoName.Append("Tetromino_T"); break;
//             case "shapeI": randomTetrominoName.Append("Tetromino_I"); break;
//             case "shapeJ": randomTetrominoName.Append("Tetromino_J"); break;
//             case "shapeL": randomTetrominoName.Append("Tetromino_L"); break;
//             case "shapeO": randomTetrominoName.Append("Tetromino_O"); break;
//             case "shapeS": randomTetrominoName.Append("Tetromino_S"); break;
//             case "shapeZ": randomTetrominoName.Append("Tetromino_Z"); break; 
//             case "shapeX": randomTetrominoName.Append("Tetromino_X"); break; 
//             case "shadowT": randomTetrominoName.Append("shadow_T"); break;
//             case "shadowI": randomTetrominoName.Append("shadow_I"); break;
//             case "shadowJ": randomTetrominoName.Append("shadow_J"); break;
//             case "shadowL": randomTetrominoName.Append("shadow_L"); break;
//             case "shadowO": randomTetrominoName.Append("shadow_O"); break;
//             case "shadowS": randomTetrominoName.Append("shadow_S"); break;
//             case "shadowZ": randomTetrominoName.Append("shadow_Z"); break;
//             case "minoJ": randomTetrominoName.Append("minoJ"); break;
//             case "minoZ": randomTetrominoName.Append("minoZ"); break;
//             case "minoL": randomTetrominoName.Append("minoL"); break;
//             case "minoS": randomTetrominoName.Append("minoS"); break;
//             case "minoO": randomTetrominoName.Append("minoO"); break;
//             case "minoT": randomTetrominoName.Append("minoT"); break;
//             case "minoI": randomTetrominoName.Append("minoI"); break;
//             case "particles": randomTetrominoName.Append("ExplosionParticles"); break;
//             }
//             return randomTetrominoName.ToString();
//         }
    }
}