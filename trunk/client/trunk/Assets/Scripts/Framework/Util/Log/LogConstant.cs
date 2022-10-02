using UnityEngine;
using System.Collections.Generic;

namespace Framework.Util {

    public class LogConstant {
        public static string lastError = string.Empty;

        public static void Initialize() {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer) {
                Rootpath = Application.persistentDataPath;
            }
        }

        static string rootpath = "";
        List<string> items = new List<string>(); // 这个没有用到

        public static string Rootpath {
            get {
                return rootpath;
            }
            set {
                rootpath = value;
            }
        }

        static Queue<string> _cache = new Queue<string>();
        public static Queue<string> Cache {
            get {
                return LogConstant._cache;
            }
        }

        public static bool lastAdd = false;
        public static void AddList(string msg) {
            _cache.Enqueue(msg);
            if (_cache.Count > 30) 
                _cache.Dequeue();
            lastAdd = true;
        }
    }
}
