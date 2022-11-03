using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

	public class GameObjectHelper {
        private const string TAG = "GameObjectHelper"; 

        public static string GetGhostTetrominoType(GameObject gameObject) { // ghostTetromino
            Debug.Log(TAG + ": GetGhostTetrominoType()"); 
            StringBuilder type = new StringBuilder("");
            Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
            string tmp = gameObject.name.Substring(9, 1);
            switch(tmp) {
                case "T" : type.Append("shadowT"); break;
                case "I" : type.Append("shadowI"); break;
                case "J" : type.Append("shadowJ"); break;
                case "L" : type.Append("shadowL"); break;
                case "O" : type.Append("shadowO"); break;
                case "S" : type.Append("shadowS"); break;
                case "Z" : type.Append("shadowZ"); break;
            }
            return type.ToString(); 
        }
	}
}
