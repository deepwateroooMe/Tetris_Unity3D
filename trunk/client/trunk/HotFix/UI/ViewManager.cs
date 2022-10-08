using UnityEngine;
using System.Collections.Generic;
using Framework.MVVM;
using Framework.Util;
using System.Collections;
using HotFix.UI.View.MidMenuView;
using UnityEngine.EventSystems;
using HotFix.UI.View.SettingsView;
using HotFix.UI.View.TestView;

namespace HotFix.UI {

    // 它说，我是一个静态管理类，我要把每个需要热更新的视图都持有一个静态引用;
    // 当需要实例化的时候，要么返回现持非空的视图，要么就新实例化一个该类型的视图
    // 同样，与每个视图相绑定的是，MVVM设计模式的ViewModel，通过UnityGuiView : IView<ViewModelBase>抽象基类的继承实体类
    // 热更新包里，看得是平淡无奇的ViewModel，但是因为继承自CrossBindingAdapter的子类的ViewModelAdapter，使用View　怎么样呢？跨域识别，相互认得即可
    // 面板管理器:  看上面，是完全可以用framework里定义的适配什么的呀
    public static class ViewManager {
        // 这里固化适配为两维三维都可以
        public static Canvas UI2DRoot; // 这些也是
        public static Canvas UI3DRoot;
        
        public static RectTransform transfom;
        public static Dictionary<string, UnityGuiView> views = new Dictionary<string, UnityGuiView>();
        // public static TMP_FontAsset pingfangregularFont; // 把字体的部分都先简单地略过
        // public static TMP_FontAsset pingfangmediumFont;
        public static Font regularFont;
        public static Font mediumFont;

        public static void InitializeStartUI() {
            // LoadBaseAsset(); // 它说先加载场景以及视图里可能会用到的必要的字体，先。。。
            CreateBaseUI();
        }
        // static void LoadBaseAsset() {
        //     LoadFont(); // 这里说，就先加载一些字体
        // }
        // static void LoadFont() {　// 他们很喜欢苹果平方体
        //     pingfangregularFont = ResourceHelper.LoadTMP_FontAsset("ui/font/pingfangregular", "pic/ngfangregular", EAssetBundleUnloadLevel.Never);
        //     pingfangregularFont.material.shader = Shader.Find("TextMeshPro/Mobile/Distance Field");
        //     pingfangmediumFont = ResourceHelper.LoadTMP_FontAsset("ui/font/pingfangmedium", "pingfangmedium", EAssetBundleUnloadLevel.Never);
        //     pingfangmediumFont.material.shader = Shader.Find("TextMeshPro/Mobile/Distance Field");
        //     regularFont = ResourceHelper.LoadFont("ui/font/regular", "regular", EAssetBundleUnloadLevel.Never);
        //     mediumFont = ResourceHelper.LoadFont("ui/font/medium", "medium", EAssetBundleUnloadLevel.Never);
        // }
// 仔细看这个方法：不是从热更新程序集里加载出unity里运行所需要的东西了吗？    
        static void CreateBaseUI() {
            ResourceHelper.LoadCloneAsyn("ui/ui2droot", "UI2DRoot", // 这里是有预设的包，读出资源就可以加载
                                         (go) => {
                                             go.name = "UI2DRoot";
                                             GameObject.DontDestroyOnLoad(go);
                                             UI2DRoot = go.GetComponent<Canvas>();
                                             var viewRoot = new GameObject("ViewRoot"); // 实例化一个新空控件当作是视图层的根节点
                                             viewRoot.layer = LayerMask.NameToLayer("UI");　
                                                                                              var viewRect = viewRoot.AddComponent<RectTransform>();
                                             viewRect.SetParent(UI2DRoot.transform, false);
                                             viewRect.sizeDelta = new Vector2(0, 0);
                                             viewRect.anchorMin = Vector2.zero;
                                             viewRect.anchorMax = Vector2.one;
                                             viewRect.pivot = new Vector2(0.5f, 0.5f);
                                             //poolRoot = new GameObject("PoolRoot").transform;
                                             //poolRoot.SetParent(UI2DRoot.transform, false);
                                             //poolRoot.gameObject.SetActive(false);
                                             ShowStartPanel();
                                         }, EAssetBundleUnloadLevel.Never);
        }
// 遍历当前视图管理器里所管理的所有的视图，凡是不是所指定特定视图的，一律隐藏起来（应该只是不让用户看见，它还在那里，在幕后的某个角落乘凉）
        public static void CloseOtherRootViews(string viewName) {
            foreach (var view in views.Values) 
                if (view.ViewName != viewName && view.IsRoot) 
                    view.Hide();
        }
    
// 这里应该是一个导航视图吧，猜测（不是视图，是panel　？）昨天晚上少眠，今天状态相对较差，期待明天会比较好
// 明天这些部分，今天所有有疑问的部分都再仔细地看一下    
        static void ShowStartPanel() {
            MenuView.Reveal();
        }
#region Util
#endregion

// #region GridItemPool
//         public static Transform poolRoot;
// // 这里所彩的数据结构栈：应该是与特定的应用特性相关的，能够保证后进先出和保证效率的
//         public static Dictionary<string, Stack<GameObject>> gridItemPool = new Dictionary<string, Stack<GameObject>>();
//         public static GameObject GetGridItemFromPool(string name) {
//             if (gridItemPool.ContainsKey(name) && gridItemPool[name].Count > 0) {
//                 var gridItem = gridItemPool[name].Pop();
//                 return gridItem;
//             }
//             return null;
//         }
//         public static void CacheGridItemToPool(string name, GameObject go) {
//             if (!gridItemPool.ContainsKey(name)) 
//                 gridItemPool[name] = new Stack<GameObject>();
//             Stack<GameObject> goList = gridItemPool[name];
//             go.transform.SetParent(poolRoot, false);
//             goList.Push(go);
//         }
// #endregion
    
// 视图里的小物件管理，是热更新起始时重要的三个步骤之二:　可是仍然感觉他们只是很不起眼的一两个小物件，根本不值一提呀
// 这部分的细节暂时跳过，等改天实现自己游戏热更新需要参考的时候还可以修补上    

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
        // static MenuView _menuView;
        // public static MenuView MenuView {
        //     get {
        //         if (_menuView == null) {
        //             _menuView = new MenuView();
        //             _menuView.BindingContext = new MenuViewModel();
        //             views.Add(_menuView.ViewName, _menuView);
        //         }
        //         return _menuView;
        //     }
        // }
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
        static EducaModesView _educamodesView;
        public static EducaModesView EducaModesView {
            get {
                if (_educamodesView == null) {
                    _educamodesView = new EducaModesView();
                    _educamodesView.BindingContext = new EducaModesViewModel();
                    views.Add(_educamodesView.ViewName, _educamodesView);
                }
                return _educamodesView;
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
        static MidMenuView _midmenuView;
        public static MidMenuView MidMenuView {
            get {
                if (_midmenuView == null) {
                    _midmenuView = new MidMenuView();
                    _midmenuView.BindingContext = new MidMenuViewModel();
                    views.Add(_midmenuView.ViewName, _midmenuView);
                }
                return _midmenuView;
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
        static TestView _testView;
        public static TestView TestView {
            get {
                if (_testView == null) {
                    _testView = new TestView();
                    _testView.BindingContext = new TestViewModel();
                    views.Add(_testView.ViewName, _testView);
                }
                return _testView;
            }
        }
#endregion
    }
}
