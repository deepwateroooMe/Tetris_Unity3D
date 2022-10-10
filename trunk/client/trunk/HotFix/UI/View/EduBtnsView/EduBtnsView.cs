using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    public class EduBtnsView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/edubtnsview";
            }
        }
        public override string AssetName {
            get {
                return "EduBtnsView";
            }
        }
        public override string ViewName {
            get {
                return "EduBtnsView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(EduBtnsViewModel).FullName;
            }
        }
        public EduBtnsViewModel ViewModel {
            get {
                return (EduBtnsViewModel)BindingContext;
            }
        }

        Button swaButton; // swap current tetrominos to be a newly generating(it's coming after click) tetromino set
        Button undButton; // undo last selected tetromino landing, revert it back

        protected override void OnInitialize() {
            base.OnInitialize();

            swaButton = GameObject.FindChildByName("swaBtn").GetComponent<Button>();
            swaButton.onClick.AddListener(OnClickSwaButton);

            undButton = GameObject.FindChildByName("undBtn").GetComponent<Button>();
            undButton.onClick.AddListener(OnClickUndButton);

            Debug.Log("up to here ");
        }

        void OnClickSwaButton() {
            // 这里需要下发指令到视图数据层,并根据随机数生成的新的tetromino来重新刷新UI
        }

        void OnClickUndButton() {
            // 类似的逻辑下发数据,并由数据驱动刷新UI
        }
    }
}
