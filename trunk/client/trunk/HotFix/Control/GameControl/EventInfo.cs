using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public abstract class EventInfo { } // 这个事件传递系统,感觉还不够熟,理解得不透彻,可能还需要相对比较大的修改
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


    public class TetrominoSpawnedEventInfo : EventInfo {  // Spawned
        public Vector3 delta; 
    }
    public class TetrominoMoveEventInfo : EventInfo {     // move 
        public GameObject unitGO; // 这里前后有两上不同的版本,重构的原因不是很明白,要仔细看一下
        public Vector3 delta;
    }
    public class TetrominoRotateEventInfo : EventInfo {   // rotate
        public GameObject unitGO; // 这里前后有两上不同的版本,重构的原因不是很明白,要仔细看一下
        public Vector3 delta;
    }
    public class TetrominoLandEventInfo : EventInfo {     // land
        public GameObject unitGO; // 这里前后有两上不同的版本,重构的原因不是很明白,要仔细看一下
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
