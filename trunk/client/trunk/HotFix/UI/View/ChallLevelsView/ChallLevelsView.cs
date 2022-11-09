using Framework.MVVM;
using HotFix.Control;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

// 这里已经写了就不想再改写了;代码最后优化的时候可以再试着改写按钮的监听与回调事件
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

        GameObject level1;
        GameObject level2;
        GameObject level3;
        GameObject level4;
        GameObject level5;

        GameObject level6;
        GameObject level7;
        GameObject level8;
        GameObject level9;
        GameObject level10;

        GameObject level11;
        GameObject level12;
        GameObject level13;
        GameObject level14;
        GameObject level15;

        protected override void OnInitialize() {
            base.OnInitialize();

            basicPanel = GameObject.FindChildByName("basicPanel");
            rightBtn = GameObject.FindChildByName("right").GetComponent<Button>();
			rightBtn.onClick.AddListener(OnClickRightButton); 

            level1 = ViewManager.basePlane.gameObject.FindChildByName("level1");
            level2 = ViewManager.basePlane.gameObject.FindChildByName("level2");
            level3 = ViewManager.basePlane.gameObject.FindChildByName("level3");
            level4 = ViewManager.basePlane.gameObject.FindChildByName("level4");
            level5 = ViewManager.basePlane.gameObject.FindChildByName("level5");
            level6 = ViewManager.basePlane.gameObject.FindChildByName("level6");
    
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
            switch (level) {
            case 1:
                level1.SetActive(true);
                GloData.Instance.challengeLevel = 1;
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetroCnter = 22;
                break;
            case 2:
                level1.SetActive(false);
                level2.SetActive(true);
                GloData.Instance.challengeLevel = 2;
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 3:
                level2.SetActive(false);
                level3.SetActive(true);
                GloData.Instance.challengeLevel = 3;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 4:
                level3.SetActive(false);
                level4.SetActive(true);
                GloData.Instance.challengeLevel = 4;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 5:
                level4.SetActive(false);
                level5.SetActive(true);
                GloData.Instance.challengeLevel = 5;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 6:
                level5.SetActive(false);
                level6.SetActive(true);
                GloData.Instance.challengeLevel = 6;
                GloData.Instance.gridSize = 9;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 7:
                GloData.Instance.challengeLevel = 7;
                // GloData.Instance.gridSize = 7; // WHY commented out ??? to be fixed
                GloData.Instance.gridXSize = 10;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 8:
                GloData.Instance.challengeLevel = 8;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 9:
                GloData.Instance.challengeLevel = 9;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 10:
                GloData.Instance.challengeLevel = 10;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 42;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 11:
                GloData.Instance.challengeLevel = 11;
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                GloData.Instance.tetroCnter = 22;
                break;
            case 12:
                GloData.Instance.challengeLevel = 12;
                GloData.Instance.gridSize = 7;
                GloData.Instance.gridXSize = 7;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 13:
                GloData.Instance.challengeLevel = 13;
                GloData.Instance.gridXSize = 9;
                GloData.Instance.gridZSize = 7;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(3.0f, Model.gridHeight - 1f, 3.0f);
                break;
            case 14:
                GloData.Instance.challengeLevel = 14;
                GloData.Instance.gridSize = 8;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 8;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            case 15:
                GloData.Instance.challengeLevel = 15;
                GloData.Instance.gridXSize = 8;
                GloData.Instance.gridZSize = 9;
                GloData.Instance.tetroCnter = 22;
                GameView.nextTetrominoSpawnPos = new Vector3(4.0f, Model.gridHeight - 1f, 4.0f);
                break;
            }
            GloData.Instance.gameLevel = level;
            ViewManager.GameView.Reveal();
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
    }
}