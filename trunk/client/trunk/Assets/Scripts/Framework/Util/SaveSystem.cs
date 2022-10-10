using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace tetris3d {

    [System.Serializable]
    public static class SaveSystem {
        private const string TAG = "SaveSystem";

        // private static string currentPath;
        // private static StringBuilder path = new StringBuilder("");
            
        // public static void SaveGame(Game game) { 
        //     Debug.Log(TAG + ": SaveGame()"); 
        //     BinaryFormatter formatter = new BinaryFormatter();
        //     path.Clear();
        //     if (GameMenuData.Instance.gameMode > 0) {
        //         path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game.save"); 
        //     } else {
        //         path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/grid" + GameMenuData.Instance.gridSize + "/game.save"); 
        //     }
        //     currentPath = path.ToString();
        //     if (File.Exists(currentPath)) {
        //         try {
        //             File.Delete(currentPath);
        //         } catch (System.Exception ex) {
        //             Debug.LogException(ex);
        //         }            
        //     }
        //     FileStream stream = new FileStream(currentPath, FileMode.Create);
        //     GameData data = new GameData(game);
        //     formatter.Serialize(stream, data);
        //     stream.Close();
        // }

        // public static GameData LoadGame(string pathIn) {
        //     Debug.Log(TAG + ": LoadGame()"); 
            
        //     if (File.Exists(pathIn)) {
        //         BinaryFormatter formatter = new BinaryFormatter();
        //         FileStream stream = new FileStream(pathIn, FileMode.Open, FileAccess.Read);
        //         GameData data = formatter.Deserialize(stream) as GameData;
        //         stream.Close();

        //         // I want to delete file here too
        //         try {
        //             File.Delete(pathIn);
        //         } catch (System.Exception ex) {
        //             Debug.LogException(ex);
        //         }            
                
        //         return data;
        //     } else {
        //         Debug.LogError("Save file not found in " + currentPath);
        //         return null;
        //     }
        // } 
    }
}