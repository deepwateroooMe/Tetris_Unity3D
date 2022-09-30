using UnityEngine;

namespace tetris3d {
    
    [System.Serializable]    
    public class TetrominoType : MonoBehaviour {

        [SerializeField]
        private string tetrominoType;
        [SerializeField]
        private int _color;
        [SerializeField]
        private int _childCnt;
        
        public string type {
            get {
                return tetrominoType;
            }
            set {
                tetrominoType = value;
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
        public int childCnt {
            get {
                return _childCnt;
            }
            set {
                _childCnt = value;
            }
        }
    }
}
