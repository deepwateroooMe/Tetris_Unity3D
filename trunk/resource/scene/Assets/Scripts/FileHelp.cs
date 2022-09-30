using System;
using System.IO;
using Framework.ResMgr;
using UnityEngine;

namespace Framework.Util
{

    /// <summary>
    /// 文件操作帮助类///
    /// </summary>
    public static class FileHelp
    {
        /// <summary>
        /// 可读文件目录///
        /// </summary>
        public static string AssetBundleCacheRoot
        {
            get
            {
                return ResourceConstant.AssetBundleCacheRoot;
            }
        }

        /// <summary>
        /// 只读文件目录///
        /// </summary>
        public static string AssetBundleReadOnlyRoot
        {
            get
            {
                return ResourceConstant.AssetBundleReadOnlyRoot;
            }
        }

        #region Read

        /// <summary>
        /// 读取字节数组///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] Read(string fileName)
        {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(AssetBundleReadOnlyRoot, fileName);
            }
            if (File.Exists(filePath))
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    return bytes;
                }
            }
            else
            {
#if UNITY_ANDROID
                using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        byte[] bytes;
                        try
                        {
                            bytes = javaObject.Call<byte[]>("GetFileBytes", fileName);
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
                        return bytes;
                    }

                }
#endif
                return null;
            }
        }

        /// <summary>
        /// 读取字符串///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadString(string fileName)
        {
            byte[] data = Read(fileName);
            if (data != null)
            {
                string str = System.Text.Encoding.UTF8.GetString(data);
                return str;
            }
            return null;
        }

        #endregion

        #region Write

        /// <summary>
        /// 往文件中写字节数组///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public static void WriteByte(string fileName, byte[] data, int startIndex, int length)
        {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using (var fileStream = File.Open(fileName, FileMode.Create))
            {
                fileStream.Write(data, startIndex, length);
            }
        }

        /// <summary>
        /// 往文件中写入字符串///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void WriteString(string fileName, string content)
        {
            string filePath = Path.Combine(AssetBundleCacheRoot, fileName);
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using (var fileStream = File.Open(fileName, FileMode.Create))
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        #endregion

        /// <summary>
        /// 文件是否存在///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileExists(string fileName)
        {
            string cachePath = Path.Combine(AssetBundleCacheRoot, fileName);
            if (File.Exists(cachePath))
            {
                return true;
            }
            else
            {
                cachePath = Path.Combine(AssetBundleReadOnlyRoot, fileName);
            }
            if (File.Exists(cachePath))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
