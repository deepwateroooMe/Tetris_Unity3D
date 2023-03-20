using Framework.Core;
using System.IO;
using UnityEngine;
using Framework.MVVM;

// AssetBundle来源枚举: 定义了几个最基本的资源包来源以便区分对待
public enum EAssetBunbleSourceType {
    Primary = 0,
    Hotfix = 1,
    Server = 2
}

// AssetBundle包卸载层级
public enum EAssetBundleUnloadLevel {
    None = -1,
    Never = 0,
    LoadOver = 1,
    ChangeSceneOver = 2
}

namespace Framework.ResMgr {

    public static class ResourceConstant {
        private const string TAG = "ResourceConstant";
// 这个跨程序域级别的公用接口可以连接两个不同程序域,方便热更新调用unity里的函数资源等        
        public static IResourceLoader Loader {
            get;
            set;
        }
        public static readonly string bundleExtension = ".ab";
        // static string _resourceWebRoot = "http://localhost/"; // 本地资源复制资源包到特定目录下，没有问题。现测试，本地服务器
        static string _resourceWebRoot = "http://127.0.0.1:8080/"; // 本地资源复制资源包到特定目录下，没有问题。现测试，本地服务器
        // 资源服务器路径
        public static string ResourceWebRoot {
            get { return _resourceWebRoot; }
            set { _resourceWebRoot = value; }
        }
        // 是否缓存AssetBundle
        public static bool CacheAssetBundle = true;

        // 只读目录: 不同平台下的地址也略有不同
        public static string AssetBundleReadOnlyRoot {
            get {
                string path = string.Empty;
#if UNITY_EDITOR
                path = GetEditorAssetPath();
#elif UNITY_STANDALONE_WIN
                path = Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
                path = Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
                path = Application.dataPath + "/Raw/";
#endif
                return path;
            }
        }

        // 可写更新目录
        public static string AssetBundleCacheRoot {
            get {
                string path = string.Empty;
#if UNITY_EDITOR
                path = GetEditorAssetPath();
#elif UNITY_STANDALONE_WIN
                path = Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
                path = Application.persistentDataPath;
#elif UNITY_IPHONE
                path = Application.persistentDataPath;
#endif  // 要把这个字符串打印一遍
                Debug.Log(TAG + " AssetBundleCacheRoot() path: " + path);
                return path;
            }
        }
        public static string AssetBundleUrl {
            get {
                string path = string.Empty;
#if UNITY_EDITOR
                path = GetEditorAssetBundleUrl();
#elif UNITY_STANDALONE_WIN
                path = "file:///" + Application.dataPath + "/TempStreamingAssets/";
#elif UNITY_ANDROID
                path = Application.streamingAssetsPath;
#elif UNITY_IPHONE
                path = "file://" + Application.dataPath + "/Raw/";
#endif
                return path;
            }
        }
        public static string AssetBundleCacheUrl {
            get {
                string path = string.Empty;
#if UNITY_EDITOR
                path = GetEditorAssetBundleUrl();
#elif UNITY_STANDALONE_WIN
                path = "file:///" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
                path = "file://" + Application.persistentDataPath + "/";
#elif UNITY_IPHONE
                path = "file://" + Application.persistentDataPath + "/";
#endif
                return path;
            }
        }
        // 资源文件服务器：资源包在服务器中的路径，分平台。【因为我参考斗地主游戏，所以服务器端也分平台来布置不同平台的热更新资源包】
        public static string RemoteAssetBundleUrl { // 这里改成与斗地主游戏一样 PC
            get {
                string path = ResourceWebRoot;
#if UNITY_EDITOR 
                if (!GameApplication.Instance.useLocal) { // 不用本地资源包
                    if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android) {
                        path += "Android/";
                    } else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS) {
                        path += "IOS/";
                    } else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows || UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows64) {
                        path += "PC/"; // 自已改的
                        // path += "Windows/"; // 原始的
                    }
                } else {
                    path = GetEditorAssetPath();
                }
// 服务器上的配置,是根据客户端的平台来的.我的项目只需要一个笼统的一个安卓就可以了
#elif UNITY_STANDALONE_WIN
                path += "PC/";
                // path += "Windows/"; //ori
#elif UNITY_ANDROID
                path += "Android/";
#elif UNITY_IPHONE
                path += "IOS/";
#endif
                return path;
            }
        }
        // 获取文件路径
        public static string GetFileUrl(string name, out bool isInCacheFile) {
            var cachePath = Path.Combine(AssetBundleCacheRoot, name);
            if (File.Exists(cachePath)) {
                isInCacheFile = true;
                return Path.Combine(AssetBundleCacheUrl, name);
            }
            if (Application.platform == RuntimePlatform.Android) {
                string filePath = Path.Combine(AssetBundleUrl, name);
                var www = new WWW(filePath);
                if (string.IsNullOrEmpty(www.error)) {
                    isInCacheFile = false;
                    return Path.Combine(AssetBundleUrl, name);
                }
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                var filePath = Path.Combine(AssetBundleReadOnlyRoot, name);
                if (File.Exists(filePath)) {
                    isInCacheFile = true;
                    return Path.Combine(AssetBundleUrl, name);
                }
            } else {
                var filePath = Path.Combine(AssetBundleReadOnlyRoot, name);
                if (File.Exists(filePath)) {
                    isInCacheFile = false;
                    return Path.Combine(AssetBundleUrl, name);
                }
            }
            isInCacheFile = false;
            return string.Empty;
        }
#region EditorCode
#if UNITY_EDITOR
        static string GetEditorAssetBundleUrl() {
            string path = "file:///" + GetEditorAssetPath();
            return path;
        }
        // 获取编辑器运行时资源路径
        static string GetEditorAssetPath() {
            string path = Path.Combine(Application.dataPath, "../TempStreamingAssets/" + GetPlatformFolderName());
            return path;
        }
        // 获取平台文件夹路径
        static string GetPlatformFolderName() {
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android) {
                return "Android";
            } else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS) {
                return "IOS";
            } else {
                return "Windows";
            }
        }
#endif
#endregion
    }
}
