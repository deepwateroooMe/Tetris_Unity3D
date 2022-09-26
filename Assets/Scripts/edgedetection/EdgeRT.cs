using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EdgeRT : MonoBehaviour { // EdgeRT: Edge Render Target {

    [HideInInspector]
    [SerializeField]
    private Camera camera; 
    private int currentWidth;
    private int currentHeight;
    // private string globalTextureName = "GlobalEdgeTex";
    
    void SetupRT() {
        // let's only render Depth and Normals...
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.DepthNormals; // only render Depth & Normals (the only inputs needed for detecting edges)
        if (camera.targetTexture != null) {
            RenderTexture temp = camera.targetTexture;
            camera.targetTexture = null;
            DestroyImmediate(temp);
        }
        // ... to a RenderTexture: against which the edge camera will render
        camera.targetTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 16);
        camera.targetTexture.filterMode = FilterMode.Bilinear;
        // we don't acturally need this
        //Shader.SetGlobalTexture(globalTextureName, camera.targetTexture);
    }

    void Update() { // checks in window size so that to adjust RenderTexture's width and height
        if (currentHeight != Screen.currentResolution.height || currentWidth != Screen.currentResolution.width) {
            currentHeight = Screen.height;
            currentWidth = Screen.width;
            SetupRT();
        }
    }
}
