namespace tetris3d {

    using Firebase.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class LoginHandler : MonoBehaviour {
        private const string TAG = "LoginHandler";

        public Text emailText;
        public Text passwordText;
        public Text emailInfo;
        public Text passwordInfo;

        private int signInMethod = 0;
    
        protected Firebase.Auth.FirebaseAuth auth;
        protected Firebase.Auth.FirebaseAuth otherAuth; 

        protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();

        protected string email = "";
        protected string password = "";
        protected string displayName = "";

        // Whether to sign in / link or reauthentication *and* fetch user profile data.
        protected bool signInAndFetchProfile = false;

        // Flag set when a token is being fetched.  This is used to avoid printing the token
        // in IdTokenChanged() when the user presses the get token button.
        private bool fetchingToken = false;
    
        // Enable / disable password input box.
        bool UIEnabled = true;
    
        // Options used to setup secondary authentication object.
        private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions {
            ApiKey = "",
            AppId = "",
            ProjectId = ""
        };
    
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

        public virtual void Start() {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                    dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available) {
                        InitializeFirebase();
                    } else {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
        }

        protected void InitializeFirebase() {
            Debug.Log("Setting up Firebase Auth");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            auth.IdTokenChanged += IdTokenChanged;

            // Specify valid options to construct a secondary authentication object.
            if (otherAuthOptions != null &&
                !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
                  String.IsNullOrEmpty(otherAuthOptions.AppId) ||
                  String.IsNullOrEmpty(otherAuthOptions.ProjectId))) {
                try {
                    otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(otherAuthOptions, "Secondary"));
                    otherAuth.StateChanged += AuthStateChanged;
                    otherAuth.IdTokenChanged += IdTokenChanged;
                } catch (Exception) {
                    Debug.Log("ERROR: Failed to initialize secondary authentication object.");
                }
            }  
            AuthStateChanged(this, null);  
        }

        // Email Login Button Clicked
        public void SigninWithEmailCredentialAsync() {
            email = emailText.text;
            password = passwordText.text;
            if (email == "") {
                emailInfo.text = "An email address must be provided.";
                passwordInfo.text = "";
                return;
            } else if (password == "") {
                emailInfo.text = "";
                passwordInfo.text = "A password must be provided.";
                return;
            }//else if (password.text.length() < ) // 这个我也需要提示一下
            signInMethod = 1;
        
            Debug.Log(String.Format("Attempting to sign in as {0}...", email));
            DisableUI();
            if (signInAndFetchProfile) {
                //return auth.SignInAndRetrieveDataWithCredentialAsync(
                auth.SignInAndRetrieveDataWithCredentialAsync(
                    Firebase.Auth.EmailAuthProvider.GetCredential(email, password))
                    //.ContinueWithOnMainThread(HandleSignInWithSignInResult);
                    .ContinueWithOnMainThread(task => {
                            EnableUI();
                            SceneManager.LoadScene("RoalUserMainMenu");
                        });
            } else {
                //return auth.SignInWithCredentialAsync(
                auth.SignInWithCredentialAsync(
                    Firebase.Auth.EmailAuthProvider.GetCredential(email, password))
                    .ContinueWithOnMainThread(HandleSignInWithUser);  // HandleSignInWithUser ori HandleSigninResule
            }
        }

        // "Go Anonymous" button clicked: Attempt to sign in anonymously.
        public void SigninAnonymouslyAsync() {
            Debug.Log("Attempting to sign anonymously...");
            DisableUI();
            signInMethod = 2;
        
            //return auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(HandleSignInWithUser);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(HandleSignInWithUser);
        }

        // Create New User: create a user with the email and password.
        public void CreateUserWithEmailAsync() {
            email = emailText.text;
            password = passwordText.text;
            if (email == "") {
                emailInfo.text = "An email address must be provided.";
                passwordInfo.text = "";
                return;
            } else if (password == "") {
                emailInfo.text = "";
                passwordInfo.text = "A password must be provided.";
                return;
            }
            //else if (password.text.length() < )
            signInMethod = 3;
        
            Debug.Log(String.Format("Attempting to create user {0}...", email));
            DisableUI();

            // This passes the current displayName through to HandleCreateUserAsync
            // so that it can be passed to UpdateUserProfile().  displayName will be
            // reset by AuthStateChanged() when the new user is created and signed in.
            string newDisplayName = displayName;
            auth.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread((task) => {
                        EnableUI();
                        if (LogTaskCompletion(task, "User Creation")) {
                            var user = task.Result;
                            DisplayDetailedUserInfo(user, 1);
                            return UpdateUserProfileAsync(newDisplayName: newDisplayName); // UpdateUserProfileAsync ori 4.5.0: HandleCreateUserAsync 
                        }
                        return task;
                    }).Unwrap();
        }
        
        // Called when a sign-in without fetching profile data completes.
        void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task) {
            EnableUI();
            if (LogTaskCompletion(task, "Sign-in")) {
                Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
                switch (signInMethod) {
                case 1: // Email
                    SceneManager.LoadScene("RoyalUserMainMenu");
                    break;
                case 2: // Anonymous
                case 3:
                    SceneManager.LoadScene("Main");
                    break;
                }
            }
            //SceneManager.LoadSceneAsync("RoyalUserMainMenu");
        }

        // FirebaseAuth.getInstance().getCurrentUser().sendEmailVerification()
        // FirebaseAuth.getInstance().getCurrentUser().isEmailVerified()
        
        // Email Password SignUP Email Verification example: Android Java 
        // https://stackoverflow.com/questions/40404567/how-to-send-verification-email-with-firebase
        // private void sendVerificationEmail() {
        //         FirebaseUser user = FirebaseAuth.getInstance().getCurrentUser();
        //         user.sendEmailVerification()
        //             .addOnCompleteListener(new OnCompleteListener<Void>() {
        //                 @Override
        //                 public void onComplete(@NonNull Task<Void> task) {
        //                 if (task.isSuccessful()) {
        //                     // email sent

        //                     // after email is sent just logout the user and finish this activity
        //                     FirebaseAuth.getInstance().signOut();
        //                     startActivity(new Intent(SignupActivity.this, LoginActivity.class));
        //                     finish();
        //                 } else {
        //                     // email not sent, so display message and restart the activity or do whatever you wish to do
        //                     //restart this activity
        //                     overridePendingTransition(0, 0);
        //                     finish();
        //                     overridePendingTransition(0, 0);
        //                     startActivity(getIntent());
        //                 }
        //             }
        //         });
        // }        
        
        // Exit if escape (or back, on mobile) is pressed.
        protected virtual void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }

        void OnDestroy() {
            if (auth != null) {
                auth.StateChanged -= AuthStateChanged;
                auth.IdTokenChanged -= IdTokenChanged;
                auth = null;
            }
            if (otherAuth != null) {
                otherAuth.StateChanged -= AuthStateChanged;
                otherAuth.IdTokenChanged -= IdTokenChanged;
                otherAuth = null;
            }  
        }

        void DisableUI() {
            UIEnabled = false;
        }

        void EnableUI() {
            UIEnabled = true;
        }
     
        // Track state changes of the auth object.
        // 设置身份验证状态更改事件处理程序并获取用户数据
        // 要响应帐号登录和退出事件，请将事件处理程序附加到全局身份验证对象。
        // 每当用户的登录状态发生变化时，系统都会调用此处理程序。
        // 处理程序仅在身份验证对象完全初始化且所有网络调用完成后才运行，因此它是获取登录用户信息的最佳位置。
        void AuthStateChanged(object sender, System.EventArgs eventArgs) {
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            Firebase.Auth.FirebaseUser user = null;
            if (senderAuth != null)
                userByAuth.TryGetValue(senderAuth.App.Name, out user);

            if (senderAuth == auth && senderAuth.CurrentUser != user) {
                bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
                if (!signedIn && user != null) {
                    Debug.Log("Signed out " + user.UserId);
                }
                user = senderAuth.CurrentUser;
                userByAuth[senderAuth.App.Name] = user;
                if (signedIn) {
                    Debug.Log("Signed in " + user.UserId);
                    displayName = user.DisplayName ?? "";
                    DisplayDetailedUserInfo(user, 1);

                    EnableUI();
                    switch (signInMethod) {
                    case 1: // Email
                        SceneManager.LoadScene("RoyalUserMainMenu");
                        break;
                    case 2: // Anonymous
                    case 3:
                        SceneManager.LoadScene("Main");
                        break;
                    }
                }
            }
        }

        // Update the user's display name with the currently selected display name.
        public Task UpdateUserProfileAsync(string newDisplayName = null) {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to update user profile");
                return Task.FromResult(0);
            }
            displayName = newDisplayName ?? displayName;
            Debug.Log("Updating user profile");
            DisableUI();
            return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile {
                    DisplayName = displayName,
                        PhotoUrl = auth.CurrentUser.PhotoUrl,
                        }).ContinueWithOnMainThread(task => {
                                EnableUI();
                                if (LogTaskCompletion(task, "User profile")) {
                                    DisplayDetailedUserInfo(auth.CurrentUser, 1);
                                }
                            });
        }

        // Track ID token changes.
        void IdTokenChanged(object sender, System.EventArgs eventArgs) {
            Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
            if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken) {
                senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
                    task => Debug.Log(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
            }
        }

        // Log the result of the specified task, returning true if the task
        // completed successfully, false otherwise.
        protected bool LogTaskCompletion(Task task, string operation) {
            bool complete = false;
            if (task.IsCanceled) {
                Debug.Log(operation + " canceled.");
            } else if (task.IsFaulted) {
                Debug.Log(operation + " encounted an error.");
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
                    string authErrorCode = "";
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null) {
                        authErrorCode = String.Format("AuthError.{0}: ",
                                                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                    }
                    Debug.Log(authErrorCode + exception.ToString());
                }
            } else if (task.IsCompleted) {
                Debug.Log(operation + " completed");
                complete = true;
            }
            return complete;
        }

        // Link the current user with an email / password credential.
        protected Task LinkWithEmailCredentialAsync() {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to link credential to user.");
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(new Exception("Not signed in"));
                return tcs.Task;
            }
            Debug.Log("Attempting to link credential to user...");
            Firebase.Auth.Credential cred =
                Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            if (signInAndFetchProfile) {
                return
                    auth.CurrentUser.LinkAndRetrieveDataWithCredentialAsync(cred).ContinueWithOnMainThread(
                        task => {
                            if (LogTaskCompletion(task, "Link Credential")) {
                                DisplaySignInResult(task.Result, 1);
                            }
                        }
                        );
            } else {
                return auth.CurrentUser.LinkWithCredentialAsync(cred).ContinueWithOnMainThread(task => {
                        if (LogTaskCompletion(task, "Link Credential")) {
                            DisplayDetailedUserInfo(task.Result, 1);
                        }
                    });
            }
        }

        // Reauthenticate the user with the current email / password.
        protected Task ReauthenticateAsync() {
            var user = auth.CurrentUser;
            if (user == null) {
                Debug.Log("Not signed in, unable to reauthenticate user.");
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(new Exception("Not signed in"));
                return tcs.Task;
            }
            Debug.Log("Reauthenticating...");
            DisableUI();
            Firebase.Auth.Credential cred = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
            if (signInAndFetchProfile) {
                return user.ReauthenticateAndRetrieveDataAsync(cred).ContinueWithOnMainThread(task => {
                        EnableUI();
                        if (LogTaskCompletion(task, "Reauthentication")) {
                            DisplaySignInResult(task.Result, 1);
                        }
                    });
            } else {
                return user.ReauthenticateAsync(cred).ContinueWithOnMainThread(task => {
                        EnableUI();
                        if (LogTaskCompletion(task, "Reauthentication")) {
                            DisplayDetailedUserInfo(auth.CurrentUser, 1);
                        }
                    });
            }
        }

        // Reload the currently logged in user.
        public void ReloadUser() {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to reload user.");
                return;
            }
            Debug.Log("Reload User Data");
            auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task => {
                    if (LogTaskCompletion(task, "Reload")) {
                        DisplayDetailedUserInfo(auth.CurrentUser, 1);
                    }
                });
        }

        // Fetch and display current user's auth token.
        public void GetUserToken() {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to get token.");
                return;
            }
            Debug.Log("Fetching user token");
            fetchingToken = true;
            auth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(task => {
                    fetchingToken = false;
                    if (LogTaskCompletion(task, "User token fetch")) {
                        Debug.Log("Token = " + task.Result);
                    }
                });
        }

        // Display information about the currently logged in user.
        void GetUserInfo() {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to get info.");
            } else {
                Debug.Log("Current user info:");
                DisplayDetailedUserInfo(auth.CurrentUser, 1);
            }
        }

        // Unlink the email credential from the currently logged in user.
        protected Task UnlinkEmailAsync() {
            if (auth.CurrentUser == null) {
                Debug.Log("Not signed in, unable to unlink");
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(new Exception("Not signed in"));
                return tcs.Task;
            }
            Debug.Log("Unlinking email credential");
            DisableUI();
            return auth.CurrentUser.UnlinkAsync(
                Firebase.Auth.EmailAuthProvider.GetCredential(email, password).Provider)
                .ContinueWithOnMainThread(task => {
                        EnableUI();
                        LogTaskCompletion(task, "Unlinking");
                    });
        }

        // Sign out the current user.
        protected void SignOut() {
            Debug.Log("Signing out.");
            auth.SignOut();
        }

        // Show the providers for the current email address.
        protected void DisplayProvidersForEmail() {
            auth.FetchProvidersForEmailAsync(email).ContinueWithOnMainThread((authTask) => {
                    if (LogTaskCompletion(authTask, "Fetch Providers")) {
                        Debug.Log(String.Format("Email Providers for '{0}':", email));
                        foreach (string provider in authTask.Result) {
                            Debug.Log(provider);
                        }
                    }
                });
        }

        // Send a password reset email to the current email address.
        protected void SendPasswordResetEmail() {
            auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread((authTask) => {
                    if (LogTaskCompletion(authTask, "Send Password Reset Email")) {
                        Debug.Log("Password reset email sent to " + email);
                    }
                });
        }

        // Determines whether another authentication object is available to focus.
        protected bool HasOtherAuth { get { return auth != otherAuth && otherAuth != null; } }

        // Swap the authentication object currently being controlled by the application.
        protected void SwapAuthFocus() {
            if (!HasOtherAuth) return;
            var swapAuth = otherAuth;
            otherAuth = auth;
            auth = swapAuth;
            Debug.Log(String.Format("Changed auth from {0} to {1}", otherAuth.App.Name, auth.App.Name));
        }

        
        // Display additional user profile information.
        protected void DisplayProfile<T>(IDictionary<T, object> profile, int indentLevel) {
            string indent = new String(' ', indentLevel * 2);
            foreach (var kv in profile) {
                var valueDictionary = kv.Value as IDictionary<object, object>;
                if (valueDictionary != null) {
                    Debug.Log(String.Format("{0}{1}:", indent, kv.Key));
                    DisplayProfile<object>(valueDictionary, indentLevel + 1);
                } else {
                    Debug.Log(String.Format("{0}{1}: {2}", indent, kv.Key, kv.Value));
                }
            }
        }

        // Display user information reported
        protected void DisplaySignInResult(Firebase.Auth.SignInResult result, int indentLevel) {
            string indent = new String(' ', indentLevel * 2);
            DisplayDetailedUserInfo(result.User, indentLevel);
            var metadata = result.Meta;
            if (metadata != null) {
                Debug.Log(String.Format("{0}Created: {1}", indent, metadata.CreationTimestamp));
                Debug.Log(String.Format("{0}Last Sign-in: {1}", indent, metadata.LastSignInTimestamp));
            }
            var info = result.Info;
            if (info != null) {
                Debug.Log(String.Format("{0}Additional User Info:", indent));
                Debug.Log(String.Format("{0}  User Name: {1}", indent, info.UserName));
                Debug.Log(String.Format("{0}  Provider ID: {1}", indent, info.ProviderId));
                DisplayProfile<string>(info.Profile, indentLevel + 1);
            }
        }

        // Display user information.
        protected void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel) {
            string indent = new String(' ', indentLevel * 2);
            var userProperties = new Dictionary<string, string> { {"Display Name", userInfo.DisplayName}, {"Email", userInfo.Email}, {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null}, {"Provider ID", userInfo.ProviderId}, {"User ID", userInfo.UserId}
            };
            foreach (var property in userProperties) {
                if (!String.IsNullOrEmpty(property.Value)) {
                    Debug.Log(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
                }
            }
        }

        // Display a more detailed view of a FirebaseUser.
        protected void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel) {
            string indent = new String(' ', indentLevel * 2);
            DisplayUserInfo(user, indentLevel);
            Debug.Log(String.Format("{0}Anonymous: {1}", indent, user.IsAnonymous));
            Debug.Log(String.Format("{0}Email Verified: {1}", indent, user.IsEmailVerified)); // Email Verified: False
            Debug.Log(String.Format("{0}Phone Number: {1}", indent, user.PhoneNumber));
            var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
            var numberOfProviders = providerDataList.Count;
            if (numberOfProviders > 0) {
                for (int i = 0; i < numberOfProviders; ++i) {
                    Debug.Log(String.Format("{0}Provider Data: {1}", indent, i));
                    DisplayUserInfo(providerDataList[i], indentLevel + 2);
                }
            }
        }

        // // Delete the currently logged in user.   // Said: to only delete our use from our database ???
        // protected Task DeleteUserAsync() {
        //     if (auth.CurrentUser != null) {
        //         Debug.Log(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
        //         DisableUI();
        //         return auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task => {
        //                 EnableUI();
        //                 LogTaskCompletion(task, "Delete user");
        //             });
        //     } else {
        //         Debug.Log("Sign-in before deleting user.");
        //         // Return a finished task.
        //         return Task.FromResult(0);
        //     }
        // }  
    }
}