using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.Util;

// // 补做7个mino预设
// public enum MinoType {
//     minoI = 0,
//     minoJ = 1,
//     minoL = 2,
//     minoO = 3,
//     minoS = 4,
//     minoT = 5,
//     minoZ = 6,
// }

namespace HotFix.Data.TypedData {

    // 需要这些个预设里因为在游戏过程中有削除的时候,需要添加删补什么的
    public class MinoTypeData { 

        // 类型ID
        public int id;
        // // GameObject名
        // public string gameObjectName;
        // mino名
        public string name;
        // mino类别
        public string type;

        public string bundleName;
        public string assetName;
        
        public static MinoTypeData JsonToObject(string json) {
            MinoTypeData typeData = new MinoTypeData();
            JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
            if (jsonObject != null) {
                typeData.id = jsonObject["id"];
                // typeData.gameObjectName = jsonObject["gameObjectName"].ToString();
                typeData.name = jsonObject["name"].ToString();
                typeData.type = jsonObject["type"];
                typeData.bundleName = jsonObject["bundleName"].ToString();
                typeData.assetName = jsonObject["assetName"].ToString();
            }
            return typeData;
        }
        
        public override string ToString() {
            return "id: " + id + " gameObjectName: " + gameObjectName + " name: " + name
            + " type: " + type
                + " bundleName: " + bundleName
                + " assetName: " + assetName;
        }
    }
}

