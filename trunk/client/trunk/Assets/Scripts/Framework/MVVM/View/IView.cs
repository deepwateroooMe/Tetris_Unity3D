using System;
namespace Framework.MVVM {

    // 传统MVVM的框架中V View与VM ViewModel是如何实现自动绑定的呢?自已多年前使用StrangeIoC写小游戏时,也是有适配器之类的
    // 这里这个接口不是泛型数据接口,是定型的ViewModelBase的视图接口类, 我觉得这里是已经实现了视图与视图模型的自动绑定;
    // 然后再通过耍杂技般的各种不同继承来达到热更新模块适配给unity模块的逻辑覆盖
    
    public interface IView<ViewModelBase> {
        ViewModelBase BindingContext {
            get;
            set;
        }
        void Reveal(bool immediate = false, Action action = null);
        void Hide(bool immediate = false, Action action = null);
    }
}
