using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
// using System.Linq;
using System.Text;

namespace tetris3d {

    public class GameMenuHandler : MonoBehaviour { 
        private const string TAG = "GameMenuHandler";

        public GameObject newGameOrLoadSavedGamePanel;
        public GameObject easyModeGridSizePanel;
        public GameObject squaresEdgesCornersPanel;
        
        public Toggle threeGrid;
        public Toggle fourGrid;
        public Toggle fiveGrid;

        public void ActiveToggle() {
            // Debug.Log(TAG + ": ActiveToggle()"); 
            if (threeGrid.isOn) {
                Debug.Log(TAG + ": 3 selected"); 
                GloData.Instance.gridSize = 3;
                GloData.Instance.gridXSize = 3;
                GloData.Instance.gridZSize = 3;
            } else if (fourGrid.isOn) {
                Debug.Log(TAG + ": 4 selected"); 
                GloData.Instance.gridSize = 4;
                GloData.Instance.gridXSize = 4;
                GloData.Instance.gridZSize = 4;
            } else if (fiveGrid.isOn) {
                Debug.Log(TAG + ": 5 selected"); 
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
            }
            if (isSavedFileExist()) {
                easyModeGridSizePanel.SetActive(false);
                newGameOrLoadSavedGamePanel.SetActive(true);
            } else
                LoadScene("Main");
        }
        
        public void onEducationalMode () {
            Debug.Log(TAG + ": onEducationalMode()"); 
            
            GloData.Instance.saveGamePathFolderName = "educational";
            GloData.Instance.gameMode = 0;
            easyModeGridSizePanel.SetActive(true);
        }

        public void onToggleGridSizeSubmit() {
            // Debug.Log(TAG + ": onToggleGridSizeSubmit()");g
            ActiveToggle();
        }
        
        public void onClasssicMode () {
            GloData.Instance.saveGamePathFolderName = "classic";
            GloData.Instance.gameMode = 1;
            if (isSavedFileExist())
                newGameOrLoadSavedGamePanel.SetActive(true);
            else
                LoadScene("Main");
        }
            
        public void onChallengeMode () {
            GloData.Instance.saveGamePathFolderName = "challenge";
            GloData.Instance.gameMode = 0;
            GloData.Instance.isChallengeMode = true;

            // if (isSavedFileExist())  // commented out for tmep
            //     newGameOrLoadSavedGamePanel.SetActive(true);
            // else

            // LoadScene("Main");
            LoadScene("ChallengeLevels");
                // squaresEdgesCornersPanel.SetActive(true);
        }

        public void onNewGame() {
            GloData.Instance.loadSavedGame = false;
            if (GloData.Instance.isChallengeMode)
                LoadScene("LevelTrials");
            else
                LoadScene("Main");
        }
        
        public void onLoadSavedGame() { // saved file in separate folders:  challenge classic educational
            GloData.Instance.loadSavedGame = true;
            if (GloData.Instance.isChallengeMode)
                LoadScene("LevelTrials");
            else
                LoadScene("Main");
        }
        
        public void onClosePanel() {
            newGameOrLoadSavedGamePanel.SetActive(false);
        }
        
        private bool isSavedFileExist() {
            Debug.Log(TAG + ": isSavedFileExist()");
            StringBuilder currentPath = new StringBuilder("");
            if (GloData.Instance.gameMode > 0 || GloData.Instance.isChallengeMode)
                currentPath.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/game.save");
            else 
                currentPath.Append(Application.persistentDataPath + "/" + GloData.Instance.saveGamePathFolderName + "/grid" + GloData.Instance.gridSize + "/game.save");
            Debug.Log(TAG + " currentPath: " + currentPath.ToString()); 
            if (File.Exists(currentPath.ToString()))
                return true;
            return false;
        }
        
        private void LoadScene(string scene) {
            SceneManager.LoadSceneAsync(scene);
        }

        public void onLogin() {
            LoadScene("Signin");
        }

        public void onLogout() {
            
        }

        public void getCredit() {
            
        }
        
        public void rateTheGame() {
            
        }
        
        public void toggleSounds() {
            
        }
        
        public void getAdsFree() {
            
        }
        
        public void toggleSettings() {
            
        }
    }
}

