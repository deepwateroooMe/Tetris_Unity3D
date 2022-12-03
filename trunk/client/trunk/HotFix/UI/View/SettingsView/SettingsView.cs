using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Core;
using Framework.MVVM;
using HotFix.Control;
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
        // Slider sndSdr;
        ExtendedSlider sndSdr;

        int maxVol, curVol;
        
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
            sndSdr = GameObject.FindChildByName("volSdr").GetComponent<ExtendedSlider>(); // 这个滑动条有一些相关的事件需要处理
// 所以最简单的办法,实现一个安卓SDK,将此界面同样地(实际上是音量控制安卓原生相关功能模块移植到SDK中去,这里不再保留)定义在安卓中,调用安卓中的Activity来显示或调控音量.那么如果这样,就涉及到热更新域(或者大一点儿Unity,但热更新域比起普通游戏unity里会更难一点儿)与安卓SDK的相互调用与传值,需要一个真心强大的安卓SDK(或是说安卓SDK与热更新域的相互调用),活宝妹也狠强大,爱表哥,爱生活!!!
// // TODO:现在的问题就是:以不写SDK的形式加入,无法转化到安卓平台测试这个prototype;所以还是接个比较完整的安卓SDK会比较方便
            maxVol = VolumeManager.getMaxVolume();
            Debug.Log(TAG + " OnInitialize() maxVol: " + maxVol);
            curVol = VolumeManager.getCurrentVolume();
            Debug.Log(TAG + " OnInitialize() curVol: " + curVol);
            VolumeManager.setVolume(50);

// slider 拖拽事件的监听回调
            sndSdr.DragStart.AddListener(OnSliderBeginDrag);
            sndSdr.DragStop.AddListener(OnSliderEndDrag);
            sndSdr.PointerDown.AddListener(OnSliderClick);
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
        void OnSliderBeginDrag(float value)
            {
                Debug.Log("开始拖拽：" + value);
            }

        void OnSliderClick(float value)
            {
                Debug.Log("点击：" + value);
            }

        void OnSliderEndDrag(float value)
            {
                Debug.Log("结束拖拽：" + value);
            }
        // public ExtendedEvent DragStart;
        // public ExtendedEvent DragStop;
        // public ExtendedEvent PointerDown;
        // public void OnBeginDrag(PointerEventData eventData) {
        //     DragStart.Invoke(m_Value);
        // }
        // public void OnEndDrag(PointerEventData eventData) {
        //     DragStop.Invoke(m_Value);
        // }
        // public override void OnPointerDown(PointerEventData eventData) {
        //     base.OnPointerDown(eventData);
        //     PointerDown.Invoke(m_Value);
        // }
    }
}
