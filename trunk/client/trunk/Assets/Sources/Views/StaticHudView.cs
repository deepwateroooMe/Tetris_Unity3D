using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.ViewModels;
using uMVVM.Sources.Infrastructure;
using uMVVM.Sources.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sources.Views {

    public class StaticHudView : UnityGuiView<StaticHudViewModel> {

        // public Image buttonImage;
        // public GameObject text;
        
        public StaticHudViewModel ViewModelBase { get { return (StaticHudViewModel)BindingContext; } }

        protected override void OnInitialize() {
            base.OnInitialize();
            Binder.Add<string>("Color",OnColorPropertyValueChanged);
            // Binder.Add<string>("Educational", OnEducationalPropertyValueChanged);
        }

        // private void OnEducationalPropertyValueChanged(string oldValue, string newValue) {
        private void OnColorPropertyValueChanged(string oldValue, string newValue) {
            // switch (newValue) {
            //     // case "Educational":
            //     case "Red":
            //         buttonImage.color = Color.red;
            //         // text.SetActive(true);
            //         break;
            //     case "Yellow":
            //         buttonImage.color = Color.yellow;
            //         break;
            //     default:
            //         break;
            // }
        }
    }
}
