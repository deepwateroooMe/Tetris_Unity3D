using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Core.Message;
using Assets.Sources.Infrastructure;
using uMVVM.Sources.Infrastructure;
using UnityEngine;

namespace Assets.Sources.ViewModels {


    public class StaticHudViewModel : ViewModelBase {

        public readonly BindableProperty<string> Color = new BindableProperty<string>();

        public StaticHudViewModel() {
            MessageAggregator<object>.Instance.Subscribe("Toggle",ToggleHandler);
        }

        private void ToggleHandler(object sender, MessageArgs<object> args) {
            // Educational.Value = (string) args.Item;
            Color.Value = (string) args.Item;
        }
    }
}
