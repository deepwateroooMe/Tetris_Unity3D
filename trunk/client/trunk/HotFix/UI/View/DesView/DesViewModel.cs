using Framework.MVVM;

namespace HotFix.UI {

    public class DesViewModel : ViewModelBase {

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        void Initialization() {
        }

        void DelegateSubscribe() {
        }
    }
}
// namespace HotFix.UI.View.DesView
