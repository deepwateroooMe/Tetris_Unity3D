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
            // 检查是否存有先前游戏进度数据,有则弹窗;无直接进游戏界面
           
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

            // if (isSavedFileExist()) {
            //     easyModeToggleSizePanel.SetActive(false);
            //     newGameOrLoadSavedGamePanel.SetActive(true);
            // } else
            //     LoadScene("Main");
        }
        
        
    }
}
