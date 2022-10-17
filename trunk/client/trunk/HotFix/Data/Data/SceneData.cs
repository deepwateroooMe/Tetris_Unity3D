using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.Util;

namespace HotFix.Data {

    // 场景实例数据: 像是一个基类,是所有场景数据所共有的
    public class SceneData { //　再去多想一想，为什么要用SceneData 与SceneTypeData相区分？

        // 实例ID
        public int instanceID;
        // 场景类型
        public int type;

// 这是同一家公司或是相同架构公司多个不同游戏的framework游戏框架通用管理
        //　接下来：需要对各自游戏应用各不同场景下的Model数据进行集装，封装到自定义的场景热更自定义类型里去
        // 可是用集合类对同一场景下的同一数据类型进行集装；
        // 同一场景下不同数据类型间，可以生成多个不同字典等集合数据结构进行集装

// 接下来就是场景中的元件:各类单个数据,或是各类各种不同数据的集合        
        // 单个数据
        // public SingleData singleData; // for tmp
        // 对于游戏场景中的大方格中的方块砖与立方体,可能要采用不同的数据结构来保存,这里不是在保存
        // //  某数据类型的集合
        public Dictionary<int, TetrominoData> tetrominoDatas;
        // // 另一数据类型的集合
        // public Dictionary<int, TypetwoData> typetwoDatas;

        // // 反序列化
        // public static SceneData JsonToObject(string json) {
        //     SceneData data = new SceneData();
        //     JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
        //     if (jsonObject != null) {
        //         if (jsonObject.ContainsKey("instanceID")) {
        //             data.instanceID = jsonObject["instanceID"];
        //         }
        //         data.type = jsonObject["type"];
        //         if (jsonObject.ContainsKey("singleData")) {
        //             JsonValue singleJonValue = jsonObject["singleData"];
        //             // data.singleData = SingleData.JsonToObject(singleJonValue.ToString());
        //         }
        //         data.typeoneDatas = new Dictionary<string, TypeoneData>();
        //         JsonValue jsonValue = jsonObject["typeoneDatas"];
        //         JsonArray jsonArray = JsonSerializer.Deserialize(jsonValue.ToString()) as JsonArray;
        //         foreach (var value in jsonArray) {
        //             TypeoneData typeoneData = TypeoneData.JsonToObject(value.ToString());
        //             data.typeoneDatas.Add(typeoneData.gameObjectName, typeoneData);
        //         }
        //         data.typetwoDatas = new Dictionary<int, TypetwoData>();
        //         JsonValue jsonValue2 = jsonObject["typetwoDatas"];
        //         JsonArray jsonArray2 = JsonSerializer.Deserialize(jsonValue2.ToString()) as JsonArray;
        //         foreach (var value in jsonArray2) {
        //             TypetwoData typetwoData = TypetwoData.JsonToObject(value.ToString());
        //             data.typetwoDatas.Add(typetwoData.instanceID, typetwoData);
        //         }
        //     }
        //     return data;
        // }
        // public override string ToString() {
        //     return ObjectToJson().ToString();
        // }
        // // 序列化
        // public JsonObject ObjectToJson() {
        //     JsonObject jsonObject = new JsonObject();
        //     jsonObject.Add("instanceID", instanceID);
        //     jsonObject.Add("type", type);
        //     JsonObject singleJsonObject = singleData.ObjectToJson();
        //     jsonObject.Add("singleData", singleJsonObject);
        //     JsonArray jsonArray = new JsonArray();
        //     foreach (var data in typeoneDatas.Values) {
        //         JsonObject dataJsonObject = data.ObjectToJson();
        //         jsonArray.Add(dataJsonObject);
        //     }
        //     jsonObject.Add("typeoneDatas", jsonArray.ToString());
        //     JsonArray jsonArray2 = new JsonArray();
        //     foreach (var data in typetwoDatas.Values) {
        //         JsonObject dataJsonObject = data.ObjectToJson();
        //         jsonArray2.Add(dataJsonObject);
        //     }
        //     jsonObject.Add("typetwoDatas", jsonArray2.ToString());
        //     return jsonObject;
        // }
        // public int GetMaxTypetwoInstanceID() {
        //     int maxID = 100000001;
        //     foreach (var key in typetwoDatas.Keys) {
        //         if (key > maxID) {
        //             maxID = key;
        //         }
        //     }
        //     return maxID;
        // }
    }
}
