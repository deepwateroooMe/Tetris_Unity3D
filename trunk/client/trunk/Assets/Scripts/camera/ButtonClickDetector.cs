using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;

namespace tetris3d {

    // 【Unity】场景中有两个摄像机时射线检测不到问题
    // http://www.voidcn.com/article/p-xxlhsgwk-bpy.html
    
    public class ButtonClickDetector : MonoBehaviour {
        private const string TAG = "ButtonClickDetector";

        public float rayLength = 1000f;
        public LayerMask layerMask;
        public Camera prevGameObjectCamera;
        
        void Update () {  

            // if (Input.GetMouseButtonDown(0)) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                RaycastHit hit;
                // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
                Ray ray = prevGameObjectCamera.ScreenPointToRay(Input.mousePosition);  
                if (Physics.Raycast(ray, out hit, rayLength, layerMask)) {  
                    // 如果与物体发生碰撞，在Scene视图中绘制射线  
                    Debug.DrawLine(ray.origin, hit.point, Color.green);  
                    // 打印射线检测到的物体的名称  
                    Debug.Log("射线检测到的物体名称: " + hit.transform.name);
                    Debug.Log(TAG + " " + hit.collider.name); 
                }  
            }
        }
            
    }
}
   