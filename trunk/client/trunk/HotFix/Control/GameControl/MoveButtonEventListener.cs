using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix.Control {

// , IPointerEnterHandler, IPointerExitHandler: 没有实现得这么细,没有到这层细节上来
    public class MoveButtonEventListener : MonoBehaviour, IPointerClickHandler { 
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



