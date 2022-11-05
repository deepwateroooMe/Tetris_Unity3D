using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]    
    public class TetrominoType : MonoBehaviour, IType {

        [SerializeField]
        private string tetrominoType;

        [SerializeField]
        private int tetrominoScore;

        [SerializeField]
        private int tetrominoColor;

        [SerializeField]
        private int tetrominoChildCnt;

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

        public int color {
            get {
                return tetrominoColor;
            }
            set {
                tetrominoColor = value;
            }
        }

        public int childCnt {
            get {
                return tetrominoChildCnt;
            }
            set {
                tetrominoChildCnt = value;
            }
        }
    }
}