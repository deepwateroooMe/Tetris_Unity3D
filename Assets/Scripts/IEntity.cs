using UnityEngine;

namespace tetris3d {
    
    public interface IEntity { // Tetromino : IEntity
        // InputReader: EventManager                            

        Transform transform { get; }
        // GameObject gameObject;

        void MoveDelta(Vector3 delta);
        void RotateDelta(Vector3 delta);
    }
}