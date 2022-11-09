using System;
using TMPro;
using UnityEngine;

namespace HotFix {
    // 资源加载接口类
    public class ResourceHelper {
        static ResourceHandleBase _handle;
        static ResourceHandleBase Handle {
            get {
                if (_handle == null) {
                    _handle = new ResourceMapHandle();
                }
                return _handle;
            }
        }
#region Load
        public static T LoadAsset<T>(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) where T : UnityEngine.Object {
            return Handle.LoadAsset<T>(bundleName, assetName, unloadLevel);
        }
        public static TMP_FontAsset LoadTMP_FontAsset(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadTMP_FontAsset(bundleName, assetName, unloadLevel);
        }
        public static Font LoadFont(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadFont(bundleName, assetName, unloadLevel);
        }
        public static AnimationClip LoadAnimationClip(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadAnimationClip(bundleName, assetName, unloadLevel);
        }
        public static AnimatorOverrideController LoadAnimatorOverrideController(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadAnimatorOverrideController(bundleName, assetName, unloadLevel);
        }

		internal static Material LoadMaterialAsyn(string v1, string v2) => throw new NotImplementedException();

		public static RuntimeAnimatorController LoadRuntimeAnimatorController(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadRuntimeAnimatorController(bundleName, assetName, unloadLevel);
        }
        public static AudioClip LoadAudioClip(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadAudioClip(bundleName, assetName, unloadLevel);
        }
        public static Material LoadMaterial(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadMaterial(bundleName, assetName, unloadLevel);
        }
        public static TextAsset LoadTextAsset(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadTextAsset(bundleName, assetName, unloadLevel);
        }
        public static Sprite LoadSprite(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadSprite(bundleName, assetName, unloadLevel);
        }
        public static Texture2D LoadTexture2D(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadTexture2D(bundleName, assetName, unloadLevel);
        }
        public static void LoadScene(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isAdditive = false) {
            Handle.LoadScene(bundleName, assetName, unloadLevel, isAdditive);
        }
        public static GameObject LoadClone(string bundleName, string assetName, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Handle.LoadClone(bundleName, assetName, unloadLevel);
        }
#endregion
#region LoadAsyn
        public static void LoadAssetAsyn<T>(string bundleName, string assetName, Action<T> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) where T : UnityEngine.Object {
            Handle.LoadAssetAsyn<T>(bundleName, assetName, onSuccess, unloadLevel);
        }
        public static void LoadTMP_FontAssetAsyn(string bundleName, string assetName, Action<TMP_FontAsset> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadTMP_FontAssetAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadFontAsyn(string bundleName, string assetName, Action<Font> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadFontAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadAnimationClipAsyn(string bundleName, string assetName, Action<AnimationClip> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadAnimationClipAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadAnimatorOverrideControllerAsyn(string bundleName, string assetName, Action<AnimatorOverrideController> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadAnimatorOverrideControllerAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadRuntimeAnimatorControllerAsyn(string bundleName, string assetName, Action<RuntimeAnimatorController> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadRuntimeAnimatorControllerAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadAudioClipAsyn(string bundleName, string assetName, Action<AudioClip> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadAudioClipAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadMaterialAsyn(string bundleName, string assetName, Action<Material> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadMaterialAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadTextAssetAsyn(string bundleName, string assetName, Action<TextAsset> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadTextAssetAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public static void LoadSpriteAsyn(string bundleName, string assetName, Action<Sprite> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadSpriteAsyn(bundleName, assetName, onSuccess, unloadLevel);
        }
        public static void LoadTexture2DAsyn(string bundleName, string assetName, Action<Texture2D> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadTexture2DAsyn(bundleName, assetName, onSuccess, unloadLevel);
        }
        public static void LoadSceneAsyn(string bundleName, string assetName, Action onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isAdditive = false) {
            Handle.LoadSceneAsyn(bundleName, assetName, onSuccess, unloadLevel, isAdditive);
        }
        public static void LoadCloneAsyn(string bundleName, string assetName, Action<GameObject> onSuccess, EAssetBundleUnloadLevel unloadLevel = EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Handle.LoadCloneAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
#endregion
#region Unload
#endregion
        public static void LoadTexture2DAsyn(string name, Action<Texture2D> onSuccess, Action onFail, bool needCache = true) {
            Handle.LoadTexture2DAsyn(name, onSuccess, onFail, needCache);
        }
    }
}
