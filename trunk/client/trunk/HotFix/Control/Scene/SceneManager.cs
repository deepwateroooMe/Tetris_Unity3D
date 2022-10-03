using System;
using UnityEngine;
using HotFix.Data.TypedData;
using HotFix.Data;
using Framework.ResMgr;
using Object = UnityEngine.Object;

namespace HotFix.Control.Scene {

// 场景管理器: 我的游戏里也写过好几个不同的管理类，热更新里的管理类与unity里的普通管理类有什么不同呢？
    public class SceneManager { // unity里不同场景的编号不同,　这里以int 值不同来区分不同的游戏场景

        private static SceneManager instance;　// 希望是单例模式，不涉及多线程安全
        public static SceneManager Instance {
            get {
                if (instance == null) 
                    instance = new SceneManager();
                return instance;
            }
        }

        // 当前场景
        public SceneBase CurrentScene {
            get;
            set;
        }
        public int currentSelectGameObjectInstanceID;

        // 创建一个新场景：根据传进来的参数值，来轮询实例化对象的场景（这个包裹里不是也定义了各个不同场景的继承于抽象基类的继承类的反序列化方法了吗？）
        public void CreateNewScene(int type) { // unity里不同场景的编号不同,　这里以int 值不同来区分不同的游戏场景
            ClearLastSceneGameObject();
            currentSelectGameObjectInstanceID = 100000001;
            SceneTypeData typeData = TypeDataManager.GetSceneTypeData(type);
// 如果我确定只要一个场景的话,我就不用切换场景,就只一个场景就可以了
            //if (typeData.type == (int)ESceneType.Edit) {
            //    CurrentScene = new EditScene(type);
            //    CurrentScene.LoadSceneGameObject();
            //} else if (typeData.type == (int)ESceneType.Show) {
            //    CurrentScene = new ShowScene(type);
            //    CurrentScene.LoadSceneGameObject();
            //} else if (typeData.type == (int)ESceneType.Camera) {
            //    CurrentScene = new CameraScene(type);
            //    CurrentScene.LoadSceneGameObject();
            //}
        }

        // 加载一个场景
        public void LoadScene(SceneData data) {
            ClearLastSceneGameObject();
            currentSelectGameObjectInstanceID = data.GetMaxFurnitureInstanceID();
            SceneTypeData typeData = TypeDataManager.GetSceneTypeData(data.type);
            //if (typeData.type == (int)ESceneType.Edit) {
            //    CurrentScene = new EditScene(data);
            //    CurrentScene.LoadSceneGameObject();
            //} else if (typeData.type == (int)ESceneType.Show) {
            //    CurrentScene = new ShowScene(data);
            //    CurrentScene.LoadSceneGameObject();
            //} else if (typeData.type == (int)ESceneType.Camera) {
            //    CurrentScene = new CameraScene(data);
            //    CurrentScene.LoadSceneGameObject();
            //}
        }
        void ClearLastSceneGameObject() {
            if (CurrentScene != null && CurrentScene.GameObject != null) 
                Object.DestroyImmediate(CurrentScene.GameObject);
        }
        public void CleanCurrentScene() {
            if (CurrentScene != null && CurrentScene.GameObject != null) {
                
                Object.DestroyImmediate(CurrentScene.GameObject);
                CurrentScene.Dispose();
                CurrentScene = null;
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }
        }
        // 加载某个特殊的场景：那么就是说对于某类过于特殊的场景，可以将其单独打包成一个资源包上传服务器和从服务器下载下来加载
        // 这里这个特殊的场景，好像不涉及任何其它数据（除了一个应用里的SceneData）？所以过程极为简单，可以跳过狠多步
        // 再想一下：这个特殊的场景到底特殊在哪里，可以独立有条短路的加载方法？
        public void LoadShowScene(string bundleName, string assetName) {
            string json = ResourceConstant.Loader.LoadTextAsset(bundleName, assetName, EAssetBundleUnloadLevel.LoadOver).text;
            //Debug.Log("json: " + json);
            SceneData sceneData = SceneData.JsonToObject(json);
            LoadScene(sceneData);
        }
    }
}


