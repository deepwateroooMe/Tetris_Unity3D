using UnityEngine;
using System.Collections.Generic;
using Framework.MVVM;
using Framework.Util;
using System.Collections;
using HotFix.Control;
using UnityEngine.EventSystems;
using deepwaterooo.tetris3d;

namespace HotFix.UI {

    public static class ViewManager {
        private const string TAG = "ViewManager"; 

        public static Canvas UI2DRoot; 
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
                        CoroutineHelperP.StartCoroutine(GetRectSize(go.GetComponent<RectTransform>()));

                        UI2DRoot = go.GetComponent<Canvas>();
                        var viewRoot = new GameObject("ViewRoot"); 
                        viewRoot.layer = LayerMask.NameToLayer("UI");

                        var managersRoot = new GameObject("Managers");
                        managersRoot.transform.SetParent(UI2DRoot.transform);
                        EventManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);
                        AudioManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);
                        ModelMono.Instance.gameObject.transform.SetParent(managersRoot.transform, false);
                        
                        var viewRect = viewRoot.AddComponent<RectTransform>();
                        viewRect.SetParent(UI2DRoot.transform, false);
                        viewRect.sizeDelta = new Vector2(0, 0);
                        viewRect.anchorMin = Vector2.zero;
                        viewRect.anchorMax = Vector2.one; 
                        viewRect.pivot = new Vector2(0.5f, 0.5f);

                        GloData.Instance.gameMode.Value = -1; 
                        ShowStartPanel(); 
                    }, EAssetBundleUnloadLevel.Never);
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/view/btnscanvasview",
                    "BtnsCanvasView", 
                    (go) => {
                        go.name = "BtnsCanvasView";
                        GameObject.DontDestroyOnLoad(go);
                        directionsImg = go.FindChildByName("directions").GetComponent<SpriteRenderer>().sprite;
                        rotationsImg = go.FindChildByName("rotations").GetComponent<SpriteRenderer>().sprite;
                        moveCanvas = go.FindChildByName("moveCanvas");
                        rotateCanvas = go.FindChildByName("rotateCanvas");
                        ComponentHelper.AddMoveCanvasComponent(moveCanvas);
                        moveCanvas.SetActive(false); 
                        ComponentHelper.AddRotateCanvasComponent(rotateCanvas);
                        rotateCanvas.SetActive(false);
// Prefabs and particles pool
                        scoreDic = new Dictionary<string, int>();
                        PoolHelper.Initialize(); // 部分相关逻辑提练到静态帮助类里去定义和完成
                        tetrosPool = go.FindChildByName("tetrosPool");
                        tetroParent = go.FindChildByName("TetrominosContainer");
                        GameObject parent = go.FindChildByName("Prefabs"); 
                        foreach (Transform child in parent.transform) {
                            // if (child.gameObject.name.Equals("TetrominoX")) // 直接将这个移除了
                            //     PoolHelper.minosDic.Add("TetrominoX", child.gameObject);
                            // else
                                PoolHelper.fillPool(child);
							string name = child.gameObject.name;
                            if (name.StartsWith("Tetromino")) {
                                switch (name) {
                                case "TetrominoI":
                                    ViewManager.scoreDic.Add(name, 500);
                                    break;
                                case "TetrominoJ":
                                    ViewManager.scoreDic.Add(name, 500);
                                    break;
                                case "TetrominoL":
                                    ViewManager.scoreDic.Add(name, 600);
                                    break;
                                case "TetrominoO":
                                    ViewManager.scoreDic.Add(name, 500);
                                    break;
                                case "TetrominoS":
                                    ViewManager.scoreDic.Add(name, 600);
                                    break;
                                case "TetrominoT":
                                    ViewManager.scoreDic.Add(name, 600);
                                    break;
                                case "TetrominoZ":
                                    ViewManager.scoreDic.Add(name, 600);
                                    break;
                                case "Tetromino0": 
                                    ViewManager.scoreDic.Add(name, 500);
                                    break;
                                case "TetrominoB": 
                                    ViewManager.scoreDic.Add(name, 700);
                                    break;
                                case "TetrominoC": 
                                    ViewManager.scoreDic.Add(name, 700);
                                    break;
                                case "TetrominoY": 
                                    ViewManager.scoreDic.Add(name, 800);
                                    break;
                                case "TetrominoR": 
                                    ViewManager.scoreDic.Add(name, 800);
                                    break;
                                default:
                                    break;
                                }
                            }
                        }
                        parent.SetActive(false);
// // 手动创建和添加一个TetrominoX: 再换到需要用的方法里去写: 
//                         GameObject tetrox = new GameObject();
//                         tetrox.AddComponent<TetrominoType>();
//                         tetrox.GetComponent<TetrominoType>().type = "TetrominoX";
//                         PoolHelper.minosDic.Add("TetrominoX", tetrox);
// for CHALLENGE MODE:
                        basePlane = go.FindChildByName("basePlane"); 
                        // basePlane.SetActive(false);
                        PoolHelper.LoadChallengeModeMaterials();
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
        public static Sprite directionsImg;
		public static Sprite rotationsImg;
        public static GameObject minoPS;
        public static Dictionary<string, int> scoreDic;
// for CHALLENGE MODE
        public static GameObject basePlane;
        public static Dictionary<int, Material> materials; // 这么写是为了适配原来的源码 
        public static Dictionary<int, Material> colors;
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

        public static void CloseOtherRootViews(string viewName) {
            foreach (var view in views.Values)
                // if (view.ViewName != viewName && view.IsRoot) // 我把这里改写了,因为我目前还没有调控IsRoot视图参数
                    if (view.ViewName != viewName) 
                    view.Hide();
        }

        static void ShowStartPanel() {
            MenuView.Reveal();
        }

#region Other
        static bool isOverUI = false;
        static bool isCheckedOverUI = false;
        static List<RaycastResult> raycastResults = new List<RaycastResult>();
        public static bool IsPointerOverUI() {
            if (isCheckedOverUI) 
                return isOverUI;
            isCheckedOverUI = true;
            isOverUI = false;
            if (EventSystem.current.IsPointerOverGameObject()) {
                isOverUI = true;
            }
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if (raycastResults.Count > 0) 
                isOverUI = true;
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
        static ChallLevelsView _challlevelsView;
        public static ChallLevelsView ChallLevelsView {
            get {
                if (_challlevelsView == null) {
                    _challlevelsView = new ChallLevelsView();
                    _challlevelsView.BindingContext = new ChallLevelsViewModel();
                    views.Add(_challlevelsView.ViewName, _challlevelsView);
                }
                return _challlevelsView;
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