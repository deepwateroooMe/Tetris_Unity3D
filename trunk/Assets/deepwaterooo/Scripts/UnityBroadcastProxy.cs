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

        public UnityBroadcastProxy() : base("com.deepwaterooo.sdk.utils.BroadcastReceiverInterface") { }

        public void onReceive(AndroidJavaObject context, AndroidJavaObject intent) {
            Debug.Log(TAG + " onReceive() (onReceiveDelegate != null): " + (onReceiveDelegate != null));
            if (onReceiveDelegate != null) 
                onReceiveDelegate(context, intent);
        }
    }
}
