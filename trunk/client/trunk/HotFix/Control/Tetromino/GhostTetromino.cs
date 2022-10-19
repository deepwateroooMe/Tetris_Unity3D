using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

    // 忘记了：它为什么没有类型之分呢？
    public class GhostTetromino : MonoBehaviour {
        private const string TAG = "GhostTetromino";

// 这里定义成观察者模式,观察模型中当前currentActiveTetromino 的位置
        private Transform currentActiveTransform; 
        
        void Start () {
            tag = "currentGhostTetromino";
            Model.nextTetromino.OnValueChanged += MyUpdate;
        } 

        void MyUpdate (Vector3 pre, Vector3 cur) {
            if (pre.x != cur.x || pre.y != cur.y) {
                FollowActiveTetromino();
                MoveDown();  
            }
            // void Update () { 
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
                if (!ViewManager.GameView.ViewModel.CheckIsInsideGrid(pos))
                    return false;

                if (ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos) != null &&
                    ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos).parent.CompareTag("currentActiveTetromino"))
                    return true;
                if (ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos) != null &&
                    ViewManager.GameView.ViewModel.GetTransformAtGridPosition(pos).parent != transform) {
                    return false;
                }
            }
            return true;
        }
    }
}