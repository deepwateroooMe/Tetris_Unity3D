using System;
using UnityEngine;
using Framework.Util;
using System.Collections;
using Framework.ResMgr;

namespace Framework.ResMgr {

    // Asset加载抽象类
    public abstract class AssetLoader {

        public GameObject root;
        public string bundleName;
        public string assetName;
        public EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.Never;

        public AssetBundleSpec Spec {
            get;
            set;
        }
        public string FinalBundleName {
            get {
                if (!bundleName.EndsWith(ResourceConstant.bundleExtension))
                    return bundleName + ResourceConstant.bundleExtension;
                return bundleName;
            }
        }

        public Action<AssetLoader> onAssetLoadEnd;
        public Action<string> onAssetLoadError;
        public abstract void LoadAssetFromBundle(AssetBundle assetBundle);
        public abstract void LoadError();
    }

    public class AssetLoader<T> : AssetLoader where T : UnityEngine.Object {

        AssetBundleRequest currentRequest;
        public Action<T> onLoadOver;
        public override void LoadAssetFromBundle(AssetBundle assetBundle) {
            CoroutineHelper.StartCoroutine(LoadAssetFromBundleAsync(assetBundle));
        }

        IEnumerator LoadAssetFromBundleAsync(AssetBundle assetBundle) {
            DateTime regTime = DateTime.Now;
            string name = string.IsNullOrEmpty(assetName) ? bundleName : assetName;
            var n = name.Split('/');
            try {
                string finalName = n[n.Length - 1];
                currentRequest = assetBundle.LoadAssetAsync<T>(finalName);
            } catch (Exception e) {
                DebugHelper.LogError(string.Format("LoadAssetAsync [{0}][{1}]Error,[{2}][{3}]", bundleName, assetName, e.Message, e.StackTrace), true);
            }
            yield return currentRequest;
            var costTime = (DateTime.Now - regTime).TotalMilliseconds;
            // DebugHelper.Log(bundleName + " LoadAssetAsync cost " + costTime, true);
            if (currentRequest != null) 
                LoadOver((T)currentRequest.asset);
        }

        protected void LoadOver(T asset) {
            if (onLoadOver != null) {
                try {
                    onLoadOver(asset);
                    if (unloadLevel == EAssetBundleUnloadLevel.LoadOver) 
                        ResourceConstant.Loader.Unload(FinalBundleName, false);
                } catch (Exception e) {
                    DebugHelper.LogError(string.Format("Load [{0}][{1}]Error,[{2}][{3}]", bundleName, assetName, e.Message, e.StackTrace), true);
                }
            }
            if (onAssetLoadEnd != null) 
                onAssetLoadEnd(this);
        }
        public override void LoadError() {
            try {
                LoadOver(null);
            } catch (Exception e) {
                DebugHelper.LogError(FinalBundleName + " loadError " + e.Message + e.StackTrace, true);
            }
            if (onAssetLoadEnd != null) 
                onAssetLoadEnd(this);
        }
    }
}