using Framework.Util;
using UnityEngine;

namespace HotFix.Control {

    // 相机基类
    public abstract class CameraBase {
// getters / setters
        public Camera Camera {
            get;
            private set;
        }
        public GameObject GameObject {
            get;
            set;
        }
        public UnityEngine.Transform Transform {
            get {
                return GameObject.transform;
            }
        }

// 抽象与虚拟方法等申明或定义
        public virtual CameraWrapBase Wrap {
            get;
            protected set;
        }

        //public Transform Target {
        //    get {
        //        return Wrap.Target;
        //    }
        //}

        public abstract CameraWrapBase CreateWrap();

        public void SetFieldOfView(float f) {
            if (Camera != null) {
                Camera.fieldOfView = f;
            }
        }
        public CameraBase(GameObject go) {
            GameObject = go;
            Camera = GameObject.GetComponent<Camera>();
            Wrap = CreateWrap();
        }

// 虚拟方法        
        public virtual void OnTouchDrag(DragGesture gesture) {
            //Wrap.OnTouchDrag(gesture);
        }
        //public virtual void OnPinch(PinchGesture gesture) {
        //    Wrap.OnPinch(gesture);
        //}
        protected virtual void Initialize() {
        }
    }
}