using Framework.MVVM;
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

        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        protected override void OnInitialize() {
            base.OnInitialize();

            eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
            eduButton.onClick.AddListener(OnClickEduButton);

            claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);

            chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);

            // SetDownRootIndex = ChageViewsIndex;
        }
        // void ChageViewsIndex() {
        //     //UnityEngine.Transform.SetAsLastSibling();
        // }

        void OnClickEduButton() {
            ViewManager.EducaModesView.Reveal();
            // 当前的视图需要隐藏起来吗? 检查一下逻辑
            Hide();
        }
        void OnClickClaButton() {
         
            // ViewManager.FindView.Reveal();
            Hide();
        }
        void OnClickChaButton() {
       
            // ViewManager.DesginView.Reveal();
            Hide();
        }
    }
}
