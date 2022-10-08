using System;
using UnityEngine;
using System.IO;

namespace tetris3d {

    [System.Serializable]
    public class MinoType : MonoBehaviour, IType {

        [SerializeField]
        private string minoType;

        public string type {
            get {
                return minoType;
            }
            set {
                minoType = value;
            }
        }
    }
}