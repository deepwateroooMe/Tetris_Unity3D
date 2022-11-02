using UnityEngine;

namespace HotFix.Control {

    public abstract class EventInfo {
        public const string TAG = "EventInfo";
        
    } // TODO: search for, stucts of individual classes ? inheritance struct ??? 

// Buttons group: Move, Rotate, Generic
    public class MoveButtonClickEventInfo : EventInfo {   // 4
        public const string TAG = "MoveButtonClickEventInfo"; 
        public GameObject unitGO;
    }
    public class RotateButtonClickEventInfo : EventInfo { // 6
        public const string TAG = "RotateButtonClickEventInfo"; 
        public GameObject unitGO;
    }
    public class GenericButtonClickEventInfo : EventInfo { // GUI buttons
        public const string TAG = "GenericButtonClickEventInfo";
        public GameObject unitGO;
    }

// Tetromino EventInfo: Spawned, Move, Rotate, Land    
    public class TetrominoSpawnedEventInfo : EventInfo {  // Spawned
        public const string TAG = "TetrominoSpawnedEventInfo";
        public Vector3 delta; 
    }
    public class TetrominoMoveEventInfo : EventInfo {     // move 
        public const string TAG = "TetrominoMoveEventInfo"; 
        public Vector3 delta;
    }
    public class TetrominoRotateEventInfo : EventInfo {   // rotate
        public const string TAG = "TetrominoRotateEventInfo"; 
        public Vector3 delta;
    }
    public class TetrominoValidMMInfo : EventInfo { // valid Move Rotate
        private const string TAG = "TetrominoValidMMInfo";
        public string type;
    }
    public class TetrominoLandEventInfo : EventInfo {     // land
        public const string TAG = "TetrominoLandEventInfo";
    }

// Game Control: Enter, Start, Pause, Resume, Stop etc
    public class GameEnterEventInfo : EventInfo {         // Entered Game View
        public const string TAG = "GameEnterEventInfo";
    }
    public class GameStartEventInfo : EventInfo {         // Start Game
        public const string TAG = "GameStartEventInfo";
    }
    public class GamePauseEventInfo : EventInfo {         // Pause Game
        public const string TAG = "GamePauseEventInfo";
    }
    public class GameResumeEventInfo : EventInfo {         // Resume Game
        public const string TAG = "GameResumeEventInfo";
    }
    public class GameStopEventInfo : EventInfo {           // Stop Game
        public const string TAG = "GameStopEventInfo";
    }
// Game main menu events:     
    public class SwapPreviewsEventInfo : EventInfo {      // swapPreviewTetrominoButton
        public const string TAG = "SwapPreviewsEventInfo";
    }
    public class SaveGameEventInfo : EventInfo {          // Save Game
        public const string TAG = "SaveGameEventInfo";
    }
    public class UndoGameEventInfo : EventInfo {          // Undo Game
        public const string TAG = "UndoGameEventInfo";
    }
    // public class FallFastEventInfo : EventInfo {          // fallFast
    //     public const string TAG = "FallFastEventInfo";
    // }
// 画布: 两个画布的切换等
    public class CanvasToggledEventInfo : EventInfo {       // moveCanvas rotateCanvas
        public const string TAG = "CanvasToggledEventInfo";
    }
    
    public class LevelButtonClickEventInfo : EventInfo {
        public const string TAG = "LevelButtonClickEventInfo";
        public GameObject unitGO;
    }
// 没想明白当初为什么需要这个 ?    
    // public class PropertyChangedEventInfo : EventInfo {
    //     public string propertyName;
    // }
}
