using HotFix.Data;
using UnityEngine;
using System.Collections.Generic;
using Framework.ResMgr;

namespace HotFix.Control {

    // 场景抽象基类: 
    public abstract class SceneBase {

        public SceneData Data {
            get;
            set;
        }
        public SceneTypeData TypeData {
            get;
            set;
        }
        public GameObject GameObject {
            get;
            set;
        }

// 这里省掉了很多程序:要结合自己的游戏来一一实现
    }
}
