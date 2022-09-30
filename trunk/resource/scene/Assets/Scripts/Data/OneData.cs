using Framework.Util;
using System.Json;

public class OneData {
    public int instanceID;
    public long type;
    
#region Transform
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
#endregion

    public OneData JsonToObject(string json) {
        OneData data = new OneData();
        JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
        if (jsonObject != null) {
            data.instanceID = jsonObject["instanceID"];
            data.type = jsonObject["type"];
            data.positionX = jsonObject["positionX"];
            data.positionY = jsonObject["positionY"];
            data.positionZ = jsonObject["positionZ"];
            data.rotationX = jsonObject["rotationX"];
            data.rotationY = jsonObject["rotationY"];
            data.rotationZ = jsonObject["rotationZ"];
            data.scaleX = jsonObject["scaleX"];
            data.scaleY = jsonObject["scaleY"];
            data.scaleZ = jsonObject["scaleZ"];
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
        jsonObject.Add("positionX", positionX);
        jsonObject.Add("positionY", positionY);
        jsonObject.Add("positionZ", positionZ);
        jsonObject.Add("rotationX", rotationX);
        jsonObject.Add("rotationY", rotationY);
        jsonObject.Add("rotationZ", rotationZ);
        jsonObject.Add("scaleX", scaleX);
        jsonObject.Add("scaleY", scaleY);
        jsonObject.Add("scaleZ", scaleZ);
        return jsonObject;
    }
}
