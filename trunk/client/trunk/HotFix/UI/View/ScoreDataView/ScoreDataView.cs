using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class ScoreDataView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/scoredataview";
            }
        }
        public override string AssetName {
            get {
                return "ScoreDataView";
            }
        }
        public override string ViewName {
            get {
                return "ScoreDataView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(ScoreDataViewModel).FullName;
            }
        }
        public ScoreDataViewModel ViewModel {
            get {
                return (ScoreDataViewModel)BindingContext;
            }
        }

        Text scoText; // Score Text
        Text lvlText; // Level Text
        Text linText; // Line Text

        protected override void OnInitialize() {
            base.OnInitialize();

            scoText = GameObject.FindChildByName("scoTxt").GetComponent<Text>();
            lvlText = GameObject.FindChildByName("lvlTxt").GetComponent<Text>();
            linText = GameObject.FindChildByName("linTxt").GetComponent<Text>();

            // 需要有来自ViewModel的数据变化来刷新UI
        }
    }
}
