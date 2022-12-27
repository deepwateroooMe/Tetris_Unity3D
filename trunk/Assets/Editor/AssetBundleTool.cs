using Framework.Util;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework.ResMgr;

// 打包工具类
public class AssetBundleTool : MonoBehaviour {
    private const string TAG = "AssetBundleTool"; 

    static string _dirName = string.Empty;
    
#region MenuItemFunction
    // 设置bundleName Assets/AssetBundle/ ==> Assets/
    [MenuItem("Assets/SetAssetBundleName")]
    static void SetAssetBundleName() {
        string path = string.Empty;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        foreach (var asset in selectedAssets) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            Debug.Log("assetPath: " + assetPath);
        }
    }
    // 增量打包
    [MenuItem("Assets/BuildChangedBundle %m")]
    static void BuildChangedBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        var root = GetBundleRoot(buildTarget);
        BuildBundle(root, buildTarget, BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression);
    }
    // 强制重新打包
    [MenuItem("Assets/RebuildAllBundle")]
    static void RebuildAllBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        var root = GetBundleRoot(buildTarget);
        BuildBundle(root, buildTarget, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode);
    }
#endregion
    
#region BuildDll
    // [MenuItem("Assets/BuildHotFixDllBundle")]
    [MenuItem("Assets/BuildHotFixDllBundle %j")]
    public static void BuildHotFixDllBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        AssetBundleBuild dll = new AssetBundleBuild() {
            assetBundleName = "hotfix.dll" + ResourceConstant.bundleExtension,
            assetBundleVariant = null,
            assetNames = new string[] { "Assets/HotFix/HotFix.dll.bytes" }
        };
        AssetBundleBuild pdb = new AssetBundleBuild() {
            assetBundleName = "hotfix.pdb" + ResourceConstant.bundleExtension,
            assetBundleVariant = null,
            assetNames = new string[] { "Assets/HotFix/HotFix.pdb.bytes" }
        };
        var tempBundleRoot = GetTempBundleRoot(buildTarget);
        var result = BuildPipeline.BuildAssetBundles(tempBundleRoot, new AssetBundleBuild[] { dll, pdb }, BuildAssetBundleOptions.ForceRebuildAssetBundle, buildTarget);
        if (result == null) {
            throw new System.Exception("build dll contains error, please check");
        }
        var bundleRoot = GetBundleRoot(buildTarget);
        CopyDirectory(tempBundleRoot, bundleRoot);
        GenerateAssetBundleList(bundleRoot);
    }
#endregion

    // 资源打包
    static void BuildBundle(string root, BuildTarget buildTarget, BuildAssetBundleOptions options) {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        List<AssetBundleBuild> assetBuildBuilds = new List<AssetBundleBuild>();
        int namesLength = assetBundleNames.Length;
        for (int i = 0; i < namesLength; i++) {
            string curName = assetBundleNames[i];
            string[] assetBundlePaths = AssetDatabase.GetAssetPathsFromAssetBundle(curName);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetBundlePaths[0]);
            AssetBundleBuild curAssetBundleBuild = new AssetBundleBuild();
            if (string.IsNullOrEmpty(assetImporter.assetBundleVariant)) {
                curAssetBundleBuild.assetBundleName = curName + bundleExtension;
            }
            curAssetBundleBuild.assetBundleVariant = null;
            curAssetBundleBuild.assetNames = assetBundlePaths;
            assetBuildBuilds.Add(curAssetBundleBuild);
        }
        Debug.Log("Start AssetBundle BuildPipeLine, buildTarget: " + buildTarget);
        AssetBundleManifest result = BuildPipeline.BuildAssetBundles(root, assetBuildBuilds.ToArray(), options, buildTarget);
        if (result == null) {
            Debug.Log("Error while building asset bundles, please check unity log");
            throw new System.Exception("Error while building asset bundles, please check unity log");
        }
        CopyBundleToClient(root, buildTarget);
        EditorUtility.DisplayDialog("CopyBundleToClient", "打包AssetBundle完成", "OK");
    }

    // AssetBundle文件后缀
    static string bundleExtension = ".ab";
    // 资源存储路径
    static string tempStreamingAssetPath = "../../../client/trunk/TempStreamingAssets";
    // 获取当前平台
    static BuildTarget GetBuildTarget() {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#if UNITY_ANDROID
        buildTarget = BuildTarget.Android;
#elif UNITY_IPHONE
        buildTarget = BuildTarget.iOS;    
#endif
        return buildTarget;
    }

    static string GetTempBundleRoot(BuildTarget buildTarget) {
        var path = Path.Combine(Application.dataPath, "../TempStreamingAssets/Temp/" + GetBundleFolderName(buildTarget));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    // 获取相应平台AssetBundle存放路径
    static string GetBundleRoot(BuildTarget buildTarget) {
        var path = Path.Combine(Application.dataPath, "../TempStreamingAssets/" + GetBundleFolderName(buildTarget));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    // 获取对应平台文件夹名字
    static string GetBundleFolderName(BuildTarget buildTarget) {
        if (buildTarget == BuildTarget.StandaloneWindows) {
            return "Windows";
        } else if (buildTarget == BuildTarget.Android) {
            return "Android";
        } else if (buildTarget == BuildTarget.iOS) {
            return "IOS";
        }
        return "Windows";
    }

    // 复制AssetBundle文件到Client相应路径下
    static void CopyBundleToClient(string root, BuildTarget buildTarget) {
        string clientPath = Path.Combine(Application.dataPath, Path.Combine(tempStreamingAssetPath, GetBundleFolderName(buildTarget)));
        CopyDirectory(root, clientPath);
        GenerateAssetBundleList(clientPath);
    }

    // 复制文件
    static void CopyDirectory(string srcPath, string tarPath) {
        Debug.Log(TAG + " CopyDirectory");
        DirectoryInfo sourceDir = new DirectoryInfo(srcPath);
        DirectoryInfo targetDir = new DirectoryInfo(tarPath);
        Debug.Log(TAG + " sourceDir.FullName: " + sourceDir.FullName);
        Debug.Log(TAG + " targetDir.FullName: " + targetDir.FullName);
        bool tmp = targetDir.FullName.StartsWith(sourceDir.FullName, System.StringComparison.CurrentCultureIgnoreCase);
        // if (targetDir.FullName.StartsWith(sourceDir.FullName, System.StringComparison.CurrentCultureIgnoreCase)) {
        if (tmp) {
            Debug.LogError("父目录不能拷贝到子目录！");
            return;
        }
        if (!sourceDir.Exists) {
            return;
        }
        if (!targetDir.Exists) {
            targetDir.Create();
        }
        FileInfo[] files = sourceDir.GetFiles();
        int filesLength = files.Length;
        for (int i = 0; i < filesLength; i++) {
            if (files[i].Extension == bundleExtension) {
                File.Copy(files[i].FullName, Path.Combine(targetDir.FullName, files[i].Name), true);
            }
        }
        // 递归
        DirectoryInfo[] dirs = sourceDir.GetDirectories();
        int dirsLength = dirs.Length;
        for (int j = 0; j < dirsLength; j++) {
            CopyDirectory(dirs[j].FullName, Path.Combine(targetDir.FullName, dirs[j].Name));
        }
    }

    // 生成AssetBundle对应的MD5List
    static void GenerateAssetBundleList(string clientPath) {
        DirectoryInfo dir = new DirectoryInfo(clientPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        string tempPath = dir.FullName.Replace("\\", "/");
        string path = string.Empty;
        foreach (var file in files) {
            if (file.FullName.EndsWith(bundleExtension)) {
                using (var sr = new StreamReader(file.FullName)) {
                    var md5 = CryptoHelp.MD5(sr.BaseStream);
                    var fullName = file.FullName.Replace("\\", "/");
                    path += fullName.Replace(tempPath + "/", "") + "," + md5 + "," + sr.BaseStream.Length + "\n";
                }
            }
        }
        string assetBundleListPath = Path.Combine(clientPath, "AssetBundleList.txt");
        if (assetBundleListPath.Length != 0) {
            FileStream cache = new FileStream(assetBundleListPath, FileMode.Create);
            var encoding = new System.Text.UTF8Encoding();
            var bytes = encoding.GetBytes(path);
            cache.Write(bytes, 0, bytes.Length);
            cache.Close();
        }
    }
}
