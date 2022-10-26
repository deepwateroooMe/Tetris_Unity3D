using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MVVM;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

// 定义应用模型: Transform.position rotation Vector3数据供UI游戏视图观察
    // currentActiveTransform
    // currentGhostTetromino
    // nextTetromino
    // previewTetromino
    // previewTetromino2
// Managers:
    // AudioManager
    // EventManager
// scoring system: all kinds of score, updates ==> ModelMono.cs
// save system:
// 我记得我以前有分过一个幼稚版本的Model.cs 和 ModelMono.cs,但还没有找到那个版本的源码    

    public class Model : MonoBehaviour {

	    void Update() {
            if (ViewManager.GameView.nextTetromino != null && ViewManager.GameView.nextTetromino.activeSelf) {
                Tetromino tetrominoScript = ComponentHelper.GetTetroComponent( ViewManager.GameView.nextTetromino);
                tetrominoScript.Update();
            }
	    }
		//public readonly static BindableProperty<Vector3> nextTetrominoPos = new BindableProperty<Vector3>();
        //public readonly static BindableProperty<Vector3> nextTetrominoRot = new BindableProperty<Vector3>();
        //public readonly static BindableProperty<Vector3> nextTetrominoSca = new BindableProperty<Vector3>();
        
    }
}
