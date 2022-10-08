using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI.View.SettingsView
{
    public class SettingsView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/settingsview";
            }
        }
        public override string AssetName {
            get {
                return "SettingsView";
            }
        }
        public override string ViewName {
            get {
                return "SettingsView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(SettingsViewModel).FullName;
            }
        }
        public SettingsViewModel ViewModel {
            get {
                return (SettingsViewModel)BindingContext;
            }
        }

        Button creBtn; // CREDIT
        Button ratBtn; // RATE GAME
        Button sunBtn; // SUNUME GAME
        Button adsBtn; // SUNUME GAME
        Button lotBtn; // TUTORIAL
        Button setBtn; // BACK TO MAIN MENU

        protected override void OnInitialize() {
            base.OnInitialize();

            creBtn = GameObject.FindChildByName("creBtn").GetComponent<Button>();
            creBtn.onClick.AddListener(OnClickCreButton);

            ratBtn = GameObject.FindChildByName("ratBtn").GetComponent<Button>();
            ratBtn.onClick.AddListener(OnClickRatButton);

            sunBtn = GameObject.FindChildByName("sunBtn").GetComponent<Button>();
            sunBtn.onClick.AddListener(OnClickSunButton);

            adsBtn = GameObject.FindChildByName("adsBtn").GetComponent<Button>();
            adsBtn.onClick.AddListener(OnClickAdsButton);

            lotBtn = GameObject.FindChildByName("lotBtn").GetComponent<Button>();
            lotBtn.onClick.AddListener(OnClickLotButton);

            setBtn = GameObject.FindChildByName("setBtn").GetComponent<Button>();
            setBtn.onClick.AddListener(OnClickSetButton);

        }

        void OnClickCreButton() {
        }
        void OnClickRatButton() {
        }
        void OnClickSunButton() {
        }
        void OnClickAdsButton() {
        }
        void OnClickLotButton() {
            // ViewSetager.DesginView.Reveal();
        }
        void OnClickSetButton() {
        }
    }
}
