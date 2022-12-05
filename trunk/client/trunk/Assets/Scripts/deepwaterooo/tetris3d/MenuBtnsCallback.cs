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

        void Awake() {
            Debug.Log(TAG + " Awake()");
        }
        // public void iniate() {
        //     Debug.Log(TAG + " iniate()");
        //     sdk = gameObject.GetComponent<SDK>();
        //     eduButton = gameObject.FindChildByName("eduBtn").GetComponent<Button>();
        //     claButton = gameObject.FindChildByName("claBtn").GetComponent<Button>();
        //     chaButton = gameObject.FindChildByName("chaBtn").GetComponent<Button>();
        //     Debug.Log(TAG + " (eduButton == null): " + (eduButton == null));
        //     Debug.Log(TAG + " (claButton == null): " + (claButton == null));
        //     Debug.Log(TAG + " (chaButton == null): " + (chaButton == null));
        //     eduButton.onClick.AddListener(OnClickEduButton);
        //     // claButton = gameObject.FindChildByName("claBtn").GetComponent<Button>();
        //     claButton.onClick.AddListener(OnClickClaButton);
        //     // chaButton = gameObject.FindChildByName("chaBtn").GetComponent<Button>();
        //     chaButton.onClick.AddListener(OnClickChaButton);
        // }

        void Start() {
            Debug.Log(TAG + " Start()");
            Debug.Log(TAG + " (gameObject.FindChildByName(eduBtn) == null): " + (gameObject.FindChildByName("eduBtn") == null));
            eduButton = gameObject.FindChildByName("eduBtn").GetComponent<Button>();
            Debug.Log(TAG + " (eduButton == null): " + (eduButton == null));

            eduButton.onClick.AddListener(OnClickEduButton);
            claButton = gameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);
            chaButton = gameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);

// 这里的原理有点儿没想明白:为什么必须得调用这个类?先试第一套方案            
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
            AsrEventCallback asrEventCallback = new AsrEventCallback();
            // 设置语音识别回调函数接口
            jo.Call("setCallback", asrEventCallback);
            
            Debug.Log(TAG + " (jc == null): " + (jc == null));
            Debug.Log(TAG + " (jo == null): " + (jo == null));
        }

        AndroidJavaClass Context, jc;
        AndroidJavaObject mAudioManager, jo;
        AndroidJavaClass AudioManager;
        int STREAM_MUSIC;
        public int getMaxVolume() {
            Debug.Log(TAG + " getMaxVolume()");
            int val = jo.Call<int>("getMaxVolume"); // 记得,这种调用是需要时间的,需要把结果给返回来
            Debug.Log(TAG + " val: " + val);
            return val;
// // #if UNITY_ANDROID //&& !UNITY_EDITOR            
//             mAudioManager = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("AUDIO_SERVICE"));
//             int tmp = mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
//             // return mAudioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC);
//             Debug.Log(TAG + " tmp: " + tmp);
//             return tmp;
// // #else
// //             return 0;
// // #endif
        }
        // int onMaxVolumeReady(int val) {
        //     Debug.Log(TAG + " onMaxVolumeReady() val: " + val);
        //     maxVol.Value = val;
        //     return val;
        // }
        // int onCurVolumeReady(int val) {
        //     Debug.Log(TAG + " onCurVolumeReady() val: " + val);
        //     curVol.Value = val;
        //     return val;
        // }
        public int getCurrentVolume() {
            Debug.Log(TAG + " getCurrentVolume()");
            curVol = jo.Call<int>("getCurrentVolume");
            Debug.Log(TAG + " curVol: " + curVol);
            return curVol;
// // #if UNITY_ANDROID //&& !UNITY_EDITOR
//             int tmp =  mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
//             Debug.Log(TAG + " tmp: " + tmp);
//             return tmp;
//             // return mAudioManager.Call<int>("getStreamVolume", STREAM_MUSIC);
// // #else
// //              return 0;
// // #endif
        }
        public void setVolume(int value) {
            Debug.Log(TAG + " setVolume() value: " + value);
            jo.Call("setVolume", 50);
//             // // Debug.Log(TAG + " (curVol == null): " + (curVol == null));
//             // // Debug.Log(TAG + " curVol.Value: " + curVol.Value);
//             // if (curVol.Value == value) return ;
//             // if (value == 0)
//             //     preVol = curVol.Value;
//             // else if (curVol.Value == 0 && value == -1)
// 			// 	value = preVol;
//             mAudioManager.Call("setStreamVolume", STREAM_MUSIC, value, AudioManager.GetStatic<int>("FLAG_PLAY_SOUND"));
// // TODO: 这里要不要考虑系统设置失败的情况,虽然极少?
//             // curVol.Value = value;
        }

        public int maxVol, curVol;
        
#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL: it works!!!
            Debug.Log(TAG + " OnClickEduButton() func as getCurrentVolume()");
            // ga.HotFix.startEducational();
            // gameObject.SetActive(false);

            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AsrEventCallback asrEventCallback = new AsrEventCallback();
            asrEventCallback.setMenuBtnsCallbackGameObject(this.gameObject);
            
            // 设置语音识别回调函数接口
            jo.Call("setCallback", asrEventCallback); // 这里回调是可以设置成功的,可是找不到方法
// E Unity   : AndroidJavaException: java.lang.NoSuchMethodError: no non-static method with name='getMaxVolume' signature='(Ljava/lang/Object;)V' in class Ljava.lang.Object;
// E Unity   : java.lang.NoSuchMethodError: no non-static method with name='getMaxVolume' signature='(Ljava/lang/Object;)V' in class Ljava.lang.Object;
// E Unity   : 	at com.unity3d.player.ReflectionHelper.getMethodID(Unknown Source:51)
// E Unity   : 	at com.unity3d.player.UnityPlayer.nativeRender(Native Method)
            // jo.CallStatic<int>("getCurrentVolume");
            // jo.Call<int>("getMaxVolume");
            // jo.Call("setVolume", 50); 
            
// // 所以换下面的方式再来写
//             AndroidJavaClass jc = new AndroidJavaClass("com.deepwaterooo.dwsdk.MainActivity");
//             AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("mActivity");
//             AsrEventCallback asrEventCallback = new AsrEventCallback(); // 这么写回调都不能设置成功, 但是下面的方法是可以设置成功的，　finally ......
// 那么回到问题的原点:我需要建一个完整的安卓应用,先在安卓中把这些功能测试好,在安卓中能够正常运行后,再回到上面的调用方法            

            jc = new AndroidJavaClass("com.deepwaterooo.dwsdk.MainActivity");
            jo = jc.GetStatic<AndroidJavaObject>("mActivity");
            Debug.Log(TAG + " (jc == null): " + (jc == null));
            Debug.Log(TAG + " (jo == null): " + (jo == null));
            
            jc.CallStatic<int>("getCurrentVolume");
            jo.Call<int>("getMaxVolume");
            jo.Call("setCurrentVolume", 5);
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