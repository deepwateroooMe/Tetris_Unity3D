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
        
        // public GameObject toggleGroupGO;
        public Toggle threeGrid;
        public Toggle fourGrid;
        public Toggle fiveGrid;
        

        // private ToggleGroup toggleGroupInstance;
        
        void Start() {
            // toggleGroupInstance = toggleGroupGO.GetComponent<ToggleGroup>();
        }
        // public Toggle currentSelection {
        //     get {
        //         return toggleGroupInstance.ActiveToggles().FirstOrDefault();
        //     }
        // }

        public void ActiveToggle() {
            Debug.Log(TAG + ": ActiveToggle()"); 
            if (threeGrid.isOn) {
                Debug.Log(TAG + ": 3 selected"); 
                GameMenuData.Instance.gridSize = 3;
            } else if (fourGrid.isOn) {
                Debug.Log(TAG + ": 4 selected"); 
                GameMenuData.Instance.gridSize = 4;
            } else if (fiveGrid.isOn) {
                Debug.Log(TAG + ": 5 selected"); 
                GameMenuData.Instance.gridSize = 5;
            }
            if (isSavedFileExist()) {
                easyModeGridSizePanel.SetActive(false);
                newGameOrLoadSavedGamePanel.SetActive(true);
            } else
                LoadScene("Main");
        }
        
        public void onEducationalMode () {
            Debug.Log(TAG + ": onEducationalMode()"); 
            
            GameMenuData.Instance.saveGamePathFolderName = "educational";
            GameMenuData.Instance.gameMode = 0;
            easyModeGridSizePanel.SetActive(true);
        }

        public void onToggleGridSizeSubmit() {
            Debug.Log(TAG + ": onToggleGridSizeSubmit()");
            ActiveToggle();
        }
        
        public void onClasssicMode () {
            GameMenuData.Instance.saveGamePathFolderName = "classic";
            GameMenuData.Instance.gameMode = 1;
            if (isSavedFileExist())
                newGameOrLoadSavedGamePanel.SetActive(true);
            else
                LoadScene("Main");
        }
            
        public void onChallengeMode () {
            GameMenuData.Instance.saveGamePathFolderName = "challenge";
            GameMenuData.Instance.gameMode = 2;
            if (isSavedFileExist())
                newGameOrLoadSavedGamePanel.SetActive(true);
            else
                LoadScene("Main");
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
        
        public void onNewGame() {
            GameMenuData.Instance.loadSavedGame = false;
            LoadScene("Main");
        }
        
        public void onLoadSavedGame() { // saved file in separate folders:  challenge classic educational
            // try to always keep the last saved game for this specific mode
            // if (isSavedFileExist()) { // set global flag
            GameMenuData.Instance.loadSavedGame = true;
            LoadScene("Main");
            // } 
        }
        
        public void onClosePanel() {
            newGameOrLoadSavedGamePanel.SetActive(false);
        }
        
        private bool isSavedFileExist() {
            Debug.Log(TAG + ": isSavedFileExist()");
            StringBuilder currentPath = new StringBuilder("");
            if (GameMenuData.Instance.gameMode > 0)
                currentPath.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game.save");
            else 
                currentPath.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/grid" + GameMenuData.Instance.gridSize + "/game.save");
            Debug.Log(TAG + " currentPath: " + currentPath.ToString()); 
            if (File.Exists(currentPath.ToString()))
                return true;
            return false;
        }
        
        private void LoadScene(string scene) {
            SceneManager.LoadSceneAsync(scene);
        }

        public void toggleSettings() {
            
        }
    }
}

