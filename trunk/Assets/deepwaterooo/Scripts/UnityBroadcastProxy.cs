using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.deepwaterooo.Scripts {
    
    public class UnityBroadcastProxy : AndroidJavaProxy {
        private const string TAG = "UnityBroadcastProxy";

        public delegate void BroadcastOnReceiveDelegate(AndroidJavaObject context, AndroidJavaObject intent);
        public BroadcastOnReceiveDelegate onReceiveDelegate;

        // public UnityBroadcastProxy() : base("com.unity3d.player.BroadcastReceiverInterface") { }
// 这里基类的名字必须得改对：        
        public UnityBroadcastProxy() : base("com.deepwaterooo.BroadcastReceiverInterface") { }

        public void onReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            Debug.Log(TAG + " onReceive()");
            Debug.Log(TAG + " (onReceiveDelegate != null): " + (onReceiveDelegate != null));
            if (onReceiveDelegate != null) 
                onReceiveDelegate(context, intent);
        }
    }
}
