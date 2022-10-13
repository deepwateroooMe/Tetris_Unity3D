using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 感觉这里与Mino层级的IType的点儿重复,再想一下
namespace HotFix.Data.Data {

    // [System.Serializable]    
    public class TetrominoType : MonoBehaviour, IType { // MonoBehaviour ??? 不需要呀

        // [SerializeField]
        private string tetrominoType; // string ==> int

        public string type {
            get {
                return tetrominoType;
            }
            set {
                tetrominoType = value;
            }
        }
    }
}
