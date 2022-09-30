using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {
    public class MoveButtonEventListener : MonoBehaviour, IPointerClickHandler { // , IPointerEnterHandler, IPointerExitHandler
        private const string TAG = "MoveButtonEventListener";

        private MoveButtonClickEventInfo moveEventInfo;

        public void OnPointerClick(PointerEventData eventData){
            if (moveEventInfo == null)
                moveEventInfo = new MoveButtonClickEventInfo();
            moveEventInfo.unitGO = this.gameObject;
            EventManager.Instance.FireEvent(moveEventInfo);
        }
    }
}