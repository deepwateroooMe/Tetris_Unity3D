using System.Collections;
using UnityEngine;

namespace Framework.Util {

// 让它具备unity控件的生命周期感知能力
// 这里并不曾覆写什么回调函数的定义,只是为了保持拥有其扩展性吗?
    // public class CoroutineBehaviour<T> : MonoBehaviour {
    public class CoroutineBehaviour : MonoBehaviour {
        private const string TAG = "CoroutineBehaviour";

        // private static CoroutineBehaviour instance;
        
        // public static CoroutineBehaviour Instance {
        //     get {
        //         return instance;
        //     }
        // }

        // public static T instance { get; private set; }
        // void Awake() {
        //     if(instance != null) {
        //         // 防止挂载了多个相同的组件
        //         DestroyImmediate(gameObject);
        //         return;
        //     }
        //     instance = GetComponent<T>();
        // }
    } 
    
    // 协程静态帮助类: 
    public class CoroutineHelper {
    //     static CoroutineBehaviour coroutine;
    // public class CoroutineHelper : CoroutineBehaviour<CoroutineHelper> {
        private const string TAG = "CoroutineHelper"; 

        static CoroutineBehaviour coroutine;
        
        static WaitForSeconds m_waitForOneSecond = new WaitForSeconds(1.1f);

        public static WaitForSeconds WaitForOneSecond {
            get {
                return m_waitForOneSecond;
            }
        }

        // 开启一个协程
        public static Coroutine StartCoroutine(IEnumerator routine) {
// 第一次创建的时候，就是新建控件，保持不销毁，并添加脚本
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


