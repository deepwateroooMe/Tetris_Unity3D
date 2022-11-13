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

        GameObject menuViewPanel;
        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        GameObject educaModesViewPanel;
        Toggle thrToggle;
        Toggle furToggle;
        Toggle fivToggle;
        Button conBtn;     // CONFIRM

        GameObject newContinuePanel;
        Button newButton; // New game
        Button conButton; // Load saved game 
        Button canButton; // Cancel

        
        protected override void OnInitialize() {
            base.OnInitialize();

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
            conBtn.onClick.AddListener(OnClickConButton);

            newContinuePanel = GameObject.FindChildByName("BgnNewContinueView");
            newButton = GameObject.FindChildByName("newBtn").GetComponent<Button>();
            newButton.onClick.AddListener(OnClickNewGameButton);
            conButton = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            conButton.onClick.AddListener(OnClickContinueButton);
            canButton = GameObject.FindChildByName("cancelBtn").GetComponent<Button>();
            canButton.onClick.AddListener(OnClickCancelButton);
        }

#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL
// TODO BUG: 这个还残存了些BUG没有改完            
            ViewModel.gameMode = 0; 

            menuViewPanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
        }
        void OnClickClaButton() { // CLASSIC MODE
            ViewModel.gameMode = 1;
            ViewModel.gridWidth = 5;

            prepareEnteringNewGame();
        }
        void OnClickChaButton() { // CHALLENGE MODE
            ViewModel.gameMode = 0; // to match above previously
            ViewModel.isChallengeMode = true;
            
            ViewManager.ChallLevelsView.Reveal();
            Hide();
        }
#endregion

#region EducaModesPanel
        void ActiveToggle() {
            if (thrToggle.isOn) {
                ViewModel.gridWidth = 3;
            } else if (furToggle.isOn) { 
                ViewModel.gridWidth = 4;
            } else if (fivToggle.isOn) {
                ViewModel.gridWidth = 5;
            }
        }
        void OnClickConButton() {
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面,这一小步暂时跳过
            ActiveToggle();

            educaModesViewPanel.SetActive(false);
            if (File.Exists(GloData.Instance.getFilePath())) {
                Debug.Log(TAG + " OnClickConButton() THERE IS a SAVED GAME");
// TODO: BUG 这里被 因为 gameMode而引起的改变已经掩盖掉了 ?                
                newContinuePanel.SetActive(true); 
            } else {
                prepareEnteringNewGame();
            }
        }
        void prepareEnteringNewGame() {
            EventManager.Instance.FireEvent("entergame"); // Audio
            ViewManager.GameView.Reveal(); // 放在前面是因为调用需要一点儿时间
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            newContinuePanel.SetActive(false);
            Hide(); 
        }
#endregion        

#region New game or continue saved game panel
        void OnClickNewGameButton() { // Start New game
            prepareEnteringNewGame();
        }
        void OnClickContinueButton() { // Load Saved Game
            // 设置标记
            ViewModel.loadGame.Value = true;

            ViewManager.GameView.Reveal();
            newContinuePanel.SetActive(false);
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            Hide();
        }
        void OnClickCancelButton() { // back to main menu
            newContinuePanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
        }
#endregion        
    }
}