using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Framework.MVVM;

public class UnityGuiViewAdapter : CrossBindingAdaptor {

// 实现基类的三个方法    
    public override Type BaseCLRType {
        get {
            return typeof(UnityGuiView);
        }
    }
    public override Type AdaptorType {
        get {
            return typeof(UnityGuiViewAdaptor);
        }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new UnityGuiViewAdaptor(appdomain, instance);
    }

    class UnityGuiViewAdaptor : UnityGuiView, CrossBindingAdaptorType {
        ILTypeInstance instance; // <<<<<<<<<< 接口类里的实例,公用接口
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public UnityGuiViewAdaptor() {}
        public UnityGuiViewAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } } // <<<<<<<<<< 
        protected override void OnInitialize() {
            if (!_onInitializeGot) {
                _onInitialize = instance.Type.GetMethod("OnInitialize");
                _onInitializeGot = true;
            }
            if (_onInitialize != null && !isOnInitializeInvoking) {
                isOnInitializeInvoking = true;
                appdomain.Invoke(_onInitialize, instance);
                isOnInitializeInvoking = false;
            } else {
                base.OnInitialize();
            }
        }
        public override void OnAppear() {
            if (!_onAppearGot) {
                _onAppear = instance.Type.GetMethod("OnAppear");
                _onAppearGot = true;
            }
            if (_onAppear != null && !isOnAppearInvoking) {
                isOnAppearInvoking = true;
                appdomain.Invoke(_onAppear, instance);
                isOnAppearInvoking = false;
            } else {
                base.OnAppear();
            }
        }
        public override void OnRevealed() {
            if (!_onRevealedGot) {
                _onRevealed = instance.Type.GetMethod("OnRevealed");
                _onRevealedGot = true;
            }
            if (_onRevealed != null && !isOnRevealedInvoking) {
                isOnRevealedInvoking = true;
                appdomain.Invoke(_onRevealed, instance);
                isOnRevealedInvoking = false;
            } else {
                base.OnRevealed();
            }
        }
        public override void OnHidden() {
            if (!_onHiddenGot) {
                _onHidden = instance.Type.GetMethod("OnHidden");
                _onHiddenGot = true;
            }
            if (_onHidden != null && !isOnHiddenInvoking) {
                isOnHiddenInvoking = true;
                appdomain.Invoke(_onHidden, instance);
                isOnHiddenInvoking = false;
            } else {
                base.OnHidden();
            }
        }
        public override void OnDisappear() {
            if (!_onDisappearGot) {
                _onDisappear = instance.Type.GetMethod("OnDisappear");
                _onDisappearGot = true;
            }
            if (_onDisappear != null && !isOnDisappearInvoking) {
                isOnDisappearInvoking = true;
                appdomain.Invoke(_onDisappear, instance);
                isOnDisappearInvoking = false;
            } else {
                base.OnDisappear();
            }
        }
        public override void OnDestory() {
            if (!_onDestoryGot) {
                _onDestory = instance.Type.GetMethod("OnDestory");
                _onDestoryGot = true;
            }
            if (_onDestory != null && !isOnDestoryInvoking) {
                isOnDestoryInvoking = true;
                appdomain.Invoke(_onDestory, instance);
                isOnDestoryInvoking = false;
            } else {
                base.OnDestory();
            }
        }
        protected override void StartAnimatedReveal() {
            if (!_startAnimatedRevealGot) {
                _startAnimatedReveal = instance.Type.GetMethod("StartAnimatedReveal");
                _startAnimatedRevealGot = true;
            }
            if (_startAnimatedReveal != null && !isStartAnimatedRevealInvoking) {
                isStartAnimatedRevealInvoking = true;
                appdomain.Invoke(_startAnimatedReveal, instance);
                isStartAnimatedRevealInvoking = false;
            } else {
                base.StartAnimatedReveal();
            }
        }
        protected override void StartAnimatedHide() {
            if (!_startAnimatedHideGot) {
                _startAnimatedHide = instance.Type.GetMethod("StartAnimatedHide");
                _startAnimatedHideGot = true;
            }
            if (_startAnimatedHide != null && !isStartAnimatedHideInvoking) {
                isStartAnimatedHideInvoking = true;
                appdomain.Invoke(_startAnimatedHide, instance);
                isStartAnimatedHideInvoking = false;
            } else {
                base.StartAnimatedHide();
            }
        }

// 这里跨域继承的本质是说:借助了一个公用接口,使得两个不同的域之间架起了一座可以调用(热更新调用unity里的资源方法回调等)的桥;
// 在每个选择性地进行了跨域适配的适配器里,都存有一个(上下文传下来的unity域的引用instance,)那么就可以借用这个来调用热更新程序域外的方法等
// 这个框架里的CRL跨域适配是选择性,只在必要的层面和必要的视图逻辑上使用跨域继承
        object[] param2 = new object[2];
        protected override void OnBindingContextChanged(ViewModelBase oldValue, ViewModelBase newValue) {
            if (!_onBindingContextChangedGot) {
                _onBindingContextChanged = instance.Type.GetMethod("OnBindingContextChanged");
                _onBindingContextChangedGot = true;
            }
            if (_onBindingContextChanged != null && !isOnBindingContextChangedInvoking) {
                isOnBindingContextChangedInvoking = true;
                appdomain.Invoke(_onBindingContextChanged, instance, param2); // <<<<<<<<<<<<<<<<<<<< 
                isOnBindingContextChangedInvoking = false;
            } else 
                base.OnBindingContextChanged(oldValue, newValue);
        }
        public override string BundleName {
            get {
                if (!_getBundleNameGot) {
                    _getBundleName = instance.Type.GetMethod("get_BundleName", 0);
                    _getBundleNameGot = true;
                }
                if (_getBundleName != null && !isGetBundleNameInvoking) {
                    isGetBundleNameInvoking = true;
                    var res = (string)appdomain.Invoke(_getBundleName, instance, null);
                    isGetBundleNameInvoking = false;
                    return res;
                } else 
                    return base.BundleName;
            }
        }
        public override string AssetName {
            get {
                if (!_getAssetNameGot) {
                    _getAssetName = instance.Type.GetMethod("get_AssetName", 0);
                    _getAssetNameGot = true;
                }
                if (_getAssetName != null && !isGetAssetNameInvoking) {
                    isGetAssetNameInvoking = true;
                    var res = (string)appdomain.Invoke(_getAssetName, instance, null);
                    isGetAssetNameInvoking = false;
                    return res;
                } else 
                    return base.AssetName;
            }
        }
        public override string ViewName {
            get {
                if (!_getViewNameGot) {
                    _getViewName = instance.Type.GetMethod("get_ViewName", 0);
                    _getViewNameGot = true;
                }
                if (_getViewName != null && !isGetViewNameInvoking) {
                    isGetViewNameInvoking = true;
                    var res = (string)appdomain.Invoke(_getViewName, instance, null);
                    isGetViewNameInvoking = false;
                    return res;
                } else 
                    return base.ViewName;
            }
        }
        public override string ViewModelTypeName {
            get {
                if (!_getViewModelTypeNameGot) {
                    _getViewModelTypeName = instance.Type.GetMethod("get_ViewModelTypeName", 0);
                    _getViewModelTypeNameGot = true;
                }
                if (_getViewModelTypeName != null && !isGetViewModelTypeNameInvoking) {
                    isGetViewModelTypeNameInvoking = true;
                    var res = (string)appdomain.Invoke(_getViewModelTypeName, instance, null);
                    isGetViewModelTypeNameInvoking = false;
                    return res;
                } else 
                    return base.ViewModelTypeName;
            }
        }
        public override bool DestoryOnHide {
            get {
                if (!_getDestoryOnHideGot) {
                    _getDestoryOnHide = instance.Type.GetMethod("get_DestoryOnHide", 0);
                    _getDestoryOnHideGot = true;
                }
                if (_getDestoryOnHide != null && !isGetDestoryOnHideInvoking) {
                    isGetDestoryOnHideInvoking = true;
                    var res = (bool)appdomain.Invoke(_getDestoryOnHide, instance, null);
                    isGetDestoryOnHideInvoking = false;
                    return res;
                } else {
                    return base.DestoryOnHide;
                }
            }
        }
        public override bool IsRoot {
            get {
                if (!_getIsRootGot) {
                    _getIsRoot = instance.Type.GetMethod("get_IsRoot", 0);
                    _getIsRootGot = true;
                }
                if (_getIsRoot != null && !isGetIsRootInvoking) {
                    isGetIsRootInvoking = true;
                    var res = (bool)appdomain.Invoke(_getIsRoot, instance, null);
                    isGetIsRootInvoking = false;
                    return res;
                } else 
                    return base.IsRoot;
            }
        }
        IMethod _onInitialize;
        bool _onInitializeGot;
        bool isOnInitializeInvoking = false;

        IMethod _onAppear;
        bool _onAppearGot;
        bool isOnAppearInvoking = false;

        IMethod _onRevealed;
        bool _onRevealedGot;
        bool isOnRevealedInvoking = false;

        IMethod _onHidden;
        bool _onHiddenGot;
        bool isOnHiddenInvoking = false;

        IMethod _onDisappear;
        bool _onDisappearGot;
        bool isOnDisappearInvoking = false;

        IMethod _onDestory;
        bool _onDestoryGot;
        bool isOnDestoryInvoking = false;

        IMethod _startAnimatedReveal;
        bool _startAnimatedRevealGot;
        bool isStartAnimatedRevealInvoking = false;

        IMethod _startAnimatedHide;
        bool _startAnimatedHideGot;
        bool isStartAnimatedHideInvoking = false;

        IMethod _getBundleName;
        bool _getBundleNameGot;
        bool isGetBundleNameInvoking = false;

        IMethod _getAssetName;
        bool _getAssetNameGot;
        bool isGetAssetNameInvoking = false;

        IMethod _getViewName;
        bool _getViewNameGot;
        bool isGetViewNameInvoking = false;

        IMethod _getDestoryOnHide;
        bool _getDestoryOnHideGot;
        bool isGetDestoryOnHideInvoking = false;

        IMethod _getIsRoot;
        bool _getIsRootGot;
        bool isGetIsRootInvoking = false;

        IMethod _getViewModelTypeName;
        bool _getViewModelTypeNameGot;
        bool isGetViewModelTypeNameInvoking = false;

        IMethod _onBindingContextChanged;
        bool _onBindingContextChangedGot;
        bool isOnBindingContextChangedInvoking = false;
    }
}
