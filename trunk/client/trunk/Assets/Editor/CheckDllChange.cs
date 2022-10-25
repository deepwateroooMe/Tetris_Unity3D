using UnityEditor;
using UnityEngine;

public class CheckDllChange : AssetPostprocessor {
    public static bool dllChanged = false;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                       string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var str in importedAssets) {
            Debug.Log("Re imported Asset: " + str);
            if (str.EndsWith("dll.bytes")) {
                dllChanged = true;
            }
        }
    }
    public static void BuildHotFixDllBundle() {
        AssetBundleTool.BuildHotFixDllBundle();
        dllChanged = false;
    }
}







