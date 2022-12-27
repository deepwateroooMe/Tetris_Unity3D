using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Util {

    public class MonoBehaviourInstance<T> : MonoBehaviour {
        private const string TAG = "MonoBehaviourInstance"; 

        public static T instance { get; private set; }
    
        void Awake() {
            if(instance != null) {
                // 防止挂载了多个相同的组件
                DestroyImmediate(gameObject);
                return;
            }
            instance = GetComponent<T>();
        }
    }
}
