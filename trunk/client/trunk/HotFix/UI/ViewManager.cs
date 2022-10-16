using UnityEngine;
using System.Collections.Generic;
using Framework.MVVM;
using Framework.Util;
using System.Collections;
using HotFix.UI.View.MidMenuView;
using UnityEngine.EventSystems;
using HotFix.UI.View.SettingsView;
using HotFix.UI.View.TestView;
using UnityEngine.UI;

namespace HotFix.UI {

    // 它说，我是一个静态管理类，我要把每个需要热更新的视图都持有一个静态引用;
    // 当需要实例化的时候，要么返回现持非空的视图，要么就新实例化一个该类型的视图
    // 同样，与每个视图相绑定的是，MVVM设计模式的ViewModel，通过UnityGuiView : IView<ViewModelBase>抽象基类的继承实体类
    // 热更新包里，看得是平淡无奇的ViewModel，但是因为继承自CrossBindingAdapter的子类的ViewModelAdapter，使用View　怎么样呢？跨域识别，相互认得即可
    // 面板管理器:  看上面，是完全可以用framework里定义的适配什么的呀
    public static class ViewManager {

        // 这里固化适配为两维三维都可以
        public static Canvas UI2DRoot; 
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
            ResourceHelper
                .LoadCloneAsyn(
                    "ui/ui2droot",
                    "UI2DRoot", // 这里是有预设的包，读出资源就可以加载
                    (go) => {
                        go.name = "UI2DRoot";
                        GameObject.DontDestroyOnLoad(go);
                        // go.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);

                        CoroutineHelper.StartCoroutine(GetRectSize(go.GetComponent<RectTransform>()));
// // 因为我相机调不太好,想把这里的位置重新设置成我先前的位置
                        // RectTransform rt = go.GetComponent<RectTransform>();
                        // rt.rotation = Quaternion.Euler(Vector3.zero);
                        
//                         // // rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
//                         // // rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rt.rect.height);

// // //改变RectTransform的top
// //                         rt.offsetMax = new Vector2(rt.offsetMax.x, top);
// //  //改变RectTransform的bottom
// //                         rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
// //改变RectTransform的width，height
//                         rt.sizeDelta = new Vector2(1920, 3412);
// //改变RectTransform的pos
//                         // rt.anchoredPosition3D = new Vector3(posx,posy,posz);
//                         // rt.anchoredPosition = new Vector2(posx,posy);
//                         rt.anchoredPosition3D = new Vector3(130, 231, 0);
//                         // Vector3 vec = rectTransform.anchoredPosition3D; // 参考的另外的写法
//                         // rectTransform.anchoredPosition3D = new Vector3(vec.x, vec.y, 0);

                        // go.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        // go.GetComponent<CanvasScaler>().referenceResolution = new Vector2 (1920,1200);

                        UI2DRoot = go.GetComponent<Canvas>();
                        var viewRoot = new GameObject("ViewRoot"); // 实例化一个新空控件当作是视图层的根节点
                        viewRoot.layer = LayerMask.NameToLayer("UI");
                        var viewRect = viewRoot.AddComponent<RectTransform>();
                        viewRect.SetParent(UI2DRoot.transform, false); // ori
                        // // viewRect.SetParent(UI2DRoot.transform, true);
                        viewRect.sizeDelta = new Vector2(0, 0);
                        viewRect.anchorMin = Vector2.zero;
                        viewRect.anchorMax = Vector2.one; // ori
                        // viewRect.anchorMax = Vector2.zero;
                        viewRect.pivot = new Vector2(0.5f, 0.5f);

                        //poolRoot = new GameObject("PoolRoot").transform;
                        //poolRoot.SetParent(UI2DRoot.transform, false);
                        //poolRoot.gameObject.SetActive(false);
                        ShowStartPanel();
                    }, EAssetBundleUnloadLevel.Never);
        }

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
            Debug.Log("rt.rotation: x: " + rt.rotation.x + ", y: " + rt.rotation.y + ", z: " + rt.rotation.z);
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
#region GridItemPool
        public static Transform poolRoot; // 固定的视图层面资源池根节点
// 资源池：每种不同类型使用一个栈来保存缓存的该类型资源,FIFO
// 这里就当是一个资源缓存池,每种类型的资源使用栈来保存,以最大限度地优化进出栈内存性能?
        public static Dictionary<string, Stack<GameObject>> gridItemPool = new Dictionary<string, Stack<GameObject>>(); // 栈

        public static GameObject GetGridItemFromPool(string name) {
            if (gridItemPool.ContainsKey(name) && gridItemPool[name].Count > 0) {
                var gridItem = gridItemPool[name].Pop();
                return gridItem;
            }
            return null; // 如果没有返回空,得保证需要的时候,资源池拿不到,也得从加工厂里加工一个出来
        }
        public static void CacheGridItemToPool(string name, GameObject go) { // 将某种类型的元件缓存到资源池中去
            if (!gridItemPool.ContainsKey(name)) 
                gridItemPool[name] = new Stack<GameObject>();
            Stack<GameObject> goList = gridItemPool[name];
            go.transform.SetParent(poolRoot, false); // 所有需要缓存的资源对象均以此poolRoot为根节点
            goList.Push(go);
        }
#endregion
    
// 视图里的小物件管理:　视图中需要可能会用到的运行时需要实例化的小物件(比如各种不同类型的方块砖/阴影砖,粒子系统等)管理
// 与此部分相关联的是UI csharp项目中这些不同类型方块砖(以及不同类型的小MINO,粒子系统)的预设制作,相关数据导入? 与那个项目(UI相关逻辑)的设计与资源打包相关联
// 视图中使用到的运行时需要实例化的小物件包括:
        // 各种不同类型的方块砖(7种)
        // 各种不同类型方块砖的一一对应阴影方块砖(7种)
        // 各种不同类型方块砖的一一对应小MINO(7种)
        // 教育模式下的粒子系统(1种?)
        // 延伸扩展的可以包括游戏中使用到的不同层级的BUTTON: 主页面的三个按钮可以是一种类型;游戏主界面的各个调控按钮(swap, undo, fallfast, pause, toggleBtn)? 但是因为目前已经本身是在热更新程序集,这个思路可能又会抽象出一层更为高层的架构,暂时就只是想想算了,但可以考虑和收集思路
    // 那么就需要使用至少三个?四个字典来管理这些个不同类型的数据,以便实时实例化
#region ItemDatas
        public static void InitializeItemDatas() {
            string planItemJson = ResourceHelper.LoadTextAsset("ui/config/planitem", "planitem", EAssetBundleUnloadLevel.LoadOver).text;
            //Debug.Log("planItemJson: " + planItemJson);
            if (!string.IsNullOrEmpty(planItemJson)) {
                InitializePlanItemData(planItemJson);
            }
            string chapterItemJson = ResourceHelper.LoadTextAsset("ui/config/chapteritem", "chapteritem", EAssetBundleUnloadLevel.LoadOver).text;
            //Debug.Log("chapterItemJson: " + chapterItemJson);
            if (!string.IsNullOrEmpty(chapterItemJson)) {
                InitializeChapterItemData(chapterItemJson);
            }
        }
        static Dictionary<int, PlanItemData> planItemDatas;
        static Dictionary<int, ChapterItemData> chapterItemDatas;
        
        public static Dictionary<int, PlanItemData> GetPlanItemDatas() {
            return planItemDatas;
        }
        public static Dictionary<int, ChapterItemData> GetChapterItemDatas() {
            return chapterItemDatas;
        }
        public static PlanItemData GetPlanItemData(int id) {
            if (planItemDatas.ContainsKey(id)) {
                return planItemDatas[id];
            } else {
                return null;
            }
        }
        public static ChapterItemData GetChapterItemData(int id) {
            if (chapterItemDatas.ContainsKey(id)) {
                return chapterItemDatas[id];
            } else {
                return null;
            }
        }
        static void InitializePlanItemData(string jsonStr) {
            if (jsonStr != null) {
                planItemDatas = new Dictionary<int, PlanItemData>();
                JsonArray jsonArray = JsonSerializer.Deserialize(jsonStr) as JsonArray;
                if (jsonArray != null) {
                    foreach (JsonValue jsonValue in jsonArray) {
                        PlanItemData data = PlanItemData.JsonToObject(jsonValue.ToString());
                        if (!planItemDatas.ContainsKey(data.id)) {
                            planItemDatas.Add(data.id, data);
                        } else {
                            Debug.LogError("planItemDatas contains key: " + data.id);
                        }
                    }
                } else {
                    Debug.LogError("planItemData jsonArray is null");
                }
            }
        }
        static void InitializeChapterItemData(string jsonStr) {
            if (jsonStr != null) {
                chapterItemDatas = new Dictionary<int, ChapterItemData>();
                JsonArray jsonArray = JsonSerializer.Deserialize(jsonStr) as JsonArray;
                if (jsonArray != null) {
                    foreach (JsonValue jsonValue in jsonArray) {
                        ChapterItemData data = ChapterItemData.JsonToObject(jsonValue.ToString());
                        if (!chapterItemDatas.ContainsKey(data.type)) {
                            chapterItemDatas.Add(data.type, data);
                        } else {
                            Debug.LogError("chapterItemDatas contains key: " + data.type);
                        }
                    }
                } else {
                    Debug.LogError("chapterItemData jsonArray is null");
                }
            }
        }
#endregion

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
        static ThreeGridView _threegridView;
        public static ThreeGridView ThreeGridView {
            get {
                if (_threegridView == null) {
                    _threegridView = new ThreeGridView();
                    _threegridView.BindingContext = new ThreeGridViewModel();
                    views.Add(_threegridView.ViewName, _threegridView);
                }
                return _threegridView;
            }
        }
        static FourGridView _fourgridView;
        public static FourGridView FourGridView {
            get {
                if (_fourgridView == null) {
                    _fourgridView = new FourGridView();
                    _fourgridView.BindingContext = new FourGridViewModel();
                    views.Add(_fourgridView.ViewName, _fourgridView);
                }
                return _fourgridView;
            }
        }
        static FiveGridView _fivegridView;
        public static FiveGridView FiveGridView {
            get {
                if (_fivegridView == null) {
                    _fivegridView = new FiveGridView();
                    _fivegridView.BindingContext = new FiveGridViewModel();
                    views.Add(_fivegridView.ViewName, _fivegridView);
                }
                return _fivegridView;
            }
        }
        static ComTetroView _comtetroView;
        public static ComTetroView ComTetroView {
            get {
                if (_comtetroView == null) {
                    _comtetroView = new ComTetroView();
                    _comtetroView.BindingContext = new ComTetroViewModel();
                    views.Add(_comtetroView.ViewName, _comtetroView);
                }
                return _comtetroView;
            }
        }
        static DesView _desView;
        public static DesView DesView {
            get {
                if (_desView == null) {
                    _desView = new DesView();
                    _desView.BindingContext = new DesViewModel();
                    views.Add(_desView.ViewName, _desView);
                }
                return _desView;
            }
        }
        static EduBtnsView _edubtnsView;
        public static EduBtnsView EduBtnsView {
            get {
                if (_edubtnsView == null) {
                    _edubtnsView = new EduBtnsView();
                    _edubtnsView.BindingContext = new EduBtnsViewModel();
                    views.Add(_edubtnsView.ViewName, _edubtnsView);
                }
                return _edubtnsView;
            }
        }
        static EduTetroView _edutetroView;
        public static EduTetroView EduTetroView {
            get {
                if (_edutetroView == null) {
                    _edutetroView = new EduTetroView();
                    _edutetroView.BindingContext = new EduTetroViewModel();
                    views.Add(_edutetroView.ViewName, _edutetroView);
                }
                return _edutetroView;
            }
        }
        static ScoreDataView _scoredataView;
        public static ScoreDataView ScoreDataView {
            get {
                if (_scoredataView == null) {
                    _scoredataView = new ScoreDataView();
                    _scoredataView.BindingContext = new ScoreDataViewModel();
                    views.Add(_scoredataView.ViewName, _scoredataView);
                }
                return _scoredataView;
            }
        }
        static StaticBtnsView _staticbtnsView;
        public static StaticBtnsView StaticBtnsView {
            get {
                if (_staticbtnsView == null) {
                    _staticbtnsView = new StaticBtnsView();
                    _staticbtnsView.BindingContext = new StaticBtnsViewModel();
                    views.Add(_staticbtnsView.ViewName, _staticbtnsView);
                }
                return _staticbtnsView;
            }
        }
        static ToggleBtnView _togglebtnView;
        public static ToggleBtnView ToggleBtnView {
            get {
                if (_togglebtnView == null) {
                    _togglebtnView = new ToggleBtnView();
                    _togglebtnView.BindingContext = new ToggleBtnViewModel();
                    views.Add(_togglebtnView.ViewName, _togglebtnView);
                }
                return _togglebtnView;
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

