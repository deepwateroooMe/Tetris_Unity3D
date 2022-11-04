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
// Main MenuView panel
        void OnClickEduButton() {
            menuViewPanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
            ViewModel.gameMode = 0; // UI点击事件触发视图模型的数据变更, 通过视图模型为桥梁传给子视图模型使用数据
        }
        void OnClickClaButton() { // classic mode
            ViewModel.gameMode = 1;
            ViewModel.gridWidth = 5;
            prepareEnteringNewGame();
        }
        void OnClickChaButton() {
            ViewModel.gameMode = 2;
// TODO:这里会是一套不同的逻辑 
            // EventManager.Instance.FireEvent("entergame");
            // Hide();
        }
// EducaModesPanel
        void ActiveToggle() {
            if (thrToggle.isOn) 
                ViewModel.gridWidth = 3;
            else if (furToggle.isOn) 
                ViewModel.gridWidth = 4;
            else if (fivToggle.isOn) 
                ViewModel.gridWidth = 5;
        }
        void OnClickConButton() {
            Debug.Log(TAG + " OnClickConButton");
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面,这一小步暂时跳过
            ActiveToggle();
// BUG TODO: 当有保存的文件存在,不知道为什么它就自动进入到了新游戏            
// TODO: 检查客户端是否存有同户的进展文件:有显示提示框,提示是否需要加载保存的进展;没有直接进入新游戏视图
            bool tmp = isSavedFileExist();
            Debug.Log(TAG + " tmp: " + tmp);
// TODO: 感觉这里有个更直接快速的但凡一toggle某个的时候就自动触发的观察者模式,改天再写
            educaModesViewPanel.SetActive(false);
            // if (isSavedFileExist()) {
            if (tmp) {
                newContinuePanel.SetActive(true); // BUG 没有显示
                Debug.Log(TAG + " newContinuePanel.activeSelf: " + newContinuePanel.activeSelf);
            } else {
                Debug.Log(TAG + " OnClickConButton() else");
                prepareEnteringNewGame();
            }
        }
// New game or continue saved game panel
        void OnClickNewGameButton() { // Start New game
            prepareEnteringNewGame();
        }
        void OnClickContinueButton() { // Load Saved Game
// TODO: load saved game
            ViewManager.GameView.Reveal();
// TODO: 这里要等视图加载完成后,加载游戏进度数据 ?
            newContinuePanel.SetActive(false);
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            Hide();
        }
        void OnClickCancelButton() { // back to main menu
            newContinuePanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
        }
        void prepareEnteringNewGame() {
            Debug.Log(TAG + " prepareEnteringNewGame");

            EventManager.Instance.FireEvent("entergame");
            ViewManager.GameView.Reveal(); // 放在前面是因为调用需要一点儿时间
            menuViewPanel.SetActive(true); // 需要激活,方便从其它视图回退到主菜单视图
            newContinuePanel.SetActive(false);
            Hide(); 
        }
        private bool isSavedFileExist() {
            Debug.Log(TAG + ": isSavedFileExist()");
            StringBuilder currentPath = new StringBuilder("");
            if (ViewModel.gameMode > 0)
                currentPath.Append(Application.persistentDataPath + ViewModel.saveGamePathFolderName + "/game.save");
            else 
                currentPath.Append(Application.persistentDataPath + ViewModel.saveGamePathFolderName + "/grid"
                                   + ViewModel.gridWidth + "/game.save");
            Debug.Log(TAG + " currentPath: " + currentPath.ToString());
            Debug.Log(TAG + " (File.Exists(currentPath.ToString())): " + (File.Exists(currentPath.ToString())));
            if (File.Exists(currentPath.ToString()))
                return true;
            return false;
        }
    }
}