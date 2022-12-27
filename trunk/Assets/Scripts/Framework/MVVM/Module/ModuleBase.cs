namespace Framework.MVVM {

    // 不是Model,是Module,是模块级别的CrossBindingAdapter适配的抽象基类
    public abstract class ModuleBase {

        public abstract void OnInitialize();
        public abstract void Excute();
    }
}
