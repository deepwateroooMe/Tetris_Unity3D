using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.Data.Data {

    // [System.Serializable]
    public class TetrominoData {
        private const string TAG = "TetrominoData";

        public string name { get; set; }
        public string type { get; set; }
        
        public SerializedTransform transform { get; set; }  
        public MinoDataCollection<TetrominoData, MinoData> children { get; private set; } 

        public TetrominoData(Transform parentTrans, string type, string name) {
            this.name = name;
            this.type = type;
            transform = new SerializedTransform(parentTrans);
            children = new MinoDataCollection<TetrominoData, MinoData>(this);
            foreach (Transform mino in parentTrans) {
                if (mino.CompareTag("mino")) { 
                    MinoData minoDataItem = new MinoData(mino, new StringBuilder("mino" + type.Substring(5, 1)).ToString()); // shapeX ==> minoX
                    children.Add(minoDataItem);
                }
            }
        }
        
        public TetrominoData(Transform parentTrans) {
            transform = new SerializedTransform(parentTrans);
            children = new MinoDataCollection<TetrominoData, MinoData>(this);
            foreach (Transform mino in parentTrans) {
                if (mino.CompareTag("mino")) {
                    MinoData minoDataItem = new MinoData(mino);
                    children.Add(minoDataItem);
                }
            }
        }
    
        public void print() {
            Debug.Log(TAG + ": Parent TetrominoData: "); 
            this.transform.print();
            foreach (var minoData in children) {
                Debug.Log(TAG + " minoData.idx: " + minoData.idx); 
                minoData.print();
            }
        }
    }
}
