﻿using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class StaticBtnsView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/staticbtnsview";
            }
        }
        public override string AssetName {
            get {
                return "StaticBtnsView";
            }
        }
        public override string ViewName {
            get {
                return "StaticBtnsView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(StaticBtnsViewModel).FullName;
            }
        }
        public StaticBtnsViewModel ViewModel {
            get {
                return (StaticBtnsViewModel)BindingContext;
            }
        }

        Button pauButton; // pause Game
        Button falButton; // fall fast button

        protected override void OnInitialize() {
            base.OnInitialize();

            pauButton = GameObject.FindChildByName("pauBtn").GetComponent<Button>();
            pauButton.onClick.AddListener(OnClickPauButton);

            falButton = GameObject.FindChildByName("falBtn").GetComponent<Button>();
            falButton.onClick.AddListener(OnClickFalButton);
        }

        void OnClickPauButton() {
            ViewManager.MidMenuView.Reveal();
        }

        void OnClickFalButton() {
        }
    }
}
