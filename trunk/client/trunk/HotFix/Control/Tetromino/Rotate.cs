using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 给游戏视图预览中的一两个方块砖添加一个旋转效果,是填加一点儿酷炫效果,也是遮掩摄像机镜头可能调得不够好; 会耗性能
// 晚些时候再添加    
    public class Rotate : MonoBehaviour  {

        private Transform previewCamera;
        private Vector3 previewTetrominoScale = new Vector3(5f, 5f, 5f);
        private Vector3 originalPosition;
    
        void Start ()  {
            previewCamera = transform.parent.GetChild(0).gameObject.transform;
            // transform.localScale += previewTetrominoScale;
            transform.LookAt(previewCamera);
        }

        void Update () {
            //vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;
            //transform.Rotate (Vector3.forward * 25 * Time.deltaTime, Space.Self);
            //originalPosition = transform.position;
            //transform.position = Quaternion.AngleAxis(-30, transform.up) * originalPosition;

            transform.rotation = Quaternion.AngleAxis(-60, Vector3.right); // 这个旋转的方向还是有点儿不太对
            transform.Rotate (transform.up * 25 * Time.deltaTime, Space.Self);
        }
    }	
}
