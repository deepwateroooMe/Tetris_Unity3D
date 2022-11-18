using UnityEngine;

namespace HotFix.Control {

	public class ComponentHelper : MonoBehaviour {

// basePlane: BaseBoardSkin.cs
		public static BaseBoardSkin AddBBSkinComponent(GameObject go) {
			return go.AddComponent<BaseBoardSkin>();
		}
// Tetromino GhostTetromino        
		public static Tetromino AddTetroComponent(GameObject go) {
			return go.AddComponent<Tetromino>();
		}
		public static GhostTetromino AddGhostComponent(GameObject go) {
			return go.AddComponent<GhostTetromino>();
		}
// MoveCanvas RotateCanvas        
		public static void AddMoveCanvasComponent(GameObject go) {
			go.AddComponent<MoveCanvas>();
		}
		public static void AddRotateCanvasComponent(GameObject go) {
			go.AddComponent<RotateCanvas>();
		}
        
// basePlane: BaseBoardSkin.cs
		public static BaseBoardSkin GetBBSkinComponent(GameObject go) {
			return go.GetComponent<BaseBoardSkin>();
		}
// Tetromino GhostTetromino        
		public static Tetromino GetTetroComponent(GameObject go) {
            return go.GetComponent<Tetromino>();
		}
		public static GhostTetromino GetGhostComponent(GameObject go) {
            return go.GetComponent<GhostTetromino>();
		}
// MoveCanvas RotateCanvas        
		public static MoveCanvas GetMoveCanvasComponent(GameObject go) {
			return go.GetComponent<MoveCanvas>();
		}
		public static RotateCanvas GetRotateCanvasComponent(GameObject go) {
			return go.GetComponent<RotateCanvas>();
		}
	}
}