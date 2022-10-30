using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using deepwaterooo.tetris3d.Events;
using UnityEngine;

namespace HotFix.Control {

	public class ComponentHelper : MonoBehaviour {

		public static void AddTetroComponent(GameObject go) {
			go.AddComponent<Tetromino>();
		}
		public static void AddGhostComponent(GameObject go) {
			go.AddComponent<GhostTetromino>();
		}
		public static void AddMoveBtnListenerComponent(GameObject go) {
			go.AddComponent<MoveButtonListener>();
		}
		public static void AddRotateBtnListenerComponent(GameObject go) {
			go.AddComponent<RotateButtonListener>();
		}

		public static Tetromino GetTetroComponent(GameObject go) {
            return go.GetComponent<Tetromino>();
		}
		public static GhostTetromino GetGhostComponent(GameObject go) {
            return go.GetComponent<GhostTetromino>();
		}
		public static MoveButtonListener GetMoveBtnListenerComponent(GameObject go) {
			return go.GetComponent<MoveButtonListener>();
		}
		public static RotateButtonListener GetRotateBtnListenerComponent(GameObject go) {
			return go.GetComponent<RotateButtonListener>();
		}
	}
}