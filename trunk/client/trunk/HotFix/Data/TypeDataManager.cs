using Framework.Util;
using System.Collections.Generic;
using System.Json;
using HotFix.Data.TypedData;
using UnityEngine;

namespace HotFix.Data {

// TypeData管理器: 静态管理类,不同场景类型,不同Tetromino类型的反序列化初始化,加载场景以及序列化的物体
// 更高一层的管理层：负责对某类特定资源包(比如场景资源包)里的产资源数组反序列化为数组元素类型的TypeData,并对这类TypeData统一管理
// 这里的层级结构就相当于是: GameData里我保存游戏场景时的几个部件,它们的序列化又是在另一个层次层面
    
    public static class TypeDataManager { // 场景的加载,场景里各可序列化,序列化过的各种不同物件的反序列化与初始化

#region TypeDatas
        // 对于每种自定义自封装的类型，启用一个字典来进行管理;同自定义类型的数据，用一个长量型的long作为key来进行区分实例
        static Dictionary<long, TetrominoTypeData> tetrominoTypeDatas;
        static Dictionary<long, SceneTypeData> sceneTypeDatas;

        public static Dictionary<long, TetrominoTypeData> GetTetrominoTypeDatas() {
            return tetrominoTypeDatas;
        }
        public static Dictionary<long, SceneTypeData> GetSceneTypeDatas() {
           return sceneTypeDatas;
        }
        
        public static TetrominoTypeData GetTetrominoTypeData(long id) {
           if (tetrominoTypeDatas.ContainsKey(id)) {
               return tetrominoTypeDatas[id];
           } else {
               return null;
           }
        }
        public static SceneTypeData GetSceneTypeData(long id) {
            if (sceneTypeDatas.ContainsKey(id)) {
                return sceneTypeDatas[id];
            } else {
                return null;
            }
        }
#endregion
        // 热更新起始时，资源包里：对于不同场景的初始化;
        // 这里是在热更新程序资源集里
        // 热更新程序域:它所拥有的是自己IType子类型的各种数据
        
        public static void InitializeTypeDatas() { // 那么加载的是场景（场景专用资源包吗？是的；不是资源包里关于场景的那一小部分）场景分场景新技术包；小控件分小控件打包
            //string sceneJson = ResourceHelper.LoadTextAsset("scene/config/scene", "scene", EAssetBundleUnloadLevel.LoadOver).text;
            //if (!string.IsNullOrEmpty(sceneJson)) // 只要从场景资源包里的读出的字符串非空，就反序列化成特定类型备用
            //    InitializeSceneTypeData(sceneJson);
            // 游戏中我并不只光光用到场景数据,还用到其它比如不同的俄罗斯方块砖等,这些资源也都是需要一一加载的
            
        }
        static void InitializeSceneTypeData(string jsonStr) { // 反序列化，将序列化数据反转成通用场景数据
            if (jsonStr != null) {
                //sceneTypeDatas = new Dictionary<long, SceneTypeData>(); // SceneTypeData
                //JsonArray jsonArray = JsonSerializer.Deserialize(jsonStr) as JsonArray;
                //if (jsonArray != null) {
                //    foreach (JsonValue jsonValue in jsonArray) {
                //        SceneTypeData typeData = SceneTypeData.JsonToObject(jsonValue.ToString());
                //        if (!sceneTypeDatas.ContainsKey(typeData.id)) // 当前资源管理器还没有这种类型（int id）的场景呢，就添加上，否则抛出异常
                //            sceneTypeDatas.Add(typeData.id, typeData);
                //        else // 当前场景类型资源已添加,给个提示
                //            Debug.LogError("sceneTypeDatas contains key: " + typeData.id);
                //    }
                //} else 
                //    Debug.LogError("sceneTypeData jsonArray is null");
            }
        }
    }
}


