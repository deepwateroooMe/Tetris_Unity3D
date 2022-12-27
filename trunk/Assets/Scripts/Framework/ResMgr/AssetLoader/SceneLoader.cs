using System;
using UnityEngine;
using Framework.Util;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Framework.ResMgr {

    // 场景Asset加载类
    public class SceneLoader : AssetLoader {
        public bool isAdditive;
        public Action onLoadOver;

        public override void LoadAssetFromBundle(AssetBundle assetBundle) {
            CoroutineHelper.StartCoroutine(LoadAssetFromBundleAsync(assetBundle));
        }

        IEnumerator LoadAssetFromBundleAsync(AssetBundle assetBundle) {
            if (isAdditive) {
                yield return SceneManager.LoadSceneAsync(assetName, LoadSceneMode.Additive);
            } else {
                yield return SceneManager.LoadSceneAsync(assetName);
            }
            LoadOver();
        }

        protected void LoadOver() {
            if (onLoadOver != null) {
                try {
                    onLoadOver();
                    if (unloadLevel == EAssetBundleUnloadLevel.LoadOver) {
                        ResourceConstant.Loader.Unload(FinalBundleName, false);
                    }
                }
                catch (Exception e) {
                    DebugHelper.LogError(string.Format("Load [{0}][{1}]Error,[{2}][{3}]", bundleName, assetName, e.Message, e.StackTrace), true);
                }
            }
            if (onAssetLoadEnd != null) {
                onAssetLoadEnd(this);
            }
        }

        public override void LoadError() {
            try {
                LoadOver();
            }
            catch (Exception e) {
                DebugHelper.LogError(FinalBundleName + " loadError " + e.Message + e.StackTrace, true);
            }
            if (onAssetLoadEnd != null) {
                onAssetLoadEnd(this);
            }
        }
    }
}
