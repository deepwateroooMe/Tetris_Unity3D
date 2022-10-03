using Framework.Util;
using System.Json;

namespace HotFix.Data.TypedData {

// 这里可以自定义几个不同的场景类型，方便程序区分
    public enum ESceneType {
        None = 0,
        Edit = 1,
        Show = 2,
        Camera = 3
    }

// 场景类型数据
    public class SceneTypeData {
        public long id;
        public string gameObjectName;
        public string name;
        public string description;

        public int type;
        public string bundleName;
        public string assetName;
        public string iconBundleName;
        public string iconAssetName;

        // 把序列化数据重新反序列化成unity场景数据
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
