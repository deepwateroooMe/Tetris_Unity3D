﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.UI {

    public class ChallLevelsViewModel : ViewModelBase {
        private const string TAG = "ChallLevelsViewModel"; 

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