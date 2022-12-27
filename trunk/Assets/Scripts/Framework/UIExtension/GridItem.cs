using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.UIExtension {

    // GridItem类: 这里终于想到一个问题,我的大立方体里的5 *5　＊　12个小立方体能够正常显示出来吗,要显示出来需要哪些前提条件呢?
    // 这是二维平面UI视图上的一个封装,但是我想要理解和知道,三维视图上的显示与此有什么不同,如何改进适配到热更新域里也可以正常显示出来?
    // 这觉得这套UI上二维平面的某种容器的向适配并不适合三维大立方体,运行一下不行的时候再想办法解决
    
    public sealed class GridItem {

        private GameObject instance;

        public Vector2 Position {
            get;
            private set;
        }
        public DelayLoadGrid Parent {
            get;
            private set;
        }
        private string _name = string.Empty;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
        public float Height {
            get;
            set;
        }
        // 显示
        public Action<GameObject> OnShow;
        // 隐藏
        public Action OnHide;
        // 销毁
        public Action OnDispose;
        // 创建实例
        public Func<GameObject, GameObject> OnCreteInstance;
        public void Initialize(DelayLoadGrid _parent, Vector2 _position) {
            Position = _position;
            Parent = _parent;
            if (instance != null) {
                RectTransform rect = instance.transform as RectTransform;
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Position.x, Parent.width);
                if (Parent.constantHeight) {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Position.y, Parent.height);
                } else {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Position.y, Height);
                }
            }
        }
        public void Show(Transform root, GameObject prefab) {
            if (instance == null) {
                instance = CreateInstance(prefab);
                instance.transform.SetParent(root, false);
                instance.name = Name;
                RectTransform rect = instance.transform as RectTransform;
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Position.x, Parent.width);
                if (Parent.constantHeight) {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Position.y, Parent.height);
                } else {
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Position.y, Height);
                }
                if (OnShow != null) {
                    OnShow(instance);
                }
            }
        }
        private GameObject CreateInstance(GameObject prefab) {
            if (OnCreteInstance != null) {
                return OnCreteInstance(prefab);
            } else {
                return GameObject.Instantiate(prefab);
            }
        }
        public void Hide() {
            if (instance != null) {
                if (OnHide != null) {
                    OnHide();
                } else {
                    GameObject.Destroy(instance);
                }
                instance = null;
            }
        }
        public void Dispose() {
            if (instance != null) {
                if (OnDispose != null) {
                    OnDispose();
                } else {
                    GameObject.Destroy(instance);
                }
                instance = null;
            }
        }
        public void Reset() {
            if (instance != null) {
                GameObject.Destroy(instance);
            }
            instance = null;
            Position = Vector2.zero;
            Parent = null;
            Name = string.Empty;
            OnShow = null;
            OnHide = null;
            OnDispose = null;
            OnCreteInstance = null;
        }
    }
}