using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Framework.MVVM;
using UnityEngine;

namespace deepwaterooo.tetris3d {

    // 在windows上,是说内存不足时的存储地址是有可能会变化的?
    // C:/Users/blue_/AppData/LocalLow/DefaultCompany/tetris3D/challenge/game.save 需要去搜索一下这套逻辑

// TODO: 里面的文件地址路径等相关不必要逻辑放入帮助类里去    
    public static class SaveSystem { 
        private const string TAG = "SaveSystem"; 
        
        public static void SaveGame(string currentPath, GameData gameData) { 
            Debug.Log(TAG + "SaveGame() currentPath: " + currentPath);
            BinaryFormatter formatter = new BinaryFormatter();
            if (File.Exists(currentPath)) {
                try {
                    File.Delete(currentPath);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }            
            }
            FileStream stream = new FileStream(currentPath, FileMode.Create);
            formatter.Serialize(stream, gameData);
            stream.Close();
        }

        public static GameData LoadGame(string pathIn) {
            Debug.Log(TAG + " LoadGame() pathIn: " + pathIn);
            Debug.Log(TAG + " File.Exists(pathIn): " + File.Exists(pathIn));
            if (File.Exists(pathIn)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(pathIn, FileMode.Open, FileAccess.Read);
                GameData data = formatter.Deserialize(stream) as GameData;
                stream.Close();

                try {
                    File.Delete(pathIn);
                } catch (System.Exception ex) {
                    Debug.LogException(ex);
                }            
                
                return data;
            } else {
                Debug.LogError("LoadGame() Save file not found in pathIn: " + pathIn);
                return null;
            }
        } 
    }
}

