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

        private Transform currentActiveTransform;
        
        void Start () {
            tag = "currentGhostTetromino";
        } 

// 作为一个阴影,它所有需要做的就是首先找到自己的摆放位置;再监听当前方块砖的位置变化,暂时不管它
// (需要注意大方格的底层也是高低不平的,需要时上进下)
        void Update () {        
            FollowActiveTetromino();
            // MoveDown();
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

            foreach (Transform mino in transform) {
// 感觉这里就是逻辑不完整
                //if (mino.CompareTag("mino")) 
                //    Vector3 pos = MathUtil.Round(mino.position);
            }
        }

// BUG: 这里检查有问题,阴影跑穿跑到地域去了.....        
        bool CheckIsValidPosition() {
            foreach (Transform mino in transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (((GameViewModel)ViewManager.GameView.BindingContext).CheckIsInsideGrid(pos))
                    return false;

                 if (((GameViewModel)ViewManager.GameView.BindingContext).GetTransformAtGridPosition(pos) != null &&
                     ((GameViewModel)ViewManager.GameView.BindingContext).GetTransformAtGridPosition(pos).parent.CompareTag("currentActiveTetromino"))
                    return true;
                if (((GameViewModel)ViewManager.GameView.BindingContext).GetTransformAtGridPosition(pos) != null &&
                    ((GameViewModel)ViewManager.GameView.BindingContext).GetTransformAtGridPosition(pos).parent != transform) {
                    return false;
                }
            }
            return true;
        }
    }
}
