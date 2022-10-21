using UnityEngine;

namespace Framework.MVVM {

    public class ViewModelBase {

        private bool _isInitialize;

        public bool IsRevealInProgress {
            get;
            private set;
        }
        public bool IsRevealed {
            get;
            private set;
        }
        public bool IsHideInProgress {
            get;
            private set;
        }

        // 这个还是狠好用的,可以得到父级视图模型的数据
        public ViewModelBase ParentViewModel {
            get;
            set;
        }

        public virtual void OnStartReveal() {
            IsRevealInProgress = true;
            if (!_isInitialize) {
                OnInitialize();
                _isInitialize = true;
            }
        }
        public virtual void OnFinishReveal() {
            IsRevealInProgress = false;
            IsRevealed = true;
        }
        public virtual void OnStartHide() {
            IsHideInProgress = true;
        }
        public virtual void OnFinishHide() {
            IsHideInProgress = false;
            IsRevealed = false;
        }

        // 两个虚拟方法,等待子类去实现
        public virtual void OnDestory() { }
        protected virtual void OnInitialize() {
            Debug.Log("OnInitialize ViewModelBase virtual");
        }
    }
}