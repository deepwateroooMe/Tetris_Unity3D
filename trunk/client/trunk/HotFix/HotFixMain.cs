using UnityEngine;
using HotFix.UI;
using HotFix.Data;

namespace HotFix {

    // 热更工程入口
    public static class HotFixMain {
        public static void Start() {
            Debug.Log("InitializeTypeDatas");
            TypeDataManager.InitializeTypeDatas();
            ViewManager.InitializeItemDatas();
            Debug.Log("HotFixMain.Start()");
            ViewManager.InitializeStartUI();
        }
    }
}
