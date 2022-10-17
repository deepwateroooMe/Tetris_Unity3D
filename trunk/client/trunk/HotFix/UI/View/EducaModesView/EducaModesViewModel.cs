using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;

namespace HotFix.UI {

    public class EducaModesViewModel : ViewModelBase {

        // public BindableProperty<int> GridWidth = new BindableProperty<int>(); // 这里暂时用不上这个
        private int gridWidth = -1;
        public int GridWidth {
            set {
                gridWidth = value;
            }
            get {
                return gridWidth;
            }
        }
        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        void Initialization() {
        }

        void DelegateSubscribe() {
        }
    }
}
