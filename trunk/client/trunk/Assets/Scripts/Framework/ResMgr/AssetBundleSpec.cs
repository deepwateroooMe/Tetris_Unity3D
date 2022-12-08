using UnityEngine;
using Framework.Util;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
//using TMPro;

namespace Framework.ResMgr {

    // 资源数据结构类
    public class AssetBundleSpec {
        // 资源名
        public string Name {
            get;
            private set;
        }
        // 资源文件MD5
        public string MD5 {
            get;
            private set;
        }
        // 文件长度
        public int Length {
            get;
            private set;
        }
        // 资源来源分类
        public EAssetBunbleSourceType AssetBundleSourceType {
            get;
            private set;
        }
        // 是否被缓存
        public bool IsCached {
            get;
            private set;
        }
        // 是否强行中断加载
        public bool IsForceInterruptLoad {
            get;
            private set;
        }
        public EAssetBundleUnloadLevel UnloadLevel {
            get;
            private set;
        }
        public AssetBundle AssetBundle {
            get;
            private set;
        }
        public byte[] Data {
            get;
            private set;
        }
        bool isLoading = false;
        bool isDownloading = false;
        bool IsFileExist {
            get {
                return FileHelp.IsFileExists(Name);
            }
        }
        List<AssetLoader> assetLoaders = new List<AssetLoader>();
        public Action<AssetBundleSpec> onDownloadSuccess;
        public Action<AssetBundleSpec> onDownloadFail;
        public AssetBundleSpec(string name, string md5, int length, EAssetBunbleSourceType sourceType) {
            Name = name;
            MD5 = md5;
            Length = length;
            AssetBundleSourceType = sourceType;
        }
        public void Check(string md5, int length, EAssetBunbleSourceType sourceType) {
            if (sourceType == EAssetBunbleSourceType.Server) {
                if (md5 == MD5) {
                    // Debug.Log("Name: " + Name + " md5: " + md5 + " MD5: " + MD5);
                    IsCached = true;
                }
            } else {
                AssetBundleSourceType = sourceType;
                MD5 = md5;
                Length = length;
            }
        }

#region Unload
        // 卸载
        public void Unload(bool unloadAllLoadedObjects) {
            if (AssetBundle != null) {
                AssetBundle.Unload(unloadAllLoadedObjects);
                AssetBundle = null;
            }
            if (assetLoaders != null && IsForceInterruptLoad) 
                assetLoaders.Clear();
        }
#endregion
        
#region Download
        // 开始从网上下载
        public void TryDownloadAsset() {
            string url = Path.Combine(ResourceConstant.RemoteAssetBundleUrl, Name);
            url = url + "?timestamp=" + DateTime.Now.ToString();
            isDownloading = true;
            DateTime regTime = DateTime.Now;
            HttpHelper.Instance.Get(url, (request) => {
                var costTime = (DateTime.Now - regTime).TotalMilliseconds;
                DebugHelper.Log(Name + " TryDownloadAsset cost " + costTime + " " + url, true);
                DownloadSuccess(request, Name);
            }, (UnityWebRequest request) => {
                DownloadFail(request, TryDownloadAsset);
            }, 3);
        }
        public UnityWebRequest request;
        public void DownloadAsset() {
            DateTime regTime = DateTime.Now;
            if (request == null) {
                string path = Path.Combine(ResourceConstant.RemoteAssetBundleUrl, Name);
                path = path + "?timestamp=" + DateTime.Now.ToString();
                isDownloading = true;
                request = HttpHelper.Instance.Get(path);
                request.timeout = 30;
            }
            request.SendWebRequest();
            while (!request.isDone) {
                continue;
            }
            var costTime = (DateTime.Now - regTime).TotalMilliseconds;
            DebugHelper.Log(Name + " DownloadAsset cost " + costTime, true);
            if (request.error == null && request.responseCode == 200) {
                DownloadSuccess(request, Name);
            } else 
                DownloadFail(request, DownloadAsset);
        }
        // 下载成功
        void DownloadSuccess(UnityWebRequest request, string name) {
            isDownloading = false;
            string cachedAssetBundlePath = Path.Combine(ResourceConstant.AssetBundleCacheRoot, name);
            string dir = Path.GetDirectoryName(cachedAssetBundlePath);
            if (!Directory.Exists(dir)) 
                Directory.CreateDirectory(dir);
            byte[] bytes = request.downloadHandler.data;
            MD5 = CryptoHelp.MD5(bytes, 0, bytes.Length);
            Length = bytes.Length;
            if (ResourceConstant.CacheAssetBundle) {
                FileStream fs = new FileStream(cachedAssetBundlePath, FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
            }
            DebugHelper.Log("CacheAsset: " + cachedAssetBundlePath, true);
            IsCached = true;
            AssetBundleSourceType = EAssetBunbleSourceType.Hotfix;
            if (Name.EndsWith(ResourceConstant.bundleExtension)) {
                if (AssetBundle == null) {
                    DateTime regTime = DateTime.Now;
                    AssetBundle = AssetBundle.LoadFromMemory(bytes);
                    var costTime = (DateTime.Now - regTime).TotalMilliseconds;
                    DebugHelper.Log(Name + " LoadFromMemory cost " + costTime, true);
                }
            } else 
                Data = bytes;
            ResourceMap.Instance.OnCachedAssetBundleSpec(this);
            AssetBundleDownLoadReady();
            if (onDownloadSuccess != null) 
                onDownloadSuccess(this);
        }
        // 下载失败
        void DownloadFail(UnityWebRequest request, Action retryAction) {
            isDownloading = false;
            DebugHelper.LogError("DownloadFail " + request.error + " uri: " + request.url, true);
            if (onDownloadFail != null) {
                onDownloadFail(this);
            }
            // retryAction();
        }
        void AssetBundleDownLoadReady() {
            foreach (var assetLoader in assetLoaders) {
                assetLoader.LoadAssetFromBundle(AssetBundle);
            }
            assetLoaders.Clear();
        }

#endregion
        
#region Load
        // 同步加载Asset
        public T LoadAsset<T>(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) where T : UnityEngine.Object {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(T).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<T>(n[n.Length - 1]);
            return asset;
        }
        public TMP_FontAsset LoadTMP_FontAsset(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
           TryReadyAssetBundle(unloadLevel);
           if (AssetBundle == null) {
               DebugHelper.LogError("LoadAsset " + typeof(TMP_FontAsset).Name + " " + Name + " " + assetName + " return null", true);
               return null;
           }
           var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
           var n = name.Split('/');
           var asset = AssetBundle.LoadAsset<TMP_FontAsset>(n[n.Length - 1]);
           return asset;
        }
        public Font LoadFont(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(Font).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<Font>(n[n.Length - 1]);
            return asset;
        }
        public AnimationClip LoadAnimationClip(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(AnimationClip).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<AnimationClip>(n[n.Length - 1]);
            return asset;
        }
        public AnimatorOverrideController LoadAnimatorOverrideController(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(AnimatorOverrideController).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<AnimatorOverrideController>(n[n.Length - 1]);
            return asset;
        }
        public RuntimeAnimatorController LoadRuntimeAnimatorController(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(RuntimeAnimatorController).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<RuntimeAnimatorController>(n[n.Length - 1]);
            return asset;
        }
        public AudioClip LoadAudioClip(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(AudioClip).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<AudioClip>(n[n.Length - 1]);
            return asset;
        }
        public Material LoadMaterial(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(Material).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<Material>(n[n.Length - 1]);
            return asset;
        }
        public TextAsset LoadTextAsset(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(TextAsset).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<TextAsset>(n[n.Length - 1]);
            return asset;
        }
        public Sprite LoadSprite(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(Sprite).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<Sprite>(n[n.Length - 1]);
            return asset;
        }
        public Texture2D LoadTexture2D(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadAsset " + typeof(Texture2D).Name + " " + Name + " " + assetName + " return null", true);
                return null;
            }
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            var asset = AssetBundle.LoadAsset<Texture2D>(n[n.Length - 1]);
            return asset;
        }
        // 同步加载场景
        public void LoadScene(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            if (AssetBundle == null) {
                DebugHelper.LogError("LoadScene " + Name + " " + assetName + " return null", true);
                return;
            }
            SceneManager.LoadScene(assetName);
        }
        // 同步加载GameObject
        public GameObject LoadClone(string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            TryReadyAssetBundle(unloadLevel);
            var name = string.IsNullOrEmpty(assetName) ? Name : assetName;
            var n = name.Split('/');
            GameObject obj = null;
            obj = AssetBundle.LoadAsset<GameObject>(n[n.Length - 1]);
            UnityEngine.Object tempObject = null;
            if (obj == null) {
                tempObject = AssetBundle.mainAsset;
                if (tempObject == null) {
                    var allAssets = AssetBundle.LoadAllAssets();
                    foreach (var asset in allAssets) {
                        DebugHelper.LogError(asset.name + asset.GetType(), true);
                    }
                }
                DebugHelper.LogError((name == null ? "aname==null" : name) + " not find,use default" + (tempObject == null ? " 返回空" : (tempObject.GetType() + "--" + tempObject.name)), true);
                return null;
            } else {
                var cloneObj = GameObject.Instantiate(obj);
                return cloneObj;
            }
        }
        public void TryReadyAssetBundle(EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            // Debug.Log("AssetBundleName: " + Name + " IsCached: " + IsCached + " IsFileExist: " + IsFileExist);
            if (!IsCached || !IsFileExist) {
                // Debug.Log("AssetBundleName:" + Name + "DownloadAsset");
                DownloadAsset();
                // TryDownloadAsset();
                if (UnloadLevel != EAssetBundleUnloadLevel.Never) {
                    UnloadLevel = unloadLevel;
                }
            } else if (AssetBundle == null) {
                // Debug.Log("AssetBundleName:" + Name + "LoadFromFile");
                AssetBundle = LoadAssetBundleFromFile(unloadLevel);
            }
        }
        AssetBundle LoadAssetBundleFromFile(EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            try {
                byte[] datas = FileHelp.Read(Name);
                if (datas == null) {
                    return null;
                }
                var assetBundle = AssetBundle.LoadFromMemory(datas);
                if (assetBundle != null) {
                    return assetBundle;
                } else {
                    DebugHelper.LogError("LoadAssetBundleFromFile return null " + Name, true);
                    return null;
                }
            }
            catch (Exception e) {
                DebugHelper.LogError("LoadAssetBundleFromFile AESDecrypt Error finalBundleName: " + Name + " " + e, true);
                return null;
            }
        }
#endregion
        
#region LoadAsyn
        public void TryLoadAsyn(AssetLoader assetLoader, EAssetBundleUnloadLevel unloadLevel, bool isForceInterruptLoad = false) {
            IsForceInterruptLoad = isForceInterruptLoad;
            if (AssetBundle != null) {
                assetLoader.LoadAssetFromBundle(AssetBundle);
            } else {
                if (UnloadLevel != EAssetBundleUnloadLevel.Never) {
                    UnloadLevel = unloadLevel;
                }
                // DebugHelper.Log(Name + "TryLoadAsyn ", true);
                if (isDownloading || isLoading) {
                    assetLoaders.Add(assetLoader);
                } else if (!IsCached || !IsFileExist) {
                    assetLoaders.Add(assetLoader);
                    TryDownloadAsset();
                } else {
                    ReadyLoadAsyn(assetLoader);
                }
            }
        }
        Coroutine asynLoadCoroutine;
        WWW thisWWW;
        // 开始异步加载
        void ReadyLoadAsyn(AssetLoader assetLoader) {
            bool isInCacheFile = false;
            string url = ResourceConstant.GetFileUrl(Name, out isInCacheFile);
            if (asynLoadCoroutine != null) {
                CoroutineHelper.StopCoroutine(asynLoadCoroutine);
            }
            if (string.IsNullOrEmpty(url)) {
                DebugHelper.LogError("LoadAssetBundleAsyn url IsNullOrEmpty " + Name, true);
                LoadError();
                return;
            }
            isLoading = true;
            asynLoadCoroutine = CoroutineHelper.StartCoroutine(LoadAsyn(url, assetLoader));
        }
        IEnumerator LoadAsyn(string url, AssetLoader assetLoader) {
            try {
                thisWWW = new WWW(url);
            } catch (Exception e) {
                DebugHelper.LogError("DownloadAsyn " + url + " error" + e.Message + e.StackTrace, true);
            }
            yield return thisWWW;
            if (thisWWW == null) {
                DebugHelper.LogError("DownloadAsyn " + url + " thisWWW==null", true);
                LoadError();
            } else if (!string.IsNullOrEmpty(thisWWW.error)) {
                DebugHelper.LogError("DownloadAsyn " + thisWWW.error + "URL: " + url, true);
                yield return new WaitForEndOfFrame();
                if (thisWWW != null) {
                    thisWWW.Dispose();
                    thisWWW = null;
                }
                LoadError();
            } else 
                LoadAsynSuccess(url, assetLoader);
        }
        void LoadAsynSuccess(string url, AssetLoader assetLoader) {
            if (AssetBundle == null) 
                AssetBundle = thisWWW.assetBundle;
            isLoading = false;
            if (AssetBundle != null) {
                assetLoader.LoadAssetFromBundle(AssetBundle);
                AssetBundleDownLoadReady();
            } else {
                DebugHelper.LogError("DownloadAsynSuccess thisWWW.assetBundle==null " + url, true);
                assetLoader.LoadError();
            }
        }
        public void LoadError() {
            isLoading = false;
        }
#endregion
    }
}





