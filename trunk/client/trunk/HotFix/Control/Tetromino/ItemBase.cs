using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotFix.Data;
using UnityEngine;

namespace HotFix.UI {

// 大致的设计思路: 这个序列化,反序列化等
    // GameView/Items: 一个文件列出所有的 IType 一个.json文件 
    // ViewManager.cs: parse 出三四种不同的类型，用三四个不同的字典来管理
    // 过程中将资源池整合到ViewManager中去
    // 定义ItemControlBase抽象基类公用控制逻辑，继承出三四种不同的实现，
    // 资源池根据类型激活或是失活脚本
// 这里不能把自定义类型的MinoData当作简单字段类型来集合类序列化,必须自定义序列化(可序列化的简单字段类型的 List<T>,这里不适用)
    // public List<MinoData> children; 

// // 补做7个mino预设
// public enum MinoType {
//     minoI = 0,
//     minoJ = 1,
//     minoL = 2,
//     minoO = 3,
//     minoS = 4,
//     minoT = 5,
//     minoZ = 6,
// }
// public enum TetrominoType { // 这里的导进来的,与预设里的只保留一套,否则有重复和冲突
//     tetro_I = 0 + 7,  // 常规各种类型: 这么列不足以列出所有的 + 7, 必段一个一个列出来一 + 7, 下同
//     tetro_J = 1 + 7,  
//     tetro_L = 2 + 7,  
//     tetro_O = 3 + 7,  
//     tetro_S = 4 + 7,  
//     tetro_T = 5 + 7,  
//     tetro_Z = 6 + 7,  

//     shado_I = 7 + 7,   // 阴影类型
//     shado_J = 8 + 7,  
//     shado_L = 9 + 7,  
//     shado_O = 10 + 7, 
//     shado_S = 11 + 7,  
//     shado_T = 12 + 7,  
//     shado_Z = 13 + 7,  

//     PARTIC = 14  // 粒子系统
// }

    // control logics base
    public abstract class ItemBase : IType {
        // string Type {
        //     set;
        //     get;
        // }
        public virtual string Type {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        //// 基类申明:类型到名字，预设存放地址的转换
        //public virtual getPrefabName() {
        //}

        //// 实例化方法的基类实现

        //// 脚本的类型设置，添加与激活失活等，根据类型来实现

        //public virtual resetTransform(Vector3 pos) {
            
        //}

        //public virtual resetRotation(Quaternion rot) {
            
        //}
    }
}
