using System;
using System.Collections.Generic;
using System.IO;

namespace Framework.Util {

    [Serializable]
    public static class LoggerProvider {

        static Dictionary<string, ILogFile> ILogFiles = new Dictionary<string, ILogFile>();

        private static List<ILogFile> logList = new List<ILogFile>();
        public static List<ILogFile> LogList {
            get { return logList; }
            set { logList = value; }
        }
        
        public static string RootPath = "Log";

        public static ILogFile GetLogFile(string name) {
            if (!ILogFiles.ContainsKey(name)) {
                var result = new LogFile(Path.Combine(LogConstant.Rootpath, RootPath), name);
                ILogFiles[name] = result;
                LogList.Add(result);
                result.Name = name;
                return result;
            } else {
                return ILogFiles[name];
            }
        }
        public static void CloseAll() {
            foreach (var log in LogList) {
                log.CloseFile();
            }
        }
        public static ILogFile Login {
            get {
                return GetLogFile("Login");
            }
        }
        static ILogFile _Temp;
        public static ILogFile Temp {
            get {
                if (_Temp == null) {
                    _Temp = GetLogFile("Temp");
                }
                return _Temp;
            }
        }
        public static ILogFile Debug {
            get {
                return GetLogFile("Debug");
            }
        }
        public static ILogFile Fatel {
            get {
                return GetLogFile("Fatel");
            }
        }
        public static ILogFile Web {
            get {
                return GetLogFile("Web");
            }
        }

        public static ILogFile Error {
            get {
                return GetLogFile("Error");
            }
        }
        public static ILogFile ConnectionError {
            get {
                return GetLogFile("ConnectionError");
            }
        }
        static ILogFile _Warning;
        public static ILogFile Warning {
            get {
                if (_Warning == null) {
                    _Warning = GetLogFile("Warning");
                }
                return _Warning;
            }
        }
        public static ILogFile OnRecv {
            get {
                return GetLogFile("Recv");
            }
        }
        public static ILogFile OnSend {
            get {
                return GetLogFile("Send");
            }
        }
        public static ILogFile Connection {
            get {
                return GetLogFile("Connection");
            }
        }
    }
}