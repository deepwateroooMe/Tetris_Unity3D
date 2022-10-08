using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris3d {
public static class TransformExtension {
    
    public static void SetTransformEx(this Transform original, Transform copy) {
        original.position = copy.position;
        original.rotation = copy.rotation;
        original.localScale = copy.localScale;
    }
}
    
}
