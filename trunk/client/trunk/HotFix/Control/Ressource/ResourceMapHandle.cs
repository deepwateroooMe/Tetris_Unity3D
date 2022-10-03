using UnityEngine;
using Framework.ResMgr;
using System;
using TMPro;
namespace HotFix {

    public class ResourceMapHandle : ResourceHandleBase {
// 它说它有一个两个程序域间公用接口类的索引;并且公用接口类经过适配可以被两个程序域相认;那么拿到这个索引就可以去调用unity程序集里基本所有的东西了
        IResourceLoader Loader {
            get {
                return ResourceConstant.Loader;
            }
        }
        
#region Load
        public override T LoadAsset<T>(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadAsset<T>(bundleName, assetName, unloadLevel);
        }
        public override TMP_FontAsset LoadTMP_FontAsset(string bundleName, string assetName, 
                                                        EAssetBundleUnloadLevel unloadLevel = 
                                                        EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadTMP_FontAsset(bundleName, assetName, unloadLevel);
        }
        public override Font LoadFont(string bundleName, string assetName, 
                                      EAssetBundleUnloadLevel unloadLevel = 
                                      EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadFont(bundleName, assetName, unloadLevel);
        }
        public override AnimationClip LoadAnimationClip(string bundleName, string assetName, 
                                                        EAssetBundleUnloadLevel unloadLevel = 
                                                        EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadAnimationClip(bundleName, assetName, unloadLevel);
        }
        public override AnimatorOverrideController LoadAnimatorOverrideController(string bundleName, string assetName, 
                                                                                  EAssetBundleUnloadLevel unloadLevel = 
                                                                                  EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadAnimatorOverrideController(bundleName, assetName, unloadLevel);
        }
        public override RuntimeAnimatorController LoadRuntimeAnimatorController(string bundleName, string assetName, 
                                                                                EAssetBundleUnloadLevel unloadLevel = 
                                                                                EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadRuntimeAnimatorController(bundleName, assetName, unloadLevel);
        }
        public override AudioClip LoadAudioClip(string bundleName, string assetName, 
                                                EAssetBundleUnloadLevel unloadLevel = 
                                                EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadAudioClip(bundleName, assetName, unloadLevel);
        }
        public override Material LoadMaterial(string bundleName, string assetName, 
                                              EAssetBundleUnloadLevel unloadLevel = 
                                              EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadMaterial(bundleName, assetName, unloadLevel);
        }
        public override TextAsset LoadTextAsset(string bundleName, string assetName, 
                                                EAssetBundleUnloadLevel unloadLevel = 
                                                EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadTextAsset(bundleName, assetName, unloadLevel);
        }
        public override Sprite LoadSprite(string bundleName, string assetName, 
                                          EAssetBundleUnloadLevel unloadLevel = 
                                          EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadSprite(bundleName, assetName, unloadLevel);
        }
        public override Texture2D LoadTexture2D(string bundleName, string assetName, 
                                                EAssetBundleUnloadLevel unloadLevel = 
                                                EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadTexture2D(bundleName, assetName, unloadLevel);
        }
        public override void LoadScene(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false) {
            Loader.LoadScene(bundleName, assetName, unloadLevel);
        }
        public override GameObject LoadClone(string bundleName, string assetName, 
                                             EAssetBundleUnloadLevel unloadLevel = 
                                             EAssetBundleUnloadLevel.ChangeSceneOver) {
            return Loader.LoadClone(bundleName, assetName, unloadLevel);
        }
#endregion
#region LoadAsyn
        public override void LoadAssetAsyn<T>(string bundleName, string assetName, Action<T> onSuccess, 
                                              EAssetBundleUnloadLevel unloadLevel = 
                                              EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadAssetAsyn<T>(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadTMP_FontAssetAsyn(string bundleName, string assetName, Action<TMP_FontAsset> onSuccess, 
                                                   EAssetBundleUnloadLevel unloadLevel = 
                                                   EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadTMP_FontAssetAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadFontAsyn(string bundleName, string assetName, Action<Font> onSuccess, 
                                          EAssetBundleUnloadLevel unloadLevel = 
                                          EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadFontAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadAnimationClipAsyn(string bundleName, string assetName, Action<AnimationClip> onSuccess, 
                                                   EAssetBundleUnloadLevel unloadLevel = 
                                                   EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadAnimationClipAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadAnimatorOverrideControllerAsyn(string bundleName, string assetName, Action<AnimatorOverrideController> onSuccess, 
                                                                EAssetBundleUnloadLevel unloadLevel = 
                                                                EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadAnimatorOverrideControllerAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadRuntimeAnimatorControllerAsyn(string bundleName, string assetName, Action<RuntimeAnimatorController> onSuccess, 
                                                               EAssetBundleUnloadLevel unloadLevel = 
                                                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadRuntimeAnimatorControllerAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadAudioClipAsyn(string bundleName, string assetName, Action<AudioClip> onSuccess, 
                                               EAssetBundleUnloadLevel unloadLevel = 
                                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadAudioClipAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadMaterialAsyn(string bundleName, string assetName, Action<Material> onSuccess, 
                                              EAssetBundleUnloadLevel unloadLevel = 
                                              EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadMaterialAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadTextAssetAsyn(string bundleName, string assetName, Action<TextAsset> onSuccess, 
                                               EAssetBundleUnloadLevel unloadLevel = 
                                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadTextAssetAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadSpriteAsyn(string bundleName, string assetName, Action<Sprite> onSuccess, 
                                            EAssetBundleUnloadLevel unloadLevel = 
                                            EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadSpriteAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadTexture2DAsyn(string bundleName, string assetName, Action<Texture2D> onSuccess, 
                                               EAssetBundleUnloadLevel unloadLevel = 
                                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadTexture2DAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
        public override void LoadSceneAsyn(string bundleName, string assetName, Action onSuccess, 
                                           EAssetBundleUnloadLevel unloadLevel = 
                                           EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false) {
            Loader.LoadSceneAsyn(bundleName, assetName, onSuccess, unloadLevel, isAddtive);
        }
        public override void LoadCloneAsyn(string bundleName, string assetName, Action<GameObject> onSuccess, 
                                           EAssetBundleUnloadLevel unloadLevel = 
                                           EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) {
            Loader.LoadCloneAsyn(bundleName, assetName, onSuccess, unloadLevel, isForceInterruptLoad);
        }
#endregion
#region Unload
        public override void Unload(string keyName, bool allObjects) {
            Loader.Unload(keyName, allObjects);
        }
        public override void UnloadAll() {
            Loader.UnloadAll();
        }
#endregion
        public override void LoadTexture2DAsyn(string name, Action<Texture2D> onSuccess, Action onFail, bool needCache) {
            Loader.LoadTexture2DAsyn(name, onSuccess, onFail, needCache);
        }
    }
}
