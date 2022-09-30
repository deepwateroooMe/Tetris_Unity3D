using UnityEngine;

namespace tetris3d {
    public class GenericButtonListener : MonoBehaviour {
        private const string TAG = "GenericButtonListener";

        public GameObject [] genericButtons; 
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            EventManager.Instance.RegisterListener<GenericButtonClickEventInfo>(onGenericButtonClicked); 
        }

        void onGenericButtonClicked(GenericButtonClickEventInfo info) { 
            Debug.Log(TAG + ": onGenericButtonClicked()");
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 

            if (info.unitGO == genericButtons[0]) {
                if (info.unitGO.name == "saveGame")  {           // SaveGame Button
                    SaveGameEventInfo saveGameInfo = new SaveGameEventInfo();
                    EventManager.Instance.FireEvent(saveGameInfo);
                } else if (info.unitGO.name == "swapButton") {   // swapButton
                    SwapPreviewsEventInfo swapPreviewsInfo = new SwapPreviewsEventInfo();
                    EventManager.Instance.FireEvent(swapPreviewsInfo);
                } else if (info.unitGO.name == "undoButton") {   // undoButton
                    UndoGameEventInfo undoGameInfo = new UndoGameEventInfo();
                    EventManager.Instance.FireEvent(undoGameInfo);
                } else if (info.unitGO.name == "pauseButton") {  // pauseGame
                    PauseGameEventInfo pauseGameInfo = new PauseGameEventInfo();
                    EventManager.Instance.FireEvent(pauseGameInfo);
                } else if (info.unitGO.name == "toggleButton") { // toggleButtons
                    ToggleActionEventInfo toggleActionInfo = new ToggleActionEventInfo();
                    EventManager.Instance.FireEvent(toggleActionInfo);
                }  
            } else if (genericButtons.Length > 1) {
                if (info.unitGO == genericButtons[1] && info.unitGO.name == "fallButton") { // fallButton
                    Debug.Log(TAG + " (info.unitGO.name == \"fallButton\"): " + (info.unitGO.name == "fallButton")); 
                    FallFastEventInfo fallFastInfo = new FallFastEventInfo();
                    EventManager.Instance.FireEvent(fallFastInfo);
                }
            }
        }

        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            if (EventManager.Instance != null) {
                EventManager.Instance.UnregisterListener<GenericButtonClickEventInfo>(onGenericButtonClicked); 
            }
        }
    }
}