namespace deepwaterooo.tetris3d {

    public class GhostTetrominoType : IType {
        private string ghostTetrominoType;

        public string type {
            get {
                return ghostTetrominoType;
            }
            set {
                ghostTetrominoType = value;
            }
        }
        public int score {
            get {
                return 0;
            }
            set {
            }
        }
    }
}
