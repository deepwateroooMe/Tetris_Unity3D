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

        // private TetrominoMoveEventInfo moveInfo;
        private Vector3 delta;
        private GameObject left;
        private GameObject right;
        private GameObject up;
        private GameObject down;
        
        public void Awake() {
            Debug.Log(TAG + " Awake");
            //moveInfo = new TetrominoMoveEventInfo();
            delta = Vector3.zero;
            left = gameObject.FindChildByName("leftBtn");
            right = gameObject.FindChildByName("rightBtn");
            up = gameObject.FindChildByName("upBtn");
            down = gameObject.FindChildByName("downBtn");
// // 添加启动四个按键的感受器: 但是因为它们是静态的,测试运行感觉他们并不能被主动触发,所以还是按照把四个按钮放这里管理的热更新程序域里的相对传统的写法
//             ComponentHelper.AddMoveBtnsListener(left);
//             ComponentHelper.AddMoveBtnsListener(right);
//             ComponentHelper.AddMoveBtnsListener(up);
//             ComponentHelper.AddMoveBtnsListener(down);
        }

        public void Start() {
            Debug.Log(TAG + " Start");
// MoveCanvas: Four buttons:
            left.GetComponent<Button>().onClick.AddListener(OnClickLeftButton);
            right.GetComponent<Button>().onClick.AddListener(OnClickRightButton);
            up.GetComponent<Button>().onClick.AddListener(OnClickUpButton);
            down.GetComponent<Button>().onClick.AddListener(OnClickDownButton);
// // 平移按钮的事件接收: 触发进一步的回调
//             EventManager.Instance.RegisterListener<MoveButtonClickEventInfo>(onMoveButtonClicked);
// Tetrominon: Spawned, Move, Rotate, Land,
            EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
// Canvas: Toggled
            EventManager.Instance.RegisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
        }

        // public void onMoveButtonClicked(MoveButtonClickEventInfo info) { // assign different callbacks/events according to button clicked
        //     Debug.Log(TAG + ": onMoveButtonClicked()");
        //     Debug.Log(TAG + " info.unitGO.name: " + info.unitGO.name); 
        //     if (info.unitGO == right)        // right XPos
        //         delta = new Vector3(1, 0, 0);
        //     else if (info.unitGO == left)  // left XNeg
        //         delta = new Vector3(-1, 0, 0);
        //     else if (info.unitGO == up)  // up ZPos
        //         delta = new Vector3(0, 0, 1);
        //     else if (info.unitGO == down)  // down ZNeg
        //         delta = new Vector3(0, 0, -1);
        //     moveInfo.delta = delta;
        //     EventManager.Instance.FireEvent(moveInfo);
        // }
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
            if (ViewManager.MenuView.ViewModel.gameMode > 0 || ViewManager.MenuView.ViewModel.gameMode == 0) {
                // ComponentHelper.GetMoveCanvasComponent(ViewManager.moveCanvas).enabled = true;
                ViewManager.moveCanvas.SetActive(true);
            }
        }
        
        void onActiveTetrominoMove(TetrominoMoveEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoMove");
// 平移画布只上下移动            
            if ((int)info.delta.y != 0) 
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
            ViewManager.moveCanvas.SetActive(!ViewManager.moveCanvas.activeSelf);
        }

// TODO: 适配器适配的方法太少,会导致一堆的资源泄露?        
        public void onDestroy() {
            
        }
    }
}