using Framework.Core;
using Framework.Util;
using UnityEngine;
using UnityEngine.UI;

namespace deepwaterooo.tetris3d {

    public class MenuBtnsCallback : SingletonMono<MenuBtnsCallback> {
        private const string TAG = "MenuBtnsCallback";

        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        [SerializeField]
        public GameApplication ga;
        private SDK sdk;
        private Text title;
        
        void Start() {
            Debug.Log(TAG + " Start()");
            title = gameObject.FindChildByName("Text").GetComponent<Text>();
            eduButton = gameObject.FindChildByName("eduBtn").GetComponent<Button>();
            eduButton.onClick.AddListener(OnClickEduButton);
            claButton = gameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);
            chaButton = gameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);

// // 这里的原理有点儿没想明白:为什么必须得调用这个类?先试第一套方案            
//             jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//             jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
//             AsrEventCallback asrEventCallback = new AsrEventCallback();
//             // 设置语音识别回调函数接口
//             jo.Call("setCallback", asrEventCallback);
            // Debug.Log(TAG + " (jc == null): " + (jc == null));
            // Debug.Log(TAG + " (jo == null): " + (jo == null));
        }

        AndroidJavaClass androidSDK;
        // AndroidJavaObject jo;

        // int STREAM_MUSIC;
        // public int getMaxVolume() {
        //     Debug.Log(TAG + " getMaxVolume()");
        //     int val = jo.Call<int>("getMaxVolume"); // 记得,这种调用是需要时间的,需要把结果给返回来
        //     Debug.Log(TAG + " val: " + val);
        //     return val;
        // }
        // public int getCurrentVolume() {
        //     Debug.Log(TAG + " getCurrentVolume()");
        //     curVol = jo.Call<int>("getCurrentVolume");
        //     Debug.Log(TAG + " curVol: " + curVol);
        //     return curVol;
        // }
        // public void setVolume(int value) {
        //     Debug.Log(TAG + " setVolume() value: " + value);
        //     jo.Call("setVolume", 50);
        // }

        public int maxVol, curVol;
        
#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL: it works!!!
            Debug.Log(TAG + " OnClickEduButton() func as getCurrentVolume()");
            // ga.HotFix.startEducational();
            // gameObject.SetActive(false);

// 设置回调,调用各种方法都没有问题
            androidSDK = new AndroidJavaClass("com.deepwaterooo.dwsdk.DWSDK");
            // jo = jc.GetStatic<AndroidJavaObject>("mActivity");
// // 源项目没有使用接口的方式,暂时还不考虑,等把这些基础逻辑弄通,理解比较好一点儿之后再去连通            
//             AsrEventCallback asrEventCallback = new AsrEventCallback();
//             asrEventCallback.setMenuBtnsCallbackGameObject(this.gameObject); // <<<<<<<<<< 这里过会儿还可以再简写一下
//             // 设置语音识别回调函数接口
//             jo.Call("setCallback", asrEventCallback); // 这里回调是可以设置成功的,

            androidSDK.CallStatic<int>("add", 20, 7);
            // jo.Call<int>("getMaxVolume");
            // jo.Call("setCurrentVolume", 5);

            // ga.HotFix.startEducational(); // 还是进入热更新
            // gameObject.SetActive(false);

//             AndroidJavaClass androidSDK = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); // 因为这套不好调用方法,所以不用,就用上面的了
//             AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
//             AsrEventCallback asrEventCallback = new AsrEventCallback();
//             asrEventCallback.setMenuBtnsCallbackGameObject(this.gameObject);
//             // 设置语音识别回调函数接口
//             jo.Call("setCallback", asrEventCallback); // 这里回调是可以设置成功的,可是找不到方法
        }

        public void onAddResultReady(string v) { // 好像private也可以的
            Debug.Log(TAG + " onAddResultReady() v: " + v);
            title.text = v;
        }
        void OnClickClaButton() { // CLASSIC MODE
            Debug.Log(TAG + " OnClickClassicButton()");
            ga.HotFix.startClassical();
            gameObject.SetActive(false);
        }

        void OnClickChaButton() { // CHALLENGE MODE
            Debug.Log(TAG + " OnClickClallengeButton()");
            ga.HotFix.startChallenging();
            gameObject.SetActive(false);
        }
#endregion
    }
}