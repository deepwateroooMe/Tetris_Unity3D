using System;
using UnityEngine;
using System.Collections.Generic;
using Framework.MVVM;
using Framework.Util;
using System.Collections;
using System.Json;
using System.Text;
using deepwaterooo.tetris3d;
using HotFix.Control;
using HotFix.Data;
using UnityEngine.EventSystems;
using HotFix.UI.View.SettingsView;
using UnityEngine.UI;

namespace HotFix.UI {

    public static class ViewManager {
        private const string TAG = "ViewManager"; 

        public static Canvas UI2DRoot; // 更应该叫UI3DRoot,无所谓了
        public static Dictionary<string, UnityGuiView> views = new Dictionary<string, UnityGuiView>();

        public static void InitializeStartUI() {
            CreateBaseUI();
        }
        static void CreateBaseUI() {
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/ui2droot",
                    "UI2DRoot", 
                    (go) => {
                        go.name = "UI2DRoot";
                        GameObject.DontDestroyOnLoad(go); 
// 游戏运行时,不知道什么原因,会呈现一些不确定性:比如旋转某些角度等,想要把它们摆定
                        // go.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
                        CoroutineHelper.StartCoroutine(GetRectSize(go.GetComponent<RectTransform>()));

                        UI2DRoot = go.GetComponent<Canvas>();
                        var viewRoot = new GameObject("ViewRoot"); // 实例化一个新空控件当作是视图层的根节点
                        viewRoot.layer = LayerMask.NameToLayer("UI");

                        // Managers: slightly higher level than GameView so they Initialized and be preapred for receiving events info
                        var managersRoot = new GameObject("Managers");
                        managersRoot.transform.SetParent(UI2DRoot.transform);
                        EventManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);
                        AudioManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);

                        var viewRect = viewRoot.AddComponent<RectTransform>();
                        viewRect.SetParent(UI2DRoot.transform, false);
                        viewRect.sizeDelta = new Vector2(0, 0);
                        viewRect.anchorMin = Vector2.zero;
                        viewRect.anchorMax = Vector2.one; 
                        viewRect.pivot = new Vector2(0.5f, 0.5f);

                        ShowStartPanel(); 
                    }, EAssetBundleUnloadLevel.Never);
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/view/btnscanvasview",
                    "BtnsCanvasView", // 世界坐标系视图
                    (go) => {
                        go.name = "BtnsCanvasView";
                        GameObject.DontDestroyOnLoad(go); 
                        moveCanvas = go.FindChildByName("moveCanvas");
                        rotateCanvas = go.FindChildByName("rotateCanvas");
                        ComponentHelper.AddMoveCanvasComponent(moveCanvas);
                        moveCanvas.SetActive(false); 
                        ComponentHelper.AddRotateCanvasComponent(rotateCanvas);
                        rotateCanvas.SetActive(false);
                        PoolHelper.Initialize(); // 部分相关逻辑提练到静态帮助类里去定义和完成
                        tetrosPool = go.FindChildByName("tetrosPool");
                        tetroParent = go.FindChildByName("TetrominosContainer");
                        GameObject parent = go.FindChildByName("Prefabs");
                        foreach (Transform child in parent.transform) {
                            PoolHelper.fillPool(child);
                        }
                        parent.SetActive(false);
                    }, EAssetBundleUnloadLevel.Never);
        }
#region BtnsCanvasView
        public static GameObject moveCanvas = null;
        public static GameObject rotateCanvas = null;
        public static GameObject nextTetromino = null; // 放这里,主要是方便GameViewModel和Tetromino GhostTetromino来拿到reference
        public static GameObject ghostTetromino = null;
        public static Dictionary<string, GameObject> minosDic = null;
        public static Dictionary<string, Stack<GameObject>> pool = null;
        public static GameObject tetrosPool = null;
        public static GameObject tetroParent = null;
#endregion
        static IEnumerator GetRectSize(RectTransform rt) { // 自己添加到这里的
//             //RectTransform rt = go.GetComponent<RectTransform>();
//             float obj_width = rt.rect.size.x;
//             float obj_height = rt.rect.size.y;
//             //Canvas出现[Some Values Driven By Canvas]提示时UI物体不能及时获取到宽高，需等待
//             yield return obj_width != 0 && obj_width != 0;
//             Debug.Log($"宽 = {obj_width}  高 = {obj_height}");
//             rt.sizeDelta = new Vector2(1920, 3412);
// //改变RectTransform的pos
//             // rt.anchoredPosition3D = new Vector3(posx,posy,posz);
//             // rt.anchoredPosition = new Vector2(posx,posy); 
//             rt.anchoredPosition3D = new Vector3(130, 231, 0);
            rt.rotation = Quaternion.Euler(Vector3.zero);
            // obj_width = rt.rect.size.x;
            // obj_height = rt.rect.size.y;
            // Debug.Log($"宽 = {obj_width}  高 = {obj_height}");
            // Debug.Log("rt.anchoredPosition3D: x: " + rt.anchoredPosition3D.x + ", y: " + rt.anchoredPosition3D.y + ", z: " + rt.anchoredPosition3D.z);
            // Debug.Log("rt.rotation: x: " + rt.rotation.x + ", y: " + rt.rotation.y + ", z: " + rt.rotation.z);
            yield return null;
        }

// 遍历当前视图管理器里所管理的所有的视图，凡是不是所指定特定视图的，并且是根视图,一律隐藏起来
        // （应该只是不让用户看见，它还在那里，在幕后的某个角落乘凉）
// 问题是:其它的不是根视图的,视图管理器它不管 ?!!!        
        public static void CloseOtherRootViews(string viewName) {
            foreach (var view in views.Values)
// 设置根视图层级:那么若是根视图下仍有好几个子视图,就能够站在更高的层面上统一调整其以及所有子视图的显示与隐藏,比如游戏视图需要设置根视图为TRUE
                // if (view.ViewName != viewName && view.IsRoot) // 我把这里改写了,因为我目前还没有调控IsRoot视图参数
                    if (view.ViewName != viewName) 
                    view.Hide();
        }

        static void ShowStartPanel() {
            MenuView.Reveal();
            // ViewManager.DownRootView.Reveal(); // 考虑这里是否需要将游戏模式转化为gameMode index或是int值?
        }
// TODO:下面的这个小部分,可以我能够弄得再更明白一点儿,从来可以优化自己按钮点击事件传递系统的一个入口        
#region Other
        static bool isOverUI = false;
        static bool isCheckedOverUI = false;
        static List<RaycastResult> raycastResults = new List<RaycastResult>();
        public static bool IsPointerOverUI() {
            if (isCheckedOverUI) {
                return isOverUI;
            }
            isCheckedOverUI = true;
            isOverUI = false;
            if (EventSystem.current.IsPointerOverGameObject()) {
                isOverUI = true;
            }
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if (raycastResults.Count > 0) {
                isOverUI = true;
            }
            TryStopTapEvent();
            return isOverUI;
        }
        public static void TryStopTapEvent() {
            CoroutineHelper.StartCoroutine(StopTapEvent());
        }
        static IEnumerator StopTapEvent() {
            yield return new WaitForEndOfFrame();
            isOverUI = false;
            isCheckedOverUI = false;
        }
#endregion
#region Views
        static MenuView _menuView;
        public static MenuView MenuView {
            get {
                if (_menuView == null) {
                    _menuView = new MenuView();
                    _menuView.BindingContext = new MenuViewModel();
                    views.Add(_menuView.ViewName, _menuView);
                }
                return _menuView;
            }
        }
        static GameView _GameView;
        public static GameView GameView {
            get {
                if (_GameView == null) {
                    _GameView = new GameView();
                    _GameView.BindingContext = new GameViewModel();
                    views.Add(_GameView.ViewName, _GameView);
                }
                return _GameView;
            }
        }
        static SettingsView _settingsView;
        public static SettingsView SettingsView {
           get {
               if (_settingsView == null) {
                   _settingsView = new SettingsView();
                   _settingsView.BindingContext = new SettingsViewModel();
                   views.Add(_settingsView.ViewName, _settingsView);
               }
               return _settingsView;
           }
        }
#endregion
    }
}