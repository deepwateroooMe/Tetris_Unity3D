using UnityEngine;

namespace tetris3d {

    public abstract class EventInfo { }
    // public class DebugEventInfo : EventInfo {
    //     public int verbosityLevel;
    // }
    
    public class MoveButtonClickEventInfo : EventInfo { // 4
        public GameObject unitGO;
    }
    public class RotateButtonClickEventInfo : EventInfo { // 6
        public GameObject unitGO;
    }
    public class GenericButtonClickEventInfo : EventInfo {
        public GameObject unitGO;
    }
    
    public class TetrominoLandEventInfo : EventInfo { // land
        public GameObject unitGO;
    }
    public class TetrominoMoveEventInfo : EventInfo { // move 
        public GameObject unitGO;
        public Vector3 delta;
    }
    public class TetrominoRotateEventInfo : EventInfo { // rotate
        public GameObject unitGO;
        public Vector3 delta;
    }
    
    public class SaveGameEventInfo : EventInfo { // Save Game
    }                                                
    public class SwapPreviewsEventInfo : EventInfo { // Load Game //  : GenericButtonClickEventInfo
        public string tag;
    }
    public class UndoGameEventInfo : EventInfo { // Undo Game: Reload game
        // public GameObject unitGO;
        public string tag;
    }
}
