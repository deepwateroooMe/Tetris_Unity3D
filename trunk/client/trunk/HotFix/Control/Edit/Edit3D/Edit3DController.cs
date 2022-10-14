using System;
using UnityEngine;

// 借助项目深入理解: MVC, MVP ? MVVM ? Control
namespace HotFix.Control.Edit {

    // 3D控制器
    public class Edit3DController {

        private static Edit3DController _instance;
        public static Edit3DController Instance {
            get {
                if (_instance == null) 
                    _instance = new Edit3DController();
                return _instance;
            }
        }

        // 当前选中的物体: 可以是教育模式下两种选择点击选择后的当前,也可以认为是传统经典模式下自动生成的下一个?
        public GameObject CurSelectGameObject {
            get;
            set;
        }

        // 手势相关的事件
        public void OnTouchTap(TapGesture gesture) {
            CurSelectGameObject = gesture.Selection.gameObject;
            Debug.Log("CurSelectGameObject: " + CurSelectGameObject);
        }
        public void OnTouchLongPress(LongPressGesture gesture) {
        }
        public void OnTouchDrag(DragGesture gesture) {
        }
        public void OnTouchPinch(PinchGesture gesture) {
        }
    }
}
