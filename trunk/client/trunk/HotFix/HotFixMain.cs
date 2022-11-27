using UnityEngine;
using HotFix.UI;
using HotFix.Data;
using HotFix.Control;

namespace HotFix {

    public static class HotFixMain {
        private const string TAG = "HotFixMain"; 

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

        public static void startEducational() {
            Debug.Log(TAG + " startEducational()");
            GloData.Instance.saveGamePathFolderName = "educational/grid";
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            // menuViewPanel.SetActive(false);
            // educaModesViewPanel.SetActive(true);
            GloData.Instance.gameMode.Value = 0;
            GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            // ViewManager.SettingsView.Hide();
        }
        public static void startClassical() {
            Debug.Log(TAG + " startClassical()");
            GloData.Instance.saveGamePathFolderName = "classic/level";
            GloData.Instance.gridSize.Value = 5;
            GloData.Instance.gridXSize = 5;
            GloData.Instance.gridZSize = 5;
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            GloData.Instance.gameMode.Value = 1;
            GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            // offerGameLoadChoice();
            // ViewManager.SettingsView.Hide();
        }
        public static void startChallenging() {
            Debug.Log(TAG + " startChallenging()");
            GloData.Instance.saveGamePathFolderName = "challenge/level";
            GloData.Instance.isChallengeMode = true;
            ViewManager.ChallLevelsView.Reveal();
            GloData.Instance.gameMode.Value = 0;
            // ViewManager.SettingsView.Hide();
            // Hide();
        }

    }
}