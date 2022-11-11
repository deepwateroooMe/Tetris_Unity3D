using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

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
        }

        bool CheckIsValidPosition() {
            foreach (Transform mino in transform) {
                Vector3 pos = MathUtilP.Round(mino.position);
                if (!Model.CheckIsInsideGrid(pos))
                    return false;
                if (Model.GetTransformAtGridPosition(pos) != null
                    && Model.GetTransformAtGridPosition(pos).parent != transform
                    && !Model.GetTransformAtGridPosition(pos).parent.CompareTag("currentActiveTetromino")) {
                    return false;
                }
            }
            return true;
        }
        public void OnDisable() {
            // Debug.Log(TAG + " OnDisable()");
            // EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onTetrominoSpawned);
            // EventManager.Instance.UnregisterListener<TetrominoValidMMInfo>(onTetrominoMoveRotate);
            // EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onTetrominoLand);
        }
    }
}
