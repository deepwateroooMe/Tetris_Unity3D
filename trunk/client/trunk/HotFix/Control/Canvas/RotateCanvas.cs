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
            // Debug.Log(TAG + " Awake");
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
        public void OnEnable() {
            // Debug.Log(TAG + " onEnable");
            Start();
        }
// 希望把每个类里所注册的事件及个数等固定,这件再有BUG的时候,会对自己的游戏逻辑比较熟悉
        public void Start() { 
            // Debug.Log(TAG + " Start");
// Canvas: Toggled
            EventManager.Instance.RegisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
// Tetrominon: Spawned, Move, Rotate, Land,
            // EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.RegisterListener<TetrominoValidMMInfo>(onActiveTetrominoMoveRotate);
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
            EventManager.Instance.RegisterListener<GameStopEventInfo>(onGameLeave);
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

        void onActiveTetrominoMoveRotate(TetrominoValidMMInfo info) { // 这个信息没有带变量,不知道移动位置
            // Debug.Log(TAG + " onActiveTetrominoMove");
            if (info.type.Equals("move") && (int)info.delta.y != 0) { // 平移画布只上下移动            
                ViewManager.moveCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
                ViewManager.rotateCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
            }
        }
        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            // Debug.Log(TAG + " onActiveTetrominoLand");
            // ViewManager.rotateCanvas.transform.position += new Vector3(0, 1, 0); // 向上移动一格至[0, 0, 0]
            ViewManager.rotateCanvas.SetActive(false); // 这里没有失活
            ViewManager.moveCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            ViewManager.rotateCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            if (GloData.Instance.gameMode > 0) 
                ViewManager.moveCanvas.SetActive(true); 
        }

        void onGameLeave(GameStopEventInfo info) {
            ViewManager.moveCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            ViewManager.rotateCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            ViewManager.moveCanvas.SetActive(false);
            ViewManager.rotateCanvas.SetActive(false);
        }
        
// BUG TODO: 最开始几次好像切换得没问题; 再多切换几次就忙不过来了?!!!没响应
// 总是停要平移画布上切不过去了,并且只能切换两次,需要DEBUG一下        
// 因为它这个缓慢的调用响应速度,必须得把方块砖下降的速度调得很慢,并需要在响应回来之前把按钮失活? 几个细节需要实现        
        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
            ViewManager.GameView.togBtn.GetComponent<Button>().image.overrideSprite = ViewManager.rotationsImg;
            ViewManager.rotateCanvas.SetActive(false);
            ViewManager.moveCanvas.SetActive(true);
            //ViewManager.GameView.btnState[togBtn] = true;
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void OnDisable() {
            // Debug.Log(TAG + " OnDisable");
// Canvas: Toggled
            EventManager.Instance.UnregisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
// Tetrominon: Spawned, Move, Rotate, Land,
            // EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.UnregisterListener<TetrominoValidMMInfo>(onActiveTetrominoMoveRotate);
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
            EventManager.Instance.UnregisterListener<GameStopEventInfo>(onGameLeave);
        }
	}
}
