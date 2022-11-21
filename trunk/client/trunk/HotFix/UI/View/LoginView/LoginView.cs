using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace HotFix.UI {
	public class LoginView : UnityGuiView {
        private const string TAG = "LoginView"; 
        public override string BundleName { get { return "ui/view/loginview"; } }
        public override string AssetName { get { return "LoginView"; } }
        public override string ViewName { get { return "LoginView"; } }
        public override string ViewModelTypeName { get { return typeof(LoginViewModel).FullName; } }
        public LoginViewModel ViewModel { get { return (LoginViewModel)BindingContext; } }

        InputField nameIn;
        InputField pswdIn;
        Button emailBtn;
        Button anoyBtn;
        Button userBtn;
        Button ggBtn;
        Button fbBtn;

        protected override void OnInitialize() {
            base.OnInitialize();

            nameIn = GameObject.FindChildByName("idInput").GetComponent<InputField>();
            pswdIn = GameObject.FindChildByName("pwInput").GetComponent<InputField>();
            
            emailBtn = GameObject.FindChildByName("emaiBtn").GetComponent<Button>();
            emailBtn.onClick.AddListener(OnClickEmailButton);

            anoyBtn = GameObject.FindChildByName("anoyBtn").GetComponent<Button>();
            anoyBtn.onClick.AddListener(OnClickAnoyButton);

            userBtn = GameObject.FindChildByName("userBtn").GetComponent<Button>();
            userBtn.onClick.AddListener(OnClickUserButton);

            ggBtn = GameObject.FindChildByName("ggBtn").GetComponent<Button>();
            ggBtn.onClick.AddListener(OnClickGgButton);

            fbBtn = GameObject.FindChildByName("fbBtn").GetComponent<Button>();
            fbBtn.onClick.AddListener(OnClickFbButton);
        }

        void OnClickEmailButton() {
            
        }
        void OnClickAnoyButton() {
            
        }
        void OnClickUserButton() {
            
        }
        void OnClickGgButton() {
            
        }
        void OnClickFbButton() {
            
        }
	}
}
