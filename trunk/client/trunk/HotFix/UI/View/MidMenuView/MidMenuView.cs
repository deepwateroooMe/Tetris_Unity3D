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

        void OnClickSavButton() {
        }
        void OnClickResButton() {
        }
        void OnClickGuiButton() {
            // ViewManager.DesginView.Reveal();
        }
        void OnClickManButton() {
            // ViewManager.DesginView.Reveal();
        }
        void OnClickCreButton() {
            // ViewManager.DesginView.Reveal();
        }
    }
}
