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

// 这里游戏的视图界面被定义成了很多个小的视图,所以把其它小视图都移除是不对的,会移走游戏界面需要用到的控件        
        // public override void OnAppear() {
        //     base.OnAppear();
        //     CloseOtherRootView = CloseOtherRootViews;
        // }
        // void CloseOtherRootViews() {
        //     ViewManager.CloseOtherRootViews(ViewName);
        // }

// 当进行了这一次的热更新重构,这一次是否可以:
// 从最底层来优化游戏设计与性能:
        // 先前接触到一个ipad上比较好的方块砖游戏是:下左右前后五个面均投射出方块砖在其平面上应有的投影
        // 是一个要求更高手的实现,却有点儿跃跃欲试,看能否NDK或至少渲染上能够实现?
        // 能否从NDK层来优化游戏主场景方格的渲染,是通过渲染来实现,还是说要把游戏主逻辑封到NDK层呢?

        // Button eduButton; // Education
        // Button claButton; // Classic
        // Button chaButton; // Challenge

        // protected override void OnInitialize() {
        //     base.OnInitialize();

        //     eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
        //     eduButton.onClick.AddListener(OnClickEduButton);

        //     claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
        //     claButton.onClick.AddListener(OnClickClaButton);

        //     chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
        //     chaButton.onClick.AddListener(OnClickChaButton);

        //     // SetDownRootIndex = ChageViewsIndex;
        // }
        // // void ChageViewsIndex() {
        // //     //UnityEngine.Transform.SetAsLastSibling();
        // // }

        // void OnClickEduButton() {
        //     ViewManager.EducaModesView.Reveal();
        //     // 当前的视图需要隐藏起来吗? 检查一下逻辑
        //     Hide();
        // }
        // void OnClickClaButton() {
         
        //     // ViewManager.FindView.Reveal();
        //     Hide();
        // }
        // void OnClickChaButton() {
       
        //     // ViewManager.DesginView.Reveal();
        //     Hide();
        // }
    }
}
