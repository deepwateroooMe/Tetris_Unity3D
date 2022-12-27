using UnityEngine;

namespace deepwaterooo.tetris3d {

    public class ParticleType : MonoBehaviour, IType {

        [SerializeField]
        private string particleType;

        [SerializeField]
        private int particleScore;

        public string type {
            get {
                return particleType;
            }
            set {
                particleType = value;
            }
        }

        public int score {
            get {
                return particleScore;
            }
            set {
                particleScore = value;
            }
        }
    }
}
