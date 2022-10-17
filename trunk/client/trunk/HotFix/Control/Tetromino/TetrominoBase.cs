using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d;
using deepwaterooo.tetris3d.Data;
using Framework.ResMgr;
using HotFix.Data;
using HotFix.Data.TypedData;
using UnityEngine;

namespace HotFix.Control {

    // 因为改写为根据配置文件来,不再把脚本添加到预设中,所以还是抽象出公用逻辑的部分在这个BASE里
    // 方块砖基类
    public abstract class TetrominoBase {
        public TetrominoData Data {
            get;
            set;
        }
        public TetrominoTypeData TypeData {
            get;
            set;
        }
        public GameObject GameObject {
            get;
            set;
        }
        public TetrominoBase(long type) {
            Data = new TetrominoData();
            InitializeNewData(type);
            TypeData = TypeDataManager.GetTetrominoTypeData(Data.type);
            InitializeGameObject();
        }
        public TetrominoBase(TetrominoData data) {
            Data = data;
            TypeData = TypeDataManager.GetTetrominoTypeData(Data.type);
        }
        protected virtual void InitializeNewData(long type) {
            Data.instanceID = SceneManager.Instance.currentSelectGameObjectInstanceID++;
            Data.type = type;
            Data.positionX = 0.0f;
            Data.positionY = 0.0f;
            Data.positionZ = 0.0f;
            Data.rotationX = 0.0f;
            Data.rotationY = 0.0f;
            Data.rotationZ = 0.0f;
            Data.scaleX = 1.0f;
            Data.scaleY = 1.0f;
            Data.scaleZ = 1.0f;
        }
        void InitializeGameObject() {
            ResourceConstant.Loader.LoadCloneAsyn(TypeData.bundleName, TypeData.assetName, (go) => {
                GameObject = go;
                GameObject.name = TypeData.name + "_" + Data.instanceID;
                // GameObject.transform.SetParent(parent);
                Initialize();
            }, EAssetBundleUnloadLevel.ChangeSceneOver);
        }
        public void LoadTetrominoGameObject(Transform parent) {
            ResourceConstant.Loader.LoadCloneAsyn(TypeData.bundleName, TypeData.assetName, (go) => {
                GameObject = go;
                GameObject.name = TypeData.name + "_" + Data.instanceID;
                GameObject.transform.SetParent(parent);
                Initialize();
            }, EAssetBundleUnloadLevel.ChangeSceneOver);
            // GameObject = ResourceConstant.Loader.LoadClone(TypeData.bundleName, TypeData.assetName, EAssetBundleUnloadLevel.ChangeSceneOver);
            // GameObject.name = TypeData.name + "_" + Data.instanceID;
            // GameObject.transform.SetParent(parent);
            // Initialize();
        }
        protected void Initialize() {
            InitializeDataWrap();
            InitializeTransform();
            InitializeOther();
        }
        void InitializeDataWrap() {
            DataWrap dataWrap = GameObject.GetOrAddComponent<DataWrap>();
            dataWrap.gameObjectType = ESelectGameObjectType.Tetromino;
            dataWrap.instanceID = Data.instanceID;
        }
        void InitializeTransform() {
            GameObject.transform.localPosition = new Vector3(Data.positionX, Data.positionY, Data.positionZ);
            GameObject.transform.localEulerAngles = new Vector3(Data.rotationX, Data.rotationY, Data.positionZ);
            GameObject.transform.localScale = new Vector3(Data.scaleX, Data.scaleY, Data.scaleZ);
        }
        protected abstract void InitializeOther();
        public virtual void Dispose() {
            ResourceConstant.Loader.Unload(TypeData.bundleName, true);
        }
    }
}
