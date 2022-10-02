using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Util {

    // 信息调试接口类
    public static class DebugHelper {

        // Debug信息
        public static void Log(string content, bool writeToFile = false) {
            Debug.Log(content);
            if (writeToFile) 
                LoggerProvider.Debug.Write(content);
        }

        // Error信息
        public static void LogError(string content, bool writeToFile = false) {
            Debug.LogError(content);
            if (writeToFile) 
                LoggerProvider.Error.Write(content);
        }
    }
}
