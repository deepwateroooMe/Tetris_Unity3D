using System.Collections.Generic;
using System.Json;
using System.Linq;
using Framework.MVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace HotFix.Data {

    // 方块砖实例数据:  就是说,现在暂时是两套机制,原源码的BinaryFormatter机制,以及热更新重构后的Json序列化反序列化机制,还要让两套机制同时运行时能适配原源码
    public class TetrominoDataCon {
        private const string TAG = "TetrominoData";

        // 实例ID
        public long id; // instanceID
        // 名字
        public string name;
        // 类型
        public string type;

        
// 大致的设计思路: 这个序列化,反序列化等
        // GameView/Items: 两三个文件列出不同的类型来
        // ViewManager.cs: parse 出三四种不同的类型，用三四个不同的字典来管理
        // 过程中将资源池整合到ViewManager中去
        // 定义TetrominoBase抽象基类公用控制逻辑，继承出三四种不同的实现，
        // 资源池根据类型激活或是失活脚本

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

        public List<MinoDataCon> _children; 
// 方块砖所特有的: 这里的这层父子们的嵌套逻辑会把序列化给搞昏的,所以必须得自定义序列化,比现项目中的序列化要稍微复杂那么一点点儿
        // 现在的逻辑没有继续明确父子关系,在什么地方可能会有影响呢? 有影响,模型里会判断小立方体的父控制是谁,所以这层关系要维护起来
        public MinoDataCollectionCon<TetrominoDataCon, MinoDataCon> children { get; private set; } 
#endregion

        // 因为这里暂时更多的只是在资源加载的时候的操作,并不是很频繁,暂时不考虑这些
        // private BindableProperty<Vector3> pos = new BindableProperty<Vector3>();
        // private BindableProperty<Vector3> rot = new BindableProperty<Vector3>();
        // private BindableProperty<Vector3> sca = new BindableProperty<Vector3>();
        // private SerializedTransform _serTransform = new SerializedTransform(_transform);
        private Transform _transform = new GameObject().transform;
        
// 添加一个适配原游戏源码的公用方法        
        public  SerializedTransform transform {
            get {
                _transform.position = new Vector3(positionX, positionY, positionZ);
                _transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
                _transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                return new SerializedTransform(_transform); // 不能永远这么new,写个reset方法供重用
            }
            set {
                _transform.position = new Vector3(value.pos[0], value.pos[1], value.pos[2]);
                _transform.rotation = Quaternion.Euler(value.rot[1], value.rot[2], value.rot[3]);
                _transform.localScale = new Vector3(value.sca[0], value.sca[1], value.sca[2]);
                // _transform.rotation = value.rotation;
                // _transform.localScale = value.localScale;
                positionX = _transform.position.x;
                positionY = _transform.position.y;
                positionZ = _transform.position.z;
                rotationX = _transform.rotation.x;
                rotationY = _transform.rotation.y;
                rotationZ = _transform.rotation.z;
                scaleX = _transform.localScale.x;
                scaleY = _transform.localScale.y;
                scaleZ = _transform.localScale.z;
            }
        }
        private void OnValueChanged(Vector3 pre, Vector3 cur) {
            
        }
// 套路的三个公用方法
        // 反序列化: 这里需要更多的工作来维护父子关系的?下午再好好想想这个有没有必要,如何实现
        public static TetrominoDataCon JsonToObject(string json) {
            TetrominoDataCon data = new TetrominoDataCon();
            JObject tetrominoItem = (JObject)JsonConvert.DeserializeObject(json);
            data.id = (long)tetrominoItem.SelectToken("id");
            data.name = tetrominoItem.SelectToken("name").ToString();
            data.type = tetrominoItem.SelectToken("type").ToString();
            data.positionX = (float)tetrominoItem.SelectToken("positionX");
            data.positionY = (float)tetrominoItem.SelectToken("positionY");
            data.positionZ = (float)tetrominoItem.SelectToken("positionZ");
            data.rotationX = (float)tetrominoItem.SelectToken("rotationX");
            data.rotationY = (float)tetrominoItem.SelectToken("rotationY");
            data.rotationZ = (float)tetrominoItem.SelectToken("rotationZ");
            data.scaleX = (float)tetrominoItem.SelectToken("scaleX");
            data.scaleY = (float)tetrominoItem.SelectToken("scaleY");
            data.scaleZ = (float)tetrominoItem.SelectToken("scaleZ");

            IList<JToken> children = tetrominoItem["children"].Children().ToList();
            foreach (JToken mino in children) {
                string minoType = mino.SelectToken("type").ToString();
                if (minoType.StartsWith("mino")) {
					MinoDataCon minoData = mino.ToObject<MinoDataCon>();
                    data._children.Add(minoData);
                }
            }
            return data;
        }

        public override string ToString() {
            // 因为嵌套关系,这里还能够这么简单地写吗?大概不可以,暂时好像没有发现用它的地方
            return ObjectToJson().ToString();
        }

        // 序列化
        public JsonObject ObjectToJson() {
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("instanceID", id);
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
            jsonObject.Add("children", JsonConvert.SerializeObject(children));
            return jsonObject;
        }
    }
}





