using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://forum.unity.com/threads/how-to-save-a-transform.495981/

// 这里最主要的逻辑分岐是:这个自己游戏原始模块的游戏数据序列化与反序列化可能与ILRuntime这里使用json序列化反序列伦用了两种不同的方式
// 但是为了快速把重构,把游戏逻辑在热更新模块里快速连接起来,暂时先如此摆放
// 在热更新模块中,也需要注意在使用Vector2 Vector3 Quer.....的使用的GC Alloc优化用法
namespace Framework.MVVM {

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

            // pos[0] = transform.localPosition.x;
            // pos[1] = transform.localPosition.y;
            // pos[2] = transform.localPosition.z;

            // rot[0] = transform.localRotation.w;
            // rot[1] = transform.localRotation.x;
            // rot[2] = transform.localRotation.y;
            // rot[3] = transform.localRotation.z;

            // sca[0] = transform.localScale.x;
            // sca[1] = transform.localScale.y;
            // sca[2] = transform.localScale.z;
        }
    
        // public SerializedTransform SerializedTranslation(float x, float y, float z) {
        //     pos[0] += x;
        //     pos[1] += y;
        //     pos[2] += z;
        //     return this;
        // }
        // public SerializedTransform SerializedRotation(float x, float y, float z) {
        //     rot[1] = (x + rot[1]) % 360;
        //     rot[2] = (y + rot[2]) % 360;
        //     rot[3] = (z + rot[3]) % 360;
        //     return this;
        // }

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
            // Debug.Log(TAG + ": SerializedTransform.print()"); 
            Debug.Log(TAG + " SerializedTransform.pos: [" + this.pos[0] + ", " + this.pos[1] + ", " + this.pos[2] + "]"); 
            Debug.Log(TAG + " SerializedTransform.rot: [" + this.rot[1] + ", " + this.rot[2] + ", " + this.rot[3] + "]");
            Debug.Log(TAG + " SerializedTransform.sca: [" + this.sca[0] + ", " + this.sca[1] + ", " + this.sca[2] + "]");
        }
    
    }
}