using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class ToggleBtnView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/togglebtnview";
            }
        }
        public override string AssetName {
            get {
                return "ToggleBtnView";
            }
        }
        public override string ViewName {
            get {
                return "ToggleBtnView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(ToggleBtnViewModel).FullName;
            }
        }
        public ToggleBtnViewModel ViewModel {
            get {
                return (ToggleBtnViewModel)BindingContext;
            }
        }

        // enum Type { // Buttons group type
        //     DIRS = 0,
        //     ROTATIONS = 1
        // }
        // Type type = 0;
        int type = 0;
        Button togButton; // Togcation

        protected override void OnInitialize() {
            base.OnInitialize();

            togButton = GameObject.FindChildByName("togBtn").GetComponent<Button>();
            togButton.onClick.AddListener(OnClickTogButton);

        }
        void OnClickTogButton() {
            // 当前的视图需要隐藏起来吗? 检查一下逻辑
            type ^= 1;
// 根据当前需要显示的按钮组的值的不同,来显示平移组或是旋转组
// 这两个组的按钮也需要分别制作成视图,两个不同的视图
            // if (type == 0)
            // else
            Hide();
        }
    }
}
