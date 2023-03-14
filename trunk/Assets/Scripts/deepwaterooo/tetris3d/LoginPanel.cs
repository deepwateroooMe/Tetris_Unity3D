using Framework.Core;
using Framework.Util;
using UnityEngine;
using UnityEngine.UI;

namespace deepwaterooo.tetris3d {

    public class LoginPanel : SingletonMono<LoginPanel> {
        private const string TAG = "LoginPanel";

        public InputField useranme;
        public InputField password;
        public Button loginButton;

        [SerializeField]
        public GameApplication ga;
        
        void Start() {
            Debug.Log(TAG + " Start()");

            title = gameObject.FindChildByName("Text").GetComponent<Text>();
            loginButton = gameObject.FindChildByName("loginBtn").GetComponent<Button>();
            loginButton.onClick.AddListener(OnClickLoginButton);
        }

        void OnClickLoginButton() {
            Debug.Log(TAG + " OnClickLoginButton()");

            gameObject.SetActive(false);
        }
#endregion
    }
}