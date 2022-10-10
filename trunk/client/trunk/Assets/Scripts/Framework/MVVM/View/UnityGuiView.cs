using System;
using UnityEngine;
using Framework.ResMgr;

namespace Framework.MVVM {

// unity程序集中的抽象基类,定义必要的逻辑(提取出子类可能都会用的方法逻辑这里处理)和需要继承者实现的各类方法
    public abstract class UnityGuiView : IView<ViewModelBase> {

// 热更新资源相关的一些getter/setters
        public virtual string BundleName {
            get {
                return string.Empty;
            }
        }
        public virtual string AssetName {
            get {
                return string.Empty;
            }
        }
        public virtual string ViewName {
            get {
                return string.Empty;
            }
        }
        public virtual string ViewModelTypeName {
            get {
                return string.Empty;
            }
        }

// 这里,热更新里接下来会继承续写的视图: 实则被定义为UI上的一个一个的控件,所以可以获取控件的GameObject,而并非一个一个的场景
// 并且现框架的热更新只作同一场景下的视图热更新,好像并不曾汲及同一个应用下不同的游戏场景的切换,这块可以再检查扩展一下,如果自己的游戏需要的话        
        public GameObject GameObject {
            get;
            set;
        }
        private Transform _transform;
        public Transform Transform {
            get {
                if (_transform == null) 
                    _transform = GameObject.transform;
                return _transform;
            }
        }
        private CanvasGroup _canvasGroup;
        public CanvasGroup CanvasGroup {
            get {
                if (_canvasGroup == null) 
                    _canvasGroup = GameObject.GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
        private bool _isInitialized;
        public virtual bool DestoryOnHide {
            get {
                return false;
            }
        }
        public virtual bool IsRoot {
            get {
                return false;
            }
        }
        public static Action SetDownRootIndex;
        public Action CloseOtherRootView;
        protected readonly PropertyBinder<ViewModelBase> binder = new PropertyBinder<ViewModelBase>();
        public readonly BindableProperty<ViewModelBase> viewModelProperty = new BindableProperty<ViewModelBase>();
        public Action RevealedAction {
            get;
            set;
        }
        public Action HiddenAction {
            get;
            set;
        }
        public ViewModelBase BindingContext {
            get {
                return viewModelProperty.Value;
            }
            set {
                if (!_isInitialized) {
                    OnInitialize();
                    _isInitialized = true;
                }
                viewModelProperty.Value = value;
            }
        }
        protected virtual void OnInitialize() {
            GameObject = ResourceConstant.Loader.LoadClone(BundleName, AssetName, EAssetBundleUnloadLevel.Never);
            GameObject.AddComponent<CanvasGroup>();
            Transform.SetParent(GameObject.Find("ViewRoot").transform, false);
            viewModelProperty.OnValueChanged += OnBindingContextChanged;
        }
        public void Reveal(bool immediate = true, Action action = null) {
            if (action != null) 
                RevealedAction += action;
            OnAppear();
            OnReveal(immediate);
            OnRevealed();
        }
        public void Hide(bool immediate = true, Action action = null) {
            if (action != null) 
                HiddenAction += action;
            OnHide(immediate);
            OnHidden();
            OnDisappear();
        }
        public virtual void OnAppear() {
            GameObject.SetActive(true);
        }
        private void OnReveal(bool immediate) {
            BindingContext.OnStartReveal();
            if (immediate) {
                Transform.localScale = Vector3.one;
                CanvasGroup.alpha = 1;
            } else 
                StartAnimatedReveal();
        }
        public virtual void OnRevealed() {
            BindingContext.OnFinishReveal();
            if (RevealedAction != null) 
                RevealedAction();
            if (IsRoot) // 这种情况下,会框架会要求自动关闭其它所有视图
                if (CloseOtherRootView != null) 
                    CloseOtherRootView();
            if (SetDownRootIndex != null) 
                SetDownRootIndex();
        }
        private void OnHide(bool immediate) {
            BindingContext.OnStartHide();
            if (immediate) {
                Transform.localScale = Vector3.zero;
                CanvasGroup.alpha = 0;
            } else 
                StartAnimatedHide();
        }
        public virtual void OnHidden() {
            if (HiddenAction != null) 
                HiddenAction();
        }
        public virtual void OnDisappear() {
            GameObject.SetActive(false);
            BindingContext.OnFinishHide();
            if (DestoryOnHide) 
                UnityEngine.Object.Destroy(GameObject);
        }
        public virtual void OnDestory() {
            if (BindingContext.IsRevealed) 
                Hide(true);
            BindingContext.OnDestory();
            BindingContext = null;
            viewModelProperty.OnValueChanged = null;
        }
        protected virtual void StartAnimatedReveal() {
            CanvasGroup.interactable = false;
            Transform.localScale = Vector3.one;
        }
        protected virtual void StartAnimatedHide() {
            CanvasGroup.interactable = false;
        }
        protected virtual void OnBindingContextChanged(ViewModelBase oldValue, ViewModelBase newValue) {
            binder.UnBind(oldValue);
            binder.Bind(newValue);
        }
    }
}

