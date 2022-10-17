using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class DesView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/desview";
            }
        }
        public override string AssetName {
            get {
                return "DesView";
            }
        }
        public override string ViewName {
            get {
                return "DesView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(DesViewModel).FullName;
            }
        }
        public DesViewModel ViewModel {
            get {
                return (DesViewModel)BindingContext;
            }
        }
    }
}
