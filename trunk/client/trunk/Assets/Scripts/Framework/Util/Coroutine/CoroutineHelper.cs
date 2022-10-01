using System.Collections;
using UnityEngine;

namespace Framework.Util {

    // 让它具备unity控件的生命周期感知能力
    public class CoroutineBehaviour : MonoBehaviour {} 

    
    // 协程帮助类
    public class CoroutineHelper {

        static CoroutineBehaviour coroutine;

        // 开启一个协程
        public static Coroutine StartCoroutine(IEnumerator routine) {
            if (coroutine == null) {
                GameObject coroutineGameObject = new GameObject("CoroutineGameObject");
                Object.DontDestroyOnLoad(coroutineGameObject);
                coroutine = coroutineGameObject.AddComponent<CoroutineBehaviour>();
            }
            return coroutine.StartCoroutine(routine);
        }

        // 终止协程
        public static void StopCoroutine(Coroutine routine) {
            if (coroutine != null) 
                coroutine.StopCoroutine(routine);
        }
    }
}
