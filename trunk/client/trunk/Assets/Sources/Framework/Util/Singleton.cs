using System;
using UnityEngine;

namespace tetris3d {

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour { 
        // public class Singleton<T> : MonoBehaviour where T : MonoBehaviour { 
        private const string TAG = "Singleton";
        // 上面的单例模式的实现在单线程下确实是完美的,
        // 然而在多线程的情况下会得到多个Singleton实例,因为在两个线程同时运行GetInstance方法时，此时两个线程判断(uniqueInstance ==null)这个条件时都返回真，
        // 此时两个线程就都会创建Singleton的实例，这样就违背了我们单例模式初衷了，既然上面的实现会运行多个线程执行，
        // 那我们对于多线程的解决方案自然就是使GetInstance方法在同一时间只运行一个线程运行就好了，
        // 也就是我们线程同步的问题了(对于线程同步大家也可以参考我线程同步的文章),
        // 具体的解决多线程的代码如下:
    
        [SerializeField]
        private static bool dontDestroy = true;

        // private static T _instance;
        // private static object locker = new object();

        // volatile修饰：编译器在编译代码的时候会对代码的顺序进行微调，用volatile修饰保证了严格意义的顺序。
        // 一个定义为volatile的变量是说这变量可能会被意想不到地改变，这样，编译器就不会去假设这个变量的值了。
        // 精确地说就是，优化器在用到这个变量时必须每次都小心地重新读取这个变量的值，而不是使用保存在寄存器里的备份。

        // [CanBeNull]
        // private static T _instance;
        private static volatile T _instance; // I think this one is better
        // 定义一个标识确保线程同步
        // [NotNull]
        private static readonly object locker = new object();

        private static bool applicationIsQuiting = false;

        public static T Instance {
            set {
                _instance = value;
            }
            get {
                // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
                // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
                // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
                /*lock (locker) {
                  if (_instance == null) {  */
                // 上面代码对于每个线程都会对线程辅助对象locker加锁之后再判断实例是否存在，对于这个操作完全没有必要的，
                // 因为当第一个线程创建了该类的实例之后，后面的线程此时只需要直接判断（uniqueInstance==null）为假，
                // 此时完全没必要对线程辅助对象加锁之后再去判断，所以上面的实现方式增加了额外的开销，损失了性能，
                // 为了改进上面实现方式的缺陷，我们只需要在lock语句前面加一句（uniqueInstance==null）的判断就可以避免锁所增加的额外开销，这种实现方式我们就叫它 “双重锁定”

                // Debug.Log(TAG + " applicationIsQuiting: " + applicationIsQuiting); 
                if (applicationIsQuiting) {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit." + " Won't create again - returning null.");
                    return null;
                }
                // Debug.Log(TAG + " (_instance == null): " + (_instance == null)); 
                if (_instance == null) {
                    lock (locker) {
                        _instance = FindObjectOfType<T>(); 
                        if (_instance == null) {
                            GameObject singleton = new GameObject(typeof(T).Name);
                            _instance = singleton.AddComponent<T>();
                        }
                        if (dontDestroy) { // made dontDestroy static here
                            _instance.transform.parent = null;  
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
                return _instance;
            }
        }

        // some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)
        // The following scene GameObjects were found:
        // EventManager

        public virtual void Awake() {
        // public void Awake() {
            Debug.Log(TAG + ": Awake()");
            // Debug.Log(TAG + " (gameObject == null): " + (gameObject == null));
            if (gameObject != null) {
                Debug.Log(TAG + " gameObject.name: " + gameObject.name);
                // Debug.Log(TAG + " transform.position: " + transform.position); 
            }

            Debug.Log(TAG + " (_instance == null): " + (_instance == null)); 
            if (_instance == null) {
                _instance = this as T; // 再考虑一下这个 ？？？
                // if (_instance == null) {
                //     GameObject singleton = new GameObject(typeof(T).Name);
                //     _instance = singleton.AddComponent<T>();
                // }
                // Debug.Log(TAG + " dontDestroy: " + dontDestroy); 
                if (dontDestroy) {
                    transform.parent = null;  
                    DontDestroyOnLoad(this.gameObject);
                }
            } else {
                Destroy(gameObject);
            }

            // if (_instance == null) { // 这个 ！！！
            //     // _instance = this as T; // 再考虑一下这个 ？？？
            //     _instance = FindObjectOfType<T>();
            //     if (_instance == null) {
            //         GameObject singleton = new GameObject(typeof(T).Name);
            //         _instance = singleton.AddComponent<T>();
            //     }
            //     // Debug.Log(TAG + " dontDestroy: " + dontDestroy); 
            //     if (dontDestroy) {
            //         transform.parent = null;  
            //         DontDestroyOnLoad(this.gameObject);
            //     }
            // } else {
            //     Destroy(gameObject);
            // }
        }

        public virtual void OnDestroy() {
            Debug.Log(TAG + ": OnDestroy()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
        }

        [RuntimeInitializeOnLoadMethod]
        public static void RunOnStart() { 
            Application.quitting += () => applicationIsQuiting = true;
        }
    }
}