using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.MVVM;
using Framework.Util;
using UnityEngine;

namespace HotFix.Data.Data {

    // follow examples, that only difference is that it has children components, which can also be serialized objects
    public class TetrominoData {
        private const string TAG = "TetrominoData";

        // 家具实例数据
        public class tetrominoData {
            // 实例ID
            public int instanceID;
            // 类型
            public long type;

            // 忘记了原游戏中旋转的逻辑是如何处理的了,需要回头再回去查看一下
            // 方块砖: 位置,与旋转方向都很重要;缩放有两种模式(游戏大方格中的正常比例,与预览中的小尺寸预览)
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

// 方块砖所特有的
            public MinoDataCollection<TetrominoData, MinoData> children { get; private set; } 
#endregion

        // 套路的三个公用方法
            // 反序列化
            public static tetrominoData JsonToObject(string json) {
                tetrominoData data = new tetrominoData();
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


//         public string name { get; set; }
//         public string type { get; set; }
//         public SerializedTransform transform { get; set; }  
//         public MinoDataCollection<TetrominoData, MinoData> children { get; private set; } 
// // 为什么我会写两三个构造器呢?
//         public TetrominoData(Transform parentTrans, string type, string name) {
//             this.name = name;
//             this.type = type;
//             transform = new SerializedTransform(parentTrans);
//             children = new MinoDataCollection<TetrominoData, MinoData>(this);
//             foreach (Transform mino in parentTrans) {
//                 if (mino.CompareTag("mino")) { 
//                     MinoData minoDataItem = new MinoData(mino, new StringBuilder("mino" + type.Substring(5, 1)).ToString()); // shapeX ==> minoX
//                     children.Add(minoDataItem);
//                 }
//             }
//         }
//         public TetrominoData(Transform parentTrans) {
//             transform = new SerializedTransform(parentTrans);
//             children = new MinoDataCollection<TetrominoData, MinoData>(this);
//             foreach (Transform mino in parentTrans) {
//                 if (mino.CompareTag("mino")) {
//                     MinoData minoDataItem = new MinoData(mino);
//                     children.Add(minoDataItem);
//                 }
//             }
//         }
//         public void print() {
//             Debug.Log(TAG + ": Parent TetrominoData: "); 
//             this.transform.print();
//             foreach (var minoData in children) {
//                 Debug.Log(TAG + " minoData.idx: " + minoData.idx); 
//                 minoData.print();
//             }
//         }
    }
}
