using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]
    public class MinoType : MonoBehaviour, IType {
        // public class MinoType : IType { // MonoBehaviour ? 这里去理解是在哪里受限制:? 预设里所使用到的脚本我是要放在这里的吗?

        [SerializeField]
        private string minoType;

        [SerializeField]
        private int minoScore;

        [SerializeField]
        private int minoColor;
        
        public string type {
            get {
                return minoType;
            }
            set {
                minoType = value;
            }
        }
        public int score {
            get {
                return minoScore;
            }
            set {
                minoScore = value;
            }
        }
        public int color {
            get {
                return minoColor;
            }
            set {
                minoColor = value;
            }
        }
    }
}
