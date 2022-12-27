using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util {

    public class Camera3DWrap : CameraWrapBase {

        protected override void Initialize() {
            Transform.LookAt(Target);
            Vector3 angles = Transform.eulerAngles;
            curX = angles.x;
            curY = angles.y;
            curDistance = defaultDistance;
            desiredDistance = defaultDistance;
            correctedDistance = defaultDistance;
            if (Transform.GetComponent<Rigidbody>()) {
                Transform.GetComponent<Rigidbody>().freezeRotation = true;
            }
            Transform.LookAt(Target);
        }
        //// 拖拽
        //// <param name="gesture"></param>
        //public override void OnTouchDrag(DragGesture gesture) {
        //    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
        //        if (Input.GetMouseButton(0)) {
        //            RotateCamera(gesture.DeltaMove);
        //        } else if (Input.GetMouseButton(1) && FingerGestures.Touches.Count == 1) {
        //            TranslationCamera();
        //        }
        //    } else {
        //        if (FingerGestures.Touches.Count == 1) {
        //            RotateCamera(gesture.DeltaMove);
        //        } else {
        //            if (Mathf.Abs(gesture.TotalMove.x) > 80 || Mathf.Abs(gesture.TotalMove.y) > 80) {
        //                TranslationCamera();
        //            }
        //        }
        //    }
        //}
        Vector2 curMove = Vector2.zero;
        // 旋转相机
        // <param name="vec"></param>
        public void RotateCamera(Vector2 vec) {
            curMove = new Vector2(-vec.y * speedY, -vec.x * speedX) * Time.deltaTime;
        }
        // 平移相机
        public void TranslationCamera() {
            Vector3 mf = Vector3.zero;
            var dx = Input.GetAxis("Horizontal") * 0.015f;
            var dz = Input.GetAxis("Vertical") * 0.015f;
            dx = Mathf.Clamp(dx, -15, 15);
            dz = Mathf.Clamp(dz, -15, 15);
            if (dx != 0 || dz != 0) {
                var f = GameObject.transform.forward;
                var ff = new Vector3(-dx, 0, -dz);
                var d = new Vector3(-dx, 0, -dz);
                mf = Quaternion.LookRotation(d) * ff * d.magnitude * desiredDistance * 0.01f;
                if (Target != null) {
                    Target.position += mf;
                }
            }
        }
        public override void LateUpdate() {
            if (Target == null) {
                return;
            }
            GetInput();
            if (moveX != 0 || moveY != 0) {
                curMove = new Vector2(moveX, moveY);
            }
            CheckMove();
        }
        Vector3 position;
        void CheckMove() {
            if (curMove != Vector2.zero) {
                curX += curMove.x;
                curY -= curMove.y;
            }
            curX = ClampAngle(curX, minLimitX, maxDistance);
            Quaternion rotation = Quaternion.Euler(curX, curY, 0);
            correctedDistance = desiredDistance;
            bool isCorrected = false;
            curDistance = !isCorrected || correctedDistance > curDistance ? Mathf.Lerp(curDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;
        }
    }
}