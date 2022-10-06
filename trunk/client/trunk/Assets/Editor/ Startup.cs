using UnityEditor;

[InitializeOnLoad]
public class Startup {

    static Startup() {
        EditorApplication.update += Update;
    }

    static int status = 0;
    static void Update() {
        if (CheckDllChange.dllChanged) {
            if (EditorApplication.isPlaying) 
                EditorApplication.isPlaying = false;
            status++;
            if (status > 0) {
                CheckDllChange.BuildHotFixDllBundle();
                status = 0;
                CheckDllChange.dllChanged = false;
            }
        }
    }
}
