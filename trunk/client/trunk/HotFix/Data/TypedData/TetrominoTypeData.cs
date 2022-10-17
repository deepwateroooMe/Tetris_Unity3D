using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.Util;

public enum TetrominoPos {
    // NONE = 0,
    NORMAL = 0,  // 常规摆放地(2, 12, 2)
    CURRENT = 1, // 允许重置到想要的位置
    PARTIC = 2   // 粒子系统
}

public enum TetrominoType {
    tetro_I = 0, // 常规各种类型: 这么列不足以列出所有的,必段一个一个列出来一,下同
    tetro_J = 1, 
    tetro_L = 2, 
    tetro_O = 3, 
    tetro_S = 4, 
    tetro_T = 5, 
    tetro_Z = 6, 

    shado_I = 7,  // 阴影类型
    shado_J = 8, 
    shado_L = 9, 
    shado_O = 10,
    shado_S = 11, 
    shado_T = 12, 
    shado_Z = 13, 

    PARTIC = 14  // 粒子系统
}

namespace HotFix.Data.TypedData {

// 能再提炼成MINO TYPE, TETROMINO TYPE,以及SCENE TYPE吗? 之后再考虑
    public class TetrominoTypeData {

        // 类型ID
        public int id;
        // GameObject名
        public string gameObjectName;
        // mino名
        public string name;
        // tetromino类别
        public int tetrominoType;

        public string bundleName;
        public string assetName;
        
        public static TetrominoTypeData JsonToObject(string json) {
            TetrominoTypeData typeData = new TetrominoTypeData();
            JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
            if (jsonObject != null) {
                typeData.id = jsonObject["id"];
                typeData.gameObjectName = jsonObject["gameObjectName"].ToString();
                typeData.name = jsonObject["name"].ToString();
                typeData.tetrominoType = jsonObject["tetrominoType"];
                typeData.bundleName = jsonObject["bundleName"].ToString();
                typeData.assetName = jsonObject["assetName"].ToString();
            }
            return typeData;
        }
        
        public override string ToString() {
            return "id: " + id + " gameObjectName: " + gameObjectName + " name: " + name
            + " tetrominoType: " + tetrominoType
                + " bundleName: " + bundleName
                + " assetName: " + assetName;
        }
    }
}
