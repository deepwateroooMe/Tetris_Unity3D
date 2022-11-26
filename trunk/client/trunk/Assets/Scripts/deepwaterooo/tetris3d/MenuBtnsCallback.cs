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
        
        void Start() {
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
            ga.HotFix.startEducational();
            gameObject.SetActive(false);
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
