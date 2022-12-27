using System;
using System.IO;
using Framework.ResMgr;
using UnityEngine;

namespace Framework.Util {

    // 文件操作帮助类
    public static class FileHelp {
        // 可读文件目录
        public static string AssetBundleCacheRoot {
            get {
                return ResourceConstant.AssetBundleCacheRoot;
            }
        }
        // 只读文件目录
        public static string AssetBundleReadOnlyRoot {
            get {
                return ResourceConstant.AssetBundleReadOnlyRoot;
            }
        }

#region Read
        // 读取字节数组
        public static byte[] Read(string fileName) {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            if (!File.Exists(filePath)) {
                filePath = Path.Combine(AssetBundleReadOnlyRoot, fileName);
            }
            if (File.Exists(filePath)) {
                using (var fileStream = File.OpenRead(filePath)) {
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    return bytes;
                }
            } else {
#if UNITY_ANDROID
                using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                        byte[] bytes;
                        try {
                            bytes = javaObject.Call<byte[]>("GetFileBytes", fileName);
                        } catch (Exception e) {
                            return null;
                        }
                        return bytes;
                    }
                }
#endif
                return null;
            }
        }
        // 读取字符串
        public static string ReadString(string fileName) {
            byte[] data = Read(fileName);
            if (data != null) {
                string str = System.Text.Encoding.UTF8.GetString(data);
                return str;
            }
            return null;
        }
#endregion
        
#region Write
        // 往文件中写字节数组
        public static void WriteByte(string fileName, byte[] data, int startIndex, int length) {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath)) 
                Directory.CreateDirectory(dirPath);
            using (var fileStream = File.Open(fileName, FileMode.Create)) {
                fileStream.Write(data, startIndex, length);
            }
        }
        // 往文件中写入字符串
        public static void WriteString(string fileName, string content) {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath)) 
                Directory.CreateDirectory(dirPath);
            using (var fileStream = File.Open(fileName, FileMode.Create)) {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
#endregion
        // 文件是否存在
        public static bool IsFileExists(string fileName) {
            string cachePath = Path.Combine(AssetBundleCacheRoot, fileName);
            if (File.Exists(cachePath)) {
                return true;
            } else {
                cachePath = Path.Combine(AssetBundleReadOnlyRoot, fileName);
            }
            if (File.Exists(cachePath)) {
                return true;
            } else 
                return false;
        }
    }
}
