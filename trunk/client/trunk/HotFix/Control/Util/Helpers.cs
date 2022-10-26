using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {
    
	public class Helpers {
		public static void resetPos(GameObject gameObject, Vector3 pos) {
			gameObject.transform.position = pos;
		}
		public static void resetRot(GameObject gameObject, Quaternion rot) {
			gameObject.transform.rotation = rot;
		}
		public static void resetSca(GameObject gameObject, Vector3 sca) {
			gameObject.transform.localScale = sca;
		}

		public static void resetTrans(GameObject gameObject, Transform transform) {
			gameObject.transform.position = transform.position;
			gameObject.transform.rotation = transform.rotation;
			gameObject.transform.localScale = transform.localScale;
		}
	}
}
