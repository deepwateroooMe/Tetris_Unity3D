using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris3d {

    [System.Serializable]
    public class SerializedTransform {
        private const string TAG = "SerializedTransform";
    
        public float [] pos = new float[3];
        public float [] rot = new float[4];
        public float [] sca = new float[3];

        public SerializedTransform(Transform transform) {
            if (transform == null)
                return;
            pos[0] = transform.position.x;
            pos[1] = transform.position.y;
            pos[2] = transform.position.z;

            rot[0] = transform.rotation.w;
            rot[1] = transform.rotation.x;
            rot[2] = transform.rotation.y;
            rot[3] = transform.rotation.z;

            sca[0] = transform.localScale.x;
            sca[1] = transform.localScale.y;
            sca[2] = transform.localScale.z;
        }
    
        public void reset() {
            pos[0] = -1;
            pos[1] = -1;
            pos[2] = -1;
            rot[0] = 0;
            rot[1] = 0;
            rot[2] = 0;
            rot[3] = 1;
            sca[0] = 1;
            sca[1] = 1;
            sca[2] = 1;
        }
    
        public void print() {
            Debug.Log(TAG + " SerializedTransform.pos: [" + this.pos[0] + ", " + this.pos[1] + ", " + this.pos[2] + "]"); 
            Debug.Log(TAG + " SerializedTransform.rot: [" + this.rot[1] + ", " + this.rot[2] + ", " + this.rot[3] + "]");
            Debug.Log(TAG + " SerializedTransform.sca: [" + this.sca[0] + ", " + this.sca[1] + ", " + this.sca[2] + "]");
        }
    }
}