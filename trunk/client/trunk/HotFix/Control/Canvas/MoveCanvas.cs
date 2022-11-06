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
        // private Button togBtn;
        
        public void Awake() {
            // Debug.Log(TAG + " Awake");
            // togBtn = ViewManager.GameView.togBtn; // 会不会报错报空
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
            // EventManager.Instance.RegisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); // 接收不到
            EventManager.Instance.RegisterListener<TetrominoValidMMInfo>(onActiveTetrominoMoveRotate);
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
        
        void onActiveTetrominoMoveRotate(TetrominoValidMMInfo info) { // 这个信息没有带变量,不知道移动位置
            Debug.Log(TAG + " onActiveTetrominoMove");
            if (info.type.Equals("move") && (int)info.delta.y != 0) { // 平移画布只上下移动            
                ViewManager.moveCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
                ViewManager.rotateCanvas.gameObject.transform.position += new Vector3(0, info.delta.y, 0);
            }
        }

        void onActiveTetrominoLand(TetrominoLandEventInfo info) {
            Debug.Log(TAG + " onActiveTetrominoLand");
            ViewManager.moveCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            ViewManager.rotateCanvas.transform.position = new Vector3(2.0f, 11.0f, 2f);
            if (ViewManager.GameView.ViewModel.gameMode.Value == 0)            
                ViewManager.moveCanvas.SetActive(false); // 这里没有失活, 还是说它需要那么久的影应时间呢?
        }

        void onCanvasToggled(CanvasToggledEventInfo info) {
            Debug.Log(TAG + " CanvasToggledEventInfo");
// 切换失活与激活的先后顺序有一定的关系,会产生交叉会死锁
            ViewManager.GameView.togBtn.GetComponent<Button>().image.overrideSprite = ViewManager.directionsImg;
            ViewManager.moveCanvas.SetActive(false);
            ViewManager.rotateCanvas.SetActive(true);
            //ViewManager.GameView.btnState[togBtn] = true;
        }

        public void OnDisable() {
            Debug.Log(TAG + " OnDisable");
// Canvas: Toggled
            EventManager.Instance.UnregisterListener<CanvasToggledEventInfo>(onCanvasToggled); 
// Tetrominon: Spawned, Move, Rotate, Land,
            EventManager.Instance.UnregisterListener<TetrominoSpawnedEventInfo>(onActiveTetrominoSpawn); 
            EventManager.Instance.UnregisterListener<TetrominoValidMMInfo>(onActiveTetrominoMoveRotate);
            EventManager.Instance.UnregisterListener<TetrominoLandEventInfo>(onActiveTetrominoLand); 
        }
    }
}