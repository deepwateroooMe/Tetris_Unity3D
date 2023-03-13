using Framework.Util;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Collections;
//using TMPro;
using Framework.Core;
using TMPro;

namespace Framework.ResMgr {

    // 按需加载
    public class ResourceMap : MonoBehaviour, IResourceLoader {
        private const string TAG = "ResourceMap"; 

        // 静态实例，在是游戏程序集，但是借助公用接口，这个实例用样作为引用借给热更新程序集调用相关函数获取资源等
        static ResourceMap _instance; 
        public static ResourceMap Instance {
            get {
                return _instance;
            }
        }

        // ResourceList初始化成功，委托
        public Action OnInitializeSuccess; 
        
        // Assetbundle资源包集合: 资源包字典,资源包管理器
        // Key: AssetBundleName
        public Dictionary<string, AssetBundleSpec>  assetBundleSpecs = new Dictionary<string, AssetBundleSpec>();
        // 正在下载的AssetBundle
        public AssetBundleSpec downloadingAssetBundleSpec;
        // AssetBundle下载队列
        public Queue<AssetLoader> downloadingAssets = new Queue<AssetLoader>();

        // AssetBundle下载信息缓存集合
        public Dictionary<string, AssetBundleDownloadReference> downloadingAssetBundleSpecs
            = new Dictionary<string, AssetBundleDownloadReference>();
        // 封装AssetBundle引用计数
        public class AssetBundleDownloadReference {
            public AssetBundleSpec spec;
            public int referenceCount;
        }

        public Action<AssetLoader> onNextAssetLoader;
        float checkSaveTimes = 0;
        bool haveCachedChanged = false;

// 具备unity控件的生命周期，先看一下启动过程        
#region Initialize
        void Awake() {
            _instance = this;
            IOSSetting();
            Initialize();
            InitializeLoadingPanel();
        }
        // 初始化
        void Initialize() {
            CheckResourcePathExist();　// 资源路径是否存在，不存在则创建
            if (ResourceConstant.CacheAssetBundle) {　// 默认是缓存资源的: 说的是什么意思呢 ?
                FillResourceList();
                FillHotFixList();
            }
            DownLoadServerResourceList();　// <<<<<<<<<<<<<<<<<<<< 这里负责从服务器下载热更新资源
        }
        // 校验
        void CheckResourcePathExist() {
            if (!Directory.Exists(ResourceConstant.AssetBundleCacheRoot)) {
                Directory.CreateDirectory(ResourceConstant.AssetBundleCacheRoot);
            }
// 热更新资源列表 空文件 : 如果没有,要建一个.晚点儿md5 比对出来,需要更新的资源包相关信息,会加进这个文件            
            if (!File.Exists(ResourceConstant.AssetBundleCacheRoot + "/HotFixList.txt")) {
                File.Create(ResourceConstant.AssetBundleCacheRoot + "/HotFixList.txt").Close();
            }
        }
        // 分析只读路径bundleList
        void FillResourceList() {
            Debug.Log(TAG + " FillResourceList()");
            string text = FileHelp.ReadString("AssetBundleList.txt");
            Debug.Log("ResourceList:  " + text);
            if (!string.IsNullOrEmpty(text)) {
// 把文件中的所有:一个一个的资源包都解析(若是最新资源包)或是作好需要下载更新的标记 ?               
                AnalysisResourceList(text, EAssetBunbleSourceType.Primary);
                Debug.Log(TAG + " FillResourceList() text: " + text);
            }
        }
        // 分析可读写路径bundleList
        void FillHotFixList() {
            Debug.Log("FillHotFixList");
            // string text = File.ReadAllText(ResourceConstant.AssetBundleCacheRoot + "/HotFixList.txt");
// 从可作热更新的资源包列表里读出相关(下载的)资源?            
            string text = FileHelp.ReadString("HotFixList.txt");
            Debug.Log("HotFixList:  " + text);
            if (!string.IsNullOrEmpty(text)) {
                AnalysisResourceList(text, EAssetBunbleSourceType.Hotfix); // 同样的方法分析可作热更新的资源包列表数据
            }
        }
        // 从资源服务器下载bundleList
        void DownLoadServerResourceList() {
            Debug.Log(TAG + " DownLoadServerResourceList() (!GameApplication.Instance.useLocal): " + (!GameApplication.Instance.useLocal));
            if (!GameApplication.Instance.useLocal) { // 如果不使用本地资源：有热更新服务器可供热更新的话
                string url = Path.Combine(ResourceConstant.RemoteAssetBundleUrl, "AssetBundleList.txt");
                url = url + "?timestamp=" + DateTime.Now.ToString();
                Debug.Log("Server AssetBundleURL: " + url);
                DateTime currentTime = DateTime.Now;
                HttpHelper.Instance.Get(url, (request) => {
                    Debug.Log("ServerResourceList:  " + request.downloadHandler.text);
                    AnalysisResourceList(request.downloadHandler.text, EAssetBunbleSourceType.Server);
                    if (OnInitializeSuccess != null) {
                        OnInitializeSuccess();
                    }
                }, (request) => {
                    DebugHelper.LogError("DownLoadServerResourceList Fail", true);
                }, 3);
            } else { // 如果没有或是不是热更新服务器,则把本地资源再遍历检查一遍
                string text = FileHelp.ReadString("AssetBundleList.txt");
                Debug.Log(TAG + " text: " + text);
                if (!string.IsNullOrEmpty(text)) {
                    AnalysisResourceList(text, EAssetBunbleSourceType.Server);
                }
                GameApplication.Instance.StartHotFix(); // 直接调用热更新
            }
        }
        // 显示加载进度条
        void InitializeLoadingPanel() { }

        // 分析资源文件: 包括所有资源的列表,每一行是一个程序或是材料等相关资源包的信息
        void AnalysisResourceList(string text, EAssetBunbleSourceType type) {
            string[] fileInfos = text.Split('\n'); // 以行为单位区分不同的资源包
            int fileInfosLength = fileInfos.Length;
            for (int i = 0; i < fileInfosLength - 1; i++) { // 遍历每个资源包(从程序资源包到材料资源包等),为什么是 <　n-1 ? < n or <= n-1
                string[] fileInfoParams = fileInfos[i].Split(','); // 每行每个资源包以,为单位区分不同列与属性
                if (fileInfoParams.Length >= 2) {
                    if (!assetBundleSpecs.ContainsKey(fileInfoParams[0])) { // 某个层级某种类型的资源的名字,如果不存在,则添加
                        AssetBundleSpec asset = new AssetBundleSpec(fileInfoParams[0], fileInfoParams[1], int.Parse(fileInfoParams[2]), type);
                        assetBundleSpecs.Add(fileInfoParams[0], asset);
                    } else { // 已经存在,则作MD5比对,作好相应的标记(是否　需要热更新下载服务器资源包等？)
                        assetBundleSpecs[fileInfoParams[0]].Check(fileInfoParams[1], int.Parse(fileInfoParams[2]), type);
                    }
                } else {
                    DebugHelper.LogError("fileInfo error! fileInfo: " + fileInfos[i]);
                }
            }
            Debug.Log(TAG + " AnalysisResourceList() assetBundleSpecs.Count: " + assetBundleSpecs.Count);
        }
#endregion
#region Util
        // 获得处理过的正确Bundle名
        // <param name="bundleName"></param>
        string GetFinalBundleName(string bundleName) {
            string finalBundleName = bundleName.ToLower();
            if (!bundleName.EndsWith(ResourceConstant.bundleExtension)) {
                finalBundleName = bundleName.ToLower() + ResourceConstant.bundleExtension;
            }
            return finalBundleName;
        }
#endregion
#region Load
        // 同步加载资源
        public T LoadAsset<T>(string bundleName, string assetName, 
                              EAssetBundleUnloadLevel unloadLevel = 
                              EAssetBundleUnloadLevel.ChangeSceneOver) where T : UnityEngine.Object {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadAsset<T>(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public TMPro.TMP_FontAsset LoadTMP_FontAsset(string bundleName, string assetName, 
                                              EAssetBundleUnloadLevel unloadLevel = 
                                              EAssetBundleUnloadLevel.ChangeSceneOver) {
           string finalBundleName = GetFinalBundleName(bundleName);
           if (assetBundleSpecs.ContainsKey(finalBundleName)) {
               return assetBundleSpecs[finalBundleName].LoadTMP_FontAsset(assetName, unloadLevel);
           } else {
               DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
               return null;
           }
        }
        public Font LoadFont(string bundleName, string assetName, 
                             EAssetBundleUnloadLevel unloadLevel = 
                             EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadFont(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public AnimationClip LoadAnimationClip(string bundleName, string assetName, 
                                               EAssetBundleUnloadLevel unloadLevel = 
                                               EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadAnimationClip(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public AnimatorOverrideController LoadAnimatorOverrideController(string bundleName, string assetName, 
                                                                         EAssetBundleUnloadLevel unloadLevel = 
                                                                         EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadAnimatorOverrideController(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public RuntimeAnimatorController LoadRuntimeAnimatorController(string bundleName, string assetName, 
                                                                       EAssetBundleUnloadLevel unloadLevel = 
                                                                       EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadRuntimeAnimatorController(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public AudioClip LoadAudioClip(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadAudioClip(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public Material LoadMaterial(string bundleName, string assetName, 
                                     EAssetBundleUnloadLevel unloadLevel = 
                                     EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadMaterial(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public TextAsset LoadTextAsset(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadTextAsset(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public Sprite LoadSprite(string bundleName, string assetName, 
                                 EAssetBundleUnloadLevel unloadLevel = 
                                 EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadSprite(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        public Texture2D LoadTexture2D(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadTexture2D(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
                return null;
            }
        }
        // 同步加载场景
        public void LoadScene(string bundleName, string assetName, 
                              EAssetBundleUnloadLevel unloadLevel = 
                              EAssetBundleUnloadLevel.ChangeSceneOver, bool isAdditive = false) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                assetBundleSpecs[finalBundleName].LoadScene(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName, true);
            }
        }
        // 同步加载GameObject
        public GameObject LoadClone(string bundleName, string assetName, 
                                    EAssetBundleUnloadLevel unloadLevel = 
                                    EAssetBundleUnloadLevel.ChangeSceneOver) {
            string finalBundleName = GetFinalBundleName(bundleName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                return assetBundleSpecs[finalBundleName].LoadClone(assetName, unloadLevel);
            } else {
                DebugHelper.LogError("ResourceMap not contains " + finalBundleName + " " + StackTraceUtility.ExtractStackTrace(), true);
                return null;
            }
        }
#endregion
#region LoadAsyn
        // 异步加载资源
        public void LoadAssetAsyn<T>(string bundleName, string assetName, Action<T> loadOver, 
                                     EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) where T : UnityEngine.Object {
            AssetLoader<T> assetLoader = new AssetLoader<T>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadTMP_FontAssetAsyn(string bundleName, string assetName, Action<TMP_FontAsset> loadOver, 
                                         EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
           AssetLoader<TMP_FontAsset> assetLoader = new AssetLoader<TMP_FontAsset>();
           assetLoader.bundleName = bundleName.ToLower();
           assetLoader.assetName = assetName;
           assetLoader.onLoadOver = loadOver;
           assetLoader.unloadLevel = unloadLevel;
           AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadFontAsyn(string bundleName, string assetName, Action<Font> loadOver, 
                                 EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<Font> assetLoader = new AssetLoader<Font>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadAnimationClipAsyn(string bundleName, string assetName, Action<AnimationClip> loadOver, 
                                          EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<AnimationClip> assetLoader = new AssetLoader<AnimationClip>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadAnimatorOverrideControllerAsyn(string bundleName, string assetName, Action<AnimatorOverrideController> loadOver, 
                                                       EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<AnimatorOverrideController> assetLoader = new AssetLoader<AnimatorOverrideController>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadRuntimeAnimatorControllerAsyn(string bundleName, string assetName, Action<RuntimeAnimatorController> loadOver, 
                                                      EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<RuntimeAnimatorController> assetLoader = new AssetLoader<RuntimeAnimatorController>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadAudioClipAsyn(string bundleName, string assetName, Action<AudioClip> loadOver, 
                                      EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<AudioClip> assetLoader = new AssetLoader<AudioClip>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadMaterialAsyn(string bundleName, string assetName, Action<Material> loadOver, 
                                     EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<Material> assetLoader = new AssetLoader<Material>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadTextAssetAsyn(string bundleName, string assetName, Action<TextAsset> loadOver, 
                                      EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<TextAsset> assetLoader = new AssetLoader<TextAsset>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadSpriteAsyn(string bundleName, string assetName, Action<Sprite> loadOver, 
                                   EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<Sprite> assetLoader = new AssetLoader<Sprite>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        public void LoadTexture2DAsyn(string bundleName, string assetName, Action<Texture2D> loadOver, 
                                      EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            AssetLoader<Texture2D> assetLoader = new AssetLoader<Texture2D>();
            assetLoader.bundleName = bundleName.ToLower();
            assetLoader.assetName = assetName;
            assetLoader.onLoadOver = loadOver;
            assetLoader.unloadLevel = unloadLevel;
            AddAssetLoaderToLoadQueue(assetLoader, unloadLevel, isForceInterrupLoad);
        }
        // 异步加载场景
        // <param name="bundleName"></param>
        // <param name="aseetName"></param>
        // <param name="success"></param>
        // <param name="unloadLevel"></param>
        // <param name="isAdditive"></param>
        public void LoadSceneAsyn(string bundleName, string assetName, Action success, 
                                  EAssetBundleUnloadLevel unloadLevel, bool isAdditive = false) {
            SceneLoader sceneLoader = new SceneLoader();
            sceneLoader.bundleName = bundleName.ToLower();
            sceneLoader.assetName = assetName;
            sceneLoader.onLoadOver = success;
            sceneLoader.unloadLevel = unloadLevel;
            sceneLoader.isAdditive = isAdditive;
            AddAssetLoaderToLoadQueue(sceneLoader, unloadLevel, false);
        }
        // 异步加载GameObject
        // <param name="bundleName"></param>
        // <param name="assetName"></param>
        // <param name="success"></param>
        // <param name="unloadLevel"></param>
        // <param name="isForceInterrupLoad"></param>
        public void LoadCloneAsyn(string bundleName, string assetName, Action<GameObject> success, 
                                  EAssetBundleUnloadLevel unloadLevel, bool isForceInterrupLoad = false) {
            GameObjectLoader gameObjectLoader = new GameObjectLoader();
            gameObjectLoader.bundleName = bundleName.ToLower();
            gameObjectLoader.assetName = assetName;
            gameObjectLoader.unloadLevel = unloadLevel;
            gameObjectLoader.onCloneOver = success;
            AddAssetLoaderToLoadQueue(gameObjectLoader, unloadLevel, isForceInterrupLoad);
        }
        // 添加AssetLoader到加载队列中
        // <param name="assetLoader"></param>
        // <param name="unloadLevel"></param>
        // <param name="isForceInterruptLoad"></param>
        void AddAssetLoaderToLoadQueue(AssetLoader assetLoader, 
                                       EAssetBundleUnloadLevel unloadLevel, bool isForceInterruptLoad) {
            if (assetBundleSpecs.ContainsKey(assetLoader.FinalBundleName)) {
                assetLoader.onAssetLoadEnd = LoadNextAsset;
                var assetLoaderSpec = assetBundleSpecs[assetLoader.FinalBundleName];
                assetLoader.Spec = assetLoaderSpec;
                if (downloadingAssetBundleSpec == null) {
                    downloadingAssetBundleSpec = assetLoaderSpec;
                    downloadingAssetBundleSpec.TryLoadAsyn(assetLoader, unloadLevel);
                } else {
                    downloadingAssets.Enqueue(assetLoader);
                }
                if (!downloadingAssetBundleSpecs.ContainsKey(assetLoaderSpec.Name)) {
                    downloadingAssetBundleSpecs[assetLoaderSpec.Name]
                        = new AssetBundleDownloadReference() { spec = assetLoaderSpec, referenceCount = 1 };
                } else {
                    downloadingAssetBundleSpecs[assetLoaderSpec.Name].referenceCount++;
                }
            } else {
                DebugHelper.LogError("ResourceMap not contains " + assetLoader.FinalBundleName, true);
            }
        }
        // 加载下一个AssetLoader
        void LoadNextAsset(AssetLoader al) {
            if (downloadingAssetBundleSpec != null && downloadingAssetBundleSpecs.ContainsKey(downloadingAssetBundleSpec.Name)) {
                AssetBundleDownloadReference r = downloadingAssetBundleSpecs[downloadingAssetBundleSpec.Name];
                r.referenceCount--;
                if (r.referenceCount <= 0) {
                    downloadingAssetBundleSpecs.Remove(downloadingAssetBundleSpec.Name);
                }
            }
            if (downloadingAssets.Count > 0) {
                AssetLoader assetLoader = downloadingAssets.Dequeue();
                downloadingAssetBundleSpec = assetLoader.Spec;
                downloadingAssetBundleSpec.TryLoadAsyn(assetLoader,
                                                       downloadingAssetBundleSpec.UnloadLevel,
                                                       downloadingAssetBundleSpec.IsForceInterruptLoad);
                if (onNextAssetLoader != null) {
                    onNextAssetLoader(assetLoader);
                }
            } else {
                downloadingAssetBundleSpec = null;
            }
        }
#endregion
#region  GetVideoClipURL
        public void GetFileURLAsyn(string name, Action<string> onSuccess, Action onFail, bool needCache = true) {
            if (!FileHelp.IsFileExists("VideoClip/" + name)) {
                string url = Path.Combine(ResourceConstant.RemoteAssetBundleUrl + "VideoClip", name);
                Debug.Log("url: " + url);
                HttpHelper.Instance.Get(url, (request) => {
                    if (needCache) {
                        if (!Directory.Exists(ResourceConstant.AssetBundleCacheRoot + "/VideoClip")) {
                            Directory.CreateDirectory(ResourceConstant.AssetBundleCacheRoot + "/VideoClip");
                        }
                        FileStream fileStream = File.Create(Path.Combine(ResourceConstant.AssetBundleCacheRoot + "/VideoClip", name));
                        byte[] data = request.downloadHandler.data;
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Close();
                        fileStream.Dispose();
                        if (onSuccess != null) {
                            onSuccess("VideoClip/" + name);
                        }
                    }
                }, (request) => {
                    if (onFail != null) {
                        onFail();
                    }
                });
            } else {
                if (onSuccess != null) {
                    onSuccess("VideoClip/" + name);
                }
            }
        }
#endregion
#region LoadTexture
        // 异步从网上加载图片
        public void LoadTexture2DAsyn(string name, Action<Texture2D> onSuccess, Action onFail, bool needCache = true) {
            if (FileHelp.IsFileExists("DownloadTexture/" + name)) {
                CoroutineHelper.StartCoroutine(LoadTexture2DFromCacahe(name, onSuccess, onFail));
            } else {
                string url = Path.Combine(ResourceConstant.RemoteAssetBundleUrl, name);
                HttpHelper.Instance.GetTexture(url, (texture, request) => {
                    if (needCache) {
                        if (!Directory.Exists(ResourceConstant.AssetBundleCacheRoot + "/DownloadTexture")) {
                            Directory.CreateDirectory(ResourceConstant.AssetBundleCacheRoot + "/DownloadTexture");
                        }
                        FileStream fileStream = File.Create(Path.Combine(ResourceConstant.AssetBundleCacheRoot + "/DownloadTexture", name));
                        byte[] data = request.downloadHandler.data;
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    if (onSuccess != null) {
                        onSuccess(texture);
                    }
                }, (request) => {
                    if (onFail != null) {
                        onFail();
                    }
                });
            }
        }
        WWW thisWWW;
        IEnumerator LoadTexture2DFromCacahe(string name, Action<Texture2D> onSuccess, Action onFail) {
            string url = Path.Combine(ResourceConstant.AssetBundleCacheUrl + "/DownloadTexture", name);
            thisWWW = new WWW(url);
            yield return thisWWW;
            if (thisWWW == null) {
                DebugHelper.LogError("Download Texture2D " + url + " thisWWW == null", true);
                if (onFail != null) {
                    onFail();
                }
            } else if (!string.IsNullOrEmpty(thisWWW.error)) {
                DebugHelper.LogError("Download Texture2D " + thisWWW.error + " url: " + url, true);
                yield return new WaitForEndOfFrame();
                if (thisWWW != null) {
                    thisWWW.Dispose();
                    thisWWW = null;
                    if (onFail != null) {
                        onFail();
                    }
                }
            } else {
                if (onSuccess != null) {
                    onSuccess(thisWWW.texture);
                }
            }
        }
#endregion
#region Unload
        // 卸载
        public void Unload(string keyName, bool allObjects) {
            string finalBundleName = GetFinalBundleName(keyName);
            if (assetBundleSpecs.ContainsKey(finalBundleName)) {
                assetBundleSpecs[finalBundleName].Unload(allObjects);
            }
        }
        // 全部卸载
        public void UnloadAll() {
            var tempData = downloadingAssets.Where(p => p.Spec.UnloadLevel == 
                                                   EAssetBundleUnloadLevel.Never);
            downloadingAssets = new Queue<AssetLoader>(tempData);
            downloadingAssetBundleSpecs.Clear();
            foreach (var assetBundle in assetBundleSpecs) {
                if (assetBundle.Value.UnloadLevel == 
                    EAssetBundleUnloadLevel.ChangeSceneOver) {
                    assetBundle.Value.Unload(false);
                }
            }
            if (downloadingAssetBundleSpecs == null) 
                LoadNextAsset(null);
        }
#endregion
        void Update() {
            checkSaveTimes += Time.deltaTime;
            if (checkSaveTimes > 5) {
                checkSaveTimes = 0;
                if (haveCachedChanged) {
                    haveCachedChanged = false;
                    SaveHotFixResourceList();
                }
            }
        }
        public void SaveHotFixResourceList() {
            if (ResourceConstant.CacheAssetBundle) {
                DateTime regTime = DateTime.Now;
                StringBuilder stringBuilder = new StringBuilder();
                string hotFixListPath = ResourceConstant.AssetBundleCacheRoot + "/HotFixList.txt";
                FileStream fileStream;
                if (!File.Exists(hotFixListPath)) {
                    fileStream = File.Create(hotFixListPath);
                } else {
                    fileStream = new FileStream(hotFixListPath, FileMode.Truncate);
                }
                string ts = string.Empty;
                var ec = new UTF8Encoding();
                foreach (var assetBundleSpec in assetBundleSpecs) {
                    if (assetBundleSpec.Value.IsCached && assetBundleSpec.Value.AssetBundleSourceType == EAssetBunbleSourceType.Hotfix) {
                        ts = assetBundleSpec.Value.Name + "," + assetBundleSpec.Value.MD5 + "," + assetBundleSpec.Value.Length + '\n';
                        var bytes = ec.GetBytes(ts);
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                }
                fileStream.Close();
                fileStream.Dispose();
                DebugHelper.LogError("Save HotFixList.txt cost " + (DateTime.Now - regTime).TotalMilliseconds, true);
            }
        }
        public void OnCachedAssetBundleSpec(AssetBundleSpec spec) {
            haveCachedChanged = true;
        }
        // IOS平台设置
        void IOSSetting() {
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
#endif
        }
	}
}