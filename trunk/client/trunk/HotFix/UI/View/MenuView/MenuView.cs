using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

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
        }
        void OnClickClaButton() {
         
            // ViewManager.FindView.Reveal();
        }
        void OnClickChaButton() {
       
            // ViewManager.DesginView.Reveal();
        }
    }
}
