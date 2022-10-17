using Framework.MVVM;

namespace HotFix.UI {

    public class GameViewModel : ViewModelBase {

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        void Initialization() {
            this.ParentViewModel = ViewManager.EducaModesView.BindingContext;
        }

        void DelegateSubscribe() {
        }

        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() {
        }
     }
}
