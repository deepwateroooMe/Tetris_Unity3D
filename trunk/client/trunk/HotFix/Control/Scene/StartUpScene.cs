using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.Data;

namespace HotFix.Control {
    // 方块砖游戏场景
    public class StartUpScene : SceneBase {
        // // 家具根节点
        // public Transform FurnitureRoot {
        //     get;
        //     set;
        // }
        public StartUpScene(int type) : base(type) {
        }
        public StartUpScene(SceneData data) : base(data) {
        }
        protected override void SetGameObjectName() {
            GameObject.name = "StartUp_" + TypeData.id;
        }
        protected override void Initialize() {
        //     FurnitureRoot = GameObject.FindChildByName("FurnitureRoot").transform;
        //     InitializeFurnitures();
        }
        // void InitializeFurnitures() {
        //     foreach (var furnitureData in Data.furnitureDatas.Values) {
        //         FurnitureTypeData furnitureTypeData = TypeDataManager.GetFurnitureTypeData(furnitureData.type);
        //         FurnitureBase furnitureBase;
        //         if (furnitureTypeData.placeType == (int)EFurniturePlaceType.Ground) {
        //             furnitureBase = new GroundPlaceFurniture(furnitureData);
        //             furnitureBase.LoadFurnitureGameObject(FurnitureRoot);
        //             if (!furnitures.ContainsKey(furnitureData.instanceID)) {
        //                 furnitures.Add(furnitureData.instanceID, furnitureBase);
        //             } else {
        //                 Debug.LogError("furnitures contains key already! key: " + furnitureData.instanceID);
        //             }
        //         } else if (furnitureTypeData.placeType == (int)EFurniturePlaceType.Wall) {
        //             furnitureBase = new WallPlaceFurniture(furnitureData);
        //             furnitureBase.LoadFurnitureGameObject(FurnitureRoot);
        //             if (!furnitures.ContainsKey(furnitureData.instanceID)) {
        //                 furnitures.Add(furnitureData.instanceID, furnitureBase);
        //             } else {
        //                 Debug.LogError("furnitures contains key already! key: " + furnitureData.instanceID);
        //             }
        //         } else if (furnitureTypeData.placeType == (int)EFurniturePlaceType.Ceiling) {
        //             furnitureBase = new CeilingPlaceFurniture(furnitureData);
        //             furnitureBase.LoadFurnitureGameObject(FurnitureRoot);
        //             if (!furnitures.ContainsKey(furnitureData.instanceID)) {
        //                 furnitures.Add(furnitureData.instanceID, furnitureBase);
        //             } else {
        //                 Debug.LogError("furnitures contains key already! key: " + furnitureData.instanceID);
        //             }
        //         }
        //     }
        // }
        // public override void Dispose() {
        //     base.Dispose();
        //     foreach (var furniture in furnitures.Values) {
        //         furniture.Dispose();
        //     }
        // }
    }
}
