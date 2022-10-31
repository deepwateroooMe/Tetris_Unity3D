using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tetris3d;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Control {

    public class MoveButtonListener : MonoBehaviour {
        private const string TAG = "MoveButtonListener";

        // public GameObject [] moveButtons;
        private GameObject right;
        private GameObject left;
        private GameObject up;
        private GameObject down;

        private TetrominoMoveEventInfo moveInfo = new TetrominoMoveEventInfo();
        private Vector3 delta;
        
        public void Awake() {
            Debug.Log(TAG + " Awake");
            right = gameObject.FindChildByName("rightBtn");
            left = gameObject.FindChildByName("leftBtn");
            up = gameObject.FindChildByName("upBtn");
            down = gameObject.FindChildByName("downBtn");
            Debug.Log(TAG + " (right != null && left != null && up != null && down != null): " + (right != null && left != null && up != null && down != null));
        }

        public void Start() {
            Debug.Log(TAG + ": Start()");
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            // EventManager.Instance.RegisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked);
            right.GetComponent<Button>().onClick.AddListener(onRightButtonClicked);
            down.GetComponent<Button>().onClick.AddListener(onDownButtonClicked);
            // right.GetComponent<Button>().onClick.AddListener(onRightButtonClicked());
            // right.GetComponent<Button>().onClick.AddListener(onRightButtonClicked());
        }

        void onRightButtonClicked() {
            delta = new Vector3(1, 0, 0);
        }
        void onDownButtonClicked() {
            delta = new Vector3(0, 0, -1);
        }
        // void onMoveButtonClicked(MoveButtonClickEventInfo info) { // assign different callbacks/events according to button clicked
        void onMoveButtonClicked(GameObject info) { // assign different callbacks/events according to button clicked
            Debug.Log(TAG + ": onMoveButtonClicked()");
            //Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 

            // Vector3 delta = Vector3.zero;
            // // if (info.unitGO == moveButtons[0])        // right XPos
            // //     delta = new Vector3(1, 0, 0);
            // // else if (info.unitGO == moveButtons[1])  // left XNeg
            // //     delta = new Vector3(-1, 0, 0);
            // // else if (info.unitGO == moveButtons[2])  // up ZPos
            // //     delta = new Vector3(0, 0, 1);
            // // else if (info.unitGO == moveButtons[3])  // down ZNeg
            // //     delta = new Vector3(0, 0, -1);
            // if (info == right)        // right XPos
            //     delta = new Vector3(1, 0, 0);
            // else if (info == left)  // left XNeg
            //     delta = new Vector3(-1, 0, 0);
            // else if (info == up)  // up ZPos
            //     delta = new Vector3(0, 0, 1);
            // else if (info == down)  // down ZNeg
            //     delta = new Vector3(0, 0, -1);

            moveInfo.delta = delta;
            EventManager.Instance.FireEvent(moveInfo);
        }
        
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            //if (EventManager.Instance != null)
            //    EventManager.Instance.UnregisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked); 
        }
    }
}
