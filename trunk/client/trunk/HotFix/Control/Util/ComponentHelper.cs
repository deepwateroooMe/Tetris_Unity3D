using UnityEngine;

namespace HotFix.Control {

	public class ComponentHelper : MonoBehaviour {

		public static void AddTetroComponent(GameObject go) {
			go.AddComponent<Tetromino>();
		}
		public static void AddGhostComponent(GameObject go) {
			go.AddComponent<GhostTetromino>();
		}
		public static void AddMoveCanvasComponent(GameObject go) {
			go.AddComponent<MoveCanvas>();
		}
		public static void AddRotateCanvasComponent(GameObject go) {
			go.AddComponent<RotateCanvas>();
		}

		public static Tetromino GetTetroComponent(GameObject go) {
            return go.GetComponent<Tetromino>();
		}
		public static GhostTetromino GetGhostComponent(GameObject go) {
            return go.GetComponent<GhostTetromino>();
		}
		public static MoveCanvas GetMoveCanvasComponent(GameObject go) {
			return go.GetComponent<MoveCanvas>();
		}
		public static RotateCanvas GetRotateCanvasComponent(GameObject go) {
			return go.GetComponent<RotateCanvas>();
		}
	}
}