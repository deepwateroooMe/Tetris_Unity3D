using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    // 也可以想成这个视图可以基本不用变,只绘制方块砖即可
    public class FiveGridView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/fivegridview";
            }
        }
        public override string AssetName {
            get {
                return "FiveGridView";
            }
        }
        public override string ViewName {
            get {
                return "FiveGridView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(FiveGridViewModel).FullName;
            }
        }
        public FiveGridViewModel ViewModel {
            get {
                return (FiveGridViewModel)BindingContext;
            }
        }
    }
}
