using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.ViewModels;
using Assets.Sources.Views;
using uMVVM.Sources.ViewModels;
using uMVVM.Sources.Views;
using UnityEngine;

namespace uMVVM.Sources {
    
    public class Install : MonoBehaviour { // game starts from here

        public SetupView setupView;
        public StaticHudView staticHudView;

        void Start() {
            //绑定上下文
            setupView.BindingContext = new SetupViewModel();
            staticHudView.BindingContext = new StaticHudViewModel();

            //以动画模式缓慢显示
            setupView.Reveal(false, () => {
                Debug.Log("测试");
            });

            //立刻显示
            staticHudView.Reveal(true);
        }
    }
}
