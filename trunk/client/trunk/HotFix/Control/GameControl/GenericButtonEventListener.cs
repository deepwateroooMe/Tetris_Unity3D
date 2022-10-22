using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix.Control {

    // 当有两个不同的程序域,当项目大到一定的程度之后,
    // 必须得像最初测试热更新框架一样来测试这些是否如期运行,否则现在整合过来的,可能到时还都得删除掉
    public class GenericButtonEventListener : MonoBehaviour, IPointerClickHandler { 
        private const string TAG = "GenericButtonEventListener";

        private GenericButtonClickEventInfo info;

        public void OnPointerClick(PointerEventData eventData){
            Debug.Log(TAG + ": OnPointerClick()");
            Debug.Log(TAG + " this.gameObject.name: " + this.gameObject.name); 

            info = new GenericButtonClickEventInfo();
            info.unitGO = this.gameObject;
            Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 
            //EventManager.Instance.FireEvent(info);
        }

    }
}
