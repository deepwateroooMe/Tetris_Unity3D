using System;
using UnityEngine;

namespace Framework.Util {

    public class Singleton<T> where T : class, new() {
        protected static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public static T GetInstance() {
            return Instance;
        }
    }
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
        protected static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<T>();
                    obj.name = _instance.GetType().Name;
                }
                return _instance;
            }
        }
        public static T GetInstance() {
            return Instance;
        }
        public static void DestoryInstance() {
            if (_instance == null)
                return;
            GameObject obj = _instance.gameObject;
            //ResourceMgr.Instance.DestroyObject(obj);
        }
    }
    
    // public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour { 
    //     private const string TAG = "Singleton";
    //     [SerializeField]
    //     private static bool dontDestroy = true;
    //     private static volatile T _instance; // I think this one is better
    //     private static readonly object locker = new object();
    //     private static bool applicationIsQuiting = false;
    //     public static T Instance {
    //         set {
    //             _instance = value;
    //         }
    //         get {
    //             if (applicationIsQuiting) {
    //                 Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit."
    //                                  + " Won't create again - returning null.");
    //                 return null;
    //             }
    //             if (_instance == null) {
    //                 lock (locker) {
    //                     _instance = FindObjectOfType<T>(); 
    //                     if (_instance == null) {
    //                         GameObject singleton = new GameObject(typeof(T).Name);
    //                         _instance = singleton.AddComponent<T>();
    //                     }
    //                     if (dontDestroy) { // made dontDestroy static here
    //                         _instance.transform.parent = null;  
    //                         DontDestroyOnLoad(_instance.gameObject);
    //                     }
    //                 }
    //             }
    //             return _instance;
    //         }
    //     }
    //     public virtual void Awake() {
    //         Debug.Log(TAG + ": Awake()");
    //         if (gameObject != null) {
    //             Debug.Log(TAG + " gameObject.name: " + gameObject.name);
    //         }
    //         Debug.Log(TAG + " (_instance == null): " + (_instance == null)); 
    //         if (_instance == null) {
    //             _instance = this as T; // 再考虑一下这个 ？？？
    //             if (dontDestroy) {
    //                 transform.parent = null;  
    //                 DontDestroyOnLoad(this.gameObject);
    //             }
    //         } else {
    //             Destroy(gameObject);
    //         }
    //     }
    //     public virtual void OnDestroy() {
    //         Debug.Log(TAG + ": OnDestroy()");
    //         Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
    //     }
    //     [RuntimeInitializeOnLoadMethod]
    //     public static void RunOnStart() { 
    //         Application.quitting += () => applicationIsQuiting = true;
    //     }
    // }
}