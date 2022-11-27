using Framework.Core;
using Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace deepwaterooo.tetris3d {
    public class MenuBtnsCallback : SingletonMono<MenuBtnsCallback> {
        private const string TAG = "MenuBtnsCallback";

        Button eduButton; // Education
        Button claButton; // Classic
        Button chaButton; // Challenge

        [SerializeField]
        public GameApplication ga;
        private SDK sdk;
        
        void Start() {
            sdk = gameObject.GetComponent<SDK>();
            
            eduButton = gameObject.FindChildByName("eduBtn").GetComponent<Button>();
            eduButton.onClick.AddListener(OnClickEduButton);
            claButton = gameObject.FindChildByName("claBtn").GetComponent<Button>();
            claButton.onClick.AddListener(OnClickClaButton);
            chaButton = gameObject.FindChildByName("chaBtn").GetComponent<Button>();
            chaButton.onClick.AddListener(OnClickChaButton);
        }

#region EDUCATIONAL CLASSIC CHALLENGE MODES
        void OnClickEduButton() { // EDUCATIONAL
            Debug.Log(TAG + " OnClickEduButton()");
            // ga.HotFix.startEducational();
            // gameObject.SetActive(false);

// 这里,自己有个误区就是,对SDK的调用,不是调用的时候就能够拿到结果的,它是需要处理时间的,必须是在拿到结果的时候再返回给游戏端才对,估计现在仍是白试一下浪费时间            
            // GetDeviceID()
            string s = sdk.GetDeviceID();
            Debug.Log(TAG + " s: " + s);
        }

        void OnClickClaButton() { // CLASSIC MODE
            Debug.Log(TAG + " OnClickClassicButton()");
            ga.HotFix.startClassical();
                gameObject.SetActive(false);
        }

        void OnClickChaButton() { // CHALLENGE MODE
            Debug.Log(TAG + " OnClickClallengeButton()");
            ga.HotFix.startChallenging();
            gameObject.SetActive(false);
        }
#endregion
    }
}
