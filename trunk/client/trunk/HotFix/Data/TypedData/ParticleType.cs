using HotFix.Data;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    public class ParticleType : MonoBehaviour, IType {

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
