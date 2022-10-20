using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework.Util;

// public enum TetrominoPos {
//     // NONE = 0,
//     INIT = 0,    // 常规摆放地(2, 12, 2) 
//     PREVIEW = 1, // 预览模式的位置,随游戏模式是教育启蒙还是经典等而有怕不同
//     RUNTIME = 2  // 粒子系统等运行时再定的位置
// }

//public enum TetrominoType { // 这里的导进来的,与预设里的只保留一套,否则有重复和冲突
//    tetro_I = 0, // 常规各种类型: 这么列不足以列出所有的,必段一个一个列出来一,下同
//    tetro_J = 1, 
//    tetro_L = 2, 
//    tetro_O = 3, 
//    tetro_S = 4, 
//    tetro_T = 5, 
//    tetro_Z = 6, 

//    shado_I = 7,  // 阴影类型
//    shado_J = 8, 
//    shado_L = 9, 
//    shado_O = 10,
//    shado_S = 11, 
//    shado_T = 12, 
//    shado_Z = 13, 

//    PARTIC = 14  // 粒子系统
//}

namespace HotFix.Data {

    // 这里实现了接口,那么制作打包下载下来的预设里就可以没有添加脚本,在运行实例化的时候再根据类型添加
// 能再提炼成MINO TYPE, TETROMINO TYPE,以及SCENE TYPE吗? 之后再考虑
// 这里会有一种限制:当你限定一个方块砖一个TetrominoType只能拥有相同同STRING type MINO的时候,晚点儿当你想要一个方块砖可以拥有四到七种不同类型MINO的时候就不好扩展了???
    public class TetrominoTypeData {

        // 类型ID
        public int id;
        // GameObject名
        public string gameObjectName;
        // mino名
        public string name;
        // tetromino类别
        public string type {
            get;
            set;
        }

        public string bundleName;
        public string assetName;
        
        public static TetrominoTypeData JsonToObject(string json) {
            TetrominoTypeData typeData = new TetrominoTypeData();
            JsonObject jsonObject = JsonSerializer.Deserialize(json) as JsonObject;
            if (jsonObject != null) {
                typeData.id = jsonObject["id"];
                typeData.gameObjectName = jsonObject["gameObjectName"].ToString();
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
