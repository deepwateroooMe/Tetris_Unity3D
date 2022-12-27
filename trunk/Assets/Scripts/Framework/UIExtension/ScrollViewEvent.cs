using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollViewEvent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    // 开始拖拽
    public Action onBeginDrag;
    // 正在拖拽
    public Action onDraging;
    // 结束拖拽
    public Action onEndDrag;
    // 是否正在拖拽
    bool isDraging = false;

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        isDraging = true;
        if (onBeginDrag != null) {
            onBeginDrag();
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        isDraging = false;
        if (onEndDrag != null) {
            onEndDrag();
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        if (onDraging != null) {
            onDraging();
        }
    }
}
