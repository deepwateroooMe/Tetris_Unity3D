using Framework.Util;
using System.Collections.Generic;
using System.Json;

// 场景实例数据
public class SceneData {
    public int instanceID;
    public int type;

    public Dictionary<string, OneData> oneDatas;
    
    public SceneData JsonToObject(string json) {
        SceneData data = new SceneData();
        JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
        if (jsonObject != null) {
            data.instanceID = jsonObject["instanceID"];
            data.type = jsonObject["type"];
// ldfkjdflkdkfjdkj
        }
        return data;
    }
    public override string ToString() {
        return ObjectToJson().ToString();
    }
    public JsonObject ObjectToJson() {
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("instanceID", instanceID);
        jsonObject.Add("type", type);
        JsonArray jsonArray = new JsonArray();

        foreach (var data in oneDatas.Values) {
            JsonObject dataJsonObject = data.ObjectToJson();
            jsonArray.Add(dataJsonObject);
        }
        jsonObject.Add("oneDatas", jsonArray.ToString());
// .....
// 可以有好几种不同类型的数据
        return jsonObject;
    }
}
