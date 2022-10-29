using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

    // 忘记了：它为什么没有类型之分呢？它不需要类型之分,他所要做的一切就是跟随它的小主Tetromino,然后往下掉到最底层的方块砖上一格就可以了
    public class GhostTetromino : MonoBehaviour {
        private const string TAG = "GhostTetromino";

        private Transform currentActiveTransform;
        
        void Start () {
            tag = "currentGhostTetromino";
        } 

// 这里作为阴影掉得快与慢,肉眼无法区分,无所谓
        private float timer = 1.0f;

// 作为一个阴影方块砖,它所有需要做的就是首先找到自己的摆放位置;再监听当前方块砖的位置变化,暂时不管它
// (需要注意大方格的底层也是高低不平的,需要时上进下)
        void Update () {        
            // timer -= Time.deltaTime;
            // if (timer > 0) return;

            FollowActiveTetromino();
            MoveDown();

            // timer = 1.0f;
        }

        void FollowActiveTetromino() {
            currentActiveTransform = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;
            transform.position = currentActiveTransform.position;
            transform.rotation = currentActiveTransform.rotation;
        }

        public void MoveDown() {
            while (CheckIsValidPosition()) 
                transform.position += new Vector3(0, -1, 0);

            if (!CheckIsValidPosition()) 
                transform.position += new Vector3(0, 1, 0);

//             foreach (Transform mino in transform) {
// // 感觉这里就是逻辑不完整
//                 //if (mino.CompareTag("mino")) 
//                 //    Vector3 pos = MathUtil.Round(mino.position);
//             }
        }

// BUG: 这里检查有问题,阴影跑穿跑到地域去了.....        
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
