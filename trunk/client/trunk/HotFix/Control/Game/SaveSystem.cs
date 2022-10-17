using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using HotFix.Data.Data;
using UnityEngine;

namespace HotFix.Control.Game {

// 游戏的存储系统:我认为是属于游戏逻辑,应该放在Game control里

// 跟以前非热更新的游戏很像,都是将当前游戏模式和游戏进展画面存到客户端的某个文件里,加载的话就是从这个文件读取
// 还是需要从BinaryFormater转换为Json的,因为彻头彻尾地改变了立方体和方块砖的序列化与反序列化方式,所以没有了支撑BinaryFormater来存储的基础数据结构了
// 文件的存储地址稍作修改,也搜一下网络上更为优化的游戏进展存储系统
    // 在windows上,是说内存不足时的存储地址是有可能会变化的?
    // C:/Users/blue_/AppData/LocalLow/DefaultCompany/tetris3D/challenge/game.save 需要去搜索一下这套逻辑
    
    public static class SaveSystem { 
        private const string TAG = "SaveSystem";

        private static string currentPath;
        private static StringBuilder path = new StringBuilder("");
            
        public static void SaveGame(Game game) { 
            Debug.Log(TAG + ": SaveGame()"); 
            BinaryFormatter formatter = new BinaryFormatter();
            path.Length = 0; // .NET 4.0 封装到了Clear()方法里去
            if (GameMenuData.Instance.gameMode > 0) {
                path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/game.save"); 
            } else {
                path.Append(Application.persistentDataPath + "/" + GameMenuData.Instance.saveGamePathFolderName + "/grid" + GameMenuData.Instance.gridSize + "/game.save"); 
            }
            currentPath = path.ToString();
            if (File.Exists(currentPath)) {
                try {
                    File.Delete(currentPath);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }            
            }
            FileStream stream = new FileStream(currentPath, FileMode.Create);
            GameData data = new GameData(game);
            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static GameData LoadGame(string pathIn) {
            Debug.Log(TAG + ": LoadGame()"); 
            
            if (File.Exists(pathIn)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(pathIn, FileMode.Open, FileAccess.Read);
                GameData data = formatter.Deserialize(stream) as GameData;
                stream.Close();

                // I want to delete file here too
                try {
                    File.Delete(pathIn);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }            
                
                return data;
            } else {
                Debug.LogError("Save file not found in " + currentPath);
                return null;
            }
        } 
    }
}
