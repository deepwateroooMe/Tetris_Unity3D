using UnityEngine;
using Framework.Util;
using System;
using System.Collections;

namespace Framework.ResMgr {

    // GameObjectAsset加载类
    public class GameObjectLoader : AssetLoader {
        AssetBundleRequest currentRequest;
        public Action<GameObject> onCloneOver;
        public override void LoadAssetFromBundle(AssetBundle assetBundle) {
            CoroutineHelper.StartCoroutine(LoadAssetFromBundleAsync(assetBundle));
        }

        IEnumerator LoadAssetFromBundleAsync(AssetBundle assetBundle) {
            var name = string.IsNullOrEmpty(assetName) ? bundleName : assetName;
            var n = name.Split('/');
            try {
                string finalName = n[n.Length - 1];
                currentRequest = assetBundle.LoadAssetAsync<GameObject>(finalName);
            }
            catch (Exception e) {
                DebugHelper.LogError(string.Format("LoadAssetAsync [{0}][{1}]Error,[{2}][{3}]", bundleName, assetName, e.Message, e.StackTrace), true);
            }
            yield return currentRequest;
            if (currentRequest != null) {
                LoadGameObject(assetBundle, currentRequest.asset, name);
            }
        }

        void LoadGameObject(AssetBundle ab, UnityEngine.Object asset, string aname) {
            var b = asset;
            if (b != null) {
                var clone = GameObject.Instantiate(b) as GameObject;
                CloneOver(clone);
            } else {
                DebugHelper.LogError((aname == null ? "aname==null" : aname) + " not find,use default" + (b == null ? " 返回空" : (b.GetType() + "--" + b.name)), true);
            }
        }

        protected void CloneOver(GameObject go) {
            if (!go) 
                return;
            if (root) 
                go.transform.parent = root.transform;
            // go.transform.localPosition = Vector3.zero;
            // go.transform.localScale = Vector3.one;
            // go.transform.localRotation = new Quaternion();
            try {
                if (onCloneOver != null) {
                    onCloneOver(go);
                    if (unloadLevel == EAssetBundleUnloadLevel.LoadOver) 
                        ResourceConstant.Loader.Unload(FinalBundleName, false);
                }
            } catch (Exception e) {
                DebugHelper.LogError(string.Format("CloneOver [{0}][{1}]Error,[{2}][{3}]", bundleName, assetName, e.Message, e.StackTrace), true);
            }
            if (onAssetLoadEnd != null) 
                onAssetLoadEnd(this);
        }
        public override void LoadError() {
            try {
                if (onCloneOver != null) 
                    onCloneOver(null);
            } catch (Exception e) {
                DebugHelper.LogError(FinalBundleName + " loadError " + e.Message + e.StackTrace, true);
            }
            if (onAssetLoadEnd != null) 
                onAssetLoadEnd(this);
        }
    }
}
