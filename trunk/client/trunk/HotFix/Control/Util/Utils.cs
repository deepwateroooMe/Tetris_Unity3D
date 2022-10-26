using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {
	public class Utils {
        private const string TAG = "Utils"; 

        public static void print(Vector3 var) {
            Debug.Log(TAG + "[" + var.x + ", " + var.y + ", " + var.z + "]");
        }
	}
}

