using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Framework.Util;

namespace tetris3d {
// C# Memory Management for Unity Developers (part 3 of 3)
// https://www.gamasutra.com/blogs/WendelinReich/20131127/203843/C_Memory_Management_for_Unity_Developers_part_3_of_3.php
// 2 https://www.gamasutra.com/blogs/WendelinReich/20131119/203842/C_Memory_Management_for_Unity_Developers_part_2_of_3.php
// 1 https://www.gamasutra.com/blogs/WendelinReich/20131109/203841/C_Memory_Management_for_Unity_Developers_part_1_of_3.php
// 扩展方法用法及其原理和注意事项    
// https://www.cnblogs.com/CreateMyself/p/4724527.html    
    
    [System.Serializable]
    public class PoolInfo {
        public string type;
        public int amount;
        public GameObject prefab;
        public GameObject container;
        [HideInInspector]
        public List<GameObject> pool = new List<GameObject>(); 
    }

    public class PoolManager : Singleton<PoolManager> {
        private const string TAG = "PoolManager";

        private Vector3 defaultPos = new Vector3(-100, -100, -100);
        public List<PoolInfo> listOfPool; 

        public Material [] materials; // [red, green, blue, yellow]
        public Material [] colors;
        
        // void OnEnable() {
        //     Debug.Log(TAG + ": OnEnable()");
        //     Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
        // }
        // void OnDisable() {
        //     Debug.Log(TAG + ": OnDisable()");
        //     Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
        // }
        
        public GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, int color = 0) { // for challenge mode colored minos only
            PoolInfo selected = GetPoolByType(type);
            List<GameObject> pool = selected.pool;
            GameObject objInstance = null;
            if (pool.Count > 0) {
                objInstance = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
            } else
                objInstance = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(type), typeof(GameObject)));
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;

            if (GloData.Instance.isChallengeMode && type.Substring(0, 4).Equals("mino") && !type.Substring(0, 5).Equals("minoP")) { // isChallengeMode = true, gameMode = 0
                objInstance.GetComponent<MinoType>().color = color;
                objInstance.GetComponent<Renderer>().sharedMaterial = materials[color];
            }
            objInstance.SetActive(true);
            objInstance.transform.SetParent(selected.container.transform, false); // default set here 吧
            return objInstance;
        }

        public GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3? localScale = null) {
            Debug.Log(TAG + " type: " + type); 
            PoolInfo selected = GetPoolByType(type);
            List<GameObject> pool = selected.pool;
            GameObject objInstance = null;
            if (pool.Count > 0) {
                objInstance = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
            } else
                objInstance = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(type), typeof(GameObject)));
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;

            if (GloData.Instance.isChallengeMode && type.Substring(0, 5).Equals("shape")) { // isChallengeMode = true, gameMode = 0
                int rand = 0, randomColor1 = 0, randomColor2 = 0, cnt = 0;

                Debug.Log(TAG + " GloData.Instance.challengeLevel: " + GloData.Instance.challengeLevel); 
                if (GameController.isChallengeMode && GloData.Instance.challengeLevel > 10 && GloData.Instance.challengeLevel < 16) { // match 2
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
                            child.gameObject.GetComponent<Renderer>().sharedMaterial = materials[randomColor1];
                            Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                        } else {
                            child.gameObject.GetComponent<MinoType>().color = randomColor2;
                            child.gameObject.GetComponent<Renderer>().sharedMaterial = materials[randomColor2];
                            Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
                        }
                    }
                } else { // 1 color per tetromino
                    int randomColor = UnityEngine.Random.Range(0, 4);
                    objInstance.GetComponent<TetrominoType>().color = randomColor;
                    Debug.Log(TAG + " randomColor: " + randomColor); 

                    foreach (Transform child in objInstance.transform) {
                        child.gameObject.GetComponent<MinoType>().color = randomColor;
                        child.gameObject.GetComponent<Renderer>().sharedMaterial = materials[randomColor];
                        // Debug.Log(TAG + " child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString: " + child.gameObject.GetComponent<Renderer>().sharedMaterial.ToString()); 
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
            objInstance.SetActive(true);
            objInstance.transform.SetParent(selected.container.transform, false);
            return objInstance;
        }
        
        public GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale, int color) {
            PoolInfo selected = GetPoolByType(type);
            List<GameObject> pool = selected.pool;
            GameObject objInstance = null;
            if (pool.Count > 0) {
                objInstance = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
            } else
                objInstance = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(type), typeof(GameObject)));
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            objInstance.transform.localScale = localScale;

            objInstance.GetComponent<TetrominoType>().color = color;
            foreach (Transform child in objInstance.transform) {
                child.gameObject.GetComponent<MinoType>().color = color;
                child.gameObject.GetComponent<Renderer>().sharedMaterial = materials[color];
            }
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
            for (int i = 0; i < listOfPool.Count; ++i) {
                FillPool(listOfPool[i]);
            }
        }
        private void FillPool(PoolInfo info) {
            for (int i = 0; i < info.amount; i++) {
                GameObject objInstance = null;
                objInstance = Instantiate(info.prefab, info.container.transform);
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
            } else 
                objInstance = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(type), typeof(GameObject)));
            objInstance.SetActive(true);
            return objInstance;
        }

        public void ReturnToPool(GameObject gameObject, string type, float delay) {
            StartCoroutine(DelayedReturnToPool(gameObject, type, delay));
        }

        IEnumerator DelayedReturnToPool(GameObject gameObject, string type, float delayTime) {
            while (delayTime > 0f) {
                yield return null;
                if (!gameObject.activeInHierarchy) { // If the instance was deactivated while waiting here, just quit
                    yield break;
                }
                delayTime -= Time.deltaTime;
            }
            ReturnToPool(gameObject, type);
        }

        private PoolInfo GetPoolByType(string type) {
            for (int i = 0; i < listOfPool.Count; i++) {
                if (type == listOfPool[i].type) {
                    return listOfPool[i];
                }
            }
            return null;
        }

        private string GetRandomTetromino(string type) {
            StringBuilder randomTetrominoName = new StringBuilder("Prefabs/");
            switch(type) {
                case "shape0": randomTetrominoName.Append("Tetromino_0"); break;
                case "shapeB": randomTetrominoName.Append("Tetromino_B"); break;
                case "shapeC": randomTetrominoName.Append("Tetromino_C"); break;
                case "shapeI": randomTetrominoName.Append("Tetromino_I"); break;
                case "shapeJ": randomTetrominoName.Append("Tetromino_J"); break;
                case "shapeL": randomTetrominoName.Append("Tetromino_L"); break;
                case "shapeO": randomTetrominoName.Append("Tetromino_O"); break;
                case "shapeS": randomTetrominoName.Append("Tetromino_S"); break;
                case "shapeT": randomTetrominoName.Append("Tetromino_T"); break;
                case "shapeR": randomTetrominoName.Append("Tetromino_R"); break; 
                case "shapeY": randomTetrominoName.Append("Tetromino_Y"); break; 
                case "shapeX": randomTetrominoName.Append("Tetromino_X"); break; 
                case "shapeZ": randomTetrominoName.Append("Tetromino_Z"); break; 

                case "shadow0": randomTetrominoName.Append("shadow_0"); break;
                case "shadowB": randomTetrominoName.Append("shadow_B"); break;
                case "shadowC": randomTetrominoName.Append("shadow_C"); break;
                case "shadowI": randomTetrominoName.Append("shadow_I"); break;
                case "shadowJ": randomTetrominoName.Append("shadow_J"); break;
                case "shadowL": randomTetrominoName.Append("shadow_L"); break;
                case "shadowO": randomTetrominoName.Append("shadow_O"); break;
                case "shadowS": randomTetrominoName.Append("shadow_S"); break;
                case "shadowT": randomTetrominoName.Append("shadow_T"); break;
                case "shadowR": randomTetrominoName.Append("shadow_R"); break; 
                case "shadowX": randomTetrominoName.Append("shadow_X"); break; 
                case "shadowY": randomTetrominoName.Append("shadow_Y"); break; 
                case "shadowZ": randomTetrominoName.Append("shadow_Z"); break;

                case "mino0": randomTetrominoName.Append("mino0"); break;
                case "minoB": randomTetrominoName.Append("minoB"); break;
                case "minoC": randomTetrominoName.Append("minoC"); break;
                case "minoI": randomTetrominoName.Append("minoI"); break;
                case "minoJ": randomTetrominoName.Append("minoJ"); break;
                case "minoL": randomTetrominoName.Append("minoL"); break;
                case "minoO": randomTetrominoName.Append("minoO"); break;
                case "minoS": randomTetrominoName.Append("minoS"); break;
                case "minoT": randomTetrominoName.Append("minoT"); break;
                case "minoR": randomTetrominoName.Append("minoR"); break; 
                case "minoY": randomTetrominoName.Append("minoY"); break; 
                case "minoZ": randomTetrominoName.Append("minoZ"); break;

                case "gostR": randomTetrominoName.Append("gostR"); break;
                case "gostG": randomTetrominoName.Append("gostG"); break; 
                case "gostB": randomTetrominoName.Append("gostB"); break; 
                case "gostY": randomTetrominoName.Append("gostY"); break;

                case "minoPS": randomTetrominoName.Append("minoPS"); break;
                case "particles": randomTetrominoName.Append("ExplosionParticles"); break;
            }
            return randomTetrominoName.ToString();
        }
    }
}
