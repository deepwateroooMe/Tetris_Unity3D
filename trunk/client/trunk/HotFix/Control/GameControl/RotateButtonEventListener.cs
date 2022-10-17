using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix.Control {

    public class RotateButtonEventListener : MonoBehaviour, IPointerClickHandler { 
        private const string TAG = "RotateButtonEventListener";

        private RotateButtonClickEventInfo rotateEventInfo;

        public void OnPointerClick(PointerEventData eventData){
            Debug.Log(TAG + ": OnPointerClick()");

            if (rotateEventInfo == null)
                rotateEventInfo = new RotateButtonClickEventInfo();
            rotateEventInfo.unitGO = this.gameObject;
            Debug.Log(TAG + " rotateEventInfo.unitGO.name: " + rotateEventInfo.unitGO.name); 
            EventManager.Instance.FireEvent(rotateEventInfo);
        }

    }
}
