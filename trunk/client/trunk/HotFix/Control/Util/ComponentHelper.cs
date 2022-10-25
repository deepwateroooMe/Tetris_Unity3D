﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

	public class ComponentHelper : MonoBehaviour {

		public static void AddTetroComponent(GameObject go) {
			go.AddComponent<Tetromino>();
		}
		public static void AddGhostComponent(GameObject go) {
			go.AddComponent<GhostTetromino>();
		}

		public static Tetromino GetTetroComponent(GameObject go) {
            return go.GetComponent<Tetromino>();
		}
		public static GhostTetromino GetGhostComponent(GameObject go) {
            return go.GetComponent<GhostTetromino>();
		}
	}
}


