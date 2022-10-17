using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    // 还有两个世界坐标系的视图：MoveCanvasView 和RotateCanvasView供其调控
    public class GameView : UnityGuiView {
        public override string BundleName {
            get {
                return "/view/gameview";
            }
        }
        public override string AssetName {
            get {
                return "GameView";
            }
        }
        public override string ViewName {
            get {
                return "GameView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(GameViewModel).FullName;
            }
        }
        public GameViewModel ViewModel {
            get {
                return (GameViewModel)BindingContext;
            }
        }
        public override bool IsRoot { // 方便调控显示与隐藏
            get {
                return true;
            }
        }

// Five Four ThreeGridView
        // 因为整合到一个大的视图,那么要同时有三种格子,只显示其中的一种,并隐藏剩余的两种而已
        GameObject baseBoard3;
        GameObject baseBoard4;
        GameObject baseBoard5;
// DesView 里的个文本框基本不变，不用管它们        
// ScoreDataView
        Text scoText; // Score Text
        Text lvlText; // Level Text
        Text linText; // Line Text
// StaticBtnsView
        Button pauButton; // pause Game
        Button falButton; // fall fast button
// ToggleBtnView
        int type = 0;
        Button togButton; // Togcation
// EduBtnsView
        Button swaButton; // swap current tetrominos to be a newly generating(it's coming after click) tetromino set
        Button undButton; // undo last selected tetromino landing, revert it back

// ComTetroView
        Button ptoButton; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个
// EduTetroView
        Button pteButton; // preview Tetromino Button COMMON BUTTON
        //  这里有个方块砖需要显示出来，但是视图上好像不用做什么？逻辑层负责生产一个
        
        // 需要有来自ViewModel的数据变化来刷新UI: 观察者模式观察视图模型中数据的变体
        protected override void OnInitialize() {
            base.OnInitialize();

            switch (((EducaModesViewModel)BindingContext.ParentViewModel).GridWidth) { // 大方格的类型
            case 3:
                baseBoard3.SetActive(true);
                break;
            case 4:
                baseBoard4.SetActive(true);
                break;
            case 5:
                baseBoard5.SetActive(true);
                break;
            }
            
            scoText = GameObject.FindChildByName("scoTxt").GetComponent<Text>();
            lvlText = GameObject.FindChildByName("lvlTxt").GetComponent<Text>();
            linText = GameObject.FindChildByName("linTxt").GetComponent<Text>();

            pauButton = GameObject.FindChildByName("pauBtn").GetComponent<Button>();
            pauButton.onClick.AddListener(OnClickPauButton);
            falButton = GameObject.FindChildByName("falBtn").GetComponent<Button>();
            falButton.onClick.AddListener(OnClickFalButton);

            togButton = GameObject.FindChildByName("togBtn").GetComponent<Button>();
            togButton.onClick.AddListener(OnClickTogButton);

            swaButton = GameObject.FindChildByName("swaBtn").GetComponent<Button>();
            swaButton.onClick.AddListener(OnClickSwaButton);
            undButton = GameObject.FindChildByName("undBtn").GetComponent<Button>();
            undButton.onClick.AddListener(OnClickUndButton);

            ptoButton = GameObject.FindChildByName("ptoBtn").GetComponent<Button>();
            ptoButton.onClick.AddListener(OnClickPtoButton);
            pteButton = GameObject.FindChildByName("pteBtn").GetComponent<Button>();
            pteButton.onClick.AddListener(OnClickPteButton);

        }
        void OnClickPauButton() {
            ViewModel.PauseGame(); // 游戏暂停
            ViewManager.MidMenuView.Reveal();
            // Hide(); // 若是隐藏,这里也只会隐藏当前视图- StaticBtnsView一个视图,而不是游戏场景里的所有小视图
        }
        void OnClickFalButton() {
        }

        void OnClickTogButton() {
            // 当前的视图需要隐藏起来吗? 检查一下逻辑
            type ^= 1;
// 根据当前需要显示的按钮组的值的不同,来显示平移组或是旋转组
// 这两个组的按钮也需要分别制作成视图,两个不同的视图
            // if (type == 0)
            // else
            Hide();
        }
        void OnClickPtoButton() { // preview Tetromino Button COMMON BUTTON
        }
        void OnClickPteButton() { // preview Tetromino Button EDUCATION BUTTON
        }

        void OnClickSwaButton() {
            // 这里需要下发指令到视图数据层,并根据随机数生成的新的tetromino来重新刷新UI
        }
        void OnClickUndButton() {
            // 类似的逻辑下发数据,并由数据驱动刷新UI
        }
        
    }
}


