using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;

namespace HotFix.UI {

    public class BgnNewContinueViewModel : ViewModelBase {

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
