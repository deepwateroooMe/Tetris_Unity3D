using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace tetris3d {

    // https://thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/ 很详细的解释

    public interface IMinoDataItem<P> where P : class { // Defines the contract for an object that has a parent object

        P Parent { get; set; }  
    }

    [System.Serializable]
    public class MinoData : IMinoDataItem <TetrominoData> {
        private const string TAG = "MinoData";

        public int idx { get; set; }
        public string type { get; set; }
        public SerializedTransform transform { get; set; }

        [NonSerialized]
        public TetrominoData parentData; // { get; internal set; } 

#region IMinoItem<TetrominoData> Members
        TetrominoData IMinoDataItem<TetrominoData>.Parent {
            get {
                return this.parentData;
            }
            set {
                this.parentData = value;
            }
        }
#endregion

        public MinoData(Transform trans) {
            this.idx = MathUtil.getIndex(trans);
            this.transform = new SerializedTransform(trans);
            this.type = "";
        }

        public MinoData(Transform trans, string type) {
            this.type = type;
            this.idx = MathUtil.getIndex(trans);
            this.transform = new SerializedTransform(trans);
        }

        public void reset() {
            transform.reset();
            parentData = null;
            idx = -31;
            type = "";
        }

        public void print() {
            transform.print();
        }
    }
}