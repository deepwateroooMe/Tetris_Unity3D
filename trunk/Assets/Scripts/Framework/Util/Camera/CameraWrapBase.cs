using UnityEngine;
namespace Framework.Util {

    public abstract class CameraWrapBase : MonoBehaviour {

        public KeyCode leftKey = KeyCode.LeftArrow;
        public KeyCode rightKey = KeyCode.RightArrow;
        public KeyCode upKey = KeyCode.UpArrow;
        public KeyCode downKey = KeyCode.DownArrow;

        public KeyCode zoomInKey = KeyCode.Q;
        public KeyCode zoomOutKey = KeyCode.E;

        public float focusOffsetY = 1f;
        public float defaultDistance = 5f;
        public bool acceptInput = true;
        public bool isZoomSpeedUp = true;

        public float speedX = 180f;
        public float speedY = 80f;
        public float speedZoom = 30f;
        protected float moveX = 0f;
        protected float moveY = 0f;
        public float curX = 0f;
        public float curY = 0f;
        public float curDistance;
        public float desiredDistance;
        public float correctedDistance;
        public float maxDistance = 40f;
        public float minDistance = 3f;
        public float minLimitX = 0f;
        public float maxLimitX = 90f;
        public float zoomDampening = 10f;

        public virtual Transform Target {
            get;
            protected set;
        }
        public GameObject GameObject {
            get {
                return gameObject;
            }
        }
        public Transform Transform {
            get {
                return transform;
            }
        }
        protected virtual void Awake() {
        }
        public virtual void Update() {
        }
        public abstract void LateUpdate();

        public void GetInput() {
            moveX = 0f;
            moveY = 0f;
            if (!acceptInput) {
                return;
            }
            if (Input.GetKey(downKey)) {
                moveY = -speedY * Time.deltaTime;
            }
            if (Input.GetKey(leftKey)) {
                moveX = speedX * Time.deltaTime;
            }
            if (Input.GetKey(rightKey)) {
                moveX = -speedX * Time.deltaTime;
            }
            if (Input.GetKey(upKey)) {
                moveY = speedY * Time.deltaTime;
            }
            if (Input.GetKey(zoomInKey)) {
                Zoom(1);
            } else if (Input.GetKey(zoomOutKey)) {
                Zoom(-1);
            }
        }

        protected void Zoom(float input) {
            input = Mathf.Clamp(input, -1, 1);
            float final = input * Time.deltaTime * speedZoom;
            if (isZoomSpeedUp) {
                final += Mathf.Abs(desiredDistance) * 0.1f;
            }
            desiredDistance = Mathf.Clamp(desiredDistance + final, minDistance, maxDistance);
        }
        //public virtual void OnTouchDrag(DragGesture gesture) {
        //}
        //public virtual void OnPinch(PinchGesture gesture) {
        //    float delta = gesture.Delta;
        //    if (delta != 0) {
        //        Zoom(-delta);
        //    }
        //}
        protected float ClampAngle(float angle, float min, float max) {
            if (angle < -360) {
                angle += 360;
            }
            if (angle > 360) {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
        protected virtual void Initialize() {
        }
    }
}