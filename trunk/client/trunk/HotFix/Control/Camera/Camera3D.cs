using Framework.Util;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

    // 3D场景相机
    public class Camera3D : CameraBase {

        public Camera3DWrap Camera3DWrap {
            get;
            set;
        }
        public Camera3D(GameObject go) : base(go) { }

        public override CameraWrapBase CreateWrap() {
            Camera3DWrap = GameObject.GetOrAddComponent<Camera3DWrap>();
            Wrap = Camera3DWrap;
            return Camera3DWrap;
        }

        public override void OnTouchDrag(DragGesture gesture) {
            // if (ViewManager.IsPo)
        }
    }
}

