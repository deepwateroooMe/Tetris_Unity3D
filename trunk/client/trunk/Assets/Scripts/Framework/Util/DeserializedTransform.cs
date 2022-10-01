using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris3d {

    public static class DeserializedTransform {
        private const string TAG = "DeserializedTransform";

        public static Vector3 getDeserializedTransPos (this SerializedTransform serializedTransform) {
            Vector3 res = new Vector3(serializedTransform.pos[0], serializedTransform.pos[1], serializedTransform.pos[2]);
            return res;
        }

        public static Quaternion getDeserializedTransRot (this SerializedTransform serializedTransform) {
            Quaternion res = new Quaternion(serializedTransform.rot[1], serializedTransform.rot[2], serializedTransform.rot[3], serializedTransform.rot[0]);
            return res;
        }

        public static Vector3 getDeserializedTransLocalScale (this SerializedTransform serializedTransform) {
            Vector3 res = new Vector3(serializedTransform.sca[0], serializedTransform.sca[1], serializedTransform.sca[2]);
            return res;
        }

        // public static void setDeserializedTransform(this SerializedTransform serializedTransform, Transform transform) {
        //     transform.position = new Vector3(serializedTransform.pos[0], serializedTransform.pos[1], serializedTransform.pos[2]);
        //     transform.rotation = new Quaternion(serializedTransform.rot[1], serializedTransform.rot[2], serializedTransform.rot[3], serializedTransform.rot[0]);
        //     transform.localScale = new Vector3(serializedTransform.sca[0], serializedTransform.sca[1], serializedTransform.sca[2]);
        // }
    }
}
