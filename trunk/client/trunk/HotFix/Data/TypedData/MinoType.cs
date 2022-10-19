using HotFix.Data;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    [System.Serializable]
    public class MinoType : MonoBehaviour, IType {
        // public class MinoType : IType { // MonoBehaviour ? 这里去理解是在哪里受限制:? 预设里所使用到的脚本我是要放在这里的吗?

        [SerializeField]
        private string minoType;

        public string type {
            get {
                return minoType;
            }
            set {
                minoType = value;
            }
        }
    }
}
