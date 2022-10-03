using HotFix.Data;
using UnityEngine;
using Framework.ResMgr;

namespace HotFix.Control {

    // 场景抽象基类: 
    public abstract class SceneBase {

        public SceneData Data {
            get;
            set;
        }
        public SceneTypeData TypeData {
            get;
            set;
        }
        public GameObject GameObject {
            get;
            set;
        }

        // 可以有同类物体的集合管理,集合数据结构map 等

        public SceneBase(int type) {
            Data = new SceneData();
            Data.type = type;
// .....
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
