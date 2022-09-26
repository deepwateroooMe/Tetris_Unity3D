using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris3d {

    public class GhostTetromino : MonoBehaviour {
        private const string TAG = "GhostTetromino";

        private Transform currentActiveTransform;
    
        void Start () {
            tag = "currentGhostTetromino";
        } 

        void Update () { 
            FollowActiveTetromino();
            MoveDown();  
        }

        void FollowActiveTetromino() {
            currentActiveTransform = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;
            transform.position = currentActiveTransform.position;
            transform.rotation = currentActiveTransform.rotation;
        }

        public void MoveDown() {
            while (CheckIsValidPosition()) {
                transform.position += new Vector3(0, -1, 0);
            }
            if (!CheckIsValidPosition()) {
                transform.position += new Vector3(0, 1, 0);
            }

            foreach (Transform mino in transform) {
                if (mino.CompareTag("mino")) {
                    Vector3 pos = MathUtil.Round(mino.position);
                }
            }
        }
    
        bool CheckIsValidPosition() {
            foreach (Transform mino in transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (!FindObjectOfType<Game>().CheckIsInsideGrid(pos))
                    return false;

                if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                    FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent.CompareTag("currentActiveTetromino"))
                    return true;
                if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                    FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform) {
                    return false;
                }
            }
            return true;
        }
    }
}