using HotFix.Control;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix.Control {

    public class MoveButtonEventListener : MonoBehaviour, IPointerClickHandler { 
        private const string TAG = "MoveButtonEventListener";

        private MoveButtonClickEventInfo moveEventInfo;

        public void Awake() {
            Debug.Log(TAG + " Awake()");
            moveEventInfo = new MoveButtonClickEventInfo();
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log(TAG + " OnPointerClick");
            moveEventInfo.unitGO = this.gameObject;
            EventManager.Instance.FireEvent(moveEventInfo);
        }
    }
}
