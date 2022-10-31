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
// 6 rotate Buttons: 想当然地要实现至少三组不同的旋转及位置(可以不实现三组;
// 也可以只用一组,但需要更为精确的摆放,以便他们也可以旋转,让他们的显示与否变得更聪明一点儿)
            XPosBtn = gameObject.FindChildByName("posX").GetComponent<Button>();
            XNegBtn = gameObject.FindChildByName("negX").GetComponent<Button>();
            YPosBtn = gameObject.FindChildByName("posY").GetComponent<Button>();
            YNegBtn = gameObject.FindChildByName("negY").GetComponent<Button>();
            ZPosBtn = gameObject.FindChildByName("posZ").GetComponent<Button>();
            ZNegBtn = gameObject.FindChildByName("negZ").GetComponent<Button>();
            XPosBtn.onClick.AddListener(OnClickXPosButton);
            XNegBtn.onClick.AddListener(OnClickXNegButton);
            YPosBtn.onClick.AddListener(OnClickYPosButton);
            YNegBtn.onClick.AddListener(OnClickYNegButton);
            ZPosBtn.onClick.AddListener(OnClickZPosButton);
            ZNegBtn.onClick.AddListener(OnClickZNegButton);
        }
        
        public void Start() {
            Debug.Log(TAG + " Start");
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
            // Debug.Log(TAG + " onActiveTetrominoMove");
            if ((int)info.delta.y != 0) // 平移画布只上下移动            
                ViewManager.rotateCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
        }
        void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            // Debug.Log(TAG + " onActiveTetrominoRotate");
        }
        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoLand");
            ViewManager.rotateCanvas.SetActive(false); // 这里没有失活
        }

// BUG TODO: 最开始几次好像切换得没问题; 再多切换几次就忙不过来了?!!!没响应
// 总是停要平移画布上切不过去了,并且只能切换两次,需要DEBUG一下        
// 因为它这个缓慢的调用响应速度,必须得把方块砖下降的速度调得很慢,并需要在响应回来之前把按钮失活? 几个细节需要实现        
        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
            ViewManager.moveCanvas.SetActive(true);
            ViewManager.rotateCanvas.SetActive(false);
            // ViewManager.moveCanvas.SetActive(true);
            // gameObject.SetActive(false);
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void OnDisable() {
            Debug.Log(TAG + " OnDisable");
// Tetrominon: Spawned, Move, Rotate, Land,
            // EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
// Canvas: Toggled
            EventManager.Instance.UnregisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
        }
	}
}

