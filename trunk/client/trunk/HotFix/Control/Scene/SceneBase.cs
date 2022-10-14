﻿using HotFix.Data;
using UnityEngine;
using System.Collections.Generic;
using Framework.ResMgr;

namespace HotFix.Control {

    // 场景抽象基类
    public abstract class SceneBase {

        public SceneData Data { // 场景里的基本数据
            get;
            set;
        }
        public SceneTypeData TypeData { // 分不同的场景类型的附加数据
            get;
            set;
        }

// 序列化的背后,场景也是作成一个预设的,那么它所有的子元件也是从属于一个GameObject之下
        public GameObject GameObject { 
            get;
            set;
        }

        // 家具的集合
        public Dictionary<int, FurnitureBase> furnitures = new Dictionary<int, FurnitureBase>();
        public SceneBase(int type) {
            Data = new SceneData();
            Data.type = type;
            Data.materialDatas = new Dictionary<string, MaterialData>();
            Data.furnitureDatas = new Dictionary<int, FurnitureData>();
            TypeData = TypeDataManager.GetSceneTypeData(Data.type);
        }
        public SceneBase(SceneData data) {
            Data = data;
            TypeData = TypeDataManager.GetSceneTypeData(Data.type);
        }
        public void LoadSceneGameObject() {
            ResourceConstant.Loader.LoadCloneAsyn(TypeData.bundleName, TypeData.assetName, (go) => {
                GameObject = go;
                SetGameObjectName();
                SceneManager.Instance.CurrentScene = this;
                Initialize();
            }, EAssetBundleUnloadLevel.ChangeSceneOver);
        }
        protected abstract void SetGameObjectName();
        protected abstract void Initialize();
        public virtual void Dispose() {
            ResourceConstant.Loader.Unload(TypeData.bundleName, true);
        }
    }
}
