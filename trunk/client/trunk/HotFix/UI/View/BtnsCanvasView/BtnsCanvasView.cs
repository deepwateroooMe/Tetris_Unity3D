using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using UnityEngine;

namespace HotFix.UI
{

// 这里最大的重点是：不能把它放在相对坐标系下，一定要放在世界坐标系下
    public class BtnsCanvasView : UnityGuiView
    {
        private const string TAG = "BtnsCanvasView";

        public override string BundleName
        {
            get { return "ui/view/btnscanvasview"; }
        }

        public override string AssetName
        {
            get { return "BtnsCanvasView"; }
        }

        public override string ViewName
        {
            get { return "BtnsCanvasView"; }
        }

        public override string ViewModelTypeName
        {
            get { return typeof(BtnsCanvasViewModel).FullName; }
        }

        public BtnsCanvasViewModel ViewModel
        {
            get { return (BtnsCanvasViewModel)BindingContext; }
        }

        public override bool IsRoot
        {
            // 方便调控显示与隐藏
            get { return true; }
        }

//         public override void OnAppear() {
//             base.OnAppear(); // 这里,在基类UnityGuiView里OnRevealed()基类实现里,若是根视图,为自动关闭其它所有视图
// // 这里是只是赋值回调函数,方便将来回调,还是说这里已经回调了呢(这里应该还没有,回去检查)@!            
//             CloseOtherRootView = CloseOtherRootViews; // 只是赋值
//         }
//         void CloseOtherRootViews() { 
//         }

// public: 方便游戏视图调用它        
        public GameObject moveCanvas;

// 实际运行时,需要做多组至少3组ROTATE CANVAS,目的是当用户转动相机的时候,必要的背后视图组会隐藏,但用户前端的方便点击的会显示,
// 平衡组没有这个需求　        
        public GameObject rotateCanvas;
        // 接下来会有　4　＋　6　共10个按键

//         GameObject menuViewPanel;
//         Button eduButton; // Education
//         Button claButton; // Classic
//         Button chaButton; // Challenge
// // 把EducaModesView整合进来
//         GameObject educamodesviewPanel;
//         Toggle thrToggle;
//         Toggle furToggle;
//         Toggle fivToggle;
//         Button conBtn; // CONFIRM

        protected override void OnInitialize()
        {
            base.OnInitialize();

            moveCanvas = GameObject.FindChildByName("moveCanvas");
            rotateCanvas = GameObject.FindChildByName("rotateCanvas");

// 可以测试一下是否可隐藏

            // menuViewPanel = GameObject.FindChildByName("MenuViewPanel");
            // eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
            // eduButton.onClick.AddListener(OnClickEduButton);
            // claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
            // claButton.onClick.AddListener(OnClickClaButton);
            // chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
            // chaButton.onClick.AddListener(OnClickChaButton);
            // educamodesviewPanel = GameObject.FindChildByName("EducaModesPanel");
            // thrToggle = GameObject.FindChildByName("Toggle3").GetComponent<Toggle>();
            // furToggle = GameObject.FindChildByName("Toggle4").GetComponent<Toggle>();
            // fivToggle = GameObject.FindChildByName("Toggle5").GetComponent<Toggle>();
            // conBtn = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            // conBtn.onClick.AddListener(OnClickConButton);
        }
    }
}