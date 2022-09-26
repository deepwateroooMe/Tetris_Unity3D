using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {
    public class MoveButtonEventListener : MonoBehaviour, IPointerClickHandler { // , IPointerEnterHandler, IPointerExitHandler
        private const string TAG = "MoveButtonEventListener";

        private MoveButtonClickEventInfo moveEventInfo;

        public void OnPointerClick(PointerEventData eventData){
            // Debug.Log(TAG + ": OnPointerClick()");

            if (moveEventInfo == null)
                moveEventInfo = new MoveButtonClickEventInfo();
            moveEventInfo.unitGO = this.gameObject;
            // Debug.Log(TAG + " moveEventInfo.unitGO.name: " + moveEventInfo.unitGO.name); 
            EventManager.Instance.FireEvent(moveEventInfo);
        }
    }
}