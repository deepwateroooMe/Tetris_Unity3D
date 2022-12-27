using UnityEngine;

namespace DWater {

    public class EditorSDK : ISDK {
        private const string TAG = "EditorSDK";
        
        public EditorSDK () { }
        public void Init () {
            Debug.Log(TAG + " INIT");
        }
        public void sendMsg(string s) {
            Debug.Log(TAG + " SendMsg()");
        }
        public void ManagePlayers () {
            Debug.Log(TAG + " ManagePlayers()");
        }
        public int add(int v1, int v2) {
            Debug.Log(TAG + " add()");
            return 0;
        }
        public void ShowLogin () {
            Debug.Log(TAG + " ShowLogin");
        }
        public void Credits () {
            Debug.Log(TAG + " Credits");
        }
        public void Terms () {
            Debug.Log(TAG + " Terms");
        }
        public void Privacy () {
            Debug.Log(TAG + " Privacy");
        }
        public void Logout () {
            Debug.Log(TAG + " Logout");
        }
        public bool IsUserLoggedIn () {
            Debug.Log(TAG + " IsUserLoggedIn");
            return true;
        }
        public void GetProfileURL() {
            Debug.Log(TAG + " get url profile pic");
        }
        public void UploadFileWithName (string data, string name){
            Debug.Log(TAG + " upload file with name");
        }
        public void DownloadFileWithName (string name){
            Debug.Log(TAG + " downlaod file with name");
        }
    }
}