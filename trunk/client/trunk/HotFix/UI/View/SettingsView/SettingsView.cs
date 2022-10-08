using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;

namespace HotFix.UI.View.SettingsView
{
    public class SettingsView : UnityGuiView {
        public override string BundleName {
            get {
                return "ui/view/settingsview";
            }
        }
        public override string AssetName {
            get {
                return "SettingsView";
            }
        }
        public override string ViewName {
            get {
                return "SettingsView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(SettingsViewModel).FullName;
            }
        }
        public SettingsViewModel ViewModel {
            get {
                return (SettingsViewModel)BindingContext;
            }
        }

        // Button eduButton; // Education
        // Button claButton; // Classic
        // Button chaButton; // Challenge

        // protected override void OnInitialize() {
        //     base.OnInitialize();

        //     eduButton = GameObject.FindChildByName("eduBtn").GetComponent<Button>();
        //     eduButton.onClick.AddListener(OnClickEduButton);

        //     claButton = GameObject.FindChildByName("claBtn").GetComponent<Button>();
        //     claButton.onClick.AddListener(OnClickClaButton);

        //     chaButton = GameObject.FindChildByName("chaBtn").GetComponent<Button>();
        //     chaButton.onClick.AddListener(OnClickChaButton);
        //     // SetDownRootIndex = ChageViewsIndex;
        // }

        void ChageViewsIndex() {
            //UnityEngine.Transform.SetAsLastSibling();
        }
        void OnClickEduButton() {
            // ViewManager.MainView.Reveal();
        }
        void OnClickClaButton() {
         
            // ViewManager.FindView.Reveal();
        }
        void OnClickChaButton() {
       
            // ViewManager.DesginView.Reveal();
        }
    }
}
