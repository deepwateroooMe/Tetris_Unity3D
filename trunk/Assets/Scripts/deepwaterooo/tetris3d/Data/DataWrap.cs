using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d.Data;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    // Data封装:这个封装就让热更新视图中的控件可以感知其所依附的控件的生命周期
    public class DataWrap : MonoBehaviour {

        public ESelectGameObjectType gameObjectType;
        public int instanceID;
    }
}
