using System;
using UnityEngine;
using System.IO;

namespace tetris3d {

    [System.Serializable]
    public class MinoType : MonoBehaviour {

        [SerializeField]
        private string minoType;
        [SerializeField]
        private int _color;

        public string type {
            get {
                return minoType;
            }
            set {
                minoType = value;
            }
        }
        public int color {
            get {
                return _color;
            }
            set {
                _color = value;
            }
        }
    }
}