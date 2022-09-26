using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tetris3d {

public class LoadGame : MonoBehaviour {
    public void LoadScene() {
        SceneManager.LoadSceneAsync("Main");
    }
}
}