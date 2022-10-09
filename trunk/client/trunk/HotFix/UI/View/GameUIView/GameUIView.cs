using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    // 暂时仍只使用UI2DRoot视图,有必要时再用它优化
    public class GameUIView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/gameuiview";
            }
        }
        public override string AssetName {
            get {
                return "GameUIView";
            }
        }
        public override string ViewName {
            get {
                return "GameUIView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(GameUIViewModel).FullName;
            }
        }
        public GameUIViewModel ViewModel {
            get {
                return (GameUIViewModel)BindingContext;
            }
        }
        
    }
}

