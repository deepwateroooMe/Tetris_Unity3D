using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {

    public class LevelButtonEventListener : MonoBehaviour, IPointerClickHandler { 
        private const string TAG = "LevelButtonEventListener";

        private LevelButtonClickEventInfo info;
        
        public void OnPointerClick(PointerEventData eventData){
            Debug.Log(TAG + ": OnPointerClick()");
            Debug.Log(TAG + " this.gameObject.name: " + this.gameObject.name); 

            info = new LevelButtonClickEventInfo();
            info.unitGO = this.gameObject;
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 
            EventManager.Instance.FireEvent(info);
        }

    }
}