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
            // GloData.Instance.saveGamePathFolderName = "educational/grid";
            // GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            // GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            // menuViewPanel.SetActive(false);
            // educaModesViewPanel.SetActive(true);
            // GloData.Instance.gameMode.Value = 0;
            // GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            // ViewManager.SettingsView.Hide();
        }
        void OnClickClaButton() { // CLASSIC MODE
            Debug.Log(TAG + " OnClickClassicButton()");
            ga.HotFix.startClassical();
                gameObject.SetActive(false);
            // GloData.Instance.saveGamePathFolderName = "classic/level";
            // GloData.Instance.gridSize.Value = 5;
            // GloData.Instance.gridXSize = 5;
            // GloData.Instance.gridZSize = 5;
            // GloData.Instance.camPos.Value = new Vector3(14.10899f, 23.11789f, -1.698298f);
            // GloData.Instance.camRot.Value = Quaternion.Euler(new Vector3(490.708f, -251.184f, -539.973f));
            // GloData.Instance.gameMode.Value = 1;
            // GameView.nextTetrominoSpawnPos = new Vector3(2.0f, Model.gridHeight - 1f, 2.0f);
            // offerGameLoadChoice();
            // ViewManager.SettingsView.Hide();
        }
        void OnClickChaButton() { // CHALLENGE MODE
            Debug.Log(TAG + " OnClickClallengeButton()");
            ga.HotFix.startChallenging();
            gameObject.SetActive(false);
            // GloData.Instance.saveGamePathFolderName = "challenge/level";
            // GloData.Instance.isChallengeMode = true;
            // ViewManager.ChallLevelsView.Reveal();
            // GloData.Instance.gameMode.Value = 0;
            // ViewManager.SettingsView.Hide();
            // Hide();
        }
#endregion
    }
}
