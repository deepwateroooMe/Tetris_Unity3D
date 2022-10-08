using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI {

    public class BgnNewContinueView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/bgnnewcontinueview";
            }
        }
        public override string AssetName {
            get {
                return "BgnNewContinueView";
            }
        }
        public override string ViewName {
            get {
                return "BgnNewContinueView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(BgnNewContinueViewModel).FullName;
            }
        }
        public BgnNewContinueViewModel ViewModel {
            get {
                return (BgnNewContinueViewModel)BindingContext;
            }
        }

        Button newButton; 
        Button conButton; 

        protected override void OnInitialize() {
            base.OnInitialize();

            newButton = GameObject.FindChildByName("newBtn").GetComponent<Button>();
            newButton.onClick.AddListener(OnClickNewGameButton);

            conButton = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            conButton.onClick.AddListener(OnClickContinueButton);
        }

        void OnClickNewGameButton() {
         
            // ViewManager.FindView.Reveal();
        }
        void OnClickContinueButton() {
       
            // ViewManager.DesginView.Reveal();
        }
    }
}
