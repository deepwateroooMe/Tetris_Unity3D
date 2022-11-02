using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Framework.MVVM;
using HotFix.Data;
using HotFix.UI;
using UnityEngine;

namespace HotFix.Control {

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
            
        // public static void SaveGame(Game game) { // 以前写成了相当于是全局静态数据,现在传ViewModelBase就可以了
            public static void SaveGame(ViewModelBase viewModel) { 
            Debug.Log(TAG + ": SaveGame()");
            MenuViewModel parentViewModel = (MenuViewModel)viewModel.ParentViewModel;
            int gameMode = parentViewModel.gameMode;
            BinaryFormatter formatter = new BinaryFormatter();
            path.Length = 0; // .NET 4.0 封装到了Clear()方法里去
            if (gameMode > 0) {
                path.Append(Application.persistentDataPath + "/" + parentViewModel.saveGamePathFolderName + "/game.save"); 
            } else {
                path.Append(Application.persistentDataPath + "/" + parentViewModel.saveGamePathFolderName
                            + "grid" + parentViewModel.gridWidth + "/game.save"); 
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
            // following commented for tmp
            // GameData data = new GameData(game); // 如果有了视图模型以及更幕后的游戏应用模型,不是不应该再保存视图层面的这些了吗,想想
            //formatter.Serialize(stream, data);
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
                Debug.Log(TAG + " currentPath: " + currentPath);
                Debug.LogError("Save file not found in " + currentPath);
                return null;
            }
        } 
    }
}
