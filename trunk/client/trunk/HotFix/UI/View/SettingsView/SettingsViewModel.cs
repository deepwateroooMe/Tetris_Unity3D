using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;

namespace HotFix.UI.View.SettingsView
{
    public class SettingsViewModel : ViewModelBase {

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
