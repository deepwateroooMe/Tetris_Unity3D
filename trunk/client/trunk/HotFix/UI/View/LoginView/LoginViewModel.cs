using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.UI {
	public class LoginViewModel : ViewModelBase {

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
