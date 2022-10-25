using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    // 游戏中会用到比较多的预设,其加载时机,
    // 我认为是在这个游戏菜单视图会比较有停顿,所以把游戏主视图里需要用到的预设资源读取准备工作放在这进而进行

    public class MenuView : UnityGuiView {
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

//         public override void OnAppear() {
//             base.OnAppear(); // 这里,在基类UnityGuiView里OnRevealed()基类实现里,若是根视图,为自动关闭其它所有视图
// // 这里是只是赋值回调函数,方便将来回调,还是说这里已经回调了呢(这里应该还没有,回去检查)@!            
//             CloseOtherRootView = CloseOtherRootViews; // 只是赋值
//         }
//         void CloseOtherRootViews() {
//             ViewManager.CloseOtherRootViews("MenuView");
//         }

        GameObject menuViewPanel;
        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        GameObject educaModesViewPanel;
        Toggle thrToggle;
        Toggle furToggle;
        Toggle fivToggle;
        Button conBtn;     // CONFIRM

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
        }

        void OnClickEduButton() {
            menuViewPanel.SetActive(false);
            educaModesViewPanel.SetActive(true);
            ViewModel.gameMode = 0; // UI点击事件触发视图模型的数据变更, 通过视图模型为桥梁传给子视图模型使用数据
        }
        void OnClickClaButton() {
            ViewModel.gameMode = 1;
            Hide();
        }
        void OnClickChaButton() {
            ViewModel.gameMode = 2;
            Hide();
        }

        void ActiveToggle() {
            if (thrToggle.isOn) 
                ViewModel.gridWidth = 3;
            else if (furToggle.isOn) 
                ViewModel.gridWidth = 4;
            else if (fivToggle.isOn) 
                ViewModel.gridWidth = 5;
// TODO: 这里这个视图的加载之后再考虑,太简单            
            // if (isSavedFileExist()) { 
            //     easyModeToggleSizePanel.SetActive(false);
            //     newGameOrLoadSavedGamePanel.SetActive(true);
            // } else
            //     LoadScene("Main");
        }

        void OnClickConButton() {
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面,这一小步暂时跳过
            ActiveToggle();
            // 感觉这里有个更直接快速的但凡一toggle某个的时候就自动触发的观察者模式,改天再写
            
            ViewManager.GameView.Reveal();

// 需要激活,方便从其它视图回退到主菜单视图            
            menuViewPanel.SetActive(true);
            educaModesViewPanel.SetActive(false);
            Hide(); // 这里没有Hide住应该是
        }
    }
}
