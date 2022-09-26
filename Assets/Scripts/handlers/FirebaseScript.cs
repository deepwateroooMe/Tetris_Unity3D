using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;

public class FirebaseScript : MonoBehaviour {

    public Text email;
    public Text password;
    public Text emailInfo;
    public Text passwordInfo;
    
    public void LoginButtonPressed() {
        if (email.text == "") {
            emailInfo.text = "An email address must be provided.";
            passwordInfo.text = "";
            return;
        } else if (password.text == "") {
            emailInfo.text = "";
            passwordInfo.text = "A password must be provided.";
            return;
        }
        //else if (password.text.length() < )
        
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password.text).
            ContinueWith(obj => {
                    SceneManager.LoadScene("RoalUserMainMenu");
                });
    }        
    
    public void GoAnonymousButtonPressed() {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().
            ContinueWith(obj => {
                    SceneManager.LoadScene("Main");
                });
    }        
    
    public void CreateNewUserButtonPressed() {
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email.text, password.text).
            ContinueWith(obj => {
                    SceneManager.LoadScene("Main");
                });
    }        
}
