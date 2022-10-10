using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.MVVM {

    // commented out all Game.cs related for tmp
    public static class MathUtil {
        private const string TAG = "MathUtilities";

        // private static int m = GameMenuData.Instance.gridSize;
        private static int m = 5;
        private static int n = 12;

        public static int getIndex(int x, int y, int z) {
            int idx = (int)(m * m * y + m * z + x);
            return idx;
        }

        public static int getIndex(float [] pos) {
            print(pos);
            int idx = (int)(m * m * (int)pos[1] + m * (int)pos[0] + (int)pos[1]);
            return idx;
        }
    
        public static int getIndex(Vector3 pos) {
            int idx = m * m * (int)pos.y + m * (int)pos.z + (int)pos.x;
            return idx;
        }
        public static int getIndex(Transform transform) { // this idx has NOT considered any rotation, so it could be wrong, should always use transform instead
            int idx = m * m * (int)transform.position.y + m * (int)transform.position.z + (int)transform.position.x;
            return idx;
        }
        public static Vector3 getWorldPos(Transform transform, Vector3 pivot) {
            Vector3 pos = Vector3.zero;
            pos = transform.rotation * (transform.position - pivot) + pivot;
            return pos;
        }
    
        public static int [] getIndex(int idx) {
            // Debug.Log(TAG + ": getIndex()");
            // Debug.Log(TAG + " m: " + m); 
            int [] result = new int[3];
            result[0] = idx % (m * m) % m;
            result[1] = idx / (m * m);
            result[2] = idx % (m * m) / m;
            return result;
        }
    
        public static Vector3 Round(Vector3 pos) {
            return new Vector3((int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y), (int)Mathf.Round(pos.z));
        }

        public static Vector3 Round(Quaternion rot) {
            return new Vector3((int)Mathf.Round(rot.x), (int)Mathf.Round(rot.y), (int)Mathf.Round(rot.z));
        }

        public static float [] Round(float [] pos) {
            return new float[]{(int)Mathf.Round(pos[0]), (int)Mathf.Round(pos[1]), (int)Mathf.Round(pos[2])};
        }
    
        public static float [] Round(float x, float y, float z) {
            float [] result = new float[3];
            result[0] = (int)Mathf.Round(x);
            result[1] = (int)Mathf.Round(y);
            result[2] = (int)Mathf.Round(z);
            return result;
        }
        public static float [] Round(float x, float y, float z,
                                     float a, float b, float c) {
            float [] result = new float[3];
            result[0] = (int)Mathf.Round(x + a);
            result[1] = (int)Mathf.Round(y + b);
            result[2] = (int)Mathf.Round(z + c);
            return result;
        }
    
        public static float [] Round(float [] pos, float x, float y, float z) {
            return new float [] {(int)Mathf.Round(pos[0] + x),
                (int)Mathf.Round(pos[1] + y),
                (int)Mathf.Round(pos[2] + z)};
        }
    
        public static void print(Vector3 tmp) {
            Debug.Log(TAG + " tmp(x, y, z): " + tmp.x + ", " + tmp.y + ", " +tmp.z); 
        }

        public static void print(float x, float y, float z) {
            Debug.Log(TAG + " (x, y, z): " + x + ", " + y + ", " +z); 
        }

        public static void print(Transform transform) {
            print(transform.position);
            print(transform.rotation);
            print(transform.localScale);
        }

                
        public static void print(float [] tmp) {
            if (tmp.Length == 3)
                Debug.Log(TAG + " tmp[]: " + tmp[0] + ", " + tmp[1] + ", " + tmp[2]);
            else 
                Debug.Log(TAG + " tmp[]: " + tmp[0] + ", " + tmp[1] + ", " + tmp[2] + ", " +tmp[3]);
        }

        public static void print(int [] tmp) {
            if (tmp.Length == 3)
                Debug.Log(TAG + " tmp[]: " + tmp[0] + ", " + tmp[1] + ", " + tmp[2]);
            else 
                Debug.Log(TAG + " tmp[]: " + tmp[0] + ", " + tmp[1] + ", " + tmp[2] + ", " +tmp[3]);
        }

        public static void print(Quaternion tmp) {
            Debug.Log(TAG + " tmp(x, y, z): " + tmp.x + ", " + tmp.y + ", " + tmp.z + ", " + tmp.w); 
        }

        public static void print(int x, int y, int z) {
            Debug.Log("(x,y,z): [" + x + ", " + y + ", " + z +"]: ");
        }
        // public static void printWithGridVal(int x, int y, int z) {
        //     Debug.Log("(x,y,z): [" + x + ", " + y + ", " + z +"]: " + Game.gridOcc[x, y, z]);
        // }
        // public static void printWithGridVal(int [] pos) {
        //     Debug.Log("(x,y,z): [" + pos[0] + ", " + pos[1] + ", " + pos[2] +"]: " + Game.gridOcc[pos[0], pos[1], pos[2]]);
        // }

        public static void printBoard(int[,,] gridOcc) {
            Debug.Log(TAG + ": printBoard gridOcc[,,]:"); 
            bool empty = true;
            int z = 0;
            Debug.Log(TAG + "   " + "Z0 " + "Z1 " + "Z2 " + "Z3 " + "Z4"); 
            for (int y = 0; y < n; y++) {
                empty = true;
                Debug.Log(TAG + " y: " + y); 
                for (int x = 0; x < m; x++) {
                    z = -1;
                    Debug.Log("X" + x + ":      " + gridOcc[x, y, ++z] + "    " + gridOcc[x, y, ++z] + "    " + gridOcc[x, y, ++z] + "    " + gridOcc[x, y, ++z] + "    " + gridOcc[x, y, ++z]);
                    for (z = 0; z < m; z++) {
                        if (gridOcc[x, y, z] > 0)
                            empty = false;
                    }
                }
                if (empty)
                    break;
            }
        }

        public static void printBoard(List<int> gridOccOcc) {
            bool empty = true;
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < gridOccOcc.Count; i++) {
                x = i % (m * m) % m;
                y = i / (m * m);
                z = i % (m * m) / m;
                for (int j = 0; j < n; j++) {
                    empty = true;
                    for (int h = 0; h < m; h++) {
                        for (int k = 0; k < m; k++) {
                            Debug.Log("(h,j,k): [" + h + "," + j + "," + k +"]: " + gridOccOcc[i]);
                            if (gridOccOcc[i] > 0)
                                empty = false;
                        }
                    }
                    if (empty)
                        break;
                }
            }
        }
        public static void reset(Transform transform) { // 测试一下这个可不可 正常 工作
            transform.position.Set(-1f, -1f, -1f);
            transform.rotation = Quaternion.identity;
            transform.localScale.Set(1f, 1f, 1f);
        }
    }
}