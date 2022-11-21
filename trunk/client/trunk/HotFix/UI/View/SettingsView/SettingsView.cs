using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    public class SettingsView : UnityGuiView {
        private const string TAG = "SettingsView";
        public override string BundleName { get { return "ui/view/settingsview"; } }
        public override string AssetName { get { return "SettingsView"; } }
        public override string ViewName { get { return "SettingsView"; } }
        public override string ViewModelTypeName { get { return typeof(SettingsViewModel).FullName; } }
        public SettingsViewModel ViewModel { get { return (SettingsViewModel)BindingContext; } }

        Button lgiBtn; // LOGIN
        Button creBtn; // CREDIT
        Button ratBtn; // RATE GAME
        Button sunBtn; // SOUND ON/OFF ADJUSTMENT
        Button adsBtn; // ads free
        Button lotBtn; // LOGOUT
        Button setBtn; // user account profiles settings or what ?

        GameObject sndPanel;
        Button sndOn;
        Button sndOff;
        Slider sndSdr;
        
        protected override void OnInitialize() {
            base.OnInitialize();

            lgiBtn = GameObject.FindChildByName("lgiBtn").GetComponent<Button>();
            lgiBtn.onClick.AddListener(OnClickLgiButton);

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

            sndPanel = GameObject.FindChildByName("soundPanel");
            sndOn = GameObject.FindChildByName("sndOn").GetComponent<Button>();
            sndOn.onClick.AddListener(onClickSoundOnButton);
            sndOff = GameObject.FindChildByName("sndOff").GetComponent<Button>();
            sndOff.onClick.AddListener(onClickSoundOffButton);
            sndSdr = GameObject.FindChildByName("volSdr").GetComponent<Slider>(); // 这个滑动条有一些相关的事件需要处理
        }

        void OnClickLgiButton() { // LOGIN
            ViewManager.LoginView.Reveal();
            ViewManager.MenuView.Hide();
            Hide();
        }
        void OnClickCreButton() {
        }
        void OnClickRatButton() {
        }
        void OnClickSunButton() {
            sndPanel.SetActive(true);
        }
        void OnClickAdsButton() {
        }
        void OnClickLotButton() { // LOGOUT
        }
        void OnClickSetButton() {
        }
        void onClickSoundOnButton() {
            sndPanel.SetActive(false);
        }
        void onClickSoundOffButton() {
            sndPanel.SetActive(false);
        }
    }
}
