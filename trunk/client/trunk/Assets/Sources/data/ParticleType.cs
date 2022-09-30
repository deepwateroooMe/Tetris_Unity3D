using UnityEngine;

namespace tetris3d {
    // public class ParticleType : MonoBehaviour, IType {
    public class ParticleType : MonoBehaviour {

        [SerializeField]
        private string particleType;

        public string type {
            get {
                return particleType;
            }
            set {
                particleType = value;
            }
        }
    }
}
