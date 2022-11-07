using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Framework.Util {

// 想要日志文件,但是不需要那堆乱七八糟的stacktrace 日志,这个也不可以    
    public class Me : MonoBehaviour { 
        private const string TAG = "Me"; 

        void Start () {
                Log ("Aaaaarrrrgh!");
            }

// 只在Editor里面没有显示更多的内容,可是文件里面还是有很多的内容,很不方便,不起作用,不是真正想要的        
        public static void Log (string message) {
                typeof (Debug).GetMethod (
                    "LogPlayerBuildError",
                    BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic
                    ).Invoke (
                        null,
                        new object[]{message, "", 0, 0}
                        );
        }
    }
}

