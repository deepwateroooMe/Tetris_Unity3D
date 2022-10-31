using UnityEngine;

namespace HotFix.Control {

	public class ComponentHelper : MonoBehaviour {

// Tetromino GhostTetromino        
		public static void AddTetroComponent(GameObject go) {
			go.AddComponent<Tetromino>();
		}
		public static void AddGhostComponent(GameObject go) {
			go.AddComponent<GhostTetromino>();
		}
// MoveCanvas RotateCanvas        
		public static void AddMoveCanvasComponent(GameObject go) {
			go.AddComponent<MoveCanvas>();
		}
		public static void AddRotateCanvasComponent(GameObject go) {
			go.AddComponent<RotateCanvas>();
		}
// // MoveButtonEventListener RotateButtonClickEventInfo
//         public static void AddMoveBtnsListener(GameObject go) {
//             go.AddComponent<MoveButtonEventListener>();
//         }
//         public static void AddRotateBtnsListener(GameObject go) {
//             go.AddComponent<RotateButtonEventListener>();
//         }
        
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
// // MoveButtonEventListener RotateButtonClickEventInfo
//         public static MoveButtonEventListener GetMoveBtnsListener(GameObject go) {
//             return go.GetComponent<MoveButtonEventListener>();
//         }
//         public static RotateButtonEventListener GetRotateBtnsListener(GameObject go) {
//             return go.GetComponent<RotateButtonEventListener>();
//         }
	}
}