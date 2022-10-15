using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using UnityEngine;

namespace HotFix.Data.Data {

    // https:// thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/ 很详细的解释
    public interface IMinoData<P> where P : class { // Defines the contract for an object that has a parent object
        P Parent { get; set; }  
    }

    public class MinoData : IMinoData<TetrominoData> {
        private const string TAG = "MinoData";

        public int instanceID;
        public long type;

        public TetrominoData parentData; 

#region Transform
        // PositionX
        public float positionX;
        // PositionY
        public float positionY;
        // PositionZ
        public float positionZ;
// 对于小Mino来说,因为它是一个立方体,其实它只需要知道一个位置信息就可以了
        // // EulerAngleX
        // public float rotationX;
        // // EulerAngleY
        // public float rotationY;
        // // EulerAngleZ
        // public float rotationZ;
        // // ScaleX
        // public float scaleX;
        // // ScaleY
        // public float scaleY;
        // // ScaleZ
        // public float scaleZ;
#endregion

#region IMinoItem<TetrominoData> Members
        TetrominoData IMinoData<TetrominoData>.Parent {
            get {
                return this.parentData;
            }
            set {
                this.parentData = value;
            }
        }
#endregion

        // 反序列化
        public static MinoData JsonToObject(string json) {
                MinoData data = new MinoData();
                JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
                if (jsonObject != null) {
                    data.instanceID = jsonObject["instanceID"];
                    data.type = jsonObject["type"];
                    data.positionX = jsonObject["positionX"];
                    data.positionY = jsonObject["positionY"];
                    data.positionZ = jsonObject["positionZ"];
                    // data.rotationX = jsonObject["rotationX"];
                    // data.rotationY = jsonObject["rotationY"];
                    // data.rotationZ = jsonObject["rotationZ"];
                    // data.scaleX = jsonObject["scaleX"];
                    // data.scaleY = jsonObject["scaleY"];
                    // data.scaleZ = jsonObject["scaleZ"];
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
                // jsonObject.Add("rotationX", rotationX);
                // jsonObject.Add("rotationY", rotationY);
                // jsonObject.Add("rotationZ", rotationZ);
                // jsonObject.Add("scaleX", scaleX);
                // jsonObject.Add("scaleY", scaleY);
                // jsonObject.Add("scaleZ", scaleZ);
                return jsonObject;
            }
    }
        
//         public int idx { get; set; } // 这里原本是用一位标来标注的是什么信息呢?
//         public string type { get; set; }
//         public SerializedTransform transform { get; set; }
//         [NonSerialized]
//         public TetrominoData parentData; // { get; internal set; } 
// #region IMinoItem<TetrominoData> Members
//         TetrominoData IMinoDataItem<TetrominoData>.Parent {
//             get {
//                 return this.parentData;
//             }
//             set {
//                 this.parentData = value;
//             }
//         }
// #endregion

    // // 这里当初游戏中的几个构造函数,到用时再补上
    //     public MinoData(Transform trans) {
    //         this.idx = MathUtil.getIndex(trans);
    //         this.transform = new SerializedTransform(trans);
    //         this.type = "";
    //     }
    //     public MinoData(Transform trans, string type) {
    //         this.type = type;
    //         this.idx = MathUtil.getIndex(trans);
    //         this.transform = new SerializedTransform(trans);
    //     }
    //     public void reset() {
    //         transform.reset();
    //         parentData = null;
    //         idx = -31;
    //         type = "";
    //     }
    //     public void print() {
    //         transform.print();
    //     }
    // }
}