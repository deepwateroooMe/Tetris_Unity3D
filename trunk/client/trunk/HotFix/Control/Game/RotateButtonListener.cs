using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control.Game {

    public class RotateButtonListener : MonoBehaviour {
        private const string TAG = "RotateButtonListener";

        public GameObject [] rotateButtons;
        
        private TetrominoRotateEventInfo rotateInfo;
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()"); 
            EventManager.Instance.RegisterListener<RotateButtonClickEventInfo>(onRotateButtonClicked); 
        }

        void onRotateButtonClicked(RotateButtonClickEventInfo info) { // assign different callbacks/events according to button clicked
            Debug.Log(TAG + ": onRotateButtonClicked()");

            Vector3 delta = Vector3.zero;  // 这里的旋转方向是需要修改的
            if (info.unitGO == rotateButtons[0]) {        // XPos
                delta = new Vector3(90, 0, 0);
            } else if (info.unitGO == rotateButtons[1]) { // XNeg
                delta = new Vector3(-90, 0, 0);
            } else if (info.unitGO == rotateButtons[2]) { // YPos
                delta = new Vector3(0, 90, 0);
            } else if (info.unitGO == rotateButtons[3]) { // YNeg
                delta = new Vector3(0, -90, 0);
            } else if (info.unitGO == rotateButtons[4]) { // ZPos
                delta = new Vector3(0, 0, 90);
            } else if (info.unitGO == rotateButtons[5]) { // ZNeg
                delta = new Vector3(0, 0, -90);
            }

            rotateInfo = new TetrominoRotateEventInfo();
            rotateInfo.delta = delta;
            rotateInfo.unitGO = Game.nextTetromino;
            EventManager.Instance.FireEvent(rotateInfo);
        }
        
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()"); 
            if (EventManager.Instance != null)
                EventManager.Instance.UnregisterListener<RotateButtonClickEventInfo>(onRotateButtonClicked); 
        }
    }
}
