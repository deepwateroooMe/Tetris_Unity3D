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
    // NONE = 0,
    TETRO = 0, // 常规各种类型
    GHOST = 1, // 阴影: 求心理阴影面积
    PARTIC = 2  // 粒子系统
}

namespace HotFix.Data.TypedData {
    public class TetrominoTypeData {

        
    }
}
