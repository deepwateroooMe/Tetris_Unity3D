﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uMVVM.Sources.Infrastructure {

    // 统一的视图接口
    public interface IView<T> where T : ViewModelBase {

        T BindingContext { get; set; }

        void Reveal(bool immediate = false, Action action = null);
        void Hide(bool immediate = false, Action action = null);
    }
}
