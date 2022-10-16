using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control.Game {

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
