using Framework.MVVM;
using HotFix.Control;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

// 这里已经写了就不想再改写了;代码最后优化的时候可以再试着改写按钮的监听与回调事件3
    public class ChallLevelsView : UnityGuiView {
        private const string TAG = "ChallLevelsView"; 
        public override string BundleName { get { return "ui/view/challlevelsview"; } }
        public override string AssetName { get { return "ChallLevelsView"; } }
        public override string ViewName { get { return "ChallLevelsView"; } }
        public override string ViewModelTypeName { get { return typeof(ChallLevelsViewModel).FullName; } }
        public ChallLevelsViewModel ViewModel { get { return (ChallLevelsViewModel)BindingContext; } }

        GameObject basicPanel;
        Button rightBtn;
        Button oneBtn;
        Button twoBtn;
        Button thrBtn;
        Button forBtn;
        Button fivBtn;
        Button sixBtn;
        Button sevBtn;
        Button eitBtn;
        Button ninBtn;
        Button tenBtn;
        GameObject advancedPanel;
        Button leftBtn;
        Button eleBtn;
        Button twlBtn;
        Button thtBtn;
        Button fotBtn;
        Button fifBtn;
        Button sitBtn;
        Button svtBtn;
        Button attBtn;
        Button nitBtn;
        Button twtBtn;
        GameObject [] levels;
        private GameObject currentLevel;
        
        protected override void OnInitialize() {
            base.OnInitialize();
                
            basicPanel = GameObject.FindChildByName("basicPanel");
            rightBtn = GameObject.FindChildByName("right").GetComponent<Button>();
			rightBtn.onClick.AddListener(OnClickRightButton);
// 目前只先实现这11 关,其它以后有机会再说
            levels = new GameObject[12];
            for (int i = 1; i < 12; i++) 
                levels[i] = ViewManager.basePlane.gameObject.FindChildByName("level" + i);    
    
            oneBtn = GameObject.FindChildByName("oneBtn").GetComponent<Button>();
            oneBtn.onClick.AddListener(OnClickOneButton);
            twoBtn = GameObject.FindChildByName("twoBtn").GetComponent<Button>();
            twoBtn.onClick.AddListener(OnClickTwoButton);
            thrBtn = GameObject.FindChildByName("thrBtn").GetComponent<Button>();
            thrBtn.onClick.AddListener(OnClickThrButton);
            forBtn = GameObject.FindChildByName("forBtn").GetComponent<Button>();
            forBtn.onClick.AddListener(OnClickForButton);
            fivBtn = GameObject.FindChildByName("fivBtn").GetComponent<Button>();
            fivBtn.onClick.AddListener(OnClickFivButton);

            sixBtn = GameObject.FindChildByName("sixBtn").GetComponent<Button>();
            sixBtn.onClick.AddListener(OnClickSixButton);
            sevBtn = GameObject.FindChildByName("sevBtn").GetComponent<Button>();
            sevBtn.onClick.AddListener(OnClickSevButton);
            eitBtn = GameObject.FindChildByName("eitBtn").GetComponent<Button>();
            eitBtn.onClick.AddListener(OnClickEitButton);
            ninBtn = GameObject.FindChildByName("ninBtn").GetComponent<Button>();
            ninBtn.onClick.AddListener(OnClickNinButton);
            tenBtn = GameObject.FindChildByName("tenBtn").GetComponent<Button>();
            tenBtn.onClick.AddListener(OnClickTenButton);

            advancedPanel = GameObject.FindChildByName("advancedPanel");
            leftBtn = GameObject.FindChildByName("left").GetComponent<Button>();
            leftBtn.onClick.AddListener(OnClickLeftButton);

            eleBtn = GameObject.FindChildByName("eleBtn").GetComponent<Button>();
            eleBtn.onClick.AddListener(OnClickEleButton);
            twlBtn = GameObject.FindChildByName("twlBtn").GetComponent<Button>();
            twlBtn.onClick.AddListener(OnClickTwlButton);
            thtBtn = GameObject.FindChildByName("thtBtn").GetComponent<Button>();
            thtBtn.onClick.AddListener(OnClickThtButton);
            fotBtn = GameObject.FindChildByName("fotBtn").GetComponent<Button>();
            fotBtn.onClick.AddListener(OnClickFotButton);
            fifBtn = GameObject.FindChildByName("fifBtn").GetComponent<Button>();
            fifBtn.onClick.AddListener(OnClickFifButton);

            sitBtn = GameObject.FindChildByName("sitBtn").GetComponent<Button>();
            // sitBtn.onClick.AddListener(OnClickSitButton);
            svtBtn = GameObject.FindChildByName("svtBtn").GetComponent<Button>();
            // svtBtn.onClick.AddListener(OnClickSvtButton);
            attBtn = GameObject.FindChildByName("attBtn").GetComponent<Button>();
            // attBtn.onClick.AddListener(OnClickAttButton);
            nitBtn = GameObject.FindChildByName("nitBtn").GetComponent<Button>();
            // nitBtn.onClick.AddListener(OnClickNitButton);
            twtBtn = GameObject.FindChildByName("twtBtn").GetComponent<Button>();
            // twtBtn.onClick.AddListener(OnClickTwtButton);
        }

        void OnClickRightButton() {
            basicPanel.SetActive(false);
            advancedPanel.SetActive(true);
        }        
        void OnClickLeftButton() {
            advancedPanel.SetActive(false);
            basicPanel.SetActive(true);
        }        

        void CallBackHelper(int level) {
// BaseBoardSkin.cs MonoBehaviour: 需要它尽可能早地执行
            GameObject go = ViewManager.basePlane.gameObject.FindChildByName("level" + level);
            if (ComponentHelper.GetBBSkinComponent(go) == null) {
                // Debug.Log(TAG + " (ComponentHelper.GetBBSkinComponent(go) == null): " + (ComponentHelper.GetBBSkinComponent(go) == null));
                BaseBoardSkin baseBoardSkin = ComponentHelper.AddBBSkinComponent(go);
                baseBoardSkin.initateBaseCubesColors(); // could be tried here too.
            }
            switch (level) {
            case 1:
                GloData.Instance.camPos.Value = new Vector3(18.57f, 18.67f, -2.27f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(504.392f, -256.317f, -536.693f));
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetroCnter = 37;
                GloData.Instance.challengeLevel.Value = 1;
                break;
            case 2:
                GloData.Instance.camPos.Value = new Vector3(18.57f, 18.67f, -2.27f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(500.3f, -251.965f, -538.096f));
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                GloData.Instance.challengeLevel.Value = 2;
                break;
            case 3:
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                GloData.Instance.camPos.Value = new Vector3(14.35f, 18.42f, -1.1f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(493.046f, -250.002f, -535.711f));
                GloData.Instance.challengeLevel.Value = 3;
                break;
            case 4:
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                GloData.Instance.camPos.Value = new Vector3(18.57f, 18.67f, -2.27f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(500.037f, -249.718f, -536.287f));
                GloData.Instance.challengeLevel.Value = 4;
                break;
            case 5:
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                GloData.Instance.camPos.Value = new Vector3(18.57f, 18.67f, -2.27f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(500.79f, -247.952f, -536.396f));
                GloData.Instance.challengeLevel.Value = 5;
                break;
            case 6:
                GloData.Instance.gridSize = 9;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                GloData.Instance.camPos.Value = new Vector3(20.94f, 20.5f, -2.7f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(499.977f, -248.536f, -539.603f));
                GloData.Instance.challengeLevel.Value = 6;
                break;
            case 7:
                GloData.Instance.gridXSize = 10;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                GloData.Instance.camPos.Value = new Vector3(20.94f, 20.5f, -2.7f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(499.983f, -248.367f, -537.492f));
                GloData.Instance.challengeLevel.Value = 7;
                break;
            case 8:
                GloData.Instance.camPos.Value = new Vector3(19.87f, 20.31f, -1.58f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(499.11f, -254.084f, -538.676f));
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                GloData.Instance.challengeLevel.Value = 8;
                break;
            case 9:
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                GloData.Instance.camPos.Value = new Vector3(20.04f, 19.84f, 1.1f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(499.773f, -261.044f, -541.281f));
                GloData.Instance.challengeLevel.Value = 9;
                break;
            case 10:
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                GloData.Instance.camPos.Value = new Vector3(22.49f, 22.08f, 1.43f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(498.792f, -262.813f, -537.034f));
                GloData.Instance.challengeLevel.Value = 10;
                break;
            case 11:
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetroCnter = 37;
                GloData.Instance.camPos.Value = new Vector3(21.73f, 24.49f, -0.18f);
                GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(498.488f, -264.273f, -538.78f));
                GloData.Instance.challengeLevel.Value = 11;
                break;
            case 12:
                GloData.Instance.challengeLevel.Value = 12;
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 13:
                GloData.Instance.challengeLevel.Value = 13;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 14:
                GloData.Instance.challengeLevel.Value = 14;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 15:
                GloData.Instance.challengeLevel.Value = 15;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 37;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            }
            GloData.Instance.gameLevel = level;


            levels[level].SetActive(true);
            currentLevel = levels[level];
            hideAllOtherLevelPanel(level);
            if (!ViewManager.basePlane.activeSelf)
                ViewManager.basePlane.SetActive(true);
// 开始新游戏,或是加载保存过的游戏,如果有保存过的文件存在的话
            if (File.Exists(GloData.Instance.getFilePath())) {
                ViewManager.MenuView.Reveal();
                ViewManager.MenuView.menuViewPanel.SetActive(false);
                ViewManager.MenuView.newContinuePanel.SetActive(true);
            } else {
                EventManager.Instance.FireEvent("entergame");
                ViewManager.GameView.Reveal();
            }
            Hide(); // 没有隐藏起来是因为材质没有准备好,其它地为的空异常
        }

        void OnClickOneButton() {
            CallBackHelper(1);
        }
        void OnClickTwoButton() {
            CallBackHelper(2);
        }
        void OnClickThrButton() {
            CallBackHelper(3);
        }
        void OnClickForButton() {
            CallBackHelper(4);
        }
        void OnClickFivButton() {
            CallBackHelper(5);
        }
        void OnClickSixButton() {
            CallBackHelper(6);
        }

        void OnClickSevButton() {
            CallBackHelper(7);
        }
        void OnClickEitButton() {
            CallBackHelper(8);
        }
        void OnClickNinButton() {
            CallBackHelper(9);
        }
        void OnClickTenButton() {
            CallBackHelper(10);
        }
        void OnClickEleButton() {
            CallBackHelper(11);
        }
        void OnClickTwlButton() {
            CallBackHelper(12);
        }
        void OnClickThtButton() {
            CallBackHelper(13);
        }
        void OnClickFotButton() {
            CallBackHelper(14);
        }
        void OnClickFifButton() {
            CallBackHelper(15);
        }
        public void hideAllLevelPanels() {
            currentLevel.SetActive(false);
        }
        void hideAllOtherLevelPanel(int level) {
            for (int i = 1; i < 12; i++) 
                if (i != level && levels[i].activeSelf)
                    levels[i].SetActive(false);
        }
    }
}