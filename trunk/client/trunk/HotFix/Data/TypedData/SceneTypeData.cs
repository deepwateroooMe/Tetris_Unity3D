using Framework.Util;
using System.Json;

namespace HotFix.Data {

// 这里可以自定义几个不同的场景类型，方便程序区分: 这里是同一家公司里的几个不同的应用,使用了同一个热更新程序集
// 这个类也包括其所在的资源包的一些相关信息
    public enum ESceneType { // 每个应用对应于其中的一个场景
        None = 0,
        StartUp = 1,
        Show = 2,
        Camera = 3
    }

// 场景类型数据:　场景数据的反序列化,由Json字条串反序列化为SceneTypeData
    public class SceneTypeData { // 场景的反序列化数据定义
        public long id;
        public string gameObjectName;
        public string name;
        public string description;

        public int type;
        public string bundleName;
        public string assetName;
        public string iconBundleName;
        public string iconAssetName;

        // 把序列化过的场景数据字符串 重新反序列化成 SceneTypeData
        public static SceneTypeData JsonToObject(string json) {
            SceneTypeData typeData = new SceneTypeData();
            JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
            if (jsonObject != null) {
                typeData.id = jsonObject["id"];
                typeData.gameObjectName = jsonObject["gameObjectName"].ToString();
                typeData.name = jsonObject["name"].ToString();
                typeData.description = jsonObject["description"].ToString();
                typeData.type = jsonObject["type"];
                typeData.bundleName = jsonObject["bundleName"].ToString();
                typeData.assetName = jsonObject["assetName"].ToString();
                typeData.iconBundleName = jsonObject["iconBundleName"].ToString();
                typeData.iconAssetName = jsonObject["iconAssetName"].ToString();
            }
            return typeData;
        }

        public override string ToString() {
            return "id: " + id + " gameObjectName: " + gameObjectName + " name: " + name + " description: " + description
                + " type: " + type + " bundleName: " + bundleName + " assetName: " + assetName
                + " iconBundleName: " + iconBundleName + " iconAssetName: " + iconAssetName;
        }
    }
}
