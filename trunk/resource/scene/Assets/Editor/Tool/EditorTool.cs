using Framework.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// scene编辑器工具类
public class EditorTool : MonoBehaviour {
    static int furnitureInstanceID;

    // 获取场景Json数据
    [MenuItem("EditorTool/GetGameMenuSceneDataJson")]
    static void GetSpaceShowSceneDataJson() {
        // .....
        GameObject selectGameObject = Selection.activeGameObject;
        if (selectGameObject.name.StartsWith("GameMenu")) {
            SceneData sceneData = new SceneData();
            sceneData.instanceID = 999999999; // xyz

//　比较搞笑的是：主程序菜单基本没什么数据可写，要写也该是有不秒俄罗斯广场砖的场景或是视图来写呀。。。。。。。
            
// 这中间的逻辑省略掉了，可以写几个方法帮助自动将场景数据写进各个不同场景的相对应的文本里，不需要手动来写
        
            string sceneDataJson = sceneData.ObjectToJson().ToString();
            Debug.Log("SceneData: " + sceneDataJson);
            string url = Application.dataPath + "/HotFix/Scene/Config/GameMenuSceneData/" + selectGameObject.name + ".txt";
            Debug.Log("url: " + url);
            FileHelp.WriteString(url, sceneDataJson);
        } else {
            Debug.LogError("请选中spaceshow开头的GameObject");
        }
    }
}
