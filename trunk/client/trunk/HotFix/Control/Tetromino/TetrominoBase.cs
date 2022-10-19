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
    // 方块砖基类: 想要把方块砖的基本控制逻辑抽象提炼到这个基类里,其它类只是继写
    public abstract class TetrominoBase : MonoBehaviour, IType, IEntity {

// implement interface methods
        private IType tetrominoType; 
        public string type {
            get {
                return tetrominoType.type;
            }
            set {
                tetrominoType.type = value;
            }
        }
        private Vector3 tetroPos;
        public Vector3 pos {
            set {
                tetroPos = pos;
            }
            get {
                return tetroPos;
            }
        }
        private Vector3 tetroRot;
        public Vector3 rot {
            set {
                tetroRot = rot;
            }
            get {
                return tetroRot;
            }
        }
        private Vector3 tetroSca;
        public Vector3 sca {
            set {
                tetroSca = sca;
            }
            get {
                return tetroSca;
            }
        }
// 这是两个还没能重构成功的案例方法, 想要使用命令式驱动?这里需要再想一想        
        public void MoveDelta(Vector3 delta) {     // 使用 _command_
            //     if (delta != Vector3.zero) {
            //         moveCommand = new MoveCommand(this, delta); // this: Tetromino as IEntity
            //         _commandProcessor.ExecuteCommand(moveCommand);
            //     }
        }

        public void RotateDelta(Vector3 delta) {
            // if (delta != Vector3.zero) {
            //     rotateCommand = new RotateCommand(this, delta); // this: Tetromino as IEntity
            //     _commandProcessor.ExecuteCommand(rotateCommand);
            // }
        }

        public TetrominoBase() { }        

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
        public TetrominoBase(string type) {
            Data = new TetrominoData();
            InitializeNewData(type);
            TypeData = TypeDataManager.GetTetrominoTypeData(Data.id);
            InitializeGameObject();
        }
        public TetrominoBase(TetrominoData data) {
            Data = data;
            TypeData = TypeDataManager.GetTetrominoTypeData(Data.id);
        }
        protected virtual void InitializeNewData(string type) {
            Data.id = SceneManager.Instance.currentSelectGameObjectInstanceID++;
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
                GameObject.name = TypeData.name + "_" + Data.id;
                // GameObject.transform.SetParent(parent);
                Initialize();
            }, EAssetBundleUnloadLevel.ChangeSceneOver);
        }
        public void LoadTetrominoGameObject(Transform parent) {
            ResourceConstant.Loader.LoadCloneAsyn(TypeData.bundleName, TypeData.assetName, (go) => {
                GameObject = go;
                GameObject.name = TypeData.name + "_" + Data.id;
                GameObject.transform.SetParent(parent);
                Initialize();
            }, EAssetBundleUnloadLevel.ChangeSceneOver);
            // GameObject = ResourceConstant.Loader.LoadClone(TypeData.bundleName, TypeData.assetName, EAssetBundleUnloadLevel.ChangeSceneOver);
            // GameObject.name = TypeData.name + "_" + Data.id;
            // GameObject.transform.SetParent(parent);
            // Initialize();
        }
        protected void Initialize() {
            InitializeDataWrap();
            InitializeTransform();
            //InitializeOther();
        }
        void InitializeDataWrap() {
            DataWrap dataWrap = GameObject.GetOrAddComponent<DataWrap>();
            dataWrap.gameObjectType = ESelectGameObjectType.Tetromino;
            dataWrap.instanceID = (int)Data.id;
        }
        void InitializeTransform() {
            GameObject.transform.localPosition = new Vector3(Data.positionX, Data.positionY, Data.positionZ);
            GameObject.transform.localEulerAngles = new Vector3(Data.rotationX, Data.rotationY, Data.positionZ);
            GameObject.transform.localScale = new Vector3(Data.scaleX, Data.scaleY, Data.scaleZ);
        }

        // protected abstract void InitializeOther();
        
        public virtual void Dispose() {
            ResourceConstant.Loader.Unload(TypeData.bundleName, true);
        }
    }
}
