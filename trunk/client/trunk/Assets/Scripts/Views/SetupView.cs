using uMVVM.Sources.Infrastructure;
using uMVVM.Sources.Models;
using uMVVM.Sources.ViewModels;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace uMVVM.Sources.Views {

    public class SetupView : UnityGuiView<SetupViewModel> {
        private const string TAG = "SetupView";
        
        public Button educationalButton;
        public Button classicButton;
        public Button challengeButton;

        public SetupViewModel ViewModel {
            get {
                return (SetupViewModel)BindingContext;
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();
        }

        public override void OnBindingContextChanged(SetupViewModel oldVal, SetupViewModel newVal) {
            base.OnBindingContextChanged(oldVal, newVal);
            educationalButton.onClick.AddListener(EducationalMode);
        }

        public void EducationalMode() {
            Debug.Log(TAG + ": EducationalMode()");
            // ViewModel.onEducationalMode();

            // this.BindingContext = new StaticHudViewModel();
        }
    }
}
