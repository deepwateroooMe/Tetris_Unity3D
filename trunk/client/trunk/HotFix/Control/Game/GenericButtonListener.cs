using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control.Game {

    public class GenericButtonListener : MonoBehaviour {
        private const string TAG = "GenericButtonListener";

        public GameObject [] genericButtons; // 0 swap 1 undo
        
        // private SwapPreviewsEventInfo swapInfo;
        
        void OnEnable() {
            Debug.Log(TAG + ": Start()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            EventManager.Instance.RegisterListener<GenericButtonClickEventInfo>(onGenericButtonClicked); 
        }

        void onGenericButtonClicked(GenericButtonClickEventInfo info) { // assign different callbacks/events according to button clicked
            Debug.Log(TAG + ": onGenericButtonClicked()");
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 
            Vector3 delta = Vector3.zero;
            if (info.unitGO == genericButtons[0]) { // SaveGame Button
                SaveGameEventInfo saveGameInfo = new SaveGameEventInfo();
                EventManager.Instance.FireEvent(saveGameInfo);
            }
            // else if (info.unitGO == genericButtons[1]) {        // swap Preview Tetrominos
            //     SwapPreviewsEventInfo swapInfo = new SwapPreviewsEventInfo();
            //     swapInfo.tag = "swap";
            //     Debug.Log(TAG + " info.tag: " + swapInfo.tag); 
            //     // swapInfo.unitGO = gameObject.FindObjectOfType<Game>().gameObject;
            //     // Debug.Log(TAG + " swapInfo.unitGO.name: " + swapInfo.unitGO.name);
            //     EventManager.Instance.FireEvent(info);
            // }
            // if (info.unitGO == genericButtons[0]) { // SaveGame Button
            //     UndoGameEventInfo undoInfo = new UndoGameEventInfo();
            //     undoInfo.tag = "undo";
            //     Debug.Log(TAG + " info.tag: " + undoInfo.tag); 
            //     // undoInfo.unitGO = gameObject.FindObjectOfType<Game>().gameObject;
            //     EventManager.Instance.FireEvent(info);
            // }
        }

        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<GenericButtonClickEventInfo>(onGenericButtonClicked); 
            }
        }
    }
}
