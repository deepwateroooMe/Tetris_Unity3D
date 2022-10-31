using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Control {

// 尽量把 旋转画布 的相关必要逻辑全部放这里面来处理(平移画布的做完了,并通过测试; 旋转画面刚开始做)    
	public class RotateCanvas : MonoBehaviour {
        private const string TAG = "RotateCanvas"; 

        private Vector3 delta;
        private Button XPosBtn;
        private Button XNegBtn;
        private Button YPosBtn;
        private Button YNegBtn;
        private Button ZPosBtn;
        private Button ZNegBtn;

        public void Awake() {
            Debug.Log(TAG + " Awake");
            delta = Vector3.zero;
// 6 rotate Buttons: 想当然地要实现至少三组不同的旋转及位置(可以不实现三组,只用一组,但需要更为精确的摆放,以便他们也可以旋转,让他们的显示与否变得聪明一些)
            XPosBtn = gameObject.FindChildByName("posX").GetComponent<Button>();
            XNegBtn = gameObject.FindChildByName("negX").GetComponent<Button>();
            YPosBtn = gameObject.FindChildByName("posY").GetComponent<Button>();
            YNegBtn = gameObject.FindChildByName("negY").GetComponent<Button>();
            ZPosBtn = gameObject.FindChildByName("posZ").GetComponent<Button>();
            ZNegBtn = gameObject.FindChildByName("negZ").GetComponent<Button>();
        }
        
        public void Start() {
            Debug.Log(TAG + " Start");
// RotateCanvas: 六个旋转方向
            XPosBtn.onClick.AddListener(OnClickXPosButton);
            XNegBtn.onClick.AddListener(OnClickXNegButton);
            YPosBtn.onClick.AddListener(OnClickYPosButton);
            YNegBtn.onClick.AddListener(OnClickYNegButton);
            ZPosBtn.onClick.AddListener(OnClickZPosButton);
            ZNegBtn.onClick.AddListener(OnClickZNegButton);
// Tetrominon: Spawned, Move, Rotate, Land,
            // EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
// Canvas: Toggled
            EventManager.Instance.RegisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
        }

        void OnClickXPosButton() {
            delta = new Vector3(90, 0, 0);
            EventManager.Instance.FireEvent("rotate", delta);
        }
        void OnClickXNegButton() {
            delta = new Vector3(-90, 0, 0);
            EventManager.Instance.FireEvent("rotate", delta);
        }
        void OnClickYPosButton() {
            delta = new Vector3(0, 90, 0);
            EventManager.Instance.FireEvent("rotate", delta);
        }
        void OnClickYNegButton() {
            delta = new Vector3(0, -90, 0);
            EventManager.Instance.FireEvent("rotate", delta);
        } 
        void OnClickZPosButton() {
            delta = new Vector3(0, 0, 90);
            EventManager.Instance.FireEvent("rotate", delta);
        }
        void OnClickZNegButton() {
            delta = new Vector3(0, 0, -90);
            EventManager.Instance.FireEvent("rotate", delta);
        }

        void onActiveTetrominoMove(TetrominoMoveEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoMove");
// 平移画布只上下移动            
            if ((int)info.delta.y != 0) 
                ViewManager.rotateCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
        }

        void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoRotate");
        }

        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoLand");
            ViewManager.rotateCanvas.SetActive(false); // 这里没有失活
        }

        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
// 这里的一点儿交叉:是因为当另一个控件是失活状态,会丢掉或说接到不到相关事件,需要另一个画布帮助先激活            
            ViewManager.moveCanvas.SetActive(!ViewManager.moveCanvas.activeSelf);
            ViewManager.rotateCanvas.SetActive(!ViewManager.rotateCanvas.activeSelf);
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void onDestroy() {
            
        }
	}
}
