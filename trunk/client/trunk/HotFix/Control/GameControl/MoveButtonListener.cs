using UnityEngine;

namespace HotFix.Control {

    // 移动的不仅仅只是画布,还有当前的方块砖,所以放热更新程序域里去
    // 更多的应该是MoveCanvasView MoveCanvasViewModel里的按钮点击事件与回调逻辑
    // 建立层级:从这里可以看见,游戏场景确实需要一个统管各个不同小视图的视图(或是说应该是场景)
    // 而如果我的热更新里涉及场景切换,我必须自己扩展所有场景切换相关的逻辑, 需要把这个相对较高层面的逻辑理清楚
    public class MoveButtonListener : MonoBehaviour {
        private const string TAG = "MoveButtonListener";

        public GameObject [] moveButtons;

        // 因为这个回调,把它放在哪个程序包会比较好呢?
        private TetrominoMoveEventInfo moveInfo;
        
        void OnEnable() {
            Debug.Log(TAG + ": Start()");
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            EventManager.Instance.RegisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked); 
        }

        void onMoveButtonClicked(MoveButtonClickEventInfo info) { // assign different callbacks/events according to button clicked
            Debug.Log(TAG + ": onMoveButtonClicked()");
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 

            Vector3 delta = Vector3.zero;
            if (info.unitGO == moveButtons[0]) {        // right XPos
                delta = new Vector3(1, 0, 0);
            } else if (info.unitGO == moveButtons[1]) { // left XNeg
                delta = new Vector3(-1, 0, 0);
            } else if (info.unitGO == moveButtons[2]) { // up ZPos
                delta = new Vector3(0, 0, 1);
            } else if (info.unitGO == moveButtons[3]) { // down ZNeg
                delta = new Vector3(0, 0, -1);
            }

            moveInfo = new TetrominoMoveEventInfo();
            moveInfo.delta = delta;
            moveInfo.unitGO = Game.nextTetromino;
            Debug.Log(TAG + " moveInfo.unitGO.name: " + moveInfo.unitGO.name); 
            EventManager.Instance.FireEvent(moveInfo); // 点击事件的触发传递与回调
        }
        
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            if (EventManager.Instance != null) // 热更新程序包里的回调更新
                EventManager.Instance.UnregisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked); 
        }
    }
}
