using Framework.Core;
using Framework.Util;
using UnityEngine;
using UnityEngine.UI;

namespace deepwaterooo.tetris3d {
    // 这个类，实现的，功能作用：就是例子中的 Client 里的连接服务器，登录注册。不用这个，直接去用 Client.cs 类好了
    public class LoginPanel : SingletonMono<LoginPanel> {
        private const string TAG = "LoginPanel";

        public InputField useranme;
        public InputField password;
        public Button loginButton;

        [SerializeField]
        public GameApplication ga;
        
        void Start() {
            Debug.Log(TAG + " Start()");

            //title = gameObject.FindChildByName("Text").GetComponent<Text>();
            loginButton = gameObject.FindChildByName("loginBtn").GetComponent<Button>();
            loginButton.onClick.AddListener(OnClickLoginButton);
        }

        void OnClickLoginButton() {
            Debug.Log(TAG + " OnClickLoginButton()");

            gameObject.SetActive(false);
        }
    }
}