// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace tetris3d {

//namespace Firebase.Sample.Database {
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handler for UI buttons on the scene.  Also performs some
// necessary setup (initializing the firebase app, etc) on
// startup.
public class DatabaseHandler : MonoBehaviour {

    public Text scoreText;
    public Text nameText;
    public Text leaderBoardText;
    
    ArrayList leaderBoard = new ArrayList();

    private const int MaxScores = 10;
    private string logText = "";
    private string email = "";
    private int score = 100;
    protected bool UIEnabled = true;

    const int kMaxLogSize = 16382;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    protected virtual void Start() {
        leaderBoard.Clear();
        leaderBoard.Add("Firebase Top " + MaxScores.ToString() + " Scores");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    InitializeFirebase();
                } else {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
    }

    // Initialize the Firebase database:
    protected virtual void InitializeFirebase() {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        // NOTE: You'll need to replace this url with your Firebase App's database
        // path in order for the database connection to work correctly in editor.
        app.SetEditorDatabaseUrl("https://my-firebase-unity-games.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        StartListener();
        isFirebaseInitialized = true;
    }

    protected void StartListener() {
        FirebaseDatabase.DefaultInstance
            .GetReference("Leaders").OrderByChild("score")
            .ValueChanged += (object sender2, ValueChangedEventArgs e2) => {
            if (e2.DatabaseError != null) {
                Debug.LogError(e2.DatabaseError.Message);
                return;
            }
            Debug.Log("Received values for Leaders.");
            string title = leaderBoard[0].ToString();
            leaderBoard.Clear();
            leaderBoard.Add(title);
            if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0) {
                foreach (var childSnapshot in e2.Snapshot.Children) {
                    if (childSnapshot.Child("score") == null
                        || childSnapshot.Child("score").Value == null) {
                        Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                        break;
                    } else {
                        Debug.Log("Leaders entry : " +
                                  childSnapshot.Child("email").Value.ToString() + " - " +
                                  childSnapshot.Child("score").Value.ToString());
                        leaderBoard.Insert(1, childSnapshot.Child("score").Value.ToString()
                                           + "  " + childSnapshot.Child("email").Value.ToString());

                        // set leaderBoardText box with updated values
                        leaderBoardText.text = "";
                        foreach (string item in leaderBoard) {
                            leaderBoardText.text += "\n" + item;
                        }
                    }
                }
            }
        };
    }

    // Exit if escape (or back, on mobile) is pressed.
    protected virtual void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s) {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize) {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }
    }

    // A realtime database transaction receives MutableData which can be modified
    // and returns a TransactionResult which is either TransactionResult.Success(data) with
    // modified data or TransactionResult.Abort() which stops the transaction with no changes.
    TransactionResult AddScoreTransaction(MutableData mutableData) {
        List<object> leaders = mutableData.Value as List<object>;

        if (leaders == null) {
            leaders = new List<object>();
        } else if (mutableData.ChildrenCount >= MaxScores) {
            // If the current list of scores is greater or equal to our maximum allowed number,
            // we see if the new score should be added and remove the lowest existing score.
            long minScore = long.MaxValue;
            object minVal = null;
            foreach (var child in leaders) {
                if (!(child is Dictionary<string, object>))
                    continue;
                long childScore = (long)((Dictionary<string, object>)child)["score"];
                if (childScore < minScore) {
                    minScore = childScore;
                    minVal = child;
                }
            }
            // If the new score is lower than the current minimum, we abort.
            if (minScore > score) {
                return TransactionResult.Abort();
            }
            // Otherwise, we remove the current lowest to be replaced with the new score.
            leaders.Remove(minVal);
        }

        // Now we add the new score as a new entry that contains the email address and score.
        Dictionary<string, object> newScoreMap = new Dictionary<string, object>();
        newScoreMap["score"] = score;
        newScoreMap["email"] = email;
        leaders.Add(newScoreMap);

        // You must set the Value to indicate data at that location has changed.
        mutableData.Value = leaders;
        return TransactionResult.Success(mutableData);
    }

    public void AddScore() {
        score =  Int32.Parse(scoreText.text);
        email = nameText.text;
        
        if (score == 0 || string.IsNullOrEmpty(email)) {
            DebugLog("invalid score or email.");
            return;
        }
        DebugLog(String.Format("Attempting to add score {0} {1}",
                               email, score.ToString()));

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Leaders");

        DebugLog("Running Transaction...");
        // Use a transaction to ensure that we do not encounter issues with
        // simultaneous updates that otherwise might create more than MaxScores top scores.
        reference.RunTransaction(AddScoreTransaction)
            .ContinueWithOnMainThread(task => {
                    if (task.Exception != null) {
                        DebugLog(task.Exception.ToString());
                    } else if (task.IsCompleted) {
                        DebugLog("Transaction complete.");
                    }
                });
    }

    void GUIDisplayLeaders() {
        GUI.skin.box.fontSize = 36;
        GUILayout.BeginVertical(GUI.skin.box);

        foreach (string item in leaderBoard) {
            GUILayout.Label(item, GUI.skin.box, GUILayout.ExpandWidth(true));
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
}