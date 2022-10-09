using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class StaticBtnsView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/staticbtnsview";
            }
        }
        public override string AssetName {
            get {
                return "StaticBtnsView";
            }
        }
        public override string ViewName {
            get {
                return "StaticBtnsView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(StaticBtnsViewModel).FullName;
            }
        }
        public StaticBtnsViewModel ViewModel {
            get {
                return (StaticBtnsViewModel)BindingContext;
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
