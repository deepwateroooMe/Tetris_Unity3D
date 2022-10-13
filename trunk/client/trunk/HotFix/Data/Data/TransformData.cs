using Framework.Util;
using System.Json;

namespace HotFix.Data {

    // 某个实例(一个立方体)的位置旋转缩放相关的数据: 最简单基础的json的序列化与反序列化; 核心会是后来的方块砖实现设计逻辑

// 就像是打资源包大小的粒度考虑,这里也有一个序列化与反序列化的粒度考虑: 是以每个方块砖为最小单位来序列化,还是说以最小单位的立方体来序列化呢?
    // 概念混淆:这里可能有概念上的混淆,当我打资源包的时候,比如我想要在目前有7种不同方块砖的基础上再增加3个,那么我资源包时的序/反序列化的对象是3个新的,在方块砖的精度上;
    // 但是考虑到游戏过程中的消除与再生,同样需要提供小立方体粒度级别的资源支持支撑消除与再生
    // 所以每种类型的Tetromino资源包里同样搭配一个其类型的小立方体资源(那么我的这些个组件就会相对复杂一点儿,这里需要OOP/OOD设计理念上的提取共同点,把每个方块砖都可能有的公用方法提取到一个抽象类里定义为基类)
    
    public class TransformData {

// 所有序列化与反序列化的对象都是特异的个体,所以还是需要id与类型相区分的
        // 实例ID
        public int instanceID;
        // 家具类型
        public long type;

#region Transform
        // PositionX
        public float positionX;
        // PositionY
        public float positionY;
        // PositionZ
        public float positionZ;
        // EulerAngleX
        public float rotationX;
        // EulerAngleY
        public float rotationY;
        // EulerAngleZ
        public float rotationZ;
        // ScaleX
        public float scaleX;
        // ScaleY
        public float scaleY;
        // ScaleZ
        public float scaleZ;
#endregion

// 任何实例都可以有的3个公用方法        
        // 反序列化
        public static TransformData JsonToObject(string json) {
            TransformData data = new TransformData();
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

        // 序列化
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
}
