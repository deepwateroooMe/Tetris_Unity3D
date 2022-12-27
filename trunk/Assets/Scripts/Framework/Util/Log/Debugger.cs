using System;
using UnityEngine;

namespace Framework.Util {

    public static class Debugger {

        public static int debugLevel = 0;
        public static Action<string> ShowErrorMessage;
        static string tempString = "";

        public static void Log(Func<string> f, int level) {
            if (level <= debugLevel) {
                tempString = f();
                Debug.Log(tempString);
                LoggerProvider.Temp.Write("{0}", tempString);
            }
        }

        public static Action<string> onHandleError;
        public static void LogError(Func<string> f, int level) {
            tempString = f();
            LoggerProvider.Error.Write("{0}", tempString);
            if (level <= debugLevel) {
                Debug.LogError(tempString);
                if (onHandleError != null) 
                    onHandleError(tempString);
                else if (ShowErrorMessage != null) 
                    ShowErrorMessage(tempString);
            }
        }
        public static void ShowError(Func<string> f) {
            Debug.LogError(f());
            if (onHandleError != null) 
                onHandleError(f());
        }
        
        public static void LogWarning(Func<string> f, int level) {
            if (level <= debugLevel) {
                tempString = f();
                Debug.LogWarning(tempString);
                LoggerProvider.Warning.Write("{0}", tempString);
            }
        }
        public static void Log(object s) {
            if (debugLevel > 0) 
                Debug.Log(s);
        }
        public static void Log(object s, UnityEngine.Object context) {
            if (debugLevel > 0) 
                Debug.Log(s, context);
        }
        public static void LogWarning(object s) {
            if (debugLevel > 0) 
                Debug.LogWarning(s);
        }
        public static void LogWarning(object message, UnityEngine.Object context) {
            if (debugLevel > 0) 
                Debug.LogWarning(message, context);
        }
        public static void LogError(object s) {
            if (debugLevel > 0)
                Debug.LogError(s);
        }
        public static void LogError(object message, UnityEngine.Object context) {
            if (debugLevel > 0) 
                Debug.LogError(message, context);
        }
    }
}