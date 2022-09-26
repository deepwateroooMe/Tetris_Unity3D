using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {
    public class GenericButtonEventListener : MonoBehaviour, IPointerClickHandler { 
        private const string TAG = "GenericButtonEventListener";

        private GenericButtonClickEventInfo info;
        
        public void OnPointerClick(PointerEventData eventData){
            Debug.Log(TAG + ": OnPointerClick()");
            Debug.Log(TAG + " this.gameObject.name: " + this.gameObject.name); 

            info = new GenericButtonClickEventInfo();
            info.unitGO = this.gameObject;
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 
            EventManager.Instance.FireEvent(info);
        }

    }
}