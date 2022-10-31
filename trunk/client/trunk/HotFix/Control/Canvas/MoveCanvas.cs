using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Control {

// 尽量把平移画布的相关必要逻辑全部放这里面来处理    
    public class MoveCanvas : MonoBehaviour {
        private const string TAG = "MoveCanvas"; 

        private Vector3 delta;
        private GameObject left;
        private GameObject right;
        private GameObject up;
        private GameObject down;
        
        public void Awake() {
            Debug.Log(TAG + " Awake");
            delta = Vector3.zero;
            left = gameObject.FindChildByName("leftBtn");
            right = gameObject.FindChildByName("rightBtn");
            up = gameObject.FindChildByName("upBtn");
            down = gameObject.FindChildByName("downBtn");
// MoveCanvas: Four buttons:
            left.GetComponent<Button>().onClick.AddListener(OnClickLeftButton);
            right.GetComponent<Button>().onClick.AddListener(OnClickRightButton);
            up.GetComponent<Button>().onClick.AddListener(OnClickUpButton);
            down.GetComponent<Button>().onClick.AddListener(OnClickDownButton);
        }

        public void Start() {
            Debug.Log(TAG + " Start");
// Tetrominon: Spawned, Move, Rotate, Land,
            EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
// Canvas: Toggled
            EventManager.Instance.RegisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
        }

// [阴影会自动跟随;] 游戏视图模型会需要更新表格; 方块砖需要移动; 音频管理器需要操作背景音乐
        void OnClickLeftButton() {
            delta = new Vector3(-1, 0, 0);
            EventManager.Instance.FireEvent("move", delta);
        }
        void OnClickRightButton() {
            delta = new Vector3(1, 0, 0);
            EventManager.Instance.FireEvent("move", delta);
        }
        void OnClickUpButton() {
            delta = new Vector3(0, 0, 1);
            EventManager.Instance.FireEvent("move", delta);
        }
        void OnClickDownButton() {
            delta = new Vector3(0, 0, -1);
            EventManager.Instance.FireEvent("move", delta);
        }
        
// 经典模式下,或是启蒙模式游戏已经开始,激活按钮布
        void onActiveTetrominoSpawn(TetrominoSpawnedEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoSpawn");
            if (ViewManager.MenuView.ViewModel.gameMode > 0 || ViewManager.MenuView.ViewModel.gameMode == 0) 
                ViewManager.moveCanvas.SetActive(true);
        }
        
        void onActiveTetrominoMove(TetrominoMoveEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoMove");
            if ((int)info.delta.y != 0) // 平移画布只上下移动            
                ViewManager.moveCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
        }

        void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoRotate");
        }

        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoLand");
            ViewManager.moveCanvas.SetActive(false); // 这里没有失活
        }

        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
            ViewManager.rotateCanvas.SetActive(true);
            // ViewManager.moveCanvas.SetActive(false);
            gameObject.SetActive(false);
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void onDestroy() {
        }
    }
}