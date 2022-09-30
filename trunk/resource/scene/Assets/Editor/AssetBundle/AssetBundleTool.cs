using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// 打包工具类: 提供一个unity将场景和材质等打包成资源包的自动化按钮工具
public class AssetBundleTool : MonoBehaviour {
    static string _dirName = string.Empty;

   // 提供了三个相对好用的方法按钮
#region MenuItemFunction
    // 设置bundleName
    [MenuItem("Assets/AssetBundle/SetAssetBundleName")]
    static void SetAssetBundleName() {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        string path = string.Empty;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Assets);
        foreach (var asset in selectedAssets) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!Directory.Exists(assetPath)) {
                string[] pathSplits = assetPath.Split('/');
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                string subStr = assetPath.Substring(14);
                string[] splitStrs = subStr.Split('.');
                importer.assetBundleName = splitStrs[0];
            }
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    // 增量打包
    [MenuItem("Assets/AssetBundle/BuildChangedBundle")]
    static void BuildChangedBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        var root = GetBundleRoot(buildTarget);
        BuildBundle(root, buildTarget, BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression);
    }

    // 强制重新打包
    [MenuItem("Assets/AssetBundle/RebuildAllBundle")]
    static void RebuildAllBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        var root = GetBundleRoot(buildTarget);
        ClearOldResources(root);
        BuildBundle(root, buildTarget, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode);
    }
#endregion

    static void ClearOldResources(string root) {
        DirectoryInfo di = new DirectoryInfo(root);
        DeleteDictAndFiles(di);
    }
    static void DeleteDictAndFiles(DirectoryInfo dictIno, bool isRoot = true) {
        FileInfo[] fileInfos = dictIno.GetFiles();
        int len = fileInfos.Length;
        for (int i = 0; i < len; i++) {
            fileInfos[i].Delete();
        }
        DirectoryInfo[] dictInfos = dictIno.GetDirectories();
        foreach (var dict in dictInfos) {
            DeleteDictAndFiles(dict, false);
        }
    }

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
// 暂时想把它放这里，可是感觉生成的位置不对，先确定能生成唯一一个场景的资源包：是可以生成了，那么我可以先生成一个游戏起始的场景
    // static string tempStreamingAssetPath = "../../../client/trunk/TempStreamingAssets";
    static string tempStreamingAssetPath = "../../../../TempStreamingAssets";　
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
    // 获取相应平台AssetBundle存放路径
    // <param name="buildTarget"></param>
    static string GetBundleRoot(BuildTarget buildTarget) {
        var path = Path.Combine(Application.dataPath, "../TempStreamingAssets/" + GetBundleFolderName(buildTarget));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        return path;
    }
    // 获取对应平台文件夹名字
    // <param name="buildTarget"></param>
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
    // <param name="srcPath"></param>
    // <param name="tarPath"></param>
    static void CopyDirectory(string srcPath, string tarPath) {
        DirectoryInfo sourceDir = new DirectoryInfo(srcPath);
        DirectoryInfo targetDir = new DirectoryInfo(tarPath);
        if (targetDir.FullName.StartsWith(sourceDir.FullName, System.StringComparison.CurrentCultureIgnoreCase)) {
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
        // 递归// 
        DirectoryInfo[] dirs = sourceDir.GetDirectories();
        int dirsLength = dirs.Length;
        for (int j = 0; j < dirsLength; j++) {
            CopyDirectory(dirs[j].FullName, Path.Combine(targetDir.FullName, dirs[j].Name));
        }
    }
    // 生成AssetBundle对应的MD5List
    // <param name="clientPath"></param>
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
