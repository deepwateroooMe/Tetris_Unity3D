using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    public class Path {
        public static readonly string path = Application.persistentDataPath + "/game" + ".save";
    }

    public class Tags {
        public const string mino = "mino";
        public const string currentActiveTetromino = "currentActiveTetromino";
        public const string ghostTetromino = "ghostTetromino";
    }

    public class Layers {
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int UI = LayerMask.NameToLayer("UI");
    }

    // PS: Bonus points for using
    // gameObject.CompareTag(Constants.Tags.board);
    // instead of
    // gameObject.tag == Constants.Tags.board;    
}
