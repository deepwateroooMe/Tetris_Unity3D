using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control.Game {

    public class MoveButtonListener : MonoBehaviour {
        private const string TAG = "MoveButtonListener";

        public GameObject [] moveButtons;
        
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
            EventManager.Instance.FireEvent(moveInfo);
        }
        
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            if (EventManager.Instance != null)
                EventManager.Instance.UnregisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked); 
        }
    }
}
