using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIExtension {
    // 延迟加载组件
    public class DelayLoadGrid : MonoBehaviour {
        public ScrollRect scroll;
        ScrollViewEvent scrollViewEvent;
        private RectTransform scrollRectTransform;
        // 内容节点
        public RectTransform ContentRoot {
            get {
                return scroll.content;
            }
        }
        // Item预制
        public GameObject itemPrefab;
        // GridItem集合
        List<GridItem> items = new List<GridItem>();
        public float width = 50;
        public float height = 50;
        public float startX = 10;
        public float startY = 5;
        float x = 0;
        float y = 0;
        public float xInterval = 4;
        public float yInterval = 4;
        public int cellCount = 2;
        public bool constantHeight = true;
        void Awake() {
            scrollRectTransform = scroll.transform as RectTransform;
            scroll.onValueChanged.AddListener(OnScrollChanged);
            scrollViewEvent = scroll.GetComponent<ScrollViewEvent>();
        }
        public void InitializeItems(List<GridItem> _items) {
            foreach (var item in items) {
                item.Dispose();
            }
            items = _items;
            InitializeItems();
        }
        public void InitializeItems() {
            if (scroll.horizontal) {
                int count = items.Count;
                int row = Mathf.CeilToInt((float)count / cellCount);
                y = startY;
                x = -width + startX - xInterval;
                int xIndex = 0;
                ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startX + row * (width + xInterval));
                ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startY + cellCount * (yInterval + height));
                int thisRow = 0;
                for (int i = 0; i < count; i++) {
                    if (i % cellCount == 0) {
                        y = startY;
                        x += width + xInterval;
                        xIndex = 0;
                        thisRow++;
                    } else {
                        xIndex++;
                        y += height + yInterval;
                    }
                    var p = new Vector2(x, y);
                    items[i].Initialize(this, p);
                    if (string.IsNullOrEmpty(items[i].Name)) {
                        items[i].Name = i + " " + thisRow;
                    }
                }
            } else {
                if (constantHeight) {
                    int count = items.Count;
                    int row = Mathf.CeilToInt((float)count / cellCount);
                    y = -height + startY;
                    x = startX;
                    ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startX + cellCount * (width + xInterval));
                    ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startY + row * (yInterval + height));
                    int thisRow = 0;
                    for (int i = 0; i < count; i++) {
                        if (i % cellCount == 0) {
                            y += height + yInterval;
                            x = startX;
                            thisRow++;
                        } else {
                            x += width + xInterval;
                        }
                        var p = new Vector2(x, y);
                        items[i].Initialize(this, p);
                        if (string.IsNullOrEmpty(items[i].Name)) {
                            items[i].Name = i + " " + thisRow;
                        }
                    }
                } else {
                    if (cellCount == 2) {
                        int count = items.Count;
                        float cellOneY = startY;
                        float cellTwoY = startY;
                        float x = startX;
                        Vector2 pos;
                        //One
                        if (count > 0) {
                            x = startX;
                            pos = new Vector2(x, cellOneY);
                            items[0].Initialize(this, pos);
                            cellOneY += items[0].Height;
                            //Two
                            if (count > 1) {
                                x = startX + width + xInterval;
                                pos = new Vector2(x, cellTwoY);
                                items[1].Initialize(this, pos);
                                cellTwoY += items[1].Height;
                                for (int i = 2; i < count; i++) {
                                    if (cellOneY <= cellTwoY) {
                                        cellOneY += yInterval;
                                        x = startX;
                                        pos = new Vector2(x, cellOneY);
                                        cellOneY += items[i].Height;
                                    }
                                    else {
                                        cellTwoY += yInterval;
                                        x = startX + width + xInterval;
                                        pos = new Vector2(x, cellTwoY);
                                        cellTwoY += items[i].Height;
                                    }
                                    items[i].Initialize(this, pos);
                                }
                            }
                            ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startX + 2 * (width + xInterval));
                            if (cellOneY > cellTwoY) {
                                ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellOneY);
                            }
                            else {
                                ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellTwoY);
                            }
                        }
                    } else {
                        Debug.LogError("CellCount Only == 2");
                    }
                }
            }
            CheckShow();
        }
        public GridItem AddItem() {
            GridItem gi = new GridItem();
            items.Add(gi);
            return gi;
        }
        public void RemoveItem(GridItem gi) {
            items.Remove(gi);
            gi.Dispose();
            InitializeItems();
        }
        void OnScrollChanged(Vector2 V) {
            CheckShow();
        }
        void CheckShow() {
            foreach (var item in items) {
                if (NeedShow(item.Position)) {
                    item.Show(ContentRoot, itemPrefab);
                } else {
                    item.Hide();
                }
            }
        }
        bool NeedShow(Vector2 position) {
            if (scrollRectTransform == null) {
                return false;
            }
            if (!scroll.horizontal && position.y > ContentRoot.localPosition.y - height && position.y < ContentRoot.localPosition.y + scrollRectTransform.rect.height + height) {
                return true;
            }
            if (scroll.horizontal && position.x > startX - ContentRoot.localPosition.x - width && position.x < -ContentRoot.localPosition.x + scrollRectTransform.rect.width + width) {
                return true;
            }
            return false;
        }
    }
}