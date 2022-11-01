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

        private GameObject left;
        private GameObject right;
        private GameObject up;
        private GameObject down;
        private Vector3 delta;
        
        public void Awake() {
            // Debug.Log(TAG + " Awake");
            delta = Vector3.zero;
            left = gameObject.FindChildByName("leftBtn");
            right = gameObject.FindChildByName("rightBtn");
            up = gameObject.FindChildByName("upBtn");
            down = gameObject.FindChildByName("downBtn");
            left.GetComponent<Button>().onClick.AddListener(OnClickLeftButton);
            right.GetComponent<Button>().onClick.AddListener(OnClickRightButton);
            up.GetComponent<Button>().onClick.AddListener(OnClickUpButton);
            down.GetComponent<Button>().onClick.AddListener(OnClickDownButton);
        }

// 在Unity3D中，实际上 Start 函数只在脚本运行时，运行一次，然后便不再执行此函数，
// 那么如何能够多次使用呢？这里实际上用了一个讨巧的方法，就是利用  OnEnable 函数。
        public void OnEnable() {
            // Debug.Log(TAG + " OnEnable");
            // 在ViewManager中第一次设置这个对象SetActive(false)的时候, OnEnable()没能调用,导致注册监听会容易不成功         
            Start(); // 借一步调用,能够确保保证监听注册成功
        }

        public void Start() { 
            Debug.Log(TAG + " Start");
// Canvas: Toggled
            EventManager.Instance.RegisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
// Tetrominon: Spawned, Move, Rotate, Land,
            EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.RegisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            // EventManager.Instance.RegisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.RegisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand);
// TODO: onUndoGame: SetActive(false);
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
// TODO: FOR GAMEMODE = 0, GameView里清除掉帮助启动的逻辑
                ViewManager.moveCanvas.SetActive(true); 
        }
        
        void onActiveTetrominoMove(TetrominoMoveEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoMove");
            if ((int)info.delta.y != 0) // 平移画布只上下移动            
                ViewManager.moveCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
        }

        // void onActiveTetrominoRotate(TetrominoRotateEventInfo info) {
        //     // Debug.Log(TAG + " onActiveTetrominoRotate");
        // }

        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoLand");
            ViewManager.moveCanvas.SetActive(false); // 这里没有失活
        }

        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
// 切换失活与激活的先后顺序有一定的关系,会产生交叉会死锁
            ViewManager.moveCanvas.SetActive(false);
            ViewManager.rotateCanvas.SetActive(true);
        }

        public void OnDisable() {
            Debug.Log(TAG + " OnDisable");
// Canvas: Toggled
            EventManager.Instance.UnregisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
// Tetrominon: Spawned, Move, Rotate, Land,
            EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.UnregisterListener<TetrominoMoveEventInfo>(onActiveTetrominoMove); 
            // EventManager.Instance.UnregisterListener<TetrominoRotateEventInfo>(onActiveTetrominoRotate); 
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
        }
    }
}