using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Framework.MVVM;

public class ViewModelBaseAdapter : CrossBindingAdaptor {

    // 实现基类的三个方法
    public override Type BaseCLRType {
        get {
            return typeof(ViewModelBase);
        }
    }
    public override Type AdaptorType {
        get {
            return typeof(ViewModelBaseAdaptor);
        }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new ViewModelBaseAdaptor(appdomain, instance);
    }

    class ViewModelBaseAdaptor : ViewModelBase, CrossBindingAdaptorType {

        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ViewModelBaseAdaptor() {}
        public ViewModelBaseAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this.instance = instance;
        }
        public ILTypeInstance ILInstance { get { return instance; } } // 对公用接口类公用接口方法的实现

        // ViewModelBase 六个虚拟方法的实现
        public override void OnStartReveal() {
            if (!_onStartRevealGot) {
                _onStartReveal = instance.Type.GetMethod("OnStartReveal");
                _onStartRevealGot = true;
            }
            // 始终是存在跨域调用和反射两种情况,所以也要始终区分这两种情况;其它方法类同
            if (_onStartReveal != null && !_isOnStartRevealInvoking) {
                _isOnStartRevealInvoking = true;
                appdomain.Invoke(_onStartReveal, instance);
                _isOnStartRevealInvoking = false;
            } else 
                base.OnStartReveal();
        }
        public override void OnFinishReveal() {
            if (!_onFinishRevealGot) {
                _onFinishReveal = instance.Type.GetMethod("OnFinishReveal");
                _onFinishRevealGot = true;
            }
            if (_onFinishReveal != null && !_isOnFinishRevealInvoking) {
                _isOnFinishRevealInvoking = true;
                appdomain.Invoke(_onFinishReveal, instance);
                _isOnFinishRevealInvoking = false;
            } else 
                base.OnFinishReveal();
        }
        public override void OnStartHide() {
            if (!_onStartHideGot) {
                _onStartHide = instance.Type.GetMethod("OnStartHide");
                _onStartHideGot = true;
            }
            if (_onStartHide != null && !_isOnStartHideInvoking) {
                _isOnStartHideInvoking = true;
                appdomain.Invoke(_onStartHide, instance);
                _isOnStartHideInvoking = false;
            } else 
                base.OnStartHide();
        }
        public override void OnFinishHide() {
            if (!_onFinishHideGot) {
                _onFinishHide = instance.Type.GetMethod("OnFinishHide");
                _onFinishHideGot = true;
            }
            if (_onFinishHide != null && !_isOnFinishHideInvoking) {
                _isOnFinishHideInvoking = true;
                appdomain.Invoke(_onFinishHide, instance);
                _isOnFinishHideInvoking = false;
            } else 
                base.OnFinishHide();
        }
        public override void OnDestory() {
            if (!_onDestoryGot) {
                _onDestory = instance.Type.GetMethod("OnDestory");
                _onDestoryGot = true;
            }
            if (_onDestory != null && !_isOnDestoryInvoking) {
                _isOnDestoryInvoking = true;
                appdomain.Invoke(_onDestory, instance);
                _isOnDestoryInvoking = false;
            } else {
                base.OnDestory();
            }
        }
        
        protected override void OnInitialize() {
            if (!_onInitializeGot) {
                _onInitialize = instance.Type.GetMethod("OnInitialize");
                _onInitializeGot = true;
            }
            if (_onInitialize != null && !_isOnInitializeInvoking) {
                _isOnInitializeInvoking = true;
                appdomain.Invoke(_onInitialize, instance);
                _isOnInitializeInvoking = false;
            } else 
                base.OnInitialize();
        }       
        IMethod _onStartReveal;
        bool _onStartRevealGot;
        bool _isOnStartRevealInvoking = false;

        IMethod _onFinishReveal;
        bool _onFinishRevealGot;
        bool _isOnFinishRevealInvoking = false;

        IMethod _onStartHide;
        bool _onStartHideGot;
        bool _isOnStartHideInvoking = false;

        IMethod _onFinishHide;
        bool _onFinishHideGot;
        bool _isOnFinishHideInvoking = false;

        IMethod _onDestory;
        bool _onDestoryGot;
        bool _isOnDestoryInvoking = false;

        IMethod _onInitialize;
        bool _onInitializeGot;
        bool _isOnInitializeInvoking = false;
    }
}
