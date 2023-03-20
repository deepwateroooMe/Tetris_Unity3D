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
    // 增量打包：与下面的强制重新打包，打全部包相比，这里是说，没有变化的资源包，可以不用重新再打一遍了？这里【比较表层的地方，看不出处理上有什么不同 ]
    [MenuItem("Assets/BuildChangedBundle %m")] // 只打包：更改过的资源包文件
    static void BuildChangedBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        var root = GetBundleRoot(buildTarget);
        BuildBundle(root, buildTarget, BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression); // <<<<<<<<<< 
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
    // 在 Startup.cs 里面，有个自动检测脚本变化的：当它检测到.dll 文件有变动，就会自动调用这个方法，来打包热更新程序集包
    [MenuItem("Assets/BuildHotFixDllBundle %j")] // 所以，这里打包，也就是把构建好的程序集打成资源包
    // 能想明白：在 Hotfix.csproj 中的最后几行，是命令行将构建成功【编译成功编译之后，已经是字节码了】的程序集复制一份到如下两个目录文件。并且复制后的就成了编译后的字节码程序包，可以用来打资源包了
    public static void BuildHotFixDllBundle() {
        BuildTarget buildTarget = GetBuildTarget();
        AssetBundleBuild dll = new AssetBundleBuild() {
            assetBundleName = "hotfix.dll" + ResourceConstant.bundleExtension,
            assetBundleVariant = null,
            assetNames = new string[] { "Assets/HotFix/HotFix.dll.bytes" } // 这里是字符串名字，实际对应的是这个目录文件，需要被打进这个资源包里。所以它打的就是热更新的程序集了
        };
        AssetBundleBuild pdb = new AssetBundleBuild() {
            assetBundleName = "hotfix.pdb" + ResourceConstant.bundleExtension,
            assetBundleVariant = null,
            assetNames = new string[] { "Assets/HotFix/HotFix.pdb.bytes" }
        };
        var tempBundleRoot = GetTempBundleRoot(buildTarget); // 拿到热更新程序集资源包的临时的存放位置：F:\tetris3D\trunk\TempStreamingAssets\Temp\Windows
// 调用的是：下面这个 API 来实现程序集的打包。可是原理在底层，找不出来，没有看懂
        var result = BuildPipeline.BuildAssetBundles(tempBundleRoot, new AssetBundleBuild[] { dll, pdb }, BuildAssetBundleOptions.ForceRebuildAssetBundle, buildTarget);
        if (result == null) {
            throw new System.Exception("build dll contains error, please check");
        }
        var bundleRoot = GetBundleRoot(buildTarget);
        CopyDirectory(tempBundleRoot, bundleRoot);
        GenerateAssetBundleList(bundleRoot); // 这里也是说，生成新的码表文件 
    }
#endregion

    // 资源打包：这个方法，更多的是去理解，Unity 游戏引擎内部打包过程、打包选项等基础原理
    static void BuildBundle(string root, BuildTarget buildTarget, BuildAssetBundleOptions options) {
        AssetDatabase.RemoveUnusedAssetBundleNames(); // 自动移除没有使用的资源包名
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames(); // 系统资源数据库里注册过的：所有资源包的名字
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

    static string GetTempBundleRoot(BuildTarget buildTarget) { // 这里的地址是：F:\tetris3D\trunk\TempStreamingAssets\Temp\Windows
        var path = Path.Combine(Application.dataPath, "../TempStreamingAssets/Temp/" + GetBundleFolderName(buildTarget));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    // 获取相应平台AssetBundle存放路径
    static string GetBundleRoot(BuildTarget buildTarget) { // 拿到的是：客户端本地的资源包相应平台的存储路径 F:\tetris3D\trunk\TempStreamingAssets\Windows
        var path = Path.Combine(Application.dataPath, "../TempStreamingAssets/" + GetBundleFolderName(buildTarget)); // 这里客户端，仍让它继续保持使用 Windows, 服务器使用PC 名
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
        GenerateAssetBundleList(clientPath); // 这个步骤：永远会重新生成一个新的码表文件，并一一重新计算所有资源包的MD5 值
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

    // 生成AssetBundle对应的MD5List: 生成码表文件
    static void GenerateAssetBundleList(string clientPath) {
        DirectoryInfo dir = new DirectoryInfo(clientPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        string tempPath = dir.FullName.Replace("\\", "/");
        string path = string.Empty;
        foreach (var file in files) { // 这里是：每个资源包文件，作为一行，加到字符串的结尾
            if (file.FullName.EndsWith(bundleExtension)) {
                using (var sr = new StreamReader(file.FullName)) {
                    var md5 = CryptoHelp.MD5(sr.BaseStream);
                    var fullName = file.FullName.Replace("\\", "/");
                    path += fullName.Replace(tempPath + "/", "") + "," + md5 + "," + sr.BaseStream.Length + "\n";
                }
            }
        }
        string assetBundleListPath = Path.Combine(clientPath, "AssetBundleList.txt"); // 码表文件路径
        if (assetBundleListPath.Length != 0) {
            FileStream cache = new FileStream(assetBundleListPath, FileMode.Create); // 创建并写入文件
            var encoding = new System.Text.UTF8Encoding();
            var bytes = encoding.GetBytes(path);
            cache.Write(bytes, 0, bytes.Length);
            cache.Close();
        }
    }
}


