using deepwaterooo.tetris3d;
using DeepwateroooWang;
using UnityEngine;
using UnityEngine.UI;

// 普通游戏端视图: 不可热更新,主要涉及与安卓SDK的交互
public class SettingsCallback : MonoBehaviour {
    private const string TAG = "SettingsCallback";

    // public string BundleName { get { return "ui/view/settingsview"; } }
    // public string AssetName { get { return "SettingsView"; } }
    // public string ViewName { get { return "SettingsView"; } }
    // public string ViewModelTypeName { get { return typeof(SettingsViewModel).FullName; } }
    // public SettingsViewModel ViewModel { get { return (SettingsViewModel)BindingContext; } }

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

    public void Start() {
        lgiBtn = gameObject.FindChildByName("lgiBtn").GetComponent<Button>();
        lgiBtn.onClick.AddListener(OnClickLgiButton);
        creBtn = gameObject.FindChildByName("creBtn").GetComponent<Button>();
        creBtn.onClick.AddListener(OnClickCreButton);
        ratBtn = gameObject.FindChildByName("ratBtn").GetComponent<Button>();
        ratBtn.onClick.AddListener(OnClickRatButton);
        sunBtn = gameObject.FindChildByName("sunBtn").GetComponent<Button>();
        sunBtn.onClick.AddListener(OnClickSunButton);
        adsBtn = gameObject.FindChildByName("adsBtn").GetComponent<Button>();
        
        adsBtn.onClick.AddListener(OnClickAdsButton);
        lotBtn = gameObject.FindChildByName("lotBtn").GetComponent<Button>();
        lotBtn.onClick.AddListener(OnClickLotButton);
        setBtn = gameObject.FindChildByName("setBtn").GetComponent<Button>(); // <<<<<<<<<< 
        setBtn.onClick.AddListener(OnClickSetButton);
        sndPanel = gameObject.FindChildByName("soundPanel");
        sndOn = gameObject.FindChildByName("sndOn").GetComponent<Button>();
        sndOn.onClick.AddListener(onClickSoundOnButton);
        sndOff = gameObject.FindChildByName("sndOff").GetComponent<Button>();
        sndOff.onClick.AddListener(onClickSoundOffButton);
        sndSdr = gameObject.FindChildByName("volSdr").GetComponent<ExtendedSlider>(); // 这个滑动条有一些相关的事件需要处理
        // sndSdr = gameObject.FindChildByName("volSdr").GetComponent<Slider>(); // 这个滑动条有一些相关的事件需要处理
    }

// 测试调用安卓SDK: 显示splash Screen. 所以游戏端,最好是调用游戏的底端接口,而非直接与安卓SDK交互
    void OnClickLgiButton() { // LOGIN
        Debug.Log(TAG + " OnClickLoginButton()");
// // 这里想调一个SDK中的方法,测试一下: 这个调用太弱了，怎么也得调个别的
//         Deepwaterooo.instance.DisplaySplash(); // 调用游戏的相对底层所提供给游戏的公用APIs
// 这里试着调用 ManagePlayerActivity画面,只作测试用,到时修会调用 登录界面
        Deepwaterooo.instance.OpenManagePlayers();
        // ViewManager.LoginView.Reveal();
        // ViewManager.MenuView.Hide();
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

// 想把这里设计为:点击任何按钮,都自动隐藏,但是因为用户可能不止只想做一件事,所以可以用另一个save来隐藏安卓SDK 相关的界面.
// 逻辑理顺: 这样安卓SDK与游戏的相互调用就狠简单了
// 这里,游戏的暂停与恢复,稍微涉及一点儿安卓SDK与游戏两端的相互通知交接,尽量让这个相互调用的过程,两端的界面都平滑流畅.尽量不出现不必要的背景屏
    void OnClickSetButton() { // 设置完成,恢复游戏.这里可能要稍微复杂一点儿 ?
        Debug.Log(TAG + " OnClickSetButton()");
        gameObject.SetActive(false); // 这部分逻辑交由桥接层去处理
// 怎么去通知热更新域里的游戏,继续呢?> 所以,安卓SDK中DWUnityActivity里关于 gamePause, gameResume的管理逻辑,要搞清楚
// 所以仍然最好是往下走,到DWSDK DWUnityActivity,以实现两端的相互通知
        
    }
    void onClickSoundOnButton() {
        Debug.Log(TAG + " onClickSoundOnButton()");
        // VolumeManager.Instance.setVolume(-1);
        sndPanel.SetActive(false);
    }
    void onClickSoundOffButton() {
        Debug.Log(TAG + " onClickSoundOffButton()");
        // VolumeManager.Instance.setVolume(0);
        sndPanel.SetActive(false);
    }
}