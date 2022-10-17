using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class EduTetroView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/edutetroview";
            }
        }
        public override string AssetName {
            get {
                return "EduTetroView";
            }
        }
        public override string ViewName {
            get {
                return "EduTetroView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(EduTetroViewModel).FullName;
            }
        }
        public EduTetroViewModel ViewModel {
            get {
                return (EduTetroViewModel)BindingContext;
            }
        }
    }
}
