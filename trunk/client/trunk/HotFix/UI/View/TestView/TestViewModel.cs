using Framework.MVVM;

namespace HotFix.UI.View.TestView {

    public class TestViewModel : ViewModelBase {

    }
    // public class TestViewModel : ViewModelBase { // 把这些个不同类的继承关系串起来，怎么串起来呢？ Guide
    //     protected override void OnInitialize() {
    //         base.OnInitialize(); // <<<<<<<<<<<<<<<<<<<< 这里还是有点儿不懂
    //         Initialization();
    //         DelegateSubscribe();
    //     }
    //     void Initialization() {}
    //     void DelegateSubscribe() {}
    // }
}