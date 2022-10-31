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

    // 它说，我是一个静态管理类，我要把每个需要热更新的视图都持有一个静态引用;
    // 当需要实例化的时候，要么返回现持非空的视图，要么就新实例化一个该类型的视图
    // 同样，与每个视图相绑定的是，MVVM设计模式的ViewModel，通过UnityGuiView : IView<ViewModelBase>抽象基类的继承实体类
    // 热更新包里，看得是平淡无奇的ViewModel，但是因为继承自CrossBindingAdapter的子类的ViewModelAdapter，使用View　怎么样呢？跨域识别，相互认得即可
    // 面板管理器:  看上面，是完全可以用framework里定义的适配什么的呀
    public static class ViewManager {
        private const string TAG = "ViewManager"; 

        // 这里固化适配为两维三维都可以
        public static Canvas UI2DRoot; 
        public static Canvas UI3DRoot;
        
        public static RectTransform transfom;
        public static Dictionary<string, UnityGuiView> views = new Dictionary<string, UnityGuiView>();
        public static Font regularFont;
        public static Font mediumFont;

        public static void InitializeStartUI() {
            CreateBaseUI();
        }

// 这些,等项目再成熟一点儿,可以都清理掉,或是整理得更有序一些        
        public static Transform eventRoot; // 固定的视图层面资源池根节点
        public static Transform audioRoot; // 固定的视图层面资源池根节点

        static void CreateBaseUI() {
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/ui2droot",
                    "UI2DRoot", // 这里是有预设的包，读出资源就可以加载
                    (go) => {
                        go.name = "UI2DRoot";
                        GameObject.DontDestroyOnLoad(go); // 以此为父节点的所有子节点都不会被销毁,包括各种管理类
// 游戏运行时,不知道什么原因,会呈现一些不确定性:比如旋转某些角度等,想要把它们摆定
                        // go.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
                        CoroutineHelper.StartCoroutine(GetRectSize(go.GetComponent<RectTransform>()));

                        UI2DRoot = go.GetComponent<Canvas>();
                        var viewRoot = new GameObject("ViewRoot"); // 实例化一个新空控件当作是视图层的根节点
                        viewRoot.layer = LayerMask.NameToLayer("UI");
// Managers: slightly higher level for receiving game events
                        var managersRoot = new GameObject("Managers");
                        managersRoot.transform.SetParent(UI2DRoot.transform);
// EventManager: 这里是已经创建了单例实例(放在这里的问题是,会丢掉一些事件,它需要在更早的时间启动,转到ViewManager里去)           
                        EventManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);
                        AudioManager.Instance.gameObject.transform.SetParent(managersRoot.transform, false);

                        var viewRect = viewRoot.AddComponent<RectTransform>();
                        viewRect.SetParent(UI2DRoot.transform, false);
                        viewRect.sizeDelta = new Vector2(0, 0);
                        viewRect.anchorMin = Vector2.zero;
                        viewRect.anchorMax = Vector2.one; // ori
                        viewRect.pivot = new Vector2(0.5f, 0.5f);
                        ShowStartPanel(); // <<<<<<<<<<<<<<<<<<<< 
                    }, EAssetBundleUnloadLevel.Never);
// 不知道该如何调整两个画布：MoveCanvas RotateCanvas, 暂时把它们直接放在世界坐标系下
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/view/btnscanvasview",
                    "BtnsCanvasView", // 这里是有预设的包，读出资源就可以加载
                    (go) => {
                        go.name = "BtnsCanvasView";
                        GameObject.DontDestroyOnLoad(go); // 以此为父节点的所有子节点都不会被销毁,包括各种管理类
                        moveCanvas = go.FindChildByName("moveCanvas");
                        rotateCanvas = go.FindChildByName("rotateCanvas");
                        ComponentHelper.AddMoveCanvasComponent(moveCanvas);
                        moveCanvas.SetActive(false); 
                        ComponentHelper.AddRotateCanvasComponent(rotateCanvas);
                        rotateCanvas.SetActive(false);
// 我先试图在这里把预设都先整理一下?
                        minosDic = new Dictionary<string, GameObject>();
                        pool = new Dictionary<string, Stack<GameObject>>();
                        // PoolManager.Initialize();
                        tetrosPool = go.FindChildByName("tetrosPool");
                        tetroParent = go.FindChildByName("TetrominosContainer");
                        GameObject parent = go.FindChildByName("Prefabs");
                        foreach (Transform child in parent.transform) { // go ==> parent 这个破BUG让我找了好久.....只仅仅是实现的时候手误.....
                            // PoolManager.fillPool(child);
                            string name = child.gameObject.name;
                            Debug.Log(TAG + " name: " + name);
                            // if (child.gameObject.name.StartsWith("mino"))
                            //     type = child.GetComponent<MinoType>();
                            // else type = child.GetComponent<TetrominoType>();
                            minosDic.Add(name, child.gameObject);
                            Stack<GameObject> stack = new Stack<GameObject>();
// 这里我写的是手动生成对象池里的缓存对象:并在这里根据不同的类型添加相应的脚本
                            bool isTetro = name.StartsWith("Tetromino");
                            bool isGhost = name.StartsWith("shadow");
                            // Debug.Log(TAG + " isTetro: " + isTetro);
                            for (int i = 0; i < 10; i++) {
                                GameObject tmp = GameObject.Instantiate(child.gameObject);
                                tmp.name = name;
// 被劫持了的新的GetComponent()方法: 可能并不支持其.enabled 的属性修改?
                                if (isTetro) {
                                    ComponentHelper.AddTetroComponent(tmp);
                                    Tetromino tetromino = ComponentHelper.GetTetroComponent(tmp);
                                    ComponentHelper.GetTetroComponent(tmp).enabled = false;
                                } else if (isGhost) {
                                    ComponentHelper.AddGhostComponent(tmp);
                                    ComponentHelper.GetGhostComponent(tmp).enabled = false;
                                }
                                tmp.transform.SetParent(tetrosPool.transform, true); // 把它们放在一个容器下面,免得弄得游戏界面乱七八糟的
                                tmp.SetActive(false);
                                stack.Push(tmp);
                            }
                            pool.Add(name, stack);
                        }
                        parent.SetActive(false);
                    }, EAssetBundleUnloadLevel.Never);
        }

// 两块不同按钮的画布,两架相机,以及游戏过程中生成的所有的方块砖都位于这个视图下        
#region BtnsCanvasView

        // public static GameObject eventManager = null;
        public static GameObject moveCanvas = null;
        public static GameObject rotateCanvas = null;
        public static GameObject nextTetromino = null; // 放这里,主要是方便GameViewModel和Tetromino GhostTetromino来拿到reference
        public static GameObject ghostTetromino = null;
        public static Dictionary<string, GameObject> minosDic = null;
        public static Dictionary<string, Stack<GameObject>> pool = null;
        public static GameObject tetrosPool = null;
        public static GameObject tetroParent = null;
        private static Vector3 defaultPos = new Vector3(-100, -100, -100); // 不同类型的起始位置不一样(可否设置在预设里呢>??)
        
        // public Material [] materials; // [red, green, blue, yellow]
        // public Material [] colors;
        public static GameObject GetFromPool(string type, Vector3 pos, Quaternion rotation, Vector3 localScale) {
            Debug.Log(TAG + " GetFromPool");
            Debug.Log(TAG + " type: " + type); 
            Stack<GameObject> st = pool[type];
            GameObject objInstance = null;
            if (st.Count > 0) 
                objInstance = st.Pop();
            else 
                objInstance = GameObject.Instantiate(ViewManager.minosDic[type]); 
            objInstance.transform.position = pos;
            objInstance.transform.rotation = rotation;
            if (localScale == null)
                objInstance.transform.localScale = Vector3.one;
            else
                objInstance.transform.localScale = (Vector3)localScale;
            objInstance.SetActive(true);
            objInstance.transform.SetParent(ViewManager.tetroParent.transform, false); // default set here 吧
// 这里的逻辑不对,只在非启蒙模式下才立即激活,逻辑还是放游戏视图里去处理            
            // if (type.StartsWith("Tetromino"))
            //     ComponentHelper.GetTetroComponent(objInstance).enabled = true;
            return objInstance;
        }
    
        public static void ReturnToPool(GameObject gameObject, string type) {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
                if (pool[type].Count < 10) {
                    gameObject.transform.position = defaultPos;
                    pool[type].Push(gameObject);
                } else GameObject.DestroyImmediate(gameObject);
            } 
        }

        public static GameObject GetFromPool(string type) {
            GameObject objInstance = null;
            if (pool.ContainsKey(type) && pool[type].Count > 0) {
                objInstance = pool[type].Pop();
                objInstance.SetActive(true);
            } else  // tmp commented out
                objInstance = GameObject.Instantiate(minosDic[type]);
            return objInstance;
        }
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

// 这里应该是一个导航视图吧，猜测（不是视图，是panel　？）昨天晚上少眠，今天状态相对较差，期待明天会比较好
// 明天这些部分，今天所有有疑问的部分都再仔细地看一下    
        static void ShowStartPanel() {
            MenuView.Reveal();
            // ViewManager.DownRootView.Reveal(); // 考虑这里是否需要将游戏模式转化为gameMode index或是int值?
        }
#region Util
#endregion

// 此区域目前只作参考 
// 这个过于简单的资源池 理解参考:在视图层面的某个视图中,对视图中可能会出现的各元素使用了资源池.
// 希望方块砖游戏中要用到的资源池能够设计得再好一点儿,不同类型的缓存资源需要有缓存上限
// #region GridItemPool
        public static Transform poolRoot; // 固定的视图层面资源池根节点
// // 资源池：每种不同类型使用一个栈来保存缓存的该类型资源,FIFO
// // 这里就当是一个资源缓存池,每种类型的资源使用栈来保存,以最大限度地优化进出栈内存性能?
//         public static Dictionary<string, Stack<GameObject>> gridItemPool = new Dictionary<string, Stack<GameObject>>(); // 栈
//         public static GameObject GetGridItemFromPool(string name) {
//             if (gridItemPool.ContainsKey(name) && gridItemPool[name].Count > 0) {
//                 var gridItem = gridItemPool[name].Pop();
//                 return gridItem;
//             }
//             return null; // 如果没有返回空,得保证需要的时候,资源池拿不到,也得从加工厂里加工一个出来
//         }
//         public static void CacheGridItemToPool(string name, GameObject go) { // 将某种类型的元件缓存到资源池中去
//             if (!gridItemPool.ContainsKey(name)) 
//                 gridItemPool[name] = new Stack<GameObject>();
//             Stack<GameObject> goList = gridItemPool[name];
//             go.transform.SetParent(poolRoot, false); // 所有需要缓存的资源对象均以此poolRoot为根节点
//             goList.Push(go);
//         }
// #endregion

#region Other
        static bool isOverUI = false;
        static bool isCheckedOverUI = false;
        static List<RaycastResult> raycastResults = new List<RaycastResult>();
        // 是否触摸到UI控件
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

// 热更新的视图，远远不止这两个，但是留这两个已经够参考了，其它删除了
#region Views
        static BtnsCanvasView _btnscanvasView;
        public static BtnsCanvasView BtnsCanvasView {
            get {
                if (_btnscanvasView == null) {
                    _btnscanvasView = new BtnsCanvasView();
                    _btnscanvasView.BindingContext = new BtnsCanvasViewModel();
                    views.Add(_btnscanvasView.ViewName, _btnscanvasView);
                }
                return _btnscanvasView;
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
        static BgnNewContinueView _bgnnewcontinueView;
        public static BgnNewContinueView BgnNewContinueView {
            get {
                if (_bgnnewcontinueView == null) {
                    _bgnnewcontinueView = new BgnNewContinueView();
                    _bgnnewcontinueView.BindingContext = new BgnNewContinueViewModel();
                    views.Add(_bgnnewcontinueView.ViewName, _bgnnewcontinueView);
                }
                return _bgnnewcontinueView;
            }
        }
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
