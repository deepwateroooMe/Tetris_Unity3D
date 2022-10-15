using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.Util;

// 补做7个mino预设
public enum MinoType {
    Imino = 0,
    Jmino = 1,
    Lmino = 2,
    Omino = 3,
    Smino = 4,
    Tmino = 5,
    Zmino = 6,
}

namespace HotFix.Data.TypedData {

    // 需要这些个预设里因为在游戏过程中有削除的时候,需要添加删补什么的
    public class MinoTypeData { 

        // 类型ID
        public int id;
        // GameObject名
        public string gameObjectName;
        // mino名
        public string name;
        // mino类别
        public int minoType;
        public string bundleName;
        public string assetName;
        
        public static MinoTypeData JsonToObject(string json) {
            MinoTypeData typeData = new MinoTypeData();
            JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
            if (jsonObject != null) {
                typeData.id = jsonObject["id"];
                typeData.gameObjectName = jsonObject["gameObjectName"].ToString();
                typeData.name = jsonObject["name"].ToString();
                typeData.minoType = jsonObject["minoType"];
                typeData.bundleName = jsonObject["bundleName"].ToString();
                typeData.assetName = jsonObject["assetName"].ToString();
            }
            return typeData;
        }
        
        public override string ToString() {
            return "id: " + id + " gameObjectName: " + gameObjectName + " name: " + name;
            //   + " code: " + code
            //+ " description: " + description + " link: " + link
            //+ " defaultLength: " + defaultLength + " defaultWidth: " + defaultWidth
            //+ " defaultHeight: " + defaultHeight + " minLengthScale: " + minLengthScale
        }
    }
}
