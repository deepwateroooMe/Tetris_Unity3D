using UnityEngine;
using HotFix.UI;
using HotFix.Data;

namespace HotFix {

    // 热更工程入口
    public static class HotFixMain {
        public static void Start() {
            Debug.Log("InitializeTypeDatas");

            // 热更新资源包中加载数据,反序列化:从资源包里反序列化地加载各种类型数据(若有预设,这些与预设中的元件部件类型等一一对应)
            TypeDataManager.InitializeTypeDatas();

            // UI视图中:某些场景元件部件的数据加载反序列化等
            //ViewManager.InitializeItemDatas();
            Debug.Log("HotFixMain.Start()");

            // 初始化加载UI视图中的起始视图
            ViewManager.InitializeStartUI();
        }
    }
}