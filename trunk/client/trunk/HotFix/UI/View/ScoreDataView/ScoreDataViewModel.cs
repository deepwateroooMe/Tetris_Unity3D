﻿using Framework.MVVM;

namespace HotFix.UI {

    public class ScoreDataViewModel : ViewModelBase {

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