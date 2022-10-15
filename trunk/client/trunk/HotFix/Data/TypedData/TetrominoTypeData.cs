using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class TetrominoTypeData {

        
    }
}
