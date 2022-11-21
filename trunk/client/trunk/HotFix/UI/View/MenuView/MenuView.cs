using Framework.MVVM;
using HotFix.Control;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    // 游戏中会用到比较多的预设,其加载时机,
    // 我认为是在这个游戏菜单视图会比较有停顿,所以把游戏主视图里需要用到的预设资源读取准备工作放在这进而进行

    public class MenuView : UnityGuiView {
        private const string TAG = "MenuView";
        public override string BundleName {
            get {
                return "ui/view/menuview";
            }
        }
        public override string AssetName {
            get {
                return "MenuView";
            }
        }
        public override string ViewName {
            get {
                return "MenuView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(MenuViewModel).FullName;
            }
        }
        public MenuViewModel ViewModel {
            get {
                return (MenuViewModel)BindingContext;
            }
        }

        public GameObject menuViewPanel;
        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        public GameObject educaModesViewPanel;
        Toggle thrToggle;
        Toggle furToggle;
        Toggle fivToggle;
        Button conBtn;     // CONFIRM

        public GameObject newContinuePanel;
        Button newButton; // New game
        Button conButton; // Load saved game 
        Button canButton; // Cancel
        
        protected override void OnInitialize() {
            base.OnInitialize();
            Debug.Log(TAG + " OnInitialize()");
            GloData.Instance.gameMode.Value = -1; // 因为比如:从挑战模式 切换到 启蒙模式,不改值不能调用回调
            // GloData.Instance.gameMode.OnValueChanged += GloData.Instance.onGameModeSelected; // 将其注册在这里,因为会触发相应路径的改变,但是调不出来
            GloData.Instance.loadSavedGame = false;
            GloData.Instance.isChallengeMode = false; 
            
            menuViewPanel = GameObject.FindChildByName("MenuViewPanel");
            eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
            eduButton.onClick.AddListener(OnClickEduButton);
            claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);
            chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);

            educaModesViewPanel = GameObject.FindChildByName("EducaModesPanel");
            thrToggle = GameObject.FindChildByName("Toggle3").GetComponent<Toggle>();
            furToggle = GameObject.FindChildByName("Toggle4").GetComponent<Toggle>();
            fivToggle = GameObject.FindChildByName("Toggle5").GetComponent<Toggle>();
            conBtn = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            conBtn.onClick.AddListener(OnClickConfirmButton);

            newContinuePanel = GameObject.FindChildByName("BgnNewContinueView");
            newButton = GameObject.FindChildByName("newBtn").GetComponent<Button>();
            
            newButton.onClick.AddListener(OnClickNewGameButton);

            conButton = GameObject.FindChildByName("lodBtn").GetComponent<Button>();
            Debug.Log(TAG + " (conButton == null): " + (conButton == null));
            conButton.onClick.AddListener(OnClickContinueButton);

            canButton = GameObject.FindChildByName("cancelBtn").GetComponent<Button>();
            canButton.onClick.AddListener(OnClickCancelButton);
        }

#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL
            Debug.Log(TAG + " OnClickEduButton()");
            GloData.Instance.saveGamePathFolderName = "educational/grid";
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            menuViewPanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
            GloData.Instance.gameMode.Value = 0;
            GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            ViewManager.SettingsView.Hide();
        }
        void OnClickClaButton() { // CLASSIC MODE
            Debug.Log(TAG + " OnClickClassicButton()");
            GloData.Instance.saveGamePathFolderName = "classic/level";
            GloData.Instance.gridSize.Value = 5;
            GloData.Instance.gridXSize = 5;
            GloData.Instance.gridZSize = 5;
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            GloData.Instance.gameMode.Value = 1;
            GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            offerGameLoadChoice();
            ViewManager.SettingsView.Hide();
        }
        void OnClickChaButton() { // CHALLENGE MODE
            Debug.Log(TAG + " OnClickClallengeButton()");
            GloData.Instance.saveGamePathFolderName = "challenge/level";
            GloData.Instance.isChallengeMode = true;
            ViewManager.ChallLevelsView.Reveal();
            GloData.Instance.gameMode.Value = 0;
            ViewManager.SettingsView.Hide();
            Hide();
        }
#endregion

#region EducaModesPanel
        void OnClickConfirmButton() {
            Debug.Log(TAG + " OnClickConfirmButton()");
            ActiveToggle();
            educaModesViewPanel.SetActive(false);
            offerGameLoadChoice();
        }
        void prepareEnteringNewGame() {
            EventManager.Instance.FireEvent("entergame"); // Audio
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            newContinuePanel.SetActive(false);
// special help for CLASSIC MODE moveCanvas: 只在这个模式下是自动生成,并且最开始两个画布都是失活状态
            if (GloData.Instance.gameMode.Value > 0)
                ViewManager.moveCanvas.SetActive(true);
            ViewManager.GameView.Reveal(); // 放在前面是因为调用需要一点儿时间
            Hide();
        }
        void offerGameLoadChoice() {
            Debug.Log(TAG + " offerGameLoadChoice()");
            if (File.Exists(GloData.Instance.getFilePath())) {
                Debug.Log(TAG + " offerGameLoadChoice() THERE IS a SAVED GAME");
// TODO: BUG 这里被 因为 gameMode而引起的改变已经掩盖掉了 ?                
                newContinuePanel.SetActive(true); 
            } else {
                Debug.Log(TAG + " offerGameLoadChoice() load new game");
                prepareEnteringNewGame();
            }
        }
        void ActiveToggle() {
            if (thrToggle.isOn) {
                ViewModel.gridSize = 3;
                GloData.Instance.gridSize.Value = 3;
                GloData.Instance.gridXSize = 3;
                GloData.Instance.gridZSize = 3;
            } else if (furToggle.isOn) { 
                ViewModel.gridSize = 4;
                GloData.Instance.gridSize.Value = 4;
                GloData.Instance.gridXSize = 4;
                GloData.Instance.gridZSize = 4;
            } else if (fivToggle.isOn) {
                ViewModel.gridSize = 5;
                GloData.Instance.gridSize.Value = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
            }
        }
#endregion        

#region New game or continue saved game panel
        void OnClickNewGameButton() { // Start New game
            Debug.Log(TAG + " OnClickNewGameButton() for NEW GAME");
            GloData.Instance.loadSavedGame = false;
            prepareEnteringNewGame();
        }
        void OnClickContinueButton() { // Load Saved Game
            Debug.Log(TAG + " OnClickLoadSavedGameButton(): for load saved game");
            GloData.Instance.loadSavedGame = true;
            prepareEnteringNewGame();
        }
        void OnClickCancelButton() { // back to main menu
            Debug.Log(TAG + " OnClickCancelButton() back to grid choose");
            newContinuePanel.SetActive(false);
// 只在教育模式下显示如下画板            
            if (GloData.Instance.gameMode.Value == 0 && !GloData.Instance.isChallengeMode)
                educaModesViewPanel.SetActive(true);
        }
#endregion        
    }
}