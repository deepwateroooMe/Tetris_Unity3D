using HotFix.Control;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tetris3d {
    public class MoveButtonEventListener : MonoBehaviour, IPointerClickHandler { // , IPointerEnterHandler, IPointerExitHandler
        private const string TAG = "MoveButtonEventListener";

        private MoveButtonClickEventInfo moveEventInfo;

//  BUG: 这里有主要的问题就是:热更新程序域里,这个事件并不能自动触发,还没想明白到底是为什么       
        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log(TAG + " OnPointerClick");
            if (moveEventInfo == null)
                moveEventInfo = new MoveButtonClickEventInfo();
            moveEventInfo.unitGO = this.gameObject;
            EventManager.Instance.FireEvent(moveEventInfo);
        }
    }
}