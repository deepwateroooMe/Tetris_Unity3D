using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace tetris3d {

public class GameMenu : MonoBehaviour {

    public Text levelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;

    void Start() {
        levelText.text = "0";
        //PlayerPrefs.SetInt("highscore", 0); // save this data in our database
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
    }
    
    public void PlayGame() {
        if (Game.startingLevel == 0) 
            Game.startingAtLevelZero = true;
        else
            Game.startingAtLevelZero = false;
        SceneManager.LoadScene("Main");
    }

    public void ChangedValue(float value) {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchGameMenu() {
        SceneManager.LoadScene("GameMenu");
    }
}
}