using UnityEngine;
using HotFix.UI;
using HotFix.Data;

namespace HotFix {
    // 热更工程入口
    public static class HotFixMain {
        private static final String TAG = "HotFixMain"; 

        public static void Start() {
            Debug.Log("HotFixMain InitializeTypeDatas");
            TypeDataManager.InitializeTypeDatas();
            ViewManager.InitializeItemDatas();
            Debug.Log("HotFixMain HotFixMain.Start()");
            ViewManager.InitializeStartUI();
        }
    }
}
