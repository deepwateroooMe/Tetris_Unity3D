using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace tetris3d {

    public class MenuSystem : MonoBehaviour {
        public void PlayAgain() {
            SceneManager.LoadScene("Main");
        }
    }
}
