using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    // 这里类似GameView,将MenuView与EducaModesView合并成一个视图，方便其共同的视图模型数据作为ParentViewModel提供给其子视图游戏主视图使用
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

        public override void OnAppear() {
            base.OnAppear(); // 这里,在基类UnityGuiView里OnRevealed()基类实现里,若是根视图,为自动关闭其它所有视图
// 这里是只是赋值回调函数,方便将来回调,还是说这里已经回调了呢(这里应该还没有,回去检查)@!            
            CloseOtherRootView = CloseOtherRootViews; // 只是赋值
        }
        void CloseOtherRootViews() {
            // ViewManager.CloseOtherRootViews(ViewName);
            ViewManager.CloseOtherRootViews("MenuView");
        }

// TODO:　需要把这3个按钮放在一个panel里,方便隐藏(教育模式下选方格时需要隐藏它)
// todo: unity gameobjects, 只在代码里整合了,unity控件里还没能合并在一个视图里
        GameObject menuViewPanel;
        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge
// 把EducaModesView整合进来
        GameObject educamodesviewPanel;
        Toggle thrToggle;
        Toggle furToggle;
        Toggle fivToggle;
        Button conBtn; // CONFIRM

        protected override void OnInitialize() {
            base.OnInitialize();

            menuViewPanel = GameObject.FindChildByName("MenuViewPanel");

            eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
            eduButton.onClick.AddListener(OnClickEduButton);

            claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);

            chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);


            educamodesviewPanel = GameObject.FindChildByName("EducaModesPanel");

            thrToggle = GameObject.FindChildByName("Toggle3").GetComponent<Toggle>();
            furToggle = GameObject.FindChildByName("Toggle4").GetComponent<Toggle>();
            fivToggle = GameObject.FindChildByName("Toggle5").GetComponent<Toggle>();
            
            conBtn = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            conBtn.onClick.AddListener(OnClickConButton);
        }
        // void ChageViewsIndex() {
        //     //UnityEngine.Transform.SetAsLastSibling();
        // }

        void OnClickEduButton() {
            // ViewManager.EducaModesView.Reveal();
            menuViewPanel.SetActive(false);
            educamodesviewPanel.SetActive(true);
            ViewModel.gameMode = 0; // UI点击事件触发视图模型的数据变更, 通过视图模型为桥梁传给子视图模型使用数据
            // Hide();
        }
        void OnClickClaButton() {
                ViewModel.gameMode = 1;
            // ViewManager.FindView.Reveal();
            Hide();
        }
        void OnClickChaButton() {
            ViewModel.gameMode = 2;
            // ViewManager.DesginView.Reveal();
            Hide();
        }

        void ActiveToggle() {
            if (thrToggle.isOn) {
                ViewModel.gridWidth = 3;
            } else if (furToggle.isOn) {
                ViewModel.gridWidth = 4;
            } else if (fivToggle.isOn) {
                ViewModel.gridWidth = 5;
            }

// 这里这个视图的加载之后再考虑,太简单            
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
// // 所有游戏场景公用视图资源等
//             // 想要把这些视图合并成一个的原因是:合并成一个共用一个ViewModel(不合并也可以共用同一个),试图实现视图层与视图模型层模块化双向数据传递
//             // 这个游戏比参考项目中的逻辑更为复杂一点儿,必须去回想上半年当初车载按摩模块View ViewModel的双向数据传递逻辑并在这个项目中实现出来
//             // 这里细分是因为游戏逻辑中有逻辑相关视图的隐藏也显示, 但是我现在去游戏逻辑里找,又还没有找出来.若是并非必要折解成很多小视图,可能还是会把公用游戏逻辑整合到一个视图中来
//             ViewManager.DesView.Reveal(); // 不可变的
//             ViewManager.ScoreDataView.Reveal(); // 可变数据
//             ViewManager.ComTetroView.Reveal();// 所有游戏主场景需要用到的方块砖视图
//             ViewManager.EduTetroView.Reveal();// 教育儿童模式专用的方块砖视图
//             ViewManager.StaticBtnsView.Reveal();// 基本只有按钮的图像变化刷新
//             ViewManager.ToggleBtnView.Reveal(); // 需要改变按钮视图组,调用更为频繁,单列为一个视图(但是可能还是应该合并到上面static里,因其逻辑复杂只是单列出来,能够文件小逻辑更为清淅一点儿?)
//             ViewManager.EduBtnsView.Reveal();   // 教育儿童模式专用两个按钮,只有图像变化

            Hide();  
        }
    }
}
