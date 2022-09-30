using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tetris3d {

    public class LoadGame : MonoBehaviour {
        private const string TAG = "LoadGame";
        
        public void LoadScene() {
            Debug.Log(TAG + ": LoadScene()"); 
            SceneManager.LoadSceneAsync("GameMenu");
        }
    }
}