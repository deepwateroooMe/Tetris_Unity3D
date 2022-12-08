using deepwaterooo.tetris3d;
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
        Slider sndSdr;
        // ExtendedSlider sndSdr;
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
            // sndSdr = GameObject.FindChildByName("volSdr").GetComponent<ExtendedSlider>(); // 这个滑动条有一些相关的事件需要处理
            sndSdr = GameObject.FindChildByName("volSdr").GetComponent<Slider>(); // 这个滑动条有一些相关的事件需要处理
            
            // maxVol = VolumeManager.getMaxVolume();
            // Debug.Log(TAG + " OnInitialize() maxVol: " + maxVol);
            // curVol = VolumeManager.getCurrentVolume();
            // Debug.Log(TAG + " OnInitialize() curVol: " + curVol);
            // VolumeManager.setVolume(50);
// 这里现想的两种方法: 
            // 主界面与安卓SDK全放入热更新域中:不知道是否会有我没有想到的障碍,需要测试一下,现先试着测这第二种方法
                // 先不用包装,直接 call MainActivity 里的方法: 直接全部放在热更新域里,是调不通的
            // 游戏主界面与安卓SDK主界面各占一部分: 问题是,游戏过程中会有多个想要重入主界面的调用,两个界面交互共同显示的主界面除了第一次显示之外其它任何时候调用感觉都不太方便
                // 重入的时候就是调用主界面,再测一下这个
            
            // maxVol = Deepwaterooo.Instance.getMaxVolume();
            // Debug.Log(TAG + " OnInitialize() maxVol: " + maxVol);
            // curVol = Deepwaterooo.Instance.getCurrentVolume();
            // Debug.Log(TAG + " OnInitialize() curVol: " + curVol);
            // Deepwaterooo.Instance.setVolume(50);
// // slider 拖拽事件的监听回调
//             sndSdr.DragStart.AddListener(OnSliderBeginDrag);
//             sndSdr.DragStop.AddListener(OnSliderEndDrag);
//             sndSdr.PointerDown.AddListener(OnSliderClick);
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
            Debug.Log(TAG + " OnClickSunButton: Sound()");
            sndPanel.SetActive(true);
        }
        void OnClickAdsButton() {
        }
        void OnClickLotButton() { // LOGOUT
        }
        void OnClickSetButton() {
        }
        void onClickSoundOnButton() {
            Debug.Log(TAG + " onClickSoundOnButton()");
            VolumeManager.Instance.setVolume(-1);
            sndPanel.SetActive(false);
        }
        void onClickSoundOffButton() {
            Debug.Log(TAG + " onClickSoundOffButton()");
            VolumeManager.Instance.setVolume(0);
            sndPanel.SetActive(false);
        }
        // void OnSliderBeginDrag(float value) {
        //     Debug.Log("开始拖拽：" + value);
        // }
        // void OnSliderClick(float value) {
        //     Debug.Log("点击：" + value);
        // }
        // void OnSliderEndDrag(float value) {
        //     Debug.Log("结束拖拽：" + value);
        // }
    }
}
