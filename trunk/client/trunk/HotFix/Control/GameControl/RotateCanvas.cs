using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

// 尽量把 旋转画布 的相关必要逻辑全部放这里面来处理    
	public class RotateCanvas : MonoBehaviour {
        private const string TAG = "RotateCanvas"; 

        public void Awake() {
            Debug.Log(TAG + " Awake");
        }
        public void Start() {
            Debug.Log(TAG + " Start");
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
// 暂时不管,将来可能可以旋转
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
        }

        void onActiveTetrominoMove(TetrominoMoveEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoMove");
// 平移画布只上下移动            
            if ((int)info.delta.y != 0) 
                ViewManager.rotateCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void onDestroy() {
            
        }
	}
}

