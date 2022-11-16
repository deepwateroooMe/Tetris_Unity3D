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
            GloData.Instance.loadSavedGame.Value = false;
            
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
            Debug.Log(TAG + " (conButton == null): " + (conButton == null));
            conButton.onClick.AddListener(OnClickContinueButton);

            canButton = GameObject.FindChildByName("cancelBtn").GetComponent<Button>();
            canButton.onClick.AddListener(OnClickCancelButton);
        }

#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL
            // GloData.Instance.gameMode.Value = 0;
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            ViewModel.gameMode = 0; // 试一下延迟设置这个值
            menuViewPanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
        }
        void OnClickClaButton() { // CLASSIC MODE
            ViewModel.gridWidth = 5;
            ViewModel.gameMode = 1;
            GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            offerGameLoadChoice();
        }
        void OnClickChaButton() { // CHALLENGE MODE
            ViewModel.isChallengeMode = true;
            GloData.Instance.gameMode.Value = 0;
            ViewModel.gameMode = 0; // to match above previously
            ViewManager.ChallLevelsView.Reveal();
            Hide();
        }
#endregion

#region EducaModesPanel
        void OnClickConButton() {
            Debug.Log(TAG + " OnClickConButton()");
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面,这一小步暂时跳过
            ActiveToggle();
// TODO: BUG 因为射线检测还是什么原因,直接调用了加载保存过的游戏,这里需要再改一下            
            offerGameLoadChoice();
            educaModesViewPanel.SetActive(false);
        }
        void prepareEnteringNewGame() {
            EventManager.Instance.FireEvent("entergame"); // Audio
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            newContinuePanel.SetActive(false);
            ViewManager.GameView.Reveal(); // 放在前面是因为调用需要一点儿时间
            Hide();
// special help for CLASSIC MODE moveCanvas: 只在这个模式下是自动生成,并且最开始两个画布都是失活状态
            if (ViewModel.gameMode > 0)
                ViewManager.moveCanvas.SetActive(true);
        }
        void offerGameLoadChoice() {
            Debug.Log(TAG + " offerGameLoadChoice()");
            if (File.Exists(GloData.Instance.getFilePath())) {
                Debug.Log(TAG + " offerGameLoadChoice() THERE IS a SAVED GAME");
// TODO: BUG 这里被 因为 gameMode而引起的改变已经掩盖掉了 ?                
                newContinuePanel.SetActive(true); 
                Debug.Log(TAG + " newContinuePanel.activeSelf: " + newContinuePanel.activeSelf);
            } else {
                Debug.Log(TAG + " offerGameLoadChoice() load new game");
                prepareEnteringNewGame();
            }
        }
        void ActiveToggle() {
            if (thrToggle.isOn) {
                ViewModel.gridWidth = 3;
            } else if (furToggle.isOn) { 
                ViewModel.gridWidth = 4;
            } else if (fivToggle.isOn) {
                ViewModel.gridWidth = 5;
            }
        }
#endregion        

#region New game or continue saved game panel
        void OnClickNewGameButton() { // Start New game
            GloData.Instance.loadSavedGame.Value = false;
            prepareEnteringNewGame();
        }
        void OnClickContinueButton() { // Load Saved Game
            Debug.Log(TAG + " OnClickContinueButton(): for load saved game");
            // 设置标记
            // ViewModel.loadGame.Value = true; // 太慢了
            GloData.Instance.loadSavedGame.Value = true;
            prepareEnteringNewGame();
        }
        void OnClickCancelButton() { // back to main menu
            newContinuePanel.SetActive(false);
// 只在教育模式下显示如下画板            
            if (ViewModel.gameMode == 0 && !GloData.Instance.isChallengeMode)
                educaModesViewPanel.SetActive(true);
        }
#endregion        
    }
}