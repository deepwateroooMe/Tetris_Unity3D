using UnityEngine;
using System.Collections;

namespace Framework.Util {

//摄像机跟随
    public class FollowPlayer : MonoBehaviour {
        private const string TAG = "FollowPlayer";
    
        public Transform playerRotate;
        public GameObject[] rotateCanvas;
        public float horizontalSpeed, verticalSpeed;
    
        private Transform player;        // 角色
    
        private  Vector3 offsetPosition; // 位置偏移
        private bool isRotating = false; // 是否有在滑动

        public float distance;           // 向量长度
        public float scrollSpeed = 10;   // 拉近拉远的速度
    
        public float rotateSpeed = 30f;  // 旋转的速度
        private float originalY;

        private Vector3 originalPos;
        private Quaternion originalRotation;
        private float x, y, z, h, v;
    
        void Start ()  {
            player = GameObject.FindGameObjectWithTag("rotatePlayer").transform;
            transform.LookAt(player.transform);
            offsetPosition = transform.position - player.position; // 得到偏移量

            originalPos = transform.position;
            originalY = transform.eulerAngles.y;
        }
    
        void LateUpdate() {   
// #if UNITY_ANDROID
//         if (Input.touchCount > 0) {
//             Touch t = Input.GetTouch(0);
//             if (t.phase == TouchPhase.Began) {
//                 previousUnitPosition = new Vector2(t.position.x, t.position.y);
//             } else if (t.phase == TouchPhase.Moved) {
//                 Vector2 touchDeltaPosition = t.deltaPosition;
//                 direction = touchDeltaPosition.normalized;
//                 if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal
//                     && direction.x < 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) { // move left
//                     MoveXNeg();
//                     previousUnitPosition = t.position;
//                     // moved = true;
//                 } else if (Mathf.Abs(t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal
//                            && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) { // move right
//                     MoveXPos();
//                     previousUnitPosition = t.position;
//                     // moved = true;
//                 } else if (Mathf.Abs(t.position.y - previousUnitPosition.y) >= touchSensitivityVertical &&
//                            direction.y < 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10) { // move Down
//                     // Movedown();
//                     previousUnitPosition = t.position;
//                     // moved = true;
//                 }
//             }
//             else if (t.phase == TouchPhase.Ended) { // double click?  // 这里我替时不考虑这种情况
//                 if (!moved && t.position.x > Screen.width / 4) 
//                     Rotate();
//                 // moved = false;
//             }
//         }
#if UNITY_ANDROID  //if (Application.platform == RuntimePlatform.Android) {
            if (Input.touchCount > 0) {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) {
                    //if (Input.touchCount == 1) {  // 单指触屏滑动，物体旋转
                    originalPos = transform.position;  // 这个取值可能是有些问题的
                    originalRotation = transform.rotation;
                    if (t.phase == TouchPhase.Moved) {
                        h = Input.GetAxis("Mouse X"); // 右正左负
                        v = Input.GetAxis("Mouse Y"); // 上正下负
                        if (Mathf.Abs(h) >= Mathf.Abs(v)) 
                            transform.RotateAround(player.position, player.up, horizontalSpeed * h);
                        else 
                            transform.RotateAround(playerRotate.position, transform.right, -verticalSpeed * v);
                    }
                    // 限制上下滑动的度数大小
                    x = transform.eulerAngles.x;
                    y = transform.eulerAngles.y;
                    z = transform.eulerAngles.z;
                    // 旋转的范围是 [0， 00]度
                    if (x < 0 || x > 80 || z > 80) { // 当超出范围之后，我们将属性归位，让旋转无效
                        transform.position = originalPos;
                        transform.rotation = originalRotation;
                    }
                }
            }
#endif
        
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1)) 
                isRotating = true;
            if (Input.GetMouseButtonUp(1)) 
                isRotating = false;
            if (isRotating) {
                Vector3 originalPos = transform.position;
                Quaternion originalRotation = transform.rotation;
                float dx = Input.GetAxis("Mouse X");
                float dy = - Input.GetAxis("Mouse Y");
                if (Mathf.Abs(dx) > Mathf.Abs(dy))
                    transform.RotateAround(player.position, player.up, rotateSpeed * Input.GetAxis("Mouse X")); // 围拢角色滑动左右, 方向问题
                else
                    transform.RotateAround(playerRotate.position, transform.right, - rotateSpeed * Input.GetAxis("Mouse Y")); // playerRotate
                // 上下 (会影响到的属性一个是Position, 一个是rotation)
                // 限制上下滑动的度数大小 ====》 这个一定程度上的限制，也是需要的
                float x = transform.eulerAngles.x;
                float y = transform.eulerAngles.y;
                float z = transform.eulerAngles.z;
                // 旋转的范围是 [10， 80]度
                if (x < 10 || x > 80 || z > 80) { // 当超出范围之后，我们将属性归位，让旋转无效
                    transform.position = originalPos;
                    transform.rotation = originalRotation;
                }
                offsetPosition = transform.position - player.position;
            }
#endif
            // // rotateCanvas 的随机生成问题
            // //float offsetY = (y - originalY) % 360;
            // if ((offsetY >= 0 && offsetY < 90) || (offsetY >= -180 && offsetY < -90) || 
            //     (offsetY >= 180 && offsetY < 270) || (offsetY < -270 && offsetY > -360)) { // trials
            //     if ((offsetY >= 90 && offsetY < 180) || (offsetY >= 270 && offsetY < 360) || 
            //         (offsetY >= -90 && offsetY < 0) || (offsetY >= -270 && offsetY < -180)) {
            //         Debug.Log(TAG + ": LateU() rotateCanvas[1].enabled: " + rotateCanvas[1].activeSelf); 
            //         rotateCanvas[0].SetActive(false);
            //         rotateCanvas[1].SetActive(true);
            //     } else {
            //         rotateCanvas[0].SetActive(true);
            //         rotateCanvas[1].SetActive(false);
            //     }
            //     //=====》》 当移出屏幕后，最后能快速复位，回到原始的位置，方便操作 another trial
            //     dx *= rotateSpeed;
            //     dy *= rotateSpeed;
            //     if (Mathf.Abs(dx) > 0 || Mathf.Abs(dy) > 0) {
            //         // 获取摄像机欧拉角
            //         Vector3 angles = transform.rotation.eulerAngles;
            //         // 欧拉角表示按照坐标顺序旋转，比如angles.x=30，表示按x轴旋转30°，dy改变引起x轴的变化
            //         angles.x = Mathf.Repeat(angles.x + 180f, 360f) - 180f;
            //         angles.y += dx;
            //         angles.x -= dy;
            //         // 计算摄像头旋转
            //         targetRotation.eulerAngles = new Vector3(angles.x, angles.y, 0);
            //         // 随着旋转，摄像头位置自动恢复
            //         Vector3 temp_position = Vector3.Lerp(targetPosition, model.position, Time.deltaTime * moveLerp);
            //         targetPosition = Vector3.Lerp(targetPosition, temp_position, Time.deltaTime * moveLerp);
            //     }
            // }
            // offsetPosition = transform.position - player.position;
        }

        // 处理视野的拉近和拉远效果
        void ScrollView() {
            // print(Input.GetAxis("Mouse ScrollWheel"));//鼠标向后滑动返回负数（拉近视野），向前正数（拉远视野）
            distance = offsetPosition.magnitude;
            distance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            distance = Mathf.Clamp(distance, 1, 40);
            offsetPosition = offsetPosition.normalized * distance;//改变位置便移
        }

        //控制视野左右上下旋转
        void RotateView() { // 右键 -- 旋转
            if (Input.GetMouseButtonDown(1)) 
                isRotating = true;

            if (Input.GetMouseButtonUp(1)) 
                isRotating = false;

            if (isRotating) {
                Vector3 originalPos = transform.position;
                Quaternion originalRotation = transform.rotation;

                transform.RotateAround(player.position, player.up, rotateSpeed * Input.GetAxis("Mouse X")); // 围拢角色滑动左右
                //transform.RotateAround(player.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));

                // 上下 (会影响到的属性一个是Position, 一个是rotation)

                // 限制上下滑动的度数大小
                float x = transform.eulerAngles.x;
                // 旋转的范围是 [10， 80]度
                if (x < 10 || x > 80) { // 当超出范围之后，我们将属性归位，让旋转无效
                    transform.position = originalPos;
                    transform.rotation = originalRotation;
                }
            }
            offsetPosition = transform.position - player.position;
        }

        // 另一个版本，用键控制的绕 y, x 轴旋转的 i k j l
        // void RotateView() { // 右键 -- 旋转
        //     if (Input.GetKey(KeyCode.I))     // Rotate x up
        //         RotateXUp();
        //     if (Input.GetKey(KeyCode.K))     // rotate x down
        //         RotateXDown();
        //     if (Input.GetKeyDown(KeyCode.J)) // rotate y left
        //         RotateYLeft();
        //     if (Input.GetKey(KeyCode.L))     // rotate y right 这个可以运行，但是反方向不行，为什么呢？
        //         RotateYRight();	
        // }

        void RotateYLeft() {
            transform.RotateAround(new Vector3(2.5f, 6f, 2.5f), player.up, rotateSpeed * -1f); // 围拢角色滑动左右 , player
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            offsetPosition = transform.position - player.position; 
        }

        void RotateYRight() {
            transform.RotateAround(new Vector3(2.5f, 6f, 2.5f), player.up, rotateSpeed); // 围拢角色滑动左右 , player
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            // transform.RotateAround(player.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
            // 上下 (会影响到的属性一个是Position, 一个是rotation)
            // 限制上下滑动的度数大小
            // float x = transform.eulerAngles.x;
            // if (x < 5 || x > 85) { // 当超出范围之后，我们将属性归位，让旋转无效
            //     transform.position = originalPos;
            //     transform.rotation = originalRotation;
            // }
            offsetPosition = transform.position - player.position; 
        }
    
        void RotateXUp() {
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            transform.RotateAround(player.position, transform.right, -rotateSpeed * 0.6f);
            // 上下 (会影响到的属性一个是Position, 一个是rotation)

            // 限制上下滑动的度数大小
            float x = transform.eulerAngles.x;
            if (x < 5 || x > 85) { // 当超出范围之后，我们将属性归位，让旋转无效
                transform.position = originalPos;
                transform.rotation = originalRotation;
            } 
            offsetPosition = transform.position - player.position; 
        }
    
        void RotateXDown() {
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            transform.RotateAround(player.position, transform.right, rotateSpeed * 1f);
            // 上下 (会影响到的属性一个是Position, 一个是rotation)

            // 限制上下滑动的度数大小
            float x = transform.eulerAngles.x;
            if (x < 5 || x > 85) { // 当超出范围之后，我们将属性归位，让旋转无效
                transform.position = originalPos;
                transform.rotation = originalRotation;
            } 
            offsetPosition = transform.position - player.position; 
        }

        //控制视野左右上下旋转
        // void RotateView() { // 右键 -- 旋转
        //     if (Input.GetMouseButtonDown(1)) 
        //         isRotating = true;
        //     if (Input.GetMouseButtonUp(1)) 
        //         isRotating = false;
        //     if (isRotating) {
        //         transform.RotateAround(new Vector3(2.5f, 6f, 2.5f), player.up, rotateSpeed * Input.GetAxis("Mouse X")); // 围拢角色滑动左右 , player.position
        //         Vector3 originalPos = transform.position;
        //         Quaternion originalRotation = transform.rotation;
        //         transform.RotateAround(player.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
        //         // 上下 (会影响到的属性一个是Position, 一个是rotation)
        //         // 限制上下滑动的度数大小
        //         float x = transform.eulerAngles.x;
        //         if (x < 5 || x > 85) { // 当超出范围之后，我们将属性归位，让旋转无效
        //             transform.position = originalPos;
        //             transform.rotation = originalRotation;
        //         }
        //     }
        //     offsetPosition = transform.position - player.position;
        // }
    }
}