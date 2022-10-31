using UnityEngine;

namespace HotFix.Control {

    public abstract class EventInfo {
        public const string TAG = "EventInfo"; 
    } // TODO: search for, stucts of individual classes ? inheritance struct ??? 

// Buttons group: Move, Rotate, Generic
    public class MoveButtonClickEventInfo : EventInfo {   // 4
        private const string TAG = "MoveButtonClickEventInfo"; 
        public GameObject unitGO;
    }
    public class RotateButtonClickEventInfo : EventInfo { // 6
        private const string TAG = "RotateButtonClickEventInfo"; 
        public GameObject unitGO;
    }
    public class GenericButtonClickEventInfo : EventInfo { // GUI buttons
        private const string TAG = "GenericButtonClickEventInfo";
        public GameObject unitGO;
    }
// Tetromino EventInfo: Spawned, Move, Rotate, Land    
    public class TetrominoSpawnedEventInfo : EventInfo {  // Spawned
        private const string TAG = "TetrominoSpawnedEventInfo";
        public Vector3 delta; 
    }
    public class TetrominoMoveEventInfo : EventInfo {     // move 
        private const string TAG = "TetrominoMoveEventInfo"; 
        public Vector3 delta;
    }
    public class TetrominoRotateEventInfo : EventInfo {   // rotate
        private const string TAG = "TetrominoRotateEventInfo"; 
        public Vector3 delta;
    }
    public class TetrominoLandEventInfo : EventInfo {     // land
        private const string TAG = "TetrominoLandEventInfo";
    }
// Game Control: Started, Pause, Resume, Stop etc
    public class GameEnterEventInfo : EventInfo {         // Entered Game View
        private const string TAG = "GameEnterEventInfo";
    }
    public class StartedGameEventInfo : EventInfo {       // Started Game
        private const string TAG = "StartedGameEventInfo";
    }
    public class PauseGameEventInfo : EventInfo {         // Pause Game
        private const string TAG = "PauseGameEventInfo";
    }
    
    public class SwapPreviewsEventInfo : EventInfo {      // swapPreviewTetrominoButton
        private const string TAG = "SwapPreviewsEventInfo";
    }
    public class SaveGameEventInfo : EventInfo {          // Save Game
        private const string TAG = "SaveGameEventInfo";
    }
    public class UndoGameEventInfo : EventInfo {          // Undo Game
        private const string TAG = "UndoGameEventInfo";
    }
    public class ToggleActionEventInfo : EventInfo {      // toggleButtons
        private const string TAG = "ToggleActionEventInfo";
    }
    public class FallFastEventInfo : EventInfo {          // fallFast
        private const string TAG = "FallFastEventInfo";
    }
// 画布: 两个画布的切换等
    public class CanvasToggledEventInfo : EventInfo {       // moveCanvas rotateCanvas
        private const string TAG = "CanvasToggledEventInfo";
    }
    
    public class LevelButtonClickEventInfo : EventInfo {
        private const string TAG = "LevelButtonClickEventInfo";
        public GameObject unitGO;
    }
// 没想明白当初为什么需要这个 ?    
    // public class PropertyChangedEventInfo : EventInfo {
    //     public string propertyName;
    // }
}
