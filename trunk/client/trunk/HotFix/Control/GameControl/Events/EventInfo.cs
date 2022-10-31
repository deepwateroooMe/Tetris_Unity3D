using UnityEngine;

namespace HotFix.Control {

    public abstract class EventInfo { } // TODO: search for, stucts of individual classes ? inheritance struct ??? 

// Buttons group: Move, Rotate, Generic
    public class MoveButtonClickEventInfo : EventInfo {   // 4
        public GameObject unitGO;
    }
    public class RotateButtonClickEventInfo : EventInfo { // 6
        public GameObject unitGO;
    }
    public class GenericButtonClickEventInfo : EventInfo { // GUI buttons
        public GameObject unitGO;
    }
// Tetromino EventInfo: Spawned, Move, Rotate, Land    
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
// Game Control: Started, Pause, Resume, Stop etc
    public class StartedGameEventInfo : EventInfo {       // Started Game
    }
    public class PauseGameEventInfo : EventInfo {         // Pause Game
    }
    
    public class SwapPreviewsEventInfo : EventInfo {      // swapPreviewTetrominoButton
    }
    public class SaveGameEventInfo : EventInfo {          // Save Game
    }
    public class UndoGameEventInfo : EventInfo {          // Undo Game
    }
    public class ToggleActionEventInfo : EventInfo {      // toggleButtons
    }
    public class FallFastEventInfo : EventInfo {          // fallFast
    }
// 画布: 两个画布的切换等
    public class CanvasToggledEventInfo : EventInfo {       // moveCanvas rotateCanvas
    }
    
    public class LevelButtonClickEventInfo : EventInfo {
        public GameObject unitGO;
    }
// 没想明白当初为什么需要这个 ?    
    // public class PropertyChangedEventInfo : EventInfo {
    //     public string propertyName;
    // }
}
