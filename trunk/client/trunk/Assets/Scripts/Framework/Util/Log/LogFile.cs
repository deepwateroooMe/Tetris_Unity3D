using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using System.Reflection;

namespace Framework.Util {

    [Serializable]
    public class LogFile : ILogFile {

        int maxCache = 300;
        int _maxLength = 10 * 1024 * 1024;
        // int _maxLength = 1 * 1024;
        int index = 1;
        string baseName;
        string baseDir;
        FileStream _fs;
        StreamWriter _sw;
        DateTime _fileTime;

        public static Action<string> OnWriteAnyLine;
        public Action<string> OnWriteLine {
            get;
            set;
        }
        public Queue<string> Cache {
            get;
            set;
        }
        public string FileName {
            get {
                return baseName + DateTime.Now.ToString("yyyyMMdd");
            }
        }
        public string Name {
            get;
            set;
        }
        
        public LogFile(string strDir, string name) {
            try {
                baseName = name;
                baseDir = Path.Combine(strDir, name);
                Cache = new Queue<string>();
            } catch (Exception e) {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        void ZipOld(int maxIndex) {
            while (index < maxIndex) {
                CloseFile(true);
                index++;
                OpenFile();
            }
        }

        void CheckZip() {
            if (_fileTime.Day != DateTime.Now.Day) {
                CloseFile(true);
                index = 1;
                OpenFile();
            }
            if (_fs != null && _fs.Length >= _maxLength) {
                CloseFile(true);
                index++;
                OpenFile();
            }
        }

        public void ChekDir() {
            var dir = Path.Combine(baseDir, baseName + DateTime.Now.ToString("yyyyMM"));
            if (Directory.Exists(dir)) {
                // DirectoryInfo di = new DirectoryInfo(dir);
                var fis = Directory.GetFiles(dir);
                if (fis.Any(p => p.Contains(FileName))) {
                    var tis = fis.Select(p => {
                        //                        LogManager.Instance.AddList("fname="+p);
                        var matche = Regex.Match(p, @"\d{6,8}\-(?<index>.*?)\.log");
                        string ts = matche.Groups["index"].Value;
                        //                        LogManager.Instance.AddList("matchs="+ts);
                        int rv = 0;
                        if (int.TryParse(ts, out rv)) 
                            return rv;
                        else 
                            return 0;
                    }).ToList();
                    if (tis.Count > 0) {
                        var max = tis.Max();
                        ZipOld(max);
                        index = max;
                    }
                }
            }
        }

        public void OpenFile() {
            try {
                if (_fs != null) 
                    CloseFile();
                var dirPerMonth = Path.Combine(baseDir, baseName + DateTime.Now.ToString("yyyyMM"));
                if (!Directory.Exists(dirPerMonth))
                    Directory.CreateDirectory(dirPerMonth);
                string filePath = Path.Combine(dirPerMonth, FileName + "-" + index + ".log");
                // LogManager.AddList(filePath);
                _fs = new FileStream(filePath, FileMode.Append);
                _sw = new StreamWriter(_fs);
                _sw.AutoFlush = true;
                // _fileTime = File.GetCreationTime(filePath);    
                _fileTime = DateTime.Now;
                // UnityEngine.Debug.LogWarning("OpenFile " + filePath);
            } catch (Exception e) {
                Debugger.ShowError(() => e.Message + e.StackTrace);
            }
        }

        public void CloseFile(bool zipOld = false) {
            if (_sw != null)
                _sw.Close();
            if (_fs != null)
                _fs.Close();
            if (zipOld) {
                var dirPerMonth = Path.Combine(baseDir, baseName + DateTime.Now.ToString("yyyyMM"));
                ThisZipFile(Path.Combine(dirPerMonth, FileName + "-" + index));
            }
        }
        public bool CanWrite {
            get {
                return Application.platform == RuntimePlatform.WindowsPlayer
                    || Application.platform == RuntimePlatform.WindowsEditor
                    || Application.platform == RuntimePlatform.OSXEditor
                    || Application.platform == RuntimePlatform.Android;
            }
        }
        public void Write(string FormatString, params object[] args) {
            try {
                if (CanWrite) {
                    if (_sw == null) {
                        ChekDir();
                        OpenFile();
                    }
                    CheckZip();
                }
                if (_sw == null) 
                    return;
                WriteLine("[{0}]{1}", DateTime.Now.ToString("MM月dd日HH:mm:ss.fff"), string.Format(CultureInfo.InvariantCulture, FormatString, args));
            } catch (Exception e) {
                Debugger.ShowError(() => string.Format("[{0}];WriteLog Exception!: [{1}]\n[{2}][{3}]", DateTime.Now, FormatString, e.Message, e.StackTrace));
                WriteLine(e.Message + e.StackTrace + ":" + FormatString);
            } finally {
            }
        }
        void WriteLine(string FormatString, params object[] args) {
            var s = string.Format(CultureInfo.CurrentCulture, FormatString, args);
            WriteLine(s);
        }

        public static string GetStackNamesString() {
            try {
                StringBuilder sb = new StringBuilder();
                StackTrace st = new StackTrace();
                StackFrame[] frames = st.GetFrames();
                for (int i = 1; i < frames.Length; i++) {
                    MethodBase method = frames[i].GetMethod();
                    if (method != null) {
                        if (method.Name == "InvokeCommand")
                            break;
                        sb.Append(method.Name);
                        sb.Append("<-");
                    }
                }
                sb.Remove(sb.Length - 2, 2);
                return sb.ToString();
            } catch (Exception e) {
                Debugger.ShowError(() => e.Message + e.StackTrace);
                return "";
            }
        }

        void WriteLine(string content) {
            try {
                if (OnWriteAnyLine != null) 
                    OnWriteAnyLine(content);
                if (OnWriteLine != null) 
                    OnWriteLine(content);
                Cache.Enqueue(content);
                if (Cache.Count >= maxCache) 
                    Cache.Dequeue();
                if (CanWrite) 
                    _sw.WriteLine(content);
                else 
                    Debugger.ShowError(() => baseName + ":" + content + GetStackNamesString());
            } catch (Exception e) {
                Debugger.ShowError(() => e.Message + e.StackTrace + content);
            }
        }

        private bool ThisZipFile(string temppath) { // 打包文件 
            try {
                FileInfo tfi = new FileInfo(temppath + ".log");
                lock (tfi) {
                    if (!tfi.Exists)
                        return false;
                    // ZipFile.CompressFile(temppath + ".log", temppath + ".zip");
                    CompressExtension.Compress(tfi);
                    // tfi.Delete();
                    File.Delete(tfi.FullName);
                    return true;
                }
            } catch (Exception e) {
                Debugger.ShowError(() => e.Message + e.StackTrace);
                return false;
            }
        }
    }
}
