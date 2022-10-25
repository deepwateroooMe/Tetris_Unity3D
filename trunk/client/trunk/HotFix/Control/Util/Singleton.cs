using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 如果我用主工程中定义的,它居然还说适配不了AudioManager;
// 没办法,暂时在热更新工程内部再复制一遍这个单例模式(这样AudioManager就没有问题了)
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
}
