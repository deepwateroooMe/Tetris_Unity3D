using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 这里可能是风马牛不相干的：只是考虑可以触屏实现大立方体座基的旋转；是最后的可有可可无的功能，可以再作考虑
// 是转变viewport等，就当是拖相机呗
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
