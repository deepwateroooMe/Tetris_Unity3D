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

        void onEnable() {
            Start();
        }
        void Start () {
            tag = "currentGhostTetromino";
// 只注册监听有效移动: 当前方块砖的生成,平移或是旋转, 和方块砖着地事件
            // EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onTetrominoSpawned);
// significat delays
            // EventManager.Instance.RegisterListener<TetrominoValidMMInfo>(onTetrominoMoveRotate);
            // EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        } 

// 作为一个阴影方块砖,它所有需要做的就是首先找到自己的摆放位置;再监听当前方块砖的位置变化,暂时不管它
// (需要注意大方格的底层也是高低不平的,需要时上进下)

        void Update () {        
            FollowActiveTetromino();
            MoveDown();
        }
        void FollowActiveTetromino() {
            currentActiveTransform = ViewManager.nextTetromino.transform;
            transform.position = currentActiveTransform.position;
            transform.rotation = currentActiveTransform.rotation;
        }

        void onTetrominoSpawned(TetrominoSpawnedEventInfo info) { // delta: position
            transform.position = ViewManager.nextTetromino.transform.position;
            transform.rotation = ViewManager.nextTetromino.transform.rotation;
            MoveDown();
        }
        // void onTetrominoMoveRotate(TetrominoValidMMInfo info) {
        //     if (info.type.Equals("move"))
        //         transform.position += info.delta;
        //     else 
        //         transform.Rotate(info.delta);
        // }
        void onTetrominoLand(TetrominoLandEventInfo info) {
			// OnDisable();
// TODO: 这里不知道是什么原因回收不成功            
            // PoolHelper.recycleGhostTetromino(); 
        }

// TODO: BUG 有时候阴影在当前方块砖的上面        
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

        bool CheckIsValidPosition() {
            foreach (Transform mino in transform) {
                Vector3 pos = MathUtil.Round(mino.position);
                if (!Model.CheckIsInsideGrid(pos))
                    return false;

                 if (Model.GetTransformAtGridPosition(pos) != null &&
                     Model.GetTransformAtGridPosition(pos).parent.CompareTag("currentActiveTetromino"))
                    return true;
                if (Model.GetTransformAtGridPosition(pos) != null &&
                    Model.GetTransformAtGridPosition(pos).parent != transform) {
                    return false;
                }
            }
            return true;
        }

        public void OnDisable() {
            Debug.Log(TAG + " OnDisable()");
            // EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onTetrominoSpawned);
            // EventManager.Instance.UnregisterListener<TetrominoValidMMInfo>(onTetrominoMoveRotate);
            // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }
    }
}
