using UnityEngine;

namespace tetris3d {

    public abstract class EventInfo { } // TODO: search for, stucts of individual classes ? inheritance struct ??? 
    // public class DebugEventInfo : EventInfo {
    //     public int verbosityLevel;
    // }
    
    public class MoveButtonClickEventInfo : EventInfo {   // 4
        public GameObject unitGO;
    }
    public class RotateButtonClickEventInfo : EventInfo { // 6
        public GameObject unitGO;
    }
    public class GenericButtonClickEventInfo : EventInfo {// GUI buttons
        public GameObject unitGO;
    }
    
    public class TetrominoSpawnedEventInfo : EventInfo {  // Spawned
        public Vector3 delta; 
    }
    public class TetrominoMoveEventInfo : EventInfo {     // move 
        public Vector3 delta;
    }
    public class TetrominoRotateEventInfo : EventInfo {   // rotate
        public Vector3 delta;
    }
    public class TetrominoLandEventInfo : EventInfo {     // land
    }
    
    public class SwapPreviewsEventInfo : EventInfo {      // swapPreviewTetrominoButton
    }
    public class SaveGameEventInfo : EventInfo {          // Save Game
    }
    public class UndoGameEventInfo : EventInfo {          // Undo Game
    }
    public class PauseGameEventInfo : EventInfo {         // Pause Game
    }
    public class ToggleActionEventInfo : EventInfo {      // toggleButtons
    }
    public class FallFastEventInfo : EventInfo {          // fallFast
    }
    
    public class CanvasMovedEventInfo : EventInfo {       // moveCanvas rotateCanvas
        public Vector3 delta;
    }

    public class LevelButtonClickEventInfo : EventInfo {
        public GameObject unitGO;
    }

    public class PropertyChangedEventInfo : EventInfo {
        public string propertyName;
    }
}
