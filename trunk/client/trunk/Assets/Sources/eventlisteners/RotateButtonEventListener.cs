using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {
    public class RotateButtonEventListener : MonoBehaviour, IPointerClickHandler { // , IPointerEnterHandler, IPointerExitHandler
        private const string TAG = "RotateButtonEventListener";

        private RotateButtonClickEventInfo rotateEventInfo;

        public void OnPointerClick(PointerEventData eventData){
            Debug.Log(TAG + ": OnPointerClick()");

            if (rotateEventInfo == null)
                rotateEventInfo = new RotateButtonClickEventInfo();
            rotateEventInfo.unitGO = this.gameObject;
            EventManager.Instance.FireEvent(rotateEventInfo);
        }

    }
}