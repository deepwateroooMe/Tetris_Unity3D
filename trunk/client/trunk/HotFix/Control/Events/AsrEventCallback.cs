using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public class AsrEventCallback : AndroidJavaProxy {
        private const string TAG = "AsrEventCallback";

        public AsrEventCallback() : base("com.deepwaterooo.dwsdk.UnityasrEventCallback") {  }

        public void onIntValReady(int val, string type) {
            Debug.Log(TAG + " onIntValReady()");
            Debug.Log(TAG + " val: " + val);
            Debug.Log(TAG + " type: " + type);
            if (type == "max")
                VolumeManager.Instance.maxVol.Value = val;
            else 
                VolumeManager.Instance.curVol.Value = val;
        }
        public void Speechcontent(int content) {
            Debug.Log(TAG + " Speechcontent() content: " + content);
            int a = content;
        }
        public void Test1(string msg) {
            Debug.Log(TAG + " Test1() msg: " + msg);
            string b = msg;
        }
    }    
}
