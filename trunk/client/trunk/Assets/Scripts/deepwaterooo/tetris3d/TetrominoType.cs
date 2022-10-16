using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]    
    public class TetrominoType : MonoBehaviour, IType {

        [SerializeField]
        private string tetrominoType;

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





