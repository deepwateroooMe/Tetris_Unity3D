using System;
using TMPro;
using UnityEngine;
//using TMPro;

namespace Framework.ResMgr {

    public interface IResourceLoader {

#region Load
        T LoadAsset<T>(string bundleName, string assetName, 
                       EAssetBundleUnloadLevel unloadLevel = 
                       EAssetBundleUnloadLevel.ChangeSceneOver) where T : UnityEngine.Object;
        TMP_FontAsset LoadTMP_FontAsset(string bundleName, string assetName, 
                                       EAssetBundleUnloadLevel unloadLevel = 
                                       EAssetBundleUnloadLevel.ChangeSceneOver);
        Font LoadFont(string bundleName, string assetName, 
                      EAssetBundleUnloadLevel unloadLevel = 
                      EAssetBundleUnloadLevel.ChangeSceneOver);
        AnimationClip LoadAnimationClip(string bundleName, string assetName, 
                                        EAssetBundleUnloadLevel unloadLevel = 
                                        EAssetBundleUnloadLevel.ChangeSceneOver);
        AnimatorOverrideController LoadAnimatorOverrideController(string bundleName, string assetName, 
                                                                  EAssetBundleUnloadLevel unloadLevel = 
                                                                  EAssetBundleUnloadLevel.ChangeSceneOver);
        RuntimeAnimatorController LoadRuntimeAnimatorController(string bundleName, string assetName, 
                                                                EAssetBundleUnloadLevel unloadLevel = 
                                                                EAssetBundleUnloadLevel.ChangeSceneOver);
        AudioClip LoadAudioClip(string bundleName, string assetName, 
                                EAssetBundleUnloadLevel unloadLevel = 
                                EAssetBundleUnloadLevel.ChangeSceneOver);
        Material LoadMaterial(string bundleName, string assetName, 
                              EAssetBundleUnloadLevel unloadLevel = 
                              EAssetBundleUnloadLevel.ChangeSceneOver);
        TextAsset LoadTextAsset(string bundleName, string assetName, 
                                EAssetBundleUnloadLevel unloadLevel = 
                                EAssetBundleUnloadLevel.ChangeSceneOver);
        Sprite LoadSprite(string bundleName, string assetName, 
                          EAssetBundleUnloadLevel unloadLevel = 
                          EAssetBundleUnloadLevel.ChangeSceneOver);
        Texture2D LoadTexture2D(string bundleName, string assetName, 
                                EAssetBundleUnloadLevel unloadLevel = 
                                EAssetBundleUnloadLevel.ChangeSceneOver);
        void LoadScene(string bundleName, string assetName, 
                       EAssetBundleUnloadLevel unloadLevel = 
                       EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false);
        GameObject LoadClone(string bundleName, string assetName, 
                             EAssetBundleUnloadLevel unloadLevel = 
                             EAssetBundleUnloadLevel.ChangeSceneOver);
#endregion

#region LoadAsyn
        void LoadAssetAsyn<T>(string bundleName, string assetName, Action<T> onSuccess, 
                              EAssetBundleUnloadLevel unloadLevel = 
                              EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false) where T : UnityEngine.Object;
        void LoadTMP_FontAssetAsyn(string bundleName, string assetName, Action<TMP_FontAsset> onSuccess, 
                                  EAssetBundleUnloadLevel unloadLevel = 
                                  EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadFontAsyn(string bundleName, string assetName, Action<Font> onSuccess, 
                          EAssetBundleUnloadLevel unloadLevel = 
                          EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadAnimationClipAsyn(string bundleName, string assetName, Action<AnimationClip> onSuccess, 
                                   EAssetBundleUnloadLevel unloadLevel = 
                                   EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadAnimatorOverrideControllerAsyn(string bundleName, string assetName, Action<AnimatorOverrideController> onSuccess, 
                                                EAssetBundleUnloadLevel unloadLevel = 
                                                EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadRuntimeAnimatorControllerAsyn(string bundleName, string assetName, Action<RuntimeAnimatorController> onSuccess, 
                                               EAssetBundleUnloadLevel unloadLevel = 
                                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadAudioClipAsyn(string bundleName, string assetName, Action<AudioClip> onSuccess, 
                               EAssetBundleUnloadLevel unloadLevel = 
                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadMaterialAsyn(string bundleName, string assetName, Action<Material> onSuccess, 
                              EAssetBundleUnloadLevel unloadLevel = 
                              EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadTextAssetAsyn(string bundleName, string assetName, Action<TextAsset> onSuccess, 
                               EAssetBundleUnloadLevel unloadLevel = 
                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadSpriteAsyn(string bundleName, string assetName, Action<Sprite> onSuccess, 
                            EAssetBundleUnloadLevel unloadLevel = 
                            EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadTexture2DAsyn(string bundleName, string assetName, Action<Texture2D> onSuccess, 
                               EAssetBundleUnloadLevel unloadLevel = 
                               EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
        void LoadSceneAsyn(string bundleName, string assetName, Action onSuccess, 
                           EAssetBundleUnloadLevel unloadLevel = 
                           EAssetBundleUnloadLevel.ChangeSceneOver, bool isAddtive = false);
        void LoadCloneAsyn(string bundleName, string assetName, Action<GameObject> onSuccess, 
                           EAssetBundleUnloadLevel unloadLevel = 
                           EAssetBundleUnloadLevel.ChangeSceneOver, bool isForceInterruptLoad = false);
#endregion

#region Unload
        void Unload(string keyName, bool allObjects);
        void UnloadAll();
#endregion

        void LoadTexture2DAsyn(string name, Action<Texture2D> onSuccess, Action onFail, bool needCache);
    }
}