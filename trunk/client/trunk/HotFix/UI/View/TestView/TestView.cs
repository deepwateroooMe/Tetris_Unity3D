using Framework.MVVM;
using UnityEngine;
using HotFix.Control;
using System.Collections.Generic;
using HotFix.Data;

namespace HotFix.UI.View.TestView {

// 热更新里拿取一个样例热更新的视图: 这种视图热更新在网上应该能够成堆地找到
    public class TestView : UnityGuiView {
        // Button buttonOne;
        // Button buttonTwo;
        // Button buttonThree;
        // GameObject pullDownRefresh;
        // TextMeshProUGUI refreshText;
        // DelayLoadGrid findItemGridRoot;
        // ScrollViewEvent scrollViewEvent;
        // List<GridItem> findGridItems = new List<GridItem>();
        // List<FindData> findDatas;

        // protected override void OnInitialize() {
        //     base.OnInitialize();　//　对基类抽象方法的继承实现　
        //                             buttonOne = GameObject.FindChildByName("ButtonOne").GetComponent<Button>();
        //     buttonOne.onClick.AddListener(OnClickButtonOne);
        //     buttonTwo = GameObject.FindChildByName("ButtonTwo").GetComponent<Button>();
        //     buttonTwo.onClick.AddListener(OnClickButtonTwo);
        //     buttonThree = GameObject.FindChildByName("ButtonThree").GetComponent<Button>();
        //     buttonThree.onClick.AddListener(OnClickButtonThree);
        //     findItemGridRoot = GameObject.FindChildByName("FindItemGridRoot").GetComponent<DelayLoadGrid>();
        //     scrollViewEvent = GameObject.FindChildByName("FindItemGridScrollView").GetComponent<ScrollViewEvent>();
        //     scrollViewEvent.onBeginDrag = OnBeginDrag;
        //     scrollViewEvent.onDraging = OnDraging;
        //     scrollViewEvent.onEndDrag = OnEndDrag;
        //     pullDownRefresh = GameObject.FindChildByName("PulldownRefresh");
        //     refreshText = pullDownRefresh.FindChildByName("Text").GetComponent<TextMeshProUGUI>();
        //     pullDownRefresh.SetActive(false);
        //     InitializeDatas(); // <<<<<<<<<<<<<<<<<<<< 
        // }
        // public override void OnAppear() {
        //     base.OnAppear();
        //     CloseOtherRootView = CloseOtherRootViews;
        // }
        // void CloseOtherRootViews() {
        //     ViewManager.CloseOtherRootViews(ViewName);
        // }
        // void InitializeDatas() {
        //     findDatas = new List<FindData>();
        //     for (int i = 0; i < 10000; i++) {
        //         FindData findData;
        //         if (i % 7 == 0) {
        //             findData = new FindData(i, i.ToString(), 250);
        //         } else if (i % 7 == 1) {
        //             findData = new FindData(i, i.ToString(), 300);
        //         } else if (i % 7 == 2) {
        //             findData = new FindData(i, i.ToString(), 220);
        //         } else if (i % 7 == 3) {
        //             findData = new FindData(i, i.ToString(), 180);
        //         } else if (i % 7 == 4) {
        //             findData = new FindData(i, i.ToString(), 280);
        //         } else if (i % 7 == 5) {
        //             findData = new FindData(i, i.ToString(), 320);
        //         } else {
        //             findData = new FindData(i, i.ToString());
        //         }
        //         findDatas.Add(findData);
        //     }
        // }
        // // Enter2D
        // void OnClickButtonOne() {
        //     TestCreateCustomEditScene.Instance.CreateCustomEditScene();
        // }
        // void OnClickButtonTwo() {
        //     SceneManager.Instance.LoadSpaceShowScene("scene/config/spaceshowscenedata/spaceshow_10001", "SpaceShow_10001");
        // }
        // void OnClickButtonThree() {
        //     findItemGridRoot.gameObject.DestoryImmediateAllChildren();
        //     findGridItems.Clear();
        //     if (findDatas != null && findDatas.Count > 0) {
        //         foreach (var findData in findDatas) {
        //             GridItem gridItem = new GridItem();
        //             gridItem.Name = "FindItemPrefab_" + findData.id;
        //             findGridItems.Add(gridItem);
        //             FindItemTemp findItem = new FindItemTemp(gridItem, findData);
        //         }
        //     }
        //     findItemGridRoot.InitializeItems(findGridItems);
        // }
        // void OnBeginDrag() {
        //     Debug.Log("OnBeginDrag");
        // }
        // void OnDraging() {
        //     if (findItemGridRoot.GetComponent<RectTransform>().anchoredPosition.y < 0f) {
        //         pullDownRefresh.SetActive(true);
        //         refreshText.text = "刷新...";
        //     } else {
        //         pullDownRefresh.SetActive(false);
        //     }
        // }
        //void OnEndDrag() {
        //    if (findItemGridRoot.GetComponent<RectTransform>().anchoredPosition.y < 0f) {
        //        refreshText.text = "刷新完";
        //    }
        //}

// 某些属性，对抽象基类的抽象方法的实现与覆写    
        public override string BundleName {
            get {
                return "ui/view/testview";
            }
        }
        public override string AssetName {
            get {
                return "TestView";
            }
        }
        public override string ViewName {
            get {
                return "TestView";
            }
        }
        public override string ViewModelTypeName {
            get {
                return typeof(TestViewModel).FullName;
            }
        }
        public TestViewModel ViewModel {
            get {
                return (TestViewModel)BindingContext;
            }
        }
        public override bool IsRoot {
            get {
                return true;
            }
        }
    }
}
