using Framework.MVVM;

namespace HotFix.UI {

    public class ToggleBtnViewModel : ViewModelBase {

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
