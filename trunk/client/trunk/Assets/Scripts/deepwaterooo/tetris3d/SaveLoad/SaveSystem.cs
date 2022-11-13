using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Framework.MVVM;
using UnityEngine;

namespace deepwaterooo.tetris3d {

// TODO: 里面的文件地址路径等相关不必要逻辑放入帮助类里去    
    public static class SaveSystem { 
        private const string TAG = "SaveSystem"; 
        
        public static void SaveGame(string currentPath, GameData gameData) { 
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