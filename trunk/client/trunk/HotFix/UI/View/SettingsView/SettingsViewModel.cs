using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;

namespace HotFix.UI {
    public class SettingsViewModel : ViewModelBase {

        private int mCurVol;
        private int mMaxVol;
        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }
        void Initialization() {
            mCurVol = 0;
            mMaxVol = 100;
        }
        void DelegateSubscribe() {
        }
    }
}
