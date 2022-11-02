using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.MVVM;
using Framework.Util;
using UnityEngine;

namespace HotFix.Data {

    // https:// thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/ 很详细的解释
    public interface IMinoData<P> where P : class { // Defines the contract for an object that has a parent object
        P Parent { get; set; }  
    }

    public class MinoDataCon : IMinoData<TetrominoDataCon> {
        // public class MinoData {
        private const string TAG = "MinoData";

        public int instanceID;
        // public string type;

        // public TetrominoData parentData = null; 

#region Transform
        // PositionX
        public float positionX; 
        // PositionY
        public float positionY;
        // PositionZ
        public float positionZ;
        
// 对于小Mino来说,因为它是一个立方体,其实它只需要知道一个位置信息就可以了
// 我认为是这个样子的,但实际游戏上存大需要将方块砖以及小立方体放大缩小的地方,所以还是需要维护它的至少是缩放相关的相信的
// 如果我不维护旋转信息,那么在大立方体被要求旋转在实时渲染的过程中,是否小立方体因为不旋转而呈现卡壳渲染呢?
        // 所以这些信息,相关必要,不可人偷惰减少
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

// #region IMinoItem<TetrominoData> Members
//         TetrominoData IMinoData<TetrominoData>.Parent {
//             get {
//                 return this.parentData;
//             }
//             set {
//                 this.parentData = value;
//             }
//         }
// #endregion

        // 反序列化
        public static MinoDataCon JsonToObject(string json) {
			MinoDataCon data = new MinoDataCon();
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
        public System.Json.JsonObject ObjectToJson() {
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

        // 为适配原游戏源码,暂时存放的信息,等游戏可以运行后,会再重构精减源码
        public int idx { get; set; } // 这里原本是用一位标来标注的是什么信息呢?大致标四个子立方体的位置信息,方便遍历而已
        public string type { get; set; }
        public SerializedTransform transform { get; set; }
        public TetrominoDataCon parentData; // { get; internal set; } 
		#region IMinoItem<TetrominoData> Members
		TetrominoDataCon IMinoData<TetrominoDataCon>.Parent {
            get {
                return this.parentData;
            }
            set {
                this.parentData = value;
            }
        }

		public TetrominoDataCon Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		#endregion

		// 这里当初游戏中的几个构造函数,到用时再补上
		public MinoDataCon() {
            Transform trans = new GameObject().transform;
            // MinoData(trans);
            this.idx = MathUtil.getIndex(trans);
            this.transform = new SerializedTransform(trans);
            this.type = "";
        }
        public MinoDataCon(Transform trans) {
            this.idx = MathUtil.getIndex(trans);
            this.transform = new SerializedTransform(trans);
            this.type = "";
        }
        public MinoDataCon(Transform trans, string type) {
            this.type = type;
            this.idx = MathUtil.getIndex(trans);
            this.transform = new SerializedTransform(trans);
        }
        public void reset() {
            transform.reset();
            parentData = null;
            idx = -31;
            type = "";
        }
        public void print() {
            transform.print();
        }
    }
}


