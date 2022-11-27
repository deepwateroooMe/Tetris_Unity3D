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
        Slider sndSdr;

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

// TODO: 安卓SDK是一定会去定义和实现的.那现在的问题就变成是:哪些unity游戏通用功能和模块是,我可以包装进安卓SDK的,以后的游戏中都可以反复地使用自己包装出来的安卓SDK ?
// 账户登录登出,音量等安卓原生调用相关,电池耗电量提醒,低电量提醒,数据库,与服务器交互的逻辑等 还有哪些呢?
// Unity与安卓的交互都一个交互界面layer层;现在我并不是安卓直接与unity相互调用,而是与热更新域(不能与热更新域,太复杂了,只与unity域,也就是说安卓SDK里的逻辑设置为不可热更新,当游戏安装后,除非卸载重新安装,将永久不变)

// TODO: 这里的逻辑仍然没有想透; 安卓SDK与unity的相互调用逻辑相对很简单. 现在的问题仍然是在于,设置不设置独立的安卓SDK(还是使用现没有独立SDK的打劫方式?)是一个相对不太重要的问题,现在最主要的问题是现架构所有游戏逻辑放在热更新域里,安卓SDK与热更新域如何传值传数据相互调用? （要把游戏菜单界面 MenuView + SettingsView移出热更新之外方便与安卓SDK交互吗? 这部分逻辑的移动要怎么移,可能会造成哪些问题? 还是说放弃这个音量调控,放弃一个安卓SDK直接去解决热更新服务商服务器的问题?）先网上再搜索了解一下
// 如果安卓SDK的包是在unity里的,重点是unity知道sdk中定义的是什么方法,在热更新域里直接调用完全没有区别 ? 试一下            
// 解决思路一: SettingsView + (fake)MenuView 放Unity域: 这里仍然是还没想透, 这个主菜单界面会在游戏中反复出现,仍涉及SDK与热更新域的相互调用
// 另一个可以考虑的思路是:相比于java安卓SDK,使用c/c++ jni打包.so库,以期待实现热更新.dll库与.so库多个域的同时存在与相互调用? 感觉这个思路也还有点儿探讨价值,那么游戏的入口顶层架构需要可以同时支持两三个域,会需要一些重构工作.可能比这个想法还要简单,想一想其实其它第三方库是如何在热更新域里使用的呢?.dll .so打包进Assets/Plugins/Android可能就可以了,或者加上必要的帮助类 ? 那么这里所有游戏逻辑放在热更新域里的好处就是,任何第三方库或是自定义库都在自己热更新程序域的内部,不涉及复杂的与外部unity域的相互调用或是事件数据传递等
            
// 所以最简单的办法,实现一个安卓SDK,将此界面同样地(实际上是音量控制安卓原生相关功能模块移植到SDK中去,这里不再保留)定义在安卓中,调用安卓中的Activity来显示或调控音量.那么如果这样,就涉及到热更新域(或者大一点儿Unity,但热更新域比起普通游戏unity里会更难一点儿)与安卓SDK的相互调用与传值,需要一个真心强大的安卓SDK(或是说安卓SDK与热更新域的相互调用),活宝妹也狠强大,爱表哥,爱生活!!!
            sndPanel = GameObject.FindChildByName("soundPanel");
            sndOn = GameObject.FindChildByName("sndOn").GetComponent<Button>();
            sndOn.onClick.AddListener(onClickSoundOnButton);
            sndOff = GameObject.FindChildByName("sndOff").GetComponent<Button>();
            sndOff.onClick.AddListener(onClickSoundOffButton);
            sndSdr = GameObject.FindChildByName("volSdr").GetComponent<Slider>(); // 这个滑动条有一些相关的事件需要处理
// 所以最简单的办法,实现一个安卓SDK,将此界面同样地(实际上是音量控制安卓原生相关功能模块移植到SDK中去,这里不再保留)定义在安卓中,调用安卓中的Activity来显示或调控音量.那么如果这样,就涉及到热更新域(或者大一点儿Unity,但热更新域比起普通游戏unity里会更难一点儿)与安卓SDK的相互调用与传值,需要一个真心强大的安卓SDK(或是说安卓SDK与热更新域的相互调用),活宝妹也狠强大,爱表哥,爱生活!!!
// // TODO:现在的问题就是:以不写SDK的形式加入,无法转化到安卓平台测试这个prototype;所以还是接个比较完整的安卓SDK会比较方便
            maxVol = VolumeManager.getMaxVolume();
            Debug.Log(TAG + " OnInitialize() maxVol: " + maxVol);
            curVol = VolumeManager.getCurrentVolume();
            Debug.Log(TAG + " OnInitialize() curVol: " + curVol);
            VolumeManager.setVolume(50);
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
