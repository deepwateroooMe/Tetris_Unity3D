using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User {

    public string userName;
    public int userScore;
    public string localId;
    

    public User() {
        userName = PlayerScore.playerName;
        userScore = PlayerScore.playerScore;
        localId = PlayerScore.localId;
    }

    
}
