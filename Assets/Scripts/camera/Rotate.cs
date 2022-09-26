using UnityEngine;
using System.Collections;

namespace tetris3d {

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