using UnityEngine;
using System;
using TMPro;

namespace HotFix {

    public abstract class ResourceHandleBase {
#region Load
        public abstract T LoadAsset<T>(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) where T : UnityEngine.Object;
        public abstract TMP_FontAsset LoadTMP_FontAsset(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract Font LoadFont(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract AnimationClip LoadAnimationClip(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract AnimatorOverrideController LoadAnimatorOverrideController(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract RuntimeAnimatorController LoadRuntimeAnimatorController(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract AudioClip LoadAudioClip(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract Material LoadMaterial(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract TextAsset LoadTextAsset(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract Sprite LoadSprite(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract Texture2D LoadTexture2D(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
        public abstract void LoadScene(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false);
        public abstract GameObject LoadClone(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver);
#endregion
#region LoadAsyn
        public abstract void LoadAssetAsyn<T>(string bundleName, string assetName, Action<T> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) where T : UnityEngine.Object;
        public abstract void LoadTMP_FontAssetAsyn(string bundleName, string assetName, Action<TMP_FontAsset> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadFontAsyn(string bundleName, string assetName, Action<Font> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadAnimationClipAsyn(string bundleName, string assetName, Action<AnimationClip> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadAnimatorOverrideControllerAsyn(string bundleName, string assetName, Action<AnimatorOverrideController> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadRuntimeAnimatorControllerAsyn(string bundleName, string assetName, Action<RuntimeAnimatorController> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadAudioClipAsyn(string bundleName, string assetName, Action<AudioClip> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadMaterialAsyn(string bundleName, string assetName, Action<Material> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadTextAssetAsyn(string bundleName, string assetName, Action<TextAsset> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadSpriteAsyn(string bundleName, string assetName, Action<Sprite> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadTexture2DAsyn(string bundleName, string assetName, Action<Texture2D> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        public abstract void LoadSceneAsyn(string bundleName, string assetName, Action onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false);
        public abstract void LoadCloneAsyn(string bundleName, string assetName, Action<GameObject> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
#endregion
#region Unload
        public abstract void Unload(string keyName, bool allObjects);
        public abstract void UnloadAll();
#endregion
        public abstract void LoadTexture2DAsyn(string name, Action<Texture2D> onSuccess, Action onFail, bool needCache);
    }
}
