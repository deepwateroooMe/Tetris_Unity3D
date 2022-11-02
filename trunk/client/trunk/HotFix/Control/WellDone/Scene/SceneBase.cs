using HotFix.Data;
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

        // 方块砖的集合
        //public Dictionary<int, TetrominoBase> tetrominos = new Dictionary<int, TetrominoBase>();

        public SceneBase(int type) {
            Data = new SceneData();
            Data.type = type;
            //Data.tetrominoDatas = new Dictionary<int, TetrominoDataCon>();
            TypeData = TypeDataManager.GetSceneTypeData(Data.type);
        }

        public SceneBase(SceneData data) {
            Data = data;
            TypeData = TypeDataManager.GetSceneTypeData(Data.type);
        }

// 这里也就说明:按场景打包,这样方便场景加载时加载所有接下来场景所必要的所有资源,与卸载的时候释放不必要的资源
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
