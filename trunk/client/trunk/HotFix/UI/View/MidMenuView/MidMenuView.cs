using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using UnityEngine.UI;

namespace HotFix.UI.View.MidMenuView
{
    public class MidMenuView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/midmenuview";
            }
        }
        public override string AssetName {
            get {
                return "MidMenuView";
            }
        }
        public override string ViewName {
            get {
                return "MidMenuView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(MidMenuViewModel).FullName;
            }
        }
        public MidMenuViewModel ViewModel {
            get {
                return (MidMenuViewModel)BindingContext;
            }
        }

        Button savBtn; // SAVE GAME
        Button resBtn; // RESUME GAME
        Button guiBtn; // TUTORIAL
        Button manBtn; // BACK TO MAIN MENU
        Button creBtn; // CREDIT

        protected override void OnInitialize() {
            base.OnInitialize();

            savBtn = GameObject.FindChildByName("savBtn").GetComponent<Button>();
            savBtn.onClick.AddListener(OnClickSavButton);

            resBtn = GameObject.FindChildByName("resBtn").GetComponent<Button>();
            resBtn.onClick.AddListener(OnClickResButton);

            guiBtn = GameObject.FindChildByName("guiBtn").GetComponent<Button>();
            guiBtn.onClick.AddListener(OnClickGuiButton);

            manBtn = GameObject.FindChildByName("manBtn").GetComponent<Button>();
            manBtn.onClick.AddListener(OnClickManButton);

            creBtn = GameObject.FindChildByName("creBtn").GetComponent<Button>();
            creBtn.onClick.AddListener(OnClickCreButton);
        }

        void OnClickSavButton() { // SAVE GAME
        }
        void OnClickResButton() { // RESUME GAME: 隐藏当前游戏过程中的视图,就可以了
            Hide(); // 隐藏当前视图就可以了
        }
        void OnClickGuiButton() { // 可以视频GUIDE吗?
            // ViewManager.DesginView.Reveal();
        }

// 等预制做得再大一点儿,好一点儿:需要检查一下这里的逻辑,不知道是否因为射线的穿透点到其它实则隐藏的视图,调起了其它窗口,这是不应该的        
        void OnClickManButton() { // BACK TO MAIN MENU
            ViewManager.MenuView.Reveal();
            // 这里需要隐藏所有的游戏视图相关界面,有很多个;请视图管理者来关
            // ViewManager.CloseOtherRootViews("MenuView");
        }

        void OnClickCreButton() {
            // ViewManager.DesginView.Reveal();
        }
    }
}
