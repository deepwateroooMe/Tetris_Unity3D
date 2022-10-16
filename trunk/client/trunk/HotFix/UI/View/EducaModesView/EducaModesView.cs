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
        int curToggle = 3;

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
            // 这里想到的简单实现是:因为只是三四五三种不同的格子,将三种不同的格子的视图绑定到同一个视图模型上,进行数据的统一分情况管理?
            // curToggle = 5; // 得到了想要的格子的设置值,根据这个值来配置视图
            switch (curToggle) {
            case 3:
                ViewManager.ThreeGridView.Reveal();
                break;
            case 4:
                ViewManager.FourGridView.Reveal();
                break;
            case 5:
                ViewManager.FiveGridView.Reveal();
                break;
            }

// 所有游戏场景公用视图资源等
            // 想要把这些视图合并成一个的原因是:合并成一个共用一个ViewModel(不合并也可以共用同一个),试图实现视图层与视图模型层模块化双向数据传递
            // 这个游戏比参考项目中的逻辑更为复杂一点儿,必须去回想上半年当初车载按摩模块View ViewModel的双向数据传递逻辑并在这个项目中实现出来
            // 这里细分是因为游戏逻辑中有逻辑相关视图的隐藏也显示, 但是我现在去游戏逻辑里找,又还没有找出来
            ViewManager.DesView.Reveal(); // 不可变的
            ViewManager.ScoreDataView.Reveal(); // 可变数据
            ViewManager.ComTetroView.Reveal();// 所有游戏主场景需要用到的方块砖视图
            ViewManager.EduTetroView.Reveal();// 教育儿童模式专用的方块砖视图

            ViewManager.StaticBtnsView.Reveal();// 基本只有按钮的图像变化刷新
            ViewManager.ToggleBtnView.Reveal(); // 需要改变按钮视图组,调用更为频繁,单列为一个视图(但是可能还是应该合并到上面static里,因其逻辑复杂只是单列出来,能够文件小逻辑更为清淅一点儿?)
            ViewManager.EduBtnsView.Reveal();   // 教育儿童模式专用两个按钮,只有图像变化

            Hide(); 
        }
        
        void ActiveToggle() {
            if (thrToggle.isOn) {
// 这里需要去想: 怎么实现数据与视图层的分享,可能写在ViewModel里吧;
// 因为程序的所有的逻辑都是在热更新里,所以unity端除了几乎为空的物件加载起热更新程序集,unity其实可以基本不用再作什么了
// 因为打算全游戏放热更新里,所以不存在两个域间大规模的数据交互等复杂逻辑;
                curToggle = 3;
            } else if (furToggle.isOn) {
                curToggle = 4;
            } else if (fivToggle.isOn) {
                curToggle = 5;
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
