using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    public class EducaModesView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/educamodesview";
            }
        }
        public override string AssetName {
            get {
                return "EducaModesView";
            }
        }
        public override string ViewName {
            get {
                return "EducaModesView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(EducaModesViewModel).FullName;
            }
        }
        public EducaModesViewModel ViewModel {
            get {
                return (EducaModesViewModel)BindingContext;
            }
        }

        Toggle thrToggle;
        Toggle furToggle;
        Toggle fivToggle;
        // int curToggle = 3; // 这里在折腾数据是不对的,应该是

        Button conBtn; // CONFIRM

        protected override void OnInitialize() {
            base.OnInitialize();

            thrToggle = GameObject.FindChildByName("Toggle3").GetComponent<Toggle>();
            furToggle = GameObject.FindChildByName("Toggle4").GetComponent<Toggle>();
            fivToggle = GameObject.FindChildByName("Toggle5").GetComponent<Toggle>();
            
            conBtn = GameObject.FindChildByName("conBtn").GetComponent<Button>();
            conBtn.onClick.AddListener(OnClickConButton);
        }

        void OnClickConButton() {
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面,这一小步暂时跳过
            ActiveToggle();
            // 感觉这里有个更直接快速的但凡一toggle某个的时候就自动触发的观察者模式,改天再写
            
            ViewManager.GameView.Reveal();
// // 所有游戏场景公用视图资源等
//             // 想要把这些视图合并成一个的原因是:合并成一个共用一个ViewModel(不合并也可以共用同一个),试图实现视图层与视图模型层模块化双向数据传递
//             // 这个游戏比参考项目中的逻辑更为复杂一点儿,必须去回想上半年当初车载按摩模块View ViewModel的双向数据传递逻辑并在这个项目中实现出来
//             // 这里细分是因为游戏逻辑中有逻辑相关视图的隐藏也显示, 但是我现在去游戏逻辑里找,又还没有找出来.若是并非必要折解成很多小视图,可能还是会把公用游戏逻辑整合到一个视图中来
//             ViewManager.DesView.Reveal(); // 不可变的
//             ViewManager.ScoreDataView.Reveal(); // 可变数据
//             ViewManager.ComTetroView.Reveal();// 所有游戏主场景需要用到的方块砖视图
//             ViewManager.EduTetroView.Reveal();// 教育儿童模式专用的方块砖视图
//             ViewManager.StaticBtnsView.Reveal();// 基本只有按钮的图像变化刷新
//             ViewManager.ToggleBtnView.Reveal(); // 需要改变按钮视图组,调用更为频繁,单列为一个视图(但是可能还是应该合并到上面static里,因其逻辑复杂只是单列出来,能够文件小逻辑更为清淅一点儿?)
//             ViewManager.EduBtnsView.Reveal();   // 教育儿童模式专用两个按钮,只有图像变化

            Hide();  
        }
        
        void ActiveToggle() {
            if (thrToggle.isOn) {
                ((EducaModesViewModel)BindingContext).GridWidth = 3;
            } else if (furToggle.isOn) {
                ((EducaModesViewModel)BindingContext).GridWidth = 4;
            } else if (fivToggle.isOn) {
                ((EducaModesViewModel)BindingContext).GridWidth = 5;
            }

// 这里这个视图的加载之后再考虑,太简单            
            // if (isSavedFileExist()) { 
            //     easyModeToggleSizePanel.SetActive(false);
            //     newGameOrLoadSavedGamePanel.SetActive(true);
            // } else
            //     LoadScene("Main");
        }
    }
}

