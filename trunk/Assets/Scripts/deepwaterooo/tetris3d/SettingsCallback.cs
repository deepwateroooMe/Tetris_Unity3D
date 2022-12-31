using deepwaterooo.tetris3d;
using DeepwateroooWang;
using UnityEngine;
using UnityEngine.UI;

// 吃完中饭，下午会把这个测试连通

// 普通游戏端视图: 不可热更新,主要涉及与安卓SDK的交互
public class SettingsCallback : MonoBehaviour {
    private const string TAG = "SettingsCallback";

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
    Text sndTxt;
    int maxVol = 0, curVol = 0, preVol = -1;
    
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
        setBtn = gameObject.FindChildByName("setBtn").GetComponent<Button>(); 
        setBtn.onClick.AddListener(OnClickSetButton);
        sndPanel = gameObject.FindChildByName("soundPanel");
        sndOn = gameObject.FindChildByName("sndOn").GetComponent<Button>();
        sndOn.onClick.AddListener(onClickSoundOnButton);
        sndOff = gameObject.FindChildByName("sndOff").GetComponent<Button>();
        sndOff.onClick.AddListener(onClickSoundOffButton);

        // sndSdr = gameObject.FindChildByName("volSdr").GetComponent<ExtendedSlider>(); // 这个滑动条有一些相关的事件需要处理
        sndSdr = gameObject.FindChildByName("volSdr").GetComponent<Slider>(); // 这个滑动条有一些相关的事件需要处理
        sndTxt = gameObject.FindChildByName("sndTxt").GetComponent<Text>();

// 在这里对安卓的调用与回调作必要的初始化:
        // // 安卓SDK接收到手机安卓系统广播后回传回来的 接口回调: It works! But try another way of receiving broadcast from Unity side
        // VoiceVolumnWrapper.Instance.Init(SetVolumnListener); // <<<<<<<<<<<<<<<<<<<<
        
        InitSlider();
    }
// TODO: 哪里没有设置好,实时运行时,当用户触屏拖动slider,居然拖不动,需要可以从游戏直接从UI上设置音量

// // 安卓SDK收到手机音量改变了的安卓系统广播,之后的回调,以接口的形式回传回来的,能运行得通
//     void SetVolumnListener(int value) { 
//         Debug.Log(TAG + " SetVolumnListener() value: " + value);
//         if (curVol != 0) preVol = curVol;
//         curVol = value;
//         SetSliderValue(value);
//         SetTextValue(value);
//     }
    void SetSliderValue(int value) {
        sndSdr.value = value;
    }
    void SetTextValue(int value) {
        sndTxt.text = value.ToString();
    }
    void InitSlider() {
#if UNITY_EDITOR
        // 设置最大值
        sndSdr.maxValue = Random.Range(30,31);
        // 设置最小值
        sndSdr.minValue = Random.Range(0, 1);
        // 设置整数变化
        sndSdr.wholeNumbers = true;
        // 设置当前值
        sndSdr.value = Random.Range(0, 30); 
        // 设置 Slider 监听事件
        sndSdr.onValueChanged.AddListener((value) => {
            SetTextValue((int)value);
        });
        // 设置 Text 
        SetTextValue((int)sndSdr.value);
#else
        // 设置最大值
        sndSdr.maxValue = VoiceVolumnWrapper.Instance.GetMusicVoiceMax();
        // 设置最小值
        sndSdr.minValue = VoiceVolumnWrapper.Instance.GetMusicVoiceMin();
        // 设置整数变化
        sndSdr.wholeNumbers = true;
        // 设置当前值
        // sndSdr.value = VoiceVolumnWrapper.Instance.GetMusicVoiceCurrentValue();
        sndSdr.value = GetMusicVoiceCurrentValue();
        // 设置 Slider 监听事件
        sndSdr.onValueChanged.AddListener((value) => {
            Debug.Log(TAG + " value: " + value);
            // VoiceVolumnWrapper.Instance.SetMusicVoiceVolumn((int)value);
            SetMusicVoiceVolumn((int)value);
        });
        // 设置 Text 
        SetTextValue(VoiceVolumnWrapper.Instance.GetMusicVoiceCurrentValue());
#endif
        curVol = (int)sndSdr.value;
    }
    void SetMusicVoiceVolumn(int value) {
        Debug.Log(TAG + " SetMusicVoiceVolumn() value: " + value);
        VoiceVolumnWrapper.Instance.SetMusicVoiceVolumn(value);
    }
    int GetMusicVoiceCurrentValue() {
        // 设置当前值
        int vol = VoiceVolumnWrapper.Instance.GetMusicVoiceCurrentValue();
        Debug.Log(TAG + " GetMusicVoiceCurrentValue() vol: " + vol);
        if (sndSdr.value != 0) // <==> if (curVol != 0)
            preVol = (int)sndSdr.value;
        sndSdr.value = vol;
        curVol = vol;
        return vol;
    }
    void onClickSoundOnButton() { // 设置的是静音前最后一次的值
        Debug.Log(TAG + " onClickSoundOnButton()");
        SetMusicVoiceVolumn(preVol);
        sndPanel.SetActive(false);
    }
    void onClickSoundOffButton() {
        Debug.Log(TAG + " onClickSoundOffButton()");
        if (curVol != 0)
            preVol = curVol;
        SetMusicVoiceVolumn(0);
        sndPanel.SetActive(false);
    }
    
// 测试调用安卓SDK: 显示splash Screen. 所以游戏端,最好是调用游戏的底端接口,而非直接与安卓SDK交互
    void OnClickLgiButton() { // LOGIN
        Debug.Log(TAG + " OnClickLoginButton()");
// // 这里想调一个SDK中的方法,测试一下: 这个调用太弱了，怎么也得调个别的
//         Deepwaterooo.instance.DisplaySplash(); // 调用游戏的相对底层所提供给游戏的公用APIs
// 这里试着调用 ManagePlayerActivity画面,只作测试用,到时修会调用 登录界面
        Deepwaterooo.instance.OpenManagePlayers(); // 这里调不通是因为我无法登录成功,拿不到用户数据,所以不通无所谓,两边能够互相切换就可以了
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
}