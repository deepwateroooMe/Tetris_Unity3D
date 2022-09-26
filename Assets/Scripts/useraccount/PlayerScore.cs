using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;   // ===> needs to be integrated
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {
    private const string TAG = "PlayerScore";     // docs
// https://firebase.google.com/docs/reference/rest/auth?authuser=0    
    
    public Text scoreText;
    public Text getScoreText;

    private System.Random random = new System.Random();

// web api key: AIzaSyCIafb1DBdsdPlc0Tb7-3ndV4iM2kVTHrA
    private string databaseUrl = "https://my-firebase-unity-games.firebaseio.com/users";
    private string authKey = ""; // have a copy in wechat

    public static int playerScore;
    public static string playerName;
    public static string localId;
    public static fsSerializer serializer = new fsSerializer();
    
    public Text usernameText;
    public Text emailText;
    public Text passwordText;
    
    private string idToken;
    private User user = new User();
    private string getLocalId;
    
    void Start() {
        playerScore = random.Next(0, 10000);
        scoreText.text = "Score: " + playerScore;
    }

    public void OnSubmit() {
        PostToDatabase();
    }

    public void OnGetScore() {
        GetLocalId();
        RetriveFromDatabase();
    }

    private void UpdateScore() {
        scoreText.text = user.userScore.ToString();
    }
    // need update these when using Firebase Auth
    private void PostToDatabase(bool emptyScore = false, string idTokenTemp = "") {
        if (idTokenTemp == "") {
                idTokenTemp = idToken;
        }
        User user = new User();
        if (emptyScore) {
            user.userScore = 0;
        }
        RestClient.Put(databaseUrl + "/" + localId + "/.json?auth=" + idTokenTemp, user); // push into users subfolder
    }

    public void SignUpUserButton() {
        Debug.Log(TAG + ": SignUpUserButton() start");
        Debug.Log(TAG + ": SignUpUserButton() emailText.text: " + emailText.text); 
        SignUpUser(emailText.text, usernameText.text, passwordText.text);
    }
    
    public void SignInUserButton() {
        SignInUser();
    }

    public void SignInAnonymouslyButton() {
        SignInAnonymously();
    }
    private void SignInAnonymously() {
        // https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=[API_KEY]    
    }

    // password reset
    private void resetPassword() {
        // https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=[API_KEY]

        //Verify password reset code
        //Method: POST
        //https://identitytoolkit.googleapis.com/v1/accounts:resetPassword?key=[API_KEY]
    }
    
    private void RetriveFromDatabase() {
        // http requests take time to response, as a result, it will just simply returns null
        RestClient.Get<User>(databaseUrl + "/" + getLocalId + "/.json?auth=" + idToken).Then(response => {
                //return response; // due to http call response delay, we cannot return this immediately, but write functions to do the work when response is ready
                user = response;
                UpdateScore();
            });
        //return null; // throw some exception if something wired happened, instead of returnning meaningless null  
    }  

    private void SignUpUser(string email, string username, string password) { // signupNewUser
        Debug.Log(TAG + ": SignUpUser() start"); 
        // {"email": "EMAIL", "password": "PASSWORD", "returnSecureToken": true} 
        // '{"email":"[user@example.com]","password":"[PASSWORD]","returnSecureToken":true}'
        string userData = "{\"email\":\"" + email +"\",\"password\": \"" + password + "\",\"returnSecureToken\":true}";
        Debug.Log(TAG + ": SignUpUser() userData: " + userData); // {"email":"ter_000@hotmail.com","password": "123456","returnSecureToken":true}

// Seret key, 100%: by using authKey
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingpart/signupNewUser?key=" + authKey, userData).Then(response => {
        //RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + authKey, userData).Then(response => {
                string emailVerification = "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\": \"" + response.idToken + "\"}";
                RestClient.Post("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + authKey, emailVerification);
                //RestClient.Post("https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + authKey, emailVerification);
                localId = response.localId;
                playerName = username;
                PostToDatabase(true, response.idToken);
            }).Catch(error => {
                    Debug.Log(TAG + "SignUpUser Post() error"); 
                     });
    }
    
    private void SignInUser() { // verifyPassword
        //string userData = "{\"email\":\"" +email+"\",\"password\": \"" + passwordText.text + "\",\"returnSecureToken\":true}";
        string userData = "{\"email\":\"" + emailText.text +"\",\"password\": \"" + passwordText.text + "\",\"returnSecureToken\":true}";

        //RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingpart/verifyPassword?key=" + authKey, userData).Then(response => {
        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + authKey, userData).Then(response => {
                string emailVerification = "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=" + authKey, emailVerification)
                .Then(emailResponse => {
                        fsData emailVerificationData = fsJsonParser.Parse(emailResponse.Text);
                        EmailConfirmationInfo emailConfirmationInfo = new EmailConfirmationInfo();
                        serializer.TryDeserialize(emailVerificationData, ref emailConfirmationInfo).AssertSuccessWithoutWarnings();
                        if (emailConfirmationInfo.users[0].emailVerified) {
                            idToken = response.idToken;
                            localId = response.localId;
                            GetUsername();
                        } else {
                            Debug.Log("You have NOT verify your email address yet. Please verify your email. ");
                        }
                    });
            }).Catch(error => {
                    Debug.Log(TAG + "SignInUser Post() error"); 
                });
    }

    private void GetUsername() {
        RestClient.Get<User>(databaseUrl + "/" + localId + "/.json?auth=" + idToken).Then(response => {
                //return response; // due to http call response delay, we cannot return this immediately, but write functions to do the work when response is ready
                playerName = response.userName;
            });
    }

    private void GetLocalId() { // full serializer library
        // {"localId": {"localId": "LOCALID", "username": "USERNAME", "userscore": 86}}
        var username = getScoreText.text;
        
        RestClient.Get(databaseUrl + "/.json?auth=" + idToken).Then(response => {
                fsData userData = fsJsonParser.Parse(response.Text);
                Dictionary<string, User> users = null;
                serializer.TryDeserialize(userData, ref users);

                foreach (var user in users.Values) {
                    if (user.userName == username) {
                        getLocalId = user.localId;
                        RetriveFromDatabase();
                        break;
                    }
                }
            });
    }
}
