using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]    
    public class TetrominoType : MonoBehaviour, IType {

        [SerializeField]
        private string tetrominoType;

        [SerializeField]
        private int tetrominoScore;

        public string type {
            get {
                return tetrominoType;
            }
            set {
                tetrominoType = value;
            }
        }

        public int score {
            get {
                return tetrominoScore;
            }
            set {
                tetrominoScore = value;
            }
        }

    }
}