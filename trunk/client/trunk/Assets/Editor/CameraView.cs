using UnityEngine;
using System.Collections;
using UnityEditor;
 
public class CameraView : EditorWindow {

    Camera myCamera = Camera.main;  // 获取摄像机
    RenderTexture renderTexture;
 
    [MenuItem("Assets/Camera Viewer")]  // 在菜单栏创建按钮
    static void Init() {
        EditorWindow editorWindows = GetWindow(typeof(CameraView));  // 创建新窗口
        editorWindows.autoRepaintOnSceneChange = true;
        editorWindows.Show();
    }
 
    public void Awake() {  // 当跳出窗口时首先调用该方法 {
        renderTexture = new RenderTexture((int)position.width,
                                          (int)position.height, 1);  // 获取renderTexture
    }
 
    public void Update() { // 跳出窗口后每帧调用该方法 {
        if (myCamera != null) {
            myCamera.targetTexture = renderTexture;
        }
    }
 
    void OnGUI() {
        if (renderTexture != null) {
            GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), renderTexture, ScaleMode.ScaleAndCrop, true);
        }
    }
}