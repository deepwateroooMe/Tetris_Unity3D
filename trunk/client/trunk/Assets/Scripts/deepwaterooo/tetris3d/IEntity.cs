using UnityEngine;

namespace deepwaterooo.tetris3d {

    public interface IEntity { // Tetromino : IEntity
        // InputReader: EventManager                            
        // GameObject gameObject;

// 这里想要把它细化得更小,放平移旋转与缩放,方便功能解藉       
        // Transform transform { get; set; }
        Vector3 pos { get; set; }
        Vector3 rot { get; set; }
        Vector3 sca { get; set; }
        
        void MoveDelta(Vector3 delta);
        void RotateDelta(Vector3 delta);
    }
}
